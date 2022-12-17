using System;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

[Category("✫ Blackboard")]
public class CheckEnumFlag : ConditionTask
{
    public readonly BBObjectParameter Flag = new(typeof(Enum));

    [BlackboardOnly] [RequiredField] public readonly BBObjectParameter Variable = new(typeof(Enum));

    protected override string info => $"{Variable} has {Flag} flag";

    protected override bool OnCheck()
    {
        return ((Enum)Variable.value).HasFlag((Enum)Flag.value);
    }

#if UNITY_EDITOR

    protected override void OnTaskInspectorGUI()
    {
        DrawDefaultInspector();

        if (Flag.varType != Variable.refType) Flag.SetType(Variable.refType);
    }

#endif
}