using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Characters.Base;
using XCSJ.Extension.Characters;
using XCSJ.EditorTools;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Characters
{
    /// <summary>
    /// 角色变换器检查器
    /// </summary>
    [Name("角色变换器检查器")]
    [CustomEditor(typeof(CharacterTransformer), true)]
    public class CharacterTransformerInspector : BaseCharacterTransformerInspector<CharacterTransformer>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(CharacterTransformer)));

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
