using System;
using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 渲染器数据记录器:渲染器数据记录器组件是用于还原程序启动时渲染器的颜色、材质和可用性等属性的执行体。组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererDataRecorder))]
    [Tip("渲染器数据记录器组件是用于还原程序启动时渲染器的颜色、材质和可用性等属性的执行体。组件执行完毕后切换为完成态。", "The renderer datalogger component is the executor used to restore the color, material, availability and other attributes of the renderer when the program starts. After the component is executed, it is switched to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33648)]
    [RequireComponent(typeof(GameObjectSet))]
    public class RendererDataRecorder : DataRecorder<RendererDataRecorder, RendererDataRecorder.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器数据记录器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(RendererDataRecorder))]
        [Tip("渲染器数据记录器组件是用于还原程序启动时渲染器的颜色、材质和可用性等属性的执行体。组件执行完毕后切换为完成态。", "The renderer datalogger component is the executor used to restore the color, material, availability and other attributes of the renderer when the program starts. After the component is executed, it is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集合
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : RendererRecorder, IRecoverableDataRecorder<RendererDataRecorder>
        {
            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rendererDataRecorder"></param>
            public void Record(RendererDataRecorder rendererDataRecorder)
            {
                if (!rendererDataRecorder.gameObjectSet) return;
                _records.Clear();
                Record(rendererDataRecorder.gameObjectSet.objects);
            }

            /// <summary>
            /// 恢复
            /// </summary>
            /// <param name="dataRecoveryRule"></param>
            /// <param name="dataRecoveryRuleValue"></param>
            public void Recovery(EDataRecoveryRule dataRecoveryRule, string dataRecoveryRuleValue)
            {
                switch (dataRecoveryRule)
                {
                    case EDataRecoveryRule.All:
                        {
                            Recover();
                            break;
                        }
                    case EDataRecoveryRule.NameEquals:
                        {
                            Recover(i => i.renderer && i.renderer.name == dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameNotEquals:
                        {
                            Recover(i => i.renderer && i.renderer.name != dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameContains:
                        {
                            Recover(i => i.renderer && i.renderer.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameNotContains:
                        {
                            Recover(i => i.renderer && !i.renderer.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexMatch:
                        {
                            Recover(i => i.renderer && Regex.IsMatch(i.renderer.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexNotMatch:
                        {
                            Recover(i => i.renderer && !Regex.IsMatch(i.renderer.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.IsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.renderer && i.renderer.transform.IsChildOf(t));
                            }
                            break;
                        }
                    case EDataRecoveryRule.NotIsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.renderer && !i.renderer.transform.IsChildOf(t));
                            }
                            break;
                        }
                    case EDataRecoveryRule.None:
                    default:
                        {
                            break;
                        }
                }
            }
        }
    }
}
