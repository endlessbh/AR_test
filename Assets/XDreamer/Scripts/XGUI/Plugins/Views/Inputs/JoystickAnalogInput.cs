using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 摇杆模拟输入:使用UGUI模拟输入摇杆轴的值
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [Name("摇杆模拟输入")]
    [Tip("使用UGUI模拟输入摇杆轴的值", "Use UGUI to simulate the value of input rocker shaft")]
    [Tool(XGUICategory.Input, nameof(XGUIManager))]
    public class JoystickAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler, IDragHandler, IOnDrawGizmosSelected
    {
        /// <summary>
        /// 输入模式
        /// </summary>
        [Name("输入模式")]
        public enum EInputMode
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Name("默认")]
            Default,

            /// <summary>
            /// 输入横向-输入扩展纵向
            /// </summary>
            [Name("输入横向-输入扩展纵向")]
            Input_Horizontal__InputExtension_Vertical,

            /// <summary>
            /// 输入纵向-输入扩展横向
            /// </summary>
            [Name("输入纵向-输入扩展横向")]
            Input_Vertical__InputExtension_Horizontal,
        }

        /// <summary>
        /// 输入模式
        /// </summary>
        [Name("输入模式")]
        [EnumPopup]
        public EInputMode _inputMode = EInputMode.Default;

        /// <summary>
        /// 输入扩展
        /// </summary>
        [Name("输入扩展")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_inputMode), EValidityCheckType.Equal, EInputMode.Default)]
        public EInput _inputExtension = EInput.VirtualInput;

        /// <summary>
        /// 输入扩展
        /// </summary>
        public IInput inputExtension => _inputExtension.GetInput();

        /// <summary>
        /// 当前对象开始拖拽时相对父节点的坐标
        /// </summary>
        public Vector3 originLocalPosition { get; protected set; }

        /// <summary>
        /// 当前对象开始拖拽时屏幕坐标
        /// </summary>
        public Vector3 originPosition { get; protected set; }

        /// <summary>
        /// 当前对象开始拖拽时世界矩阵;Gizmos绘制时使用；
        /// </summary>
        public Matrix4x4 originWorldMatrix { get; protected set; }

        /// <summary>
        /// 控件拖拽中相对开始拖拽时的位置偏移量
        /// </summary>
        protected Vector3 offset = new Vector3();

        /// <summary>
        /// 标识当前Button是否在拖拽中
        /// </summary>
        public bool isDragging { get; protected set; }

        /// <summary>
        /// 限定范围类型
        /// </summary>
        [Name("限定范围类型")]
        public enum ELimitRangeType
        {
            [Name("半边长")]
            [Tip("正方形的限定范围类型；为限定正方形的边长的一半；", "Limited range type of square; Is half of the side length of the defined square;")]
            HalfLength = 0,

            [Name("半径")]
            [Tip("圆形的限定范围类型；为限定圆的半径；", "Limited range type of circle; Is to define the radius of the circle;")]
            Radius,
        }

        /// <summary>
        /// 限定轴类型
        /// </summary>
        [Name("限定轴类型")]
        public enum ELimitAxisType
        {
            [Name("全部")]
            [Tip("限定在横向轴与纵向轴均可移动；", "It is limited that the transverse axis and longitudinal axis can move;")]
            Both,

            [Name("仅横向")]
            [Tip("限定在横向轴移动；", "Limited to lateral axis movement;")]
            OnlyHorizontal,

            [Name("仅纵向")]
            [Tip("限定在纵向轴移动；", "Limited movement in the longitudinal axis;")]
            OnlyVertical
        }

        [Name("运动范围")]
        [Tip("当前按钮被拖拽时以初始位置为中心可移动的范围；", "The movable range centered on the initial position when the current button is dragged;")]
        [Range(0.01f, 540f)]
        [FormerlySerializedAs(nameof(movementRange))]
        public float _movementRange = 30;

        public float movementRange => _movementRange;

        [Name("运动范围限定类型")]
        [Tip("当前按钮被拖拽时可移动的范围的类型；", "The type of range that can be moved when the current button is dragged;")]
        [EnumPopup]
        [FormerlySerializedAs(nameof(limitRangeType))]
        public ELimitRangeType _limitRangeType = ELimitRangeType.Radius;

        public ELimitRangeType limitRangeType => _limitRangeType;

        [Name("运动轴向限定类型")]
        [Tip("当前按钮被拖拽时可移动的轴向的类型；", "The type of axial direction that can be moved when the current button is dragged;")]
        [EnumPopup]
        [FormerlySerializedAs(nameof(limitAxisType))]
        public ELimitAxisType _limitAxisType = ELimitAxisType.Both;

        public ELimitAxisType limitAxisType => _limitAxisType;

        [Name("横向输入")]
        [Input]
        [FormerlySerializedAs(nameof(horizontalInput))]
        public string _horizontalInput = "Horizontal";

        [Name("纵向输入")]
        [Input]
        [FormerlySerializedAs(nameof(verticaInput))]
        public string _verticalInput = "Vertical";

        IInput horizontalInput
        {
            get
            {
                switch (_inputMode)
                {
                    case EInputMode.Default: return input;
                    case EInputMode.Input_Horizontal__InputExtension_Vertical: return input;
                    case EInputMode.Input_Vertical__InputExtension_Horizontal: return inputExtension;
                    default: return null;
                }
            }
        }

        IInput verticaInput
        {
            get
            {
                switch (_inputMode)
                {
                    case EInputMode.Default: return input;
                    case EInputMode.Input_Horizontal__InputExtension_Vertical: return inputExtension;
                    case EInputMode.Input_Vertical__InputExtension_Horizontal: return input;
                    default: return null;
                }
            }
        }

        /// <summary>
        /// 摇杆当前水平方向值
        /// </summary>
        public float horizontalValue => offset.x / _movementRange;

        /// <summary>
        /// 摇杆当前垂直方向值
        /// </summary>
        public float verticalValue => offset.y / _movementRange;

        private void BroadcastInput()
        {
            switch (_limitAxisType)
            {
                case ELimitAxisType.Both:
                    {
                        UpdateAxis(horizontalInput, _horizontalInput, horizontalValue);
                        UpdateAxis(verticaInput, _verticalInput, verticalValue);
                        break;
                    }
                case ELimitAxisType.OnlyHorizontal:
                    {
                        UpdateAxis(horizontalInput, _horizontalInput, horizontalValue);
                        break;
                    }
                case ELimitAxisType.OnlyVertical:
                    {
                        UpdateAxis(verticaInput, _verticalInput, verticalValue);
                        break;
                    }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            CommonFun.BeginOnUI();
            originLocalPosition = transform.localPosition;
            originPosition = transform.position;
            originWorldMatrix = transform.localToWorldMatrix;

            offset = Vector3.zero;
            BroadcastInput();
        }

        public void OnDrag(PointerEventData eventData)
        {
            offset.x = eventData.position.x - originPosition.x;
            offset.y = eventData.position.y - originPosition.y;
            //Debug.Log("offset: " + offset + " , delta: " + data.delta + ", data.position: " + data.position );
            switch (_limitRangeType)
            {
                case ELimitRangeType.HalfLength:
                    {
                        offset.x = Mathf.Clamp(offset.x, -_movementRange, _movementRange);
                        offset.y = Mathf.Clamp(offset.y, -_movementRange, _movementRange);
                        break;
                    }
                case ELimitRangeType.Radius:
                default:
                    {
                        offset = offset.normalized * Mathf.Min(offset.magnitude, _movementRange);
                        break;
                    }
            }
            switch (_limitAxisType)
            {
                case ELimitAxisType.OnlyHorizontal:
                    {
                        offset.y = 0;
                        break;
                    }
                case ELimitAxisType.OnlyVertical:
                    {
                        offset.x = 0;
                        break;
                    }
            }
            transform.localPosition = originLocalPosition + offset;

            BroadcastInput();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            CommonFun.EndOnUI();
            transform.localPosition = originLocalPosition;

            offset = Vector3.zero;
            BroadcastInput();
        }

        public void OnDrawGizmosSelected()
        {
            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            if (Application.isPlaying) Gizmos.matrix = originWorldMatrix;
            else Gizmos.matrix = this.transform.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.green;

            switch (_limitRangeType)
            {
                case ELimitRangeType.HalfLength:
                    {
                        Gizmos.DrawLine(new Vector3(_movementRange, _movementRange, 0), new Vector3(_movementRange, -_movementRange, 0));
                        Gizmos.DrawLine(new Vector3(_movementRange, -_movementRange, 0), new Vector3(-_movementRange, -_movementRange, 0));
                        Gizmos.DrawLine(new Vector3(-_movementRange, -_movementRange, 0), new Vector3(-_movementRange, _movementRange, 0));
                        Gizmos.DrawLine(new Vector3(-_movementRange, _movementRange, 0), new Vector3(_movementRange, _movementRange, 0));
                        break;
                    }
                case ELimitRangeType.Radius:
                default:
                    {
                        // 绘制圆环
                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;
                        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.0001f)
                        {
                            float x = _movementRange * Mathf.Cos(theta);
                            float y = _movementRange * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, y, 0);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        // 绘制最后一条线段
                        Gizmos.DrawLine(firstPoint, beginPoint);
                        break;
                    }
            }
            switch (_limitAxisType)
            {
                case ELimitAxisType.OnlyHorizontal:
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(new Vector3(-_movementRange, 0, 0), new Vector3(_movementRange, 0, 0));
                        break;
                    }
                case ELimitAxisType.OnlyVertical:
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(new Vector3(0, _movementRange, 0), new Vector3(0, -_movementRange, 0));
                        break;
                    }
            }

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
        }
    }
}
