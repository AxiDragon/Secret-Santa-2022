using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_OverridableFloatAttribute))]
    public class FPD_OverridableFloat : PropertyDrawer
    {
        private FPD_OverridableFloatAttribute Attribute => (FPD_OverridableFloatAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var boolProp = property.serializedObject.FindProperty(Attribute.BoolVarName);
            var valProp = property.serializedObject.FindProperty(Attribute.TargetVarName);

            var disabled = new Color(0.8f, 0.8f, 0.8f, 0.6f);
            var preCol = GUI.color;
            if (!boolProp.boolValue) GUI.color = disabled;
            else GUI.color = preCol;

            EditorGUI.BeginProperty(position, label, property);

            var boolRect = new Rect(position.x, position.y, Attribute.LabelWidth + 15f, position.height);

            EditorGUIUtility.labelWidth = Attribute.LabelWidth;
            EditorGUI.PrefixLabel(position, label);
            EditorGUI.PropertyField(boolRect, boolProp);

            EditorGUIUtility.labelWidth = 14;
            var valRect = new Rect(position.x + Attribute.LabelWidth + 15, position.y,
                position.width - (Attribute.LabelWidth + 15), position.height);
            EditorGUI.PropertyField(valRect, valProp, new GUIContent(" "));

            EditorGUIUtility.labelWidth = 0;

            GUI.color = preCol;
            EditorGUI.EndProperty();
        }
    }


    // -------------------------- Next F Property Drawer -------------------------- \\


    [CustomPropertyDrawer(typeof(BackgroundColorAttribute))]
    public class BackgroundColorDecorator : DecoratorDrawer
    {
        private BackgroundColorAttribute Attribute => (BackgroundColorAttribute)attribute;

        public override float GetHeight()
        {
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            GUI.backgroundColor = Attribute.Color;
        }
    }


    // -------------------------- Next F Property Drawer -------------------------- \\


    [CustomPropertyDrawer(typeof(FPD_WidthAttribute))]
    public class FPD_Width : PropertyDrawer
    {
        private FPD_WidthAttribute Attribute => (FPD_WidthAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = Attribute.LabelWidth;
            EditorGUI.PrefixLabel(position, label);
            EditorGUI.PropertyField(position, property);
            EditorGUIUtility.labelWidth = 0;
        }
    }

    // -------------------------- Next F Property Drawer -------------------------- \\

    [CustomPropertyDrawer(typeof(FPD_IndentAttribute))]
    public class FPD_Indent : PropertyDrawer
    {
        private FPD_IndentAttribute Attribute => (FPD_IndentAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = Attribute.LabelsWidth;
            for (var i = 0; i < Attribute.IndentCount; i++) EditorGUI.indentLevel++;
            EditorGUI.PrefixLabel(position, label);
            EditorGUI.PropertyField(position, property);
            for (var i = 0; i < Attribute.IndentCount; i++) EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = 0;
            GUILayout.Space(Attribute.SpaceAfter);
        }
    }

    // -------------------------- Next F Property Drawer -------------------------- \\

    [CustomPropertyDrawer(typeof(FPD_HorizontalLineAttribute))]
    public class FPD_HorizontalLine : PropertyDrawer
    {
        private FPD_HorizontalLineAttribute Attribute => (FPD_HorizontalLineAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FGUI_Inspector.DrawUILine(Attribute.color);
        }
    }
}