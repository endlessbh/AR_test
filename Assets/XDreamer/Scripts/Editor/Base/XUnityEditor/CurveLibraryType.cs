using XCSJ.Algorithms;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 曲线库类型
    /// </summary>
    [Name("曲线库类型")]
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurveLibraryType))]
    public enum CurveLibraryType
    {
        Unbounded,
        NormalizedZeroToOne
    }
}
