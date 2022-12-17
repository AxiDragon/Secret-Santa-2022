﻿#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public static class FGUI_Resources
    {
        /// Background Icons ----------------------------------------------------
        public static GUIStyle HeaderBoxStyle
        {
            get
            {
                if (__headerBoxStyle != null) return __headerBoxStyle;
                __headerBoxStyle = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FHelpBox");
                __headerBoxStyle.normal.background = bg;
                __headerBoxStyle.border = new RectOffset(6, 6, 6, 6);
                return __headerBoxStyle;
            }
        }

        private static GUIStyle __headerBoxStyle;

        public static GUIStyle HeaderBoxStyleH
        {
            get
            {
                if (__headerBoxStyleH != null) return __headerBoxStyleH;
                __headerBoxStyleH = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FHelpBoxH");
                __headerBoxStyleH.normal.background = bg;
                __headerBoxStyleH.border = new RectOffset(6, 6, 6, 6);
                return __headerBoxStyleH;
            }
        }

        private static GUIStyle __headerBoxStyleH;

        public static GUIStyle ViewBoxStyle
        {
            get
            {
                if (__viewBoxStyle != null) return __viewBoxStyle;
                __viewBoxStyle = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FViewBox");
                __viewBoxStyle.normal.background = bg;
                __viewBoxStyle.border = new RectOffset(6, 6, 6, 6);
                __viewBoxStyle.padding = new RectOffset(0, 0, 0, 5);
                return __viewBoxStyle;
            }
        }

        private static GUIStyle __viewBoxStyle;

        public static GUIStyle FrameBoxStyle
        {
            get
            {
                if (__frameBoxStyle != null) return __frameBoxStyle;
                __frameBoxStyle = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FFrameBox");
                __frameBoxStyle.normal.background = bg;
                __frameBoxStyle.border = new RectOffset(6, 6, 6, 6);
                __frameBoxStyle.padding = new RectOffset(1, 1, 1, 1);
                return __frameBoxStyle;
            }
        }

        private static GUIStyle __frameBoxStyle;

        public static GUIStyle BGBoxStyle
        {
            get
            {
                if (__bgBoxStyle != null) return __bgBoxStyle;
                __bgBoxStyle = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FBGBox");
                __bgBoxStyle.normal.background = bg;
                __bgBoxStyle.border = new RectOffset(6, 6, 6, 6);
                __bgBoxStyle.padding = new RectOffset(1, 1, 1, 1);
                return __bgBoxStyle;
            }
        }

        private static GUIStyle __bgBoxStyle;

        public static GUIStyle BGInBoxStyle
        {
            get
            {
                if (__inBoxStyle != null) return __inBoxStyle;
                __inBoxStyle = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FInBox");
                __inBoxStyle.normal.background = bg;
                __inBoxStyle.border = new RectOffset(4, 4, 4, 4);
                __inBoxStyle.padding = new RectOffset(8, 6, 5, 5);
                __inBoxStyle.margin = new RectOffset(0, 0, 0, 0);
                return __inBoxStyle;
            }
        }

        private static GUIStyle __inBoxStyle;

        public static GUIStyle BGInBoxStyleH
        {
            get
            {
                if (__inBoxStyleH != null) return __inBoxStyleH;
                __inBoxStyleH = new GUIStyle(EditorStyles.helpBox);
                var bg = Resources.Load<Texture2D>("Fimp/Backgrounds/FInBoxH");
                __inBoxStyleH.normal.background = bg;
                __inBoxStyleH.border = new RectOffset(4, 4, 4, 4);
                __inBoxStyleH.padding = new RectOffset(8, 6, 5, 5);
                __inBoxStyleH.margin = new RectOffset(0, 0, 0, 0);
                return __inBoxStyleH;
            }
        }

        private static GUIStyle __inBoxStyleH;

        public static GUIStyle BGInBoxBlankStyle
        {
            get
            {
                if (__inBoxBlankStyle != null) return __inBoxBlankStyle;
                __inBoxBlankStyle = new GUIStyle();
                __inBoxBlankStyle.padding = BGInBoxStyle.padding;
                __inBoxBlankStyle.margin = BGInBoxStyle.margin;
                return __inBoxBlankStyle;
            }
        }

        private static GUIStyle __inBoxBlankStyle;

        public static GUIStyle BGInBoxLightStyle
        {
            get
            {
                if (__inBoxLightStyle != null) return __inBoxLightStyle;
                __inBoxLightStyle = new GUIStyle(BGInBoxStyle);
                __inBoxLightStyle.normal.background = Resources.Load<Texture2D>("Fimp/Backgrounds/FInBoxLight");
                return __inBoxLightStyle;
            }
        }

        private static GUIStyle __inBoxLightStyle;

        public static GUIStyle ButtonStyle
        {
            get
            {
                if (__buttStyle != null) return __buttStyle;
                __buttStyle = new GUIStyle(EditorStyles.miniButton);
                __buttStyle.fixedHeight = 0;
                __buttStyle.padding = new RectOffset(3, 3, 3, 3);
                __buttStyle.normal.background = Resources.Load<Texture2D>("Fimp/Backgrounds/Fbutton");
                __buttStyle.hover.background = Resources.Load<Texture2D>("Fimp/FbuttonHover");
                __buttStyle.focused.background = __buttStyle.hover.background;
                __buttStyle.active.background = Resources.Load<Texture2D>("Fimp/Backgrounds/FbuttonPress");
                return __buttStyle;
            }
        }

        private static GUIStyle __buttStyle;

        public static GUIStyle ButtonStyleR
        {
            get
            {
                if (__buttStyler != null) return __buttStyler;
                __buttStyler = new GUIStyle(EditorStyles.miniButton);
                __buttStyler.fixedHeight = 0;
                __buttStyler.padding = new RectOffset(3, 3, 3, 3);
                __buttStyler.normal.background = Resources.Load<Texture2D>("Fimp/Backgrounds/Fbutton");
                __buttStyler.hover.background = Resources.Load<Texture2D>("Fimp/FbuttonHover");
                __buttStyler.focused.background = __buttStyler.hover.background;
                __buttStyler.active.background = Resources.Load<Texture2D>("Fimp/Backgrounds/FbuttonPress");
                __buttStyler.richText = true;
                return __buttStyler;
            }
        }

        private static GUIStyle __buttStyler;

        /// Text Styles ----------------------------------------------------
#if UNITY_2019_3_OR_NEWER
        public static GUIStyle HeaderStyle
        {
            get
            {
                if (__headerStyle != null) return __headerStyle;
                __headerStyle = new GUIStyle(EditorStyles.boldLabel);
                __headerStyle.richText = true;
                __headerStyle.padding = new RectOffset(0, 0, 0, 0);
                __headerStyle.margin = __headerStyle.padding;
                __headerStyle.alignment = TextAnchor.MiddleCenter;
                __headerStyle.active.textColor = Color.white;
                return __headerStyle;
            }
        }

        private static GUIStyle __headerStyle;

        public static GUIStyle HeaderStyleBig
        {
            get
            {
                if (__headerStyleBig != null) return __headerStyleBig;
                __headerStyleBig = new GUIStyle(HeaderStyle);
                __headerStyleBig.fontSize = 17;
                __headerStyleBig.fontStyle = FontStyle.Normal;
                return __headerStyle;
            }
        }

        private static GUIStyle __headerStyleBig;
#else
        public static GUIStyle HeaderStyle { get { if (__headerStyle != null) return __headerStyle; __headerStyle =
 new GUIStyle(EditorStyles.boldLabel); __headerStyle.richText = true; __headerStyle.padding =
 new RectOffset(0, 0, 1, 0); __headerStyle.margin = __headerStyle.padding; __headerStyle.alignment =
 TextAnchor.MiddleCenter; __headerStyle.active.textColor = Color.white; return __headerStyle; } }
        private static GUIStyle __headerStyle = null;

        public static GUIStyle HeaderStyleBig { get { if (__headerStyleBig != null) return __headerStyleBig; __headerStyleBig
 = new GUIStyle(HeaderStyle); __headerStyleBig.fontSize = 17; return __headerStyle; } }
        private static GUIStyle __headerStyleBig = null;
#endif

#if UNITY_2019_3_OR_NEWER
        public static GUIStyle FoldStyle
        {
            get
            {
                if (__foldStyle != null) return __foldStyle;
                __foldStyle = new GUIStyle(EditorStyles.boldLabel);
                __foldStyle.richText = true;
                __foldStyle.padding = new RectOffset(0, 0, 0, 0);
                __foldStyle.margin = __foldStyle.padding;
                __foldStyle.alignment = TextAnchor.MiddleLeft;
                __foldStyle.active.textColor = Color.white;
                return __foldStyle;
            }
        }

        private static GUIStyle __foldStyle;

        public static GUIStyle FoldStyleBig
        {
            get
            {
                if (__foldStyleBig != null) return __foldStyleBig;
                __foldStyleBig = new GUIStyle(FoldStyle);
                __foldStyleBig.fontSize = 16;
                __foldStyleBig.fontStyle = FontStyle.Normal;
                return __foldStyleBig;
            }
        }

        private static GUIStyle __foldStyleBig;
#else
        public static GUIStyle FoldStyle { get { if (__foldStyle != null) return __foldStyle; __foldStyle =
 new GUIStyle(EditorStyles.boldLabel); __foldStyle.richText = true; __foldStyle.padding =
 new RectOffset(0, 0, 1, 0); __foldStyle.margin = __foldStyle.padding; __foldStyle.alignment =
 TextAnchor.MiddleLeft; __foldStyle.active.textColor = Color.white; return __foldStyle; } }
        private static GUIStyle __foldStyle = null;

        public static GUIStyle FoldStyleBig { get { if (__foldStyleBig != null) return __foldStyleBig; __foldStyleBig =
 new GUIStyle(FoldStyle);  __foldStyleBig.fontSize = 16; return __foldStyleBig; } }
        private static GUIStyle __foldStyleBig = null;
#endif

        /// Icons ----------------------------------------------------

        public static Texture2D Tex_GearSetup
        {
            get
            {
                if (__texSetup != null) return __texSetup;
                __texSetup = Resources.Load<Texture2D>("Fimp/Icons/FGearSetup");
                return __texSetup;
            }
        }

        private static Texture2D __texSetup;

        public static Texture2D Tex_Optimization
        {
            get
            {
                if (__texOptim != null) return __texOptim;
                __texOptim = Resources.Load<Texture2D>("Fimp/Icons/FOptimization");
                return __texOptim;
            }
        }

        private static Texture2D __texOptim;

        public static Texture2D Tex_Sliders
        {
            get
            {
                if (__texSld != null) return __texSld;
                __texSld = Resources.Load<Texture2D>("Fimp/Icons/FSliders");
                return __texSld;
            }
        }

        private static Texture2D __texSld;

        public static Texture2D Tex_GearMain
        {
            get
            {
                if (__texMain != null) return __texMain;
                __texMain = Resources.Load<Texture2D>("Fimp/Icons/FGearMain");
                return __texMain;
            }
        }

        private static Texture2D __texMain;

        public static Texture2D Tex_Add
        {
            get
            {
                if (__texAdd != null) return __texAdd;
                __texAdd = Resources.Load<Texture2D>("Fimp/Icons/FAdd");
                return __texAdd;
            }
        }

        private static Texture2D __texAdd;

        public static Texture2D Tex_Gear
        {
            get
            {
                if (__texGear != null) return __texGear;
                __texGear = Resources.Load<Texture2D>("Fimp/Icons/FGear");
                return __texGear;
            }
        }

        private static Texture2D __texGear;

        public static Texture2D Tex_Expose
        {
            get
            {
                if (__texExpose != null) return __texExpose;
                __texExpose = Resources.Load<Texture2D>("Fimp/Icons/FExpose");
                return __texExpose;
            }
        }

        private static Texture2D __texExpose;

        public static Texture2D Tex_Knob
        {
            get
            {
                if (__texKnob != null) return __texKnob;
                __texKnob = Resources.Load<Texture2D>("Fimp/Icons/FKnob");
                return __texKnob;
            }
        }

        private static Texture2D __texKnob;

        public static Texture2D Tex_Repair
        {
            get
            {
                if (__texRepair != null) return __texRepair;
                __texRepair = Resources.Load<Texture2D>("Fimp/Icons/FRepair");
                return __texRepair;
            }
        }

        private static Texture2D __texRepair;

        public static Texture2D Tex_Physics
        {
            get
            {
                if (__texPhysics != null) return __texPhysics;
                __texPhysics = Resources.Load<Texture2D>("Fimp/Icons/FPhysics");
                return __texPhysics;
            }
        }

        private static Texture2D __texPhysics;

        public static Texture2D Tex_Tweaks
        {
            get
            {
                if (__texTweaks != null) return __texTweaks;
                __texTweaks = Resources.Load<Texture2D>("Fimp/Icons/FTweaks");
                return __texTweaks;
            }
        }

        private static Texture2D __texTweaks;

        public static Texture2D Tex_Extension
        {
            get
            {
                if (__texExt != null) return __texExt;
                __texExt = Resources.Load<Texture2D>("Fimp/Icons/FExtension");
                return __texExt;
            }
        }

        private static Texture2D __texExt;

        public static Texture2D Tex_Module
        {
            get
            {
                if (__texModls != null) return __texModls;
                __texModls = Resources.Load<Texture2D>("Fimp/Icons/FModule");
                return __texModls;
            }
        }

        private static Texture2D __texModls;

        public static Texture2D Tex_Limits
        {
            get
            {
                if (__texLimts != null) return __texLimts;
                __texLimts = Resources.Load<Texture2D>("Fimp/Icons/FLimits");
                return __texLimts;
            }
        }

        private static Texture2D __texLimts;

        public static Texture2D Tex_ExportIcon
        {
            get
            {
                if (_texExportIc != null) return _texExportIc;
                _texExportIc = Resources.Load<Texture2D>("Fimp/Icons/FExport");
                return _texExportIc;
            }
        }

        private static Texture2D _texExportIc;

        public static Texture2D Tex_Load
        {
            get
            {
                if (__texLoadIc != null) return __texLoadIc;
                __texLoadIc = Resources.Load<Texture2D>("Fimp/Icons/FLoad");
                return __texLoadIc;
            }
        }

        private static Texture2D __texLoadIc;

        /// Misc Icons ----------------------------------------------------
        public static Texture2D Tex_ArrowUp
        {
            get
            {
                if (__texArrUp != null) return __texArrUp;
                __texArrUp = Resources.Load<Texture2D>("Fimp/Misc Icons/FArrowUp");
                return __texArrUp;
            }
        }

        private static Texture2D __texArrUp;

        public static Texture2D Tex_ArrowDown
        {
            get
            {
                if (__texArrD != null) return __texArrD;
                __texArrD = Resources.Load<Texture2D>("Fimp/Misc Icons/FArrowDown");
                return __texArrD;
            }
        }

        private static Texture2D __texArrD;

        public static Texture2D Tex_ArrowLeft
        {
            get
            {
                if (__texArrL != null) return __texArrL;
                __texArrL = Resources.Load<Texture2D>("Fimp/Misc Icons/FArrowLeft");
                return __texArrL;
            }
        }

        private static Texture2D __texArrL;

        public static Texture2D Tex_ArrowRight
        {
            get
            {
                if (__texArrR != null) return __texArrR;
                __texArrR = Resources.Load<Texture2D>("Fimp/Misc Icons/FArrowRight");
                return __texArrR;
            }
        }

        private static Texture2D __texArrR;

        public static Texture2D Tex_AB
        {
            get
            {
                if (__texAB != null) return __texAB;
                __texAB = Resources.Load<Texture2D>("Fimp/Misc Icons/FABSwitch");
                return __texAB;
            }
        }

        private static Texture2D __texAB;

        public static Texture2D Tex_Gizmos
        {
            get
            {
                if (__texGizm != null) return __texGizm;
                __texGizm = Resources.Load<Texture2D>("Fimp/Misc Icons/FGizmos");
                return __texGizm;
            }
        }

        private static Texture2D __texGizm;

        public static Texture2D Tex_GizmosOff
        {
            get
            {
                if (__texGizmOff != null) return __texGizmOff;
                __texGizmOff = Resources.Load<Texture2D>("Fimp/Misc Icons/FGizmosOff");
                return __texGizmOff;
            }
        }

        private static Texture2D __texGizmOff;

        public static Texture2D Tex_Manual
        {
            get
            {
                if (__texManual != null) return __texManual;
                __texManual = Resources.Load<Texture2D>("Fimp/Misc Icons/FManual");
                return __texManual;
            }
        }

        private static Texture2D __texManual;

        public static Texture2D Tex_Website
        {
            get
            {
                if (__texWeb != null) return __texWeb;
                __texWeb = Resources.Load<Texture2D>("Fimp/Misc Icons/FWebsite");
                return __texWeb;
            }
        }

        private static Texture2D __texWeb;

        public static Texture2D Tex_Tutorials
        {
            get
            {
                if (__texTutorials != null) return __texTutorials;
                __texTutorials = Resources.Load<Texture2D>("Fimp/Misc Icons/FTutorials");
                return __texTutorials;
            }
        }

        private static Texture2D __texTutorials;

        public static Texture2D Tex_Default
        {
            get
            {
                if (__texDef != null) return __texDef;
                __texDef = Resources.Load<Texture2D>("Fimp/Misc Icons/FDefault");
                return __texDef;
            }
        }

        private static Texture2D __texDef;

        public static Texture2D Tex_HierSwitch
        {
            get
            {
                if (__hierSwitch != null) return __hierSwitch;
                __hierSwitch = Resources.Load<Texture2D>("Fimp/Misc Icons/FHierarchySwitch");
                return __hierSwitch;
            }
        }

        private static Texture2D __hierSwitch;

        public static Texture2D Tex_Rectangle
        {
            get
            {
                if (__texRect != null) return __texRect;
                __texRect = Resources.Load<Texture2D>("Fimp/Misc Icons/FRect");
                return __texRect;
            }
        }

        private static Texture2D __texRect;

        public static Texture2D Tex_RightFold
        {
            get
            {
                if (__texRightFold != null) return __texRightFold;
                __texRightFold = Resources.Load<Texture2D>("Fimp/Misc Icons/FRightFolded");
                return __texRightFold;
            }
        }

        private static Texture2D __texRightFold;

        public static Texture2D Tex_LeftFold
        {
            get
            {
                if (__texLeftFold != null) return __texLeftFold;
                __texLeftFold = Resources.Load<Texture2D>("Fimp/Misc Icons/FLeftFolded");
                return __texLeftFold;
            }
        }

        private static Texture2D __texLeftFold;

        public static Texture2D Tex_UpFold
        {
            get
            {
                if (__texUpFold != null) return __texUpFold;
                __texUpFold = Resources.Load<Texture2D>("Fimp/Misc Icons/FUpFolded");
                return __texUpFold;
            }
        }

        private static Texture2D __texUpFold;

        public static Texture2D Tex_DownFold
        {
            get
            {
                if (__texDownFold != null) return __texDownFold;
                __texDownFold = Resources.Load<Texture2D>("Fimp/Misc Icons/FUnfolded");
                return __texDownFold;
            }
        }

        private static Texture2D __texDownFold;

        public static Texture2D Tex_RightFoldGray
        {
            get
            {
                if (__texRightFoldG != null) return __texRightFoldG;
                __texRightFoldG = Resources.Load<Texture2D>("Fimp/Misc Icons/FGRightFolded");
                return __texRightFoldG;
            }
        }

        private static Texture2D __texRightFoldG;

        public static Texture2D Tex_LeftFoldGray
        {
            get
            {
                if (__texLeftFoldG != null) return __texLeftFoldG;
                __texLeftFoldG = Resources.Load<Texture2D>("Fimp/Misc Icons/FGLeftFolded");
                return __texLeftFoldG;
            }
        }

        private static Texture2D __texLeftFoldG;

        public static Texture2D Tex_UpFoldGray
        {
            get
            {
                if (__texUpFoldG != null) return __texUpFoldG;
                __texUpFoldG = Resources.Load<Texture2D>("Fimp/Misc Icons/FGUpFolded");
                return __texUpFoldG;
            }
        }

        private static Texture2D __texUpFoldG;

        public static Texture2D Tex_DownFoldGray
        {
            get
            {
                if (__texDownFoldG != null) return __texDownFoldG;
                __texDownFoldG = Resources.Load<Texture2D>("Fimp/Misc Icons/FGUnfolded");
                return __texDownFoldG;
            }
        }

        private static Texture2D __texDownFoldG;

        public static Texture2D TexWaitIcon
        {
            get
            {
                if (__texWaitIcon != null) return __texWaitIcon;
                __texWaitIcon = Resources.Load<Texture2D>("Fimp/Misc Icons/FWait");
                return __texWaitIcon;
            }
        }

        private static Texture2D __texWaitIcon;

        public static Texture2D Tex_Random
        {
            get
            {
                if (__texRand != null) return __texRand;
                __texRand = Resources.Load<Texture2D>("Fimp/Misc Icons/FRandom");
                return __texRand;
            }
        }

        private static Texture2D __texRand;

        /// GUI Contents

        public static GUIContent GUIC_FoldGrayLeft
        {
            get
            {
                if (__guicFoldGrL != null) return __guicFoldGrL;
                __guicFoldGrL = new GUIContent(Tex_LeftFoldGray);
                return __guicFoldGrL;
            }
        }

        private static GUIContent __guicFoldGrL;

        public static GUIContent GUIC_FoldGrayRight
        {
            get
            {
                if (__guicFoldGrR != null) return __guicFoldGrR;
                __guicFoldGrR = new GUIContent(Tex_RightFoldGray);
                return __guicFoldGrR;
            }
        }

        private static GUIContent __guicFoldGrR;

        public static GUIContent GUIC_FoldGrayDown
        {
            get
            {
                if (__guicFoldGrD != null) return __guicFoldGrD;
                __guicFoldGrD = new GUIContent(Tex_DownFoldGray);
                return __guicFoldGrD;
            }
        }

        private static GUIContent __guicFoldGrD;

        public static GUIContent GUIC_FoldGrayUp
        {
            get
            {
                if (__guicFoldGrU != null) return __guicFoldGrU;
                __guicFoldGrU = new GUIContent(Tex_UpFoldGray);
                return __guicFoldGrU;
            }
        }

        private static GUIContent __guicFoldGrU;

        public static GUIContent GUIC_Info
        {
            get
            {
                if (__guicInfo != null) return __guicInfo;
                __guicInfo = new GUIContent(Tex_Info, "Click to display info dialog.");
                return __guicInfo;
            }
        }

        private static GUIContent __guicInfo;

        public static GUIContent GUIC_More
        {
            get
            {
                if (__guicMore != null) return __guicMore;
                __guicMore = new GUIContent(Tex_MoreMenu, "Click to display more options menu");
                return __guicMore;
            }
        }

        private static GUIContent __guicMore;

        public static GUIContent GUIC_Remove
        {
            get
            {
                if (__guicRemv != null) return __guicRemv;
                __guicRemv = new GUIContent(Tex_Remove, "Click to remove element");
                return __guicRemv;
            }
        }

        private static GUIContent __guicRemv;

        public static GUIContent GUIC_Save
        {
            get
            {
                if (__guicSave != null) return __guicSave;
                __guicSave = new GUIContent(Tex_Save, "Click to save file in project files.");
                return __guicSave;
            }
        }

        private static GUIContent __guicSave;


        /// Small Icons ----------------------------------------------------


        public static Texture2D Tex_Info
        {
            get
            {
                if (__texInfo != null) return __texInfo;
                __texInfo = Resources.Load<Texture2D>("Fimp/Small Icons/Finfo");
                return __texInfo;
            }
        }

        private static Texture2D __texInfo;

        public static Texture2D Tex_Save
        {
            get
            {
                if (__texSave != null) return __texSave;
                __texSave = Resources.Load<Texture2D>("Fimp/Icons/FSave");
                return __texSave;
            }
        }

        private static Texture2D __texSave;

        public static Texture2D Tex_Warning
        {
            get
            {
                if (__texWarning != null) return __texWarning;
                __texWarning = Resources.Load<Texture2D>("Fimp/Small Icons/FWarning");
                return __texWarning;
            }
        }

        private static Texture2D __texWarning;

        public static Texture2D Tex_Error
        {
            get
            {
                if (__texError != null) return __texError;
                __texError = Resources.Load<Texture2D>("Fimp/Small Icons/FError");
                return __texError;
            }
        }

        private static Texture2D __texError;

        public static Texture2D Tex_Bone
        {
            get
            {
                if (__texBone != null) return __texBone;
                __texBone = Resources.Load<Texture2D>("Fimp/Small Icons/FBone");
                return __texBone;
            }
        }

        private static Texture2D __texBone;

        public static Texture2D TexMotionIcon
        {
            get
            {
                if (__texMotIcon != null) return __texMotIcon;
                __texMotIcon = Resources.Load<Texture2D>("Fimp/Small Icons/Motion");
                return __texMotIcon;
            }
        }

        private static Texture2D __texMotIcon;

        public static Texture2D TexAddIcon
        {
            get
            {
                if (__texAddIcon != null) return __texAddIcon;
                __texAddIcon = Resources.Load<Texture2D>("Fimp/Small Icons/Additional");
                return __texAddIcon;
            }
        }

        private static Texture2D __texAddIcon;

        public static Texture2D TexTargetingIcon
        {
            get
            {
                if (__texTargIcon != null) return __texTargIcon;
                __texTargIcon = Resources.Load<Texture2D>("Fimp/Small Icons/Target");
                return __texTargIcon;
            }
        }

        private static Texture2D __texTargIcon;

        public static Texture2D TexBehaviourIcon
        {
            get
            {
                if (__texBehIcon != null) return __texBehIcon;
                __texBehIcon = Resources.Load<Texture2D>("Fimp/Small Icons/Behaviour");
                return __texBehIcon;
            }
        }

        private static Texture2D __texBehIcon;

        public static Texture2D TexSmallOptimizeIcon
        {
            get
            {
                if (__texSmOptimizeIcon != null) return __texSmOptimizeIcon;
                __texSmOptimizeIcon = Resources.Load<Texture2D>("Fimp/Small Icons/Optimize");
                return __texSmOptimizeIcon;
            }
        }

        private static Texture2D __texSmOptimizeIcon;

        public static Texture2D Tex_MiniGear
        {
            get
            {
                if (__texSGear != null) return __texSGear;
                __texSGear = Resources.Load<Texture2D>("Fimp/Small Icons/MiniGear");
                return __texSGear;
            }
        }

        private static Texture2D __texSGear;

        public static Texture2D Tex_Debug
        {
            get
            {
                if (__texDebg != null) return __texDebg;
                __texDebg = Resources.Load<Texture2D>("Fimp/Small Icons/FDebug");
                return __texDebg;
            }
        }

        private static Texture2D __texDebg;

        public static Texture2D Tex_Refresh
        {
            get
            {
                if (__texRefresh != null) return __texRefresh;
                __texRefresh = Resources.Load<Texture2D>("Fimp/Small Icons/FRefresh");
                return __texRefresh;
            }
        }

        private static Texture2D __texRefresh;

        public static Texture2D Tex_MiniMotion
        {
            get
            {
                if (__texMiniMotion != null) return __texMiniMotion;
                __texMiniMotion = Resources.Load<Texture2D>("Fimp/Small Icons/MiniMotion");
                return __texMiniMotion;
            }
        }

        private static Texture2D __texMiniMotion;

        public static Texture2D Tex_HiddenIcon
        {
            get
            {
                if (__texHiddenIcon != null) return __texHiddenIcon;
                __texHiddenIcon = Resources.Load<Texture2D>("Fimp/Small Icons/FHidden");
                return __texHiddenIcon;
            }
        }

        private static Texture2D __texHiddenIcon;

        public static Texture2D Tex_Collider
        {
            get
            {
                if (__texColl != null) return __texColl;
                __texColl = Resources.Load<Texture2D>("Fimp/Small Icons/FCollider");
                return __texColl;
            }
        }

        private static Texture2D __texColl;

        public static Texture2D Tex_Curve
        {
            get
            {
                if (__texCurve != null) return __texCurve;
                __texCurve = Resources.Load<Texture2D>("Fimp/Small Icons/FCurve");
                return __texCurve;
            }
        }

        private static Texture2D __texCurve;

        public static Texture2D Tex_Remove
        {
            get
            {
                if (__texRemove != null) return __texRemove;
                __texRemove = Resources.Load<Texture2D>("Fimp/Small Icons/FRemove");
                return __texRemove;
            }
        }

        private static Texture2D __texRemove;

        public static Texture2D Tex_Drag
        {
            get
            {
                if (__texDrag != null) return __texDrag;
                __texDrag = Resources.Load<Texture2D>("Fimp/Small Icons/FDragAndDrop");
                return __texDrag;
            }
        }

        private static Texture2D __texDrag;

        public static Texture2D Tex_Rename
        {
            get
            {
                if (__texRename != null) return __texRename;
                __texRename = Resources.Load<Texture2D>("Fimp/Small Icons/FRename");
                return __texRename;
            }
        }

        private static Texture2D __texRename;

        public static Texture2D Tex_Distance
        {
            get
            {
                if (__texDistanc != null) return __texDistanc;
                __texDistanc = Resources.Load<Texture2D>("Fimp/Small Icons/FDistance");
                return __texDistanc;
            }
        }

        private static Texture2D __texDistanc;

        public static Texture2D Tex_Movement
        {
            get
            {
                if (__texMovement != null) return __texMovement;
                __texMovement = Resources.Load<Texture2D>("Fimp/Small Icons/FMovement");
                return __texMovement;
            }
        }

        private static Texture2D __texMovement;

        public static Texture2D Tex_Rotation
        {
            get
            {
                if (__texRotation != null) return __texRotation;
                __texRotation = Resources.Load<Texture2D>("Fimp/Small Icons/FRotation");
                return __texRotation;
            }
        }

        private static Texture2D __texRotation;

        public static Texture2D Tex_Connections
        {
            get
            {
                if (__texConns != null) return __texConns;
                __texConns = Resources.Load<Texture2D>("Fimp/Small Icons/Connections");
                return __texConns;
            }
        }

        private static Texture2D __texConns;

        public static Texture2D Tex_MoreMenu
        {
            get
            {
                if (__texMoreMnu != null) return __texMoreMnu;
                __texMoreMnu = Resources.Load<Texture2D>("Fimp/Small Icons/FMore");
                return __texMoreMnu;
            }
        }

        private static Texture2D __texMoreMnu;

        public static Texture2D Tex_Preview
        {
            get
            {
                if (__texPrev != null) return __texPrev;
                __texPrev = Resources.Load<Texture2D>("Fimp/Small Icons/Preview");
                return __texPrev;
            }
        }

        private static Texture2D __texPrev;

        public static Texture2D Tex_Variables
        {
            get
            {
                if (__texVars != null) return __texVars;
                __texVars = Resources.Load<Texture2D>("Fimp/Small Icons/Variables");
                return __texVars;
            }
        }

        private static Texture2D __texVars;

        public static Texture2D Tex_VariablesArrows
        {
            get
            {
                if (__texVarsArr != null) return __texVarsArr;
                __texVarsArr = Resources.Load<Texture2D>("Fimp/Small Icons/Variables2");
                return __texVarsArr;
            }
        }

        private static Texture2D __texVarsArr;

        public static Texture2D Tex_Prepare
        {
            get
            {
                if (__texPrep != null) return __texPrep;
                __texPrep = Resources.Load<Texture2D>("Fimp/Small Icons/FPrepare");
                return __texPrep;
            }
        }

        private static Texture2D __texPrep;

        // Additional ----------------------------

        public static Texture2D Tex_SearchNumeric
        {
            get
            {
                if (__texSearchNum != null) return __texSearchNum;
                __texSearchNum = Resources.Load<Texture2D>("Fimp/Additional/SPR_SearchNumeric");
                return __texSearchNum;
            }
        }

        private static Texture2D __texSearchNum;

        public static Texture2D Tex_SearchDirectory
        {
            get
            {
                if (__texSearchDir != null) return __texSearchDir;
                __texSearchDir = Resources.Load<Texture2D>("Fimp/Additional/SPR_SearchDirectory");
                return __texSearchDir;
            }
        }

        private static Texture2D __texSearchDir;

        public static GUIStyle GetTextStyle(int size, bool bold, TextAnchor align)
        {
            var s = new GUIStyle(EditorStyles.label);

            s.fontSize = size;
            if (bold) s.fontStyle = FontStyle.Bold;
            s.alignment = align;

            return s;
        }

        public static string GetFoldSimbol(bool foldout = false, int size = 8, string hidden = "▲")
        {
            // ►
#if UNITY_2019_3_OR_NEWER

            if (EditorGUIUtility.isProSkin)
            {
                if (foldout) return "<size=" + size + "><color=#90909099>▼</color></size>";
                return "<size=" + size + "><color=#90909099>" + hidden + "</color></size>";
            }

            if (foldout) return "<size=" + size + "><color=#50505099>▼</color></size>";
            return "<size=" + size + "><color=#50505099>" + hidden + "</color></size>";
#else
            if (EditorGUIUtility.isProSkin)
            {
                if (foldout) return "<size=" + (size + 2) + "><color=#80808088>▼</color></size>";
                else
                    return "<size=" + (size + 2) + "><color=#80808088>" + hidden + "</color></size>";
            }
            else
            {
                if (foldout) return "<size=" + (size + 2) + "><color=#50505099>▼</color></size>";
                else
                    return "<size=" + (size + 2) + "><color=#50505099>" + hidden + "</color></size>";
            }

#endif
        }

        public static string GetFoldSimbol(bool foldout, bool rightArrow)
        {
            if (foldout) return "▼";

            if (rightArrow) return "►";
            return "▲";
        }

        public static GUIContent GetFoldSimbolTex(bool foldout, bool rightArrow)
        {
            if (foldout) return GUIC_FoldGrayDown;

            if (rightArrow) return GUIC_FoldGrayRight;
            return GUIC_FoldGrayUp;
        }
    }
}

#endif