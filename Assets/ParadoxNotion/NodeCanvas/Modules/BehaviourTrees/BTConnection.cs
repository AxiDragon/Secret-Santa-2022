using NodeCanvas.Framework;
using ParadoxNotion;

namespace NodeCanvas.BehaviourTrees
{
    ///<summary>The connection object for BehaviourTree nodes</summary>
    public class BTConnection : Connection
    {
        /// ----------------------------------------------------------------------------------------------
        /// ---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override PlanarDirection direction
        {
            get { return PlanarDirection.Vertical; }
        }

#endif
        ///----------------------------------------------------------------------------------------------
    }
}