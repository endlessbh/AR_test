using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.States.Motions;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 路径动画
    /// </summary>
    [Name("路径动画")]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Path)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public sealed class PathAnimation : PlayableContent
    {
        #region 移动-位置

        /// <summary>
        /// 移动路径补间类型
        /// </summary>
        [Group("移动路径设置", textEN = "Move Path Settings", needBoundBox = true, defaultIsExpanded = true)]
        [Name("移动路径补间类型")]
        [EnumPopup]
        public ELineType _movePathType = ELineType.Liner;

        [Name("移动路径变换类型")]
        [EnumPopup]
        public EPathTransformType _movePathTransformType = EPathTransformType.Transform;

        [Name("移动变换排序规则")]
        [HideInSuperInspector(nameof(_movePathTransformType), EValidityCheckType.NotEqual, EPathTransformType.TransformChildren)]
        [EnumPopup]
        public EPathTransformSortRule _moveTransformSortRule = EPathTransformSortRule.NameAsc;

        [Name("移动路径")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [FormerlySerializedAs(nameof(movePath))]
        public List<Transform> _movePath = new List<Transform>();

        public bool validMovePath => _movePath.Count >= 1;

        private List<Transform> movePath => CacheMovePath();


        private List<Transform> movePathCache = new List<Transform>();

        private List<Transform> CacheMovePath()
        {
            movePathCache.Clear();
            switch (_movePathTransformType)
            {
                case EPathTransformType.Transform:
                    {
                        movePathCache.AddRange(_movePath);
                        break;
                    }
                case EPathTransformType.TransformChildren:
                    {
                        foreach (var t in _movePath)
                        {
                            foreach (Transform child in t)
                            {
                                movePathCache.Add(child);
                            }
                        }
                        PathTransformSortRuleHelper.Sort(movePathCache, _moveTransformSortRule);
                        break;
                    }
            }
            return movePathCache;
        }

        [Name("移动完美间距")]
        public bool _movePrettySpace = true;

        [Name("移动间隔类型")]
        [HideInSuperInspector(nameof(_movePrettySpace), EValidityCheckType.True)]
        [EnumPopup]
        public ESpaceType moveSpaceType = ESpaceType.Perent;

        [Name("移动间距值")]
        [HideInSuperInspector(nameof(_movePrettySpace), EValidityCheckType.True)]
        public double _moveSpaceValue = 0.1f;

        #endregion

        #region 视图-方向

        [Group("视图路径设置", textEN = "View Path Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("视图规则")]
        [Tip("对象在移动路径上发生移动时，其Z轴目标方向的更新处理规则；", "When the object moves on the moving path, the update processing rules of its z-axis target direction;")]
        [EnumPopup]
        public EViewRule _viewRule = EViewRule.MovePath;

        [Name("视图路径补间类型")]
        [HideInSuperInspector(nameof(_viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public ELineType _viewPathType = ELineType.Liner;

        [Name("视图路径变换类型")]
        [HideInSuperInspector(nameof(_viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public EPathTransformType _viewPathTransformType = EPathTransformType.Transform;

        [Name("视图变换排序规则")]
        [HideInSuperInspector(nameof(_viewPathTransformType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EPathTransformType.TransformChildren, nameof(_viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public EPathTransformSortRule _viewTransformSortRule = EPathTransformSortRule.NameAsc;

        [Name("视图路径")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [HideInSuperInspector(nameof(_viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [FormerlySerializedAs(nameof(viewPath))]
        public List<Transform> _viewPath = new List<Transform>();

        public bool validViewPath => _viewPath.Count >= 1;

        public List<Transform> viewPath => CacheViewPath();


        private List<Transform> viewPathCache = new List<Transform>();

        private List<Transform> CacheViewPath()
        {
            viewPathCache.Clear();
            switch (_viewPathTransformType)
            {
                case EPathTransformType.Transform:
                    {
                        viewPathCache.AddRange(_viewPath);
                        break;
                    }
                case EPathTransformType.TransformChildren:
                    {
                        foreach (var t in _viewPath)
                        {
                            foreach (Transform child in t)
                            {
                                viewPathCache.Add(child);
                            }
                        }
                        PathTransformSortRuleHelper.Sort(viewPathCache, _viewTransformSortRule);
                        break;
                    }
            }
            return viewPathCache;
        }

        [Name("视图前移系数")]
        [Range(0.0001f, 0.1f)]
#if XDREAMER_EDITION_DEVELOPER
        [HideInSuperInspector(nameof(_viewRule), EValidityCheckType.NotEqual, EViewRule.MovePath)]
#else
        [HideInSuperInspector]
#endif
        public float viewForwardCoefficient = 0.01f;


        [Name("视图完美间距")]
        public bool _viewPrettySpace = true;

        [Name("视图间隔类型")]
        [HideInSuperInspector(nameof(_viewPrettySpace), EValidityCheckType.True)]
        [EnumPopup]
        public ESpaceType _viewSpaceType = ESpaceType.Perent;

        [Name("视图间距值")]
        [HideInSuperInspector(nameof(_viewPrettySpace), EValidityCheckType.True)]
        public double _viewSpaceValue = 0.1f;

        #endregion

        #region 检查器窗口

        /// <summary>
        /// 批量处理对象
        /// </summary>
        [Name("批量处理对象")]
        [HideInSuperInspector]
        public Transform _batchGameObject;

        /// <summary>
        /// 包含:为True时，将 批处理游戏对象<see cref="_batchGameObject"/> 添加到对象集数组(包括移动路径<see cref="_movePath"/>、视图路径<see cref="_viewPath"/>)中；无则添加，有则不变；
        /// </summary>
        [Name("包含")]
        [Tip("为True时，将 批处理游戏对象 添加到对象集数组(包括移动路径、视图路径、移动对象列表)中；无则添加，有则不变；", "When true, add the batch game object to the object set array (including moving path and view path); If there is none, it will be added, and if there is one, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool _include = true;

        /// <summary>
        /// 成员:为True时，将 批处理游戏对象<see cref="_batchGameObject"/> 的子级成员全部添加到对象集数组(包括移动路径<see cref="_movePath"/>、视图路径<see cref="_viewPath"/>)中；无则添加，缺则补漏，有则不变；
        /// </summary>
        [Name("成员")]
        [Tip("为True时，将 批处理游戏对象 的子级成员全部添加到对象集数组(包括移动路径、视图路径)中；无则添加，缺则补漏，有则不变；", "When true, all child members of batch game objects are added to the object set array (including moving path and view path); If there is no, add, if there is a lack, make up the leak, and if there is, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool _chileren = false;

        #endregion

        private bool Init()
        {
            _movePath.RemoveAll(t => !t);
            _viewPath.RemoveAll(t => !t);
            viewPercent.Init(this);
            return true;
        }

        internal static Vector3[] GetFullPath(Vector3[] path, ELineType pathPatchType)
        {
            return XGizmos.PathControlPointGenerator(pathPatchType, path);
        }

        private Vector3[] GetFullMovePath() => GetFullPath(GetMovePath(), _movePathType);

        private Vector3[] GetMovePath() => GetPath(movePath);

        private Vector3[] GetFullViewPath() => GetFullPath(GetViewPath(), _viewPathType);

        private Vector3[] GetViewPath() => GetPath(viewPath);

        private Vector3[] GetPath(IList<Transform> transforms)
        {
            try
            {
                List<Vector3> list = new List<Vector3>();
                foreach (var t in transforms)
                {
                    if (t) list.Add(t.position);
                }

                switch (list.Count)
                {
                    case 0: return null;
                    case 1:
                        {
                            list.Add(list[0]);
                            break;
                        }
                }
                return list.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private Percent viewPercent = new Percent();

        /// <summary>
        /// 间隔类型
        /// </summary>
        [Name("间隔类型")]
        public enum ESpaceType
        {
            [Name("百分比")]
            Perent = 0,

            [Name("时间")]
            Time,

            [Name("距离")]
            Distance,
        }
        
        [Name("转场")]
        public enum ETransition
        {
            [Name("无")]
            None,

            [Name("渐入")]
            FadeIn,

            [Name("循环")]
            Loop,

            [Name("渐出")]
            FadeOut,
        }

        /// <summary>
        /// 转场效果
        /// </summary>
        [Group("运动设置", textEN = "Motion Settings")]
        [Name("转场效果")]
        [EnumPopup]
        public ETransition _transition = ETransition.FadeIn;

        /// <summary>
        /// 循环转场效果
        /// </summary>
        [Name("循环转场效果")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_loopType), EValidityCheckType.Equal, ELoopType.None)]
        public ETransition _transitionLoop = ETransition.Loop;

        /// <summary>
        /// 移动对象列表
        /// </summary>
        [Name("移动对象列表")]
        public List<Transform> _moveTransforms = new List<Transform>();

        protected override void OnEnable()
        {
            Init();

            base.OnEnable();
        }

        private ETransition GetTransition(double percent)
        {
            var transition = _transition;

            if (loop)
            {
                if (percent > 1)
                {
                    transition = _transitionLoop;
                }
            }
            return transition;
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            var transition = GetTransition(percent.percent);

            switch (_viewRule)
            {
                case EViewRule.None:
                    {
                        if (validMovePath) OnUpdatePosition(_moveTransforms, GetFullMovePath(), percent, _movePathType, transition);
                        break;
                    }
                case EViewRule.MovePath:
                    {
                        if (!validMovePath) break;
                        var movePath01 = GetFullMovePath();
                        OnUpdatePosition(_moveTransforms, movePath01, percent, _movePathType, transition);

                        OnUpdateView(_moveTransforms, movePath01, viewPercent.Update(percent.percent01OfWorkCurve + viewForwardCoefficient), _movePathType, transition);
                        break;
                    }
                case EViewRule.ViewPath:
                    {
                        if (validMovePath) OnUpdatePosition(_moveTransforms, GetFullMovePath(), percent, _movePathType, transition);
                        if (validViewPath) OnUpdateView(_moveTransforms, GetFullViewPath(), percent, _viewPathType, transition);
                        break;
                    }
            }
        }

        private double GetSpacePercent(int count, bool prettySpace, ESpaceType spaceType, double spaceValue, Vector3[] path)
        {
            if (count <= 1) return 0;
            if (prettySpace) return 1f / (count - 1);

            switch (spaceType)
            {
                case ESpaceType.Time:
                    {
                        return MathX.ApproximatelyZero(timeLength) ? 0 : spaceValue / timeLength;
                    }
                case ESpaceType.Distance:
                    {
                        var length = MathU.PathLength(path);
                        return MathX.ApproximatelyZero(length) ? 0 : spaceValue / length;
                    }
                case ESpaceType.Perent:
                default:
                    {
                        return spaceValue;
                    }
            }
        }

        private void OnUpdatePosition(List<Transform> transformList, Vector3[] path, Percent percent, ELineType pathPatchType, ETransition transition)
        {
            try
            {
                switch (transition)
                {
                    case ETransition.FadeIn:
                        {
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _movePrettySpace, moveSpaceType, _moveSpaceValue, path);
                            for (int i = 0; i < count; i++)
                            {
                                var p = percent.percentOfWorkCurve - spacePercent * i;
                                transformList[i].transform.position = Move.Interp(pathPatchType, path, MathX.Clamp01(p));
                            }
                            break;
                        }
                    case ETransition.FadeOut:
                        {
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _movePrettySpace, moveSpaceType, _moveSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = percent.percentOfWorkCurve + spacePercent * i;
                                transformList[j].transform.position = Move.Interp(pathPatchType, path, MathX.Clamp01(p));
                            }
                            break;
                        }
                    case ETransition.Loop:
                        {
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _movePrettySpace, moveSpaceType, _moveSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = percent.percentOfWorkCurve + spacePercent * i;
                                transformList[j].transform.position = Move.Interp(pathPatchType, path, Percent.Loop01(p));
                            }
                            break;
                        }
                    default:
                        {
                            var movePoint = Move.Interp(pathPatchType, path, percent.percent01OfWorkCurve);
                            foreach (var transform in transformList)
                            {
                                transform.position = movePoint;
                            }
                            break;
                        }
                }

            }
            catch { }
        }

        private void OnUpdateView(List<Transform> transformList, Vector3[] path, Percent percent, ELineType pathPatchType, ETransition transition)
        {
            try
            {
                switch (transition)
                {
                    case ETransition.FadeIn:
                        {
                            var willEnd = percent.percent > 1;
                            var percentage = percent.percentOfWorkCurve < viewForwardCoefficient ? percent.percentOfWorkCurve + (willEnd ? 1 : 0) : percent.percentOfWorkCurve;
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _viewPrettySpace, _viewSpaceType, _viewSpaceValue, path);

                            for (int i = 0; i < count; i++)
                            {
                                var p = percentage - spacePercent * i;
                                if (p < viewForwardCoefficient) p = viewForwardCoefficient;
                                else if (p > 1) p = 1 + viewForwardCoefficient;

                                transformList[i].transform.LookAt(Move.Interp(pathPatchType, path, p));
                            }
                            break;
                        }
                    case ETransition.FadeOut:
                        {
                            var willEnd = percent.percent >= 1;
                            var percentage = percent.percentOfWorkCurve < viewForwardCoefficient ? percent.percentOfWorkCurve + (willEnd ? 1 : 0) : percent.percentOfWorkCurve;
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _viewPrettySpace, _viewSpaceType, _viewSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = percentage + spacePercent * i;
                                if (p < viewForwardCoefficient) p = viewForwardCoefficient;
                                else if (p > 1) p = 1 + viewForwardCoefficient;
                                transformList[j].transform.LookAt(Move.Interp(pathPatchType, path, p));
                            }
                            break;
                        }
                    case ETransition.Loop:
                        {
                            var count = transformList.Count;
                            var spacePercent = GetSpacePercent(count, _viewPrettySpace, _viewSpaceType, _viewSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = MathX.DecimalPart(percent.percent01OfWorkCurve + spacePercent * i);
                                if (p >= viewForwardCoefficient)
                                {
                                    transformList[j].transform.LookAt(Move.Interp(pathPatchType, path, p));
                                }
                            }
                            break;
                        }
                    default:
                        {
                            var lookAtPoint = Move.Interp(pathPatchType, path, percent.percent01OfWorkCurve);
                            foreach (var transform in transformList)
                            {
                                transform.LookAt(lookAtPoint);
                            }
                            break;
                        }
                }
            }
            catch { }
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            var colorBK = Gizmos.color;
            var keyPointColor = UnityEngine.Color.magenta;
            var pathColor = UnityEngine.Color.green;
            var r = 0.1f;
            try
            {
                Gizmos.color = keyPointColor;
                foreach (var p in GetMovePath())
                {
                    Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                }
                XGizmos.DrawPath(_movePathType, GetMovePath(), pathColor);

                switch (_viewRule)
                {
                    case EViewRule.ViewPath:
                        {
                            Gizmos.color = keyPointColor;
                            foreach (var p in GetViewPath())
                            {
                                Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                            }

                            XGizmos.DrawPath(_viewPathType, GetViewPath(), pathColor);
                            break;
                        }
                }

            }
            catch { }
            finally
            {
                Gizmos.color = colorBK;
            }
#endif
        }
    }
}
