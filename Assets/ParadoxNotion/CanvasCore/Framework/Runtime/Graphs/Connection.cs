using System;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.Framework
{
#if UNITY_EDITOR //handles missing types
    [fsObject(Processor = typeof(fsRecoveryProcessor<Connection, MissingConnection>))]
#endif

    ///<summary>Base class for connections between nodes in a graph</summary>
    [SpoofAOT]
    [Serializable]
    [fsDeserializeOverwrite]
    public abstract partial class Connection : IGraphElement, ISerializationCollectable
    {
        [SerializeField] private string _UID;
        [SerializeField] private bool _isDisabled;

        [SerializeField] [fsSerializeAsReference]
        private Node _sourceNode;

        [NonSerialized] private Status _status = Status.Resting;

        [SerializeField] [fsSerializeAsReference]
        private Node _targetNode;

        ///----------------------------------------------------------------------------------------------

        //required
        public Connection()
        {
        }

        ///<summary>The source node of the connection</summary>
        public Node sourceNode
        {
            get => _sourceNode;
            protected set => _sourceNode = value;
        }

        ///<summary>The target node of the connection</summary>
        public Node targetNode
        {
            get => _targetNode;
            protected set => _targetNode = value;
        }

        ///<summary>Is the connection active?</summary>
        public bool isActive
        {
            get => !_isDisabled;
            set
            {
                if (!_isDisabled && value == false) Reset();
                _isDisabled = !value;
            }
        }

        ///<summary>The Unique ID of the node. One is created only if requested.</summary>
        public string UID => string.IsNullOrEmpty(_UID) ? _UID = Guid.NewGuid().ToString() : _UID;

        string IGraphElement.name => "Connection";

        ///<summary>The connection status</summary>
        public Status status
        {
            get => _status;
            set => _status = value;
        }

        ///<summary>The graph this connection belongs to taken from the source node.</summary>
        public Graph graph => sourceNode != null ? sourceNode.graph : null;

        ///<summary>Create a new Connection. Use this for constructor</summary>
        public static Connection Create(Node source, Node target, int sourceIndex = -1, int targetIndex = -1)
        {
            if (source == null || target == null)
            {
                Logger.LogError("Can't Create a Connection without providing Source and Target Nodes");
                return null;
            }

            if (source is MissingNode)
            {
                Logger.LogError(
                    "Creating new Connections from a 'MissingNode' is not allowed. Please resolve the MissingNode node first");
                return null;
            }

            var newConnection = (Connection)Activator.CreateInstance(source.outConnectionType);

            UndoUtility.RecordObject(source.graph, "Create Connection");

            var resultSourceIndex = newConnection.SetSourceNode(source, sourceIndex);
            var resultTargetIndex = newConnection.SetTargetNode(target, targetIndex);

            newConnection.OnValidate(resultSourceIndex, resultTargetIndex);
            newConnection.OnCreate(resultSourceIndex, resultTargetIndex);
            UndoUtility.SetDirty(source.graph);

            return newConnection;
        }

        ///<summary>Duplicate the connection providing a new source and target</summary>
        public Connection Duplicate(Node newSource, Node newTarget)
        {
            if (newSource == null || newTarget == null)
            {
                Logger.LogError("Can't Duplicate a Connection without providing NewSource and NewTarget Nodes");
                return null;
            }

            //deep clone
            var newConnection = JSONSerializer.Clone(this);

            UndoUtility.RecordObject(newSource.graph, "Duplicate Connection");

            newConnection._UID = null;
            newConnection.sourceNode = newSource;
            newConnection.targetNode = newTarget;
            newSource.outConnections.Add(newConnection);
            newTarget.inConnections.Add(newConnection);

            if (newSource.graph != null)
                foreach (var task in Graph.GetTasksInElement(newConnection))
                    task.Validate(newSource.graph);
            //--

            newConnection.OnValidate(newSource.outConnections.Count - 1, newTarget.inConnections.Count - 1);
            UndoUtility.SetDirty(newSource.graph);
            return newConnection;
        }

        ///<summary>Sets the source node of the connection</summary>
        public int SetSourceNode(Node newSource, int index = -1)
        {
            if (sourceNode == newSource) return -1;

            UndoUtility.RecordObject(graph, "Set Source");

            //relink
            if (sourceNode != null && sourceNode.outConnections.Contains(this))
            {
                var i = sourceNode.outConnections.IndexOf(this);
                sourceNode.OnChildDisconnected(i);
                sourceNode.outConnections.Remove(this);
            }

            index = index == -1 ? newSource.outConnections.Count : index;
            newSource.outConnections.Insert(index, this);
            newSource.OnChildConnected(index);
            sourceNode = newSource;

#if UNITY_EDITOR
            if (sourceNode != null && targetNode != null) targetNode.TrySortConnectionsByPositionX();
#endif

            OnValidate(index, targetNode != null ? targetNode.inConnections.IndexOf(this) : -1);
            UndoUtility.SetDirty(graph);
            return index;
        }

        ///<summary>Sets the target node of the connection</summary>
        public int SetTargetNode(Node newTarget, int index = -1)
        {
            if (targetNode == newTarget) return -1;

            UndoUtility.RecordObject(graph, "Set Target");

            //relink
            if (targetNode != null && targetNode.inConnections.Contains(this))
            {
                var i = targetNode.inConnections.IndexOf(this);
                targetNode.OnParentDisconnected(i);
                targetNode.inConnections.Remove(this);
            }

            index = index == -1 ? newTarget.inConnections.Count : index;
            newTarget.inConnections.Insert(index, this);
            newTarget.OnParentConnected(index);
            targetNode = newTarget;

#if UNITY_EDITOR
            if (sourceNode != null && targetNode != null) targetNode.TrySortConnectionsByPositionX();
#endif

            OnValidate(sourceNode != null ? sourceNode.outConnections.IndexOf(this) : -1, index);
            UndoUtility.SetDirty(graph);
            return index;
        }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>Execute the connection for the specified agent and blackboard.</summary>
        public Status Execute(Component agent, IBlackboard blackboard)
        {
            if (!isActive) return Status.Optional;
            status = targetNode.Execute(agent, blackboard);
            return status;
        }

        ///<summary>Resets the connection and its targetNode, optionaly recursively</summary>
        public void Reset(bool recursively = true)
        {
            if (status == Status.Resting) return;
            status = Status.Resting;
            if (recursively) targetNode.Reset(recursively);
        }

        /// ----------------------------------------------------------------------------------------------
        /// <summary>Called once when the connection is created.</summary>
        public virtual void OnCreate(int sourceIndex, int targetIndex)
        {
        }

        ///<summary>Called when the Connection is created, duplicated or otherwise needs validation.</summary>
        public virtual void OnValidate(int sourceIndex, int targetIndex)
        {
        }

        /// <summary>
        ///     Called when the connection is destroyed (always through graph.RemoveConnection or when a node is removed
        ///     through graph.RemoveNode)
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        ///----------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return GetType().FriendlyName();
        }
    }
}