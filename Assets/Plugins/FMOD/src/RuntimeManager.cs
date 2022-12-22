#if UNITY_ADDRESSABLES_EXIST
using UnityEngine.AddressableAssets;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AOT;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using ADVANCEDSETTINGS = FMOD.ADVANCEDSETTINGS;
using CPU_USAGE = FMOD.Studio.CPU_USAGE;
using Debug = FMOD.Debug;
using GUID = FMOD.GUID;
using INITFLAGS = FMOD.Studio.INITFLAGS;
using SYSTEM_CALLBACK = FMOD.SYSTEM_CALLBACK;
using SYSTEM_CALLBACK_TYPE = FMOD.SYSTEM_CALLBACK_TYPE;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FMODUnity
{
    [AddComponentMenu("")]
    public class RuntimeManager : MonoBehaviour
    {
        public const string BankStubPrefix = "bank stub:";

        private static SystemNotInitializedException initException;
        private static RuntimeManager instance;

        private Platform currentPlatform;
        private DEBUG_CALLBACK debugCallback;
        private SYSTEM_CALLBACK errorCallback;

        private FMOD.Studio.System studioSystem;
        private FMOD.System coreSystem;
        private DSP mixerHead;

        private bool isMuted;

        private readonly Dictionary<GUID, EventDescription> cachedDescriptions = new(new GuidComparer());

        private readonly Dictionary<string, LoadedBank> loadedBanks = new();
        private readonly List<string> sampleLoadRequests = new();

        private readonly List<AttachedInstance> attachedInstances = new(128);

#if UNITY_EDITOR
        private readonly List<EventInstance> eventPositionWarnings = new();
#endif

        private bool listenerWarningIssued;

        protected bool isOverlayEnabled;
        private FMODRuntimeManagerOnGUIHelper overlayDrawer;
        private Rect windowRect = new(10, 10, 300, 100);

        private string lastDebugText;
        private float lastDebugUpdate;

        private int loadingBanksRef;

        public static bool IsMuted => Instance.isMuted;

        [MonoPInvokeCallback(typeof(DEBUG_CALLBACK))]
        private static RESULT DEBUG_CALLBACK(DEBUG_FLAGS flags, IntPtr filePtr, int line, IntPtr funcPtr,
            IntPtr messagePtr)
        {
            var file = new StringWrapper(filePtr);
            var func = new StringWrapper(funcPtr);
            var message = new StringWrapper(messagePtr);

            if (flags == DEBUG_FLAGS.ERROR)
                RuntimeUtils.DebugLogError(string.Format("[FMOD] {0} : {1}", (string)func, (string)message));
            else if (flags == DEBUG_FLAGS.WARNING)
                RuntimeUtils.DebugLogWarning(string.Format("[FMOD] {0} : {1}", (string)func, (string)message));
            else if (flags == DEBUG_FLAGS.LOG)
                RuntimeUtils.DebugLog(string.Format("[FMOD] {0} : {1}", (string)func, (string)message));
            return RESULT.OK;
        }

        [MonoPInvokeCallback(typeof(SYSTEM_CALLBACK))]
        private static RESULT ERROR_CALLBACK(IntPtr system, SYSTEM_CALLBACK_TYPE type, IntPtr commanddata1,
            IntPtr commanddata2, IntPtr userdata)
        {
            var callbackInfo =
                (ERRORCALLBACK_INFO)MarshalHelper.PtrToStructure(commanddata1, typeof(ERRORCALLBACK_INFO));

            // Filter out benign expected errors.
            if ((callbackInfo.instancetype == ERRORCALLBACK_INSTANCETYPE.CHANNEL ||
                 callbackInfo.instancetype == ERRORCALLBACK_INSTANCETYPE.CHANNELCONTROL) &&
                callbackInfo.result == RESULT.ERR_INVALID_HANDLE) return RESULT.OK;

            RuntimeUtils.DebugLogError(string.Format("[FMOD] {0}({1}) returned {2} for {3} (0x{4}).",
                (string)callbackInfo.functionname, (string)callbackInfo.functionparams, callbackInfo.result,
                callbackInfo.instancetype, callbackInfo.instance.ToString("X")));
            return RESULT.OK;
        }

        private static RuntimeManager Instance
        {
            get
            {
                if (initException != null) throw initException;

                if (instance == null)
                {
                    var initResult =
                        RESULT.OK; // Initialize can return an error code if it falls back to NO_SOUND, throw it as a non-cached exception

                    // When reloading scripts the static instance pointer will be cleared, find the old manager and clean it up
                    foreach (var manager in Resources.FindObjectsOfTypeAll<RuntimeManager>())
                        DestroyImmediate(manager.gameObject);

                    var gameObject = new GameObject("FMOD.UnityIntegration.RuntimeManager");
                    instance = gameObject.AddComponent<RuntimeManager>();

                    if (Application.isPlaying) // This class is used in edit mode by the Timeline auditioning system
                        DontDestroyOnLoad(gameObject);
                    gameObject.hideFlags = HideFlags.HideAndDontSave;

                    try
                    {
                        RuntimeUtils.EnforceLibraryOrder();

#if UNITY_ANDROID && !UNITY_EDITOR
                        // First, obtain the current activity context
                        AndroidJavaObject activity = null;
                        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        {
                            activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                        }

                        using (var fmodJava = new AndroidJavaClass("org.fmod.FMOD"))
                        {
                            if (fmodJava != null)
                            {
                                fmodJava.CallStatic("init", activity);
                            }
                            else
                            {
                                RuntimeUtils.DebugLogWarning("[FMOD] Cannot initialize Java wrapper");
                            }
                        }
#endif

                        initResult = instance.Initialize();
                    }
                    catch (Exception e)
                    {
                        initException = e as SystemNotInitializedException;
                        if (initException == null) initException = new SystemNotInitializedException(e);
                        throw initException;
                    }

                    if (initResult != RESULT.OK)
                        throw new SystemNotInitializedException(initResult, "Output forced to NO SOUND mode");
                }

                return instance;
            }
        }

        public static FMOD.Studio.System StudioSystem => Instance.studioSystem;

        public static FMOD.System CoreSystem => Instance.coreSystem;

        private struct LoadedBank
        {
            public Bank Bank;
            public int RefCount;
        }

        // Explicit comparer to avoid issues on platforms that don't support JIT compilation
        private class GuidComparer : IEqualityComparer<GUID>
        {
            bool IEqualityComparer<GUID>.Equals(GUID x, GUID y)
            {
                return x.Equals(y);
            }

            int IEqualityComparer<GUID>.GetHashCode(GUID obj)
            {
                return obj.GetHashCode();
            }
        }

        private void CheckInitResult(RESULT result, string cause)
        {
            if (result != RESULT.OK)
            {
                ReleaseStudioSystem();
                throw new SystemNotInitializedException(result, cause);
            }
        }

        private void ReleaseStudioSystem()
        {
            if (studioSystem.isValid())
            {
                studioSystem.release();
                studioSystem.clearHandle();
            }
        }

        private RESULT Initialize()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
            AppDomain.CurrentDomain.DomainUnload += HandleDomainUnload;
#endif // UNITY_EDITOR

            var result = RESULT.OK;
            var initResult = RESULT.OK;
            var fmodSettings = Settings.Instance;
            currentPlatform = fmodSettings.FindCurrentPlatform();

            var sampleRate = currentPlatform.SampleRate;
            var realChannels = Math.Min(currentPlatform.RealChannelCount, 256);
            var virtualChannels = currentPlatform.VirtualChannelCount;
            var dspBufferLength = (uint)currentPlatform.DSPBufferLength;
            var dspBufferCount = currentPlatform.DSPBufferCount;
            var speakerMode = currentPlatform.SpeakerMode;
            var outputType = currentPlatform.GetOutputType();

            var advancedSettings = new ADVANCEDSETTINGS();
            advancedSettings.randomSeed = (uint)DateTime.UtcNow.Ticks;
            advancedSettings.maxAT9Codecs = GetChannelCountForFormat(CodecType.AT9);
            advancedSettings.maxFADPCMCodecs = GetChannelCountForFormat(CodecType.FADPCM);
            advancedSettings.maxOpusCodecs = GetChannelCountForFormat(CodecType.Opus);
            advancedSettings.maxVorbisCodecs = GetChannelCountForFormat(CodecType.Vorbis);
            advancedSettings.maxXMACodecs = GetChannelCountForFormat(CodecType.XMA);

            SetThreadAffinities(currentPlatform);

            currentPlatform.PreSystemCreate(CheckInitResult);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            debugCallback = DEBUG_CALLBACK;
            result = Debug.Initialize(fmodSettings.LoggingLevel, DEBUG_MODE.CALLBACK, debugCallback);
            if (result == RESULT.ERR_UNSUPPORTED)
                RuntimeUtils.DebugLogWarning(
                    "[FMOD] Unable to initialize debug logging: Logging will be disabled.\nCheck the Import Settings of the FMOD libs to enable the logging library.");
            else
                CheckInitResult(result, "FMOD.Debug.Initialize");
#endif

            var studioInitFlags = INITFLAGS.NORMAL | INITFLAGS.DEFERRED_CALLBACKS;
            if (currentPlatform.IsLiveUpdateEnabled)
            {
                studioInitFlags |= INITFLAGS.LIVEUPDATE;
                advancedSettings.profilePort = (ushort)currentPlatform.LiveUpdatePort;
            }

            retry:
            result = FMOD.Studio.System.create(out studioSystem);
            CheckInitResult(result, "FMOD.Studio.System.create");

            result = studioSystem.getCoreSystem(out coreSystem);
            CheckInitResult(result, "FMOD.Studio.System.getCoreSystem");

            result = coreSystem.setOutput(outputType);
            CheckInitResult(result, "FMOD.System.setOutput");

            result = coreSystem.setSoftwareChannels(realChannels);
            CheckInitResult(result, "FMOD.System.setSoftwareChannels");

            result = coreSystem.setSoftwareFormat(sampleRate, speakerMode, 0);
            CheckInitResult(result, "FMOD.System.setSoftwareFormat");

            if (dspBufferLength > 0 && dspBufferCount > 0)
            {
                result = coreSystem.setDSPBufferSize(dspBufferLength, dspBufferCount);
                CheckInitResult(result, "FMOD.System.setDSPBufferSize");
            }

            result = coreSystem.setAdvancedSettings(ref advancedSettings);
            CheckInitResult(result, "FMOD.System.setAdvancedSettings");

            if (fmodSettings.EnableErrorCallback)
            {
                errorCallback = ERROR_CALLBACK;
                result = coreSystem.setCallback(errorCallback, SYSTEM_CALLBACK_TYPE.ERROR);
                CheckInitResult(result, "FMOD.System.setCallback");
            }

            if (!string.IsNullOrEmpty(fmodSettings.EncryptionKey))
            {
                var studioAdvancedSettings = new FMOD.Studio.ADVANCEDSETTINGS();
                result = studioSystem.setAdvancedSettings(studioAdvancedSettings, Settings.Instance.EncryptionKey);
                CheckInitResult(result, "FMOD.Studio.System.setAdvancedSettings");
            }

            if (fmodSettings.EnableMemoryTracking) studioInitFlags |= INITFLAGS.MEMORY_TRACKING;

            currentPlatform.PreInitialize(studioSystem);

            var callbackHandler = currentPlatform.CallbackHandler;

            if (callbackHandler != null) callbackHandler.PreInitialize(studioSystem, CheckInitResult);

            result = studioSystem.initialize(virtualChannels, studioInitFlags, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            if (result != RESULT.OK && initResult == RESULT.OK)
            {
                initResult =
                    result; // Save this to throw at the end (we'll attempt NO SOUND to shield ourselves from unexpected device failures)
                outputType = OUTPUTTYPE.NOSOUND;
                RuntimeUtils.DebugLogErrorFormat(
                    "[FMOD] Studio::System::initialize returned {0}, defaulting to no-sound mode.", result.ToString());

                goto retry;
            }

            CheckInitResult(result, "Studio::System::initialize");

            // Test network functionality triggered during System::update
            if ((studioInitFlags & INITFLAGS.LIVEUPDATE) != 0)
            {
                studioSystem.flushCommands(); // Any error will be returned through Studio.System.update

                result = studioSystem.update();
                if (result == RESULT.ERR_NET_SOCKET_ERROR)
                {
                    studioInitFlags &= ~INITFLAGS.LIVEUPDATE;
                    RuntimeUtils.DebugLogWarning(
                        "[FMOD] Cannot open network port for Live Update (in-use), restarting with Live Update disabled.");

                    result = studioSystem.release();
                    CheckInitResult(result, "FMOD.Studio.System.Release");

                    goto retry;
                }
            }

            currentPlatform.LoadPlugins(coreSystem, CheckInitResult);
            LoadBanks(fmodSettings);

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            RegisterSuspendCallback(HandleInterrupt);
#endif

            return initResult;
        }

        private int GetChannelCountForFormat(CodecType format)
        {
            var channelCount = currentPlatform.CodecChannels.Find(x => x.format == format);

            return channelCount == null ? 0 : Math.Min(channelCount.channels, 256);
        }

        private static void SetThreadAffinities(Platform platform)
        {
            foreach (var group in platform.ThreadAffinities)
            foreach (var thread in group.threads)
            {
                var fmodThread = RuntimeUtils.ToFMODThreadType(thread);
                var fmodAffinity = RuntimeUtils.ToFMODThreadAffinity(group.affinity);

                Thread.SetAttributes(fmodThread, fmodAffinity);
            }
        }

        private class AttachedInstance
        {
            public EventInstance instance;
#if UNITY_PHYSICS_EXIST
            public Rigidbody rigidBody;
#endif
#if UNITY_PHYSICS2D_EXIST
            public Rigidbody2D rigidBody2D;
#endif
            public Transform transform;
        }

        private void Update()
        {
            if (studioSystem.isValid())
            {
                if (StudioListener.ListenerCount <= 0 && !listenerWarningIssued)
                {
                    listenerWarningIssued = true;
                    RuntimeUtils.DebugLogWarning(
                        "[FMOD] Please add an 'FMOD Studio Listener' component to your camera in the scene for correct 3D positioning of sounds.");
                }

                StudioEventEmitter.UpdateActiveEmitters();

                for (var i = 0; i < attachedInstances.Count; i++)
                {
                    var playbackState = PLAYBACK_STATE.STOPPED;
                    if (attachedInstances[i].instance.isValid())
                        attachedInstances[i].instance.getPlaybackState(out playbackState);

                    if (playbackState == PLAYBACK_STATE.STOPPED ||
                        attachedInstances[i].transform == null // destroyed game object
                       )
                    {
                        attachedInstances[i] = attachedInstances[attachedInstances.Count - 1];
                        attachedInstances.RemoveAt(attachedInstances.Count - 1);
                        i--;
                        continue;
                    }

#if UNITY_PHYSICS_EXIST
                    if (attachedInstances[i].rigidBody)
                        attachedInstances[i].instance
                            .set3DAttributes(RuntimeUtils.To3DAttributes(attachedInstances[i].transform,
                                attachedInstances[i].rigidBody));
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (attachedInstances[i].rigidBody2D)
                        attachedInstances[i].instance
                            .set3DAttributes(RuntimeUtils.To3DAttributes(attachedInstances[i].transform,
                                attachedInstances[i].rigidBody2D));
                    else
#endif
                        attachedInstances[i].instance.set3DAttributes(attachedInstances[i].transform.To3DAttributes());
                }

#if UNITY_EDITOR
                ApplyMuteState();

                for (var i = eventPositionWarnings.Count - 1; i >= 0; i--)
                {
                    if (eventPositionWarnings[i].isValid())
                    {
                        ATTRIBUTES_3D attribs;
                        eventPositionWarnings[i].get3DAttributes(out attribs);
                        if (attribs.position.x == 1e+18F &&
                            attribs.position.y == 1e+18F &&
                            attribs.position.z == 1e+18F)
                        {
                            string path;
                            EventDescription desc;
                            eventPositionWarnings[i].getDescription(out desc);
                            desc.getPath(out path);
                            RuntimeUtils.DebugLogWarningFormat(
                                "[FMOD] Instance of Event {0} has not had EventInstance.set3DAttributes() called on it yet!",
                                path);
                        }
                    }

                    eventPositionWarnings.RemoveAt(i);
                }

                isOverlayEnabled = currentPlatform.IsOverlayEnabled;
#endif

                if (isOverlayEnabled)
                {
                    if (!overlayDrawer)
                    {
                        overlayDrawer = Instance.gameObject.AddComponent<FMODRuntimeManagerOnGUIHelper>();
                        overlayDrawer.TargetRuntimeManager = this;
                    }
                    else
                    {
                        overlayDrawer.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (overlayDrawer != null && overlayDrawer.gameObject.activeSelf)
                        overlayDrawer.gameObject.SetActive(false);
                }

                studioSystem.update();
            }
        }

        public static void AttachInstanceToGameObject(EventInstance instance, Transform transform)
        {
            var attachedInstance = Instance.attachedInstances.Find(x => x.instance.handle == instance.handle);
            if (attachedInstance == null)
            {
                attachedInstance = new AttachedInstance();
                Instance.attachedInstances.Add(attachedInstance);
            }

            instance.set3DAttributes(transform.To3DAttributes());
            attachedInstance.transform = transform;
            attachedInstance.instance = instance;
        }

#if UNITY_PHYSICS_EXIST
        public static void AttachInstanceToGameObject(EventInstance instance, Transform transform, Rigidbody rigidBody)
        {
            var attachedInstance = Instance.attachedInstances.Find(x => x.instance.handle == instance.handle);
            if (attachedInstance == null)
            {
                attachedInstance = new AttachedInstance();
                Instance.attachedInstances.Add(attachedInstance);
            }

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rigidBody));
            attachedInstance.transform = transform;
            attachedInstance.instance = instance;
            attachedInstance.rigidBody = rigidBody;
        }
#endif

#if UNITY_PHYSICS2D_EXIST
        public static void AttachInstanceToGameObject(EventInstance instance, Transform transform,
            Rigidbody2D rigidBody2D)
        {
            var attachedInstance = Instance.attachedInstances.Find(x => x.instance.handle == instance.handle);
            if (attachedInstance == null)
            {
                attachedInstance = new AttachedInstance();
                Instance.attachedInstances.Add(attachedInstance);
            }

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rigidBody2D));
            attachedInstance.transform = transform;
            attachedInstance.instance = instance;
            attachedInstance.rigidBody2D = rigidBody2D;
        }
#endif

        public static void DetachInstanceFromGameObject(EventInstance instance)
        {
            var manager = Instance;
            for (var i = 0; i < manager.attachedInstances.Count; i++)
                if (manager.attachedInstances[i].instance.handle == instance.handle)
                {
                    manager.attachedInstances[i] = manager.attachedInstances[manager.attachedInstances.Count - 1];
                    manager.attachedInstances.RemoveAt(manager.attachedInstances.Count - 1);
                    return;
                }
        }

        internal void ExecuteOnGUI()
        {
            if (studioSystem.isValid() && isOverlayEnabled)
                windowRect = GUI.Window(GetInstanceID(), windowRect, DrawDebugOverlay, "FMOD Studio Debug");
        }

#if !UNITY_EDITOR
        private void Start()
        {
            isOverlayEnabled = currentPlatform.IsOverlayEnabled;
        }
#endif

        private void DrawDebugOverlay(int windowID)
        {
            if (lastDebugUpdate + 0.25f < Time.unscaledTime)
            {
                if (initException != null)
                {
                    lastDebugText = initException.Message;
                }
                else
                {
                    if (!mixerHead.hasHandle())
                    {
                        ChannelGroup master;
                        coreSystem.getMasterChannelGroup(out master);
                        master.getDSP(0, out mixerHead);
                        mixerHead.setMeteringEnabled(false, true);
                    }

                    var debug = new StringBuilder();

                    CPU_USAGE cpuUsage;
                    FMOD.CPU_USAGE cpuUsage_core;
                    studioSystem.getCPUUsage(out cpuUsage, out cpuUsage_core);
                    debug.AppendFormat("CPU: dsp = {0:F1}%, studio = {1:F1}%\n", cpuUsage_core.dsp, cpuUsage.update);

                    int currentAlloc, maxAlloc;
                    Memory.GetStats(out currentAlloc, out maxAlloc);
                    debug.AppendFormat("MEMORY: cur = {0}MB, max = {1}MB\n", currentAlloc >> 20, maxAlloc >> 20);

                    int realchannels, channels;
                    coreSystem.getChannelsPlaying(out channels, out realchannels);
                    debug.AppendFormat("CHANNELS: real = {0}, total = {1}\n", realchannels, channels);

                    DSP_METERING_INFO outputMetering;
                    mixerHead.getMeteringInfo(IntPtr.Zero, out outputMetering);
                    float rms = 0;
                    for (var i = 0; i < outputMetering.numchannels; i++)
                        rms += outputMetering.rmslevel[i] * outputMetering.rmslevel[i];
                    rms = Mathf.Sqrt(rms / outputMetering.numchannels);

                    var db = rms > 0 ? 20.0f * Mathf.Log10(rms * Mathf.Sqrt(2.0f)) : -80.0f;
                    if (db > 10.0f) db = 10.0f;

                    debug.AppendFormat("VOLUME: RMS = {0:f2}db\n", db);
                    lastDebugText = debug.ToString();
                    lastDebugUpdate = Time.unscaledTime;
                }
            }

            GUI.Label(new Rect(10, 20, 290, 100), lastDebugText);
            GUI.DragWindow();
        }

        private void OnDestroy()
        {
            coreSystem.setCallback(null, 0);
            ReleaseStudioSystem();

            initException = null;
            instance = null;

#if UNITY_EDITOR
            AppDomain.CurrentDomain.DomainUnload -= HandleDomainUnload;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
#endif
        }

#if UNITY_EDITOR
        private static void Destroy()
        {
            if (instance) DestroyImmediate(instance.gameObject);
        }

        private void HandleDomainUnload(object sender, EventArgs args)
        {
            ReleaseStudioSystem();
        }

        private void HandlePlayModeStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode)
            {
                if (state == PlayModeStateChange.ExitingEditMode && EditorSettings.enterPlayModeOptionsEnabled &&
                    (EditorSettings.enterPlayModeOptions | EnterPlayModeOptions.DisableDomainReload) != 0)
                    OnDestroy(); // When domain reload is disabled, OnDestroy is not called when entering play mode, breaking live update.
                Destroy();
            }
        }
#endif

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        [AOT.MonoPInvokeCallback(typeof(Action<bool>))]
        private static void HandleInterrupt(bool began)
        {
            if (Instance.studioSystem.isValid())
            {
                // Strings bank is always loaded
                if (Instance.loadedBanks.Count > 1)
                    PauseAllEvents(began);

                if (began)
                {
                    Instance.coreSystem.mixerSuspend();
                }
                else
                {
                    Instance.coreSystem.mixerResume();
                }
            }
        }
#else
        private void OnApplicationPause(bool pauseStatus)
        {
            if (studioSystem.isValid())
            {
                PauseAllEvents(pauseStatus);

                if (pauseStatus)
                    coreSystem.mixerSuspend();
                else
                    coreSystem.mixerResume();
            }
        }
#endif

        private static void ReferenceLoadedBank(string bankName, bool loadSamples)
        {
            var loadedBank = Instance.loadedBanks[bankName];
            loadedBank.RefCount++;

            if (loadSamples) loadedBank.Bank.loadSampleData();

            Instance.loadedBanks[bankName] = loadedBank; // Save the incremented reference count
        }

        private void RegisterLoadedBank(LoadedBank loadedBank, string bankPath, string bankName, bool loadSamples,
            RESULT loadResult)
        {
            if (loadResult == RESULT.OK)
            {
                loadedBank.RefCount = 1;

                if (loadSamples) loadedBank.Bank.loadSampleData();

                Instance.loadedBanks.Add(bankName, loadedBank);
            }
            else if (loadResult == RESULT.ERR_EVENT_ALREADY_LOADED)
            {
                RuntimeUtils.DebugLogWarningFormat(
                    "[FMOD] Unable to load {0} - bank already loaded. This may occur when attempting to load another localized bank before the first is unloaded, or if a bank has been loaded via the API.",
                    bankName);
            }
            else
            {
                throw new BankLoadException(bankPath, loadResult);
            }

            ExecuteSampleLoadRequestsIfReady();
        }

        private void ExecuteSampleLoadRequestsIfReady()
        {
            if (sampleLoadRequests.Count > 0)
            {
                foreach (var bankName in sampleLoadRequests)
                    if (!loadedBanks.ContainsKey(bankName))
                        // Not ready
                        return;

                // All requested banks are loaded, so we can now load sample data
                foreach (var bankName in sampleLoadRequests)
                {
                    var loadedBank = loadedBanks[bankName];
                    CheckInitResult(loadedBank.Bank.loadSampleData(),
                        string.Format("Loading sample data for bank: {0}", bankName));
                }

                sampleLoadRequests.Clear();
            }
        }

#if UNITY_ANDROID || UNITY_WEBGL
        private IEnumerator loadFromWeb(string bankPath, string bankName, bool loadSamples)
        {
            byte[] loadWebResult;
            FMOD.RESULT loadResult;

            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(bankPath);
            yield return www.SendWebRequest();
            loadWebResult = www.downloadHandler.data;

            LoadedBank loadedBank = new LoadedBank();
            loadResult =
 Instance.studioSystem.loadBankMemory(loadWebResult, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out loadedBank.Bank);
            if (loadResult != FMOD.RESULT.OK)
            {
                RuntimeUtils.DebugLogWarningFormat("[FMOD] loadFromWeb.  Path = {0}, result = {1}.", bankPath, loadResult);
            }
            RegisterLoadedBank(loadedBank, bankPath, bankName, loadSamples, loadResult);
            loadingBanksRef--;

            RuntimeUtils.DebugLogFormat("[FMOD] Finished loading {0}", bankPath);
        }
#endif // UNITY_ANDROID || UNITY_WEBGL

        public static void LoadBank(string bankName, bool loadSamples = false)
        {
            LoadBank(bankName, loadSamples, bankName);
        }

        private static void LoadBank(string bankName, bool loadSamples, string bankId)
        {
            if (Instance.loadedBanks.ContainsKey(bankId))
            {
                ReferenceLoadedBank(bankId, loadSamples);
            }
            else
            {
                var bankFolder = Instance.currentPlatform.GetBankFolder();

#if !UNITY_EDITOR
                if (!string.IsNullOrEmpty(Settings.Instance.TargetSubFolder))
                {
                    bankFolder =
 RuntimeUtils.GetCommonPlatformPath(Path.Combine(bankFolder, Settings.Instance.TargetSubFolder));
                }
#endif

                const string BankExtension = ".bank";

                string bankPath;

                if (Path.GetExtension(bankName) != BankExtension)
                    bankPath = string.Format("{0}/{1}{2}", bankFolder, bankName, BankExtension);
                else
                    bankPath = string.Format("{0}/{1}", bankFolder, bankName);
                Instance.loadingBanksRef++;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (Settings.Instance.AndroidUseOBB)
                {
                    Instance.StartCoroutine(Instance.loadFromWeb(bankPath, bankName, loadSamples));
                }
                else
#elif UNITY_WEBGL && !UNITY_EDITOR
                if (true)
                {
                    Instance.StartCoroutine(Instance.loadFromWeb(bankPath, bankName, loadSamples));
                }
                else
#endif // (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
                {
                    var loadedBank = new LoadedBank();
                    var loadResult =
                        Instance.studioSystem.loadBankFile(bankPath, LOAD_BANK_FLAGS.NORMAL, out loadedBank.Bank);
                    Instance.RegisterLoadedBank(loadedBank, bankPath, bankId, loadSamples, loadResult);
                    Instance.loadingBanksRef--;
                }
            }
        }

        public static void LoadBank(TextAsset asset, bool loadSamples = false)
        {
            LoadBank(asset, loadSamples, asset.name);
        }

        private static void LoadBank(TextAsset asset, bool loadSamples, string bankId)
        {
            if (Instance.loadedBanks.ContainsKey(bankId))
            {
                ReferenceLoadedBank(bankId, loadSamples);
            }
            else
            {
#if UNITY_EDITOR
                if (asset.text.StartsWith(BankStubPrefix))
                {
                    var name = asset.text.Substring(BankStubPrefix.Length);
                    LoadBank(name, loadSamples, bankId);
                    return;
                }
#endif

                var loadedBank = new LoadedBank();
                var loadResult =
                    Instance.studioSystem.loadBankMemory(asset.bytes, LOAD_BANK_FLAGS.NORMAL, out loadedBank.Bank);
                Instance.RegisterLoadedBank(loadedBank, bankId, bankId, loadSamples, loadResult);
            }
        }

#if UNITY_ADDRESSABLES_EXIST
        public static void LoadBank(AssetReference assetReference, bool loadSamples =
 false, System.Action completionCallback = null)
        {
            if (Instance.loadedBanks.ContainsKey(assetReference.AssetGUID))
            {
                ReferenceLoadedBank(assetReference.AssetGUID, loadSamples);
            }
            else
            {
                Instance.loadingBanksRef++;
                assetReference.LoadAssetAsync<TextAsset>().Completed += (obj) =>
                {
                    if (!obj.IsValid())
                    {
                        RuntimeUtils.DebugLogError("[FMOD] Unable to load AssetReference: " + obj.OperationException);
                        return;
                    }

                    TextAsset bank = obj.Result;
                    LoadBank(bank, loadSamples, assetReference.AssetGUID);

                    Instance.loadingBanksRef--;

                    if (completionCallback != null)
                    {
                        completionCallback();
                    }

                    assetReference.ReleaseAsset();
                };

            }
        }
#endif

        private void LoadBanks(Settings fmodSettings)
        {
            if (fmodSettings.ImportType == ImportType.StreamingAssets)
            {
                if (fmodSettings.AutomaticSampleLoading) sampleLoadRequests.AddRange(BanksToLoad(fmodSettings));

                try
                {
                    foreach (var bankName in BanksToLoad(fmodSettings)) LoadBank(bankName);

                    WaitForAllSampleLoading();
                }
                catch (BankLoadException e)
                {
                    RuntimeUtils.DebugLogException(e);
                }
            }
        }

        private IEnumerable<string> BanksToLoad(Settings fmodSettings)
        {
            switch (fmodSettings.BankLoadType)
            {
                case BankLoadType.All:
                    foreach (var masterBankFileName in fmodSettings.MasterBanks)
                    {
                        yield return masterBankFileName + ".strings";
                        yield return masterBankFileName;
                    }

                    foreach (var bank in fmodSettings.Banks) yield return bank;
                    break;
                case BankLoadType.Specified:
                    foreach (var bank in fmodSettings.BanksToLoad)
                        if (!string.IsNullOrEmpty(bank))
                            yield return bank;
                    break;
                case BankLoadType.None:
                    break;
            }
        }

        public static void UnloadBank(string bankName)
        {
            LoadedBank loadedBank;
            if (Instance.loadedBanks.TryGetValue(bankName, out loadedBank))
            {
                loadedBank.RefCount--;
                if (loadedBank.RefCount == 0)
                {
                    loadedBank.Bank.unload();
                    Instance.loadedBanks.Remove(bankName);
                    Instance.sampleLoadRequests.Remove(bankName);
                    return;
                }

                Instance.loadedBanks[bankName] = loadedBank;
            }
        }

        public static void UnloadBank(TextAsset asset)
        {
            UnloadBank(asset.name);
        }

#if UNITY_ADDRESSABLES_EXIST
        public static void UnloadBank(AssetReference assetReference)
        {
            UnloadBank(assetReference.AssetGUID);
        }
#endif

        [Obsolete("[FMOD] Deprecated. Use AnySampleDataLoading instead.")]
        public static bool AnyBankLoading()
        {
            return AnySampleDataLoading();
        }

        public static bool AnySampleDataLoading()
        {
            var loading = false;
            foreach (var bank in Instance.loadedBanks.Values)
            {
                LOADING_STATE loadingState;
                bank.Bank.getSampleLoadingState(out loadingState);
                loading |= loadingState == LOADING_STATE.LOADING;
            }

            return loading;
        }

        [Obsolete("[FMOD] Deprecated. Use WaitForAllSampleLoading instead.")]
        public static void WaitForAllLoads()
        {
            WaitForAllSampleLoading();
        }

        public static void WaitForAllSampleLoading()
        {
            Instance.studioSystem.flushSampleLoading();
        }

        public static GUID PathToGUID(string path)
        {
            GUID guid;
            if (path.StartsWith("{"))
            {
                Util.parseID(path, out guid);
            }
            else
            {
                var result = Instance.studioSystem.lookupID(path, out guid);
                if (result == RESULT.ERR_EVENT_NOTFOUND) throw new EventNotFoundException(path);
            }

            return guid;
        }

        public static EventReference PathToEventReference(string path)
        {
            GUID guid;

            try
            {
                guid = PathToGUID(path);
            }
            catch (EventNotFoundException)
            {
                guid = new GUID();
            }

#if UNITY_EDITOR
            return new EventReference { Path = path, Guid = guid };
#else
            return new EventReference() { Guid = guid };
#endif
        }

        public static EventInstance CreateInstance(EventReference eventReference)
        {
            try
            {
                return CreateInstance(eventReference.Guid);
            }
            catch (EventNotFoundException)
            {
                throw new EventNotFoundException(eventReference);
            }
        }

        public static EventInstance CreateInstance(string path)
        {
            try
            {
                return CreateInstance(PathToGUID(path));
            }
            catch (EventNotFoundException)
            {
                // Switch from exception with GUID to exception with path
                throw new EventNotFoundException(path);
            }
        }

        public static EventInstance CreateInstance(GUID guid)
        {
            var eventDesc = GetEventDescription(guid);
            EventInstance newInstance;
            eventDesc.createInstance(out newInstance);

#if UNITY_EDITOR
            var is3D = false;
            eventDesc.is3D(out is3D);
            if (is3D)
            {
                // Set position to 1e+18F, set3DAttributes should be called by the dev after this.
                newInstance.set3DAttributes(new Vector3(1e+18F, 1e+18F, 1e+18F).To3DAttributes());
                instance.eventPositionWarnings.Add(newInstance);
            }
#endif

            return newInstance;
        }

        public static void PlayOneShot(EventReference eventReference, Vector3 position = new())
        {
            try
            {
                PlayOneShot(eventReference.Guid, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
            }
        }

        public static void PlayOneShot(string path, Vector3 position = new())
        {
            try
            {
                PlayOneShot(PathToGUID(path), position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
            }
        }

        public static void PlayOneShot(GUID guid, Vector3 position = new())
        {
            var instance = CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());
            instance.start();
            instance.release();
        }

        public static void PlayOneShotAttached(EventReference eventReference, GameObject gameObject)
        {
            try
            {
                PlayOneShotAttached(eventReference.Guid, gameObject);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
            }
        }

        public static void PlayOneShotAttached(string path, GameObject gameObject)
        {
            try
            {
                PlayOneShotAttached(PathToGUID(path), gameObject);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
            }
        }

        public static void PlayOneShotAttached(GUID guid, GameObject gameObject)
        {
            var instance = CreateInstance(guid);
#if UNITY_PHYSICS_EXIST
            AttachInstanceToGameObject(instance, gameObject.transform, gameObject.GetComponent<Rigidbody>());
#elif UNITY_PHYSICS2D_EXIST
            AttachInstanceToGameObject(instance, gameObject.transform, gameObject.GetComponent<Rigidbody2D>());
#else
            AttachInstanceToGameObject(instance, gameObject.transform);
#endif
            instance.start();
            instance.release();
        }

        public static EventDescription GetEventDescription(EventReference eventReference)
        {
            try
            {
                return GetEventDescription(eventReference.Guid);
            }
            catch (EventNotFoundException)
            {
                throw new EventNotFoundException(eventReference);
            }
        }

        public static EventDescription GetEventDescription(string path)
        {
            try
            {
                return GetEventDescription(PathToGUID(path));
            }
            catch (EventNotFoundException)
            {
                throw new EventNotFoundException(path);
            }
        }

        public static EventDescription GetEventDescription(GUID guid)
        {
            EventDescription eventDesc;
            if (Instance.cachedDescriptions.ContainsKey(guid) && Instance.cachedDescriptions[guid].isValid())
            {
                eventDesc = Instance.cachedDescriptions[guid];
            }
            else
            {
                var result = Instance.studioSystem.getEventByID(guid, out eventDesc);

                if (result != RESULT.OK) throw new EventNotFoundException(guid);

                if (eventDesc.isValid()) Instance.cachedDescriptions[guid] = eventDesc;
            }

            return eventDesc;
        }

#if UNITY_PHYSICS_EXIST
        public static void SetListenerLocation(GameObject gameObject, Rigidbody rigidBody,
            GameObject attenuationObject = null)
        {
            SetListenerLocation(0, gameObject, rigidBody, attenuationObject);
        }

        public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody rigidBody,
            GameObject attenuationObject = null)
        {
            if (attenuationObject)
                Instance.studioSystem.setListenerAttributes(listenerIndex,
                    RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody),
                    attenuationObject.transform.position.ToFMODVector());
            else
                Instance.studioSystem.setListenerAttributes(listenerIndex,
                    RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody));
        }
#endif

#if UNITY_PHYSICS2D_EXIST
        public static void SetListenerLocation(GameObject gameObject, Rigidbody2D rigidBody2D,
            GameObject attenuationObject = null)
        {
            SetListenerLocation(0, gameObject, rigidBody2D, attenuationObject);
        }

        public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody2D rigidBody2D,
            GameObject attenuationObject = null)
        {
            if (attenuationObject)
                Instance.studioSystem.setListenerAttributes(listenerIndex,
                    RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody2D),
                    attenuationObject.transform.position.ToFMODVector());
            else
                Instance.studioSystem.setListenerAttributes(listenerIndex,
                    RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody2D));
        }
#endif

        public static void SetListenerLocation(GameObject gameObject, GameObject attenuationObject = null)
        {
            SetListenerLocation(0, gameObject, attenuationObject);
        }

        public static void SetListenerLocation(int listenerIndex, GameObject gameObject,
            GameObject attenuationObject = null)
        {
            if (attenuationObject)
                Instance.studioSystem.setListenerAttributes(listenerIndex, gameObject.transform.To3DAttributes(),
                    attenuationObject.transform.position.ToFMODVector());
            else
                Instance.studioSystem.setListenerAttributes(listenerIndex, gameObject.transform.To3DAttributes());
        }

        public static Bus GetBus(string path)
        {
            Bus bus;
            if (StudioSystem.getBus(path, out bus) != RESULT.OK) throw new BusNotFoundException(path);
            return bus;
        }

        public static VCA GetVCA(string path)
        {
            VCA vca;
            if (StudioSystem.getVCA(path, out vca) != RESULT.OK) throw new VCANotFoundException(path);
            return vca;
        }

        public static void PauseAllEvents(bool paused)
        {
            if (HaveMasterBanksLoaded)
            {
                Bus masterBus;
                if (StudioSystem.getBus("bus:/", out masterBus) == RESULT.OK) masterBus.setPaused(paused);
            }
        }

        public static void MuteAllEvents(bool muted)
        {
            Instance.isMuted = muted;

            ApplyMuteState();
        }

        private static void ApplyMuteState()
        {
            if (HaveMasterBanksLoaded)
            {
                Bus masterBus;
                if (StudioSystem.getBus("bus:/", out masterBus) == RESULT.OK)
                {
#if UNITY_EDITOR
                    masterBus.setMute(Instance.isMuted || EditorUtility.audioMasterMute);
#else
                    masterBus.setMute(Instance.isMuted);
#endif
                }
            }
        }

        public static bool IsInitialized => instance != null && instance.studioSystem.isValid();

        public static bool HaveAllBanksLoaded => Instance.loadingBanksRef == 0;

        public static bool HaveMasterBanksLoaded
        {
            get
            {
                var banks = Settings.Instance.MasterBanks;
                foreach (var bank in banks)
                    if (!HasBankLoaded(bank))
                        return false;
                return true;
            }
        }

        public static bool HasBankLoaded(string loadedBank)
        {
            return Instance.loadedBanks.ContainsKey(loadedBank);
        }

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void RegisterSuspendCallback(Action<bool> func);
#endif
    }
}