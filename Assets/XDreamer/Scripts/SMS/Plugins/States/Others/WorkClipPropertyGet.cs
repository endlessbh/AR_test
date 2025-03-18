using System;
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
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Base.Others
{
    /// <summary>
    /// 工作剪辑属性获取: 工作剪辑属性获取
    /// </summary>
    [ComponentMenu("其它/" + Title)]
    [Name(Title, nameof(WorkClipPropertyGet))]
    [Tip("工作剪辑属性获取")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class WorkClipPropertyGet : BasePropertyGet<WorkClipPropertyGet>, IDropdownPopupAttribute, IPropertyPathList, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "工作剪辑属性获取";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("其它", typeof(SMSManager))]
        [StateComponentMenu("其它/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(WorkClipPropertyGet))]
        [Tip("工作剪辑属性获取")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [RequireManager(typeof(SMSManager))]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// 工作剪辑:Work Clip
        /// </summary>
        [Name("工作剪辑")]
        [Tip("Work Clip")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup]
        public WorkClip _WorkClip;
        
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
            
            #region 字段
            
            /// <summary>
            /// 启用(字段):表示当前对象是否正常工作
            /// </summary>
            [Name("启用(字段)")]
            [Tip("表示当前对象是否正常工作", "Indicates whether the current object is working properly")]
            [EnumFieldName("字段/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field__enable = 1,
            
            /// <summary>
            /// 单次时长(字段):当前状态组件完整执行一次表现逻辑的期望时长
            /// </summary>
            [Name("单次时长(字段)")]
            [Tip("当前状态组件完整执行一次表现逻辑的期望时长", "The expected length of time for the current state component to fully execute the presentation logic once")]
            [EnumFieldName("字段/单次时长")]
            Field__onceTimeLength,
            
            /// <summary>
            /// 超出工作区间继续循环(字段):当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件是否继续执行循环逻辑;为True时,继续执行循环逻辑;为False时,不再继续执行循环逻辑;
            /// </summary>
            [Name("超出工作区间继续循环(字段)")]
            [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件是否继续执行循环逻辑;为True时,继续执行循环逻辑;为False时,不再继续执行循环逻辑;", "If the working time (percentage) of the current state component exceeds (exceeds) the right value of the working effective time (percentage) interval, whether the current state component continues to execute the loop logic; When true, continue to execute the loop logic; When it is false, the loop logic will not be executed again;")]
            [EnumFieldName("字段/超出工作区间继续循环")]
            Field_continueLoopAfterWorkRange,
            
            /// <summary>
            /// Epsilon(字段):
            /// </summary>
            [Name("Epsilon(字段)")]
            [EnumFieldName("字段/Epsilon")]
            Field_Epsilon,
            
            /// <summary>
            /// 最少循环次数(字段):当已经循环次数大于等于本值时,本状态组件设置为完成态;
            /// </summary>
            [Name("最少循环次数(字段)")]
            [Tip("当已经循环次数大于等于本值时,本状态组件设置为完成态;", "When the number of cycles is greater than or equal to this value, the component in this status is set to the completed status;")]
            [EnumFieldName("字段/最少循环次数")]
            Field_leastLoopCount,
            
            /// <summary>
            /// 锁定比例(字段):锁定百分比与时间的比例关系,根据锁定时当前状态组件总时长,对二者进行等比例同步调整;即其中一区间数据发生修改，另一区间数据将同步进行等比例的数据修改;
            /// </summary>
            [Name("锁定比例(字段)")]
            [Tip("锁定百分比与时间的比例关系,根据锁定时当前状态组件总时长,对二者进行等比例同步调整;即其中一区间数据发生修改，另一区间数据将同步进行等比例的数据修改;", "The proportional relationship between locking percentage and time, and the two are adjusted synchronously in equal proportion according to the total time of components in the current state at the time of locking; That is, if one section data is modified, the other section data will be modified in equal proportion synchronously;")]
            [EnumFieldName("字段/锁定比例")]
            Field_lockRatioOfWorkRange,
            
            /// <summary>
            /// 循环类型(字段):当前组件在其有效时间区间(百分比区间)内执行逻辑的循环类型;
            /// </summary>
            [Name("循环类型(字段)")]
            [Tip("当前组件在其有效时间区间(百分比区间)内执行逻辑的循环类型;", "The type of loop in which the current component executes logic within its effective time interval (percentage interval);")]
            [EnumFieldName("字段/循环类型")]
            Field_loopType,
            
            /// <summary>
            /// 超出工作区间时百分比(字段):当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件将保持当前百分比设定的值所对应的状态;
            /// </summary>
            [Name("超出工作区间时百分比(字段)")]
            [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件将保持当前百分比设定的值所对应的状态;", "After the working time (percentage) of the current status component exceeds (exceeds) the right value of the working effective time (percentage) interval, the current status component will maintain the state corresponding to the value set by the current percentage;")]
            [EnumFieldName("字段/超出工作区间时百分比")]
            Field_percentOnAfterWorkRange,
            
            /// <summary>
            /// 进入时百分比(字段):当状态组件进入后,将当前状态组件逻辑数尝试保持在本百分比进度;
            /// </summary>
            [Name("进入时百分比(字段)")]
            [Tip("当状态组件进入后,将当前状态组件逻辑数尝试保持在本百分比进度;", "When the status component enters, try to keep the current status component logic number at this percentage progress;")]
            [EnumFieldName("字段/进入时百分比")]
            Field_percentOnEntry,
            
            /// <summary>
            /// 退出时百分比(字段):当状态组件退出后,将当前状态组件逻辑数据保持在本百分比进度;值为0,可将数据尽量还原到初始化/进入的状态;
            /// </summary>
            [Name("退出时百分比(字段)")]
            [Tip("当状态组件退出后,将当前状态组件逻辑数据保持在本百分比进度;值为0,可将数据尽量还原到初始化/进入的状态;", "When the status component exits, keep the logic data of the current status component at this percentage progress; If the value is 0, the data can be restored to the initialization / entry state as much as possible;")]
            [EnumFieldName("字段/退出时百分比")]
            Field_percentOnExit,
            
            /// <summary>
            /// 进入时设置百分比(字段):为True时,在当前状态组件进入时设置百分比进度为0;为False时,不做处理;
            /// </summary>
            [Name("进入时设置百分比(字段)")]
            [Tip("为True时,在当前状态组件进入时设置百分比进度为0;为False时,不做处理;", "When true, set the percentage progress to 0 when the current state component enters; If it is false, it will not be processed;")]
            [EnumFieldName("字段/进入时设置百分比")]
            Field_setPercentOnEntry,
            
            /// <summary>
            /// 退出时设置百分比(字段):为True时,在当前状态组件退出时设置百分比进度为1;为False时,不做处理;
            /// </summary>
            [Name("退出时设置百分比(字段)")]
            [Tip("为True时,在当前状态组件退出时设置百分比进度为1;为False时,不做处理;", "When true, set the percentage progress to 1 when the current state component exits; If it is false, it will not be processed;")]
            [EnumFieldName("字段/退出时设置百分比")]
            Field_setPercentOnExit,
            
            /// <summary>
            /// 同步OTL(字段):将单次时长与当前状态组件的有效时长保持同步,即二者修改会互相影响;OTL,即单次时长(Once Time Length缩写)
            /// </summary>
            [Name("同步OTL(字段)")]
            [Tip("将单次时长与当前状态组件的有效时长保持同步,即二者修改会互相影响;OTL,即单次时长(Once Time Length缩写)", "Keep the single time duration synchronized with the effective duration of the current state component, that is, the modification of the two will affect each other; OTL, i.e. single time length (abbreviation for once time length)")]
            [EnumFieldName("字段/同步OTL")]
            Field_syncOTL,
            
            /// <summary>
            /// 同步时长(字段):将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)
            /// </summary>
            [Name("同步时长(字段)")]
            [Tip("将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)", "Automatically synchronize the duration in real time to the content duration carried by some effective field members in the current status component; Ensure that the starting time remains unchanged during synchronization; TL, time length (short for time length)")]
            [EnumFieldName("字段/同步时长")]
            Field_syncTL,
            
            /// <summary>
            /// 锁定比例的总时长(字段):
            /// </summary>
            [Name("锁定比例的总时长(字段)")]
            [EnumFieldName("字段/锁定比例的总时长")]
            Field_ttlOfLockRatio,
            
            /// <summary>
            /// 使用初始化数据(字段):为True时，则使用初始化时记录的数据信息进行后续的更新与处理；为False时，则使用进入时(即状态启用或再次启用时)记录的数据信息进行后续的更新与处理
            /// </summary>
            [Name("使用初始化数据(字段)")]
            [Tip("为True时，则使用初始化时记录的数据信息进行后续的更新与处理；为False时，则使用进入时(即状态启用或再次启用时)记录的数据信息进行后续的更新与处理", "When the basic setting is true, the data information recorded during initialization is used for subsequent update and processing; If it is false, the data information recorded when entering (i.e. when the status is enabled or enabled again) is used for subsequent update and processing")]
            [EnumFieldName("字段/使用初始化数据")]
            Field_useInitData,
            
            /// <summary>
            /// 工作曲线(字段):工作曲线仅在当前组件的工作区间的有效百分比区间内生效
            /// </summary>
            [Name("工作曲线(字段)")]
            [Tip("工作曲线仅在当前组件的工作区间的有效百分比区间内生效", "The working curve takes effect only within the effective percentage range of the working range of the current component")]
            [EnumFieldName("字段/工作曲线")]
            Field_workCurve,
            
            /// <summary>
            /// 工作区间(字段):当前组件在状态生命周期内的工作区间(时间与百分比)信息
            /// </summary>
            [Name("工作区间(字段)")]
            [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息", "Loopsetting the working interval (time and percentage) information of the current component in the state life cycle")]
            [EnumFieldName("字段/工作区间")]
            Field_workRange,
            
            #endregion
            
            #region 属性
            
            /// <summary>
            /// 百分比(属性):
            /// </summary>
            [Name("百分比(属性)")]
            [EnumFieldName("属性/百分比")]
            Property_percent = 1000,
            
            /// <summary>
            /// Progress(属性):
            /// </summary>
            [Name("Progress(属性)")]
            [EnumFieldName("属性/Progress")]
            Property_progress,
            
            /// <summary>
            /// Loop(属性):
            /// </summary>
            [Name("Loop(属性)")]
            [EnumFieldName("属性/Loop")]
            Property_loop,
            
            /// <summary>
            /// Loop Count(属性):
            /// </summary>
            [Name("Loop Count(属性)")]
            [EnumFieldName("属性/Loop Count")]
            Property_loopCount,
            
            /// <summary>
            /// 单次时长(属性):当前状态组件完整执行一次表现逻辑的期望时长
            /// </summary>
            [Name("单次时长(属性)")]
            [Tip("当前状态组件完整执行一次表现逻辑的期望时长", "")]
            [EnumFieldName("属性/单次时长")]
            Property_onceTimeLength,
            
            /// <summary>
            /// Once Percent Length(属性):
            /// </summary>
            [Name("Once Percent Length(属性)")]
            [EnumFieldName("属性/Once Percent Length")]
            Property_oncePercentLength,
            
            /// <summary>
            /// Total Time Length(属性):
            /// </summary>
            [Name("Total Time Length(属性)")]
            [EnumFieldName("属性/Total Time Length")]
            Property_totalTimeLength,
            
            /// <summary>
            /// Begin Time(属性):
            /// </summary>
            [Name("Begin Time(属性)")]
            [EnumFieldName("属性/Begin Time")]
            Property_beginTime,
            
            /// <summary>
            /// End Time(属性):
            /// </summary>
            [Name("End Time(属性)")]
            [EnumFieldName("属性/End Time")]
            Property_endTime,
            
            /// <summary>
            /// Time Length(属性):
            /// </summary>
            [Name("Time Length(属性)")]
            [EnumFieldName("属性/Time Length")]
            Property_timeLength,
            
            /// <summary>
            /// Begin Percent(属性):
            /// </summary>
            [Name("Begin Percent(属性)")]
            [EnumFieldName("属性/Begin Percent")]
            Property_beginPercent,
            
            /// <summary>
            /// End Percent(属性):
            /// </summary>
            [Name("End Percent(属性)")]
            [EnumFieldName("属性/End Percent")]
            Property_endPercent,
            
            /// <summary>
            /// Percent Length(属性):
            /// </summary>
            [Name("Percent Length(属性)")]
            [EnumFieldName("属性/Percent Length")]
            Property_percentLength,
            
            /// <summary>
            /// Work Clip Player(属性):
            /// </summary>
            [Name("Work Clip Player(属性)")]
            [EnumFieldName("属性/Work Clip Player")]
            Property_workClipPlayer,
            
            /// <summary>
            /// Component Collection(属性):
            /// </summary>
            [Name("Component Collection(属性)")]
            [EnumFieldName("属性/Component Collection")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_componentCollection,
            
            /// <summary>
            /// Entry Time(属性):
            /// </summary>
            [Name("Entry Time(属性)")]
            [EnumFieldName("属性/Entry Time")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_entryTime,
            
            /// <summary>
            /// Exit Time(属性):
            /// </summary>
            [Name("Exit Time(属性)")]
            [EnumFieldName("属性/Exit Time")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_exitTime,
            
            /// <summary>
            /// Update Time(属性):
            /// </summary>
            [Name("Update Time(属性)")]
            [EnumFieldName("属性/Update Time")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_updateTime,
            
            /// <summary>
            /// 父级(属性):父级
            /// </summary>
            [Name("父级(属性)")]
            [Tip("父级", "")]
            [EnumFieldName("属性/父级")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_parent,
            
            /// <summary>
            /// 启用(属性):表示当前对象是否正常工作
            /// </summary>
            [Name("启用(属性)")]
            [Tip("表示当前对象是否正常工作", "")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enable,
            
            /// <summary>
            /// Host Component(属性):
            /// </summary>
            [Name("Host Component(属性)")]
            [EnumFieldName("属性/Host Component")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hostComponent,
            
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
            /// Clone(方法):
            /// </summary>
            [Name("Clone(方法)")]
            [EnumFieldName("方法/Clone")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Clone = 10000,
            
            /// <summary>
            /// Clone From(方法):
            /// </summary>
            [Name("Clone From(方法)")]
            [EnumFieldName("方法/Clone From")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CloneFrom_Model,
            
            /// <summary>
            /// Data Validity(方法):
            /// </summary>
            [Name("Data Validity(方法)")]
            [EnumFieldName("方法/Data Validity")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_DataValidity,
            
            /// <summary>
            /// Default Delete(方法):
            /// </summary>
            [Name("Default Delete(方法)")]
            [EnumFieldName("方法/Default Delete")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_DefaultDelete_Boolean,
            
            /// <summary>
            /// Delete(方法):
            /// </summary>
            [Name("Delete(方法)")]
            [EnumFieldName("方法/Delete")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Delete_Boolean,
            
            /// <summary>
            /// Finished(方法):
            /// </summary>
            [Name("Finished(方法)")]
            [EnumFieldName("方法/Finished")]
            Method_Finished,
            
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
            /// 获取百分比(方法):
            /// </summary>
            [Name("获取百分比(方法)")]
            [EnumFieldName("方法/获取百分比")]
            Method_GetPercent_Double,
            
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
            /// Get Will Delete Models(方法):
            /// </summary>
            [Name("Get Will Delete Models(方法)")]
            [EnumFieldName("方法/Get Will Delete Models")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetWillDeleteModels,
            
            /// <summary>
            /// Set Percent(方法):
            /// </summary>
            [Name("Set Percent(方法)")]
            [EnumFieldName("方法/Set Percent")]
            Method_SetPercent_Double,
            
            /// <summary>
            /// Set Percent Of State(方法):
            /// </summary>
            [Name("Set Percent Of State(方法)")]
            [EnumFieldName("方法/Set Percent Of State")]
            Method_SetPercentOfState_Double,
            
            /// <summary>
            /// Set Time(方法):
            /// </summary>
            [Name("Set Time(方法)")]
            [EnumFieldName("方法/Set Time")]
            Method_SetTime_Double,
            
            /// <summary>
            /// Set Time Of State(方法):
            /// </summary>
            [Name("Set Time Of State(方法)")]
            [EnumFieldName("方法/Set Time Of State")]
            Method_SetTimeOfState_Double,
            
            /// <summary>
            /// To Friendly String(方法):
            /// </summary>
            [Name("To Friendly String(方法)")]
            [EnumFieldName("方法/To Friendly String")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToFriendlyString,
            
            /// <summary>
            /// 转字符串(方法):
            /// </summary>
            [Name("转字符串(方法)")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            /// <summary>
            /// Validity(方法):
            /// </summary>
            [Name("Validity(方法)")]
            [EnumFieldName("方法/Validity")]
            Method_Validity_Double,
            
            #endregion
            
        }
        
        #region 方法
        
        /// <summary>
        /// from:
        /// </summary>
        [Name("from")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CloneFrom_Model)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Method_CloneFrom_Model__from = new UnityObjectPropertyValue();
        
        /// <summary>
        /// deleteObject:
        /// </summary>
        [Name("deleteObject")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_DefaultDelete_Boolean)]
        [OnlyMemberElements]
        public BoolPropertyValue _Method_DefaultDelete_Boolean__deleteObject = new BoolPropertyValue();
        
        /// <summary>
        /// deleteObject:
        /// </summary>
        [Name("deleteObject")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Delete_Boolean)]
        [OnlyMemberElements]
        public BoolPropertyValue _Method_Delete_Boolean__deleteObject = new BoolPropertyValue();
        
        /// <summary>
        /// percentOfState:
        /// </summary>
        [Name("percentOfState")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetPercent_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_GetPercent_Double__percentOfState = new DoublePropertyValue();
        
        /// <summary>
        /// percent:
        /// </summary>
        [Name("percent")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetPercent_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_SetPercent_Double__percent = new DoublePropertyValue();
        
        /// <summary>
        /// percent:
        /// </summary>
        [Name("percent")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetPercentOfState_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_SetPercentOfState_Double__percent = new DoublePropertyValue();
        
        /// <summary>
        /// 时间:
        /// </summary>
        [Name("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetTime_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_SetTime_Double__time = new DoublePropertyValue();
        
        /// <summary>
        /// 时间:
        /// </summary>
        [Name("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetTimeOfState_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_SetTimeOfState_Double__time = new DoublePropertyValue();
        
        /// <summary>
        /// ttl:
        /// </summary>
        [Name("ttl")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Validity_Double)]
        [OnlyMemberElements]
        public DoublePropertyValue _Method_Validity_Double__ttl = new DoublePropertyValue();
        
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
            switch (_propertyName)
            {
                case EPropertyName.Field__enable:
                    {
                        value = _WorkClip._enable;
                        return true;
                    }
                case EPropertyName.Field__onceTimeLength:
                    {
                        value = _WorkClip._onceTimeLength;
                        return true;
                    }
                case EPropertyName.Field_continueLoopAfterWorkRange:
                    {
                        value = _WorkClip.continueLoopAfterWorkRange;
                        return true;
                    }
                case EPropertyName.Field_Epsilon:
                    {
                        value = WorkClip.Epsilon;
                        return true;
                    }
                case EPropertyName.Field_leastLoopCount:
                    {
                        value = _WorkClip.leastLoopCount;
                        return true;
                    }
                case EPropertyName.Field_lockRatioOfWorkRange:
                    {
                        value = _WorkClip.lockRatioOfWorkRange;
                        return true;
                    }
                case EPropertyName.Field_loopType:
                    {
                        value = _WorkClip.loopType;
                        return true;
                    }
                case EPropertyName.Field_percentOnAfterWorkRange:
                    {
                        value = _WorkClip.percentOnAfterWorkRange;
                        return true;
                    }
                case EPropertyName.Field_percentOnEntry:
                    {
                        value = _WorkClip.percentOnEntry;
                        return true;
                    }
                case EPropertyName.Field_percentOnExit:
                    {
                        value = _WorkClip.percentOnExit;
                        return true;
                    }
                case EPropertyName.Field_setPercentOnEntry:
                    {
                        value = _WorkClip.setPercentOnEntry;
                        return true;
                    }
                case EPropertyName.Field_setPercentOnExit:
                    {
                        value = _WorkClip.setPercentOnExit;
                        return true;
                    }
                case EPropertyName.Field_syncOTL:
                    {
                        value = _WorkClip.syncOTL;
                        return true;
                    }
                case EPropertyName.Field_syncTL:
                    {
                        value = _WorkClip.syncTL;
                        return true;
                    }
                case EPropertyName.Field_ttlOfLockRatio:
                    {
                        value = _WorkClip.ttlOfLockRatio;
                        return true;
                    }
                case EPropertyName.Field_useInitData:
                    {
                        value = _WorkClip.useInitData;
                        return true;
                    }
                case EPropertyName.Field_workCurve:
                    {
                        value = _WorkClip.workCurve;
                        return true;
                    }
                case EPropertyName.Field_workRange:
                    {
                        value = _WorkClip.workRange;
                        return true;
                    }
                case EPropertyName.Property_percent:
                    {
                        value = _WorkClip.percent;
                        return true;
                    }
                case EPropertyName.Property_progress:
                    {
                        value = _WorkClip.progress;
                        return true;
                    }
                case EPropertyName.Property_loop:
                    {
                        value = _WorkClip.loop;
                        return true;
                    }
                case EPropertyName.Property_loopCount:
                    {
                        value = _WorkClip.loopCount;
                        return true;
                    }
                case EPropertyName.Property_onceTimeLength:
                    {
                        value = _WorkClip.onceTimeLength;
                        return true;
                    }
                case EPropertyName.Property_oncePercentLength:
                    {
                        value = _WorkClip.oncePercentLength;
                        return true;
                    }
                case EPropertyName.Property_totalTimeLength:
                    {
                        value = _WorkClip.totalTimeLength;
                        return true;
                    }
                case EPropertyName.Property_beginTime:
                    {
                        value = _WorkClip.beginTime;
                        return true;
                    }
                case EPropertyName.Property_endTime:
                    {
                        value = _WorkClip.endTime;
                        return true;
                    }
                case EPropertyName.Property_timeLength:
                    {
                        value = _WorkClip.timeLength;
                        return true;
                    }
                case EPropertyName.Property_beginPercent:
                    {
                        value = _WorkClip.beginPercent;
                        return true;
                    }
                case EPropertyName.Property_endPercent:
                    {
                        value = _WorkClip.endPercent;
                        return true;
                    }
                case EPropertyName.Property_percentLength:
                    {
                        value = _WorkClip.percentLength;
                        return true;
                    }
                case EPropertyName.Property_workClipPlayer:
                    {
                        value = _WorkClip.workClipPlayer;
                        return true;
                    }
                case EPropertyName.Property_componentCollection:
                    {
                        value = _WorkClip.componentCollection;
                        return true;
                    }
                case EPropertyName.Property_entryTime:
                    {
                        value = _WorkClip.entryTime;
                        return true;
                    }
                case EPropertyName.Property_exitTime:
                    {
                        value = _WorkClip.exitTime;
                        return true;
                    }
                case EPropertyName.Property_updateTime:
                    {
                        value = _WorkClip.updateTime;
                        return true;
                    }
                case EPropertyName.Property_parent:
                    {
                        value = _WorkClip.parent;
                        return true;
                    }
                case EPropertyName.Property_enable:
                    {
                        value = _WorkClip.enable;
                        return true;
                    }
                case EPropertyName.Property_hostComponent:
                    {
                        value = _WorkClip.hostComponent;
                        return true;
                    }
                case EPropertyName.Property_name:
                    {
                        value = _WorkClip.name;
                        return true;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        value = _WorkClip.hideFlags;
                        return true;
                    }
                case EPropertyName.Method_Clone:
                    {
                        value = _WorkClip.Clone();
                        return true;
                    }
                case EPropertyName.Method_CloneFrom_Model:
                    {
                        value = _WorkClip.CloneFrom(_Method_CloneFrom_Model__from.GetValue() as Model);
                        return true;
                    }
                case EPropertyName.Method_DataValidity:
                    {
                        value = _WorkClip.DataValidity();
                        return true;
                    }
                case EPropertyName.Method_DefaultDelete_Boolean:
                    {
                        value = _WorkClip.DefaultDelete(_Method_DefaultDelete_Boolean__deleteObject.GetValue());
                        return true;
                    }
                case EPropertyName.Method_Delete_Boolean:
                    {
                        value = _WorkClip.Delete(_Method_Delete_Boolean__deleteObject.GetValue());
                        return true;
                    }
                case EPropertyName.Method_Finished:
                    {
                        value = _WorkClip.Finished();
                        return true;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        value = _WorkClip.GetHashCode();
                        return true;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        value = _WorkClip.GetInstanceID();
                        return true;
                    }
                case EPropertyName.Method_GetPercent_Double:
                    {
                        value = _WorkClip.GetPercent(_Method_GetPercent_Double__percentOfState.GetValue());
                        return true;
                    }
                case EPropertyName.Method_GetType:
                    {
                        value = _WorkClip.GetType();
                        return true;
                    }
                case EPropertyName.Method_GetWillDeleteModels:
                    {
                        value = _WorkClip.GetWillDeleteModels();
                        return true;
                    }
                case EPropertyName.Method_SetPercent_Double:
                    {
                        value = _WorkClip.SetPercent(_Method_SetPercent_Double__percent.GetValue());
                        return true;
                    }
                case EPropertyName.Method_SetPercentOfState_Double:
                    {
                        value = _WorkClip.SetPercentOfState(_Method_SetPercentOfState_Double__percent.GetValue());
                        return true;
                    }
                case EPropertyName.Method_SetTime_Double:
                    {
                        value = _WorkClip.SetTime(_Method_SetTime_Double__time.GetValue());
                        return true;
                    }
                case EPropertyName.Method_SetTimeOfState_Double:
                    {
                        value = _WorkClip.SetTimeOfState(_Method_SetTimeOfState_Double__time.GetValue());
                        return true;
                    }
                case EPropertyName.Method_ToFriendlyString:
                    {
                        value = _WorkClip.ToFriendlyString();
                        return true;
                    }
                case EPropertyName.Method_ToString:
                    {
                        value = _WorkClip.ToString();
                        return true;
                    }
                case EPropertyName.Method_Validity_Double:
                    {
                        value = _WorkClip.Validity(_Method_Validity_Double__ttl.GetValue());
                        return true;
                    }
            }
            value = default;
            return false;
        }
        
        /// <summary>
        /// 尝试获取属性值类型
        /// </summary>
        /// <param name="type">类型</param>
         public bool TryGetPropertyValueType(out Type type)
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__enable:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip._enable))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field__onceTimeLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip._onceTimeLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_continueLoopAfterWorkRange:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.continueLoopAfterWorkRange))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_Epsilon:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.Epsilon))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_leastLoopCount:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.leastLoopCount))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_lockRatioOfWorkRange:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.lockRatioOfWorkRange))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_loopType:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.loopType))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_percentOnAfterWorkRange:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.percentOnAfterWorkRange))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_percentOnEntry:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.percentOnEntry))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_percentOnExit:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.percentOnExit))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_setPercentOnEntry:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.setPercentOnEntry))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_setPercentOnExit:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.setPercentOnExit))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_syncOTL:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.syncOTL))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_syncTL:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.syncTL))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_ttlOfLockRatio:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.ttlOfLockRatio))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_useInitData:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.useInitData))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_workCurve:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.workCurve))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_workRange:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.workRange))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_percent:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.percent))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_progress:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.progress))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_loop:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.loop))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_loopCount:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.loopCount))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_onceTimeLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.onceTimeLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_oncePercentLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.oncePercentLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_totalTimeLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.totalTimeLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_beginTime:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.beginTime))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_endTime:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.endTime))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_timeLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.timeLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_beginPercent:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.beginPercent))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_endPercent:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.endPercent))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_percentLength:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.percentLength))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_workClipPlayer:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.workClipPlayer))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_componentCollection:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.componentCollection))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_entryTime:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.entryTime))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_exitTime:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.exitTime))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_updateTime:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.updateTime))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_parent:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.parent))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_enable:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.enable))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_hostComponent:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.hostComponent))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_name:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.name))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        type = TypeMemberCache.Get(typeof(WorkClip), nameof(WorkClip.hideFlags))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Method_Clone:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.Clone), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_CloneFrom_Model:
                    {
                        var types = new Type[] { _Method_CloneFrom_Model__from.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.CloneFrom), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_DataValidity:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.DataValidity), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_DefaultDelete_Boolean:
                    {
                        var types = new Type[] { _Method_DefaultDelete_Boolean__deleteObject.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.DefaultDelete), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_Delete_Boolean:
                    {
                        var types = new Type[] { _Method_Delete_Boolean__deleteObject.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.Delete), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_Finished:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.Finished), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.GetHashCode), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.GetInstanceID), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetPercent_Double:
                    {
                        var types = new Type[] { _Method_GetPercent_Double__percentOfState.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.GetPercent), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetType:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.GetType), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetWillDeleteModels:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.GetWillDeleteModels), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetPercent_Double:
                    {
                        var types = new Type[] { _Method_SetPercent_Double__percent.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.SetPercent), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetPercentOfState_Double:
                    {
                        var types = new Type[] { _Method_SetPercentOfState_Double__percent.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.SetPercentOfState), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetTime_Double:
                    {
                        var types = new Type[] { _Method_SetTime_Double__time.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.SetTime), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_SetTimeOfState_Double:
                    {
                        var types = new Type[] { _Method_SetTimeOfState_Double__time.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.SetTimeOfState), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_ToFriendlyString:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.ToFriendlyString), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_ToString:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.ToString), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_Validity_Double:
                    {
                        var types = new Type[] { _Method_Validity_Double__ttl.valueType };
                        type = typeof(WorkClip).GetMethod(nameof(WorkClip.Validity), types)?.ReturnType;
                        return type != null;
                    }
            }
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
            return ScriptOption.VarFlag + _variableName + " = " + CommonFun.Name(_propertyName);
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
        }
        
    }
}
