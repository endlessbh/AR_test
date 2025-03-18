using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XText
{
    /// <summary>
    /// 文本绑定器
    /// </summary>
    [Name("文本绑定器")]
    [DataBinder(typeof(Text), nameof(Text.text))]
    public class Text_text_Binder : TypeMemberDataBinder<Text>
    {
        /// <summary>
        /// 尝试获取成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            if (obj is Text entity && entity)
            {
                value = entity.text;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            if (obj is Text entity && entity && TryConvertTo(value, out string outValue))
            {
                entity.text = outValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(XValueEventArg eventArg, out object value)
        {
            value = target.text;
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public override void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg) 
        {
            if (linkedBindData.TryGet(eventArg, out object value) && TryConvertTo(value, out string outValue))
            {
                target.text = outValue;
            }
        }
    }
}
