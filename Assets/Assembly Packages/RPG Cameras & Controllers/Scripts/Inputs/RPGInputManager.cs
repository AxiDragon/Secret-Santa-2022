namespace JohnStairs.RCC.Inputs
{
    public class RPGInputManager
    {
        private static RPGInputActions _inputActions;

        public static RPGInputActions GetInputActions()
        {
            if (_inputActions == null)
            {
                _inputActions = new RPGInputActions();
                _inputActions.Enable();
            }

            return _inputActions;
        }
    }
}