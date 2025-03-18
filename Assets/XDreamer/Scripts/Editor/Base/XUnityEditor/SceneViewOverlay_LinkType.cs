using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类UnityEditor.SceneViewOverlay的关联类型
    /// </summary>
    [LinkType("UnityEditor.SceneViewOverlay")]
    public class SceneViewOverlay_LinkType : LinkType<SceneViewOverlay_LinkType>
    {
        public SceneViewOverlay_LinkType() { }

        public SceneViewOverlay_LinkType(object obj) : base(obj) { }

        public static XObject<Type> WindowFunction_Type { get; } = new XObject<Type>(x => Type.GetMethods().FirstOrDefault(mi => mi.Name == nameof(Window) && mi.GetParameters().Length == 4).GetParameters()[1].ParameterType);
        public static XObject<Type> WindowDisplayOption_Type { get; } = new XObject<Type>(x => Type.GetMethods().FirstOrDefault(mi => mi.Name == nameof(Window) && mi.GetParameters().Length == 4).GetParameters()[3].ParameterType);

        public static XMethodInfo Window_GUIContent_WindowFunction_int_Object_WindowDisplayOption_MethodInfo { get; } = GetXMethodInfo(nameof(Window), new Type[] { typeof(GUIContent), WindowFunction_Type, typeof(int), typeof(UnityEngine.Object), WindowDisplayOption_Type });

        /// <summary>
        /// 绘制窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="sceneViewFunc">需匹配<see cref="WindowFunction_Type"/>的约束类型</param>
        /// <param name="order"></param>
        /// <param name="target"></param>
        /// <param name="option"></param>
        public static void Window(GUIContent title, Delegate sceneViewFunc, int order, UnityEngine.Object target, int option)
        {
            Window_GUIContent_WindowFunction_int_Object_WindowDisplayOption_MethodInfo.Invoke(null, new object[] { title, sceneViewFunc, order, target, option });
        }

        public static XMethodInfo Window_GUIContent_WindowFunction_int_Object_WindowDisplayOption_EditorWindow_MethodInfo { get; } = GetXMethodInfo(nameof(Window), new Type[] { typeof(GUIContent), WindowFunction_Type, typeof(int), typeof(UnityEngine.Object), WindowDisplayOption_Type, typeof(EditorWindow) });

        /// <summary>
        /// 绘制窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="sceneViewFunc">需匹配<see cref="WindowFunction_Type"/>的约束类型</param>
        /// <param name="order"></param>
        /// <param name="target"></param>
        /// <param name="option"></param>
        /// <param name="window"></param>
        public static void Window(GUIContent title, Delegate sceneViewFunc, int order, UnityEngine.Object target, int option, EditorWindow window)
        {
            Window_GUIContent_WindowFunction_int_Object_WindowDisplayOption_EditorWindow_MethodInfo.Invoke(null, new object[] { title, sceneViewFunc, order, target, option, window });
        }
    }
}
