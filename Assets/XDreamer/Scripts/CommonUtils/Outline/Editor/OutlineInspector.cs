using UnityEditor;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginOutline;
using XCSJ.EditorCommonUtils;

namespace XCSJ.CommonUtils.EditorOutline
{
    /// <summary>
    /// 轮廓线检查器
    /// </summary>
    [Name("轮廓线检查器")]
    [CustomEditor(typeof(Outline))]
    public class OutlineInspector : MBInspector<Outline>
    {

    }

}