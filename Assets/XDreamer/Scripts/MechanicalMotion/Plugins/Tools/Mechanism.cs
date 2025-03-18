using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 运动机构
    /// </summary>
    [RequireManager(typeof(MechanicalMotionManager))]
    [Owner(typeof(MechanicalMotionManager))]
    [DisallowMultipleComponent]
    [Name("机械机构")]
    public class Mechanism : InteractProvider
    {
        #region 机构名称与路径

        /// <summary>
        /// 自定义机构名称
        /// </summary>
        [Name("自定义机构名称")]
        public bool _customMechanismName = false;

        /// <summary>
        /// 机构名称
        /// </summary>
        [Name("机构名称")]
        [HideInSuperInspector(nameof(_customMechanismName), EValidityCheckType.False)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _mechanismName = string.Empty;

        /// <summary>
        /// 机构名称
        /// </summary>
        public string mechanismName => _customMechanismName ? _mechanismName : name;

        /// <summary>
        /// 机构路径
        /// </summary>
        public string path
        {
            get
            {
                string path = "/" + mechanismName;

                transform.GetComponentByForeachTransformParent<Mechanism>((t, m) =>
                {
                    path = "/" + m.mechanismName + path;
                    return true;
                });

                return path;
            }
        }

        /// <summary>
        /// 直接父级, 包含非激活对象
        /// </summary>
        public Mechanism parent
        {
            get
            {
                Mechanism result = null;
                transform.GetComponentByForeachTransformParent<Mechanism>((t, m) => { result = m; return false; });
                return result;
            }
        }

        /// <summary>
        /// 所有父级, 包含非激活对象
        /// </summary>
        public List<Mechanism> parents
        {
            get
            {
                var list = new List<Mechanism>();
                transform.GetComponentByForeachTransformParent<Mechanism>((t, m) => { list.Add(m); return true; });
                return list;
            }
        }

        /// <summary>
        /// 子级：不包含自身
        /// </summary>
        public Mechanism[] children => transform.GetComponentsInChildren<Mechanism>(true).Where(m => m.parent == this).ToArray();

        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (CanDoMotion())
            {
                DoMotion();
            }
        }

        /// <summary>
        /// 能否做运动
        /// </summary>
        /// <returns></returns>
        public virtual bool CanDoMotion() => true;

        /// <summary>
        /// 做运动
        /// </summary>
        public virtual void DoMotion() { }
    }
}
