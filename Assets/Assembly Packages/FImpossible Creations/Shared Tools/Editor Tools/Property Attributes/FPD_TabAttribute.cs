using UnityEngine;

public class FPD_TabAttribute : PropertyAttribute
{
    public float B;
    public string FoldVariable;
    public float G;
    public string HeaderText;
    public string IconContent;
    public int IconSize;
    public float R;
    public string ResourcesIconPath;

    public FPD_TabAttribute(string headerText, float r = 0.5f, float g = 0.5f, float b = 1f, string iconContent = "",
        string resourcesIconPath = "", int iconSize = 24, string foldVariable = "")
    {
        HeaderText = headerText;
        R = r;
        G = g;
        B = b;
        IconContent = iconContent;
        ResourcesIconPath = resourcesIconPath;
        IconSize = iconSize;
        FoldVariable = foldVariable;
    }
}