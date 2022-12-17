using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_FoldFrameAttribute))]
    public class FPD_FoldFrame : PropertyDrawer
    {
        private FPD_FoldFrameAttribute Attribute => (FPD_FoldFrameAttribute)attribute;

        //private bool folded = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var props = new SerializedProperty[Attribute.VariablesToStore.Length];

            for (var i = 0; i < props.Length; i++)
                props[i] = property.serializedObject.FindProperty(Attribute.VariablesToStore[i]);

            GUILayout.BeginVertical(FGUI_Inspector.Style(new Color32(250, 250, 250, 75)));
            EditorGUI.indentLevel++;

            var foldBold = EditorStyles.foldout;
            foldBold.fontStyle = FontStyle.Bold;
            Attribute.Folded = EditorGUILayout.Foldout(Attribute.Folded, " " + Attribute.FrameTitle, true, foldBold);

            if (Attribute.Folded)
                for (var i = 0; i < props.Length; i++)
                    if (props[i] != null)
                        EditorGUILayout.PropertyField(props[i]);
                    else
                        EditorGUILayout.LabelField("Wrong property name?");

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var size = -EditorGUIUtility.singleLineHeight / 5f;
            return size;
        }
    }
}