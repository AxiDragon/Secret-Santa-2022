using UnityEditor;

namespace JohnStairs.RCC.Character.ARPG
{
    [CustomEditor(typeof(RPGMotorARPG))]
    public class RPGMotorEditor : Editor
    {
        private bool showJumpingSettings = true;
        private bool showMiscSettings = true;
        private bool showMovementSpeedSettings = true;
        private bool showMovingGroundsSettings = true;
        private bool showRotationSettings = true;

        public void OnEnable()
        {
            #region Movement speed variables

            RunSpeed = serializedObject.FindProperty("RunSpeed");
            WalkSpeed = serializedObject.FindProperty("WalkSpeed");
            CrouchSpeed = serializedObject.FindProperty("CrouchSpeed");
            SprintSpeedMultiplier = serializedObject.FindProperty("SprintSpeedMultiplier");
            SwimSpeedMultiplier = serializedObject.FindProperty("SwimSpeedMultiplier");

            #endregion

            #region Rotation variables

            CompleteTurnWhileStanding = serializedObject.FindProperty("CompleteTurnWhileStanding");
            RotationTime = serializedObject.FindProperty("RotationTime");

            #endregion

            #region Jumping variables

            JumpHeight = serializedObject.FindProperty("JumpHeight");
            EnableMidairJumps = serializedObject.FindProperty("EnableMidairJumps");
            AllowedMidairJumps = serializedObject.FindProperty("AllowedMidairJumps");
            EnableMidairMovement = serializedObject.FindProperty("EnableMidairMovement");
            MidairSpeed = serializedObject.FindProperty("MidairSpeed");

            #endregion

            #region Moving grounds variables

            MoveWithMovingGround = serializedObject.FindProperty("MoveWithMovingGround");
            RotateWithRotatingGround = serializedObject.FindProperty("RotateWithRotatingGround");
            GroundAffectsJumping = serializedObject.FindProperty("GroundAffectsJumping");

            #endregion

            #region Misc variables

            IgnoredLayers = serializedObject.FindProperty("IgnoredLayers");
            EnableSliding = serializedObject.FindProperty("EnableSliding");
            SlidingTimeout = serializedObject.FindProperty("SlidingTimeout");
            EnableCollisionMovement = serializedObject.FindProperty("EnableCollisionMovement");
            SwimmingStartHeight = serializedObject.FindProperty("SwimmingStartHeight");
            FlyingTimeout = serializedObject.FindProperty("FlyingTimeout");
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
                EditorGUILayout.PropertyField(WalkSpeed);
                EditorGUILayout.PropertyField(CrouchSpeed);
                EditorGUILayout.PropertyField(SprintSpeedMultiplier);
                EditorGUILayout.PropertyField(SwimSpeedMultiplier);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Rotation variables

            showRotationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRotationSettings, "Rotation");
            if (showRotationSettings)
            {
                EditorGUILayout.PropertyField(CompleteTurnWhileStanding);
                EditorGUILayout.PropertyField(RotationTime);
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
                    EditorGUILayout.PropertyField(MidairSpeed);
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
                EditorGUILayout.PropertyField(IgnoredLayers);
                EditorGUILayout.PropertyField(EnableSliding);
                if (EnableSliding.boolValue) EditorGUILayout.PropertyField(SlidingTimeout);
                EditorGUILayout.PropertyField(EnableCollisionMovement);
                EditorGUILayout.PropertyField(SwimmingStartHeight);
                EditorGUILayout.PropertyField(FlyingTimeout);
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
        private SerializedProperty WalkSpeed;
        private SerializedProperty CrouchSpeed;
        private SerializedProperty SprintSpeedMultiplier;
        private SerializedProperty SwimSpeedMultiplier;

        #endregion

        #region Rotation variables

        private SerializedProperty CompleteTurnWhileStanding;
        private SerializedProperty RotationTime;

        #endregion

        #region Jumping variables

        private SerializedProperty JumpHeight;
        private SerializedProperty EnableMidairJumps;
        private SerializedProperty AllowedMidairJumps;
        private SerializedProperty EnableMidairMovement;
        private SerializedProperty MidairSpeed;

        #endregion

        #region Moving grounds variables

        private SerializedProperty MoveWithMovingGround;
        private SerializedProperty RotateWithRotatingGround;
        private SerializedProperty GroundAffectsJumping;

        #endregion

        #region Misc variables

        private SerializedProperty IgnoredLayers;
        private SerializedProperty EnableSliding;
        private SerializedProperty SlidingTimeout;
        private SerializedProperty EnableCollisionMovement;
        private SerializedProperty SwimmingStartHeight;
        private SerializedProperty FlyingTimeout;
        private SerializedProperty GroundedTolerance;
        private SerializedProperty FallingThreshold;
        private SerializedProperty Gravity;

        #endregion
    }
}