using System;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Blackboard")]
    public class CheckEnum : ConditionTask
    {
        [BlackboardOnly] public BBObjectParameter valueA = new(typeof(Enum));

        public BBObjectParameter valueB = new(typeof(Enum));

        protected override string info => valueA + " == " + valueB;

        protected override bool OnCheck()
        {
            return Equals(valueA.value, valueB.value);
        }

        /// ----------------------------------------------------------------------------------------------
        /// ---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();
            if (valueB.varType != valueA.refType) valueB.SetType(valueA.refType);
        }

#endif
    }
}