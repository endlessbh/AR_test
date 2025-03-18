using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.Scripts;

namespace XCSJ.PluginPhysicses.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region Physics-目录
        /// <summary>
        /// 物理系统
        /// </summary>
        [ScriptName(PhysicsManager.Title, nameof(PhysicsSystem), EGrammarType.Category)]
        [ScriptDescription("物理系统的相关脚本目录；")]
        #endregion
        PhysicsSystem,

        #region 刚体设置
        /// <summary>
        /// 刚体设置
        /// </summary>
        [ScriptName("刚体设置", nameof(SetRigidbody))]
        [ScriptDescription("设置刚体的各个参数;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "刚体对象（限定Rigidbody类型）：", "", typeof(Rigidbody))]
        [ScriptParams(2, EParamType.Bool, "受重力影响:")]
        #endregion 刚体设置
        SetRigidbody,

        #region 设置运动约束
        /// <summary>
        /// 设置运动约束
        /// </summary>
        [ScriptName("运动约束设置", nameof(SetJoint))]
        [ScriptDescription("设置运动约束的各个参数;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "连接对象（限定Joint类型）：", "", typeof(Joint))]
        [ScriptParams(2, EParamType.GameObjectComponent, "连接体对象（限定Rigidbody类型）：", "", typeof(Rigidbody))]
        #endregion 设置运动约束
        SetJoint,

        #region 网格破坏器操作
        /// <summary>
        /// 网格破坏器操作
        /// </summary>
        [ScriptName("网格破坏器操作", nameof(MeshDamagerOperation))]
        [ScriptDescription("网格破坏器操作")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "网格破坏器", typeof(MeshDamager))]
        [ScriptParams(2, EParamType.Combo, "类型:", "开始修复", "停止修复")]
        [ScriptParams(3, EParamType.Float, "修复速度", 0, 10)]
        #endregion
        MeshDamagerOperation,
    }
}

