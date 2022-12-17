﻿using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Blackboard")]
    public class CheckInt : ConditionTask
    {
        public CompareMethod checkType = CompareMethod.EqualTo;

        [BlackboardOnly] public BBParameter<int> valueA;

        public BBParameter<int> valueB;

        protected override string info => valueA + OperationTools.GetCompareString(checkType) + valueB;

        protected override bool OnCheck()
        {
            return OperationTools.Compare(valueA.value, valueB.value, checkType);
        }
    }
}