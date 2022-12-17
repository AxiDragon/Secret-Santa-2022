using UnityEngine;

public class FPD_SuffixAttribute : PropertyAttribute
{
    public enum SuffixMode
    {
        From0to100,
        PercentageUnclamped,
        FromMinToMax,
        FromMinToMaxRounded
    }

    public readonly bool editableValue;
    public readonly float Max;
    public readonly float Min;
    public readonly SuffixMode Mode;
    public readonly string Suffix;
    public readonly int widerField;

    // °
    public FPD_SuffixAttribute(float min, float max, SuffixMode mode = SuffixMode.From0to100, string suffix = "%",
        bool editable = true, int wider = 0)
    {
        Min = min;
        Max = max;
        Mode = mode;
        Suffix = suffix;
        editableValue = editable;
        widerField = wider;
    }
}