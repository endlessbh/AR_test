using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Views.Inputs;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 网络变换同步器
    /// </summary>
    [Name("网络变换同步器")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class NetTransformSynchronizer : BaseSender
    {
        /// <summary>
        /// 网络变换同步数据
        /// </summary>
        [Name("网络变换同步数据")]
        public NetTransformDatas _netTransformDatas = new NetTransformDatas();

        /// <summary>
        /// 同步规则
        /// </summary>
        [Name("同步规则")]
        public enum ESyncRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 每帧
            /// </summary>
            [Name("每帧")]
            Update,

            /// <summary>
            /// 定时
            /// </summary>
            [Name("定时")]
            Timed,
        }

        /// <summary>
        /// 同步规则
        /// </summary>
        [Name("同步规则")]
        [EnumPopup]
        public ESyncRule _syncRule = ESyncRule.Timed;

        /// <summary>
        /// 间隔时间
        /// </summary>
        [Name("间隔时间")]
        [OnlyMemberElements]
        public IntervalTime _intervalTime = new IntervalTime();

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            switch (_syncRule)
            {
                case ESyncRule.Update:
                    {
                        SyncIfChanged();
                        break;
                    }
                case ESyncRule.Timed:
                    {
                        if (_intervalTime.Timeout())
                        {
                            SyncIfChanged();
                        }
                        break;
                    }
            }
        }

        private void SyncIfChanged()
        {
            NetTransformDatas netTransformData = new NetTransformDatas() { _guid = _netTransformDatas._guid };
            foreach (var data in _netTransformDatas._datas)
            {
                var newData = data.CloneIfChanged();
                if (newData != null)
                {
                    netTransformData._datas.Add(newData);
                }
            }
            _netEnd.Send(netTransformData);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            var data = new TransformData();
            data._transform = this.transform;
            _netTransformDatas._datas.Add(data);

            _intervalTime._intervalTime = 0.25f;
        }
    }

    /// <summary>
    /// 网络变换数据
    /// </summary>
    [Serializable]
    public class NetTransformDatas : ISyncData
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        [Name("唯一编号")]
        [Tip("用于网络通信时的唯一名称，标识可处理的数据包的唯一性，需要本信息完全相同才能做后续的处理", "It is a unique name used for network communication to identify the uniqueness of data packets that can be processed. It requires the same information before subsequent processing")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GuidCreater]
        public string _guid = GuidHelper.GetNewGuid();

        /// <summary>
        /// 数据列表
        /// </summary>
        [Name("数据列表")]
        public List<TransformData> _datas = new List<TransformData>();

        /// <summary>
        /// 转网络问题
        /// </summary>
        /// <returns></returns>
        public NetTransformQuestion ToNetQuestion()
        {
            var question = new NetTransformQuestion() { questionCode = EQuestionCode.Valid };
            question.data = this;
            return question;
        }

        /// <summary>
        /// 转网络答案
        /// </summary>
        /// <returns></returns>
        public NetTransformAnswer ToNetAnswer()
        {
            var answer = new NetTransformAnswer() { answerCode = EAnswerCode.Valid };
            answer.data = this;
            return answer;
        }

        NetQuestion ISyncData.ToNetQuestion() => ToNetQuestion();

        NetAnswer ISyncData.ToNetAnswer() => ToNetAnswer();
    }

    /// <summary>
    /// 变换数据
    /// </summary>
    [Serializable]
    public class TransformData
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        [Name("变换")]
        [Json(false)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [GuidCreater]
        public Transform _transform;

        /// <summary>
        /// 唯一编号
        /// </summary>
        [Name("唯一编号")]
        [Tip("用于网络通信时的唯一名称，本信息完全相同后，对存储的变换做后续的处理;即允许客户端与服务器之间的唯一编号一致，但控制的变换对象不一致；", "The unique name used for network communication. After this information is completely the same, the stored transformation will be processed subsequently; That is, the unique numbers between the client and the server are allowed to be consistent, but the controlled transformation objects are not consistent;")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _guid = GuidHelper.GetNewGuid();

        /// <summary>
        /// 变换同步模式
        /// </summary>
        [Name("变换同步模式")]
        [Tip("用于约束变换中具体哪些成员需要同步", "Used to constrain which members need to be synchronized in the transformation")]
        [EnumPopup]
        public ETransformSyncMember _syncMode = ETransformSyncMember.Local;

        /// <summary>
        /// 设置修改标志
        /// </summary>
        [Name("设置修改标志")]
        [Tip("在变换的修改标志为真后，对变换的修改标志做何种设置", "After the modification flag of the transformation is true, what is the setting of the modification flag of the transformation")]
        [Json(false)]
        [EnumPopup]
        public EBool _setChangedFlag = EBool.No;

        /// <summary>
        /// 尝试应用到:将当前存储的信息同步到传入参数指向的变换；
        /// </summary>
        /// <param name="transformData"></param>
        /// <returns></returns>
        public bool TryApplyTo(TransformData transformData)
        {
            if (transformData == null || transformData._guid != this._guid) return false;
            if (!transformData._transform) return true;

            var thisTransform = _transform;
            _transform = transformData._transform;

            RecoveryData(_syncMode & transformData._syncMode);

            _transform.hasChanged = CommonFun.BoolChange(_transform.hasChanged, _setChangedFlag);
            _transform = thisTransform;

            return true;
        }

        /// <summary>
        /// 如果变化执行克隆
        /// </summary>
        /// <returns></returns>
        public TransformData CloneIfChanged()
        {
            if (!_transform || !_transform.hasChanged) return default;
            _transform.hasChanged = CommonFun.BoolChange(true, _setChangedFlag);

            var data = new TransformData()
            {
                _transform = _transform,
                _guid = _guid,
                _syncMode = _syncMode,
            };

            data.RecordData();

            return data;
        }

        private void RecoveryData(ETransformSyncMember syncMode)
        {
            if ((syncMode & ETransformSyncMember.LocalPosition) == ETransformSyncMember.LocalPosition)
            {
                _transform.localPosition = localPosition;
            }
            if ((syncMode & ETransformSyncMember.LocalRotation) == ETransformSyncMember.LocalRotation)
            {
                _transform.localRotation = Quaternion.Euler(localRotation);
            }
            if ((syncMode & ETransformSyncMember.LocalScale) == ETransformSyncMember.LocalScale)
            {
                _transform.localScale = localScale;
            }
            if ((syncMode & ETransformSyncMember.Position) == ETransformSyncMember.Position)
            {
                _transform.position = position;
            }
            if ((syncMode & ETransformSyncMember.Rotation) == ETransformSyncMember.Rotation)
            {
                _transform.rotation = Quaternion.Euler(rotation);
            }
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        /// <returns></returns>
        private void RecordData()
        {
            if ((_syncMode & ETransformSyncMember.LocalPosition) == ETransformSyncMember.LocalPosition)
            {
                localPosition = _transform.localPosition;
            }
            if ((_syncMode & ETransformSyncMember.LocalRotation) == ETransformSyncMember.LocalRotation)
            {
                localRotation = _transform.localRotation.eulerAngles;
            }
            if ((_syncMode & ETransformSyncMember.LocalScale) == ETransformSyncMember.LocalScale)
            {
                localScale = _transform.localScale;
            }
            if ((_syncMode & ETransformSyncMember.Position) == ETransformSyncMember.Position)
            {
                position = _transform.position;
            }
            if ((_syncMode & ETransformSyncMember.Rotation) == ETransformSyncMember.Rotation)
            {
                rotation = _transform.rotation.eulerAngles;
            }
        }

        /// <summary>
        /// 本地位置
        /// </summary>
        [Json(exportString = true)]
        public Vector3 localPosition { get; set; }

        /// <summary>
        /// 本地旋转
        /// </summary>
        [Json(exportString = true)]
        public Vector3 localRotation { get; set; }

        /// <summary>
        /// 本地
        /// </summary>
        [Json(exportString = true)]
        public Vector3 localScale { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [Json(exportString = true)]
        public Vector3 position { get; set; }

        /// <summary>
        /// 世界
        /// </summary>
        [Json(exportString = true)]
        public Vector3 rotation { get; set; }
    }

    /// <summary>
    /// 变换同步成员
    /// </summary>
    [Flags]
    public enum ETransformSyncMember
    {
        /// <summary>
        /// 本地位置
        /// </summary>
        [Name("本地位置")]
        LocalPosition = 1 << 0,

        /// <summary>
        /// 本地旋转
        /// </summary>
        [Name("本地旋转")]
        LocalRotation = 1 << 1,

        /// <summary>
        /// 本地缩放
        /// </summary>
        [Name("本地缩放")]
        LocalScale = 1 << 2,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local = LocalPosition | LocalRotation | LocalScale,

        /// <summary>
        /// 位置
        /// </summary>
        [Name("位置")]
        Position = 1 << 10,

        /// <summary>
        /// 旋转
        /// </summary>
        [Name("旋转")]
        Rotation = 1 << 11,

        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World = Position | Rotation,
    }

    /// <summary>
    /// 网络变换问题
    /// </summary>
    public class NetTransformQuestion : NetQuestion
    {
        /// <summary>
        /// 网络变换数据
        /// </summary>
        public NetTransformDatas data { get; set; } = new NetTransformDatas();
    }

    /// <summary>
    /// 网络变换答案
    /// </summary>
    public class NetTransformAnswer : NetAnswer
    {
        /// <summary>
        /// 网络变换数据
        /// </summary>
        public NetTransformDatas data { get; set; } = new NetTransformDatas();
    }
}
