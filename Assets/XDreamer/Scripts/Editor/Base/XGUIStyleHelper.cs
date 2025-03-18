using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Kernel;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// XGUIStyle
    /// </summary>
    public class XGUIStyleHelper
    {
        private static string commonTexturePath { get; } = UICommonFun.GetAssetsPath(EFolder.EditorResources_Common) + "/Textures/";

        private static void SetDefaultHoverColor(GUIStyle style)
        {
            CommonGUIStyle.SetHoverColor(style, EditorGUIUtility.isProSkin ? CommonGUIStyle.hoverColor_Professional : CommonGUIStyle.hoverColor_Personal);
        }

        [XGUIStyleRegister(nameof(None))]
        public static XGUIStyle None()
        {
            return new XGUIStyle(nameof(GUIStyle.none));
        }

        [XGUIStyleRegister(nameof(Logo))]
        public static XGUIStyle Logo()
        {
            return new XGUIStyle(nameof(GUIStyle.none), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Logo));
                s.alignment = TextAnchor.MiddleLeft;
                s.padding = new RectOffset(20, 20, 20, 20);
                s.normal.background = UICommonFun.LoadFromAssets<Texture2D>(commonTexturePath + "Logo" + EditorIconHelper.DefaultIconExt);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Label_Head1))]
        public static XGUIStyle Label_Head1()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Head1)); 
                s.richText = true;
                s.fontSize = 20;
                s.alignment = TextAnchor.MiddleLeft;
            });
        }

        [XGUIStyleRegister(nameof(Label_Head2))]
        public static XGUIStyle Label_Head2()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Head2));
                s.richText = true;
                s.fontSize = 16;
                s.alignment = TextAnchor.MiddleLeft;
                s.fontStyle = FontStyle.Bold;
            });
        }

        [XGUIStyleRegister(nameof(Label_Head3))]
        public static XGUIStyle Label_Head3()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Head3));
                s.richText = true;
                s.fontSize = 14;
                s.alignment = TextAnchor.MiddleLeft;
                s.fontStyle = FontStyle.Bold;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal))]
        public static XGUIStyle Label_Normal()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal));
                s.alignment = TextAnchor.MiddleLeft;
                s.fontSize = 13;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_14))]
        public static XGUIStyle Label_Normal_14()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_14));
                s.margin = new RectOffset();
                s.alignment = TextAnchor.MiddleLeft;
                s.fontSize = 14;
                SetDefaultHoverColor(s);
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_16))]
        public static XGUIStyle Label_Normal_16()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_16));
                s.margin = new RectOffset();
                s.alignment = TextAnchor.MiddleLeft;
                s.fontSize = 16;
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Label_Normal_Center))]
        public static XGUIStyle Label_Normal_Center()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Center));
                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = 14;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_Center_14))]
        public static XGUIStyle Label_Normal_Center_14()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Center_14));
                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = 14;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_Center_16))]
        public static XGUIStyle Label_Normal_Center_16()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Center_16));
                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = 16;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_Right))]
        public static XGUIStyle Label_Normal_Right()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Right));
                s.alignment = TextAnchor.MiddleRight;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_Right_14))]
        public static XGUIStyle Label_Normal_Right_14()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Right_14));
                s.alignment = TextAnchor.MiddleRight;
                s.fontSize = 14;
            });
        }

        [XGUIStyleRegister(nameof(Label_Normal_Right_16))]
        public static XGUIStyle Label_Normal_Right_16()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Normal_Right_16));
                s.alignment = TextAnchor.MiddleRight;
                s.fontSize = 16;
            });
        }

        [XGUIStyleRegister(nameof(Label_Selected))]
        public static XGUIStyle Label_Selected()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Selected));
                s.margin = new RectOffset();
                s.normal.background = UICommonOption.GetSelectedTexture();
                SetDefaultHoverColor(s);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Label_Selected_14))]
        public static XGUIStyle Label_Selected_14()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Selected_14));
                s.margin = new RectOffset();
                s.fontSize = 14;
                s.normal.background = UICommonOption.GetSelectedTexture();
                SetDefaultHoverColor(s);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Label_Selected_16))]
        public static XGUIStyle Label_Selected_16()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Selected_16));
                s.margin = new RectOffset();
                s.fontSize = 16;
                s.normal.background = UICommonOption.GetSelectedTexture();
                SetDefaultHoverColor(s);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Label_Http))]
        public static XGUIStyle Label_Http()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Label_Http));
                s.normal.textColor = CommonGUIStyle.selectedColor_Personal;
                SetDefaultHoverColor(s);
            });
        }

        [XGUIStyleRegister(nameof(EGUIStyle.Separator))]
        public static XGUIStyle Separator()
        {
            return new XGUIStyle(nameof(GUIStyle.none), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Separator));
                s.normal.background = UICommonOption.GetSelectedTexture(); 
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Foldout_13))]
        public static XGUIStyle Foldout_13()
        {
            return new XGUIStyle(nameof(EditorStyles.foldout), s =>
            {
                //s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Foldout_13));
                s.fontSize = 13;
                s.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.1f, 0.1f, 0.1f);
                s.onNormal.textColor = s.normal.textColor;
            });
        }

        [XGUIStyleRegister(nameof(Box))]
        public static XGUIStyle Box()
        {
#if UNITY_2019_3_OR_NEWER
            return new XGUIStyle("GroupBox", s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Box));
                s.border = new RectOffset(3, 3, 2, 2);
                s.margin = new RectOffset(4, 4, 4, 4);
                s.padding = new RectOffset(3, 3, 3, 3);
            });
#else
            return new XGUIStyle(nameof(GUI.skin.box), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Box));
            });
#endif
        }

        [XGUIStyleRegister(nameof(Box_Blue))]
        public static XGUIStyle Box_Blue()
        {
            return new XGUIStyle(nameof(GUI.skin.box), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Box_Blue));
                s.richText = true;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.background = UICommonFun.LoadFromAssets<Texture2D>(commonTexturePath + nameof(Box_Blue) + EditorIconHelper.DefaultIconExt);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Box_Solid))]
        public static XGUIStyle Box_Solid()
        {
            return new XGUIStyle(nameof(GUI.skin.box), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Box_Solid));
                s.richText = true;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.background = UICommonFun.LoadFromAssets<Texture2D>(commonTexturePath + (EditorGUIUtility.isProSkin ? nameof(Box_Solid) : (nameof(Box_Solid) + "." + XDreamerIconOption.weakInstance.GetSkinIconMarker())) + EditorIconHelper.DefaultIconExt);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Button_NoneBackground))]
        public static XGUIStyle Button_NoneBackground()
        {
            return new XGUIStyle(nameof(GUI.skin.button), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Button_NoneBackground));
                s.alignment = TextAnchor.MiddleLeft;
                s.border = new RectOffset(0, 0, 0, 0);
                s.padding = new RectOffset(2, 2, 2, 2);
                s.normal.background = null;
                s.onNormal.background = null;
                s.active.background = UICommonOption.GetSelectedTexture(); 
                s.onActive.background = s.active.background;
            }, s => s != null && s.active.background);
        }

        [XGUIStyleRegister(nameof(Button_NoneBackground_Hover))]
        public static XGUIStyle Button_NoneBackground_Hover()
        {
            return new XGUIStyle(nameof(GUI.skin.label), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Button_NoneBackground_Hover));
                s.alignment = TextAnchor.MiddleLeft;
                s.normal.background = null;
                s.onNormal.background = null;

                s.hover.background = Texture2DHelper.GetTexture2D(EditorGUIUtility.isProSkin ? CommonGUIStyle.hoverColor_Professional : CommonGUIStyle.hoverColor_Personal);
                s.onHover.background = s.hover.background;
            }, s => s != null && s.hover.background);
        }

        [XGUIStyleRegister(nameof(Button_Border_Blue))]
        public static XGUIStyle Button_Border_Blue()
        {
            return new XGUIStyle(nameof(GUI.skin.button), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Button_Border_Blue));
                s.richText = true;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.background = UICommonFun.LoadFromAssets<Texture2D>(commonTexturePath + "RoundBox" + EditorIconHelper.DefaultIconExt);
                s.normal.textColor = Color.white;
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(Button_Blue))]
        public static XGUIStyle Button_Blue()
        {
            return new XGUIStyle(nameof(GUI.skin.button), s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.Button_Blue));
                s.richText = true;
                s.alignment = TextAnchor.MiddleCenter;
                s.normal.background = UICommonFun.LoadFromAssets<Texture2D>(commonTexturePath + "RoundSolidRectBlue" + EditorIconHelper.DefaultIconExt);
                s.normal.textColor = Color.white;
                s.margin = new RectOffset(2,2,2,2);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }

        [XGUIStyleRegister(nameof(SelectionBorder))]
        public static XGUIStyle SelectionBorder()
        {
            return new XGUIStyle("LightmapEditorSelectedHighlight", s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.SelectionBorder));
                s.richText = true;
                s.overflow = new RectOffset(3, 3, 3, 3);
                s.border = new RectOffset(6, 6, 6, 6);
                s.normal.background = EditorIconHelper.GetIconInLib(EIcon.Border);
            }, XGUIStyle.DefaultValidFuncWithNormalBackground);
        }
        
        [XGUIStyleRegister(nameof(NavigationLeftBar))]
        public static XGUIStyle NavigationLeftBar()
        {
#if UNITY_2019_3_OR_NEWER
            return new XGUIStyle("GUIEditor.BreadcrumbLeftBackground", s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.NavigationLeftBar));
                s.alignment = TextAnchor.MiddleCenter;
                s.padding = new RectOffset(5, 11, 2, 2);
            });
#else
            return new XGUIStyle("GUIEditor.BreadcrumbLeft", s=>
            { 
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.NavigationLeftBar));
                s.richText = true;
            });
#endif
        }

        [XGUIStyleRegister(nameof(NavigationMiddleBar))]
        public static XGUIStyle NavigationMiddleBar()
        {
#if UNITY_2019_3_OR_NEWER
            return new XGUIStyle("GUIEditor.BreadcrumbMidBackground", s =>
            {
                s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.NavigationMiddleBar));
                s.alignment = TextAnchor.MiddleCenter;
                s.padding = new RectOffset(11, 11, 2, 2);
            });
#else
            return new XGUIStyle("GUIEditor.BreadcrumbMid", s => s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(EGUIStyle.NavigationMiddleBar)));
#endif
        }

    }
}
