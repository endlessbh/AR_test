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
    [Name("缩放控制器")]
    [XCSJ.Attributes.Icon(EIcon.Scale)]
    public class ScaleController : ToolController, IUpdate, IReset, IInputEventSender
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

        //[Name("空间")]
        //public Space space = Space.Self;

        [Name("速度")]
        [Tip("单位时间缩放的值;", "Scaling value per unit time;")]
        public float speed = 1f;

        [Name("长加")]
        [Tip("对应X轴缩放", "Corresponding X-axis scaling")]
        public KeyCode lengthIncrease = KeyCode.None;

        [Name("长减")]
        [Tip("对应X轴缩放", "Corresponding X-axis scaling")]
        public KeyCode lengthReduce = KeyCode.None;

        [Name("宽加")]
        [Tip("对应Z轴缩放", "Corresponding Z-axis scaling")]
        public KeyCode widthIncrease = KeyCode.None;

        [Name("宽减")]
        [Tip("对应Z轴缩放", "Corresponding Z-axis scaling")]
        public KeyCode widthReduce = KeyCode.None;

        [Name("高加")]
        [Tip("对应Y轴缩放", "Corresponding Y-axis scaling")]
        public KeyCode heightIncrease = KeyCode.None;

        [Name("高减")]
        [Tip("对应Y轴缩放", "Corresponding Y-axis scaling")]
        public KeyCode heightReduce = KeyCode.None;

        /// <summary>
        /// 事件处理者，仅对目标生效
        /// </summary>
        public List<IEventHandler> handlers { get; private set; } = new List<IEventHandler>();

        public bool enableSend { get; set; } = true;

        public bool enableInput { get => enabled; set => enabled = value; }

        public ScaleEventArgs eventArgs = new ScaleEventArgs();

        private void UpdateTarget()
        {
            handlers.Clear();
            _target.GetComponents(handlers);

            eventArgs.transform = _target;
            eventArgs.space = Space.Self;
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

            //长加
            if (XInput.GetKey(lengthIncrease))
            {
                value.x += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(lengthIncrease))
            {
                noEvent = false;
            }

            //长减
            if (XInput.GetKey(lengthReduce))
            {
                value.x -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(lengthReduce))
            {
                noEvent = false;
            }

            //宽加
            if (XInput.GetKey(widthIncrease))
            {
                value.z += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(widthIncrease))
            {
                noEvent = false;
            }

            //宽减
            if (XInput.GetKey(widthReduce))
            {
                value.z -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(widthReduce))
            {
                noEvent = false;
            }

            //高加
            if (XInput.GetKey(heightIncrease))
            {
                value.y += 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(heightIncrease))
            {
                noEvent = false;
            }

            //高减
            if (XInput.GetKey(heightReduce))
            {
                value.y -= 1;
                noEvent = false;
            }
            else if (XInput.GetKeyUp(heightReduce))
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

            if (!enableSend || !this.CallEvent(eventArgs))
            {
                target.localScale += (value * Time.deltaTime);
            }
        }

        public void Reset()
        {
            target = transform;
        }
    }
}
