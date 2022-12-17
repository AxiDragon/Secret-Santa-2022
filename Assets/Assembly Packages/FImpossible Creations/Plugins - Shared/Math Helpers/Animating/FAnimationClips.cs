using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    ///     FM: Basic class for holding animation clips as hashes inside dictionary and with some other handy methods
    ///     Add your animation clips with fAnimationClip.AddClip(animator, Idle) then use it by fAnimationClip["Idle"]
    /// </summary>
    public class FAnimationClips : Dictionary<string, int>
    {
        /// <summary> Reference to Unity's Animator component </summary>
        public readonly Animator Animator;

        public int Layer = 0;

        /// <summary>
        ///     ADD ANIMATION CLIPS USING - AddClip() - method !!!
        /// </summary>
        public FAnimationClips(Animator animator)
        {
            Animator = animator;
            CurrentAnimation = "";
            PreviousAnimation = "";
        }

        /// <summary> Lastest crossfaded from code animation clip name </summary>
        public string CurrentAnimation { get; private set; }

        /// <summary> Previously crossfaded from code animation clip name </summary>
        public string PreviousAnimation { get; private set; }

        /// <summary>
        ///     Checking if inside animator clipName state exist and with 'exactClipName' false it will check all variants of
        ///     clipName, uppercase / lowercase etc.
        /// </summary>
        public void Add(string clipName, bool exactClipName = false)
        {
            AddClip(clipName, exactClipName);
        }

        /// <summary>
        ///     Checking if inside animator clipName state exist and with 'exactClipName' false it will check all variants of
        ///     clipName, uppercase / lowercase etc.
        /// </summary>
        public void AddClip(string clipName, bool exactClipName = false)
        {
            AddClip(Animator, clipName, exactClipName);
        }

        /// <summary>
        ///     Checking if inside animator clipName state exist and with 'exactClipName' false it will check all variants of
        ///     clipName, uppercase / lowercase etc.
        /// </summary>
        public void AddClip(Animator animator, string clipName, bool exactClipName = false)
        {
            if (!animator)
            {
                Debug.LogError("No animator!");
                return;
            }

            var existing = "";

            if (!exactClipName) // Checking if animation state exists with different variants for clipName word
            {
                if (animator.StateExists(clipName, Layer)) existing = clipName;
                else if (animator.StateExists(clipName.CapitalizeFirstLetter()))
                    existing = clipName.CapitalizeFirstLetter();
                else if (animator.StateExists(clipName.ToLower(), Layer)) existing = clipName.ToLower();
                else if (animator.StateExists(clipName.ToUpper(), Layer)) existing = clipName.ToUpper();
            }
            else // Checking if state with provided exact name exists inside animator
            {
                if (animator.StateExists(clipName, Layer)) existing = clipName;
            }

            if (existing == "")
            {
                Debug.LogWarning("Clip with name " + clipName + " not exists in animator from game object " +
                                 animator.gameObject.name);
            }
            else // Adding clip hash to dictionary if it exists inside animator
            {
                if (!ContainsKey(clipName))
                    Add(clipName, Animator.StringToHash(existing));
            }
        }

        /// <summary>
        ///     Transitioning to choosed animation by dictionary
        /// </summary>
        public void CrossFadeInFixedTime(string clip, float transitionTime = 0.25f, float timeOffset = 0f,
            bool startOver = false)
        {
            if (ContainsKey(clip))
            {
                RefreshClipMemory(clip);

                if (startOver)
                    Animator.CrossFadeInFixedTime(this[clip], transitionTime, Layer, timeOffset);
                else if (!IsPlaying(clip))
                    Animator.CrossFadeInFixedTime(this[clip], transitionTime, Layer, timeOffset);
            }
        }

        /// <summary>
        ///     Transitioning to choosed animation by dictionary
        /// </summary>
        public void CrossFade(string clip, float transitionTime = 0.25f, float timeOffset = 0f, bool startOver = false)
        {
            if (ContainsKey(clip))
            {
                RefreshClipMemory(clip);

                if (startOver)
                    Animator.CrossFade(this[clip], transitionTime, Layer, timeOffset);
                else if (!IsPlaying(clip))
                    Animator.CrossFade(this[clip], transitionTime, Layer, timeOffset);
            }
        }

        /// <summary>
        ///     Storing lately and currently played clip in variables
        /// </summary>
        private void RefreshClipMemory(string name)
        {
            if (name != CurrentAnimation)
            {
                PreviousAnimation = CurrentAnimation;
                CurrentAnimation = name;
            }
        }

        /// <summary>
        ///     Changing float parameter value smoothly (when speed value about 10, 60 is instant)
        /// </summary>
        public void SetFloat(string parameter, float value = 0f, float deltaSpeed = 60f)
        {
            var newValue = Animator.GetFloat(parameter);
            if (deltaSpeed >= 60f) newValue = value;
            else newValue = FLogicMethods.FLerp(newValue, value, Time.deltaTime * deltaSpeed);
            Animator.SetFloat(parameter, newValue);
        }

        /// <summary>
        ///     Changing float parameter value smoothly (when speed value about 10, 60 is instant)
        /// </summary>
        public void SetFloatUnscaledDelta(string parameter, float value = 0f, float deltaSpeed = 60f)
        {
            var newValue = Animator.GetFloat(parameter);
            if (deltaSpeed >= 60f) newValue = value;
            else newValue = FLogicMethods.FLerp(newValue, value, Time.unscaledDeltaTime * deltaSpeed);
            Animator.SetFloat(parameter, newValue);
        }

        /// <summary>
        ///     If animator is truly playing clip with given string id (added by you using clips.AddClip(name) )
        /// </summary>
        internal bool IsPlaying(string clip)
        {
            AnimatorStateInfo info;
            if (Animator.IsInTransition(Layer))
            {
                info = Animator.GetNextAnimatorStateInfo(Layer);
                if (info.shortNameHash == this[clip]) return true;
            }
            else
            {
                info = Animator.GetCurrentAnimatorStateInfo(Layer);
                if (info.shortNameHash == this[clip]) return true;
            }

            return false;
        }
    }


    public class FAnimator
    {
        /// <summary> Reference to Unity's Animator component </summary>
        public readonly Animator Animator;

        /// <summary>
        ///     ADD ANIMATION CLIPS USING - AddClip() - method !!!
        /// </summary>
        public FAnimator(Animator animator, int layer = 0)
        {
            Animator = animator;
            CurrentAnimation = "";
            PreviousAnimation = "";
            Layer = layer;
        }

        /// <summary> Lastest crossfaded from code animation clip name </summary>
        public string CurrentAnimation { get; private set; }

        /// <summary> Previously crossfaded from code animation clip name </summary>
        public string PreviousAnimation { get; private set; }

        public int Layer { get; }


        /// <summary>
        ///     Checking if inside animator clipName state exist and with 'exactClipName' false it will check all variants of
        ///     clipName, uppercase / lowercase etc.
        /// </summary>
        public bool ContainsClip(string clipName, bool exactClipName = false)
        {
            if (!Animator)
            {
                Debug.LogError("No animator!");
                return false;
            }

            var existing = "";

            if (!exactClipName) // Checking if animation state exists with different variants for clipName word
            {
                if (Animator.StateExists(clipName, Layer)) existing = clipName;
                else if (Animator.StateExists(clipName.CapitalizeFirstLetter()))
                    existing = clipName.CapitalizeFirstLetter();
                else if (Animator.StateExists(clipName.ToLower(), Layer)) existing = clipName.ToLower();
                else if (Animator.StateExists(clipName.ToUpper(), Layer)) existing = clipName.ToUpper();
            }
            else // Checking if state with provided exact name exists inside animator
            {
                if (Animator.StateExists(clipName, Layer)) existing = clipName;
            }

            if (existing == "")
            {
                Debug.LogWarning("Clip with name " + clipName + " not exists in animator from game object " +
                                 Animator.gameObject.name);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Transitioning to choosed animation by dictionary
        /// </summary>
        public void CrossFadeInFixedTime(string clip, float transitionTime = 0.25f, float timeOffset = 0f,
            bool startOver = false)
        {
            RefreshClipMemory(clip);

            if (startOver)
                Animator.CrossFadeInFixedTime(clip, transitionTime, Layer, timeOffset);
            else if (!IsPlaying(clip))
                Animator.CrossFadeInFixedTime(clip, transitionTime, Layer, timeOffset);
        }

        /// <summary>
        ///     Transitioning to choosed animation by dictionary
        /// </summary>
        public void CrossFade(string clip, float transitionTime = 0.25f, float timeOffset = 0f, bool startOver = false)
        {
            RefreshClipMemory(clip);

            if (startOver)
                Animator.CrossFade(clip, transitionTime, Layer, timeOffset);
            else if (!IsPlaying(clip))
                Animator.CrossFade(clip, transitionTime, Layer, timeOffset);
        }

        /// <summary>
        ///     Storing lately and currently played clip in variables
        /// </summary>
        private void RefreshClipMemory(string name)
        {
            if (name != CurrentAnimation)
            {
                PreviousAnimation = CurrentAnimation;
                CurrentAnimation = name;
            }
        }

        /// <summary>
        ///     Changing float parameter value smoothly (when speed value about 10, 60 is instant)
        /// </summary>
        public void SetFloat(string parameter, float value = 0f, float deltaSpeed = 60f)
        {
            var newValue = Animator.GetFloat(parameter);
            if (deltaSpeed >= 60f) newValue = value;
            else newValue = FLogicMethods.FLerp(newValue, value, Time.deltaTime * deltaSpeed);
            Animator.SetFloat(parameter, newValue);
        }

        /// <summary>
        ///     Changing float parameter value smoothly (when speed value about 10, 60 is instant)
        /// </summary>
        public void SetFloatUnscaledDelta(string parameter, float value = 0f, float deltaSpeed = 60f)
        {
            var newValue = Animator.GetFloat(parameter);
            if (deltaSpeed >= 60f) newValue = value;
            else newValue = FLogicMethods.FLerp(newValue, value, Time.unscaledDeltaTime * deltaSpeed);
            Animator.SetFloat(parameter, newValue);
        }

        /// <summary>
        ///     If animator is truly playing clip with given string id (added by you using clips.AddClip(name) )
        /// </summary>
        internal bool IsPlaying(string clip)
        {
            AnimatorStateInfo info;
            if (Animator.IsInTransition(Layer))
            {
                info = Animator.GetNextAnimatorStateInfo(Layer);
                if (info.shortNameHash == Animator.StringToHash(clip)) return true;
            }
            else
            {
                info = Animator.GetCurrentAnimatorStateInfo(Layer);
                if (info.shortNameHash == Animator.StringToHash(clip)) return true;
            }

            return false;
        }
    }
}