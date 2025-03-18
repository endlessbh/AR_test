using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginART.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// ART流客户端
    /// </summary>
    [Name("ART流客户端 ")]
    [RequireManager(typeof(ARTManager))]
    public class ARTStreamClient : InteractProvider
    {
        /// <summary>
        /// 服务器主机
        /// </summary>
        [Name("服务器主机")]
        public string _serverHost = "";

        /// <summary>
        /// 服务器端口:如仅接收数据本值为0即可；如使用DTrack2时，本端口值默认值为50105；
        /// </summary>
        [Name("服务器端口")]
        [Tip("如仅接收数据本值为0即可；如使用DTrack2时，本端口值默认值为50105；", "If only data is received, this value can be 0; If DTrack2 is used, the default value of this port is 50105;")]
        public ushort _serverPort = 0;

        /// <summary>
        /// 数据端口
        /// </summary>
        [Name("数据端口")]
        public ushort _dataPort = 5000;

        /// <summary>
        /// 远程类型
        /// </summary>
        [Name("远程类型")]
        [EnumPopup]
        public ERemoteType _remoteType = ERemoteType.Unknow;

        /// <summary>
        /// 远程类型
        /// </summary>
        [Name("远程类型")]
        public enum ERemoteType
        {
            /// <summary>
            /// 未知
            /// </summary>
            [Name("未知")]
            Unknow = 0,

            /// <summary>
            /// DTrack
            /// </summary>
            [Name("DTrack")]
            DTrack,

            /// <summary>
            /// DTrack2
            /// </summary>
            [Name("DTrack2")]
            DTrack2
        }

        /// <summary>
        /// 数据缓存
        /// </summary>
        [Name("数据缓存")]
        public int _dataBufsize = 32768;

        /// <summary>
        /// 数据超时：数据超时时间；单位：微秒；
        /// </summary>
        [Name("数据超时")]
        [Tip("数据超时时间；单位：微秒；", "Data timeout; Unit: microsecond;")]
        public int _dataTimeoutUS = 1000000;

        /// <summary>
        /// 服务超时：服务超时时间；单位：微妙us；
        /// </summary>
        [Name("服务超时")]
        [Tip("服务超时时间；单位：微秒；", "Service timeout; Unit: microsecond;")]
        public int _srvTimeoutUS = 10000000;

        /// <summary>
        /// ART坐标系统
        /// </summary>
        [Name("ART坐标系统")]
        [Tip("ART服务器端使用的坐标系统", "Coordinate system used by ART server")]
        [EnumPopup]
        public ECoordinateSystem _coordinateSystem = ARTHelper.NormalCoordinateSystem;

        private IntPtr instance;

        private object dateLocker = new object();

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => ARTNative.isUDPValid(instance);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            try
            {
                instance = ARTNative.create(ref _serverHost, _serverPort, _dataPort, (int)_remoteType, _dataBufsize, _dataTimeoutUS, _srvTimeoutUS);
                ThreadPool.QueueUserWorkItem(ReceiveData, this);
            }
            catch
            {
                enabled = false;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ARTNative.destory(instance);
            instance = default;
        }

        /// <summary>
        /// 多线程接收数据，通过ART-UDP
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveData(object state)
        {
            var body = new DTrack_Body_Type_d();
            var flyStick = new DTrack_FlyStick_Type_d();
            try
            {
                while (ARTNative.receive(instance))
                {
                    if (!Monitor.TryEnter(dateLocker))
                    {
                        //Debug.Log("临界区进入失败");
                        continue;
                    }
                    try
                    {
                        var numBody = ARTNative.getNumBody(instance);
                        for (int i = 0; i < numBody; i++)
                        {
                            if (ARTNative.getBody(instance, i, ref body) && body.quality >= 0)
                            {
                                var rbState = GetRigidBodyByID(body.id);
                                rbState.Init(body, _coordinateSystem);
                            }
                        }

                        var numFlyStick = ARTNative.getNumFlyStick(instance);
                        for (int i = 0; i < numFlyStick; i++)
                        {
                            if (ARTNative.getFlyStick(instance, i, ref flyStick) && flyStick.quality >= 0)
                            {
                                var rbState = GetFlyStickByID(flyStick.id);
                                rbState.Init(flyStick, _coordinateSystem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(GetType().FullName + ": 接收数据引发异常1.", this);
                        Debug.LogException(ex, this);
                    }
                    finally
                    {
                        Monitor.Exit(dateLocker);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(GetType().FullName + ": 接收数据引发异常2.", this);
                Debug.LogException(ex, this);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 获取姿态
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ARTPose GetPose(EDataType dataType, int id) => GetState(dataType, id)?.Pose;

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseState GetState(EDataType dataType, int id)
        {
            switch (dataType)
            {
                case EDataType.Body: return GetLatestRigidBodyState(id);
                case EDataType.FlyStick: return GetLatestFlyStickState(id);
                default: return default;
            }
        }

        #region Body

        private Dictionary<int, ARTPRigidBodyState> _latestRigidBodyStates = new Dictionary<int, ARTPRigidBodyState>();

        /// <summary>
        /// 获取最后的刚体状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ARTPRigidBodyState GetLatestRigidBodyState(int id)
        {
            ARTPRigidBodyState rbState;

            lock (dateLocker)
            {
                _latestRigidBodyStates.TryGetValue(id, out rbState);
            }

            return rbState;
        }

        private ARTPRigidBodyState GetRigidBodyByID(int id)
        {
            ARTPRigidBodyState returnedState;
            if (_latestRigidBodyStates.ContainsKey(id))
            {
                returnedState = _latestRigidBodyStates[id];
            }
            else
            {
                var newState = new ARTPRigidBodyState
                {
                    Pose = new ARTPose(),
                };

                _latestRigidBodyStates[id] = newState;

                returnedState = newState;
            }

            return returnedState;
        }

        #endregion

        #region FlyStick

        private Dictionary<int, ARTFlyStickState> _latestFlyStickStates = new Dictionary<int, ARTFlyStickState>();

        /// <summary>
        /// 获取所有的FlyStick状态ID
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllFlyStickStateIDs()
        {
            lock (dateLocker)
            {
                return _latestFlyStickStates.Keys.ToList();
            }
        }

        /// <summary>
        /// 获取最后的FlyStick状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ARTFlyStickState GetLatestFlyStickState(int id)
        {
            ARTFlyStickState lastestState;

            lock (dateLocker)
            {
                _latestFlyStickStates.TryGetValue(id, out lastestState);
            }

            return lastestState;
        }

        private ARTFlyStickState GetFlyStickByID(int id)
        {
            ARTFlyStickState returnedState;
            if (_latestFlyStickStates.ContainsKey(id))
            {
                returnedState = _latestFlyStickStates[id];
            }
            else
            {
                var newState = new ARTFlyStickState
                {
                    id = id,
                    Pose = new ARTPose(),
                };

                _latestFlyStickStates[id] = newState;

                returnedState = newState;
            }

            return returnedState;
        }

        #endregion
    }

    /// <summary>
    /// 数据类型：ART网络数据的类型
    /// </summary>
    [Name("数据类型")]
    public enum EDataType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 刚体：具有6自由度信息的刚体
        /// </summary>
        [Name("刚体")]
        [Tip("具有6自由度信息的刚体", "Rigid body with 6-DOF information")]
        Body,

        /// <summary>
        /// FlyStick:具有6自由度信息的FlyStick交互输入设备
        /// </summary>
        [Name("FlyStick")]
        [Tip("具有6自由度信息的FlyStick交互输入设备", "Flystick interactive input device with 6-DOF information")]
        FlyStick,
    }

    /// <summary>
    /// ART姿态
    /// </summary>
    public class ARTPose
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// 方向
        /// </summary>
        public Quaternion Orientation;
    }

    /// <summary>
    /// 基础数据
    /// </summary>
    public abstract class BaseState
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;

        /// <summary>
        /// 姿态
        /// </summary>
        public ARTPose Pose;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="rot"></param>
        /// <param name="coordinateSystem"></param>
        public void Init(double[] loc, double[] rot, ECoordinateSystem coordinateSystem)
        {
            var r = coordinateSystem.GetConvertMatrixToUnity();
            var rotation = r * Matrix3x3.Create(rot, true) * r.inverse;

            Pose.Position = r.MultiplyPoint3x4(new Vector3((float)loc[0], (float)loc[1], (float)loc[2]));
            Pose.Orientation = rotation.rotation;
        }
    }

    /// <summary>
    /// ART刚体状态数据
    /// </summary>
    public class ARTPRigidBodyState : BaseState
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(DTrack_Body_Type_d data, ECoordinateSystem coordinateSystem)
        {
            try
            {
                Init(data.loc, data.rot, coordinateSystem);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    /// <summary>
    /// ART FlyStick状态数据
    /// </summary>
    public class ARTFlyStickState : BaseState
    {
        /// <summary>
        /// 按钮值字典
        /// </summary>
        public Dictionary<int, int> buttons = new Dictionary<int, int>();

        /// <summary>
        /// 摇杆值字典
        /// </summary>
        public Dictionary<int, double> joysticks = new Dictionary<int, double>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(DTrack_FlyStick_Type_d data, ECoordinateSystem coordinateSystem)
        {
            try
            {
                Init(data.loc, data.rot, coordinateSystem);

                //Debug.Log(data.num_button);
                for (int i = 0; i < data.num_button; i++)
                {
                    buttons[i] = data.button[i];
                    //Debug.Log(i.ToString()+"=>按钮:"+ data.button[i]);
                }

                //Debug.Log(data.num_joystick);
                for (int i = 0; i < data.num_joystick; i++)
                {
                    joysticks[i] = data.joystick[i];
                    //Debug.Log(i.ToString() + "=>摇杆:" + data.joystick[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="flystickButton">Flystick按钮枚举</param>
        /// <param name="value"为[-1,1]区间的值></param>
        /// <returns></returns>
        public bool TryGetValue(Enum flystickButton, out double value)
        {
            value = 0;
            if (AttributeCache<ID6df2Attribute>.GetOfField(flystickButton) is ID6df2Attribute attribute)
            {
                var id = attribute.value;
                if (attribute.buttonOrJoystick)
                {
                    if (buttons.TryGetValue(id, out int resultValue))
                    {
                        value = resultValue;
                        return true;
                    }
                }
                else
                {
                    if (joysticks.TryGetValue(id, out double resultValue)
                        && resultValue >= attribute.joystickValueRangeLeft
                        && resultValue <= attribute.joystickValueRangeRight)
                    {
                        value = resultValue;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="flystickButton">Flystick按钮枚举</param>
        /// <param name="value">为[-1,1]区间的值</param>
        /// <param name="deadZone">死区值</param>
        /// <returns></returns>
        public bool TryGetValue(Enum flystickButton, out double value, double deadZone)
        {
            return TryGetValue(flystickButton, out value) && Math.Abs(value) >= deadZone;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="flystickButton">Flystick按钮枚举</param>
        /// <param name="deadZone">死区值</param>
        /// <returns>返回值均为[-1,1]区间的值</returns>
        public double GetValue(Enum flystickButton, double deadZone)
        {
            TryGetValue(flystickButton, out var value, deadZone);
            return value;
        }

        /// <summary>
        /// 尝试获取绝对值
        /// </summary>
        /// <param name="flystickButton">Flystick按钮枚举</param>
        /// <param name="value">为[0,1]区间的值</param>
        /// <param name="deadZone">死区值</param>
        /// <returns></returns>
        public bool TryGetAbsValue(Enum flystickButton, out double value, double deadZone)
        {
            if (TryGetValue(flystickButton, out value))
            {
                value = Math.Abs(value);
                return value >= deadZone;
            }
            value = 0;
            return false;
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="flystickButton">Flystick按钮枚举</param>
        /// <param name="deadZone">死区值</param>
        /// <returns>结果为[0,1]区间的值</returns>
        public double GetAbsValue(Enum flystickButton, double deadZone)
        {
            TryGetAbsValue(flystickButton, out var value, deadZone);
            return value;
        }
    }
}
