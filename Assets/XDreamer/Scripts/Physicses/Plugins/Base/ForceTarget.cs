using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginPhysicses.Base
{
    /// <summary>
    /// 施力目标
    /// </summary>
    [Serializable]
    [Name("施力目标")]
    public class ForceTarget
    {
        /// <summary>
        /// 力作用目标规则
        /// </summary>
        [Name("力作用目标规则")]
        [EnumPopup]
        public EForceTargetRule _forceTargetType = EForceTargetRule.RaycastHitObject;

        /// <summary>
        /// 射线拾取器
        /// </summary>
        [Name("射线拾取器")]
        [HideInSuperInspector(nameof(_forceTargetType), EValidityCheckType.NotEqual, EForceTargetRule.RaycastHitObject)]
        [OnlyMemberElements]
        public RayHitter _rayPicker = new RayHitter();

        /// <summary>
        /// 指定刚体
        /// </summary>
        [Name("指定刚体")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_forceTargetType), EValidityCheckType.NotEqual, EForceTargetRule.Fixed)]
        public Rigidbody _rigidbody;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ForceTarget() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="forceTargetType">力作用对象类型</param>
        public ForceTarget(EForceTargetRule forceTargetType) { _forceTargetType = forceTargetType; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Rigidbody GetObject()
        {
            switch (_forceTargetType)
            {
                case EForceTargetRule.RaycastHitObject:
                    {
                        return _rayPicker.PickRigidbody();
                    }
                case EForceTargetRule.Selection:
                    {
                        if (Selection.selection)
                        {
                            return Selection.selection.GetComponent<Rigidbody>();
                        }
                        break;
                    }
                case EForceTargetRule.Fixed:
                    {
                        return _rigidbody;
                    }
            }
            return null;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            if (_forceTargetType == EForceTargetRule.Fixed)
            {
                return _rigidbody;
            }
            return true; 
        }
    }


    /// <summary>
    /// 力作用目标规则
    /// </summary>
    [Name("力作用目标规则")]
    public enum EForceTargetRule
    {
        [Name("无")]
        None,

        [Name("射线击中对象")]
        RaycastHitObject,

        [Name("选择集")]
        Selection,

        [Name("固定对象")]
        Fixed,
    }
}
