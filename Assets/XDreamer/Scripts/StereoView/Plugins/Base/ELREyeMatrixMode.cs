using XCSJ.Attributes;

namespace XCSJ.PluginStereoView.Base
{
    /// <summary>
    /// 左右眼矩阵模式：仅在立体启用时有效
    /// </summary>
    public enum ELREyeMatrixMode
    {
        /// <summary>
        /// 无:不对左右眼进行任何设置操作
        /// </summary>
        [Name("无")]
        [Tip("不对左右眼进行任何设置操作")]
        None,

        /// <summary>
        /// 与中心相同:左右眼的投影矩阵与视图矩阵与中心的对应矩阵相同
        /// </summary>
        [Name("与中心相同")]
        [Tip("左右眼的投影矩阵与视图矩阵与中心的对应矩阵相同")]
        SameWithCenter,

        /// <summary>
        /// 使用中心投影矩阵:左右眼的投影矩阵中心的投影矩阵相同，左右眼的视图矩阵单独计算
        /// </summary>
        [Name("使用中心投影矩阵")]
        [Tip("左右眼的投影矩阵中心的投影矩阵相同，左右眼的视图矩阵单独计算")]
        UseCenterProjectionMatrix,

        /// <summary>
        /// 完整计算:左右眼的投影矩阵与视图矩阵均执行完整的单独计算
        /// </summary>
        [Name("完整计算")]
        [Tip("左右眼的投影矩阵与视图矩阵均执行完整的单独计算")]
        CompleteCalculation,
    }
}
