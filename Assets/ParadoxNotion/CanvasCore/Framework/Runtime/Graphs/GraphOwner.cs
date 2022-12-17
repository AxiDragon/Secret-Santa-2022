using System;
using System.Collections.Generic;
using NodeCanvas.Editor;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ParadoxNotion.Services.Logger;
using Object = UnityEngine.Object;

namespace NodeCanvas.Framework
{
    ///<summary> A component that is used to control a Graph in respects to the gameobject attached to</summary>
    public abstract class GraphOwner : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum DisableAction
        {
            DisableBehaviour,
            PauseBehaviour,
            DoNothing
        }

        ///----------------------------------------------------------------------------------------------
        public enum EnableAction
        {
            EnableBehaviour,
            DoNothing
        }

        public enum FirstActivation
        {
            OnEnable,
            OnStart,
            Async
        }

        ///----------------------------------------------------------------------------------------------
        [SerializeField] private SerializationPair[] _serializedExposedParameters;

        [SerializeField] [FormerlySerializedAs("boundGraphSerialization")]
        private string _boundGraphSerialization;

        [SerializeField] [FormerlySerializedAs("boundGraphObjectReferences")]
        private List<Object> _boundGraphObjectReferences;

        [SerializeField] private GraphSource _boundGraphSource = new();

        [SerializeField]
        [FormerlySerializedAs("firstActivation")]
        [Tooltip(
            "When the graph will first activate. Async mode will load the graph on a separate thread (thus no spikes), but the graph will activate a few frames later.")]
        private FirstActivation _firstActivation = FirstActivation.OnEnable;

        [SerializeField]
        [FormerlySerializedAs("enableAction")]
        [Tooltip("What will happen when the GraphOwner is enabled")]
        private EnableAction _enableAction = EnableAction.EnableBehaviour;

        [SerializeField]
        [FormerlySerializedAs("disableAction")]
        [Tooltip("What will happen when the GraphOwner is disabled")]
        private DisableAction _disableAction = DisableAction.DisableBehaviour;

        [SerializeField] [Tooltip("If enabled, bound graph prefab overrides in instances will not be possible")]
        private bool _lockBoundGraphPrefabOverrides = true;

        [SerializeField]
        [Tooltip(
            "If enabled, all subgraphs will be pre-initialized in Awake along with the root graph, but this may have a loading performance cost")]
        private bool _preInitializeSubGraphs;

        [SerializeField]
        [Tooltip(
            "Specify when (if) the behaviour is updated. Changes to this only work when the behaviour starts, or re-starts")]
        private Graph.UpdateMode _updateMode = Graph.UpdateMode.NormalUpdate;

        private readonly Dictionary<Graph, Graph> instances = new();

        ///<summary>The list of exposed parameters if any</summary>
        public List<ExposedParameter> exposedParameters { get; set; }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>The graph assigned</summary>
        public abstract Graph graph { get; set; }

        ///<summary>The blackboard assigned</summary>
        public abstract IBlackboard blackboard { get; set; }

        ///<summary>The type of graph that can be assigned</summary>
        public abstract Type graphType { get; }

        public bool initialized { get; private set; }
        public bool enableCalled { get; private set; }
        public bool startCalled { get; private set; }

        ///<summary>The bound graph source data</summary>
        public GraphSource boundGraphSource
        {
            get => _boundGraphSource;
            private set => _boundGraphSource = value;
        }

        ///<summary>The bound graph serialization if any</summary>
        public string boundGraphSerialization
        {
            get => _boundGraphSerialization;
            private set => _boundGraphSerialization = value;
        }

        ///<summary>The bound graph object references if any (this is a reference list. Dont touch if you are not sure how :) )</summary>
        public List<Object> boundGraphObjectReferences
        {
            get => _boundGraphObjectReferences;
            private set => _boundGraphObjectReferences = value;
        }

        ///<summary>Is the bound graph locked to changes from prefab instances?</summary>
        public bool lockBoundGraphPrefabOverrides
        {
            get => _lockBoundGraphPrefabOverrides && graphIsBound;
            set => _lockBoundGraphPrefabOverrides = value;
        }

        ///<summary>Will subgraphs be preinitialized along with the root graph?</summary>
        public bool preInitializeSubGraphs
        {
            get => _preInitializeSubGraphs;
            set => _preInitializeSubGraphs = value;
        }

        ///<summary>When will the first activation be (if EnableBehaviour at all)</summary>
        public FirstActivation firstActivation
        {
            get => _firstActivation;
            set => _firstActivation = value;
        }

        ///<summary>What will happen OnEnable</summary>
        public EnableAction enableAction
        {
            get => _enableAction;
            set => _enableAction = value;
        }

        ///<summary>What will happen OnDisable</summary>
        public DisableAction disableAction
        {
            get => _disableAction;
            set => _disableAction = value;
        }

        ///<summary>When is the behaviour updated? Changes to this only work when the behaviour starts (or re-starts)</summary>
        public Graph.UpdateMode updateMode
        {
            get => _updateMode;
            set => _updateMode = value;
        }

        ///<summary>Do we have a bound graph serialization?</summary>
        public bool graphIsBound => !string.IsNullOrEmpty(boundGraphSerialization);

        ///<summary>Is the assigned graph currently running?</summary>
        public bool isRunning => graph != null ? graph.isRunning : false;

        ///<summary>Is the assigned graph currently paused?</summary>
        public bool isPaused => graph != null ? graph.isPaused : false;

        ///<summary>The time is seconds the graph is running</summary>
        public float elapsedTime => graph != null ? graph.elapsedTime : 0;

        ///----------------------------------------------------------------------------------------------

        //Initialize the bound or asset graph
        protected void Awake()
        {
            Initialize();
        }

        //...
        protected void Start()
        {
            if (firstActivation == FirstActivation.OnStart)
                if (!isRunning && enableAction == EnableAction.EnableBehaviour)
                    StartBehaviour();

            InvokeStartEvent();
            startCalled = true;
        }

        //handle enable behaviour setting
        protected void OnEnable()
        {
            if (firstActivation == FirstActivation.OnEnable || enableCalled)
                if ((!isRunning || isPaused) && enableAction == EnableAction.EnableBehaviour)
                    StartBehaviour();

            enableCalled = true;
        }

        //handle disable behaviour setting
        protected void OnDisable()
        {
            if (disableAction == DisableAction.DisableBehaviour) StopBehaviour();

            if (disableAction == DisableAction.PauseBehaviour) PauseBehaviour();
        }

        //Destroy instanced graphs as well
        protected void OnDestroy()
        {
            if (Threader.applicationIsPlaying) StopBehaviour();

            foreach (var instanceGraph in instances.Values)
            {
                foreach (var subGraph in instanceGraph.GetAllInstancedNestedGraphs()) Destroy(subGraph);
                Destroy(instanceGraph);
            }
        }

        //serialize exposed parameters
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (exposedParameters == null || exposedParameters.Count == 0)
            {
                _serializedExposedParameters = null;
                return;
            }

            _serializedExposedParameters = new SerializationPair[exposedParameters.Count];
            for (var i = 0; i < _serializedExposedParameters.Length; i++)
            {
                var serializedParam = new SerializationPair();
                serializedParam._json = JSONSerializer.Serialize(typeof(ExposedParameter), exposedParameters[i],
                    serializedParam._references);
                _serializedExposedParameters[i] = serializedParam;
            }
        }

        //deserialize exposed parameters
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedExposedParameters != null)
            {
                if (exposedParameters == null)
                    exposedParameters = new List<ExposedParameter>();
                else
                    exposedParameters.Clear();
                for (var i = 0; i < _serializedExposedParameters.Length; i++)
                {
                    var exposedParam = JSONSerializer.Deserialize<ExposedParameter>(
                        _serializedExposedParameters[i]._json, _serializedExposedParameters[i]._references);
                    exposedParameters.Add(exposedParam);
                }
            }
        }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>Raised when the assigned behaviour state is changed (start/pause/stop)</summary>
        public static event Action<GraphOwner> onOwnerBehaviourStateChange;

        ///<summary>Raised only once when "Start" is called, then is set to null</summary>
        public event Action onMonoBehaviourStart;

        ///----------------------------------------------------------------------------------------------

        //Gets the instance graph for this owner from the provided graph
        protected Graph GetInstance(Graph originalGraph)
        {
            if (originalGraph == null) return null;

            //in editor the instance is always the original!
            if (!Application.isPlaying) return originalGraph;

            //if its already a stored instance, return the instance
            if (instances.ContainsValue(originalGraph)) return originalGraph;

            Graph instance = null;

            //if it's not a strored instance create, store and return a new instance.
            if (!instances.TryGetValue(originalGraph, out instance))
            {
                instance = Graph.Clone(originalGraph, null);
                instances[originalGraph] = instance;
            }

            return instance;
        }

        ///<summary>Makes and returns the runtime instance based on the current graph set.</summary>
        public Graph MakeRuntimeGraphInstance()
        {
            return graph = GetInstance(graph);
        }

        ///<summary>Start the graph assigned. It will be auto updated.</summary>
        public void StartBehaviour()
        {
            StartBehaviour(updateMode);
        }

        ///<summary>Start the graph assigned providing a callback for when it's finished if at all.</summary>
        public void StartBehaviour(Action<bool> callback)
        {
            StartBehaviour(updateMode, callback);
        }

        /// <summary>
        ///     Start the graph assigned, optionally autoUpdated or not, and providing a callback for when it's finished if at
        ///     all.
        /// </summary>
        public void StartBehaviour(Graph.UpdateMode updateMode, Action<bool> callback = null)
        {
            graph = GetInstance(graph);
            if (graph != null)
            {
                graph.StartGraph(this, blackboard, updateMode, callback);
                if (onOwnerBehaviourStateChange != null) onOwnerBehaviourStateChange(this);
            }
        }

        ///<summary>Pause the current running graph</summary>
        public void PauseBehaviour()
        {
            if (graph != null)
            {
                graph.Pause();
                if (onOwnerBehaviourStateChange != null) onOwnerBehaviourStateChange(this);
            }
        }

        ///<summary>Stop the current running graph</summary>
        public void StopBehaviour(bool success = true)
        {
            if (graph != null)
            {
                graph.Stop(success);
                if (onOwnerBehaviourStateChange != null) onOwnerBehaviourStateChange(this);
            }
        }

        ///<summary>Manually update the assigned graph</summary>
        public void UpdateBehaviour()
        {
            if (graph != null) graph.UpdateGraph();
        }

        ///<summary>The same as calling Stop, Start Behaviour</summary>
        public void RestartBehaviour()
        {
            StopBehaviour();
            StartBehaviour();
        }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>Send an event to the graph. Note that this overload has no sender argument thus sender will be null.</summary>
        public void SendEvent(string eventName)
        {
            if (graph != null) graph.SendEvent(eventName, null, null);
        }

        ///<summary>Send an event to the graph</summary>
        public void SendEvent(string eventName, object value, object sender)
        {
            if (graph != null) graph.SendEvent(eventName, value, sender);
        }

        ///<summary>Send an event to the graph</summary>
        public void SendEvent<T>(string eventName, T eventValue, object sender)
        {
            if (graph != null) graph.SendEvent(eventName, eventValue, sender);
        }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>Return an exposed parameter value</summary>
        public T GetExposedParameterValue<T>(string name)
        {
            var param = exposedParameters.Find(x => x.varRefBoxed != null && x.varRefBoxed.name == name);
            return param != null ? (param as ExposedParameter<T>).value : default;
        }

        ///<summary>Set an exposed parameter value</summary>
        public void SetExposedParameterValue<T>(string name, T value)
        {
            var param = exposedParameters?.Find(x => x.varRefBoxed != null && x.varRefBoxed.name == name);
            if (param == null) param = MakeNewExposedParameter<T>(name);
            if (param != null) (param as ExposedParameter<T>).value = value;
        }

        ///<summary>Make and return a new exposed parameter from a blackboard variable name and bind it</summary>
        public ExposedParameter MakeNewExposedParameter<T>(string name)
        {
            if (exposedParameters == null) exposedParameters = new List<ExposedParameter>();
            var variable = graph.blackboard.GetVariable<T>(name);
            if (variable != null && variable.isExposedPublic && !variable.isPropertyBound)
            {
                var exposedParam = ExposedParameter.CreateInstance(variable);
                exposedParam.Bind(graph.blackboard);
                exposedParameters.Add(exposedParam);
                return exposedParam;
            }

            Logger.LogWarning(string.Format("Exposed Graph Variable named '{0}' was not found", name));
            return null;
        }

        /// <summary>
        ///     Initialize the bound or asset graph. This is called in Awake automatically, but it's public so that you can
        ///     call this manually to pre-initialize when gameobject is deactive, if required.
        /// </summary>
        public void Initialize()
        {
            Debug.Assert(Application.isPlaying, "GraphOwner Initialize should have been called in runtime only");

            if (initialized) return;

            if (graph == null && !graphIsBound) return;

            GraphSource finalSource;
            string finalJson;
            List<Object> finalReferences;

            var newGraph = (Graph)ScriptableObject.CreateInstance(graphType);

            if (graphIsBound)
            {
                //Bound
                newGraph.name = graphType.Name;
                finalSource = boundGraphSource;
                finalJson = boundGraphSerialization;
                finalReferences = boundGraphObjectReferences;
                instances[newGraph] = newGraph;
            }
            else
            {
                //Asset reference
                newGraph.name = graph.name;
                finalSource = graph.GetGraphSource();
                finalJson = graph.GetSerializedJsonData();
                finalReferences = graph.GetSerializedReferencesData();
                instances[graph] = newGraph;
            }

            graph = newGraph;

            var loadData = new GraphLoadData();
            loadData.source = finalSource;
            loadData.json = finalJson;
            loadData.references = finalReferences;
            loadData.agent = this;
            loadData.parentBlackboard = blackboard;
            loadData.preInitializeSubGraphs = preInitializeSubGraphs;

            if (firstActivation == FirstActivation.Async)
            {
                graph.LoadOverwriteAsync(loadData, () =>
                {
                    BindExposedParameters();
                    //remark: activeInHierarchy is checked in case user instantiate and disable gameobject instantly for pooling reasons
                    if (!isRunning && enableAction == EnableAction.EnableBehaviour && gameObject.activeInHierarchy)
                    {
                        StartBehaviour();
                        InvokeStartEvent();
                    }

                    initialized = true;
                });
            }
            else
            {
                graph.LoadOverwrite(loadData);
                BindExposedParameters();
                initialized = true;
            }
        }

        ///<summary>Bind exposed parameters to local graph blackboard variables</summary>
        public void BindExposedParameters()
        {
            if (exposedParameters != null && graph != null)
                for (var i = 0; i < exposedParameters.Count; i++)
                    exposedParameters[i].Bind(graph.blackboard);
        }

        ///<summary>UnBind exposed parameters any local graph blackboard variables</summary>
        public void UnBindExposedParameters()
        {
            if (exposedParameters != null)
                for (var i = 0; i < exposedParameters.Count; i++)
                    exposedParameters[i].UnBind();
        }

        //This can actually be invoked in Start but if loading async it also needs to be called.
        //In either case, it's called only once.
        private void InvokeStartEvent()
        {
            //since "Start" is called once anyway we clear the event
            if (onMonoBehaviourStart != null)
            {
                onMonoBehaviourStart();
                onMonoBehaviourStart = null;
            }
        }


        /// ----------------------------------------------------------------------------------------------
        /// ---------------------------------------UNITY EDITOR-------------------------------------------
        /// ----------------------------------------------------------------------------------------------
#if UNITY_EDITOR
        protected Graph boundGraphInstance;

        ///<summary>Editor. Called after assigned graph is serialized.</summary>
        public void OnAfterGraphSerialized(Graph serializedGraph)
        {
            //If the graph is bound, we store the serialization data here.
            if (graphIsBound && boundGraphInstance == serializedGraph)
            {
                //---
                if (PrefabUtility.IsPartOfPrefabInstance(this))
                {
                    var boundProp = new SerializedObject(this).FindProperty(nameof(_boundGraphSerialization));
                    if (!boundProp.prefabOverride && boundGraphSerialization != serializedGraph.GetSerializedJsonData())
                    {
                        if (lockBoundGraphPrefabOverrides)
                        {
                            Logger.LogWarning(
                                "The Bound Graph is Prefab Locked!\nChanges you make are not saved!\nUnlock the Prefab Instance, or Edit the Prefab Asset.",
                                LogTag.EDITOR, this);
                            return;
                        }

                        Logger.LogWarning("Prefab Bound Graph just got overridden!", LogTag.EDITOR, this);
                    }
                }
                //---

                UndoUtility.RecordObject(this, UndoUtility.GetLastOperationNameOr("Bound Graph Change"));
                boundGraphSource = serializedGraph.GetGraphSource();
                boundGraphSerialization = serializedGraph.GetSerializedJsonData();
                boundGraphObjectReferences = serializedGraph.GetSerializedReferencesData();
                UndoUtility.SetDirty(this);
            }
        }

        ///<summary>Editor. Validate.</summary>
        protected void OnValidate()
        {
            Validate();
        }

        ///<summary>Editor. Validate.</summary>
        public void Validate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                //everything here is relevant to bound graphs only.
                //we only do this for when the object is an instance or is edited in the prefab editor.
                if (!EditorUtility.IsPersistent(this) && graphIsBound)
                {
                    if (boundGraphInstance == null)
                        boundGraphInstance = (Graph)ScriptableObject.CreateInstance(graphType);

                    boundGraphInstance.name = graphType.Name;
                    boundGraphInstance.SetGraphSourceMetaData(boundGraphSource);
                    boundGraphInstance.Deserialize(boundGraphSerialization, boundGraphObjectReferences, false);
                    boundGraphInstance.UpdateReferencesFromOwner(this);
                    boundGraphInstance.Validate();
                }
                else if (graph != null)
                {
                    graph.UpdateReferencesFromOwner(this);
                    graph.Validate();
                }

                // BindExposedParameters(); // DISABLE: was creating confusion when editing multiple graphowner instances using asset graphs and having different variable overrides
            }
        }

        ///<summary>Editor. Binds the target graph (null to delete current bound).</summary>
        public void SetBoundGraphReference(Graph target)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("SetBoundGraphReference method is an Editor only method!");
                return;
            }

            //cleanup
            graph = null;
            boundGraphInstance = null;
            if (target == null)
            {
                boundGraphSource = null;
                boundGraphSerialization = null;
                boundGraphObjectReferences = null;
                return;
            }

            //serialize target and store boundGraphSerialization data
            target.SelfSerialize();
            _boundGraphSerialization = target.GetSerializedJsonData();
            _boundGraphObjectReferences = target.GetSerializedReferencesData();
            _boundGraphSource = target.GetGraphSourceMetaDataCopy();
            Validate(); //validate to handle bound graph instance
        }

        ///<summary>Reset unity callback</summary>
        protected void Reset()
        {
            blackboard = gameObject.GetComponent<IBlackboard>();
            if (blackboard == null) blackboard = gameObject.AddComponent<Blackboard>();
        }

        //...
        protected void OnDrawGizmos()
        {
        }

        ///<summary>Forward Gizmos callback</summary>
        protected void OnDrawGizmosSelected()
        {
            if (GraphEditorUtility.activeElement != null)
            {
                var rootElement = GraphEditorUtility.activeElement.graph.GetFlatMetaGraph()
                    .FindReferenceElement(GraphEditorUtility.activeElement);
                if (rootElement != null)
                    foreach (var task in rootElement.GetAllChildrenReferencesOfType<Task>())
                        task.OnDrawGizmosSelected();
            }
        }
#endif
    }


    /// ----------------------------------------------------------------------------------------------
    /// <summary>The class where GraphOwners derive from</summary>
    public abstract class GraphOwner<T> : GraphOwner where T : Graph
    {
        [SerializeField] [Tooltip("The graph to use.")]
        private T _graph;

        [SerializeField] [Tooltip("The GameObject Blackboard to use.")]
        private Object _blackboard;

        ///<summary>The current behaviour Graph assigned</summary>
        public sealed override Graph graph
        {
            get
            {
#if UNITY_EDITOR
                //In Editor only and if graph is bound, return the bound graph instance
                if (graphIsBound && !Threader.applicationIsPlaying) return boundGraphInstance;
#endif
                //In runtime an instance of either boundGraphSerialization json or Asset Graph is created in awake
                return _graph;
            }
            set { _graph = (T)value; }
        }

        ///<summary>The current behaviour Graph assigned (same as .graph but of type T)</summary>
        public T behaviour
        {
            get => (T)graph;
            set => graph = value;
        }

        ///<summary>The blackboard that the assigned behaviour will be Started with or currently using</summary>
        public sealed override IBlackboard blackboard
        {
            //check != null to handle unity object when component is removed from inspector
            get => _blackboard != null ? _blackboard as IBlackboard : null;
            set
            {
                if (!ReferenceEquals(_blackboard, value))
                {
                    _blackboard = (Object)value;
                    if (graph != null) graph.UpdateReferences(this, value);
                }
            }
        }

        ///<summary>The Graph type this Owner can be assigned</summary>
        public sealed override Type graphType => typeof(T);

        ///<summary>Start a new behaviour on this owner</summary>
        public void StartBehaviour(T newGraph)
        {
            StartBehaviour(newGraph, updateMode);
        }

        ///<summary>Start a new behaviour on this owner and get a callback for when it's finished if at all</summary>
        public void StartBehaviour(T newGraph, Action<bool> callback)
        {
            StartBehaviour(newGraph, updateMode, callback);
        }

        /// <summary>
        ///     Start a new behaviour on this owner and optionally autoUpdated or not and optionally get a callback for when
        ///     it's finished if at all
        /// </summary>
        public void StartBehaviour(T newGraph, Graph.UpdateMode updateMode, Action<bool> callback = null)
        {
            SwitchBehaviour(newGraph, updateMode, callback);
        }

        ///<summary>Use to switch the behaviour dynamicaly at runtime</summary>
        public void SwitchBehaviour(T newGraph)
        {
            SwitchBehaviour(newGraph, updateMode);
        }

        ///<summary>Use to switch or set graphs at runtime and optionaly get a callback when it's finished if at all</summary>
        public void SwitchBehaviour(T newGraph, Action<bool> callback)
        {
            SwitchBehaviour(newGraph, updateMode, callback);
        }

        ///<summary>Use to switch or set graphs at runtime and optionaly get a callback when it's finished if at all</summary>
        public void SwitchBehaviour(T newGraph, Graph.UpdateMode updateMode, Action<bool> callback = null)
        {
            StopBehaviour();
            graph = newGraph;
            StartBehaviour(updateMode, callback);
        }
    }
}