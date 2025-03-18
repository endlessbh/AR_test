using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.ExplodedViews.States;

namespace XCSJ.EditorTools.ExplodedViews.States
{
    /// <summary>
    /// 爆炸图检查器
    /// </summary>
    [Name("爆炸图检查器")]
    [CustomEditor(typeof(ExplodedView))]
    public class ExplodedViewInspector : WorkClipInspector<ExplodedView>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(WorkClip.useInitData):
                    {
                        return ;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
