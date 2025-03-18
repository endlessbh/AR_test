using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Inputs;
using System.Collections.Generic;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过鼠标:默认通过鼠标在屏幕窗口中位置控制相机的移动
    /// </summary>
    [Name("相机移动通过鼠标")]
    [Tip("默认通过鼠标在屏幕窗口中位置控制相机的移动", "By default, the movement of the camera is controlled by the position of the mouse in the screen window")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByMouse : BaseCameraMoveController
    {
        /// <summary>
        /// 移动触发规则
        /// </summary>
        public enum EMoveTriggerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 靠近屏幕边框：当鼠标靠近屏幕边框时，触发移动逻辑
            /// </summary>
            [Name("靠近屏幕边框")]
            [Tip("当鼠标靠近屏幕边框时，触发移动逻辑", "When the mouse is close to the screen border, the movement logic is triggered")]
            CloseToScreenBorder,
        }

        /// <summary>
        /// 移动触发规则
        /// </summary>
        [Name("移动触发规则")]
        [EnumPopup]
        public EMoveTriggerRule _moveTriggerRule = EMoveTriggerRule.CloseToScreenBorder;

        /// <summary>
        /// 移动方向类型
        /// </summary>
        [Name("移动方向类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        public EMoveDirType _moveDirType = EMoveDirType.Dir8;

        /// <summary>
        /// 移动方向隐射规则
        /// </summary>
        [Name("移动方向隐射规则")]
        public enum EMoveDirProjectRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 垂直到前投影_水平到右投影：将相机前方向投影到世界X0Z平面得到前投影，使用前投影与世界上方向向量（Y）基于左手坐标系规则得到右投影；本隐射方式类似于Dota、红警等类型游戏的相机移动控制方式
            /// </summary>
            [Name("垂直到前投影_水平到右投影")]
            [Tip("将相机前方向投影到世界X0Z平面得到前投影，使用前投影与世界上方向向量（Y）基于左手坐标系规则得到右投影；本隐射方式类似于Dota、红警等类型游戏的相机移动控制方式", "Project the front direction of the camera to the world x0z plane to obtain the front projection, and use the front projection and the world up direction vector (y) to obtain the right projection based on the rules of the left-handed coordinate system; This stealth mode is similar to the camera movement control mode of DOTA, red alert and other types of games")]
            VerticalToForwardProjection_HorizontalToRightProjection,

            /// <summary>
            /// 垂直到Z_水平到X
            /// </summary>
            [Name("垂直到Z_水平到X")]
            VerticalToZ_HorizontalToX,

            /// <summary>
            /// 垂直到Y_水平到X
            /// </summary>
            [Name("垂直到Y_水平到X")]
            VerticalToY_HorizontalToX,
        }

        /// <summary>
        /// 移动方向隐射规则
        /// </summary>
        [Name("移动方向隐射规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        public EMoveDirProjectRule _moveDirProjectRule = EMoveDirProjectRule.VerticalToForwardProjection_HorizontalToRightProjection;

        /// <summary>
        /// 内边框宽度
        /// </summary>
        [Name("内边框宽度")]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        [Range(5, 100)]
        public float _innerBorderWidth = 20;

        /// <summary>
        /// 外边框宽度
        /// </summary>
        [Name("外边框宽度")]
        [HideInSuperInspector(nameof(_moveTriggerRule), EValidityCheckType.NotEqual, EMoveTriggerRule.CloseToScreenBorder)]
        [Range(5, 1000)]
        public float _outerBorderWidth = 20;

        /// <summary>
        /// 隐射移动
        /// </summary>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量，Y标识垂直（Vertical）的移动量，Z恒定为0</param>
        private void ProjectMove(Vector3 moveValue)
        {
            switch (_moveDirProjectRule)
            {
                case EMoveDirProjectRule.VerticalToForwardProjection_HorizontalToRightProjection:
                    {
                        cameraTransformer.ProjectOnPlane(Vector3.up, out var forward, out var right);

                        var dir = forward.normalized * moveValue.y + right.normalized * moveValue.x;
                        _offset = Vector3.Scale(speedRealtime, dir.normalized);
                        Move();
                        break;
                    }
                case EMoveDirProjectRule.VerticalToZ_HorizontalToX:
                    {
                        _offset = Vector3.Scale(speedRealtime, new Vector3(moveValue.x, 0, moveValue.y));
                        Move();
                        break;
                    }
                case EMoveDirProjectRule.VerticalToY_HorizontalToX:
                    {
                        _offset = Vector3.Scale(speedRealtime, new Vector3(moveValue.x, moveValue.y, 0));
                        Move();
                        break;
                    }
            }

        }

        private void HandleCloseToScreenBorder()
        {
            var width = Screen.width;
            var height = Screen.height;
            var mousePosition = Input.mousePosition;

            if (mousePosition.x < -_outerBorderWidth || mousePosition.x > width + _outerBorderWidth || mousePosition.y < -_outerBorderWidth || mousePosition.y > height + _outerBorderWidth)
            {
                //在外边框之外,不处理
                return;
            }

            switch (_moveDirType)
            {
                case EMoveDirType.Dir4:
                    {
                        if (HasMove4(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
                case EMoveDirType.Dir8:
                    {
                        if (HasMove8(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
                case EMoveDirType.DirAny:
                    {
                        if (HasMoveAny(width, height, mousePosition, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, _innerBorderWidth, out Vector3 moveValue))
                        {
                            ProjectMove(moveValue);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 鼠标按钮处理器
        /// </summary>
        [Name("鼠标按钮处理器")]
        public MouseButtonHandler _mouseButtonHandler = new MouseButtonHandler();

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();

            switch (_moveTriggerRule)
            {
                case EMoveTriggerRule.CloseToScreenBorder:
                    {
                        if (!_mouseButtonHandler.CanContinue(UpdateCache)) return;
                        HandleCloseToScreenBorder();
                        break;
                    }
            }
        }

        private void UpdateCache() { }

        /// <summary>
        /// 绘制边框:仅在编辑器中生效
        /// </summary>
        [Name("绘制边框")]
        [Tip("仅在编辑器中生效", "Effective only in the editor")]
        public bool _drawBorder = true;

#if UNITY_EDITOR

        private void OnGUI()
        {
            if (!_drawBorder) return;
            switch (_moveTriggerRule)
            {
                case EMoveTriggerRule.CloseToScreenBorder:
                    {
                        var width = Screen.width;
                        var height = Screen.height;
                        GUI.Box(new Rect(0, 0, width, _innerBorderWidth), GUIContent.none);//上
                        GUI.Box(new Rect(0, 0, _innerBorderWidth, height), GUIContent.none);//左
                        GUI.Box(new Rect(0, height - _innerBorderWidth, width, _innerBorderWidth), GUIContent.none);//下
                        GUI.Box(new Rect(width - _innerBorderWidth, 0, _innerBorderWidth, height), GUIContent.none);//右
                        break;
                    }
            }
        }

#endif

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _mouseButtonHandler._mouseButtons.Add(EMouseButton.Any);
            _mouseButtonHandler._ruleOnAnyMouseButton = MouseButtonHandler.ERule.Return;
            _mouseButtonHandler._input = EInput.StandaloneInput;
        }

        /// <summary>
        /// 移动方向类型
        /// </summary>
        [Name("移动方向类型")]
        public enum EMoveDirType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 4方向
            /// </summary>
            [Name("4方向")]
            Dir4,

            /// <summary>
            /// 8方向
            /// </summary>
            [Name("8方向")]
            Dir8,

            /// <summary>
            /// 任意方向
            /// </summary>
            [Name("任意方向")]
            DirAny,
        }

        /// <summary>
        /// 是否有任意方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMoveAny(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;
            var dir = new Vector3(x, y) - new Vector3(width / 2, height / 2);
            if (x > left && x < width - right && y > bottom && y < height - top)
            {
                return false;
            }
            moveValue = dir.normalized;
            return true;
        }

        /// <summary>
        /// 是否有4方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMove4(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;

            if (x <= left)//147
            {
                if (y <= bottom)//7
                {
                    if (Mathf.Abs(x - left) >= Mathf.Abs(y - bottom))//7->4
                    {
                        moveValue = new Vector3(-1, 0);
                    }
                    else//7->8
                    {
                        moveValue = new Vector3(0, -1);
                    }
                }
                else if (y >= height - top)//1
                {
                    if (Mathf.Abs(x - left) >= Mathf.Abs(y - height + top))//1->4
                    {
                        moveValue = new Vector3(-1, 0);
                    }
                    else//1->2
                    {
                        moveValue = new Vector3(0, 1);
                    }
                }
                else//4
                {
                    moveValue = new Vector3(-1, 0, 0);
                }
                return true;
            }
            else if (x >= width - right)//369
            {
                if (y <= bottom)//9
                {
                    if (Mathf.Abs(x - width + left) >= Mathf.Abs(y - bottom))//9->6
                    {
                        moveValue = new Vector3(1, 0);
                    }
                    else//9->8
                    {
                        moveValue = new Vector3(0, -1);
                    }
                }
                else if (y >= height - top)//3
                {
                    if (Mathf.Abs(x - width + left) >= Mathf.Abs(y - height + top))//3->6
                    {
                        moveValue = new Vector3(1, 0);
                    }
                    else//3->2
                    {
                        moveValue = new Vector3(0, 1);
                    }
                }
                else//6
                {
                    moveValue = new Vector3(1, 0);
                }
                return true;
            }
            else
            {
                if (y <= bottom)//8
                {
                    moveValue = new Vector3(0, -1);
                    return true;
                }
                else if (y >= height - top)//2
                {
                    moveValue = new Vector3(0, 1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有8方向的移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="moveValue">移动值：其中X标识水平（Horizontal）的移动量（范围[-1,1]），Y标识垂直（Vertical）的移动量（范围[-1,1]），Z恒定为0</param>
        /// <returns></returns>
        private bool HasMove8(float width, float height, Vector2 position, float left, float right, float top, float bottom, out Vector3 moveValue)
        {
            var x = position.x;
            var y = position.y;
            moveValue = default;

            if (x <= left)//147
            {
                if (y <= bottom)//7
                {
                    moveValue = new Vector3(-1, -1);
                }
                else if (y >= height - top)//1
                {
                    moveValue = new Vector3(-1, 1);
                }
                else//4
                {
                    moveValue = new Vector3(-1, 0);
                }
                return true;
            }
            else if (x >= width - right)//369
            {
                if (y <= bottom)//9
                {
                    moveValue = new Vector3(1, -1);
                }
                else if (y >= height - top)//3
                {
                    moveValue = new Vector3(1, 1);
                }
                else//6
                {
                    moveValue = new Vector3(1, 0);
                }
                return true;
            }
            else
            {
                if (y <= bottom)//8
                {
                    moveValue = new Vector3(0, -1);
                    return true;
                }
                else if (y >= height - top)//2
                {
                    moveValue = new Vector3(0, 1);
                    return true;
                }
            }
            return false;
        }
    }
}
