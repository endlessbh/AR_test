using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.Scripts;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.PluginTools.Notes.Tips
{
    /// <summary>
    /// 提示:
    /// 1、为UI或三维模型对象增加提示或批注
    /// 2、提示信息可使用本组件上的文本信息、提示游戏对象名称和提示游戏对象上的可交互对象属性组件中的属性值
    /// 3、提示的表现层分为UI文本、批注两种类型
    /// </summary>
    [Name("提示")]
    [Tip("1、为UI或三维模型对象增加提示或批注\n2、提示信息可使用本组件上的文本信息、提示游戏对象名称和提示游戏对象上的可交互对象属性组件中的属性值\n3、提示的表现层分为UI文本、批注两种类型")]
    [Tool("标注", rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Tip)]
    [DisallowMultipleComponent]
    public sealed class Tip : BaseEffect
    {
        /// <summary>
        /// 触发方式
        /// </summary>
        [Name("触发方式")]
        [EnumPopup]
        public ETipTriggerType _tipTriggerType = ETipTriggerType.Effect;

        #region 提示信息

        /// <summary>
        /// 提示关键字
        /// </summary>
        [PropertyKey] 
        public const string TipTag = "提示标签";

        private string GetTipText(GameObject go)
        {
            var text = GetInteractPropertyValueByRule(go)?.value;
            if (string.IsNullOrEmpty(text))
            {
                return go ? go.name : name;
            }
            return text;
        }

        #endregion

        #region 显示对象

        public enum ETipAssetType
        {
            [Name("UI")]
            UI,

            [Name("批注")]
            Note,
        }

        [Group("提示资产", textEN = "Tip Assets")]

        /// <summary>
        /// 提示资产类型
        /// </summary>
        [Name("提示资产类型")]
        [EnumPopup]
        public ETipAssetType _assetType = ETipAssetType.UI;

        [Name("延迟显示时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.UI)]
        public float _delayShowTime = 0.6f;

        [Name("延迟隐藏时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.UI)]
        public float _delayHideTime = 0.1f;

        /// <summary>
        /// 提示弹出框资产
        /// </summary>
        [Name("提示弹出框资产")]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.UI)]
        public TipPopupAsset _tipPopupAsset;

        /// <summary>
        /// 提示弹出框
        /// </summary>
        public TipPopup tipPopup => _tipPopupAsset.view;

        /// <summary>
        /// 弹出位置
        /// </summary>
        [Name("弹出位置")]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.UI)]
        public PopupPosition _popupPosition = new PopupPosition();

        /// <summary>
        /// 标注
        /// </summary>
        [Name("批注")]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.Note)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _note;

        /// <summary>
        /// 批注位置设置
        /// </summary>
        [Name("批注位置设置")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_assetType), EValidityCheckType.NotEqual, ETipAssetType.Note)]
        public ENotePositionRule _notePositionRule = ENotePositionRule.BoundsTop;

        public enum ENotePositionRule
        {
            [Name("变换位置")]
            TransformPosition,

            [Name("碰撞点")]
            RayHitPoint,

            [Name("包围盒顶部")]
            BoundsTop,
        }

        /// <summary>
        /// UI位置
        /// </summary>
        public Vector3 position => _assetType == ETipAssetType.Note ? targetTransform.position : _popupPosition.GetPosition(targetTransform);

        /// <summary>
        /// UI方位
        /// </summary>
        public Vector2 direction => _popupPosition.GetDirection(_popupPosition.GetOrgPosition(targetTransform));

        private GameObject target;

        private Transform targetTransform => target ? target.transform : transform;

        #endregion

        #region Unity生命周期事件

        private RectTransform _rectTransform = null;

        private EventTrigger eventTriggerOnEnable = null;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => _tagProperty._tagPropertyDatas.Add(new TagPropertyData(TipTag));

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //HideTip(_tipTriggerType);

            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform)
            {
                eventTriggerOnEnable = _rectTransform.XAddComponent<EventTrigger>();

                eventTriggerOnEnable.AddEventTrigger(EventTriggerType.PointerEnter, OnPointerEnter);
                eventTriggerOnEnable.AddEventTrigger(EventTriggerType.PointerExit, OnPointerExit);
                eventTriggerOnEnable.AddEventTrigger(EventTriggerType.PointerClick, OnPointerClick);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (eventTriggerOnEnable)
            {
                Destroy(eventTriggerOnEnable);
            }
        }

        private void Update()
        {
            if (isClickOnObject)
            {
                isClickOnObject = false;
                return;
            }
            if (Input.GetMouseButtonUp(0) && !CommonFun.IsOnUGUI())
            {
                HideTip(ETipTriggerType.Click);
            }
        }

        #endregion

        #region UI 悬停与点击

        private void OnPointerEnter(BaseEventData baseEventData) => ShowTip(ETipTriggerType.Hover, GetTipText(gameObject));

        private void OnPointerExit(BaseEventData baseEventData) => HideTip(ETipTriggerType.Hover);

        private void OnPointerClick(BaseEventData baseEventData) => ShowTip(ETipTriggerType.Click, GetTipText(gameObject));

        #endregion

        #region 碰撞体悬停与点击

        private void OnMouseEnter() => ShowTip(ETipTriggerType.Hover, GetTipText(gameObject));

        private void OnMouseExit() => HideTip(ETipTriggerType.Hover);

        private void OnMouseUpAsButton()
        {
            if (!CommonFun.IsOnUGUI())
            {
                isClickOnObject = true;
                ShowTip(ETipTriggerType.Click, GetTipText(gameObject));
            }
        }

        private bool isClickOnObject = false;

        #endregion

        #region 特效启用禁用

        /// <summary>
        /// 启用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            if (!gameObject) return;
            target = gameObject;
            ShowTip(ETipTriggerType.Effect, GetTipText(gameObject), interactData);
        }

        /// <summary>
        /// 特效工作中
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EffectWorking(InteractData interactData, GameObject gameObject)
        {
            if (!gameObject) return;
            base.EffectWorking(interactData, gameObject);

            switch (_assetType)
            {
                case ETipAssetType.UI:
                    {
                        tipPopup.UpdatePosition();
                        break;
                    }
                case ETipAssetType.Note:
                    {
                        UpdateNotePosition(interactData);
                        break;
                    }
            }
        }

        /// <summary>
        /// 禁用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
            target = null;
            HideTip(ETipTriggerType.Effect);
        }

        #endregion

        #region 显示提示

        private void ShowTip(ETipTriggerType tipTriggerType, string tipText = "", InteractData interactData = null)
        {
            if (_tipTriggerType != tipTriggerType) return;

            switch (_assetType)
            {
                case ETipAssetType.UI:
                    {
                        if (tipPopup)
                        {
                            tipPopup.DelayShow(this, _delayShowTime, tipText, position, direction);
                        }
                        break;
                    }
                case ETipAssetType.Note:
                    {
                        if (_note)
                        {
                            UpdateNotePosition(interactData);

                            var text = _note.GetComponentInChildren<INoteText>(true);
                            if (text != null)
                            {
                                text.noteText = tipText;
                            }
                            _note.SetActive(true);
                        }
                        break;
                    }
            }
        }

        private void UpdateNotePosition(InteractData interactData)
        {
            switch (_notePositionRule)
            {
                case ENotePositionRule.TransformPosition:
                    {
                        _note.transform.position = position;
                        break;
                    }
                case ENotePositionRule.RayHitPoint:
                    {
                        if (interactData.parent is RayInteractData rayInteractData && rayInteractData.raycastHit.HasValue)
                        {
                            _note.transform.position = rayInteractData.raycastHit.Value.point;
                        }
                        break;
                    }
                case ENotePositionRule.BoundsTop:
                    {
                        if (CommonFun.GetBounds(out var bounds, target))
                        {
                            _note.transform.position = bounds.center + new Vector3(0, bounds.extents.y, 0);
                        }
                        break;
                    }
            }
        }

        private void HideTip(ETipTriggerType tipTriggerType, InteractData interactData = null)
        {
            if (_tipTriggerType != tipTriggerType) return;

            switch (_assetType)
            {
                case ETipAssetType.UI:
                    {
                        if (tipPopup)
                        {
                            tipPopup.Hide(this, _delayHideTime);
                        }
                        break;
                    }
                case ETipAssetType.Note:
                    {
                        if (_note)
                        {
                            var text = _note.GetComponentInChildren<INoteText>(true);
                            if (text != null)
                            {
                                text.noteText = "";
                            }
                            _note.SetActive(false);
                        }
                        break;
                    }
            }
        }

        #endregion
    }

    /// <summary>
    /// 弹出位置计算器
    /// </summary>
    [Serializable]
    public class PopupPosition
    {
        /// <summary>
        /// 位置类型
        /// </summary>
        [Name("位置类型")]
        [EnumPopup]
        public EPositionType _positionRule = EPositionType.Transform;

        public enum EPositionType
        {
            [Name("无")]
            None,

            [Name("变换")]
            Transform,

            [Name("点击位置")]
            HitPoint,
        }

        /// <summary>
        /// 方位类型
        /// </summary>
        [Name("方位类型")]
        [EnumPopup]
        public EDirectionType _directionType = EDirectionType.TowardScreenCenter;

        public enum EDirectionType
        {
            [Name("朝向屏幕中心")]
            TowardScreenCenter,

            [Name("方位")]
            Direction,

            [Name("自定义")]
            Custom,
        }

        [Name("方位")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_directionType), EValidityCheckType.NotEqual, EDirectionType.Direction)]
        public EFourDirection _direction = EFourDirection.Bottom;

        [Name("方位偏移量")]
        public Vector2 _positionOffset = new Vector2(30, 30);// XGUI 大部分按钮宽高默认值

        /// <summary>
        /// 基于方位偏移计算出位置
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public Vector3 GetPosition(Transform transform) => GetPositionWithDirectionOffset(GetOrgPosition(transform));

        private Vector3 GetPositionWithDirectionOffset(Vector3 position) => position + Vector3.Scale(GetDirection(position), _positionOffset);

        /// <summary>
        /// 获取原始位置
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public Vector3 GetOrgPosition(Transform transform)
        {
            switch (_positionRule)
            {
                case EPositionType.Transform: 
                    {
                        if (transform is RectTransform rectTransform)
                        {
                            return rectTransform.position;
                        }
                        else
                        {
                            var cam = CameraHelperExtension.currentCamera;
                            if (cam)
                            {
                                return cam.WorldToScreenPoint(transform.position);
                            }
                        }
                        break;
                    }
                case EPositionType.HitPoint: return Input.mousePosition;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 通过交互数据获取位置
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TryGetPosition(InteractData interactData, out Vector2 position)
        {
            if (interactData != null)
            {
                switch (_positionRule)
                {
                    case EPositionType.Transform:
                        {
                            if (interactData.interactable && CommonFun.GetBounds(out var bounds, interactData.interactable.transform))
                            {
                                if (CameraHelperExtension.TryConvertWorldToScreenPoint(bounds.center, out var screenPoint))
                                {
                                    position = GetPositionWithDirectionOffset(screenPoint);
                                    return true;
                                }
                            }
                            break;
                        }
                    case EPositionType.HitPoint:
                        {
                            if (interactData is RayInteractData rayData)
                            {
                                if (rayData.raycastHit.HasValue)
                                {
                                    if (CameraHelperExtension.TryConvertWorldToScreenPoint(rayData.raycastHit.Value.point, out var screenPoint))
                                    {
                                        position = GetPositionWithDirectionOffset(screenPoint);
                                        return true;
                                    }
                                }
                                else if (rayData is MouseRayInteractData mouseRayInteractData)
                                {
                                    position = GetPositionWithDirectionOffset(mouseRayInteractData.mousePosition);
                                    return true;
                                }
                            }
                            break;
                        }
                }
            }
            position = default;
            return false;
        }

        public Vector2 GetDirection(Vector2 orgPosition)
        {
            switch (_directionType)
            {
                case EDirectionType.TowardScreenCenter:
                    {
                        var k = Screen.height * 1.0f / Screen.width;
                        var tmp = orgPosition - (new Vector2(Screen.width, Screen.height)) / 2;
                        // 上半区
                        if (tmp.y >= 0)
                        {
                            if (tmp.x == 0)
                            {
                                return new Vector2(0, -1);
                            }
                            else
                            {
                                var k2 = tmp.y / tmp.x;
                                if (Mathf.Abs(k2) >= k)
                                {
                                    return new Vector2(0, -1);
                                }
                                else
                                {
                                    if (k2 > 0)
                                    {
                                        return new Vector2(-1, 0);
                                    }
                                    else
                                    {
                                        return new Vector2(1, 0);
                                    }
                                }
                            }
                        }
                        else // 下半区
                        {
                            if (tmp.x == 0)
                            {
                                return new Vector2(0, 1);
                            }
                            else
                            {
                                var k2 = tmp.y / tmp.x;
                                if (Mathf.Abs(k2) >= k)
                                {
                                    return new Vector2(0, 1);
                                }
                                else
                                {
                                    if (k2 > 0)
                                    {
                                        return new Vector2(1, 0);
                                    }
                                    else
                                    {
                                        return new Vector2(-1, 0);
                                    }
                                }
                            }
                        }
                    }
                case EDirectionType.Direction:
                    {
                        switch (_direction)
                        {
                            case EFourDirection.None: return Vector2.zero;
                            case EFourDirection.Left: return new Vector2(-1, 0);
                            case EFourDirection.Right: return new Vector2(1, 0);
                            case EFourDirection.Top: return new Vector2(0, 1);
                            case EFourDirection.Bottom: return new Vector2(0, -1);
                        }
                        break;
                    }
                case EDirectionType.Custom: return _positionOffset.normalized;
            }
            return Vector2.zero;
        }
    }

    /// <summary>
    /// 提示触发类型
    /// </summary>
    [Name("提示触发类型")]
    public enum ETipTriggerType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 悬停
        /// </summary>
        [Name("悬停")]
        Hover,

        /// <summary>
        /// 点击
        /// </summary>
        [Name("点击")]
        [Tip("点击模型弹出提示，再次点击在非当前对象上则消失", "Click the model to pop up a prompt, and click again on a non current object to disappear")]
        Click,

        /// <summary>
        /// 作为特效调用
        /// </summary>
        [Name("特效")]
        Effect,
    }
}
