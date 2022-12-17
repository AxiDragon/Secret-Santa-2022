﻿using System;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Framework
{
    /// <summary>
    ///     A Signal definition that things can listen to. Can also be invoked in code by calling 'Invoke' but args have
    ///     to be same type and same length as the parameters defined.
    /// </summary>
    [CreateAssetMenu(menuName = "ParadoxNotion/CanvasCore/Signal Definition")]
    public class SignalDefinition : ScriptableObject
    {
        public delegate void InvokeArguments(Transform sender, Transform receiver, bool isGlobal, params object[] args);

        [SerializeField] [HideInInspector] private List<DynamicParameterDefinition> _parameters = new();

        ///<summary>The Signal parameters</summary>
        public List<DynamicParameterDefinition> parameters
        {
            get => _parameters;
            private set => _parameters = value;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Editor_Init()
        {
            AssetTracker.BeginTrackingAssetsOfType(typeof(SignalDefinition));
        }
#endif
        public event InvokeArguments onInvoke;

        ///<summary>Invoke the Signal</summary>
        public void Invoke(Transform sender, Transform receiver, bool isGlobal, params object[] args)
        {
            if (onInvoke != null) onInvoke(sender, receiver, isGlobal, args);
        }

        //...
        public void AddParameter(string name, Type type)
        {
            var param = new DynamicParameterDefinition(name, type);
            _parameters.Add(param);
        }

        //...
        public void RemoveParameter(string name)
        {
            var param = _parameters.Find(p => p.name == name);
            if (param != null) _parameters.Remove(param);
        }
    }
}