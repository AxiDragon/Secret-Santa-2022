using NodeCanvas.Editor;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.StateMachines
{
    ///<summary>The connection object for FSM nodes. AKA Transitions</summary>
    public class FSMConnection : Connection, ITaskAssignable<ConditionTask>
    {
        [SerializeField] private ConditionTask _condition;

        [SerializeField] private FSM.TransitionCallMode _transitionCallMode;

        public ConditionTask condition
        {
            get => _condition;
            set => _condition = value;
        }

        public FSM.TransitionCallMode transitionCallMode
        {
            get => _transitionCallMode;
            private set => _transitionCallMode = value;
        }

        public Task task
        {
            get => condition;
            set => condition = (ConditionTask)value;
        }

        //...
        public void EnableCondition(Component agent, IBlackboard blackboard)
        {
            if (condition != null) condition.Enable(agent, blackboard);
        }

        //...
        public void DisableCondition()
        {
            if (condition != null) condition.Disable();
        }

        ///<summary>Perform the transition disregarding whether or not the condition (if any) is valid</summary>
        public void PerformTransition()
        {
            (graph as FSM).EnterState((FSMState)targetNode, transitionCallMode);
        }


        /// ----------------------------------------------------------------------------------------------
        /// ---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override PlanarDirection direction
        {
            get { return PlanarDirection.Auto; }
        }

        public override TipConnectionStyle tipConnectionStyle => TipConnectionStyle.Arrow;

        public override bool animate => status == Status.Failure;

        protected override string GetConnectionInfo()
        {
            var result = transitionCallMode == FSM.TransitionCallMode.Normal
                ? string.Empty
                : string.Format("<b>[{0}]</b>\n", transitionCallMode.ToString());
            result += condition != null ? condition.summaryInfo : "OnFinish";
            return result;
        }

        protected override void OnConnectionInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Stacked Call Mode will push the current state to the stack and pop return to it later when any other state without outgoing transitions has been encountered. If you decide to use this feature make sure that you are not cycle stacking states.",
                MessageType.None);
            transitionCallMode =
                (FSM.TransitionCallMode)EditorGUILayout.EnumPopup("Call Mode (Experimental)", transitionCallMode);
            EditorUtils.Separator();
            TaskEditor.TaskFieldMulti(condition, graph, c => { condition = c; });
        }

#endif
    }
}