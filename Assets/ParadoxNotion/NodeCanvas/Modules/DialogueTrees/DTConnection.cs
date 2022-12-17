using NodeCanvas.Framework;
using ParadoxNotion;

namespace NodeCanvas.DialogueTrees
{
    public class DTConnection : Connection
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