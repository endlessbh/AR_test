using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 刚体属性设置: Rigidbody Property Set
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(RigidbodyPropertySet))]
    [Tip("Rigidbody Property Set")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(PhysicsManager))]
    public class RigidbodyPropertySet : BasePropertySet<RigidbodyPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "刚体属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(RigidbodyPropertySet))]
        [Tip("Rigidbody Property Set")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// Rigidbody:Rigidbody
        /// </summary>
        [Name("刚体")]
        [Tip("Rigidbody")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Rigidbody _Rigidbody;
        
        /// <summary>
        /// Rigidbody:Rigidbody
        /// </summary>
        public Rigidbody Rigidbody => this.XGetComponentInGlobal(ref _Rigidbody, true);
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.None;
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName 
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("无")]
            [EnumFieldName("无")]
            None,

            #region 属性

            /// <summary>
            /// 旋转阻力:旋转阻力
            /// </summary>
            [Name("旋转阻力")]
            [Tip("旋转阻力")]
            [EnumFieldName("属性/旋转阻力")]
            Property_angularDrag = 1000,

            /// <summary>
            /// 旋转速度:旋转速度
            /// </summary>
            [Name("旋转速度")]
            [Tip("旋转速度")]
            [EnumFieldName("属性/旋转速度")]
            Property_angularVelocity,
            
            /// <summary>
            /// Center Of Mass:Center Of Mass
            /// </summary>
            [Name("Center Of Mass")]
            [Tip("Center Of Mass")]
            [EnumFieldName("属性/Center Of Mass")]
            Property_centerOfMass,
            
            /// <summary>
            /// Collision Detection Mode:Collision Detection Mode
            /// </summary>
            [Name("Collision Detection Mode")]
            [Tip("Collision Detection Mode")]
            [EnumFieldName("属性/Collision Detection Mode")]
            Property_collisionDetectionMode,

            /// <summary>
            /// 约束:约束
            /// </summary>
            [Name("约束")]
            [Tip("约束")]
            [EnumFieldName("属性/约束")]
            Property_constraints,
            
            /// <summary>
            /// Detect Collisions:Detect Collisions
            /// </summary>
            [Name("Detect Collisions")]
            [Tip("Detect Collisions")]
            [EnumFieldName("属性/Detect Collisions")]
            Property_detectCollisions,

            /// <summary>
            /// 空气阻力:空气阻力
            /// </summary>
            [Name("空气阻力")]
            [Tip("空气阻力")]
            [EnumFieldName("属性/空气阻力")]
            Property_drag,
            
            /// <summary>
            /// Freeze Rotation:Freeze Rotation
            /// </summary>
            [Name("Freeze Rotation")]
            [Tip("Freeze Rotation")]
            [EnumFieldName("属性/Freeze Rotation")]
            Property_freezeRotation,
            
            /// <summary>
            /// Hide Flags:Hide Flags
            /// </summary>
            [Name("Hide Flags")]
            [Tip("Hide Flags")]
            [EnumFieldName("属性/Hide Flags")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// Inertia Tensor:Inertia Tensor
            /// </summary>
            [Name("Inertia Tensor")]
            [Tip("Inertia Tensor")]
            [EnumFieldName("属性/Inertia Tensor")]
            Property_inertiaTensor,
            
            /// <summary>
            /// Interpolation:Interpolation
            /// </summary>
            [Name("Interpolation")]
            [Tip("Interpolation")]
            [EnumFieldName("属性/Interpolation")]
            Property_interpolation,

            /// <summary>
            /// 运动学:运动学
            /// </summary>
            [Name("运动学")]
            [Tip("运动学")]
            [EnumFieldName("属性/运动学")]
            Property_isKinematic,
            
            /// <summary>
            /// Mass:Mass
            /// </summary>
            [Name("Mass")]
            [Tip("Mass")]
            [EnumFieldName("属性/Mass")]
            Property_mass,
            
            /// <summary>
            /// Max Angular Velocity:Max Angular Velocity
            /// </summary>
            [Name("Max Angular Velocity")]
            [Tip("Max Angular Velocity")]
            [EnumFieldName("属性/Max Angular Velocity")]
            Property_maxAngularVelocity,
            
            /// <summary>
            /// Max Depenetration Velocity:Max Depenetration Velocity
            /// </summary>
            [Name("Max Depenetration Velocity")]
            [Tip("Max Depenetration Velocity")]
            [EnumFieldName("属性/Max Depenetration Velocity")]
            Property_maxDepenetrationVelocity,
            
            /// <summary>
            /// Name:Name
            /// </summary>
            [Name("Name")]
            [Tip("Name")]
            [EnumFieldName("属性/Name")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// Position:Position
            /// </summary>
            [Name("Position")]
            [Tip("Position")]
            [EnumFieldName("属性/Position")]
            Property_position,
            
            /// <summary>
            /// Sleep Threshold:Sleep Threshold
            /// </summary>
            [Name("Sleep Threshold")]
            [Tip("Sleep Threshold")]
            [EnumFieldName("属性/Sleep Threshold")]
            Property_sleepThreshold,
            
            /// <summary>
            /// Solver Iterations:Solver Iterations
            /// </summary>
            [Name("Solver Iterations")]
            [Tip("Solver Iterations")]
            [EnumFieldName("属性/Solver Iterations")]
            Property_solverIterations,
            
            /// <summary>
            /// Solver Velocity Iterations:Solver Velocity Iterations
            /// </summary>
            [Name("Solver Velocity Iterations")]
            [Tip("Solver Velocity Iterations")]
            [EnumFieldName("属性/Solver Velocity Iterations")]
            Property_solverVelocityIterations,
            
            /// <summary>
            /// Tag:Tag
            /// </summary>
            [Name("Tag")]
            [Tip("Tag")]
            [EnumFieldName("属性/Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,

            /// <summary>
            /// 应用重力:应用重力
            /// </summary>
            [Name("应用重力")]
            [Tip("应用重力")]
            [EnumFieldName("属性/应用重力")]
            Property_useGravity,

            /// <summary>
            /// 移动速度:移动速度
            /// </summary>
            [Name("移动速度")]
            [Tip("移动速度")]
            [EnumFieldName("属性/移动速度")]
            Property_velocity,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// Add Explosion Force:Add Explosion Force
            /// </summary>
            [Name("Add Explosion Force")]
            [Tip("Add Explosion Force")]
            [EnumFieldName("方法/Add Explosion Force")]
            Method_AddExplosionForce_Single_Vector3_Single = 10000,
            
            /// <summary>
            /// Add Explosion Force:Add Explosion Force
            /// </summary>
            [Name("Add Explosion Force")]
            [Tip("Add Explosion Force")]
            [EnumFieldName("方法/Add Explosion Force")]
            Method_AddExplosionForce_Single_Vector3_Single_Single,
            
            /// <summary>
            /// Add Explosion Force:Add Explosion Force
            /// </summary>
            [Name("Add Explosion Force")]
            [Tip("Add Explosion Force")]
            [EnumFieldName("方法/Add Explosion Force")]
            Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode,
            
            /// <summary>
            /// Add Force:Add Force
            /// </summary>
            [Name("Add Force")]
            [Tip("Add Force")]
            [EnumFieldName("方法/Add Force")]
            Method_AddForce_Single_Single_Single,
            
            /// <summary>
            /// Add Force:Add Force
            /// </summary>
            [Name("Add Force")]
            [Tip("Add Force")]
            [EnumFieldName("方法/Add Force")]
            Method_AddForce_Single_Single_Single_ForceMode,
            
            /// <summary>
            /// Add Force:Add Force
            /// </summary>
            [Name("Add Force")]
            [Tip("Add Force")]
            [EnumFieldName("方法/Add Force")]
            Method_AddForce_Vector3,
            
            /// <summary>
            /// Add Force:Add Force
            /// </summary>
            [Name("Add Force")]
            [Tip("Add Force")]
            [EnumFieldName("方法/Add Force")]
            Method_AddForce_Vector3_ForceMode,
            
            /// <summary>
            /// Add Force At Position:Add Force At Position
            /// </summary>
            [Name("Add Force At Position")]
            [Tip("Add Force At Position")]
            [EnumFieldName("方法/Add Force At Position")]
            Method_AddForceAtPosition_Vector3_Vector3,
            
            /// <summary>
            /// Add Force At Position:Add Force At Position
            /// </summary>
            [Name("Add Force At Position")]
            [Tip("Add Force At Position")]
            [EnumFieldName("方法/Add Force At Position")]
            Method_AddForceAtPosition_Vector3_Vector3_ForceMode,
            
            /// <summary>
            /// Add Relative Force:Add Relative Force
            /// </summary>
            [Name("Add Relative Force")]
            [Tip("Add Relative Force")]
            [EnumFieldName("方法/Add Relative Force")]
            Method_AddRelativeForce_Single_Single_Single,
            
            /// <summary>
            /// Add Relative Force:Add Relative Force
            /// </summary>
            [Name("Add Relative Force")]
            [Tip("Add Relative Force")]
            [EnumFieldName("方法/Add Relative Force")]
            Method_AddRelativeForce_Single_Single_Single_ForceMode,
            
            /// <summary>
            /// Add Relative Force:Add Relative Force
            /// </summary>
            [Name("Add Relative Force")]
            [Tip("Add Relative Force")]
            [EnumFieldName("方法/Add Relative Force")]
            Method_AddRelativeForce_Vector3,
            
            /// <summary>
            /// Add Relative Force:Add Relative Force
            /// </summary>
            [Name("Add Relative Force")]
            [Tip("Add Relative Force")]
            [EnumFieldName("方法/Add Relative Force")]
            Method_AddRelativeForce_Vector3_ForceMode,
            
            /// <summary>
            /// Add Relative Torque:Add Relative Torque
            /// </summary>
            [Name("Add Relative Torque")]
            [Tip("Add Relative Torque")]
            [EnumFieldName("方法/Add Relative Torque")]
            Method_AddRelativeTorque_Single_Single_Single,
            
            /// <summary>
            /// Add Relative Torque:Add Relative Torque
            /// </summary>
            [Name("Add Relative Torque")]
            [Tip("Add Relative Torque")]
            [EnumFieldName("方法/Add Relative Torque")]
            Method_AddRelativeTorque_Single_Single_Single_ForceMode,
            
            /// <summary>
            /// Add Relative Torque:Add Relative Torque
            /// </summary>
            [Name("Add Relative Torque")]
            [Tip("Add Relative Torque")]
            [EnumFieldName("方法/Add Relative Torque")]
            Method_AddRelativeTorque_Vector3,
            
            /// <summary>
            /// Add Relative Torque:Add Relative Torque
            /// </summary>
            [Name("Add Relative Torque")]
            [Tip("Add Relative Torque")]
            [EnumFieldName("方法/Add Relative Torque")]
            Method_AddRelativeTorque_Vector3_ForceMode,
            
            /// <summary>
            /// Add Torque:Add Torque
            /// </summary>
            [Name("Add Torque")]
            [Tip("Add Torque")]
            [EnumFieldName("方法/Add Torque")]
            Method_AddTorque_Single_Single_Single,
            
            /// <summary>
            /// Add Torque:Add Torque
            /// </summary>
            [Name("Add Torque")]
            [Tip("Add Torque")]
            [EnumFieldName("方法/Add Torque")]
            Method_AddTorque_Single_Single_Single_ForceMode,
            
            /// <summary>
            /// Add Torque:Add Torque
            /// </summary>
            [Name("Add Torque")]
            [Tip("Add Torque")]
            [EnumFieldName("方法/Add Torque")]
            Method_AddTorque_Vector3,
            
            /// <summary>
            /// Add Torque:Add Torque
            /// </summary>
            [Name("Add Torque")]
            [Tip("Add Torque")]
            [EnumFieldName("方法/Add Torque")]
            Method_AddTorque_Vector3_ForceMode,
            
            /// <summary>
            /// Broadcast Message:Broadcast Message
            /// </summary>
            [Name("Broadcast Message")]
            [Tip("Broadcast Message")]
            [EnumFieldName("方法/Broadcast Message")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String,
            
            /// <summary>
            /// Broadcast Message:Broadcast Message
            /// </summary>
            [Name("Broadcast Message")]
            [Tip("Broadcast Message")]
            [EnumFieldName("方法/Broadcast Message")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,
            
            /// <summary>
            /// Closest Point On Bounds:Closest Point On Bounds
            /// </summary>
            [Name("Closest Point On Bounds")]
            [Tip("Closest Point On Bounds")]
            [EnumFieldName("方法/Closest Point On Bounds")]
            Method_ClosestPointOnBounds_Vector3,
            
            /// <summary>
            /// Compare Tag:Compare Tag
            /// </summary>
            [Name("Compare Tag")]
            [Tip("Compare Tag")]
            [EnumFieldName("方法/Compare Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,
            
            /// <summary>
            /// Get Component:Get Component
            /// </summary>
            [Name("Get Component")]
            [Tip("Get Component")]
            [EnumFieldName("方法/Get Component")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// Get Hash Code:Get Hash Code
            /// </summary>
            [Name("Get Hash Code")]
            [Tip("Get Hash Code")]
            [EnumFieldName("方法/Get Hash Code")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// Get Instance ID:Get Instance ID
            /// </summary>
            [Name("Get Instance ID")]
            [Tip("Get Instance ID")]
            [EnumFieldName("方法/Get Instance ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// Get Point Velocity:Get Point Velocity
            /// </summary>
            [Name("Get Point Velocity")]
            [Tip("Get Point Velocity")]
            [EnumFieldName("方法/Get Point Velocity")]
            Method_GetPointVelocity_Vector3,
            
            /// <summary>
            /// Get Relative Point Velocity:Get Relative Point Velocity
            /// </summary>
            [Name("Get Relative Point Velocity")]
            [Tip("Get Relative Point Velocity")]
            [EnumFieldName("方法/Get Relative Point Velocity")]
            Method_GetRelativePointVelocity_Vector3,
            
            /// <summary>
            /// Get Type:Get Type
            /// </summary>
            [Name("Get Type")]
            [Tip("Get Type")]
            [EnumFieldName("方法/Get Type")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// Is Sleeping:Is Sleeping
            /// </summary>
            [Name("Is Sleeping")]
            [Tip("Is Sleeping")]
            [EnumFieldName("方法/Is Sleeping")]
            Method_IsSleeping,
            
            /// <summary>
            /// Move Position:Move Position
            /// </summary>
            [Name("Move Position")]
            [Tip("Move Position")]
            [EnumFieldName("方法/Move Position")]
            Method_MovePosition_Vector3,
            
            /// <summary>
            /// Reset Center Of Mass:Reset Center Of Mass
            /// </summary>
            [Name("Reset Center Of Mass")]
            [Tip("Reset Center Of Mass")]
            [EnumFieldName("方法/Reset Center Of Mass")]
            Method_ResetCenterOfMass,
            
            /// <summary>
            /// Reset Inertia Tensor:Reset Inertia Tensor
            /// </summary>
            [Name("Reset Inertia Tensor")]
            [Tip("Reset Inertia Tensor")]
            [EnumFieldName("方法/Reset Inertia Tensor")]
            Method_ResetInertiaTensor,

            /// <summary>
            /// 发送消息(方法名)(方法):发送消息(方法名)
            /// </summary>
            [Name("发送消息(方法名)(方法)")]
            [Tip("发送消息(方法名)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,

            /// <summary>
            /// 发送消息(方法名+发送消息选项)(方法):发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)(方法)")]
            [Tip("发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,

            /// <summary>
            /// 向上发送消息(方法名)(方法):向上发送消息(方法名)
            /// </summary>
            [Name("向上发送消息(方法名)(方法)")]
            [Tip("向上发送消息(方法名)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,

            /// <summary>
            /// 向上发送消息(方法名+发送消息选项)(方法):向上发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)(方法)")]
            [Tip("向上发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,

            /// <summary>
            /// Set Density:Set Density
            /// </summary>
            [Name("Set Density")]
            [Tip("Set Density")]
            [EnumFieldName("方法/Set Density")]
            Method_SetDensity_Single,
            
            /// <summary>
            /// Sleep:Sleep
            /// </summary>
            [Name("Sleep")]
            [Tip("Sleep")]
            [EnumFieldName("方法/Sleep")]
            Method_Sleep,
            
            /// <summary>
            /// Sweep Test All:Sweep Test All
            /// </summary>
            [Name("Sweep Test All")]
            [Tip("Sweep Test All")]
            [EnumFieldName("方法/Sweep Test All")]
            Method_SweepTestAll_Vector3,
            
            /// <summary>
            /// Sweep Test All:Sweep Test All
            /// </summary>
            [Name("Sweep Test All")]
            [Tip("Sweep Test All")]
            [EnumFieldName("方法/Sweep Test All")]
            Method_SweepTestAll_Vector3_Single,
            
            /// <summary>
            /// Sweep Test All:Sweep Test All
            /// </summary>
            [Name("Sweep Test All")]
            [Tip("Sweep Test All")]
            [EnumFieldName("方法/Sweep Test All")]
            Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction,
            
            /// <summary>
            /// To String:To String
            /// </summary>
            [Name("To String")]
            [Tip("To String")]
            [EnumFieldName("方法/To String")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            /// <summary>
            /// Wake Up:Wake Up
            /// </summary>
            [Name("Wake Up")]
            [Tip("Wake Up")]
            [EnumFieldName("方法/Wake Up")]
            Method_WakeUp,
            
            #endregion
            
        }

        #region 属性

        /// <summary>
        /// 角阻力:Angular Drag
        /// </summary>
        [Name("角阻力")]
        [Tip("Angular Drag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_angularDrag)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_angularDrag = new FloatPropertyValue();

        /// <summary>
        /// 角速度:Angular Velocity
        /// </summary>
        [Name("角速度")]
        [Tip("Angular Velocity")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_angularVelocity)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_angularVelocity = new Vector3PropertyValue();

        /// <summary>
        /// 质量中心:Center Of Mass
        /// </summary>
        [Name("质量中心")]
        [Tip("Center Of Mass")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_centerOfMass)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_centerOfMass = new Vector3PropertyValue();

        /// <summary>
        /// 碰撞监测模式:Collision Detection Mode
        /// </summary>
        [Name("碰撞监测模式")]
        [Tip("Collision Detection Mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_collisionDetectionMode)]
        [OnlyMemberElements]
        public CollisionDetectionModePropertyValue _Property_collisionDetectionMode = new CollisionDetectionModePropertyValue();

        /// <summary>
        /// 运动约束:Constraints
        /// </summary>
        [Name("运动约束")]
        [Tip("Constraints")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_constraints)]
        [OnlyMemberElements]
        public RigidbodyConstraintsPropertyValue _Property_constraints = new RigidbodyConstraintsPropertyValue();

        /// <summary>
        /// 碰撞检测:Detect Collisions
        /// </summary>
        [Name("碰撞检测")]
        [Tip("Detect Collisions")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_detectCollisions)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_detectCollisions = new BoolPropertyValue();

        /// <summary>
        /// 线阻力:Drag
        /// </summary>
        [Name("线阻力")]
        [Tip("Drag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_drag)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_drag = new FloatPropertyValue();

        /// <summary>
        /// 冻结旋转:Freeze Rotation
        /// </summary>
        [Name("冻结旋转")]
        [Tip("Freeze Rotation")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_freezeRotation)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_freezeRotation = new BoolPropertyValue();
        
        /// <summary>
        /// Hide Flags:Hide Flags
        /// </summary>
        [Name("Hide Flags")]
        [Tip("Hide Flags")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_hideFlags)]
        [OnlyMemberElements]
        public HideFlagsPropertyValue _Property_hideFlags = new HideFlagsPropertyValue();
        
        /// <summary>
        /// Inertia Tensor:Inertia Tensor
        /// </summary>
        [Name("惯性张量")]
        [Tip("Inertia Tensor")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_inertiaTensor)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_inertiaTensor = new Vector3PropertyValue();
        
        /// <summary>
        /// Interpolation:Interpolation
        /// </summary>
        [Name("Interpolation")]
        [Tip("Interpolation")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_interpolation)]
        [OnlyMemberElements]
        public RigidbodyInterpolationPropertyValue _Property_interpolation = new RigidbodyInterpolationPropertyValue();

        /// <summary>
        /// 是运动学:Is Kinematic
        /// </summary>
        [Name("是运动学")]
        [Tip("Is Kinematic")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_isKinematic)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_isKinematic = new BoolPropertyValue();

        /// <summary>
        /// 质量:Mass
        /// </summary>
        [Name("质量")]
        [Tip("Mass")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_mass)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_mass = new FloatPropertyValue();
        
        /// <summary>
        /// Max Angular Velocity:Max Angular Velocity
        /// </summary>
        [Name("Max Angular Velocity")]
        [Tip("Max Angular Velocity")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_maxAngularVelocity)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_maxAngularVelocity = new FloatPropertyValue();
        
        /// <summary>
        /// Max Depenetration Velocity:Max Depenetration Velocity
        /// </summary>
        [Name("Max Depenetration Velocity")]
        [Tip("Max Depenetration Velocity")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_maxDepenetrationVelocity)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_maxDepenetrationVelocity = new FloatPropertyValue();
        
        /// <summary>
        /// Name:Name
        /// </summary>
        [Name("Name")]
        [Tip("Name")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_name)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_name = new StringPropertyValue();
        
        /// <summary>
        /// Position:Position
        /// </summary>
        [Name("Position")]
        [Tip("Position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_position)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_position = new Vector3PropertyValue();

        /// <summary>
        /// 睡眠阈值:Sleep Threshold
        /// </summary>
        [Name("睡眠阈值")]
        [Tip("Sleep Threshold")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_sleepThreshold)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_sleepThreshold = new FloatPropertyValue();

        /// <summary>
        /// 求解器迭代:Solver Iterations
        /// </summary>
        [Name("求解器迭代")]
        [Tip("Solver Iterations")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_solverIterations)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_solverIterations = new IntPropertyValue();

        /// <summary>
        /// 求解器速度迭代:Solver Velocity Iterations
        /// </summary>
        [Name("求解器速度迭代")]
        [Tip("Solver Velocity Iterations")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_solverVelocityIterations)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_solverVelocityIterations = new IntPropertyValue();
        
        /// <summary>
        /// Tag:Tag
        /// </summary>
        [Name("Tag")]
        [Tip("Tag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_tag)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_tag = new StringPropertyValue();

        /// <summary>
        /// 应用重力:Use Gravity
        /// </summary>
        [Name("应用重力")]
        [Tip("Use Gravity")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_useGravity)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_useGravity = new BoolPropertyValue();

        /// <summary>
        /// 速度:Velocity
        /// </summary>
        [Name("速度")]
        [Tip("Velocity")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_velocity)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_velocity = new Vector3PropertyValue();
        
        #endregion
        
        #region 方法
        
        /// <summary>
        /// explosionForce:explosionForce
        /// </summary>
        [Name("explosionForce")]
        [Tip("explosionForce")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single__explosionForce = new FloatPropertyValue();
        
        /// <summary>
        /// explosionPosition:explosionPosition
        /// </summary>
        [Name("explosionPosition")]
        [Tip("explosionPosition")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddExplosionForce_Single_Vector3_Single__explosionPosition = new Vector3PropertyValue();
        
        /// <summary>
        /// explosionRadius:explosionRadius
        /// </summary>
        [Name("explosionRadius")]
        [Tip("explosionRadius")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single__explosionRadius = new FloatPropertyValue();
        
        /// <summary>
        /// explosionForce:explosionForce
        /// </summary>
        [Name("explosionForce")]
        [Tip("explosionForce")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single__explosionForce = new FloatPropertyValue();
        
        /// <summary>
        /// explosionPosition:explosionPosition
        /// </summary>
        [Name("explosionPosition")]
        [Tip("explosionPosition")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single__explosionPosition = new Vector3PropertyValue();
        
        /// <summary>
        /// explosionRadius:explosionRadius
        /// </summary>
        [Name("explosionRadius")]
        [Tip("explosionRadius")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single__explosionRadius = new FloatPropertyValue();
        
        /// <summary>
        /// upwardsModifier:upwardsModifier
        /// </summary>
        [Name("upwardsModifier")]
        [Tip("upwardsModifier")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single__upwardsModifier = new FloatPropertyValue();
        
        /// <summary>
        /// explosionForce:explosionForce
        /// </summary>
        [Name("explosionForce")]
        [Tip("explosionForce")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionForce = new FloatPropertyValue();
        
        /// <summary>
        /// explosionPosition:explosionPosition
        /// </summary>
        [Name("explosionPosition")]
        [Tip("explosionPosition")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionPosition = new Vector3PropertyValue();
        
        /// <summary>
        /// explosionRadius:explosionRadius
        /// </summary>
        [Name("explosionRadius")]
        [Tip("explosionRadius")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionRadius = new FloatPropertyValue();
        
        /// <summary>
        /// upwardsModifier:upwardsModifier
        /// </summary>
        [Name("upwardsModifier")]
        [Tip("upwardsModifier")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__upwardsModifier = new FloatPropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single__z = new FloatPropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single_ForceMode__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single_ForceMode__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddForce_Single_Single_Single_ForceMode__z = new FloatPropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddForce_Single_Single_Single_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForce_Vector3__force = new Vector3PropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForce_Vector3_ForceMode__force = new Vector3PropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForce_Vector3_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddForce_Vector3_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForceAtPosition_Vector3_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForceAtPosition_Vector3_Vector3__force = new Vector3PropertyValue();
        
        /// <summary>
        /// position:position
        /// </summary>
        [Name("position")]
        [Tip("position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForceAtPosition_Vector3_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForceAtPosition_Vector3_Vector3__position = new Vector3PropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForceAtPosition_Vector3_Vector3_ForceMode__force = new Vector3PropertyValue();
        
        /// <summary>
        /// position:position
        /// </summary>
        [Name("position")]
        [Tip("position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddForceAtPosition_Vector3_Vector3_ForceMode__position = new Vector3PropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddForceAtPosition_Vector3_Vector3_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single__z = new FloatPropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single_ForceMode__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single_ForceMode__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeForce_Single_Single_Single_ForceMode__z = new FloatPropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddRelativeForce_Single_Single_Single_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddRelativeForce_Vector3__force = new Vector3PropertyValue();
        
        /// <summary>
        /// force:force
        /// </summary>
        [Name("force")]
        [Tip("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddRelativeForce_Vector3_ForceMode__force = new Vector3PropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeForce_Vector3_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddRelativeForce_Vector3_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single__z = new FloatPropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single_ForceMode__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single_ForceMode__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddRelativeTorque_Single_Single_Single_ForceMode__z = new FloatPropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddRelativeTorque_Single_Single_Single_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// torque:torque
        /// </summary>
        [Name("torque")]
        [Tip("torque")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddRelativeTorque_Vector3__torque = new Vector3PropertyValue();
        
        /// <summary>
        /// torque:torque
        /// </summary>
        [Name("torque")]
        [Tip("torque")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddRelativeTorque_Vector3_ForceMode__torque = new Vector3PropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddRelativeTorque_Vector3_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddRelativeTorque_Vector3_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single__z = new FloatPropertyValue();
        
        /// <summary>
        /// x:x
        /// </summary>
        [Name("x")]
        [Tip("x")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single_ForceMode__x = new FloatPropertyValue();
        
        /// <summary>
        /// y:y
        /// </summary>
        [Name("y")]
        [Tip("y")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single_ForceMode__y = new FloatPropertyValue();
        
        /// <summary>
        /// z:z
        /// </summary>
        [Name("z")]
        [Tip("z")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_AddTorque_Single_Single_Single_ForceMode__z = new FloatPropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddTorque_Single_Single_Single_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// torque:torque
        /// </summary>
        [Name("torque")]
        [Tip("torque")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddTorque_Vector3__torque = new Vector3PropertyValue();
        
        /// <summary>
        /// torque:torque
        /// </summary>
        [Name("torque")]
        [Tip("torque")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Vector3_ForceMode)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_AddTorque_Vector3_ForceMode__torque = new Vector3PropertyValue();
        
        /// <summary>
        /// mode:mode
        /// </summary>
        [Name("mode")]
        [Tip("mode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_AddTorque_Vector3_ForceMode)]
        [OnlyMemberElements]
        public ForceModePropertyValue _Method_AddTorque_Vector3_ForceMode__mode = new ForceModePropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// position:position
        /// </summary>
        [Name("position")]
        [Tip("position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_ClosestPointOnBounds_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_ClosestPointOnBounds_Vector3__position = new Vector3PropertyValue();
        
        /// <summary>
        /// tag:tag
        /// </summary>
        [Name("tag")]
        [Tip("tag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// type:type
        /// </summary>
        [Name("type")]
        [Tip("type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// worldPoint:worldPoint
        /// </summary>
        [Name("worldPoint")]
        [Tip("worldPoint")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetPointVelocity_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_GetPointVelocity_Vector3__worldPoint = new Vector3PropertyValue();
        
        /// <summary>
        /// relativePoint:relativePoint
        /// </summary>
        [Name("relativePoint")]
        [Tip("relativePoint")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetRelativePointVelocity_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_GetRelativePointVelocity_Vector3__relativePoint = new Vector3PropertyValue();
        
        /// <summary>
        /// position:position
        /// </summary>
        [Name("position")]
        [Tip("position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_MovePosition_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_MovePosition_Vector3__position = new Vector3PropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// density:density
        /// </summary>
        [Name("density")]
        [Tip("density")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetDensity_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_SetDensity_Single__density = new FloatPropertyValue();
        
        /// <summary>
        /// direction:direction
        /// </summary>
        [Name("direction")]
        [Tip("direction")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_SweepTestAll_Vector3__direction = new Vector3PropertyValue();
        
        /// <summary>
        /// direction:direction
        /// </summary>
        [Name("direction")]
        [Tip("direction")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3_Single)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_SweepTestAll_Vector3_Single__direction = new Vector3PropertyValue();
        
        /// <summary>
        /// maxDistance:maxDistance
        /// </summary>
        [Name("maxDistance")]
        [Tip("maxDistance")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_SweepTestAll_Vector3_Single__maxDistance = new FloatPropertyValue();
        
        /// <summary>
        /// direction:direction
        /// </summary>
        [Name("direction")]
        [Tip("direction")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__direction = new Vector3PropertyValue();
        
        /// <summary>
        /// maxDistance:maxDistance
        /// </summary>
        [Name("maxDistance")]
        [Tip("maxDistance")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__maxDistance = new FloatPropertyValue();
        
        /// <summary>
        /// queryTriggerInteraction:queryTriggerInteraction
        /// </summary>
        [Name("queryTriggerInteraction")]
        [Tip("queryTriggerInteraction")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction)]
        [OnlyMemberElements]
        public QueryTriggerInteractionPropertyValue _Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__queryTriggerInteraction = new QueryTriggerInteractionPropertyValue();
        
        #endregion
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_angularDrag:
                {
                    _Rigidbody.angularDrag = _Property_angularDrag.GetValue();
                    break;
                }
                case EPropertyName.Property_angularVelocity:
                {
                    _Rigidbody.angularVelocity = _Property_angularVelocity.GetValue();
                    break;
                }
                case EPropertyName.Property_centerOfMass:
                {
                    _Rigidbody.centerOfMass = _Property_centerOfMass.GetValue();
                    break;
                }
                case EPropertyName.Property_collisionDetectionMode:
                {
                    _Rigidbody.collisionDetectionMode = _Property_collisionDetectionMode.GetValue();
                    break;
                }
                case EPropertyName.Property_constraints:
                {
                    _Rigidbody.constraints = _Property_constraints.GetValue();
                    break;
                }
                case EPropertyName.Property_detectCollisions:
                {
                    _Rigidbody.detectCollisions = _Property_detectCollisions.GetValue();
                    break;
                }
                case EPropertyName.Property_drag:
                {
                    _Rigidbody.drag = _Property_drag.GetValue();
                    break;
                }
                case EPropertyName.Property_freezeRotation:
                {
                    _Rigidbody.freezeRotation = _Property_freezeRotation.GetValue();
                    break;
                }
                case EPropertyName.Property_hideFlags:
                {
                    _Rigidbody.hideFlags = _Property_hideFlags.GetValue();
                    break;
                }
                case EPropertyName.Property_inertiaTensor:
                {
                    _Rigidbody.inertiaTensor = _Property_inertiaTensor.GetValue();
                    break;
                }
                case EPropertyName.Property_interpolation:
                {
                    _Rigidbody.interpolation = _Property_interpolation.GetValue();
                    break;
                }
                case EPropertyName.Property_isKinematic:
                {
                    _Rigidbody.isKinematic = _Property_isKinematic.GetValue();
                    break;
                }
                case EPropertyName.Property_mass:
                {
                    _Rigidbody.mass = _Property_mass.GetValue();
                    break;
                }
                case EPropertyName.Property_maxAngularVelocity:
                {
                    _Rigidbody.maxAngularVelocity = _Property_maxAngularVelocity.GetValue();
                    break;
                }
                case EPropertyName.Property_maxDepenetrationVelocity:
                {
                    _Rigidbody.maxDepenetrationVelocity = _Property_maxDepenetrationVelocity.GetValue();
                    break;
                }
                case EPropertyName.Property_name:
                {
                    _Rigidbody.name = _Property_name.GetValue();
                    break;
                }
                case EPropertyName.Property_position:
                {
                    _Rigidbody.position = _Property_position.GetValue();
                    break;
                }
                case EPropertyName.Property_sleepThreshold:
                {
                    _Rigidbody.sleepThreshold = _Property_sleepThreshold.GetValue();
                    break;
                }
                case EPropertyName.Property_solverIterations:
                {
                    _Rigidbody.solverIterations = _Property_solverIterations.GetValue();
                    break;
                }
                case EPropertyName.Property_solverVelocityIterations:
                {
                    _Rigidbody.solverVelocityIterations = _Property_solverVelocityIterations.GetValue();
                    break;
                }
                case EPropertyName.Property_tag:
                {
                    _Rigidbody.tag = _Property_tag.GetValue();
                    break;
                }
                case EPropertyName.Property_useGravity:
                {
                    _Rigidbody.useGravity = _Property_useGravity.GetValue();
                    break;
                }
                case EPropertyName.Property_velocity:
                {
                    _Rigidbody.velocity = _Property_velocity.GetValue();
                    break;
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single:
                {
                    _Rigidbody.AddExplosionForce(_Method_AddExplosionForce_Single_Vector3_Single__explosionForce.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single__explosionPosition.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single__explosionRadius.GetValue());
                    break;
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single:
                {
                    _Rigidbody.AddExplosionForce(_Method_AddExplosionForce_Single_Vector3_Single_Single__explosionForce.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single__explosionPosition.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single__explosionRadius.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single__upwardsModifier.GetValue());
                    break;
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode:
                {
                    _Rigidbody.AddExplosionForce(_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionForce.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionPosition.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionRadius.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__upwardsModifier.GetValue(), _Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForce_Single_Single_Single:
                {
                    _Rigidbody.AddForce(_Method_AddForce_Single_Single_Single__x.GetValue(), _Method_AddForce_Single_Single_Single__y.GetValue(), _Method_AddForce_Single_Single_Single__z.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForce_Single_Single_Single_ForceMode:
                {
                    _Rigidbody.AddForce(_Method_AddForce_Single_Single_Single_ForceMode__x.GetValue(), _Method_AddForce_Single_Single_Single_ForceMode__y.GetValue(), _Method_AddForce_Single_Single_Single_ForceMode__z.GetValue(), _Method_AddForce_Single_Single_Single_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForce_Vector3:
                {
                    _Rigidbody.AddForce(_Method_AddForce_Vector3__force.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForce_Vector3_ForceMode:
                {
                    _Rigidbody.AddForce(_Method_AddForce_Vector3_ForceMode__force.GetValue(), _Method_AddForce_Vector3_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3:
                {
                    _Rigidbody.AddForceAtPosition(_Method_AddForceAtPosition_Vector3_Vector3__force.GetValue(), _Method_AddForceAtPosition_Vector3_Vector3__position.GetValue());
                    break;
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode:
                {
                    _Rigidbody.AddForceAtPosition(_Method_AddForceAtPosition_Vector3_Vector3_ForceMode__force.GetValue(), _Method_AddForceAtPosition_Vector3_Vector3_ForceMode__position.GetValue(), _Method_AddForceAtPosition_Vector3_Vector3_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single:
                {
                    _Rigidbody.AddRelativeForce(_Method_AddRelativeForce_Single_Single_Single__x.GetValue(), _Method_AddRelativeForce_Single_Single_Single__y.GetValue(), _Method_AddRelativeForce_Single_Single_Single__z.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode:
                {
                    _Rigidbody.AddRelativeForce(_Method_AddRelativeForce_Single_Single_Single_ForceMode__x.GetValue(), _Method_AddRelativeForce_Single_Single_Single_ForceMode__y.GetValue(), _Method_AddRelativeForce_Single_Single_Single_ForceMode__z.GetValue(), _Method_AddRelativeForce_Single_Single_Single_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Vector3:
                {
                    _Rigidbody.AddRelativeForce(_Method_AddRelativeForce_Vector3__force.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Vector3_ForceMode:
                {
                    _Rigidbody.AddRelativeForce(_Method_AddRelativeForce_Vector3_ForceMode__force.GetValue(), _Method_AddRelativeForce_Vector3_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single:
                {
                    _Rigidbody.AddRelativeTorque(_Method_AddRelativeTorque_Single_Single_Single__x.GetValue(), _Method_AddRelativeTorque_Single_Single_Single__y.GetValue(), _Method_AddRelativeTorque_Single_Single_Single__z.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode:
                {
                    _Rigidbody.AddRelativeTorque(_Method_AddRelativeTorque_Single_Single_Single_ForceMode__x.GetValue(), _Method_AddRelativeTorque_Single_Single_Single_ForceMode__y.GetValue(), _Method_AddRelativeTorque_Single_Single_Single_ForceMode__z.GetValue(), _Method_AddRelativeTorque_Single_Single_Single_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3:
                {
                    _Rigidbody.AddRelativeTorque(_Method_AddRelativeTorque_Vector3__torque.GetValue());
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3_ForceMode:
                {
                    _Rigidbody.AddRelativeTorque(_Method_AddRelativeTorque_Vector3_ForceMode__torque.GetValue(), _Method_AddRelativeTorque_Vector3_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single:
                {
                    _Rigidbody.AddTorque(_Method_AddTorque_Single_Single_Single__x.GetValue(), _Method_AddTorque_Single_Single_Single__y.GetValue(), _Method_AddTorque_Single_Single_Single__z.GetValue());
                    break;
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode:
                {
                    _Rigidbody.AddTorque(_Method_AddTorque_Single_Single_Single_ForceMode__x.GetValue(), _Method_AddTorque_Single_Single_Single_ForceMode__y.GetValue(), _Method_AddTorque_Single_Single_Single_ForceMode__z.GetValue(), _Method_AddTorque_Single_Single_Single_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_AddTorque_Vector3:
                {
                    _Rigidbody.AddTorque(_Method_AddTorque_Vector3__torque.GetValue());
                    break;
                }
                case EPropertyName.Method_AddTorque_Vector3_ForceMode:
                {
                    _Rigidbody.AddTorque(_Method_AddTorque_Vector3_ForceMode__torque.GetValue(), _Method_AddTorque_Vector3_ForceMode__mode.GetValue());
                    break;
                }
                case EPropertyName.Method_BroadcastMessage_String:
                {
                    _Rigidbody.BroadcastMessage(_Method_BroadcastMessage_String__methodName.GetValue());
                    break;
                }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                {
                    _Rigidbody.BroadcastMessage(_Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue(), _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue());
                    break;
                }
                case EPropertyName.Method_ClosestPointOnBounds_Vector3:
                {
                    _Rigidbody.ClosestPointOnBounds(_Method_ClosestPointOnBounds_Vector3__position.GetValue());
                    break;
                }
                case EPropertyName.Method_CompareTag_String:
                {
                    _Rigidbody.CompareTag(_Method_CompareTag_String__tag.GetValue());
                    break;
                }
                case EPropertyName.Method_GetComponent_String:
                {
                    _Rigidbody.GetComponent(_Method_GetComponent_String__type.GetValue());
                    break;
                }
                case EPropertyName.Method_GetHashCode:
                {
                    _Rigidbody.GetHashCode();
                    break;
                }
                case EPropertyName.Method_GetInstanceID:
                {
                    _Rigidbody.GetInstanceID();
                    break;
                }
                case EPropertyName.Method_GetPointVelocity_Vector3:
                {
                    _Rigidbody.GetPointVelocity(_Method_GetPointVelocity_Vector3__worldPoint.GetValue());
                    break;
                }
                case EPropertyName.Method_GetRelativePointVelocity_Vector3:
                {
                    _Rigidbody.GetRelativePointVelocity(_Method_GetRelativePointVelocity_Vector3__relativePoint.GetValue());
                    break;
                }
                case EPropertyName.Method_GetType:
                {
                    _Rigidbody.GetType();
                    break;
                }
                case EPropertyName.Method_IsSleeping:
                {
                    _Rigidbody.IsSleeping();
                    break;
                }
                case EPropertyName.Method_MovePosition_Vector3:
                {
                    _Rigidbody.MovePosition(_Method_MovePosition_Vector3__position.GetValue());
                    break;
                }
                case EPropertyName.Method_ResetCenterOfMass:
                {
                    _Rigidbody.ResetCenterOfMass();
                    break;
                }
                case EPropertyName.Method_ResetInertiaTensor:
                {
                    _Rigidbody.ResetInertiaTensor();
                    break;
                }
                case EPropertyName.Method_SendMessage_String:
                {
                    _Rigidbody.SendMessage(_Method_SendMessage_String__methodName.GetValue());
                    break;
                }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                {
                    _Rigidbody.SendMessage(_Method_SendMessage_String_SendMessageOptions__methodName.GetValue(), _Method_SendMessage_String_SendMessageOptions__options.GetValue());
                    break;
                }
                case EPropertyName.Method_SendMessageUpwards_String:
                {
                    _Rigidbody.SendMessageUpwards(_Method_SendMessageUpwards_String__methodName.GetValue());
                    break;
                }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                {
                    _Rigidbody.SendMessageUpwards(_Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue(), _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue());
                    break;
                }
                case EPropertyName.Method_SetDensity_Single:
                {
                    _Rigidbody.SetDensity(_Method_SetDensity_Single__density.GetValue());
                    break;
                }
                case EPropertyName.Method_Sleep:
                {
                    _Rigidbody.Sleep();
                    break;
                }
                case EPropertyName.Method_SweepTestAll_Vector3:
                {
                    _Rigidbody.SweepTestAll(_Method_SweepTestAll_Vector3__direction.GetValue());
                    break;
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single:
                {
                    _Rigidbody.SweepTestAll(_Method_SweepTestAll_Vector3_Single__direction.GetValue(), _Method_SweepTestAll_Vector3_Single__maxDistance.GetValue());
                    break;
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction:
                {
                    _Rigidbody.SweepTestAll(_Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__direction.GetValue(), _Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__maxDistance.GetValue(), _Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__queryTriggerInteraction.GetValue());
                    break;
                }
                case EPropertyName.Method_ToString:
                {
                    _Rigidbody.ToString();
                    break;
                }
                case EPropertyName.Method_WakeUp:
                {
                    _Rigidbody.WakeUp();
                    break;
                }
            }
        }
        
        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_angularDrag:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_angularDrag.ToFriendlyString();
                }
                case EPropertyName.Property_angularVelocity:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_angularVelocity.ToFriendlyString();
                }
                case EPropertyName.Property_centerOfMass:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_centerOfMass.ToFriendlyString();
                }
                case EPropertyName.Property_collisionDetectionMode:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_collisionDetectionMode.ToFriendlyString();
                }
                case EPropertyName.Property_constraints:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_constraints.ToFriendlyString();
                }
                case EPropertyName.Property_detectCollisions:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_detectCollisions.ToFriendlyString();
                }
                case EPropertyName.Property_drag:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_drag.ToFriendlyString();
                }
                case EPropertyName.Property_freezeRotation:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_freezeRotation.ToFriendlyString();
                }
                case EPropertyName.Property_hideFlags:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_hideFlags.ToFriendlyString();
                }
                case EPropertyName.Property_inertiaTensor:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_inertiaTensor.ToFriendlyString();
                }
                case EPropertyName.Property_interpolation:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_interpolation.ToFriendlyString();
                }
                case EPropertyName.Property_isKinematic:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_isKinematic.ToFriendlyString();
                }
                case EPropertyName.Property_mass:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_mass.ToFriendlyString();
                }
                case EPropertyName.Property_maxAngularVelocity:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_maxAngularVelocity.ToFriendlyString();
                }
                case EPropertyName.Property_maxDepenetrationVelocity:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_maxDepenetrationVelocity.ToFriendlyString();
                }
                case EPropertyName.Property_name:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_name.ToFriendlyString();
                }
                case EPropertyName.Property_position:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_position.ToFriendlyString();
                }
                case EPropertyName.Property_sleepThreshold:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_sleepThreshold.ToFriendlyString();
                }
                case EPropertyName.Property_solverIterations:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_solverIterations.ToFriendlyString();
                }
                case EPropertyName.Property_solverVelocityIterations:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_solverVelocityIterations.ToFriendlyString();
                }
                case EPropertyName.Property_tag:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_tag.ToFriendlyString();
                }
                case EPropertyName.Property_useGravity:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_useGravity.ToFriendlyString();
                }
                case EPropertyName.Property_velocity:
                {
                    return CommonFun.Name(_propertyName) + " = " + _Property_velocity.ToFriendlyString();
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForce_Single_Single_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForce_Single_Single_Single_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForce_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForce_Vector3_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeForce_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeForce_Vector3_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddTorque_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_AddTorque_Vector3_ForceMode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_BroadcastMessage_String:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_ClosestPointOnBounds_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_CompareTag_String:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetComponent_String:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetHashCode:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetInstanceID:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetPointVelocity_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetRelativePointVelocity_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_GetType:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_IsSleeping:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_MovePosition_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_ResetCenterOfMass:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_ResetInertiaTensor:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SendMessage_String:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SendMessageUpwards_String:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SetDensity_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_Sleep:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SweepTestAll_Vector3:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_ToString:
                {
                    return CommonFun.Name(_propertyName);
                }
                case EPropertyName.Method_WakeUp:
                {
                    return CommonFun.Name(_propertyName);
                }
            }
            return base.ToFriendlyString();
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_angularDrag:
                {
                    return _Rigidbody && _Property_angularDrag.DataValidity();
                }
                case EPropertyName.Property_angularVelocity:
                {
                    return _Rigidbody && _Property_angularVelocity.DataValidity();
                }
                case EPropertyName.Property_centerOfMass:
                {
                    return _Rigidbody && _Property_centerOfMass.DataValidity();
                }
                case EPropertyName.Property_collisionDetectionMode:
                {
                    return _Rigidbody && _Property_collisionDetectionMode.DataValidity();
                }
                case EPropertyName.Property_constraints:
                {
                    return _Rigidbody && _Property_constraints.DataValidity();
                }
                case EPropertyName.Property_detectCollisions:
                {
                    return _Rigidbody && _Property_detectCollisions.DataValidity();
                }
                case EPropertyName.Property_drag:
                {
                    return _Rigidbody && _Property_drag.DataValidity();
                }
                case EPropertyName.Property_freezeRotation:
                {
                    return _Rigidbody && _Property_freezeRotation.DataValidity();
                }
                case EPropertyName.Property_hideFlags:
                {
                    return _Rigidbody && _Property_hideFlags.DataValidity();
                }
                case EPropertyName.Property_inertiaTensor:
                {
                    return _Rigidbody && _Property_inertiaTensor.DataValidity();
                }
                case EPropertyName.Property_interpolation:
                {
                    return _Rigidbody && _Property_interpolation.DataValidity();
                }
                case EPropertyName.Property_isKinematic:
                {
                    return _Rigidbody && _Property_isKinematic.DataValidity();
                }
                case EPropertyName.Property_mass:
                {
                    return _Rigidbody && _Property_mass.DataValidity();
                }
                case EPropertyName.Property_maxAngularVelocity:
                {
                    return _Rigidbody && _Property_maxAngularVelocity.DataValidity();
                }
                case EPropertyName.Property_maxDepenetrationVelocity:
                {
                    return _Rigidbody && _Property_maxDepenetrationVelocity.DataValidity();
                }
                case EPropertyName.Property_name:
                {
                    return _Rigidbody && _Property_name.DataValidity();
                }
                case EPropertyName.Property_position:
                {
                    return _Rigidbody && _Property_position.DataValidity();
                }
                case EPropertyName.Property_sleepThreshold:
                {
                    return _Rigidbody && _Property_sleepThreshold.DataValidity();
                }
                case EPropertyName.Property_solverIterations:
                {
                    return _Rigidbody && _Property_solverIterations.DataValidity();
                }
                case EPropertyName.Property_solverVelocityIterations:
                {
                    return _Rigidbody && _Property_solverVelocityIterations.DataValidity();
                }
                case EPropertyName.Property_tag:
                {
                    return _Rigidbody && _Property_tag.DataValidity();
                }
                case EPropertyName.Property_useGravity:
                {
                    return _Rigidbody && _Property_useGravity.DataValidity();
                }
                case EPropertyName.Property_velocity:
                {
                    return _Rigidbody && _Property_velocity.DataValidity();
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single__explosionForce.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single__explosionPosition.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single__explosionRadius.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single__explosionForce.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single__explosionPosition.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single__explosionRadius.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single__upwardsModifier.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionForce.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionPosition.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__explosionRadius.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__upwardsModifier.DataValidity()) return false;
                    if(!_Method_AddExplosionForce_Single_Vector3_Single_Single_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddForce_Single_Single_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddForce_Single_Single_Single__x.DataValidity()) return false;
                    if(!_Method_AddForce_Single_Single_Single__y.DataValidity()) return false;
                    if(!_Method_AddForce_Single_Single_Single__z.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddForce_Single_Single_Single_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddForce_Single_Single_Single_ForceMode__x.DataValidity()) return false;
                    if(!_Method_AddForce_Single_Single_Single_ForceMode__y.DataValidity()) return false;
                    if(!_Method_AddForce_Single_Single_Single_ForceMode__z.DataValidity()) return false;
                    if(!_Method_AddForce_Single_Single_Single_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddForce_Vector3:
                {
                    return _Rigidbody && _Method_AddForce_Vector3__force.DataValidity();
                }
                case EPropertyName.Method_AddForce_Vector3_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddForce_Vector3_ForceMode__force.DataValidity()) return false;
                    if(!_Method_AddForce_Vector3_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddForceAtPosition_Vector3_Vector3__force.DataValidity()) return false;
                    if(!_Method_AddForceAtPosition_Vector3_Vector3__position.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddForceAtPosition_Vector3_Vector3_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddForceAtPosition_Vector3_Vector3_ForceMode__force.DataValidity()) return false;
                    if(!_Method_AddForceAtPosition_Vector3_Vector3_ForceMode__position.DataValidity()) return false;
                    if(!_Method_AddForceAtPosition_Vector3_Vector3_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single__x.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single__y.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single__z.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Single_Single_Single_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single_ForceMode__x.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single_ForceMode__y.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single_ForceMode__z.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Single_Single_Single_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeForce_Vector3:
                {
                    return _Rigidbody && _Method_AddRelativeForce_Vector3__force.DataValidity();
                }
                case EPropertyName.Method_AddRelativeForce_Vector3_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeForce_Vector3_ForceMode__force.DataValidity()) return false;
                    if(!_Method_AddRelativeForce_Vector3_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single__x.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single__y.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single__z.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Single_Single_Single_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single_ForceMode__x.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single_ForceMode__y.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single_ForceMode__z.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Single_Single_Single_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3:
                {
                    return _Rigidbody && _Method_AddRelativeTorque_Vector3__torque.DataValidity();
                }
                case EPropertyName.Method_AddRelativeTorque_Vector3_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddRelativeTorque_Vector3_ForceMode__torque.DataValidity()) return false;
                    if(!_Method_AddRelativeTorque_Vector3_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddTorque_Single_Single_Single__x.DataValidity()) return false;
                    if(!_Method_AddTorque_Single_Single_Single__y.DataValidity()) return false;
                    if(!_Method_AddTorque_Single_Single_Single__z.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddTorque_Single_Single_Single_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddTorque_Single_Single_Single_ForceMode__x.DataValidity()) return false;
                    if(!_Method_AddTorque_Single_Single_Single_ForceMode__y.DataValidity()) return false;
                    if(!_Method_AddTorque_Single_Single_Single_ForceMode__z.DataValidity()) return false;
                    if(!_Method_AddTorque_Single_Single_Single_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_AddTorque_Vector3:
                {
                    return _Rigidbody && _Method_AddTorque_Vector3__torque.DataValidity();
                }
                case EPropertyName.Method_AddTorque_Vector3_ForceMode:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_AddTorque_Vector3_ForceMode__torque.DataValidity()) return false;
                    if(!_Method_AddTorque_Vector3_ForceMode__mode.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_BroadcastMessage_String:
                {
                    return _Rigidbody && _Method_BroadcastMessage_String__methodName.DataValidity();
                }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                    if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_ClosestPointOnBounds_Vector3:
                {
                    return _Rigidbody && _Method_ClosestPointOnBounds_Vector3__position.DataValidity();
                }
                case EPropertyName.Method_CompareTag_String:
                {
                    return _Rigidbody && _Method_CompareTag_String__tag.DataValidity();
                }
                case EPropertyName.Method_GetComponent_String:
                {
                    return _Rigidbody && _Method_GetComponent_String__type.DataValidity();
                }
                case EPropertyName.Method_GetHashCode:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_GetInstanceID:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_GetPointVelocity_Vector3:
                {
                    return _Rigidbody && _Method_GetPointVelocity_Vector3__worldPoint.DataValidity();
                }
                case EPropertyName.Method_GetRelativePointVelocity_Vector3:
                {
                    return _Rigidbody && _Method_GetRelativePointVelocity_Vector3__relativePoint.DataValidity();
                }
                case EPropertyName.Method_GetType:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_IsSleeping:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_MovePosition_Vector3:
                {
                    return _Rigidbody && _Method_MovePosition_Vector3__position.DataValidity();
                }
                case EPropertyName.Method_ResetCenterOfMass:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_ResetInertiaTensor:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_SendMessage_String:
                {
                    return _Rigidbody && _Method_SendMessage_String__methodName.DataValidity();
                }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                    if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_SendMessageUpwards_String:
                {
                    return _Rigidbody && _Method_SendMessageUpwards_String__methodName.DataValidity();
                }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                    if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_SetDensity_Single:
                {
                    return _Rigidbody && _Method_SetDensity_Single__density.DataValidity();
                }
                case EPropertyName.Method_Sleep:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_SweepTestAll_Vector3:
                {
                    return _Rigidbody && _Method_SweepTestAll_Vector3__direction.DataValidity();
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_SweepTestAll_Vector3_Single__direction.DataValidity()) return false;
                    if(!_Method_SweepTestAll_Vector3_Single__maxDistance.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction:
                {
                    if(!_Rigidbody) return false;
                    if(!_Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__direction.DataValidity()) return false;
                    if(!_Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__maxDistance.DataValidity()) return false;
                    if(!_Method_SweepTestAll_Vector3_Single_QueryTriggerInteraction__queryTriggerInteraction.DataValidity()) return false;
                    break;
                }
                case EPropertyName.Method_ToString:
                {
                    return _Rigidbody;
                }
                case EPropertyName.Method_WakeUp:
                {
                    return _Rigidbody;
                }
            }
            return base.DataValidity();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
            if (Rigidbody) { }
        }
        
    }
}
