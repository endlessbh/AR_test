using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.MiniMaps;

namespace XCSJ.EditorXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图检查器
    /// </summary>
    [Name("导航图检查器")]
    [CustomEditor(typeof(MiniMap))]
    public class MiniMapInspector : InteractorInspector<MiniMap>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("MiniMap Init Size", "导航图初始大小")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(MiniMap.miniMapCamera):
                    {
                        base.OnDrawMember(serializedProperty, propertyData);

                        // 当前检查器中绘制:
                        // 1、导航图跟随相机或玩家旋转设置
                        // 2、导航图初始大小设置
                        var camController = targetObject.miniMapCamera;
                        if (camController)
                        {
                            EditorGUI.BeginChangeCheck();
                            var autoFollow = EditorGUILayout.Toggle(CommonFun.NameTip(typeof(MiniMapCameraController), nameof(MiniMapCameraController.autoFollow)), camController.autoFollow);
                            if (EditorGUI.EndChangeCheck())
                            {
                                camController.XModifyProperty(() => camController.autoFollow = autoFollow);
                            }

                            var cam = camController.linkCamera;
                            if (cam)
                            {
                                EditorGUI.BeginChangeCheck();
                                var size = EditorGUILayout.FloatField(Tr("MiniMap Init Size"), cam.orthographicSize);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    cam.XModifyProperty(() => cam.orthographicSize = size);
                                }
                            }
                        }
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(MiniMap)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}