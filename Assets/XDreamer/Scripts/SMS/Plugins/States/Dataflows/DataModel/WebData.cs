using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 网络数据:使用Unity的UnityWebRequest请求网络数据;仅可用于请求结果类型为文本类型；
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(WebData))]
    [Tip("使用Unity的UnityWebRequest请求网络数据;仅可用于请求结果类型为文本类型；", "Use unity's UnityWebRequest to request network data; It can only be used for request. The result type is text type;")]
    [XCSJ.Attributes.Icon(EIcon.Net)]
    public class WebData : Trigger<WebData>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "网络数据";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(WebData))]
        [Tip("使用Unity的UnityWebRequest请求网络数据;仅可用于请求结果类型为文本类型；", "Use unity's UnityWebRequest to request network data; It can only be used for request. The result type is text type;")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// URL:期望请求数据的网络URL路径
        /// </summary>
        [Name("URL")]
        [Tip("期望请求数据的网络URL路径", "Expected network URL path for requesting data")]
        public string url = "";

        /// <summary>
        /// 发生错误时继续请求:为True时，发生错误时会继续请求URL；为False时,不再请求数据并标记为完成态；
        /// </summary>
        [Name("发生错误时继续请求")]
        [Tip("为True时，发生错误时会继续请求URL；为False时,不再请求数据并标记为完成态；", "When true, the URL will continue to be requested when an error occurs; When it is false, data will not be requested and marked as completed;")]
        public bool continueRequestOnFailed = true;

        /// <summary>
        /// 结果变量:用于存放请求结果网络数据的中文脚本变量
        /// </summary>
        [Name("结果变量")]
        [Tip("用于存放请求结果网络数据的中文脚本变量", "Chinese script variable for storing network data of request results")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string variable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variable);
        }

        #endregion

        /// <summary>
        /// 当完成时回调
        /// </summary>
        public Action<WebData, WebDataInfo> onCompleted;

        /// <summary>
        /// 当失败时回调
        /// </summary>
        public Action<WebData> onFailed;

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            Request();
        }

        private void Request()
        {
            if (parent.isActive)
            {
                if (WebDataManager.Request(url, EDataType.Text, null, OnCompleted, OnFailed, null))
                {
                    //
                }
                else if (continueRequestOnFailed)
                {
                    CommonFun.DelayCall(Request);
                }
            }
        }

        private void OnCompleted(WebDataInfo webDataInfo, object tag)
        {
            variable.TrySetOrAddSetHierarchyVarValue(webDataInfo.dataInfo.text);
            finished = true;
            onCompleted?.Invoke(this, webDataInfo);
        }

        private void OnFailed(WebDataInfo webDataInfo, object tag, object error)
        {
            if (continueRequestOnFailed)
            {
                CommonFun.DelayCall(Request);
            }
            else
            {
                finished = true;
            }
            onFailed?.Invoke(this);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return "-->" + variable;
        }
    }
}
