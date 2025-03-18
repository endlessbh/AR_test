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
    /// 旋转到:旋转到组件是游戏对象的旋转动画。在给定的时间内，游戏对象从当前角度旋转到目标角度，旋转完成后，组件切换为完成态。
    /// </s移动到ummary>
    [Serializable]
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RotateTo))]
    [Tip("旋转到组件是游戏对象的旋转动画。在给定的时间内，游戏对象从当前角度旋转到目标角度，旋转完成后，组件切换为完成态。", "Rotate to component is the rotation animation of the game object. Within a given time, the game object rotates from the current angle to the target angle. After the rotation is completed, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [DisallowMultipleComponent]
    public class RotateTo : TransformMotion<RotateTo>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "旋转到";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(RotateTo))]
        [Tip("旋转到组件是游戏对象的旋转动画。在给定的时间内，游戏对象从当前角度旋转到目标角度，旋转完成后，组件切换为完成态。", "Rotate to component is the rotation animation of the game object. Within a given time, the game object rotates from the current angle to the target angle. After the rotation is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 旋转类型
        /// </summary>
        [Name("旋转类型")]
        public enum ERotateType
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
        /// 旋转类型
        /// </summary>
        [Name("旋转类型")]
        [EnumPopup]
        public ERotateType _rotateType = ERotateType.Transform;

        /// <summary>
        /// 旋转变换:将游戏对象集中游戏对象旋转到本参数变换对象的世界旋转的欧拉角度
        /// </summary>
        [Name("旋转变换")]
        [Tip("将游戏对象集中游戏对象旋转到本参数变换对象的世界旋转欧拉角度", "Rotate the game object in the game object set to the Euler angle of the world rotation of this parameter transformation object")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_rotateType), EValidityCheckType.NotEqual, ERotateType.Transform)]
        public Transform _rotateTransform;

        /// <summary>
        /// 欧拉角:将游戏对象集中游戏对象移动到本参数指向的世界坐标位置
        /// </summary>
        [Name("欧拉角")]
        [Tip("将游戏对象集中游戏对象旋转到本参数指向的世界旋转欧拉角度", "Rotate the game object in the game object set to the Euler angle of world rotation pointed by this parameter")]
        [HideInSuperInspector(nameof(_rotateType), EValidityCheckType.NotEqual, ERotateType.Vector3)]
        public Vector3 _eulerAngles = new Vector3();

        /// <summary>
        /// 变量:将游戏对象集中游戏对象旋转到本参数指向变量的世界旋转欧拉角度
        /// </summary>
        [Name("变量")]
        [Tip("将游戏对象集中游戏对象旋转到本参数指向变量的世界旋转欧拉角度", "Rotate the game object in the game object set to the Euler angle of the world rotation of the variable pointed to by this parameter")]
        [HideInSuperInspector(nameof(_rotateType), EValidityCheckType.NotEqual, ERotateType.Variable)]
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
        /// 欧拉角
        /// </summary>
        public Vector3 eulerAngles
        {
            get
            {
                switch (_rotateType)
                {
                    case ERotateType.Vector3: return _eulerAngles;
                    case ERotateType.Variable: return Converter.instance.ConvertTo<Vector3>(_variable.GetHierarchyVarValue());
                    default: return _rotateTransform.rotation.eulerAngles;
                }
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion rotation
        {
            get
            {
                switch (_rotateType)
                {
                    case ERotateType.Vector3: return Quaternion.Euler(_eulerAngles);
                    case ERotateType.Variable: return Quaternion.Euler(Converter.instance.ConvertTo<Vector3>(_variable.GetHierarchyVarValue()));
                    default: return _rotateTransform.rotation;
                }
            }
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="info"></param>
        /// <param name="percent"></param>
        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            info.transform.rotation = Quaternion.Lerp(info.worldRotation, rotation, (float)percent.percent01OfWorkCurve);
        }
    }
}
