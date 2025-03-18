using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Base.Recorders;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Dataflows.DataModel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 刚体数据记录器:刚体数据记录器组件是用于记录刚体的移动速度，旋转速度，重力, 运动学，线性阻力，角度阻力和刚体运动约束属性
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(RigidbodyDataRecorder))]
    [RequireComponent(typeof(GameObjectSet))]
    [Tip("刚体数据记录器组件是用于记录刚体的移动速度，旋转速度，重力, 运动学，线性阻力，角度阻力和刚体运动约束属性", "The rigid body data recorder component is used to record the moving speed, rotation speed, gravity, kinematics, linear resistance, angular resistance and rigid body motion constraint attributes of the rigid body")]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [Owner(typeof(PhysicsManager))]
    public class RigidbodyDataRecorder : DataRecorder<RigidbodyDataRecorder, RigidbodyDataRecorder.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "刚体数据记录器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(RigidbodyDataRecorder))]
        [Tip("刚体数据记录器组件是用于记录刚体的移动速度，旋转速度，重力, 运动学，线性阻力，角度阻力和刚体运动约束属性", "The rigid body data recorder component is used to record the moving speed, rotation speed, gravity, kinematics, linear resistance, angular resistance and rigid body motion constraint attributes of the rigid body")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集合
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : RigidbodyRecorder, IRecoverableDataRecorder<RigidbodyDataRecorder>
        {
            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rigidbodyDataRecorder"></param>
            public void Record(RigidbodyDataRecorder rigidbodyDataRecorder)
            {
                if (!rigidbodyDataRecorder.gameObjectSet) return;
                _records.Clear();
                Record(rigidbodyDataRecorder.gameObjectSet.objects);
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
                            Recover(i => i.rigidbody && i.rigidbody.name == dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameNotEquals:
                        {
                            Recover(i => i.rigidbody && i.rigidbody.name != dataRecoveryRuleValue);
                            break;
                        }
                    case EDataRecoveryRule.NameContains:
                        {
                            Recover(i => i.rigidbody && i.rigidbody.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameNotContains:
                        {
                            Recover(i => i.rigidbody && !i.rigidbody.name.Contains(dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexMatch:
                        {
                            Recover(i => i.rigidbody && Regex.IsMatch(i.rigidbody.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.NameRegexNotMatch:
                        {
                            Recover(i => i.rigidbody && !Regex.IsMatch(i.rigidbody.name, dataRecoveryRuleValue));
                            break;
                        }
                    case EDataRecoveryRule.IsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.rigidbody && i.rigidbody.transform.IsChildOf(t));
                            }
                            break;
                        }
                    case EDataRecoveryRule.NotIsChildOfGameObjectByNamePath:
                        {
                            var go = CommonFun.StringToGameObject(dataRecoveryRuleValue);
                            if (go)
                            {
                                var t = go.transform;
                                Recover(i => i.rigidbody && !i.rigidbody.transform.IsChildOf(t));
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
