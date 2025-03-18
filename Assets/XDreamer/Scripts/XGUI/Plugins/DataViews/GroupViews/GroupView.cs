using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 组视图
    /// </summary>
    [Name("组视图")]
    public abstract class GroupView : BaseModelView
    {
        /// <summary>
        /// 视图组件父级
        /// </summary>
        [Group("组视图", textEN = "Group view")]
        [Name("视图组件父级")]
        [Tip("为空则使用当前组件对象作为父级", "If it is Null, the current component object is used as the parent")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _parent;

        /// <summary>
        /// 包含基类
        /// </summary>
        [Name("包含基类")]
        public bool _includeBaseType = false;

        /// <summary>
        /// 视图绑定数据类型成员模式
        /// </summary>
        [Name("视图绑定数据类型成员模式")]
        [EnumPopup]
        public EViewBindDataTypeMemberMode _viewBindDataTypeMemberMode = EViewBindDataTypeMemberMode.Field | EViewBindDataTypeMemberMode.Property | EViewBindDataTypeMemberMode.Method;

        /// <summary>
        /// 绑定数据：除了绑定主对象数据，还将其子对象数据也进行绑定
        /// </summary>
        /// <param name="target"></param>
        public override void BindModel(Object target)
        {
            base.BindModel(target);

            // 使用已创建的数据视图
            var views = GetComponentsInChildren<BaseModelView>(true);
            foreach (var v in views)
            {
                if (v != this)
                {
                    v.BindModel(target);
                }
            }
        }

        /// <summary>
        /// 创建视图集
        /// </summary>
        /// <param name="data"></param>
        protected virtual void CreateViewsInternal(UnityEngine.Object data)
        {
            DataViewHelper.CreateMemberDataViews(data, _viewBindDataTypeMemberMode, _includeBaseType, _parent ? _parent : transform);
        }

        /// <summary>
        /// 设置子视图是否激活
        /// </summary>
        /// <param name="active"></param>
        protected void SetChildrenViewsActive(bool active)
        {
            var views = GetComponentsInChildren<BaseModelView>(true);
            foreach (var v in views)
            {
                if (v != this && v) v.gameObject.XSetActive(active);
            }
        }
    }

}
