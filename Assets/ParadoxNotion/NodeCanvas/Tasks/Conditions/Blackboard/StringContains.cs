﻿using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Blackboard")]
    public class StringContains : ConditionTask
    {
        public BBParameter<string> checkString;

        [RequiredField] [BlackboardOnly] public BBParameter<string> targetString;

        protected override string info => string.Format("{0} Contains {1}", targetString, checkString);

        protected override bool OnCheck()
        {
            return targetString.value.Contains(checkString.value);
        }
    }
}