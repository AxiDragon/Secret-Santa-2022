using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
    [Obsolete("Use Jumpers instead")]
    public class GoToNode : DTNode
    {
        [SerializeField] private readonly DTNode _targetNode = null;

        public override int maxOutConnections => 0;
        public override bool requireActorSelection => false;

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            if (_targetNode == null) return Error("Target node of GOTO node is null");

            DLGTree.EnterNode(_targetNode);
            return Status.Success;
        }
    }
}