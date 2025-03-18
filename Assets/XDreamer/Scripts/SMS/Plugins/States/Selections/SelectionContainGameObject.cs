using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Selections
{
    /// <summary>
    /// 选择集包含游戏对象:选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。
    /// </summary>
    [ComponentMenu("选择集/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(SelectionContainGameObject))]
    [Tip("选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。", "The selection set contains the game object component, which is a trigger used to judge whether the game object is in the selection set. If the condition holds, it will be switched to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33668)]
    [RequireComponent(typeof(GameObjectSet))]
    public class SelectionContainGameObject : Trigger<SelectionContainGameObject>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "选择集包含游戏对象";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("选择集", typeof(SMSManager))]
        [StateComponentMenu("选择集/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(SelectionContainGameObject))]
        [Tip("选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。", "The selection set contains the game object component, which is a trigger used to judge whether the game object is in the selection set. If the condition holds, it will be switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 规则
        /// </summary>
        [Name("规则")]
        [EnumPopup]
        public EContainRule rule = EContainRule.SelectionContainGameObjectSet;

        /// <summary>
        /// 变化时检测
        /// </summary>
        [Name("变化时检测")]
        public bool checkOnChanged = true;

        /// <summary>
        /// 选择集变量
        /// </summary>
        [Name("选择集变量")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _selectionVariableName;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _selectionVariableName);
        }

        #endregion

        private GameObjectSet gameObjectSet = null;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InitGameObjectSet();

            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (!checkOnChanged)
            {
                Check();
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Selection.selectionChanged -= OnSelectionChanged;

            base.OnExit(data);
        }

        private void InitGameObjectSet()
        {
            if (!gameObjectSet)
            {
                gameObjectSet = GetComponent<GameObjectSet>();
            }   
        }

        /// <summary>
        /// 当选择集变更时回调
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected void OnSelectionChanged(GameObject[] oldSelections, bool flag) => Check();
        
        private void Check()
        {
            if (finished) return;
            switch (rule)
            {
                case EContainRule.SelectionContainGameObjectSet:
                    {
                        finished = gameObjectSet.objects.All(s => Selection.selections.Contains(s));
                        break;
                    }
                case EContainRule.Equal:
                    {
                        if (Selection.selections.Length == gameObjectSet.objects.Count)
                        {
                            finished = gameObjectSet.objects.All(s => Selection.selections.Contains(s));
                        }
                        break;
                    }
                case EContainRule.AnyGameObjectOfSelectionNotInGameObjectSet:
                    {
                        finished = gameObjectSet.objects.All(s => !Selection.selections.Contains(s));
                        break;
                    }
                case EContainRule.GameObjectSetContainSelection:
                    {
                        finished = Selection.selections.All(s => gameObjectSet.objects.Contains(s));
                        break;
                    }
                case EContainRule.LeastOneGameObjectOfSelectionInGameObjectSet:
                    {
                        finished = gameObjectSet.objects.Any(s => Selection.selections.Contains(s));
                        break;
                    }
                case EContainRule.Always: finished = true; break;
                default: break;
            }

            if (finished)
            {
                _selectionVariableName.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectToString(Selection.selection));
            }
        }
    }

    /// <summary>
    /// 选择集规则
    /// </summary>
    [Name("选择集规则")]
    public enum EContainRule
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("总是成立")]
        Always = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 选择集包含游戏对象集合：游戏对象集合中每个游戏对象都在选择集中，则成立；即游戏对象集合是选择集的子集；
        /// </summary>
        [Name("选择集包含游戏对象集合")]
        [Tip("游戏对象集合中每个游戏对象都在选择集中，则成立；即游戏对象集合是选择集的子集；", "If each game object in the game object set is in the selection set, it is established; That is, the game object set is a subset of the selection set;")]
        SelectionContainGameObjectSet = 1,

        /// <summary>
        /// 相等：选择集中每一个游戏对象都在游戏对象集合中，并且游戏对象集合中每一个游戏对象都在选择集中，则成立；即游戏对象集合与选择集相等；
        /// </summary>
        [Name("相等")]
        [Tip("选择集中每一个游戏对象都在游戏对象集合中，并且游戏对象集合中每一个游戏对象都在选择集中，则成立；即游戏对象集合与选择集相等；", "It is true that each game object in the selection set is in the game object set, and each game object in the game object set is in the selection set; That is, the game object set is equal to the selection set;")]
        Equal = 2,

        /// <summary>
        /// 选择集中任意游戏对象不在游戏对象集合中：选择集中任意一个游戏对象有不在游戏对象集合中，则成立；即游戏对象集合与选择集无交集；
        /// </summary>
        [Name("选择集中任意游戏对象不在游戏对象集合中")]
        [Tip("选择集中任意一个游戏对象均不在游戏对象集合中，则成立；即游戏对象集合与选择集无交集；", "If any game object in the selection set is not in the game object set, it is true; That is, there is no intersection between the game object set and the selection set;")]
        AnyGameObjectOfSelectionNotInGameObjectSet = 3,

        /// <summary>
        /// 选择集中任意游戏对象在游戏对象集合中：选择集中任意一个游戏对象有在游戏对象集合中，则成立；即选择集与游戏对象集合有交集；
        /// </summary>
        [Name("选择集中至少一个游戏对象在游戏对象集合中")]
        [Tip("选择集中至少有一个游戏对象在游戏对象集合中，则成立；即选择集与游戏对象集合有交集；", "If at least one game object in the selection set is in the game object set, it is true; That is, there is an intersection between the selection set and the game object set;")]
        LeastOneGameObjectOfSelectionInGameObjectSet = 4,

        /// <summary>
        /// 游戏对象集合包含选择集：选择集中每个游戏对象都在游戏对象集合中，则成立；即选择集是游戏对象集合的子集；
        /// </summary>
        [Name("游戏对象集合包含选择集")]
        [Tip("选择集中每个游戏对象都在游戏对象集合中，则成立；即选择集是游戏对象集合的子集；", "If each game object in the selection set is in the game object set, it is true; That is, the selection set is a subset of the game object set;")]
        GameObjectSetContainSelection = 5,
    }
}

