using UnityEditor;

namespace JohnStairs.RCC.Character.ARPG
{
    [CustomEditor(typeof(RPGCameraARPG))]
    public class RPGCameraEditor : Editor
    {
        private bool showCursorSettings = true;
        private bool showDistanceSettings = true;
        private bool showGeneralSettings = true;
        private bool showRotationXSettings = true;
        private bool showRotationYSettings = true;
        private bool showUnderwaterSettings = true;

        public void OnEnable()
        {
            #region General variables

            UseNewInputSystem = serializedObject.FindProperty("UseNewInputSystem");
            LogInputWarnings = serializedObject.FindProperty("LogInputWarnings");
            UsedCamera = serializedObject.FindProperty("UsedCamera");
            CameraToUse = serializedObject.FindProperty("CameraToUse");
            UsedSkybox = serializedObject.FindProperty("UsedSkybox");
            CameraPivotLocalPosition = serializedObject.FindProperty("CameraPivotLocalPosition");
            EnableIntelligentPivot = serializedObject.FindProperty("EnableIntelligentPivot");
            PivotSmoothTime = serializedObject.FindProperty("PivotSmoothTime");
            ActivateCameraControl = serializedObject.FindProperty("ActivateCameraControl");
            AlwaysRotateCamera = serializedObject.FindProperty("AlwaysRotateCamera");
            RotationSmoothTime = serializedObject.FindProperty("RotationSmoothTime");

            #endregion

            #region Rotation X variables

            StartRotationX = serializedObject.FindProperty("StartRotationX");
            LockRotationX = serializedObject.FindProperty("LockRotationX");
            InvertRotationX = serializedObject.FindProperty("InvertRotationX");
            RotationXSensitivity = serializedObject.FindProperty("RotationXSensitivity");
            ConstrainRotationX = serializedObject.FindProperty("ConstrainRotationX");
            RotationXMin = serializedObject.FindProperty("RotationXMin");
            RotationXMax = serializedObject.FindProperty("RotationXMax");

            #endregion

            #region Rotation Y variables

            StartRotationY = serializedObject.FindProperty("StartRotationY");
            LockRotationY = serializedObject.FindProperty("LockRotationY");
            InvertRotationY = serializedObject.FindProperty("InvertRotationY");
            RotationYSensitivity = serializedObject.FindProperty("RotationYSensitivity");
            RotationYMin = serializedObject.FindProperty("RotationYMin");
            RotationYMax = serializedObject.FindProperty("RotationYMax");

            #endregion

            #region Distance variables

            StartDistance = serializedObject.FindProperty("StartDistance");
            StartZoomOut = serializedObject.FindProperty("StartZoomOut");
            ZoomSensitivity = serializedObject.FindProperty("ZoomSensitivity");
            MinDistance = serializedObject.FindProperty("MinDistance");
            MaxDistance = serializedObject.FindProperty("MaxDistance");
            DistanceSmoothTime = serializedObject.FindProperty("DistanceSmoothTime");

            #endregion

            #region Cursor variables

            HideCursor = serializedObject.FindProperty("HideCursor");
            CursorBehaviorOrbiting = serializedObject.FindProperty("CursorBehaviorOrbiting");

            #endregion

            #region Underwater variables

            UnderwaterFogColor = serializedObject.FindProperty("UnderwaterFogColor");
            UnderwaterFogDensity = serializedObject.FindProperty("UnderwaterFogDensity");
            UnderwaterThresholdTuning = serializedObject.FindProperty("UnderwaterThresholdTuning");

            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region General variables

            showGeneralSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGeneralSettings, "General");
            if (showGeneralSettings)
            {
                EditorGUILayout.PropertyField(UseNewInputSystem);
                if (!UseNewInputSystem.boolValue) EditorGUILayout.PropertyField(LogInputWarnings);
                EditorGUILayout.PropertyField(UsedCamera);
                if (UsedCamera.enumValueIndex == (int)RPGCamera.CameraUsage.AssignedCamera)
                    EditorGUILayout.PropertyField(CameraToUse);
                EditorGUILayout.PropertyField(UsedSkybox);
                EditorGUILayout.PropertyField(CameraPivotLocalPosition);
                if (((RPGCamera)serializedObject.targetObject).HasInternalPivot())
                {
                    EditorGUILayout.LabelField("└ Internal pivot logic applies");
                    EditorGUILayout.PropertyField(EnableIntelligentPivot);
                    if (EnableIntelligentPivot.boolValue) EditorGUILayout.PropertyField(PivotSmoothTime);
                }
                else
                {
                    EditorGUILayout.LabelField("└ External pivot logic applies");
                }

                EditorGUILayout.PropertyField(ActivateCameraControl);
                EditorGUILayout.PropertyField(AlwaysRotateCamera);
                EditorGUILayout.PropertyField(RotationSmoothTime);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Rotation X variables

            showRotationXSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRotationXSettings, "Rotation X");
            if (showRotationXSettings)
            {
                EditorGUILayout.PropertyField(StartRotationX);
                EditorGUILayout.PropertyField(LockRotationX);
                EditorGUILayout.PropertyField(InvertRotationX);
                EditorGUILayout.PropertyField(RotationXSensitivity);
                EditorGUILayout.PropertyField(ConstrainRotationX);
                EditorGUILayout.PropertyField(RotationXMin);
                EditorGUILayout.PropertyField(RotationXMax);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Rotation Y variables

            showRotationYSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRotationYSettings, "Rotation Y");
            if (showRotationYSettings)
            {
                EditorGUILayout.PropertyField(StartRotationY);
                EditorGUILayout.PropertyField(LockRotationY);
                EditorGUILayout.PropertyField(InvertRotationY);
                EditorGUILayout.PropertyField(RotationYSensitivity);
                EditorGUILayout.PropertyField(RotationYMin);
                EditorGUILayout.PropertyField(RotationYMax);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Distance variables

            showDistanceSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showDistanceSettings, "Distance");
            if (showDistanceSettings)
            {
                EditorGUILayout.PropertyField(StartDistance);
                EditorGUILayout.PropertyField(StartZoomOut);
                EditorGUILayout.PropertyField(ZoomSensitivity);
                EditorGUILayout.PropertyField(MinDistance);
                EditorGUILayout.PropertyField(MaxDistance);
                EditorGUILayout.PropertyField(DistanceSmoothTime);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Cursor variables

            showCursorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showCursorSettings, "Cursor");
            if (showCursorSettings)
            {
                EditorGUILayout.PropertyField(HideCursor);
                EditorGUILayout.PropertyField(CursorBehaviorOrbiting);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Underwater variables

            showUnderwaterSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showUnderwaterSettings, "Underwater");
            if (showUnderwaterSettings)
            {
                EditorGUILayout.PropertyField(UnderwaterFogColor);
                EditorGUILayout.PropertyField(UnderwaterFogDensity);
                EditorGUILayout.PropertyField(UnderwaterThresholdTuning);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region General variables

        private SerializedProperty UseNewInputSystem;
        private SerializedProperty LogInputWarnings;
        private SerializedProperty UsedCamera;
        private SerializedProperty CameraToUse;
        private SerializedProperty UsedSkybox;
        private SerializedProperty CameraPivotLocalPosition;
        private SerializedProperty EnableIntelligentPivot;
        private SerializedProperty PivotSmoothTime;
        private SerializedProperty ActivateCameraControl;
        private SerializedProperty AlwaysRotateCamera;
        private SerializedProperty RotationSmoothTime;

        #endregion

        #region Rotation X variables

        private SerializedProperty StartRotationX;
        private SerializedProperty LockRotationX;
        private SerializedProperty InvertRotationX;
        private SerializedProperty RotationXSensitivity;
        private SerializedProperty ConstrainRotationX;
        private SerializedProperty RotationXMin;
        private SerializedProperty RotationXMax;

        #endregion

        #region Rotation Y variables

        private SerializedProperty StartRotationY;
        private SerializedProperty LockRotationY;
        private SerializedProperty InvertRotationY;
        private SerializedProperty RotationYSensitivity;
        private SerializedProperty RotationYMin;
        private SerializedProperty RotationYMax;

        #endregion

        #region Distance variables

        private SerializedProperty StartDistance;
        private SerializedProperty StartZoomOut;
        private SerializedProperty ZoomSensitivity;
        private SerializedProperty MinDistance;
        private SerializedProperty MaxDistance;
        private SerializedProperty DistanceSmoothTime;

        #endregion

        #region Cursor variables

        private SerializedProperty HideCursor;
        private SerializedProperty CursorBehaviorOrbiting;

        #endregion

        #region Underwater variables

        private SerializedProperty UnderwaterFogColor;
        private SerializedProperty UnderwaterFogDensity;
        private SerializedProperty UnderwaterThresholdTuning;

        #endregion
    }
}