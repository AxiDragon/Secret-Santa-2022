using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_TabAttribute))]
    public class FD_Tab : PropertyDrawer
    {
        private static GUIStyle _headerStyle;
        //private bool isUnfolded = false;

        private SerializedProperty foldProp;

        private Texture2D icon;
        private FPD_TabAttribute Attribute => (FPD_TabAttribute)attribute;

        public static GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle(EditorStyles.helpBox);
                    _headerStyle.fontStyle = FontStyle.Bold;
                    _headerStyle.alignment = TextAnchor.MiddleCenter;
                    _headerStyle.fontSize = 11;
                }

                return _headerStyle;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var c = GUI.color;
            var att = (FPD_TabAttribute)attribute;
            position.height = Attribute.IconSize + 4;

            if (icon == null)
            {
                if (string.IsNullOrEmpty(att.IconContent) == false)
                {
                    var gc = EditorGUIUtility.IconContent(att.IconContent);
                    if (gc != null)
                        if (gc.image != null)
                            icon = (Texture2D)gc.image;
                }

                if (string.IsNullOrEmpty(att.ResourcesIconPath) == false)
                    icon = Resources.Load<Texture2D>(att.ResourcesIconPath);
            }

            var pos = position;
            pos.y += 2;
            pos.height = 28 + att.IconSize - 24;

            GUI.color = new Color(att.R, att.G, att.B);
            GUI.BeginGroup(pos, FGUI_Resources.HeaderBoxStyle);
            GUI.EndGroup();
            GUI.color = c;

            if (!string.IsNullOrEmpty(att.FoldVariable))
                if (foldProp == null)
                    foldProp = property.serializedObject.FindProperty(att.FoldVariable);

            var folded = foldProp != null;
            var f = folded ? FGUI_Resources.GetFoldSimbol(foldProp.boolValue) : "";
            var header = folded ? f + "    " + Attribute.HeaderText + "    " + f : Attribute.HeaderText;
            //if (folded) isUnfolded = foldProp.boolValue;

            if (icon != null)
                if (GUI.Button(new Rect(pos.x + 4, pos.y + 3, att.IconSize, att.IconSize), new GUIContent(icon),
                        EditorStyles.label))
                {
                    if (foldProp != null) foldProp.boolValue = !foldProp.boolValue;
                    property.serializedObject.ApplyModifiedProperties();
                }

            if (GUI.Button(pos, new GUIContent(header), FGUI_Resources.HeaderStyle))
            {
                if (foldProp != null) foldProp.boolValue = !foldProp.boolValue;
                property.serializedObject.ApplyModifiedProperties();
            }

            if (icon != null)
                if (GUI.Button(new Rect(pos.width - att.IconSize + 9, pos.y + 3, att.IconSize, att.IconSize),
                        new GUIContent(icon), EditorStyles.label))
                {
                    if (foldProp != null) foldProp.boolValue = !foldProp.boolValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!string.IsNullOrEmpty(Attribute.FoldVariable))
                if (foldProp == null)
                    foldProp = property.serializedObject.FindProperty(Attribute.FoldVariable);

            //if ( foldProp != null) isUnfolded = foldProp.boolValue;

            //if (isUnfolded)
            return base.GetPropertyHeight(property, label) + Attribute.IconSize - 8;
            //else
            //    return Attribute.IconSize - 8;
        }
    }
}