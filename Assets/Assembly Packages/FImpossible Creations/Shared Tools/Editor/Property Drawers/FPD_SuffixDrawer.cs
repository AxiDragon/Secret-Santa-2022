using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_SuffixAttribute))]
    public class FPD_Suffix : PropertyDrawer
    {
        private FPD_SuffixAttribute Attribute => (FPD_SuffixAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var sliderVal = property.floatValue;

            var suff = new GUIContent(Attribute.Suffix);
            var fieldS = EditorStyles.label.CalcSize(suff);

            var fieldSize = 34 + fieldS.x;
            var percField = new Rect(position.x + position.width - fieldSize + 5, position.y, fieldSize,
                position.height);
            var floatField = position;

            var editable = Attribute.editableValue;
            if (GUI.enabled == false) editable = false;

            if (editable)
            {
                floatField = new Rect(position.x + position.width - fieldSize + 2, position.y,
                    fieldSize - (fieldS.x + 4), position.height);
                percField.position = new Vector2(position.x + position.width - fieldS.x, percField.position.y);
                percField.width = fieldS.x;
            }

            position.width -= fieldSize + 3;
            sliderVal = GUI.HorizontalSlider(position, property.floatValue, Attribute.Min, Attribute.Max);

            float pre, value;

            switch (Attribute.Mode)
            {
                case FPD_SuffixAttribute.SuffixMode.From0to100:

                    if (!editable)
                    {
                        EditorGUI.LabelField(percField,
                            Mathf.Round(sliderVal / Attribute.Max * 100f) + Attribute.Suffix);
                    }
                    else
                    {
                        pre = Mathf.Round(sliderVal / Attribute.Max * 100f);
                        value = EditorGUI.FloatField(floatField, Mathf.Round(sliderVal / Attribute.Max * 100f));
                        if (value != pre) sliderVal = value / 100f;

                        EditorGUI.LabelField(percField, Attribute.Suffix);
                    }

                    break;

                case FPD_SuffixAttribute.SuffixMode.PercentageUnclamped:

                    if (!editable)
                    {
                        EditorGUI.LabelField(percField, Mathf.Round(sliderVal * 100f) + Attribute.Suffix);
                    }
                    else
                    {
                        pre = Mathf.Round(sliderVal * 100f);
                        value = EditorGUI.FloatField(floatField, Mathf.Round(sliderVal * 100f));
                        if (value != pre) sliderVal = value / 100f;

                        EditorGUI.LabelField(percField, Attribute.Suffix);
                    }

                    break;


                case FPD_SuffixAttribute.SuffixMode.FromMinToMax:

                    pre = sliderVal;
                    value = EditorGUI.FloatField(floatField, sliderVal);
                    if (value != pre) sliderVal = value;

                    EditorGUI.LabelField(percField, Attribute.Suffix);

                    break;

                case FPD_SuffixAttribute.SuffixMode.FromMinToMaxRounded:

                    pre = Mathf.Round(sliderVal);
                    value = EditorGUI.FloatField(floatField, Mathf.Round(sliderVal));
                    if (value != pre) sliderVal = value;

                    EditorGUI.LabelField(percField, Attribute.Suffix);

                    break;
            }

            property.floatValue = sliderVal;

            EditorGUI.EndProperty();
        }
    }
}