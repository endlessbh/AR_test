using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 获取被点击的碰撞体:获取被点击碰撞体组件是检测点击事件发生并获取点击的碰撞体信息的触发器。点击事件发生后，被点击的碰撞体将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GetClickedCollider))]
    [Tip("获取被点击碰撞体组件是检测点击事件发生并获取点击的碰撞体信息的触发器。点击事件发生后，被点击的碰撞体将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。", "The component of obtaining the clicked collider is a trigger to detect the occurrence of the click event and obtain the information of the clicked collider. After the click event occurs, the clicked collider will be stored in the specified global variable as the object full path (unity hierarchical tree path), and the component will be switched to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33603)]
    public class GetClickedCollider : Trigger<GetClickedCollider>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "获取被点击的碰撞体";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GetClickedCollider))]
        [Tip("获取被点击碰撞体组件是检测点击事件发生并获取点击的碰撞体信息的触发器。点击事件发生后，被点击的碰撞体将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。", "The component of obtaining the clicked collider is a trigger to detect the occurrence of the click event and obtain the information of the clicked collider. After the click event occurs, the clicked collider will be stored in the specified global variable as the object full path (unity hierarchical tree path), and the component will be switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClickType clickType = EClickType.DownAndUp;

        /// <summary>
        /// 最大距离:射线检测的最大距离
        /// </summary>
        [Name("最大距离")]
        [Tip("射线检测的最大距离", "Maximum distance of radiographic testing")]
        [Min(0.01f)]
        public float _maxDistance = 1000f;

        /// <summary>
        /// 图层遮罩:射线检测时的图层遮罩
        /// </summary>
        [Name("图层遮罩")]
        [Tip("射线检测时的图层遮罩", "Layer mask during radiographic testing")]
        public LayerMask _layerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// 碰撞体变量:会将检测到的碰撞体以路径字符串的形式存储在变量中
        /// </summary>
        [Name("碰撞体变量")]
        [Tip("会将检测到的碰撞体以路径字符串的形式存储在变量中", "The detected collision body is stored in the variable as a path string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string colliderVariable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref colliderVariable);
        }

        #endregion

        private Collider collider;

        /// <summary>
        /// 进入时
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            collider = null;
        }

        private Collider GetCollider()
        {
            var cam = CameraHelperExtension.currentCamera;
            if (!cam)
            {
                Log.Warning("相机缺失!");
                return null;
            }

            Ray ray = cam.ScreenPointToRay(XInput.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, _maxDistance, _layerMask))
            {
                
                return hitInfo.collider;
            }
            return null;
        }

        /// <summary>
        /// 更新时
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            if (XInput.GetMouseButtonDown(0))
            {
                switch (clickType)
                {
                    case EClickType.DownAndUp:
                        {
                            collider = GetCollider();
                            break;
                        }
                    case EClickType.Down:
                        {
                            finished = collider = GetCollider();
                            break;
                        }
                }
            }
            else if (XInput.GetMouseButtonUp(0))
            {
                switch (clickType)
                {
                    case EClickType.DownAndUp:
                        {
                            finished = collider && collider == GetCollider();
                            break;
                        }
                    case EClickType.Up:
                        {
                            finished = collider = GetCollider();
                            break;
                        }
                }
            }

            if (finished)
            {
                colliderVariable.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectComponentToString(collider));
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => "=> " + colliderVariable;
    }
}
