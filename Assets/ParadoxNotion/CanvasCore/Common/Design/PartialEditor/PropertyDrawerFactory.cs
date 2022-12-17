﻿#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ParadoxNotion.Design
{
    ///<summary>Provides object and attribute property drawers</summary>
    public static class PropertyDrawerFactory
    {
        //Type to drawer instance map
        private static Dictionary<Type, IObjectDrawer> objectDrawers = new();
        private static Dictionary<Type, IAttributeDrawer> attributeDrawers = new();

        public static void FlushMem()
        {
            objectDrawers = new Dictionary<Type, IObjectDrawer>();
            attributeDrawers = new Dictionary<Type, IAttributeDrawer>();
        }

        ///<summary>Return an object drawer instance of target inspected type</summary>
        public static IObjectDrawer GetObjectDrawer(Type objectType)
        {
            IObjectDrawer result = null;
            if (objectDrawers.TryGetValue(objectType, out result)) return result;

            // look for specific drawer first
            Type fallbackDrawerType = null;
            foreach (var drawerType in ReflectionTools.GetImplementationsOf(typeof(IObjectDrawer)))
                if (drawerType != typeof(DefaultObjectDrawer))
                {
                    var args = drawerType.BaseType.RTGetGenericArguments();
                    if (args.Length == 1)
                    {
                        if (args[0].IsEquivalentTo(objectType))
                            return objectDrawers[objectType] = Activator.CreateInstance(drawerType) as IObjectDrawer;
                        if (args[0].IsAssignableFrom(objectType)) fallbackDrawerType = drawerType;
                    }
                }

            if (fallbackDrawerType != null)
                return objectDrawers[objectType] = Activator.CreateInstance(fallbackDrawerType) as IObjectDrawer;


            // foreach ( var drawerType in ReflectionTools.GetImplementationsOf(typeof(IObjectDrawer)) ) {
            //     if ( drawerType != typeof(DefaultObjectDrawer) ) {
            //         var args = drawerType.BaseType.RTGetGenericArguments();
            //         if ( args.Length == 1 && args[0].IsAssignableFrom(objectType) ) {
            //             return objectDrawers[objectType] = Activator.CreateInstance(drawerType) as IObjectDrawer;
            //         }
            //     }
            // }

            return objectDrawers[objectType] = new DefaultObjectDrawer(objectType);
        }

        ///<summary>Return an attribute drawer instance of target attribute instance</summary>
        public static IAttributeDrawer GetAttributeDrawer(DrawerAttribute att)
        {
            return GetAttributeDrawer(att.GetType());
        }

        ///<summary>Return an attribute drawer instance of target attribute type</summary>
        public static IAttributeDrawer GetAttributeDrawer(Type attributeType)
        {
            IAttributeDrawer result = null;
            if (attributeDrawers.TryGetValue(attributeType, out result)) return result;

            foreach (var drawerType in ReflectionTools.GetImplementationsOf(typeof(IAttributeDrawer)))
                if (drawerType != typeof(DefaultAttributeDrawer))
                {
                    var args = drawerType.BaseType.RTGetGenericArguments();
                    if (args.Length == 1 && args[0].IsAssignableFrom(attributeType))
                        return attributeDrawers[attributeType] =
                            Activator.CreateInstance(drawerType) as IAttributeDrawer;
                }

            return attributeDrawers[attributeType] = new DefaultAttributeDrawer(attributeType);
        }
    }

    ///----------------------------------------------------------------------------------------------
    public interface IObjectDrawer
    {
        object DrawGUI(GUIContent content, object instance, InspectedFieldInfo info);
        object MoveNextDrawer();
    }

    public interface IAttributeDrawer
    {
        object DrawGUI(IObjectDrawer objectDrawer, GUIContent content, object instance, DrawerAttribute attribute,
            InspectedFieldInfo info);
    }

    /// ----------------------------------------------------------------------------------------------
    /// <summary>Derive this to create custom drawers for T assignable object types.</summary>
    public abstract class ObjectDrawer<T> : IObjectDrawer
    {
        ///<summary>info</summary>
        protected InspectedFieldInfo info { get; private set; }

        ///<summary>The GUIContent</summary>
        protected GUIContent content { get; private set; }

        ///<summary>The instance of the object being drawn</summary>
        protected T instance { get; private set; }

        ///<summary>The set of Drawer Attributes found on field</summary>
        protected DrawerAttribute[] attributes { get; private set; }

        ///<summary>Current attribute index drawn</summary>
        private int attributeIndex { get; set; }

        ///<summary>The reflected FieldInfo representation</summary>
        protected FieldInfo fieldInfo => info.field;

        ///<summary>The parent object the instance is drawn within</summary>
        protected object context => info.parentInstanceContext;

        ///<summary>The Unity object the instance serialized within</summary>
        protected Object contextUnityObject => info.unityObjectContext;


        ///<summary>Begin GUI</summary>
        object IObjectDrawer.DrawGUI(GUIContent content, object instance, InspectedFieldInfo info)
        {
            this.content = content;
            this.instance = (T)instance;
            this.info = info;

            attributes = info.attributes != null
                ? info.attributes.OfType<DrawerAttribute>().OrderBy(a => a.priority).ToArray()
                : null;

            attributeIndex = -1;
            var result = (this as IObjectDrawer).MoveNextDrawer();

            // //flush references
            this.info = default;
            this.content = null;
            this.instance = default;
            attributes = null;

            return result;
        }

        ///<summary>Show the next attribute drawer in order, or the object drawer itself of no attribute drawer is left to show.</summary>
        object IObjectDrawer.MoveNextDrawer()
        {
            attributeIndex++;
            if (attributes != null && attributeIndex < attributes.Length)
            {
                var currentDrawerAttribute = attributes[attributeIndex];
                var drawer = PropertyDrawerFactory.GetAttributeDrawer(currentDrawerAttribute);
                return drawer.DrawGUI(this, content, instance, currentDrawerAttribute, info);
            }

            return OnGUI(content, instance);
        }

        ///<summary>Override to implement GUI. Return the modified instance at the end.</summary>
        public abstract T OnGUI(GUIContent content, T instance);
    }

    ///<summary>The default object drawer implementation able to inspect most types</summary>
    public class DefaultObjectDrawer : ObjectDrawer<object>
    {
        private readonly Type objectType;

        public DefaultObjectDrawer(Type objectType)
        {
            this.objectType = objectType;
        }

        public override object OnGUI(GUIContent content, object instance)
        {
            return EditorUtils.DrawEditorFieldDirect(content, instance, objectType, info);
        }
    }

    /// ----------------------------------------------------------------------------------------------
    /// <summary>Derive this to create custom drawers for T DrawerAttribute.</summary>
    public abstract class AttributeDrawer<T> : IAttributeDrawer where T : DrawerAttribute
    {
        ///<summary>info</summary>
        protected InspectedFieldInfo info { get; private set; }

        ///<summary>The GUIContent</summary>
        protected GUIContent content { get; private set; }

        ///<summary>The instance of the object being drawn</summary>
        protected object instance { get; private set; }

        /// <summary>The reflected FieldInfo representation</summary>
        /// <summary>The attribute instance</summary>
        protected T attribute { get; private set; }

        ///<summary>The ObjectDrawer currently in use</summary>
        protected IObjectDrawer objectDrawer { get; private set; }

        protected FieldInfo fieldInfo => info.field;

        ///<summary>The parent object the instance is drawn within</summary>
        protected object context => info.parentInstanceContext;

        ///<summary>The Unity object the instance serialized within</summary>
        protected Object contextUnityObject => info.unityObjectContext;

        ///<summary>Begin GUI</summary>
        object IAttributeDrawer.DrawGUI(IObjectDrawer objectDrawer, GUIContent content, object instance,
            DrawerAttribute attribute, InspectedFieldInfo info)
        {
            this.objectDrawer = objectDrawer;
            this.content = content;
            this.instance = instance;
            this.attribute = (T)attribute;

            this.info = info;
            var result = OnGUI(content, instance);

            //flush references
            this.info = default;
            this.content = null;
            this.instance = null;
            this.attribute = null;
            this.objectDrawer = null;

            return result;
        }

        ///<summary>Override to implement GUI. Return the modified instance at the end.</summary>
        public abstract object OnGUI(GUIContent content, object instance);

        ///<summary>Show the next attribute drawer in order, or the object drawer itself of no attribute drawer is left to show.</summary>
        protected object MoveNextDrawer()
        {
            return objectDrawer.MoveNextDrawer();
        }
    }

    ///<summary>The default attribute drawer implementation for when an actual implementation is not found</summary>
    public class DefaultAttributeDrawer : AttributeDrawer<DrawerAttribute>
    {
        private readonly Type attributeType;

        public DefaultAttributeDrawer(Type attributeType)
        {
            this.attributeType = attributeType;
        }

        public override object OnGUI(GUIContent content, object instance)
        {
            GUILayout.Label(string.Format("Implementation of '{0}' drawer attribute not found.", attributeType));
            return MoveNextDrawer();
        }
    }
}

#endif