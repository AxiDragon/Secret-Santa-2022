using UnityEditor;

namespace JohnStairs.RCC.Character.MMO
{
    [CustomEditor(typeof(RPGMotorMMO))]
    public class RPGMotorEditor : Editor
    {
        private bool showJumpingSettings = true;
        private bool showMiscSettings = true;
        private bool showMovementSpeedSettings = true;
        private bool showMovingGroundsSettings = true;

        public void OnEnable()
        {
            #region Movement speed variables

            RunSpeed = serializedObject.FindProperty("RunSpeed");
            StrafeSpeed = serializedObject.FindProperty("StrafeSpeed");
            WalkSpeed = serializedObject.FindProperty("WalkSpeed");
            SprintSpeedMultiplier = serializedObject.FindProperty("SprintSpeedMultiplier");
            BackwardsSpeedMultiplier = serializedObject.FindProperty("BackwardsSpeedMultiplier");
            SwimSpeedMultiplier = serializedObject.FindProperty("SwimSpeedMultiplier");

            #endregion

            #region Jumping variables

            JumpHeight = serializedObject.FindProperty("JumpHeight");
            EnableMidairJumps = serializedObject.FindProperty("EnableMidairJumps");
            AllowedMidairJumps = serializedObject.FindProperty("AllowedMidairJumps");
            EnableMidairMovement = serializedObject.FindProperty("EnableMidairMovement");
            MidairSpeed = serializedObject.FindProperty("MidairSpeed");
            UnlimitedMidairMoves = serializedObject.FindProperty("UnlimitedMidairMoves");
            AllowedMidairMoves = serializedObject.FindProperty("AllowedMidairMoves");

            #endregion

            #region Moving grounds variables

            MoveWithMovingGround = serializedObject.FindProperty("MoveWithMovingGround");
            RotateWithRotatingGround = serializedObject.FindProperty("RotateWithRotatingGround");
            GroundAffectsJumping = serializedObject.FindProperty("GroundAffectsJumping");

            #endregion

            #region Misc variables

            RotationSpeed = serializedObject.FindProperty("RotationSpeed");
            IgnoredLayers = serializedObject.FindProperty("IgnoredLayers");
            EnableSliding = serializedObject.FindProperty("EnableSliding");
            SlidingTimeout = serializedObject.FindProperty("SlidingTimeout");
            EnableCollisionMovement = serializedObject.FindProperty("EnableCollisionMovement");
            SwimmingStartHeight = serializedObject.FindProperty("SwimmingStartHeight");
            DiveOnlyWhenSwimmingForward = serializedObject.FindProperty("DiveOnlyWhenSwimmingForward");
            GroundedTolerance = serializedObject.FindProperty("GroundedTolerance");
            FallingThreshold = serializedObject.FindProperty("FallingThreshold");
            Gravity = serializedObject.FindProperty("Gravity");

            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Movement speed variables

            showMovementSpeedSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(showMovementSpeedSettings, "Movement speed");
            if (showMovementSpeedSettings)
            {
                EditorGUILayout.PropertyField(RunSpeed);
                EditorGUILayout.PropertyField(StrafeSpeed);
                EditorGUILayout.PropertyField(WalkSpeed);
                EditorGUILayout.PropertyField(SprintSpeedMultiplier);
                EditorGUILayout.PropertyField(BackwardsSpeedMultiplier);
                EditorGUILayout.PropertyField(SwimSpeedMultiplier);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Jumping variables

            showJumpingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showJumpingSettings, "Jumping");
            if (showJumpingSettings)
            {
                EditorGUILayout.PropertyField(JumpHeight);
                EditorGUILayout.PropertyField(EnableMidairJumps);
                if (EnableMidairJumps.boolValue) EditorGUILayout.PropertyField(AllowedMidairJumps);
                EditorGUILayout.PropertyField(EnableMidairMovement);
                if (EnableMidairMovement.enumValueIndex != (int)RPGMotor.MidairMovement.Never)
                {
                    EditorGUILayout.PropertyField(MidairSpeed);
                    EditorGUILayout.PropertyField(UnlimitedMidairMoves);
                    if (!UnlimitedMidairMoves.boolValue) EditorGUILayout.PropertyField(AllowedMidairMoves);
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Moving grounds variables

            showMovingGroundsSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(showMovingGroundsSettings, "Moving grounds");
            if (showMovingGroundsSettings)
            {
                EditorGUILayout.PropertyField(MoveWithMovingGround);
                if (MoveWithMovingGround.boolValue)
                {
                    EditorGUILayout.PropertyField(RotateWithRotatingGround);
                    EditorGUILayout.PropertyField(GroundAffectsJumping);
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Misc variables

            showMiscSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showMiscSettings, "Miscellaneous");
            if (showMiscSettings)
            {
                EditorGUILayout.PropertyField(RotationSpeed);
                EditorGUILayout.PropertyField(IgnoredLayers);
                EditorGUILayout.PropertyField(EnableSliding);
                if (EnableSliding.boolValue) EditorGUILayout.PropertyField(SlidingTimeout);
                EditorGUILayout.PropertyField(EnableCollisionMovement);
                EditorGUILayout.PropertyField(SwimmingStartHeight);
                EditorGUILayout.PropertyField(DiveOnlyWhenSwimmingForward);
                EditorGUILayout.PropertyField(GroundedTolerance);
                EditorGUILayout.PropertyField(FallingThreshold);
                EditorGUILayout.PropertyField(Gravity);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region Movement speed variables

        private SerializedProperty RunSpeed;
        private SerializedProperty StrafeSpeed;
        private SerializedProperty WalkSpeed;
        private SerializedProperty SprintSpeedMultiplier;
        private SerializedProperty BackwardsSpeedMultiplier;
        private SerializedProperty SwimSpeedMultiplier;

        #endregion

        #region Jumping variables

        private SerializedProperty JumpHeight;
        private SerializedProperty EnableMidairJumps;
        private SerializedProperty AllowedMidairJumps;
        private SerializedProperty EnableMidairMovement;
        private SerializedProperty MidairSpeed;
        private SerializedProperty UnlimitedMidairMoves;
        private SerializedProperty AllowedMidairMoves;

        #endregion

        #region Moving grounds variables

        private SerializedProperty MoveWithMovingGround;
        private SerializedProperty RotateWithRotatingGround;
        private SerializedProperty GroundAffectsJumping;

        #endregion

        #region Misc variables

        private SerializedProperty RotationSpeed;
        private SerializedProperty IgnoredLayers;
        private SerializedProperty EnableSliding;
        private SerializedProperty SlidingTimeout;
        private SerializedProperty EnableCollisionMovement;
        private SerializedProperty SwimmingStartHeight;
        private SerializedProperty DiveOnlyWhenSwimmingForward;
        private SerializedProperty GroundedTolerance;
        private SerializedProperty FallingThreshold;
        private SerializedProperty Gravity;

        #endregion
    }
}