using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Controllers
{
    [Tool("控制")]
    [Name("旋转控制器")]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class RotateController : ToolController, IUpdate, IReset, IInputEventSender
    {
        [SerializeField]
        [Name("目标")]
        [Readonly(EEditorMode.Runtime)]
        [FormerlySerializedAs(nameof(target))]
        private Transform _target;

        public Transform target
        {
            get => _target;
            set
            {
                _target = value;
                if (_target) UpdateTarget();
            }
        }

        [Name("空间")]
        public Space space = Space.Self;

        [Name("速度")]
        [Tip("单位时间旋转的角度;", "Rotation angle per unit time;")]
        public float speed = 60f;

        [Name("仰视/抬头")]
        [Tip("对应X轴旋转", "Corresponding to X-axis rotation")]
        public KeyCode up = KeyCode.UpArrow;

        [Name("俯视/低头")]
        [Tip("对应X轴旋转", "Corresponding to X-axis rotation")]
        public KeyCode down = KeyCode.DownArrow;

        [Name("左转")]
        [Tip("对应Y轴旋转", "Corresponding to Y-axis rotation")]
        public KeyCode left = KeyCode.LeftArrow;

        [Name("右转")]
        [Tip("对应Y轴旋转", "Corresponding to Y-axis rotation")]
        public KeyCode right = KeyCode.RightArrow;

        [Name("左倾斜/逆时旋转")]
        [Tip("对应Z轴旋转", "Corresponding to Z-axis rotation")]
        public KeyCode leftTilt = KeyCode.None;

        [Name("右倾斜/顺时旋转")]
        [Tip("对应Z轴旋转", "Corresponding to Z-axis rotation")]
        public KeyCode rightTilt = KeyCode.None;

        /// <summary>
        /// 事件处理者，仅对目标生效
        /// </summary>
        public List<IEventHandler> handlers { get; private set; } = new List<IEventHandler>();

        public bool enableSend { get; set; } = true;

        public bool enableInput { get => enabled; set => enabled = value; }

        public RotateEventArgs eventArgs = new RotateEventArgs();   
        
        private void UpdateTarget()
        {
            handlers.Clear();
            _target.GetComponents(handlers);

            eventArgs.transform = _target;
        }

        public void Start()
        {
            if (!target) target = transform;
            else UpdateTarget();
        }

        public void Update()
        {
            var target = this.target;
            if (!target) return;

            Vector3 value = new Vector3();
            bool noEvent = true;

            //抬头
            if (XInput.GetKey(up))
            {
                value.x -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(up))
            {
                noEvent = false;
            }

            //低头
            if (XInput.GetKey(down))
            {
                value.x += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(down))
            {
                noEvent = false;
            }

            //左转
            if (XInput.GetKey(left))
            {
                value.y -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(left))
            {
                noEvent = false;
            }

            //右转
            if (XInput.GetKey(right))
            {
                value.y += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(right))
            {
                noEvent = false;
            }

            //左倾斜/逆时旋转
            if (XInput.GetKey(leftTilt))
            {
                value.z += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(leftTilt))
            {
                noEvent = false;
            }

            //右倾斜/顺时旋转
            if (XInput.GetKey(rightTilt))
            {
                value.z -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(rightTilt))
            {
                noEvent = false;
            }


            //没有事件发生不做处理
            if (noEvent) return;

            if (value != Vector3.zero)
            {
                value = value.normalized * speed;
            }

            eventArgs.handled.Clear();
            eventArgs.velocity = value;
            eventArgs.space = space;

            if (!enableSend || !this.CallEvent(eventArgs))
            {
                target.Rotate(value * Time.deltaTime, space);
            }
        }

        public void Reset()
        {
            target = transform;
        }
    }
}
