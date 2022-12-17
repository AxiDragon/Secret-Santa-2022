using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
    [Name("Task Action")]
    [Description("Execute an Action Task for the Dialogue Actor selected.")]
    public class ActionNode : DTNode, ITaskAssignable<ActionTask>
    {
        [SerializeField] private ActionTask _action;

        public ActionTask action
        {
            get => _action;
            set => _action = value;
        }

        public override bool requireActorSelection => true;

        public Task task
        {
            get => action;
            set => action = (ActionTask)value;
        }

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            if (action == null) return Error("Action is null on Dialogue Action Node");

            status = Status.Running;
            StartCoroutine(UpdateAction(finalActor.transform));
            return status;
        }

        private IEnumerator UpdateAction(Component actionAgent)
        {
            while (status == Status.Running)
            {
                var actionStatus = action.Execute(actionAgent, graphBlackboard);
                if (actionStatus != Status.Running)
                {
                    OnActionEnd(actionStatus == Status.Success ? true : false);
                    yield break;
                }

                yield return null;
            }
        }

        private void OnActionEnd(bool success)
        {
            if (success)
            {
                status = Status.Success;
                DLGTree.Continue();
                return;
            }

            status = Status.Failure;
            DLGTree.Stop(false);
        }

        protected override void OnReset()
        {
            if (action != null)
                action.EndAction(null);
        }

        public override void OnGraphPaused()
        {
            if (action != null)
                action.Pause();
        }
    }
}