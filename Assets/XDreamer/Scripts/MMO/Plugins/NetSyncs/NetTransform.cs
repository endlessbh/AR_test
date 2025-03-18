using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [Attributes.Icon]
    [DisallowMultipleComponent]
    [Name("网络变换")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public sealed class NetTransform : NetMB, IEventHandler, IReset
    {
        [Group("同步检测设置")]
        [Name("同步模式")]
        [EnumPopup]
        public ESyncMode syncMode = ESyncMode.Transform;

        [Name("位置阈值")]
        [Range(0, 0.1f)]
        public float positionTheshold = 0.005f;

        [Name("位置捕捉阈值")]
        [Tip("如果移动更新的目标位置与当前位置超过本值，那么当前对象将直接移动到目标位置，而不做平滑移动；单位：米(默认);", "If the updated target position and the current position exceed this value, the current object will move directly to the target position without smooth movement; Unit: meter (default);")]
        [Range(0.1f, 10)]
        public float positionSnapThreshold = 5;

        [Name("速度阈值")]
        [Range(0, 0.1f)]
        public float velocityTheshold = 0.001f;

        [Name("角度阈值")]
        [Tip("单位：度", "Unit: degrees")]
        [Range(0, 1f)]
        public float angleTheshold = 0.1f;

        [Name("角度捕捉阈值")]
        [Tip("如果旋转更新的目标角度与当前角度超过本值，那么当前对象将直接旋转到目标角度，而不做平滑旋转；单位：度;", "If the target angle updated by rotation and the current angle exceed this value, the current object will rotate directly to the target angle without smooth rotation; Unit: degree;")]
        [Range(1f, 90f)]
        public float angleSnapThreshold = 30f;

        [Name("角速度阈值")]
        [Range(0, 0.1f)]
        public float angularVelocityTheshold = 0.01f;

        [Name("缩放阈值")]
        [Range(0, 0.1f)]
        public float scaleTheshold = 0.1f;

        [Name("缩放速度阈值")]
        [Range(0, 0.1f)]
        public float scaleVelocityTheshold = 0.01f;

        [Group("同步数据")]
        [Readonly]
        [Name("数据")]
        [SyncVar(sync = false)]
        public TransformData data = new TransformData();

        [Readonly]
        [Name("目标数据")]
        public TransformData targetData = new TransformData();

        [Readonly]
        [Name("上一次数据")]
        public TransformData prevData = new TransformData();

        [Readonly]
        [Name("原始数据")]
        public TransformData originalData = new TransformData();

        [Group("其他设置")]
        /// <summary>
        /// 移动提前系数，用于运动补间时预测移动提前的系数
        /// </summary>
        [Name("移动提前系数")]
        [Tip("用于运动补间时预测游戏对象移动前移的系数", "The coefficient used to predict the movement and forward movement of the game object when making up the gap")]
        [Range(0.01f, 2f)]
        public float moveAheadRatio = 0.8f;

        /// <summary>
        /// 旋转提前系数，用于运动补间时预测旋转提前的系数
        /// </summary>
        [Name("旋转提前系数")]
        [Tip("用于运动补间时预测游戏对象旋转提前的系数", "The coefficient used to predict the rotation advance of the game object during motion compensation")]
        [Range(0.01f, 2f)]
        public float rotateAheadRatio = 0.8f;

        //[Readonly(EEditorMode.Runtime)]
        //[Name("自动设置输入")]
        //[Tip("根据当前游戏对象上网络标识所具有的各项权限以及与玩家的关联绑定关系，对当前游戏对象上的具有输入事件发送者(IInputEventSender)接口的组件自动设置启用或禁用输入")]
        //public bool autoSetInput = true;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2D;
        private CharacterController characterController;

        private void CacheData()
        {
            if (_transform) return;
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>(); ;
            _rigidbody2D = GetComponent<Rigidbody2D>(); ;
            characterController = GetComponent<CharacterController>();
        }

        public void Awake()
        {
            CacheData();
        }

        public override void OnSyncEnable()
        {
            CacheData();
            base.OnSyncEnable();
            data.SetData(_transform);
            prevData = new TransformData();//保证至少同步一次
            targetData.SetData(data);
            originalData.SetData(data);
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();
            originalData.SetTransform(_transform);
        }

        public void FixedUpdate()
        {
            if (!canSync || !interpolate) return;
            switch (syncMode)
            {
                case ESyncMode.Rigidbody:
                    {                       
                        if(interpolateMove) //平滑移动
                        {
                            var offset = targetData.position - _rigidbody.position;
                            if (offset.magnitude >= positionTheshold)
                            {
                                if(_rigidbody.isKinematic)//对于运动学刚体通过移动位置进行补间
                                {
                                    _rigidbody.MovePosition(_rigidbody.position + offset.normalized * targetData.speed * Time.fixedDeltaTime);
                                }
                                else//对于非运动学刚体通过设置速度进行补间
                                {
                                    _rigidbody.velocity = targetData.velocity;
                                }

                                targetData.position += targetData.velocity * Time.fixedDeltaTime * moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }       
                        
                        if(interpolateRotate)//平滑旋转
                        {
                            var targetRotation = targetData.rotation;
                            var angle = Quaternion.Angle(targetRotation, _rigidbody.rotation);
                            if (angle >= angleTheshold)
                            {
                                _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, Time.fixedDeltaTime / intervalTime));
                                //_rigidbody.angularVelocity = targetData.angularVelocity;
                                targetData.eulerAngles += targetData.angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime * rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
                case ESyncMode.Rigidbody2D:
                    {
                        if (interpolateMove) //平滑移动
                        {
                            var offset = (Vector2)targetData.position - _rigidbody2D.position;
                            if (offset.magnitude >= positionTheshold)
                            {
                                if(_rigidbody2D.isKinematic)//对于运动学2D刚体通过移动位置进行补间
                                {
                                    _rigidbody2D.MovePosition(_rigidbody2D.position + offset.normalized * targetData.speed * Time.fixedDeltaTime);
                                }
                                else//对于非运动学2D刚体通过设置速度进行补间
                                {
                                    _rigidbody2D.velocity = targetData.velocity;
                                }
                                targetData.position += targetData.velocity * Time.fixedDeltaTime * moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }

                        if (interpolateRotate)//平滑旋转
                        {
                            var angle = Math.Abs(_rigidbody2D.rotation - targetData.eulerAngles.z);
                            if (angle >= angleTheshold)
                            {
                                _rigidbody2D.MoveRotation(Mathf.LerpAngle(_rigidbody2D.rotation, targetData.eulerAngles.z, Time.fixedDeltaTime / intervalTime));
                                targetData.eulerAngles.z += targetData.angularVelocity.z * Time.fixedDeltaTime * rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!canSync || !interpolate) return;
            switch (syncMode)
            {
                case ESyncMode.Transform:
                    {
                        if (interpolateMove) //平滑移动
                        {
                            var offset = targetData.position - _transform.position;
                            if (offset.magnitude >= positionTheshold)
                            {
                                _transform.position += offset.normalized * targetData.speed * Time.deltaTime; ;

                                targetData.position += targetData.velocity * Time.deltaTime * moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }

                        if (interpolateRotate)//平滑旋转
                        {
                            var targetRotation = targetData.rotation;
                            var angle = Quaternion.Angle(targetRotation, _transform.rotation);
                            if (angle >= angleTheshold)
                            {
                                _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime / intervalTime);
                                targetData.eulerAngles += targetData.angularVelocity * Time.deltaTime * rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
            }
        }

        public override void Reset()
        {
            base.Reset();
            intervalTime = 0.05f;
        }

        bool interpolateMove = false;
        bool interpolateRotate = false;
        bool interpolate
        {
            get=> interpolateMove || interpolateRotate;
            set
            {
                interpolateMove = value;
                interpolateRotate = value;
            }
        }
        HashSet<int> waitReceive = new HashSet<int>();

        protected override bool OnTimedCheckChange()
        {
            if (waitReceive.Contains(version)) return false;
            return data.SetData(syncMode, _transform, _rigidbody, _rigidbody2D, characterController) && HasChange(prevData, data);
        }

        protected override string OnSerializeData()
        {
            waitReceive.Add(version);
            return data.ToJson();
        }

        protected override void OnDeserializeData(string data, Data dataObj)
        {
            if (JsonHelper.ToObject<TransformData>(data) is TransformData _data)
            {
                waitReceive.RemoveWhere(i => i <= dataObj.version);
                if (CanOptimizable(dataObj))//当前用户发送的数据不需要补间处理
                {
                    interpolate = false;
                    if (!HasChange(this.data, _data))
                    {
                        prevData.SetData(this.data);
                    }
                    return;
                } 

                //Debug.LogFormat("OnDeserializeData=={0}=={1}", name, data);
                targetData = _data;
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            if (!_transform)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float magnitude = (targetData.position - _transform.position).magnitude;
                                if (magnitude > positionSnapThreshold || targetData.velocity == Vector3.zero)
                                {
                                    _transform.position = targetData.position;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    targetData.SetSpeed(magnitude, intervalTime);
                                }
                            }

                            //平滑旋转分析
                            {
                                var targetRotation = targetData.rotation;
                                var angle = Quaternion.Angle(_transform.rotation, targetRotation);
                                if (angle > angleSnapThreshold || targetData.angularVelocity == Vector3.zero)
                                {
                                    _transform.rotation = targetRotation;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                }
                            }

                            _transform.localScale = targetData.localScale;

                            //_data.SetTransform(_transform);
                            break;
                        }
                    case ESyncMode.Rigidbody:
                        {
                            if (!_rigidbody)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float magnitude = (_rigidbody.position - targetData.position).magnitude;
                                if (magnitude > positionSnapThreshold || targetData.velocity == Vector3.zero)
                                {
                                    _rigidbody.position = targetData.position;
                                    _rigidbody.velocity = targetData.velocity;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    targetData.SetSpeed(magnitude, intervalTime);

                                    if (MathX.ApproximatelyZero(targetData.velocity.y))//纵向速度(Y)为0时，强制设置纵坐标(Y)同步
                                    {
                                        var pos = _rigidbody.position;
                                        _rigidbody.position = new Vector3(pos.x, targetData.position.y, pos.z);
                                    }
                                }
                            }

                            //平滑旋转分析
                            {
                                var targetRotation = targetData.rotation;
                                var angle = Quaternion.Angle(_rigidbody.rotation, targetRotation);
                                if (angle > angleSnapThreshold || targetData.angularVelocity == Vector3.zero)
                                {
                                    _rigidbody.rotation = targetRotation;
                                    _rigidbody.angularVelocity = targetData.angularVelocity;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                    _rigidbody.angularVelocity = targetData.angularVelocity;
                                }
                            }

                            _transform.localScale = targetData.localScale;

                            //_data.SetRigidbody(_rigidbody, _transform);
                            break;
                        }
                    case ESyncMode.Rigidbody2D:
                        {
                            if (!_rigidbody2D)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float magnitude = (_rigidbody2D.position - (Vector2)targetData.position).magnitude;
                                if (magnitude > positionSnapThreshold || targetData.velocity == Vector3.zero)
                                {
                                    _rigidbody2D.position = targetData.position;
                                    _rigidbody2D.velocity = targetData.velocity;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    targetData.SetSpeed(magnitude, intervalTime);
                                }
                            }

                            //平滑旋转分析
                            {
                                var angle = Math.Abs(_rigidbody2D.rotation - targetData.eulerAngles.z);
                                if (angle > angleSnapThreshold || Mathf.Approximately(targetData.angularVelocity.z, 0))
                                {
                                    _rigidbody2D.rotation = targetData.eulerAngles.z;
                                    _rigidbody2D.angularVelocity = targetData.angularVelocity.z;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                }
                            }
                           
                            _transform.localScale = targetData.localScale;

                            //_data.SetRigidbody2D(_rigidbody2D, _transform);
                            break;
                        }
                    case ESyncMode.CharacterController:
                        {
                            if (!characterController) return;
                            _data.SetCharacterController(characterController, _transform);
                            break;
                        }
                }
            }
            else
            {
                Log.WarningFormat("对象[{0}]执行[{1}]反序列化数据[{2}]时失败！", CommonFun.GameObjectComponentToString(this), nameof(OnDeserializeData), data);
            }
        }

        private bool HasChange(TransformData lData, TransformData rData)
        {
            if (lData == rData) return false;

            if ((lData.position - rData.position).magnitude >= positionTheshold) return true;
            if ((lData.velocity - rData.velocity).magnitude >= velocityTheshold) return true;

            if (Quaternion.Angle(Quaternion.Euler(lData.eulerAngles), Quaternion.Euler(rData.eulerAngles)) >= angleTheshold) return true;
            if ((lData.angularVelocity - rData.angularVelocity).magnitude >= angularVelocityTheshold) return true;

            if ((lData.localScale - rData.localScale).magnitude >= scaleTheshold) return true;
            if ((lData.scaleVelocity - rData.scaleVelocity).magnitude >= scaleVelocityTheshold) return true;

            return false;
        }

        public override void OnStart()
        {
            base.OnStart();

            inputs.Clear();
            foreach (var c in GetComponents<IInputEventSender>())
            {
                Record(c);
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            foreach (var kv in inputs)
            {
                kv.Key.enableInput = kv.Value;
            }
            inputs.Clear();
        }

        public override void OnStartPlayerLink()
        {
            base.OnStartPlayerLink();

            if (isLocalPlayer)
            {
                //启用输入事件
                foreach (var c in GetComponents<IInputEventSender>())
                {
                    Record(c);
                    c.enableInput = true;
                }
            }
            else
            {
                //禁用输入事件
                foreach (var c in GetComponents<IInputEventSender>())
                {
                    Record(c);
                    c.enableInput = false;
                }
            }
        }

        public override void OnStartAccess(bool local)
        {
            base.OnStartAccess(local);
            AutoSetInput();
        }

        public override void OnStopAccess(bool local)
        {
            base.OnStopAccess(local);
            AutoSetInput();
        }

        #region IInputEventSender与IEventHandler

        Dictionary<IInputEventSender, bool> inputs = new Dictionary<IInputEventSender, bool>();

        private void Record(IInputEventSender inputEventSender)
        {
            if (!inputs.ContainsKey(inputEventSender)) inputs.Add(inputEventSender, inputEventSender.enableInput);
        }

        private void AutoSetInput()
        {
            //if (autoSetInput)
            {
                var hasControlAccess = netIdentity.hasControlAccess;
                foreach (var c in GetComponents<IInputEventSender>())
                {
                    Record(c);
                    c.enableInput = hasControlAccess;
                }
            }
        }

        public bool OnEvent(IEventSender sender, XEventArgs e)
        {
            if (e is MoveEventArgs move)
            {
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            data.velocity = move.space == Space.World ? move.velocity : _transform.TransformDirection(move.velocity);
                            break;
                        }
                }
            }
            else if (e is RotateEventArgs rotate)
            {
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            data.angularVelocity = rotate.velocity;
                            break;
                        }
                }
            }
            else if (e is ScaleEventArgs scale)
            {
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            data.scaleVelocity = scale.velocity;
                            break;
                        }
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 检查当前游戏对象上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void CheckComponent<T>() where T : Component
        {
            if (!GetComponent<T>())
            {
                Log.WarningFormat("游戏对象[{0}]上没有组件[{1}]({2}),无法设置[{3}]({4})类型的同步模式！",
                    CommonFun.GameObjectToString(this.gameObject),
                    CommonFun.Name(typeof(T)),
                    typeof(T).FullName,
                    CommonFun.Name(syncMode),
                    syncMode.ToString());
                syncMode = ESyncMode.Transform;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            switch (syncMode)
            {
                case ESyncMode.Rigidbody:
                    {
                        CheckComponent<Rigidbody>();
                        break;
                    }
                case ESyncMode.Rigidbody2D:
                    {
                        CheckComponent<Rigidbody2D>();
                        break;
                    }
                case ESyncMode.CharacterController:
                    {
                        CheckComponent<CharacterController>();
                        break;
                    }
            }
        }

        /// <summary>
        /// 变换数据
        /// </summary>
        [Name("变换数据")]
        [Serializable]
        [Import]
        public class TransformData : JsonObject<TransformData>
        {
            [Json(exportString = true)]
            [Name("位置")]
            [Readonly]
            public Vector3 position;

            [Json(exportString = true)]
            [Name("速度")]
            [Readonly]
            public Vector3 velocity;

            internal float speed;
            internal void SetSpeed(float distance, float time)
            {
                speed = velocity.magnitude;
                if (Mathf.Approximately(speed, 0))
                {
                    speed = distance / time;
                }
            }

            [Json(exportString = true)]
            [Name("角度")]
            [Tip("欧拉角度，单位：度", "Euler angle in degrees")]
            [Readonly]
            public Vector3 eulerAngles;

            internal Quaternion rotation => Quaternion.Euler(eulerAngles);

            [Json(exportString = true)]
            [Name("角速度")]
            [Tip("欧拉角速度，默认单位：度/秒 ；同步类型为刚体时,单位:弧度/秒；", "Euler angular velocity, default unit: degrees / second; When the synchronization type is rigid body, the unit is radian / S;")]
            [Readonly]
            public Vector3 angularVelocity;

            [Json(exportString = true)]
            [Name("缩放")]
            [Readonly]
            public Vector3 localScale;

            [Json(exportString = true)]
            [Name("缩放速度")]
            [Readonly]
            public Vector3 scaleVelocity;

            public void SetData(TransformData data)
            {
                this.position = data.position;
                this.velocity = data.velocity;

                this.eulerAngles = data.eulerAngles;
                this.angularVelocity = data.angularVelocity;

                this.localScale = data.localScale;
                this.scaleVelocity = data.scaleVelocity;
            }
            public bool SetData(ESyncMode syncMode, Transform transform, Rigidbody rigidbody, Rigidbody2D rigidbody2D, CharacterController characterController)
            {
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            SetData(transform);
                            break;
                        }
                    case ESyncMode.Rigidbody:
                        {
                            if (!rigidbody) return false;
                            SetData(rigidbody, transform);
                            break;
                        }
                    case ESyncMode.Rigidbody2D:
                        {
                            if (!rigidbody2D) return false;
                            SetData(rigidbody2D, transform);
                            break;
                        }
                    case ESyncMode.CharacterController:
                        {
                            if (!characterController) return false;
                            SetData(characterController, transform);
                            break;
                        }
                    case ESyncMode.None:
                    default:
                        {
                            return false;
                        }
                }
                return true;
            }

            public void SetData(Transform transform)
            {
                this.position = transform.position; 
                //this.velocity = Vector3.zero;

                this.eulerAngles = transform.eulerAngles;
                //this.angularVelocity = Vector3.zero;

                this.localScale = transform.localScale;
                //this.scaleVelocity = Vector3.zero;
            }
            public void SetTransform(Transform transform)
            {
                transform.position = this.position;

                transform.eulerAngles = this.eulerAngles;

                transform.localScale = this.localScale;
            }

            public void SetData(Rigidbody rigidbody, Transform transform)
            { 
                this.position = rigidbody.position;
                this.velocity = rigidbody.velocity;

                this.eulerAngles = rigidbody.rotation.eulerAngles;
                this.angularVelocity = rigidbody.angularVelocity;

                this.localScale = transform.localScale;
            }
            public void SetRigidbody(Rigidbody rigidbody, Transform transform)
            {
                rigidbody.MovePosition(this.position);
                rigidbody.velocity = this.velocity;
                
                rigidbody.MoveRotation(Quaternion.Euler(eulerAngles));
                rigidbody.angularVelocity = this.angularVelocity;

                transform.localScale = this.localScale;
            }

            public void SetData(Rigidbody2D rigidbody2D, Transform transform)
            {
                this.position = rigidbody2D.position;
                this.velocity = rigidbody2D.velocity;

                this.eulerAngles = new Vector3(0, 0, rigidbody2D.rotation);
                this.angularVelocity = new Vector3(0, 0, rigidbody2D.angularVelocity);

                this.localScale = transform.localScale;
            }
            public void SetRigidbody2D(Rigidbody2D rigidbody2D, Transform transform)
            {
                transform.position = this.position;
                rigidbody2D.velocity = this.velocity;

                rigidbody2D.MoveRotation(this.eulerAngles.z);
                rigidbody2D.angularVelocity = this.angularVelocity.z;

                transform.localScale = this.localScale;
            }

            public void SetData(CharacterController characterController, Transform transform)
            {
                SetData(transform);
            }
            public void SetCharacterController(CharacterController characterController, Transform transform)
            {
                SetTransform(transform);
            }
        }

        [Name("同步模式")]
        public enum ESyncMode
        {
            [Name("无")]
            None = 0,

            [Name("变换")]
            Transform,

            [Name("刚体")]
            Rigidbody,

            [Name("2D刚体")]
            Rigidbody2D,

            [Name("角色控制器")]
            [Tip("目前对角色控制器同步模式支持不友好", "Currently, it is unfriendly to support role controller synchronization mode")]
            CharacterController,
        }
    }
}
