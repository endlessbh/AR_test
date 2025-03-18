using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXAR.Foundation.Faces.Tools;

#if XDREAMER_AR_FOUNDATION
#endif

namespace XCSJ.PluginXAR.Foundation.Faces.States
{
    /// <summary>
    /// 面部跟踪器属性设置
    /// </summary>
    [Name(Title, nameof(FaceTrackerPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class FaceTrackerPropertySet : BasePropertySet<FaceTrackerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "面部跟踪器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(FaceTrackerPropertySet))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 面部跟踪器
        /// </summary>
        [Name("面部跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public FaceTracker _faceTracker;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
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
            None,

            /// <summary>
            /// AR面部网格渲染器材质
            /// </summary>
            [Name("AR面部网格渲染器材质")]
            ARFace_MeshRenderer_Material,
        }

        /// <summary>
        /// AR面部网格渲染器材质
        /// </summary>
        [Name("AR面部网格渲染器材质")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.ARFace_MeshRenderer_Material)]
        [OnlyMemberElements]
        public MaterialPropertyValue _ARFace_MeshRenderer_material = new MaterialPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_faceTracker) return;
            switch (_propertyName)
            {
                case EPropertyName.ARFace_MeshRenderer_Material:
                    {
                        if (_ARFace_MeshRenderer_material.TryGetValue(out Material material)
                            && _faceTracker.trackable
                            && _faceTracker.trackable.GetComponent<MeshRenderer>() is MeshRenderer meshRenderer
                            && meshRenderer)
                        {
                            meshRenderer.material = material;
                        }
                        break;
                    }
            }
#endif
        }

        private DirtyString dirtyString = new DirtyString();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dirtyString.GetData(_ToFriendlyString);

        private string _ToFriendlyString()
        {
#if XDREAMER_AR_FOUNDATION
            switch (_propertyName)
            {
                case EPropertyName.ARFace_MeshRenderer_Material:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _ARFace_MeshRenderer_material.ToFriendlyString();
                    }
                default: return CommonFun.Name(_propertyName);
            }
#else
            return XARFoundationHelper.Title + "功能未启用！";
#endif
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_AR_FOUNDATION
            if (!_faceTracker) return false;
            switch (_propertyName)
            {
                case EPropertyName.ARFace_MeshRenderer_Material: return _ARFace_MeshRenderer_material.DataValidity();
            }
#endif
            return false;
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            dirtyString.SetDirty();
        }
    }
}
