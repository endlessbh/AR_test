using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Algorithms
{
    #region 运行时平台信息

    /// <summary>
    /// 基础运行时平台信息泛型类
    /// </summary>
    /// <typeparam name="TDetailInfo"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [LanguageFileOutput]
    public abstract class BaseRuntimePlatformInfo<TDetailInfo, TValue>
        where TDetailInfo : BaseRuntimePlatformDetailInfo<TValue>, new()
    {
        /// <summary>
        /// 值
        /// </summary>
        public abstract TValue value { get; set; }

        /// <summary>
        /// 详细信息列表
        /// </summary>
        public abstract List<TDetailInfo> detailInfos { get; }

        /// <summary>
        /// 重置：重置运行时平台信息的基础值
        /// </summary>
        /// <param name="value"></param>
        public void Reset(TValue value) => this.value = value;

        /// <summary>
        /// 重置:重置详细信息列表中对应运行时平台类型的值；有则更新，无则创建；
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <param name="value"></param>
        public TDetailInfo Reset(RuntimePlatform runtimePlatform, TValue value)
        {
            var detailInfo = GetOrCreateDetailInfo(runtimePlatform);
            detailInfo.value = value;
            return detailInfo;
        }

        /// <summary>
        /// 获取或创建详细信息；有则返回，无则创建再返回；
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        public TDetailInfo GetOrCreateDetailInfo(RuntimePlatform runtimePlatform)
        {
            var detailInfo = detailInfos.FirstOrNew(di => di._runtimePlatform == runtimePlatform, di =>
            {
                di._runtimePlatform = runtimePlatform;
                detailInfos.Add(di);
            });
            return detailInfo;
        }

        /// <summary>
        /// 获取运行时平台对应的值；如果列表中不存在对应的运行时平台详细信息，则返回基础值；
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        public TValue GetValue(RuntimePlatform runtimePlatform)
        {
            if (detailInfos.FirstOrDefault(di => di._runtimePlatform == runtimePlatform) is TDetailInfo detailInfo)
            {
                return detailInfo.value;
            }
            return this.value;
        }

        /// <summary>
        /// 获取当前运行时平台对应的值
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        public TValue GetValueOfCurrentRuntimePlatform() => GetValue(Application.platform);
    }

    /// <summary>
    /// 运行时平台信息泛型类
    /// </summary>
    /// <typeparam name="TDetailInfo"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class RuntimePlatformInfo<TDetailInfo, TValue> : BaseRuntimePlatformInfo<TDetailInfo, TValue>
        where TDetailInfo : BaseRuntimePlatformDetailInfo<TValue>, new()
    {
        /// <summary>
        /// 值：运行时平台信息的基础值
        /// </summary>
        [Name("值")]
        [Tip("运行时平台信息的基础值", "Base value of runtime platform information")]
        public TValue _value = default(TValue);

        /// <summary>
        /// 值：运行时平台信息的基础值
        /// </summary>
        public override TValue value { get => _value; set => _value = value; }

        /// <summary>
        /// 详细信息列表
        /// </summary>
        [Name("详细信息列表")]
        [OnlyMemberElements]
        public List<TDetailInfo> _detailInfos = new List<TDetailInfo>();

        /// <summary>
        /// 详细信息列表
        /// </summary>
        public override List<TDetailInfo> detailInfos => _detailInfos;
    }

    /// <summary>
    /// 枚举型运行时平台信息泛型类
    /// </summary>
    /// <typeparam name="TDetailInfo"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class EnumRuntimePlatformInfo<TDetailInfo, TValue> : BaseRuntimePlatformInfo<TDetailInfo, TValue>
        where TDetailInfo : EnumRuntimePlatformDetailInfo<TValue>, new()
    {
        /// <summary>
        /// 值：运行时平台信息的基础值
        /// </summary>
        [Name("值")]
        [Tip("运行时平台信息的基础值", "Base value of runtime platform information")]
        [EnumPopup]
        public TValue _value = default(TValue);

        /// <summary>
        /// 值：运行时平台信息的基础值
        /// </summary>
        public override TValue value { get => _value; set => _value = value; }

        /// <summary>
        /// 详细信息列表
        /// </summary>
        [Name("详细信息列表")]
        [OnlyMemberElements]
        public List<TDetailInfo> _detailInfos = new List<TDetailInfo>();

        /// <summary>
        /// 详细信息列表
        /// </summary>
        public override List<TDetailInfo> detailInfos => _detailInfos;
    }

    #endregion

    #region 运行时平台详细信息

    /// <summary>
    /// 基础基础运行时平台详细信息
    /// </summary>
    [LanguageFileOutput]
    public abstract class BaseRuntimePlatformDetailInfo
    {
        /// <summary>
        /// 运行时平台:会根据运行时平台类型，以确定是否使用当前调节信息中的值；
        /// </summary>
        [Name("运行时平台")]
        [Tip("会根据运行时平台类型，以确定是否使用当前调节信息中的值；", "Depending on the type of runtime platform, it determines whether or not to use the value of the current adjustment information.")]
        [FormerlySerializedAs(nameof(runtimePlatform))]
        public RuntimePlatform _runtimePlatform;

        /// <summary>
        /// 运行时平台:会根据运行时平台类型，以确定是否使用当前调节信息中的值；
        /// </summary>
        public RuntimePlatform runtimePlatform => _runtimePlatform;
    }

    /// <summary>
    /// 基础运行时详细信息泛型类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BaseRuntimePlatformDetailInfo<TValue> : BaseRuntimePlatformDetailInfo
    {
        /// <summary>
        /// 值:当前运行时平台的调节数值
        /// </summary>
        public abstract TValue value { get; set; }
    }

    /// <summary>
    /// 运行时详细信息泛型类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class RuntimePlatformDetailInfo<TValue> : BaseRuntimePlatformDetailInfo<TValue>
    {
        /// <summary>
        /// 值:当前运行时平台的调节数值
        /// </summary>
        [Name("值")]
        [Tip("当前运行时平台的调节数值；", "Adjustment value of the platform during current operation;")]
        [FormerlySerializedAs(nameof(value))]
        public TValue _value;

        /// <summary>
        /// 值:当前运行时平台的调节数值
        /// </summary>
        public override TValue value { get => _value; set => _value = value; }
    }

    /// <summary>
    /// 枚举型运行时详细信息泛型类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class EnumRuntimePlatformDetailInfo<TValue> : BaseRuntimePlatformDetailInfo<TValue>
    {
        /// <summary>
        /// 值:当前运行时平台的调节数值
        /// </summary>
        [Name("值")]
        [Tip("当前运行时平台的调节数值；", "Adjustment value of the platform during current operation;")]
        [EnumPopup]
        public TValue _value;

        /// <summary>
        /// 值:当前运行时平台的调节数值
        /// </summary>
        public override TValue value { get => _value; set => _value = value; }
    }

    #endregion

    #region EBool运行时平台信息

    /// <summary>
    /// <see cref="EBool"/>运行时平台信息
    /// </summary>
    [Serializable]
    [Name("布尔运行时平台信息")]
    public class EBoolRuntimePlatformInfo : EnumRuntimePlatformInfo<EBoolRuntimePlatformInfo.DetailInfo, EBool>
    {
        /// <summary>
        /// 详细信息
        /// </summary>
        [Serializable]
        [Name("详细信息")]
        public class DetailInfo : EnumRuntimePlatformDetailInfo<EBool> { }
    }

    #endregion

    #region 基础更新浮点型运行时平台信息

    /// <summary>
    /// 基础更新浮点型运行时平台信息泛型类：针对Unity的更新函数中流逝时间与浮点型值（或可与浮点数转化的类型值）的信息类；
    /// </summary>
    /// <typeparam name="TDetailInfo"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BaseUpdateFloatRuntimePlatformInfo<TDetailInfo, TValue> : RuntimePlatformInfo<TDetailInfo, TValue>
        where TDetailInfo : RuntimePlatformDetailInfo<TValue>, new()
    {
        /// <summary>
        /// 实时值：经过执行更新<see cref="Update(float, TValue)"/>函数后的实时值；不同子类中表示不同的意思；
        /// </summary>
        public TValue valueRealtime { get; protected set; }

        /// <summary>
        /// 当前运行时平台使用的详细信息；在初始化<see cref="Init"/>函数中执行的初始化；
        /// </summary>
        public TDetailInfo detailInfo { get; protected set; }

        /// <summary>
        /// 重置：重置运行时平台信息的基础值
        /// </summary>
        /// <param name="value"></param>
        public void Reset(float value) => Reset(ToValue(value));

        /// <summary>
        /// 重置:重置详细信息列表中对应运行时平台类型的值；有则更新，无则创建；
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <param name="value"></param>
        public TDetailInfo Reset(RuntimePlatform runtimePlatform, float value = 1) => Reset(runtimePlatform, ToValue(value));

        /// <summary>
        /// 用于Awake或是Start中调用一次的初始化函数
        /// </summary>
        public virtual void Init()
        {
            detailInfo = GetOrCreateDetailInfo(Application.platform);
            Update(1);
        }

        /// <summary>
        /// 在本函数中更新实时值<see cref="valueRealtime"/>
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="value"></param>
        public abstract void Update(float deltaTime, TValue value = default);

        /// <summary>
        /// 将浮点数值转为泛型值类型的对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract TValue ToValue(float value);
    }

    #endregion

    #region 更新Vector3型运行时平台信息

    /// <summary>
    /// 更新<see cref="Vector3"/>型运行时平台信息
    /// </summary>
    [Serializable]
    [Name("更新三维向量型运行时平台信息")]
    public class UpdateVector3RuntimePlatformInfo : BaseUpdateFloatRuntimePlatformInfo<UpdateVector3RuntimePlatformInfo.DetailInfo, Vector3>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public UpdateVector3RuntimePlatformInfo()
        {
            this._value = Vector3.one;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="value"></param>
        public override void Update(float deltaTime, Vector3 value)
        {
            valueRealtime = Vector3.Scale(value, Vector3.Scale(_value, deltaTime * detailInfo._value));
        }

        /// <summary>
        /// 转换值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Vector3 ToValue(float value) => Vector3.one * value;

        /// <summary>
        /// 详细信息
        /// </summary>
        [Serializable]
        [Name("详细信息")]
        public class DetailInfo : RuntimePlatformDetailInfo<Vector3>
        {
            /// <summary>
            /// 构造
            /// </summary>
            public DetailInfo()
            {
                this._value = Vector3.one;
            }
        }
    }

    #endregion

    #region 更新float型运行时平台信息

    /// <summary>
    /// 更新<see cref="float"/>型运行时平台信息
    /// </summary>
    [Serializable]
    [Name("更新浮点型运行时平台信息")]
    public class UpdateFloatRuntimePlatformInfo : BaseUpdateFloatRuntimePlatformInfo<UpdateFloatRuntimePlatformInfo.DetailInfo, float>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public UpdateFloatRuntimePlatformInfo()
        {
            this._value = 1;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="value"></param>
        public override void Update(float deltaTime, float value = 1)
        {
            valueRealtime = _value * detailInfo._value * deltaTime * value;
        }

        /// <summary>
        /// 转换值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override float ToValue(float value) => value;

        /// <summary>
        /// 详细信息
        /// </summary>
        [Serializable]
        [Name("详细信息")]
        public class DetailInfo : RuntimePlatformDetailInfo<float> 
        {
            /// <summary>
            /// 构造
            /// </summary>
            public DetailInfo()
            {
                this._value = 1;
            }
        }
    }

    #endregion
}
