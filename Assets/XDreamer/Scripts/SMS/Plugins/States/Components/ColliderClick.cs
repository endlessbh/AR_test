using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体点击:碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderClick))]
    [Tip("碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。", "Collider click component is the trigger of unity collider click event. When the mouse clicks the collider on one of the game objects, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33600)]
    public class ColliderClick : Trigger<ColliderClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体点击";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderClick))]
        [Tip("碰撞体点击组件是Unity碰撞体点击事件的触发器。当鼠标点击其中一个游戏对象上的碰撞体，组件切换为完成态。", "Collider click component is the trigger of unity collider click event. When the mouse clicks the collider on one of the game objects, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]        
        public GameObject go;

        [Name("点击类型")]
        [EnumPopup]
        public EClickType clickType = EClickType.DownAndUp;

        [Name("点击识别限定距离")]
        [Tip("点击按下和弹起的屏幕坐标差值;在这个限定数值之内认为是点击，否则为拖动", "The difference of screen coordinates between clicking and popping up; Within this limit, it is considered as clicking, otherwise it is dragging")]
        [Range(0, 1000)]
        [HideInSuperInspector(nameof(clickType), EValidityCheckType.NotEqual, EClickType.DownAndUp)]
        public float limitDistanceAsClick = 5;

        [Name("包含子对象")]
        public bool includeChildren = false;

        [Name("点击UI时无效")]
        public bool invalidOnGUI = true;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool addCollider = true;

        /// <summary>
        /// 响应交互命令
        /// </summary>
        [Name("响应交互命令")]
        [Readonly(EEditorMode.Runtime)]
        public bool _responseInteractCmd = true;

        /// <summary>
        /// 交互命令名称
        /// </summary>
        [Name("交互命令名称")]
        public string _interactCmdName = Title;

        [Name("点击配置")]
        [HideInInspector]
        public ColliderClickHandle colliderClickHandle = new ColliderClickHandle();

        private void OnValidate()
        {
            if (addCollider)
            {
                var addCollider = this.addCollider;
                SetColliderClickHandleData();
                colliderClickHandle.Init(go);
                this.addCollider = addCollider;
            }
        }

        public override bool Init(StateData data)
        {
            if (DataValidity())
            {
                SetColliderClickHandleData();
                colliderClickHandle.Init(go);                
            }
            return base.Init(data);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetColliderClickHandleData();
            colliderClickHandle.OnEntry();

            if (_responseInteractCmd)
            {
                InteractObject.onInteractFinished += OnInteractFinished;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            if (_responseInteractCmd)
            {
                InteractObject.onInteractFinished -= OnInteractFinished;
            }
        }

        private void OnInteractFinished(InteractObject interactor, InteractData interactData)
        {
            if (!finished && interactData.cmdName == _interactCmdName && interactData.interactable)
            {
                var collider = interactData.interactable.GetComponent<Collider>();
                if (collider)
                {
                    finished = colliderClickHandle.IsTrigger(collider);
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (finished) return;

            SetColliderClickHandleData();
            finished = colliderClickHandle.IsTrigger();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => go;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => go ? go.name : "";

        private void SetColliderClickHandleData()
        {
            colliderClickHandle.clickType = clickType;
            colliderClickHandle.limitDistanceAsClick = limitDistanceAsClick;
            colliderClickHandle.includeChildren = includeChildren;
            colliderClickHandle.invalidOnGUI = invalidOnGUI;
            colliderClickHandle.addCollider = addCollider;
        }
    }

    /// <summary>
    /// 碰撞体点击处理
    /// </summary>
    [Name("碰撞体点击处理")]
    [Serializable]
    public class ColliderClickHandle
    {
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

        [Name("点击识别限定距离")]
        [Tip("点击按下和弹起的屏幕坐标差值;在这个限定数值之内认为是点击，否则为拖动", "The difference of screen coordinates between clicking and popping up; Within this limit, it is considered as clicking, otherwise it is dragging")]
        [Range(0.1f, 1000)]
        [HideInSuperInspector(nameof(clickType), EValidityCheckType.NotEqual, EClickType.DownAndUp)]
        public float limitDistanceAsClick = 5;

        [Name("包含子对象")]
        public bool includeChildren = false;

        [Name("点击UI时无效")]
        public bool invalidOnGUI = true;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool addCollider = true;

        [Name("鼠标")]
        [EnumPopup]
        public EMouseButton _mouseButton = EMouseButton.Left;

        private float sqrLimitDistanceAsClick = 0;

        private bool isDown = false;

        private List<Collider> colliders = new List<Collider>();

        private List<GameObject> goList = new List<GameObject>();

        public GameObject lastClickGameObject { get; protected set; }

        public void Init(GameObject go) => Init((new GameObject[] { go }).ToList());

        public void Init(List<GameObject> goList)
        {
            this.goList = goList;

            sqrLimitDistanceAsClick = limitDistanceAsClick * limitDistanceAsClick;

            InitCollider();
        }

        public void OnEntry()
        {
            isDown = false;
            lastClickGameObject = null;
        }

        public bool IsTrigger()
        {
            if (XInput.GetMouseButtonDown((int)_mouseButton))
            {
                return OnMouseDown();
            }
            else if (XInput.GetMouseButtonUp((int)_mouseButton))
            {
                return OnMouseUp();
            }
            return false;
        }

        /// <summary>
        /// 输入碰撞体，进行判断是否触发
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public bool IsTrigger(Collider collider)
        {
            if (invalidOnGUI && CommonFun.IsOnUINow()) return false;

            return colliders.Contains(collider);
        }

        private Vector3 mousePositionWhenDown = Vector3.zero;

        private bool OnMouseDown()
        {
            bool trigger = false;
            switch (clickType)
            {
                case EClickType.DownAndUp:
                    {
                        isDown = IsOnCollider();
                        mousePositionWhenDown = XInput.mousePosition;
                        break;
                    }
                case EClickType.Down:
                    {
                        trigger = IsOnCollider();
                        break;
                    }
            }
            return trigger;
        }

        private bool OnMouseUp()
        {
            bool trigger = false;
            switch (clickType)
            {
                case EClickType.DownAndUp:
                    {
                        trigger = isDown && IsOnCollider() && ((XInput.mousePosition - mousePositionWhenDown).sqrMagnitude < sqrLimitDistanceAsClick);
                        break;
                    }
                case EClickType.Up:
                    {
                        trigger = IsOnCollider();
                        break;
                    }
            }
            return trigger;
        }

        private void InitCollider()
        {
            if (goList.Count==0) return;

            goList.ForEach(go=>
            {
                if (go)
                {
                    colliders.AddRange(go.GetComponentsInChildren<Collider>());
                }
            });

            if (colliders.Count == 0 && addCollider)
            {
                goList.ForEach(go =>
                {
                    if (go)
                    {
                        var renderers = go.GetComponentsInChildren<MeshRenderer>();
                        if (renderers.Length > 0)
                        {
                            foreach (var r in renderers)
                            {
                                colliders.Add(r.XAddComponent<MeshCollider>());
                            }
                        }
                        else
                        {
                            colliders.Add(go.XAddComponent<BoxCollider>());
                        }
                    }
                });                
            }
        }

        private bool IsOnCollider()
        {
            if (colliders.Count == 0) return false;

            if (invalidOnGUI && CommonFun.IsOnUINow()) return false;

            var cam = CameraHelperExtension.currentCamera;
            if (!cam)
            {
                Log.Warning("相机缺失!");
                return false;
            }

            if (Physics.Raycast(cam.ScreenPointToRay(XInput.mousePosition), out var hitInfo, _maxDistance, _layerMask))
            {
                lastClickGameObject = hitInfo.collider.gameObject;
                return colliders.Contains(hitInfo.collider);
            }
            return false;
        }
    }

    public enum EMouseButton
    {
        None = -1,

        [Name("左键")]
        Left = 0,

        [Name("右键")]
        Right,

        [Name("中建")]
        Middle,
    }
}
