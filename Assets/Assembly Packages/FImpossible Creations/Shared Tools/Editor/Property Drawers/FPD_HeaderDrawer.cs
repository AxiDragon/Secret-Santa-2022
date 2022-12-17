using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_HeaderAttribute))]
    public class FD_Header : DecoratorDrawer
    {
        private static GUIStyle _headerStyle;

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

        public override void OnGUI(Rect position)
        {
            var att = (FPD_HeaderAttribute)attribute;

            var pos = position;
            pos.height = base.GetHeight() + att.Height;

            pos.y += att.UpperPadding;

            GUI.Label(pos, new GUIContent(att.HeaderText), HeaderStyle);
        }

        public override float GetHeight()
        {
            var att = (FPD_HeaderAttribute)attribute;
            return base.GetHeight() + att.Height + att.BottomPadding + att.UpperPadding;
        }
    }
}