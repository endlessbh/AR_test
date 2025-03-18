using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤:步骤组件是步骤片段集合的容器。作为动画描述使用，当播放某个动画的时候可以给动画注解，组件激活后随即切换为完成态。
    /// </summary>
    [ComponentMenu("展示/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Step))]
    [Tip("步骤组件是步骤片段集合的容器。作为动画描述使用，当播放某个动画的时候可以给动画注解，组件激活后随即切换为完成态。", "A step component is a container for a collection of step fragments. It is used as an animation description. When playing an animation, you can annotate the animation. After the component is activated, it will switch to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Step)]
    [RequireState(typeof(NormalState))]
    [DisallowMultipleComponent]
    [KeyNode(nameof(IStep), Title)]
    public class Step : StateComponent<Step>, IStep, ITexture2D
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "步骤";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("展示", typeof(SMSManager))]
        [StateComponentMenu("展示/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(Step))]
        [Tip("步骤组件是步骤片段集合的容器。作为动画描述使用，当播放某个动画的时候可以给动画注解，组件激活后随即切换为完成态。", "A step component is a container for a collection of step fragments. It is used as an animation description. When playing an animation, you can annotate the animation. After the component is activated, it will switch to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateStep(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 描述
        /// </summary>
        [Name("描述(必填)")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string description;

        /// <summary>
        /// 详细描述
        /// </summary>
        [Name("详细描述(可选)")]
        public string detailDescription;

        /// <summary>
        /// 步骤图标
        /// </summary>
        [Name("图标")]
        public Texture2D _texture2D = null;

        /// <summary>
        /// 图标
        /// </summary>
        public Texture2D texture2D { get => _texture2D; set { } } 

        [HideInInspector]
        [NonSerialized]
        public List<StepClip> clips = new List<StepClip>();

        /// <summary>
        /// 步骤组件进入事件
        /// </summary>
        public static event Action<Step, StateData> onStepEntry = null;

        /// <summary>
        /// 步骤组件退出事件
        /// </summary>
        public static event Action<Step, StateData> onStepExit = null;

        #region 生命周期事件

        public override void OnStateCreated()
        {
            base.OnStateCreated();

            description = parent.name;
        }

        public override bool Init(StateData data)
        {
            expanded = true;
            stepState = EStepState.Unfinished;

            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            onStepExit?.Invoke(this, data);

            // 当data在StateWorkClipSet会执行模拟运行，这时候传入参数为空
            // 模拟运行时，不应该改变设置为当前步骤
            if (data == null) return;

            SetCurrent();

            SetUnfinished();

            stepState = EStepState.Active;

            // 使用根跳过前面步骤的动画
            StepGroupHelper.GotoState(parentStep as StepGroup, GetState());
        }

        public override void OnExit(StateData data)
        {
            onStepEntry?.Invoke(this, data);

            if (data != null)
            {
                CheckFinished();
            }

            base.OnExit(data);
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (NeedInStepGroup() && !InStepGroup())
            {
                return false;
            }
            return !string.IsNullOrEmpty(description);
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (NeedInStepGroup() && !InStepGroup())
            {
                return "需放置" + CommonFun.Name(typeof(StepGroup)) + "内!";
            }
            return "";
        }

        /// <summary>
        /// 完成条件规则
        /// </summary>
        [Name("完成条件规则")] 
        [EnumPopup]
        [HideInInspector]
        public EFinishRule _finishRule = EFinishRule.Default;

        /// <summary>
        /// 缺省完成条件
        /// </summary>
        /// <returns></returns>
        protected virtual bool DefaultFinish() => true;

        /// <summary>
        /// 扩展完成条件
        /// </summary>
        public event Func<bool> extensionFinishCondition = null;

        [Name("完成规则")]
        public enum EFinishRule
        {
            [Name("缺省条件")]
            Default,

            [Name("扩展条件")]
            ExtensionCondition,

            [Name("缺省与扩展条件")]
            DefaultAndExtensionCondition,
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            switch (_finishRule)
            {
                case EFinishRule.ExtensionCondition:
                    {
                        return extensionFinishCondition != null ? extensionFinishCondition.Invoke() : false;
                    }
                case EFinishRule.DefaultAndExtensionCondition:
                    {
                        return DefaultFinish() && (extensionFinishCondition != null ? extensionFinishCondition.Invoke() : false);
                    }
                case EFinishRule.Default:
                default:
                    {
                        return DefaultFinish();
                    }
            }
        }

        /// <summary>
        /// 需要在步骤组内
        /// </summary>
        /// <returns></returns>
        protected bool NeedInStepGroup()
        {
            switch (nodeType)
            {
                case ETreeNodeType.Sub:
                case ETreeNodeType.Leaf:
                    return true;
                case ETreeNodeType.Root:
                default:
                    return false;
            }
        }

        /// <summary>
        /// 在步骤组内
        /// </summary>
        /// <returns></returns>
        protected bool InStepGroup() => parent.parent && parent.parent.GetComponent<StepGroup>();

        #endregion

        #region 操作

        public virtual State GetState()
        {
            var clip = clips.FirstOrDefault();
            if (clip)
            {
                return clip.parent;
            }
            else
            {
                return GetComponent<IWorkClip>() != null ? parent : null;
            }
        }

        public virtual bool Skip()
        {
            clips.ForEach(c =>
            {
                if (c)
                {
                    c.parent.OnEntry(null);
                    c.parent.SetProgress(1);
                    c.parent.OnExit(null);
                }
            });
            CheckFinished();
            return true;
        }

        public virtual int count => 1;

        public void SetStateActive(bool active) => parent.active = active;

        public void SkipState(float percent, bool callEntry = true, bool callExit = true)
        {
            clips.ForEach(c =>
            {
                if (c)
                {
                    // 往后跳跃，需要调用进入，用于模拟上一次动画结束状态
                    // 向前跳跃，则不应该调用进入
                    if (callEntry) c.parent.OnEntry(null);

                    if (callExit) c.parent.OnExit(null);

                    if (percent >= 0) c.parent.SetProgress(percent);
                }
            });

            if (callEntry) parent.OnEntry(null);

            if (callExit) parent.OnExit(null);

            if (percent >= 0) parent.SetProgress(percent);
        }

        public void AddClip(StepClip stepClip)
        {
            if (!stepClip || clips.Contains(stepClip)) return;

            clips.Add(stepClip);

            SortClip();
        }

        /// <summary>
        /// 根据广度遍历算法，把离步骤近的片段放在前面
        /// </summary>
        private void SortClip()
        {
            if (!parentStep || clips.Count <= 1) return;

            var sortClips = SMSHelper.GetStateComponentsWithBreadthFirst<StepClip>(parentStep.parent as StateCollection);

            clips.Sort((x, y) => sortClips.IndexOf(x).CompareTo(sortClips.IndexOf(y)));
        }

        #endregion

        #region 选择

        /// <summary>
        /// 查找与状态匹配的步骤
        /// 如果步骤片段所在状态等于输入状态，则步骤为当前选中步骤
        /// </summary>
        /// <param name="inState">输入状态</param>
        public virtual void SetCurrent(State inState)
        {
            if (parent == inState || clips.Exists(clip => clip.parent == inState))
            {
                SetCurrent();
            }
        }

        public virtual void SetCurrent()
        {
            Step root = StepGroupHelper.GetRoot(this);
            if (root)
            {
                root.UnSelect();
            }

            Select();
        }

        public virtual void Select()
        {
            selected = true;

            if (parentStep)
            {
                parentStep.Select();
            }
        }

        public virtual void UnSelect() => selected = false;

        /// <summary>
        /// 获取选中状态列表
        /// </summary>
        /// <param name="includeWorkClip">包含工作片段对象</param>
        /// <returns>选中步骤所在状态列表（从根到叶子）</returns>
        public virtual List<State> GetSelectedStates(bool includeWorkClip = true)
        {
            List<State> states = new List<State>();
            if (selected)
            {
                states.Add(parent);

                // 将第一个片段放在状态列表中
                if (includeWorkClip)
                {
                    var firstClip = clips.FirstOrDefault();
                    if (firstClip)
                    {
                        states.Add(firstClip.parent);
                    }
                }
            }
            return states;
        }

        #endregion

        #region ITreeNodeGraphExtension

        private List<Step> _children = new List<Step>();

        public void AddChildren(params Step[] steps)
        {
            if (steps == null) return;
            foreach (var s in steps)
            {
                s.parentStep = this;

                _children.AddWithDistinct(s);
            }
        }

        public void ClearChildren() => _children.Clear();

        public string displayName => description;

        public virtual Step parentStep { get; set; }

        public Step[] children => _children.ToArray();

        ITreeNode ITreeNode.parent => parentStep;

        ITreeNode[] ITreeNode.children => children;

        ITreeNodeGraph ITreeNodeGraph.parent => parentStep;

        ITreeNodeGraph[] ITreeNodeGraph.children => children;

        public int depth => parentStep ? parentStep.depth + 1 : 0;

        public bool expanded { get; set; } = true;

        public virtual bool selected { get; set; } = false;

        public GUIContent display => new GUIContent(displayName, parent.GetNamePath());

        public bool visible { get; set; } = true;

        public virtual void OnClick()
        {
            SetCurrent();
        }

        public virtual ETreeNodeType nodeType => ETreeNodeType.Leaf;

        #endregion

        #region IStep

        IStep IStep.parent => parentStep;

        IStep[] IStep.children => children;

        public virtual EStepState stepState
        {
            get
            {
                if (_stepState == EStepState.Active)
                {
                    CheckFinished();
                }
                return _stepState;
            }

            set
            {
                _stepState = value;

                var group = parentStep as StepGroup;
                if (group)
                {
                    group.OnStepStateChanged(this, value);
                }
            }
        }

        private EStepState _stepState = EStepState.None;

        public virtual void ResetStepState()
        {
            stepState = EStepState.Unfinished;
            clips.ForEach(c => c.ResetClipState());
        }

        public virtual void CheckFinished()
        {
            if (clips.Count == 0 || clips.All(c => c.clipState == EStepState.Finished))
            {
                if (Finished())
                {
                    stepState = EStepState.Finished;
                }
            }
        }

        #endregion

        #region 事件

        public void OnStepClipStateChanged(StepClip clip, EStepState inState)
        {
            if (clips.Contains(clip))
            {
                if(inState==EStepState.Finished) CheckFinished();
            }
        }

        public void SetFinished()
        {
            stepState = EStepState.Finished;
            clips.ForEach(c=>c.clipState=EStepState.Finished);
        }

        public void SetUnfinished()
        {
            stepState = EStepState.Unfinished;
            clips.ForEach(c => c.clipState = EStepState.Unfinished);
        }

        #endregion
    }
}
