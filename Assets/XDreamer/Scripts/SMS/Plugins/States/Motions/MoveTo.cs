using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 移动到:移动到组件是游戏对象的相对路径动画。在给定的时间内，游戏对象从当前位置移动到目标位置，移动完成后，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(MoveTo))]
    [Tip("移动到组件是游戏对象的相对路径动画。在给定的时间内，游戏对象从当前位置移动到目标位置，移动完成后，组件切换为完成态。", "Moving to a component is the relative path animation of the game object. Within a given time, the game object moves from the current position to the target position. After the movement is completed, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [DisallowMultipleComponent]
    public class MoveTo : TransformMotion<MoveTo>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "移动到";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(MoveTo))]
        [Tip("移动到组件是游戏对象的相对路径动画。在给定的时间内，游戏对象从当前位置移动到目标位置，移动完成后，组件切换为完成态。", "Moving to a component is the relative path animation of the game object. Within a given time, the game object moves from the current position to the target position. After the movement is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 位置类型
        /// </summary>
        [Name("位置类型")]
        public enum EPositionType
        {
            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            Transform = 0,

            /// <summary>
            /// 三维向量
            /// </summary>
            [Name("三维向量")]
            Vector3,

            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            Variable,
        }

        /// <summary>
        /// 位置类型
        /// </summary>
        [Name("位置类型")]
        [EnumPopup]
        public EPositionType _positionType = EPositionType.Transform;

        /// <summary>
        /// 位置变换:将游戏对象集中游戏对象移动到本参数变换对象的世界坐标位置
        /// </summary>
        [Name("位置变换")]
        [Tip("将游戏对象集中游戏对象移动到本参数变换对象的世界坐标位置", "Move the game object in the game object set to the world coordinate position of the transformation object with this parameter")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_positionType), EValidityCheckType.NotEqual, EPositionType.Transform)]
        public Transform _positionTransform;

        /// <summary>
        /// 位置:将游戏对象集中游戏对象移动到本参数指向的世界坐标位置
        /// </summary>
        [Name("位置")]
        [Tip("将游戏对象集中游戏对象移动到本参数指向的世界坐标位置", "Move the game object in the game object set to the world coordinate position pointed by this parameter")]
        [HideInSuperInspector(nameof(_positionType), EValidityCheckType.NotEqual, EPositionType.Vector3)]
        public Vector3 _position = new Vector3();

        /// <summary>
        /// 变量:将游戏对象集中游戏对象旋转到本参数指向变量的世界旋转欧拉角度
        /// </summary>
        [Name("变量")]
        [Tip("将游戏对象集中游戏对象旋转到本参数指向变量的世界旋转欧拉角度", "Rotate the game object in the game object set to the Euler angle of the world rotation of the variable pointed to by this parameter")]
        [HideInSuperInspector(nameof(_positionType), EValidityCheckType.NotEqual, EPositionType.Variable)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        public string _variable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variable);
        }

        #endregion

        /// <summary>
        /// 位置,不考虑偏移量<see cref="_offset"/>，根据位置类型<see cref="_positionType" />获取的位置坐标值；
        /// </summary>
        public Vector3 position
        {
            get
            {
                switch (_positionType)
                {
                    case EPositionType.Vector3: return _position;
                    case EPositionType.Variable: return Converter.instance.ConvertTo<Vector3>(_variable.GetHierarchyVarValue());
                    default: return _positionTransform.position;
                }
            }
        }

        /// <summary>
        /// 偏移量：对位置<see cref="position"/>做偏移量纠正后得到目标位置<see cref="targetPosition"/>
        /// </summary>
        [Name("偏移量")]
        [Tip("对位置做偏移量纠正后得到目标位置", "The target position is obtained after correcting the offset of the position")]
        public Vector3 _offset = new Vector3();

        /// <summary>
        /// 移动到的目标位置,会考虑计算偏移量<see cref="_offset"/>
        /// </summary>
        public Vector3 targetPosition => position + _offset;

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="info"></param>
        /// <param name="percent"></param>
        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            info.transform.position = Vector3.Lerp(info.worldPosition, targetPosition, (float)percent.percent01OfWorkCurve);
        }
    }
}
