using UnityEditor;

namespace JohnStairs.RCC.Character.ARPG
{
    [CustomEditor(typeof(RPGControllerARPG))]
    public class RPGControllerEditor : Editor
    {
        private SerializedProperty ActivateCharacterControl;
        private SerializedProperty LogInputWarnings;
        private SerializedProperty SmoothDirectionInputChanges;
        private SerializedProperty UseNewInputSystem;

        public void OnEnable()
        {
            ActivateCharacterControl = serializedObject.FindProperty("ActivateCharacterControl");
            UseNewInputSystem = serializedObject.FindProperty("UseNewInputSystem");
            LogInputWarnings = serializedObject.FindProperty("LogInputWarnings");
            SmoothDirectionInputChanges = serializedObject.FindProperty("SmoothDirectionInputChanges");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(ActivateCharacterControl);
            EditorGUILayout.PropertyField(UseNewInputSystem);
            if (!UseNewInputSystem.boolValue) EditorGUILayout.PropertyField(LogInputWarnings);
            EditorGUILayout.PropertyField(SmoothDirectionInputChanges);

            serializedObject.ApplyModifiedProperties();
        }
    }
}