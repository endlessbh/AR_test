using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 摇杆:朝向指定关节摆动的机构
    /// </summary>
    [Name("摇杆")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    public class Rocker : Mechanism
    {
        /// <summary>
        /// 朝向关节
        /// </summary>
        [Name("朝向关节")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _lookatJoint;

        /// <summary>
        /// 朝向上向量
        /// </summary>
        [Name("朝向上向量")]
        [Tip("设定变换朝向时，需指定一个参考上向量才能正确设定朝向", "When setting the transformation orientation, you need to specify a reference upper vector to correctly set the orientation")]
        public Vector3Data _lookatUpAxis = new Vector3Data();

        /// <summary>
        /// 重置：设定朝向上向量引用为自身,默认方向为上
        /// </summary>
        public void Reset()
        {
            _lookatUpAxis._transform._transfrom = transform;
            _lookatUpAxis._dataType = EVector3DataType.Vector3;
            _lookatUpAxis._vector3._value = Vector3.up;
        }

        /// <summary>
        /// 能否做运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion() => _lookatJoint;

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion() => transform.LookAt(_lookatJoint, _lookatUpAxis.data);
    }
}
