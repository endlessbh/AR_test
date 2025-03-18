using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.Draggers;

namespace XCSJ.PluginSMS.States.Others
{
    /// <summary>
    /// 放置:放置组件是将游戏对象实时移动到当前视角下鼠标射线与当前场景碰撞体的焦点位置的执行体。当状态没有退出时，会一直执行移动操作。状态退出之后功能失效。
    /// </summary>
    [Serializable]
    [ComponentMenu("其它/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(Put))]
    [Tip("放置组件是将游戏对象实时移动到当前视角下鼠标射线与当前场景碰撞体的焦点位置的执行体。当状态没有退出时，会一直执行移动操作。状态退出之后功能失效。", "The placement component is an actuator that moves the game object to the focus position of the collision between the mouse ray and the current scene in real time. When the status does not exit, the move operation will always be performed. The function fails after the state exits.")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    public class Put : Trigger<Put>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "放置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("其它", typeof(SMSManager), categoryIndex = IndexAttribute.DefaultIndex+1)]
        [StateComponentMenu("其它/" + Title, typeof(SMSManager), categoryIndex = IndexAttribute.DefaultIndex + 1)]
        [Name(Title, nameof(Put))]
        [Tip("放置组件是将游戏对象实时移动到当前视角下鼠标射线与当前场景碰撞体的焦点位置的执行体。当状态没有退出时，会一直执行移动操作。状态退出之后功能失效。", "The placement component is an actuator that moves the game object to the focus position of the collision between the mouse ray and the current scene in real time. When the status does not exit, the move operation will always be performed. The function fails after the state exits.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreatePut(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 源对象配置
        /// </summary>
        [Name("源对象配置")]
        public PutObjectConfig _putObjectConfig = new PutObjectConfig();

        /// <summary>
        /// 放置游戏对象
        /// </summary>
        public GameObject sourceGameObject => _putObjectConfig._gameObject;

        /// <summary>
        /// 光标与点击配置
        /// </summary>
        [Name("光标配置")]
        public ClickPutObjectConfig _clickPutObjectConfig = new ClickPutObjectConfig();

        #region 射线配置

        /// <summary>
        /// 射线拾取对象面中心层
        /// </summary>
        [Name("射线拾取对象面中心层")]
        [Tip("射线拾取对象如果属于当前层，则碰撞点会被换算为碰撞面的中心", "If the ray pick object belongs to the current layer, the collision point is converted to the center of the collision surface")]
        public LayerMask _hitObjectFaceCenterLayer = 0;

        /// <summary>
        /// 放置偏移量
        /// </summary>
        [Name("放置偏移量")]
        public Vector3 _putOffset = Vector3.zero;

        /// <summary>
        /// 旋转参考为当前相机
        /// </summary>
        [Name("旋转参考为当前相机")]
        public bool _alignCurrentCamera = true; 

        /// <summary>
        /// 旋转规则
        /// </summary>
        [Name("旋转规则")]
        [EnumPopup]
        public ERotationRule _putRotationRule = ERotationRule.None;

        /// <summary>
        /// 旋转参考游戏对象
        /// </summary>
        [Name("旋转参考游戏对象")]
        [HideInSuperInspector(nameof(_alignCurrentCamera), EValidityCheckType.NotEqual, false)]
        public GameObject _alignGameObject = null;

        /// <summary>
        /// 面对相机时的偏转角
        /// </summary>
        [Name("旋转偏转角")]
        [HideInSuperInspector(nameof(_putRotationRule), EValidityCheckType.Equal, ERotationRule.None)]
        [Range(0, 360)]
        public float _offsetYAngle = 0;

        /// <summary>
        /// 射线距离
        /// </summary>
        [Name("射线距离")]
        [Min(0)]
        public float _maxDistance = 1000;

        #endregion

        /// <summary>
        /// 有效点击识别限定距离
        /// </summary>
        [Group("点击配置")]
        [Name("有效点击识别限定距离")]
        [Tip("点击按下和弹起的屏幕坐标差值;在这个限定数值之内认为是点击，否则为拖动", "The difference of screen coordinates between clicking and popping up; Within this limit, it is considered as clicking, otherwise it is dragging")]
        [Range(0.1f, 1000)]
        public float _limitDistanceAsClick = 5;

        /// <summary>
        /// 有效点击识别限定距离的平方
        /// </summary>
        public float sqrLimitDistanceAsClick => _limitDistanceAsClick * _limitDistanceAsClick;

        /// <summary>
        /// 克隆规则
        /// </summary>
        [Name("克隆规则")]
        [EnumPopup]
        public ECloneRule _cloneRule = ECloneRule.Clone_KeepParent_ActiveOnExit;

        #region 状态生命周期函数

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            _putObjectConfig.OnEntry();
            _clickPutObjectConfig.OnEntry();

            StartPut();
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            Puting();

            _clickPutObjectConfig.OnUpdate(rayValid);
        }

        public override void OnExit(StateData data)
        {
            StopPut();

            _clickPutObjectConfig.OnExit();
            _putObjectConfig.OnExit();

            base.OnExit(data);
        }

        public override bool DataValidity() => sourceGameObject;

        public override string ToFriendlyString() => sourceGameObject ? sourceGameObject.name : "";

        #endregion

        #region 射线放置处理

        private RaycastHit hitInfo;

        private bool isHit = false;

        private Transform lastHitTransform
        {
            get => _lastHitTransform;
            set
            {
                if (_lastHitTransform != value)
                {
                    _lastHitTransform = value;
                    if (_lastHitTransform)
                    {
                        CommonFun.GetBounds(out _lastHitTransformBounds, _lastHitTransform, true, false, false);
                    }
                }
            }
        }

        private Transform _lastHitTransform;

        private Bounds _lastHitTransformBounds;

        private Transform _mouseDownTransform = null;

        private Vector3 mousePositionWhenDown = Vector3.zero;

        private Vector3 _lastMousePosition = Vector3.zero;

        private bool rayValid = false;

        private void StartPut()
        {
            _lastMousePosition = XInput.mousePosition;
        }

        private void StopPut()
        {
            if (sourceGameObject && isHit)
            {
                PutGameObject(GetPutGameObject(sourceGameObject));
            }
        }

        private void Puting()
        {
            if (!sourceGameObject || !cam || CommonFun.IsOnAnyUI())
            {
                rayValid = false;
                return;
            }

            if (_lastMousePosition != XInput.mousePosition)
            {
                _lastMousePosition = XInput.mousePosition;
                isHit = Physics.Raycast(cam.ScreenPointToRay(XInput.mousePosition), out hitInfo, _maxDistance);
                lastHitTransform = hitInfo.transform;
            }
            rayValid = isHit;

            // 移动光标
            if (isHit)
            {
                PutGameObject(_clickPutObjectConfig._gameObject);
            }

            HandleClick();
        }

        private void HandleClick()
        {
            if (XInput.GetMouseButtonDown(0))
            {
                _mouseDownTransform = lastHitTransform;
                mousePositionWhenDown = XInput.mousePosition;
            }
            if (XInput.GetMouseButtonUp(0))
            {
                if (_mouseDownTransform == lastHitTransform
                    && (XInput.mousePosition - mousePositionWhenDown).sqrMagnitude < sqrLimitDistanceAsClick)
                {
                    finished = true;
                }
            }
        }

        private void PutGameObject(GameObject go)
        {
            if (!go) return;

            var clickPoint = hitInfo.point;
            // 检测射线拾取游戏对象是否是要拾取碰撞对象面中心
            if ((_hitObjectFaceCenterLayer & (1 << hitInfo.transform.gameObject.layer)) > 0)
            {
                clickPoint = PutHelper.GetRayHitObjectFaceCenter(hitInfo, _lastHitTransformBounds);
            }

            // 偏移量
            var offset = _clickPutObjectConfig.valid ? _clickPutObjectConfig.GetOffset(hitInfo) : _putObjectConfig.GetOffset(hitInfo);
            go.transform.position = clickPoint + offset + _putOffset;

            // 旋转规则
            HandleRotation(go);
        }

        private void HandleRotation(GameObject go)
        {
            var referenceObj = _alignCurrentCamera ? camGameObject : _alignGameObject;
            if (!referenceObj) return;

            go.transform.Rotate(referenceObj.transform, _putRotationRule);

            go.transform.RotateAround(go.transform.position, go.transform.up, _offsetYAngle);
        }

        private Camera cam => CameraHelperExtension.currentCamera;
        private GameObject camGameObject => cam ? cam.gameObject : null;

        public GameObject GetPutGameObject(GameObject go)
        {
            GameObject putGo = null;
            switch (_cloneRule)
            {
                case ECloneRule.None: putGo = go; break;
                case ECloneRule.Clone_ActiveOnExit:
                    {
                        putGo = GameObject.Instantiate(go);
                        if (putGo)
                        {
                            putGo.SetActive(true);
                        }
                        break;
                    }
                case ECloneRule.Clone_KeepParent_ActiveOnExit:
                    {
                        putGo = GameObject.Instantiate(go, go.transform.parent);
                        if (putGo)
                        {
                            putGo.SetActive(true);
                        }
                        break;
                    }
            }
            return putGo;
        }

        #endregion

        /// <summary>
        /// 点击放置对象配置
        /// </summary>
        [Name("点击放置对象配置")]
        [Serializable]
        public class ClickPutObjectConfig : PutObjectConfig
        {
            /// <summary>
            /// 更新时激活规则
            /// </summary>
            [Name("更新时激活规则")]
            [EnumPopup]
            public EActiveOnUpdateRule _activeOnUpdateRule = EActiveOnUpdateRule.SynActiveWithRayValidOnUpdate;

            public virtual void OnUpdate(bool rayValid)
            {
                switch (_activeOnUpdateRule)
                {
                    case EActiveOnUpdateRule.SynActiveWithRayValidOnUpdate:
                        {
                            if(_gameObject) _gameObject.SetActive(rayValid);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 更新时激活规则
        /// </summary>
        [Name("更新时激活规则")]
        public enum EActiveOnUpdateRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 更新时激活属性与射线有效性同步
            /// </summary>
            [Name("更新时与射线有效性同步")]
            SynActiveWithRayValidOnUpdate,
        }

        /// <summary>
        /// 克隆规则
        /// </summary>
        [Name("克隆规则")]
        public enum ECloneRule
        {
            [Name("无")]
            None,

            [Name("克隆退出激活")]
            Clone_ActiveOnExit,

            [Name("克隆保持父级退出激活")]
            Clone_KeepParent_ActiveOnExit,
        }

        [Name("放置对象配置")]
        [Serializable]
        public class PutObjectConfig
        {
            [Name("游戏对象")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            [Readonly(EEditorMode.Runtime)]
            public GameObject _gameObject = null;

            /// <summary>
            /// 游戏对象激活规则
            /// </summary>
            [Name("激活规则")]
            [EnumPopup]
            public EActiveRule _activeRule = EActiveRule.DisactiveOnEntry_RecoverOnExit;

            /// <summary>
            /// 碰撞对象激活规则
            /// </summary>
            [Name("碰撞体规则")]
            [EnumPopup]
            public EColliderRule _coliiderRule = EColliderRule.None;

            /// <summary>
            /// 位置规则
            /// </summary>
            [Name("位置规则")]
            [EnumPopup]
            public EPutRule _positionRule = EPutRule.BoundsBottomCenter;

            /// <summary>
            /// 包围盒规则
            /// </summary>
            [Name("包围盒规则")]
            [EnumPopup]
            public EBoundsRule _boundsRule = EBoundsRule.SelfAndChildren;

            /// <summary>
            /// 包围盒
            /// </summary>
            [Name("包围盒")]
            [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.Bounds)]
            public Bounds _bounds = new Bounds();

            public bool valid => _gameObject;

            private bool activeStateBeforeEntry = false;

            private Bounds? _goBounds;

            /// <summary>
            /// 包围盒中心与变换中心的偏移量
            /// </summary>
            private Vector3 boundsOffset = Vector3.zero;

            /// <summary>
            /// 包围盒尺寸的一半
            /// </summary>
            private float halfOfboundsMagnitude = 0;

            private ColliderRecorder colliderRecorder = new ColliderRecorder();

            /// <summary>
            /// 进入回调
            /// </summary>
            public virtual void OnEntry()
            {
                if (!_gameObject) return;

                // 计算包围盒
                switch (_boundsRule)
                {
                    case EBoundsRule.Self:
                        {
                            if (CommonFun.GetBounds(out var b, _gameObject, false, false, false))
                            {
                                _goBounds = b;
                            }
                            break;
                        }
                    case EBoundsRule.SelfAndChildren:
                        {
                            if (CommonFun.GetBounds(out var b, _gameObject, true, false, false))
                            {
                                _goBounds = b;
                            }
                            break;
                        }
                    case EBoundsRule.Bounds:
                        {
                            _goBounds = _bounds;
                            break;
                        }
                }
                if (_goBounds.HasValue)
                {
                    switch (_positionRule)
                    {
                        case EPutRule.None:
                        case EPutRule.Transform:
                            {
                                boundsOffset = Vector3.zero;
                                break;
                            }
                        case EPutRule.BoundsCenter:
                        case EPutRule.BoundsBottomCenter:
                        case EPutRule.BoundsTangent:
                            {
                                // 游戏对象本地位置与其包围盒的偏移值
                                boundsOffset = _gameObject.transform.position - _goBounds.Value.center; 
                                break;
                            }
                        case EPutRule.SpereBoundsTangentPoint:
                            {
                                // 游戏对象本地位置与其包围盒的偏移值
                                boundsOffset = _gameObject.transform.position - _goBounds.Value.center;
                                halfOfboundsMagnitude = _goBounds.Value.size.magnitude / 2;
                                break;
                            }
                    }
                }

                // 设置碰撞体
                colliderRecorder.Clear();
                colliderRecorder.Record(_gameObject.GetComponentsInChildren<Collider>());
                switch (_coliiderRule)
                {
                    case EColliderRule.DisableOnEntry_RecoverOnExit:
                        {
                            colliderRecorder._records.ForEach(r => r.collider.enabled = false);
                            break;
                        }
                    case EColliderRule.EnableOnEntry_RecoverOnExit:
                        {
                            colliderRecorder._records.ForEach(r => r.collider.enabled = true);
                            break;
                        }
                }

                // 设置游戏对象激活
                activeStateBeforeEntry = _gameObject.activeSelf;
                switch (_activeRule)
                {
                    case EActiveRule.DisactiveOnEntry_ActiveOnExit: _gameObject.SetActive(false); break;
                    case EActiveRule.ActiveOnEntry_RecoverOnExit:
                    case EActiveRule.ActiveOnEntry: _gameObject.SetActive(true); break;
                }
            }

            /// <summary>
            /// 退出回调
            /// </summary>
            public virtual void OnExit()
            {
                if (!_gameObject) return;

                // 对象碰撞体属性恢复
                switch (_coliiderRule)
                {
                    case EColliderRule.DisableOnEntry_RecoverOnExit:
                    case EColliderRule.EnableOnEntry_RecoverOnExit:
                        {
                            colliderRecorder.Recover();
                            break;
                        }
                }

                // 设置游戏对象激活属性
                switch (_activeRule)
                {
                    case EActiveRule.DisactiveOnEntry_ActiveOnExit:
                    case EActiveRule.ActiveOnExit:
                        {
                            _gameObject.SetActive(true);
                            break;
                        }
                    case EActiveRule.DisactiveOnEntry_RecoverOnExit:
                    case EActiveRule.ActiveOnEntry_RecoverOnExit:
                        {
                            _gameObject.SetActive(activeStateBeforeEntry);
                            break;
                        }
                }
            }

            public Vector3 GetOffset(RaycastHit hitInfo)
            {
                switch (_positionRule)
                {
                    case EPutRule.BoundsBottomCenter:
                        {
                            return boundsOffset + (_goBounds.HasValue ? (Vector3.up * _goBounds.Value.size.y / 2) : Vector3.zero);
                        }
                    case EPutRule.BoundsTangent:
                        {
                            return boundsOffset + (_goBounds.HasValue ? _gameObject.transform.GetBoundsTangentOffset(_goBounds.Value, hitInfo.normal) : Vector3.zero);
                        }
                    case EPutRule.SpereBoundsTangentPoint:
                        {
                            return halfOfboundsMagnitude * hitInfo.normal;
                        }
                }
                return boundsOffset;
            }
        }

        /// <summary>
        /// 激活规则
        /// </summary>
        [Name("激活规则")]
        public enum EActiveRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 进入时非激活退出激活
            /// </summary>
            [Name("进入非激活退出激活")]
            DisactiveOnEntry_ActiveOnExit,

            /// <summary>
            /// 进入时非激活退出激活
            /// </summary>
            [Name("进入非激活退出恢复")]
            DisactiveOnEntry_RecoverOnExit,

            /// <summary>
            /// 进入激活
            /// </summary>
            [Name("进入激活")]
            ActiveOnEntry,

            /// <summary>
            /// 进入激活
            /// </summary>
            [Name("进入激活退出恢复")]
            ActiveOnEntry_RecoverOnExit,

            /// <summary>
            /// 退出激活
            /// </summary>
            [Name("退出激活")]
            ActiveOnExit,
        }

        /// <summary>
        /// 碰撞体规则
        /// </summary>
        [Name("碰撞体规则")]
        public enum EColliderRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 进入时禁用退出时恢复
            /// </summary>
            [Name("进入时禁用退出时恢复")]
            DisableOnEntry_RecoverOnExit,

            /// <summary>
            /// 进入时启用退出时恢复
            /// </summary>
            [Name("进入时启用退出时恢复")]
            EnableOnEntry_RecoverOnExit,
        }
    }
}