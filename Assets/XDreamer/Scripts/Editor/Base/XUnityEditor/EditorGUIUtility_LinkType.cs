using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类<see cref="EditorGUIUtility"/>的关联类型
    /// </summary>
    [LinkType(typeof(EditorGUIUtility))]
    public class EditorGUIUtility_LinkType : GUIUtility_LinkType<EditorGUIUtility_LinkType>
    {
        #region skinIndex

        public static XPropertyInfo skinIndex_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(skinIndex), TypeHelper.StaticNotPublic);

        public static int skinIndex
        {
            get
            {
                return (int)skinIndex_PropertyInfo.GetValue(null);
            }
        }

        #endregion

        #region SetBoldDefaultFont

        public static XMethodInfo SetBoldDefaultFont_MethodInfo { get; } = new XMethodInfo(Type,nameof(SetBoldDefaultFont),TypeHelper.StaticNotPublic);

        public static void SetBoldDefaultFont(bool isBold)
        {
            SetBoldDefaultFont_MethodInfo.Invoke(null, new object[] { isBold });
        }

        #endregion

        #region GetIconForObject

        public static XMethodInfo GetIconForObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetIconForObject), TypeHelper.StaticNotPublic);

        public static Texture2D GetIconForObject(Object obj)
        {
            return (Texture2D)GetIconForObject_MethodInfo.Invoke(null, new object[] { obj });
        }

        #endregion

        #region SetIconForObject

        public static XMethodInfo SetIconForObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetIconForObject), TypeHelper.StaticNotPublic);

        public static void SetIconForObject(Object obj, Texture2D icon)
        {
            SetIconForObject_MethodInfo.Invoke(null, new object[] { obj, icon });
        }

        #endregion

        #region GUITextureBlit2SRGBMaterial

        public static XPropertyInfo GUITextureBlit2SRGBMaterial_PropertyInfo { get; } = GetXPropertyInfo(nameof(GUITextureBlit2SRGBMaterial));

        public static Material GUITextureBlit2SRGBMaterial => (Material)GUITextureBlit2SRGBMaterial_PropertyInfo.GetValue(null);

        #endregion
    }
}
