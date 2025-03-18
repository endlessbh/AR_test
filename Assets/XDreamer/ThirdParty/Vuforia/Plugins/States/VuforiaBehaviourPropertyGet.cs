using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginVuforia;
using XCSJ.Scripts;

#if XDREAMER_VUFORIA
using Vuforia;
#endif

namespace XCSJ.PluginVuforia.States
{
    /// <summary>
    /// Vuforia Behaviour属性获取: Vuforia Behaviour属性获取
    /// </summary>
    [ComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
    [Name(Title, nameof(VuforiaBehaviourPropertyGet))]
    [Tip("Vuforia Behaviour属性获取")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(VuforiaManager))]
    public class VuforiaBehaviourPropertyGet : BasePropertyGet<VuforiaBehaviourPropertyGet>, IDropdownPopupAttribute, IPropertyPathList, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Vuforia Behaviour属性获取";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VuforiaHelper.Title, typeof(VuforiaManager))]
        [StateComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
        [Name(Title, nameof(VuforiaBehaviourPropertyGet))]
        [Tip("Vuforia Behaviour属性获取")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VUFORIA

        /// <summary>
        /// Vuforia Behaviour:Vuforia Behaviour
        /// </summary>
        [Name("Vuforia Behaviour")]
        [Tip("Vuforia Behaviour")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public VuforiaBehaviour _VuforiaBehaviour;
        
        /// <summary>
        /// Vuforia Behaviour:Vuforia Behaviour
        /// </summary>
        public VuforiaBehaviour VuforiaBehaviour => this.XGetComponentInGlobal(ref _VuforiaBehaviour, true);
        
#endif

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
            [Tip("无", "None")]
            [EnumFieldName("无")]
            None,
            
            #region 属性
            
            /// <summary>
            /// World Center Mode(属性):
            /// </summary>
            [Name("World Center Mode(属性)")]
            [EnumFieldName("属性/World Center Mode")]
            Property_WorldCenterMode = 1000,
            
            /// <summary>
            /// World Center(属性):
            /// </summary>
            [Name("World Center(属性)")]
            [EnumFieldName("属性/World Center")]
            Property_WorldCenter,
            
            /// <summary>
            /// Instance(属性):
            /// </summary>
            [Name("Instance(属性)")]
            [EnumFieldName("属性/Instance")]
            Property_Instance,
            
            /// <summary>
            /// Camera Device(属性):
            /// </summary>
            [Name("Camera Device(属性)")]
            [EnumFieldName("属性/Camera Device")]
            Property_CameraDevice,
            
            /// <summary>
            /// Observer Factory(属性):
            /// </summary>
            [Name("Observer Factory(属性)")]
            [EnumFieldName("属性/Observer Factory")]
            Property_ObserverFactory,
            
            /// <summary>
            /// Device Pose Behaviour(属性):
            /// </summary>
            [Name("Device Pose Behaviour(属性)")]
            [EnumFieldName("属性/Device Pose Behaviour")]
            Property_DevicePoseBehaviour,
            
            /// <summary>
            /// Session Recorder(属性):
            /// </summary>
            [Name("Session Recorder(属性)")]
            [EnumFieldName("属性/Session Recorder")]
            Property_SessionRecorder,
            
            /// <summary>
            /// Video Background(属性):
            /// </summary>
            [Name("Video Background(属性)")]
            [EnumFieldName("属性/Video Background")]
            Property_VideoBackground,
            
            /// <summary>
            /// World(属性):
            /// </summary>
            [Name("World(属性)")]
            [EnumFieldName("属性/World")]
            Property_World,
            
            /// <summary>
            /// 使用GUI布局(属性):
            /// </summary>
            [Name("使用GUI布局(属性)")]
            [EnumFieldName("属性/使用GUI布局")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            /// <summary>
            /// 启用(属性):
            /// </summary>
            [Name("启用(属性)")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled,
            
            /// <summary>
            /// 是激活并启用(属性):
            /// </summary>
            [Name("是激活并启用(属性)")]
            [EnumFieldName("属性/是激活并启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_isActiveAndEnabled,
            
            /// <summary>
            /// Transform(属性):
            /// </summary>
            [Name("Transform(属性)")]
            [EnumFieldName("属性/Transform")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transform,
            
            /// <summary>
            /// Game Object(属性):
            /// </summary>
            [Name("Game Object(属性)")]
            [EnumFieldName("属性/Game Object")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_gameObject,
            
            /// <summary>
            /// 标签(属性):
            /// </summary>
            [Name("标签(属性)")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// 名称(属性):
            /// </summary>
            [Name("名称(属性)")]
            [EnumFieldName("属性/名称")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// 隐藏标识(属性):
            /// </summary>
            [Name("隐藏标识(属性)")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// 比较标签(标签)(方法):
            /// </summary>
            [Name("比较标签(标签)(方法)")]
            [EnumFieldName("方法/比较标签(标签)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String = 10000,
            
            /// <summary>
            /// 获取组件(类型)(方法):
            /// </summary>
            [Name("获取组件(类型)(方法)")]
            [EnumFieldName("方法/获取组件(类型)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// 获取哈希码(方法):
            /// </summary>
            [Name("获取哈希码(方法)")]
            [EnumFieldName("方法/获取哈希码")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// 获取实例ID(方法):
            /// </summary>
            [Name("获取实例ID(方法)")]
            [EnumFieldName("方法/获取实例ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// 获取类型(方法):
            /// </summary>
            [Name("获取类型(方法)")]
            [EnumFieldName("方法/获取类型")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// 是否调用(方法):
            /// </summary>
            [Name("是否调用(方法)")]
            [EnumFieldName("方法/是否调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// 是否调用(方法名)(方法):
            /// </summary>
            [Name("是否调用(方法名)(方法)")]
            [EnumFieldName("方法/是否调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// Set Maximum Simultaneous Tracked Images(方法):
            /// </summary>
            [Name("Set Maximum Simultaneous Tracked Images(方法)")]
            [EnumFieldName("方法/Set Maximum Simultaneous Tracked Images")]
            Method_SetMaximumSimultaneousTrackedImages_Int32,
            
            /// <summary>
            /// Set Model Target Reco While Extended Tracked(方法):
            /// </summary>
            [Name("Set Model Target Reco While Extended Tracked(方法)")]
            [EnumFieldName("方法/Set Model Target Reco While Extended Tracked")]
            Method_SetModelTargetRecoWhileExtendedTracked_Boolean,
            
            /// <summary>
            /// 启动协程(方法):
            /// </summary>
            [Name("启动协程(方法)")]
            [EnumFieldName("方法/启动协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// 转字符串(方法):
            /// </summary>
            [Name("转字符串(方法)")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            #endregion
            
        }
        
        #region 方法
        
        /// <summary>
        /// 标签:
        /// </summary>
        [Name("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// 类型:
        /// </summary>
        [Name("类型")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// maxNumberOfTargets:
        /// </summary>
        [Name("maxNumberOfTargets")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetMaximumSimultaneousTrackedImages_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_SetMaximumSimultaneousTrackedImages_Int32__maxNumberOfTargets = new IntPropertyValue();
        
        /// <summary>
        /// enable:
        /// </summary>
        [Name("enable")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetModelTargetRecoWhileExtendedTracked_Boolean)]
        [OnlyMemberElements]
        public BoolPropertyValue _Method_SetModelTargetRecoWhileExtendedTracked_Boolean__enable = new BoolPropertyValue();
        
        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();
        
        #endregion
        
        /// <summary>
        /// 属性路径列表
        /// </summary>
        [Name("属性路径列表")]
        public PropertyPathList _propertyPathList = new PropertyPathList();
        
        /// <summary>
        /// 变量名:将获取到的属性值存储在变量名对应的变量中
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的变量中", "Store the obtained attribute value in the variable corresponding to the variable name")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variableName;
        
        /// <summary>
        /// 变量名列表
        /// </summary>
        [Name("变量名列表")]
        [Tip("将获取到的属性值存储在变量名列表中变量名对应的变量中", "Store the obtained attribute value in the variable corresponding to the variable name in the variable name list")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public List<string> _variableNames = new List<string>();

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variableName);
        }

        #endregion

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(object value)
        {
            _variableName.TrySetOrAddSetHierarchyVarValue(value, _variableNames);
        }
        
        /// <summary>
        /// 获取属性路径值
        /// </summary>
        /// <param name="value" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(out object value)
        {
            if (TryGetPropertyValue(out var instance) && _propertyPathList.TryGetPropertyPathValue(instance, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(out Type type)
        {
            if (TryGetPropertyValueType(out var instance) && _propertyPathList.TryGetTypeMemberCacheValue(instance, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(int index, out object value)
        {
            if (TryGetPropertyValue(out var instance) && _propertyPathList.TryGetPropertyPathValue(instance, index, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(int index, out Type type)
        {
            if (TryGetPropertyValueType(out var instance) && _propertyPathList.TryGetTypeMemberCacheValue(instance, index, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }
            
        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="value">值</param>
        public bool TryGetPropertyValue(out object value)
        {

#if XDREAMER_VUFORIA
            switch (_propertyName)
            {
                case EPropertyName.Property_WorldCenterMode:
                    {
                        value = _VuforiaBehaviour.WorldCenterMode;
                        return true;
                    }
                case EPropertyName.Property_WorldCenter:
                    {
                        value = _VuforiaBehaviour.WorldCenter;
                        return true;
                    }
                case EPropertyName.Property_Instance:
                    {
                        value = VuforiaBehaviour.Instance;
                        return true;
                    }
                case EPropertyName.Property_CameraDevice:
                    {
                        value = _VuforiaBehaviour.CameraDevice;
                        return true;
                    }
                case EPropertyName.Property_ObserverFactory:
                    {
                        value = _VuforiaBehaviour.ObserverFactory;
                        return true;
                    }
                case EPropertyName.Property_DevicePoseBehaviour:
                    {
                        value = _VuforiaBehaviour.DevicePoseBehaviour;
                        return true;
                    }
                case EPropertyName.Property_SessionRecorder:
                    {
                        value = _VuforiaBehaviour.SessionRecorder;
                        return true;
                    }
                case EPropertyName.Property_VideoBackground:
                    {
                        value = _VuforiaBehaviour.VideoBackground;
                        return true;
                    }
                case EPropertyName.Property_World:
                    {
                        value = _VuforiaBehaviour.World;
                        return true;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        value = _VuforiaBehaviour.useGUILayout;
                        return true;
                    }
                case EPropertyName.Property_enabled:
                    {
                        value = _VuforiaBehaviour.enabled;
                        return true;
                    }
                case EPropertyName.Property_isActiveAndEnabled:
                    {
                        value = _VuforiaBehaviour.isActiveAndEnabled;
                        return true;
                    }
                case EPropertyName.Property_transform:
                    {
                        value = _VuforiaBehaviour.transform;
                        return true;
                    }
                case EPropertyName.Property_gameObject:
                    {
                        value = _VuforiaBehaviour.gameObject;
                        return true;
                    }
                case EPropertyName.Property_tag:
                    {
                        value = _VuforiaBehaviour.tag;
                        return true;
                    }
                case EPropertyName.Property_name:
                    {
                        value = _VuforiaBehaviour.name;
                        return true;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        value = _VuforiaBehaviour.hideFlags;
                        return true;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        value = _VuforiaBehaviour.CompareTag(_Method_CompareTag_String__tag.GetValue());
                        return true;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        value = _VuforiaBehaviour.GetComponent(_Method_GetComponent_String__type.GetValue());
                        return true;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        value = _VuforiaBehaviour.GetHashCode();
                        return true;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        value = _VuforiaBehaviour.GetInstanceID();
                        return true;
                    }
                case EPropertyName.Method_GetType:
                    {
                        value = _VuforiaBehaviour.GetType();
                        return true;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        value = _VuforiaBehaviour.IsInvoking();
                        return true;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        value = _VuforiaBehaviour.IsInvoking(_Method_IsInvoking_String__methodName.GetValue());
                        return true;
                    }
                case EPropertyName.Method_SetMaximumSimultaneousTrackedImages_Int32:
                    {
                        value = _VuforiaBehaviour.SetMaximumSimultaneousTrackedImages(_Method_SetMaximumSimultaneousTrackedImages_Int32__maxNumberOfTargets.GetValue());
                        return true;
                    }
                case EPropertyName.Method_SetModelTargetRecoWhileExtendedTracked_Boolean:
                    {
                        value = _VuforiaBehaviour.SetModelTargetRecoWhileExtendedTracked(_Method_SetModelTargetRecoWhileExtendedTracked_Boolean__enable.GetValue());
                        return true;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        value = _VuforiaBehaviour.StartCoroutine(_Method_StartCoroutine_String__methodName.GetValue());
                        return true;
                    }
                case EPropertyName.Method_ToString:
                    {
                        value = _VuforiaBehaviour.ToString();
                        return true;
                    }
            }
#endif
            value = default;
            return false;
        }
        
        /// <summary>
        /// 尝试获取属性值类型
        /// </summary>
        /// <param name="type">类型</param>
         public bool TryGetPropertyValueType(out Type type)
        {
#if XDREAMER_VUFORIA
            switch (_propertyName)
            {
                case EPropertyName.Property_WorldCenterMode:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.WorldCenterMode))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_WorldCenter:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.WorldCenter))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_Instance:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.Instance))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_CameraDevice:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.CameraDevice))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_ObserverFactory:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.ObserverFactory))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_DevicePoseBehaviour:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.DevicePoseBehaviour))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_SessionRecorder:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.SessionRecorder))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_VideoBackground:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.VideoBackground))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_World:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.World))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.useGUILayout))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_enabled:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.enabled))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_isActiveAndEnabled:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.isActiveAndEnabled))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_transform:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.transform))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_gameObject:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.gameObject))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_tag:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.tag))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_name:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.name))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        type = TypeMemberCache.Get(typeof(VuforiaBehaviour), nameof(VuforiaBehaviour.hideFlags))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var types = new Type[] { _Method_CompareTag_String__tag.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.CompareTag), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var types = new Type[] { _Method_GetComponent_String__type.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.GetComponent), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.GetHashCode), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.GetInstanceID), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetType:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.GetType), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.IsInvoking), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var types = new Type[] { _Method_IsInvoking_String__methodName.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.IsInvoking), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetMaximumSimultaneousTrackedImages_Int32:
                    {
                        var types = new Type[] { _Method_SetMaximumSimultaneousTrackedImages_Int32__maxNumberOfTargets.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.SetMaximumSimultaneousTrackedImages), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetModelTargetRecoWhileExtendedTracked_Boolean:
                    {
                        var types = new Type[] { _Method_SetModelTargetRecoWhileExtendedTracked_Boolean__enable.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.SetModelTargetRecoWhileExtendedTracked), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var types = new Type[] { _Method_StartCoroutine_String__methodName.valueType };
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.StartCoroutine), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_ToString:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(VuforiaBehaviour).GetMethod(nameof(VuforiaBehaviour.ToString), types)?.ReturnType;
                        return type != null;
                    }
            }
#endif
            type = default;
            return false;
        }
        
        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetOptions(purpose, propertyPath, out options);
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetOption(purpose, propertyPath, options, propertyValue, out option);
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
            }
            propertyValue = default;
            return false;
        }
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (TryGetPropertyPathValue(out var value))
            {
                SetToVariable(value);
            }
        }
        
        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _variableName + " = " + CommonFun.Name(_propertyName);
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
#if XDREAMER_VUFORIA
            if (VuforiaBehaviour) { }
#endif
        }

    }
}
