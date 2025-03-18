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
    /// 变换数据记录器:变换数据记录器组件是用于还原程序启动时转换的位置、角度和缩放信息的执行体。组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(TransformDataRecorder))]
    [RequireComponent(typeof(GameObjectSet))]
    [Tip("变换数据记录器组件是用于还原程序启动时转换的位置、角度和缩放信息的执行体。组件执行完毕后切换为完成态。", "The conversion datalogger component is the actuator used to restore the position, angle and zoom information converted at the start of the program. After the component is executed, it is switched to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33649)]
    public class TransformDataRecorder : DataRecorder<TransformDataRecorder, TransformDataRecorder.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变换数据记录器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(TransformDataRecorder))]
        [Tip("变换数据记录器组件是用于还原程序启动时转换的位置、角度和缩放信息的执行体。组件执行完毕后切换为完成态。", "The conversion datalogger component is the actuator used to restore the position, angle and zoom information converted at the start of the program. After the component is executed, it is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集合
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : TransformRecorder, IRecoverableDataRecorder<TransformDataRecorder>
        {
            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transformDataRecorder"></param>
            public void Record(TransformDataRecorder transformDataRecorder)
            {
                if (!transformDataRecorder.gameObjectSet) return;
                _records.Clear();
                Record(transformDataRecorder.gameObjectSet.objects);
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
                            Recover(i => i.transform && i.transform.name == dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameNotEquals:
                        {
                            Recover(i => i.transform && i.transform.name != dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameContains:
                        {
                            Recover(i => i.transform && i.transform.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameNotContains:
                        {
                            Recover(i => i.transform && !i.transform.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexMatch:
                        {
                            Recover(i => i.transform && Regex.IsMatch(i.transform.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexNotMatch:
                        {
                            Recover(i => i.transform && !Regex.IsMatch(i.transform.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.IsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.transform && i.transform.IsChildOf(t));
                            }
                            break;
                        }
                    case EDataRecoveryRule.NotIsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.transform && !i.transform.IsChildOf(t));
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
