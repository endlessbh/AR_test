using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Tools;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 零件检查器
    /// </summary>
    [Name("零件检查器")]
    [CustomEditor(typeof(Part), true)]
    public class PartInspector : PartInspector<Part>
    {
        /// <summary>
        /// 绘制零件分类检查器界面
        /// </summary>
        /// <param name="part"></param>
        public static void DrawPartCategoryName(Part part)
        {
            if (!part) return;

            //var so = new SerializedObject(part);

            //var sp = so.FindProperty(nameof(Part._tagInfo));

            //EditorGUILayout.PropertyField(sp, PropertyData.GetPropertyData(sp).trLabel);

            //so.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// 零件检查器模板
    /// </summary>
    public class PartInspector<T> : InteractObjectInspector<T> where T : Part
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}