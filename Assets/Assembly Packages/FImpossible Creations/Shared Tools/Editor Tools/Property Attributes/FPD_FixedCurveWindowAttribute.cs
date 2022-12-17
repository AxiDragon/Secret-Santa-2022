using UnityEngine;

public class FPD_FixedCurveWindowAttribute : PropertyAttribute
{
    public Color Color;
    public float EndTime;
    public float EndValue;
    public float StartTime;
    public float StartValue;

    public FPD_FixedCurveWindowAttribute(float startTime, float startValue, float endTime, float endValue, float r = 0f,
        float g = 1f, float b = 1f, float a = 1f)
    {
        StartTime = startTime;
        StartValue = startValue;
        EndTime = endTime;
        EndValue = endValue;
        Color = new Color(r, g, b, a);
    }
}