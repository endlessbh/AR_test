using System;
using Unity.Jobs;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using System.Collections.Generic;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 图像数据
    /// </summary>
    [Serializable]
    [Name("图像数据")]
    public class ImageData
#if XDREAMER_AR_FOUNDATION
        : TrackData<XRTrackedImage, ARTrackedImage>
#else
        : TrackData
#endif
    {
        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        [EnumPopup]
        public ELinkRule _linkRule = ELinkRule.SameTexture;

        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        public enum ELinkRule
        {
            /// <summary>
            /// 无:不做任何操作，即在尝试更新时不执行关联检测操作；
            /// </summary>
            [Name("无")]
            [Tip("不做任何操作，即在尝试更新时不执行关联检测操作；", "Do not do anything, that is, do not perform association detection when trying to update;")]
            None,

            /// <summary>
            /// 相同纹理：可在触发任意跟踪器事件时，执行基于本规则的关联检测操作；
            /// </summary>
            [Name("相同纹理")]
            [Tip("如[AR跟踪的图像对象]中引用图像的纹理对象与当前对象中存储的对象一致时，将[AR跟踪的图像对象]与当前对象执行关联；可在触发任意跟踪器事件时，执行基于本规则的关联检测操作；", "If the texture object of the reference image in [ar tracked image object] is consistent with the object stored in the current object, associate [ar tracked image object] with the current object; The association detection operation based on this rule can be performed when any tracker event is triggered;")]
            SameTexture,

            [Name("更新库或相同纹理")]
            [Tip("在依赖的图像跟踪器组件启用（添加纹理到库）或禁用（从库中移除纹理）时更新库；同时执行与[相同纹理]一致的关联检测操作；", "Update the library when the dependent image tracker component is enabled (add texture to the Library) or disabled (remove texture from the Library); At the same time, perform the association detection operation consistent with [same texture];")]
            UpdateLibiaryOrSameTexture,
        }

        #region 相同纹理

        /// <summary>
        /// 基础相同纹理数据
        /// </summary>
        public abstract class BaseSameTextureData
        {
            /// <summary>
            /// 纹理
            /// </summary>
            public abstract Texture2D texture { get; }

#if XDREAMER_AR_FOUNDATION

            /// <summary>
            /// 是相同纹理
            /// </summary>
            /// <param name="referenceImage"></param>
            /// <returns></returns>
            public bool IsSameImage(XRReferenceImage referenceImage)
            {
                return texture && referenceImage.texture == texture;
            }
#endif
        }

        /// <summary>
        /// 相同纹理数据
        /// </summary>
        [Name("相同纹理数据")]

        [Serializable]
        public class SameTextureData: BaseSameTextureData
        {
            /// <summary>
            /// 纹理：图像的源纹理，如[AR跟踪的图像对象]中引用图像的纹理对象与当前对象中存储的对象一致时，将[AR跟踪的图像对象]与当前对象执行关联；本对象必须已在图像引用库中；
            /// </summary>
            [Name("纹理")]
            [Tip("图像的源纹理，如[AR跟踪的图像对象]中引用图像的纹理对象与当前对象中存储的对象一致时，将[AR跟踪的图像对象]与当前对象执行关联；本对象必须已在图像引用库中；", "When the source texture of the image, such as the texture object of the reference image in [ar tracked image object] is consistent with the object stored in the current object, associate [ar tracked image object] with the current object; This object must already be in the image reference library;")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public Texture2D _texture2D;

            public override Texture2D texture => _texture2D;
        }

        /// <summary>
        /// 相同纹理数据
        /// </summary>
        [Name("相同纹理数据")]
        [HideInSuperInspector(nameof(_linkRule), EValidityCheckType.NotEqual, ELinkRule.SameTexture)]
        [OnlyMemberElements]
        public SameTextureData _sameTextureData = new SameTextureData();

#endregion

        #region 更新库或相同纹理数据

        /// <summary>
        /// 更新库或相同纹理数据
        /// </summary>
        [Serializable]
        [Name("更新库或相同纹理数据")]
        public class UpdateLibiaryOrSameTextureData: BaseSameTextureData, IImageProcessor
        {
            /// <summary>
            /// 纹理：图像的源纹理，在依赖的图像跟踪器组件启用（添加纹理到库）或禁用（从库中移除纹理）时更新库；如[AR跟踪的图像对象]中引用图像的纹理对象与当前对象中存储的对象一致时，将[AR跟踪的图像对象]与当前对象执行关联；本对象必须被标记为可读；
            /// </summary>
            [Name("纹理")]
            [Tip("图像的源纹理，在依赖的图像跟踪器组件启用（添加纹理到库）或禁用（从库中移除纹理）时更新库；如[AR跟踪的图像对象]中引用图像的纹理对象与当前对象中存储的对象一致时，将[AR跟踪的图像对象]与当前对象执行关联；本对象必须被标记为可读；", "The source texture of the image, and update the library when the dependent image tracker component is enabled (add texture to the Library) or disabled (remove texture from the Library); If the texture object of the reference image in [ar tracked image object] is consistent with the object stored in the current object, associate [ar tracked image object] with the current object; This object must be marked as readable;")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public Texture2D _texture2D;

            public override Texture2D texture => _texture2D;

            [Name("名称")]
            [Tip("图像的名称", "The name of the image")]
            [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
            public string _name;

            public string name
            {
                get => _name;
                set => _name = value;
            }

            [Name("宽度")]
            [Tip("图像在真实世界中的物理尺寸宽度；单位：米；", "The physical size and width of the image in the real world; Unit: meter;")]
            [Range(0.01f, 1)]
            public float _width = 0.1f;

            public float width
            {
                get => _width;
                set => _width = value;
            }

            /// <summary>
            /// 能否添加
            /// </summary>
            public bool canAdd => texture && texture.isReadable && !string.IsNullOrEmpty(name);

            /// <summary>
            /// 是否已添加，此时可能未添加完成；
            /// </summary>
            public bool hasAdd { get; private set; } = false;

            /// <summary>
            /// 是否添加已完成
            /// </summary>
            public bool addCompleted
            {
                get
                {
#if XDREAMER_AR_FOUNDATION
                    return exists || (hasAdd && jobState.jobHandle.IsCompleted);
#else
                return false;
#endif
                }
            }

#if XDREAMER_AR_FOUNDATION


            private Coroutine addToLibraryCoroutine;

            /// <summary>
            /// 是否存在
            /// </summary>
            public bool exists { get; private set; } = false;

            /// <summary>
            /// 工作状态
            /// </summary>
            public AddReferenceImageJobState jobState { get; private set; }

            /// <summary>
            /// 添加到库
            /// </summary>
            /// <param name="mutableLibrary"></param>
            /// <returns></returns>
            public bool AddToLibrary(MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                if (!canAdd || hasAdd || mutableLibrary == null) return false;
                if (ExistsInLibrary(mutableLibrary))
                {
                    exists = true;
                    return true;
                }

                jobState = mutableLibrary.ScheduleAddImageWithValidationJob(texture, name, width);
                hasAdd = true;
                return true;
            }

            public bool ExistsInLibrary(MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                if (!canAdd || mutableLibrary == null) return false;
                foreach (var i in mutableLibrary)
                {
                    if (IsSameImage(i)) return true;
                }
                return false;
            }

            public Coroutine AddToLibrary(ARTrackedImageManager trackedImageManager, Action<bool, IEnumerable<IImageProcessor>> onFinished = null)
            {
                if (addToLibraryCoroutine != null) return addToLibraryCoroutine;
                addToLibraryCoroutine = trackedImageManager.AddToLibrary(this, (r, list) =>
                {
                    addToLibraryCoroutine = null;
                    onFinished?.Invoke(r, list);
                });
                return addToLibraryCoroutine;
            }

            public void RemoveFromLibiary(ARTrackedImageManager trackedImageManager, Action<bool, IImageProcessor> onFinished = null)
            {
                if (addToLibraryCoroutine == null) return;
                if (trackedImageManager)
                {
                    trackedImageManager.StopCoroutine(addToLibraryCoroutine);
                    addToLibraryCoroutine = null;
                }

                //原来就是库里面的，不可做移除；
                if (exists) return;

                trackedImageManager.RemoveFromLibiary(this, onFinished);
            }

#endif
        }

        /// <summary>
        /// 更新库或相同纹理数据
        /// </summary>
        [Name("更新库或相同纹理数据")]
        [HideInSuperInspector(nameof(_linkRule), EValidityCheckType.NotEqual, ELinkRule.UpdateLibiaryOrSameTexture)]
        [OnlyMemberElements]
        public UpdateLibiaryOrSameTextureData _updateLibiaryOrSameTextureData = new UpdateLibiaryOrSameTextureData();

        #endregion

        /// <summary>
        /// 纹理
        /// </summary>
        public Texture2D texture
        {
            get
            {
                switch (_linkRule)
                {
                    case ELinkRule.SameTexture: return _sameTextureData.texture;
                    case ELinkRule.UpdateLibiaryOrSameTexture: return _updateLibiaryOrSameTextureData.texture;
                    default: return default;
                }
            }
        }

        /// <summary>
        /// 图像尺寸：基于真实世界的物理尺寸；单位：米；
        /// </summary>
        public Vector2 imageSize
        {
            get
            {
#if XDREAMER_AR_FOUNDATION
                return trackable.size;
#else
                return Vector2.one;
#endif
            }
        }

        /// <summary>
        /// 显示辅助信息
        /// </summary>
        public bool displayHelpInfo
        {
            get
            {
                switch (_linkRule)
                {
                    case ELinkRule.UpdateLibiaryOrSameTexture: return !_updateLibiaryOrSameTextureData.canAdd;
                    default: return false;
                }
            }
        }

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 是相同的可跟踪对象
        /// </summary>
        /// <param name="trackable"></param>
        /// <returns></returns>
        public override bool IsSameTrackable(ARTrackedImage trackable)
        {
            if (!trackable) return false;
            switch (_linkRule)
            {
                case ELinkRule.SameTexture: return _sameTextureData.IsSameImage(trackable.referenceImage);
                case ELinkRule.UpdateLibiaryOrSameTexture: return _updateLibiaryOrSameTextureData.IsSameImage(trackable.referenceImage);
            }
            return false;
        }

        /// <summary>
        /// 根据关联规则添加到库
        /// </summary>
        /// <param name="trackedImageManager"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public Coroutine AddToLibraryByLinkRule(ARTrackedImageManager trackedImageManager, Action<bool, IEnumerable<IImageProcessor>> onFinished = null)
        {
            switch (_linkRule)
            {
                case ELinkRule.UpdateLibiaryOrSameTexture:
                    {
                        return _updateLibiaryOrSameTextureData.AddToLibrary(trackedImageManager, onFinished);
                    }
                default:
                    {
                        onFinished?.Invoke(false, null);
                        return default;
                    }
            }
        }

        /// <summary>
        /// 根据关联规则从库中移除
        /// </summary>
        /// <param name="trackedImageManager"></param>
        /// <param name="onFinished"></param>
        public void RemoveFromLibiaryByLinkRule(ARTrackedImageManager trackedImageManager, Action<bool, IImageProcessor> onFinished = null)
        {
            switch (_linkRule)
            {
                case ELinkRule.UpdateLibiaryOrSameTexture:
                    {
                        _updateLibiaryOrSameTextureData.RemoveFromLibiary(trackedImageManager, onFinished);
                        break;
                    }
                default:
                    {
                        onFinished?.Invoke(false, null);
                        break;
                    }
            }
        }

#endif

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        public override bool DataValidity()
        {
            switch (_linkRule)
            {
                case ELinkRule.SameTexture: return _sameTextureData.texture;
                case ELinkRule.UpdateLibiaryOrSameTexture: return _updateLibiaryOrSameTextureData.canAdd;
                default: return true;
            }
        }
    }
}
