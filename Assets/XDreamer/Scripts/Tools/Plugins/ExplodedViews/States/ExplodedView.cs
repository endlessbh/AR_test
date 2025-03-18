using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginTools.ExplodedViews.States
{
    /// <summary>
    /// 爆炸图:对指定的游戏对象集合执行爆炸效果
    /// </summary>
    [Serializable]
    [Name(Title, nameof(ExplodedView))]
    [Tip("对指定的游戏对象集合执行爆炸效果", "Performs an explosion effect on the specified collection of GameObjects")]
    [ComponentMenu("动作/" + Title, typeof(ToolsExtensionManager))]
    [XCSJ.Attributes.Icon(EIcon.ExplodedView)]
    [RequireComponent(typeof(GameObjectSet))]
    public class ExplodedView : WorkClip<ExplodedView>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "爆炸图";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ExplodedView))]
        [Tip("对指定的游戏对象集合执行爆炸效果", "Performs an explosion effect on the specified collection of GameObjects")]
        [StateLib("动作", typeof(ToolsExtensionManager))]
        [StateComponentMenu("动作/" + Title, typeof(ToolsExtensionManager))]
        [XCSJ.Attributes.Icon(EIcon.Scale)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        #region 爆炸设置

        /// <summary>
        /// 爆炸数据
        /// </summary>
        [Name("爆炸数据")]
        [Readonly]
        public List<ExplodeData> datas = new List<ExplodeData>();

        /// <summary>
        /// 爆炸视图类型
        /// </summary>
        [Name("爆炸视图类型")]
        [EnumPopup]
        public EExplodeType explodeType = EExplodeType.Point;

        /// <summary>
        /// 排序规则
        /// </summary>
        [Name("排序规则")]
        [EnumPopup]
        public ESortRule _sortRule = ESortRule.DistanceAsc;

        /// <summary>
        /// 中心类型
        /// </summary>
        [Name("中心类型")]
        [EnumPopup]
        public ECenterType centerType = ECenterType.TransformPosition;

        /// <summary>
        /// 中心位置:爆炸中心的世界坐标
        /// </summary>
        [Name("中心位置")]
        [Tip("爆炸中心的世界坐标", "World coordinates of the explosion center")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.NotEqual, ECenterType.Postion)]
        public Vector3 centerPosition = Vector3.zero;

        /// <summary>
        /// 中心变换:通过中心变换获取爆炸中心的世界坐标
        /// </summary>
        [Name("中心变换")]
        [Tip("通过中心变换获取爆炸中心的世界坐标", "The world coordinates of the explosion center are obtained through the center transformation")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.Less | EValidityCheckType.Or, ECenterType.TransformPosition, nameof(centerType), EValidityCheckType.GreaterEqual, ECenterType.BoundsCenter)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform centerTransofrm;

        /// <summary>
        /// 中心偏移量:用于额外纠正爆炸中心的偏移量
        /// </summary>
        [Name("中心偏移量")]
        [Tip("用于额外纠正爆炸中心的偏移量", "Used to additionally correct the offset of the explosion center")]
        public Vector3 centerOffset = Vector3.zero;

        /// <summary>
        /// 方向类型
        /// </summary>
        [Name("方向类型")]
        [EnumPopup]
        public EDirectionType directionType = EDirectionType.Vector;

        /// <summary>
        /// 方向向量
        /// </summary>
        [Name("方向向量")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.NotEqual, EDirectionType.Vector)]
        public Vector3 directionVector = Vector3.right;

        /// <summary>
        /// 方向变换
        /// </summary>
        [Name("方向变换")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.Equal, EDirectionType.Vector)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform directionTransform;

        /// <summary>
        /// 增量间隔值:爆炸计算时，对象包围盒更新爆炸时每次的增量间隔距离值；
        /// </summary>
        [Name("增量间隔值")]
        [Tip("爆炸计算时，对象包围盒更新爆炸时每次的增量间隔距离值；", "During explosion calculation, the object bounding box updates the incremental interval distance value of each explosion;")]
        [Range(0.001f, 1)]
        public float deltaIntervalValue = 0.01f;

        /// <summary>
        /// 最小间隔值:爆炸完成后，任意两个对象包围盒之间的最小间隔距离
        /// </summary>
        [Name("最小间隔值")]
        [Tip("爆炸完成后，任意两个对象包围盒之间的最小间隔距离", "The minimum distance between any two object bounding boxes after the explosion is completed")]
        [Range(0.001f, 1)]
        public float minIntervalValue = 0.02f;

        /// <summary>
        /// 爆炸倍数:可用于将计算的爆炸结果做额外爆炸的倍数值
        /// </summary>
        [Name("爆炸倍数")]
        [Tip("可用于将计算的爆炸结果做额外爆炸的倍数值", "It can be used to calculate the explosion results as times of additional explosion")]
        [Range(0.001f, 5)]
        public float explodeMultiple = 1;

        #endregion

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        public List<GameObject> objects => gameObjectSet.objects;

        /// <summary>
        /// 爆炸中心
        /// </summary>
        public Vector3 center
        {
            get
            {
                switch (centerType)
                {
                    case ECenterType.Postion: return centerPosition;
                    case ECenterType.TransformPosition: return centerTransofrm.position;
                    case ECenterType.TransformBoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的变换包围盒中心");
                        }
                    case ECenterType.BoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, objects))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                        }
                    default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸中心
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private Vector3 GetCenter(List<GameObject> objects)
        {
            switch (centerType)
            {
                case ECenterType.Postion: return centerPosition;
                case ECenterType.TransformPosition: return centerTransofrm.position;
                case ECenterType.TransformBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的变换包围盒中心");
                    }
                case ECenterType.BoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, objects))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                    }
                default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
            }
        }

        /// <summary>
        /// 爆炸方向
        /// </summary>
        public Vector3 direction
        {
            get
            {
                switch (directionType)
                {
                    case EDirectionType.Vector: return directionVector;
                    case EDirectionType.TransformX: return directionTransform.right;
                    case EDirectionType.TransformY: return directionTransform.up;
                    case EDirectionType.TransformZ: return directionTransform.forward;
                    case EDirectionType.CenterToTransform: return center - directionTransform.position;
                    default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸方向
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        private Vector3 GetDirection(Vector3 center)
        {
            switch (directionType)
            {
                case EDirectionType.Vector: return directionVector;
                case EDirectionType.TransformX: return directionTransform.right;
                case EDirectionType.TransformY: return directionTransform.up;
                case EDirectionType.TransformZ: return directionTransform.forward;
                case EDirectionType.CenterToTransform: return center - directionTransform.position;
                default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
            }
        }

        /// <summary>
        /// 进入激活态
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            //获取基础信息
            var objects = this.objects;
            var c = GetCenter(objects);
            var dir = GetDirection(c);

            //清空数据
            datas.Clear();

            //初始化数据
            objects.ForEach(go =>
            {
                if (go)
                {
                    datas.Add(new ExplodeData(go.transform));
                }
            });

            //模拟爆炸
            datas = ExplodedViewHelper.Explode(explodeType, datas, c, dir, deltaIntervalValue, minIntervalValue, _sortRule);
        }

        /// <summary>
        /// 退出激活态
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            datas.ForEach(data =>
            {
                data.UpatePercent(percent.percent01OfWorkCurve * explodeMultiple);
                data.UpdateTranform();
            });
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch (data.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        datas.ForEach(d => d.Recover());
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (centerType)
            {
                case ECenterType.Postion: break;
                case ECenterType.BoundsCenter: break;
                default: return centerTransofrm;
            }
            switch (directionType)
            {
                case EDirectionType.Vector: break;
                default: return directionTransform;
            }
            return base.DataValidity();
        }
    }
}
