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
    [Name("移动控制器")]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class MoveController : ToolController, IUpdate, IReset, IInputEventSender
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
        [Tip("单位时间特定方向移动的距离;", "The distance to move in a specific direction per unit time;")]
        public float speed = 1f;

        [Name("前进")]
        [Tip("对应Z轴移动", "Corresponding to Z-axis movement")]
        public KeyCode forward = KeyCode.W;

        [Name("后退")]
        [Tip("对应Z轴移动", "Corresponding to Z-axis movement")]
        public KeyCode back = KeyCode.S;

        [Name("左移")]
        [Tip("对应X轴移动", "Corresponding to X-axis movement")]
        public KeyCode left = KeyCode.A;

        [Name("右移")]
        [Tip("对应X轴移动", "Corresponding to X-axis movement")]
        public KeyCode right = KeyCode.D;

        [Name("上移")]
        [Tip("对应Y轴移动", "Corresponding to Y-axis movement")]
        public KeyCode up = KeyCode.Q;

        [Name("下移")]
        [Tip("对应Y轴移动", "Corresponding to Y-axis movement")]
        public KeyCode down = KeyCode.E;

        /// <summary>
        /// 事件处理者，仅对目标生效
        /// </summary>
        public List<IEventHandler> handlers { get; private set; } = new List<IEventHandler>();

        public bool enableSend { get; set; } = true;

        public bool enableInput { get => enabled; set => enabled = value; }

        public MoveEventArgs eventArgs = new MoveEventArgs();

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

            //前
            if (XInput.GetKey(forward))
            {
                value.z += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(forward))
            {
                noEvent = false;
            }

            //后
            if (XInput.GetKey(back))
            {
                value.z -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(back))
            {
                noEvent = false;
            }

            //左
            if (XInput.GetKey(left))
            {
                value.x -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(left))
            {
                noEvent = false;
            }

            //右
            if (XInput.GetKey(right))
            {
                value.x += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(right))
            {
                noEvent = false;
            }

            //上
            if (XInput.GetKey(up))
            {
                value.y += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(up))
            {
                noEvent = false;
            }

            //下
            if (XInput.GetKey(down))
            {
                value.y -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(down))
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
                target.Translate(value * Time.deltaTime, space);
            }
        }

        public void Reset()
        {
            target = transform;
        }
    }
}
