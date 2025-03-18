using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 屏幕组检查器
    /// </summary>
    [Name("屏幕组检查器")]
    [CustomEditor(typeof(ScreenGroup))]
    public class ScreenGroupInspector : BaseScreenInspector<ScreenGroup>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDetailInfos();
        }

        /// <summary>
        /// 虚拟屏幕列表
        /// </summary>
        [Name("虚拟屏幕列表")]
        [Tip("当前屏幕组管理的虚拟屏幕对象", "Virtual screen objects managed by the current screen group")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();
            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("虚拟屏幕", "虚拟屏幕所在的游戏对象；本项只读；"), UICommonOption.Width120);
            GUILayout.Label(CommonFun.TempContent("屏幕尺寸", "虚拟屏幕的真实物理尺寸；X为宽，Y为高,Z为厚度；单位：米；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var group = targetObject;
            var groupTransform = group.transform;
            var cache = group.GetComponentsInChildren<BaseScreen>().Where(s => s.transform.parent == groupTransform);
            int i = 0;
            foreach (var component in cache)
            {
                UICommonFun.BeginHorizontal(i++);

                //编号
                EditorGUILayout.LabelField(i.ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true, UICommonOption.Width120);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var screenSize = EditorGUILayout.Vector3Field("", component.screenSize);
                if (EditorGUI.EndChangeCheck())
                {
                    component.screenSize = screenSize;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
