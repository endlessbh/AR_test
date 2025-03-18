using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.GameObjects
{
    /// <summary>
    /// 游戏对象集合
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectSet))]
    [Tip("游戏对象集合", "Game object set")]
    [XCSJ.Attributes.Icon(EIcon.Models)]
    [DisallowMultipleComponent]
    public class GameObjectSet : ObjectSet<GameObjectSet, GameObject>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象集合";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, "GameObjectSet")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("游戏对象集合", "Game object set")]
        public static State CreateGameObjectSet(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 运行时查找对象:;在当前状态组件在进入前执行本参数的处理；
        /// </summary>
        [Name("运行时查找对象")]
        [Tip("程序运行时使用参数值动态查找游戏对象;在当前状态组件在进入前执行本参数的处理；", "When the program is running, use the parameter value to dynamically find the game object; In the current state, the component executes the processing of this parameter before entering;")]
        [HideInSuperInspector]
        public bool findObjectInRuntime = false;

        /// <summary>
        /// 选择类型
        /// </summary>
        [Name("选择类型")]
        [HideInSuperInspector]
        [EnumPopup]
        public ESelectionType selectionType = ESelectionType.GameObject;

        /// <summary>
        /// 批量处理对象
        /// </summary>
        [Name("批量处理对象")]
        [HideInSuperInspector]
        public GameObject batchGameObject;

        /// <summary>
        /// 包含：为True，将 批处理游戏对象 添加到对象集数组中；无则添加，有则不变；
        /// </summary>
        [Name("包含")]
        [Tip("为True，将 批处理游戏对象 添加到对象集数组中；无则添加，有则不变；", "If true, add the batch game object to the object set array; If there is none, it will be added, and if there is one, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool include = true;

        /// <summary>
        /// 成员:为True，将 批处理游戏对象 的子级成员全部添加到对象集数组中；无则添加，缺则补漏，有则不变；
        /// </summary>
        [Name("成员")]
        [Tip("为True，将 批处理游戏对象 的子级成员全部添加到对象集数组中；无则添加，缺则补漏，有则不变；", "If it is true, all child members of the batch game object are added to the object set array; If there is no, add, if there is a lack, make up the leak, and if there is, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool children = false;

        /// <summary>
        /// 选择标签
        /// </summary>
        [Name("选择标签")]
        [HideInSuperInspector]
        public string selectedTag;

        /// <summary>
        /// 选择层
        /// </summary>
        [Name("选择层")]
        [HideInSuperInspector]
        public int selectedLayer = 0;

        /// <summary>
        /// 查找名称
        /// </summary>
        [Name("查找名称")]
        [HideInSuperInspector]
        public string findName = "";

        /// <summary>
        /// 名称比较规则
        /// </summary>
        [Name("名称比较规则")]
        [HideInSuperInspector]
        [EnumPopup]
        public ECompareNameRule compareNameRule = ECompareNameRule.Equal;

        /// <summary>
        /// 名称是变量
        /// </summary>
        [Name("名称是变量")]
        [HideInSuperInspector]
        public bool nameIsVariable = false;

        /// <summary>
        /// 变量:程序运行时将变量转为游戏对象
        /// </summary>
        [Name("变量")]
        [Tip("程序运行时将变量转为游戏对象", "Turn variables into game objects when the program runs")]
        [HideInSuperInspector]
        public string variable;

        /// <summary>
        /// 包含非激活对象:程序运行时将变量转为游戏对象
        /// </summary>
        [Name("包含非激活对象")]
        [Tip("程序运行时将变量转为游戏对象", "Turn variables into game objects when the program runs")]
        [HideInSuperInspector]
        public bool includeInactive = true;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        public override List<GameObject> objects
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) CacheObjects();
#endif
                return objectsCache;
            }
        }

        private List<GameObject> objectsCache = new List<GameObject>();

        private void CacheObjects()
        {
            objectsCache.Clear();
            switch (selectionType)
            {
                case ESelectionType.GameObjectChildren:
                    {
                        objectsCache.AddRangeWithDistinct(FindGameObjectWithRule());
                        break;
                    }
                default:
                    {
                        objectsCache.AddRange(_objects);
                        objectsCache.AddRangeWithDistinct(FindGameObjectWithRule());
                        break;
                    }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            if (includeSelf && data.gameObject) objects.AddWithDistinct(data.gameObject);
            CacheObjects();
            return base.Init(data);
        }

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);

            if (findObjectInRuntime)
            {
                CacheObjects();
            }
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            bool tmpDataValid = false;
            switch (selectionType)
            {
                case ESelectionType.Tag:
                    {
                        tmpDataValid = !string.IsNullOrEmpty(selectedTag);
                        break;
                    }
                case ESelectionType.Layer:
                    {
                        tmpDataValid = selectedLayer >= 0;
                        break;
                    }
                case ESelectionType.Name:
                    {
                        tmpDataValid = !string.IsNullOrEmpty(findName);
                        break;
                    }
                case ESelectionType.TagAndLayer:
                    {
                        tmpDataValid = !string.IsNullOrEmpty(selectedTag) && selectedLayer >= 0;
                        break;
                    }
                case ESelectionType.TagAndName:
                    {
                        tmpDataValid = !string.IsNullOrEmpty(selectedTag) && !string.IsNullOrEmpty(findName);
                        break;
                    }
                case ESelectionType.LayerAndName:
                    {
                        tmpDataValid = selectedLayer >= 0 && !string.IsNullOrEmpty(findName);
                        break;
                    }
                case ESelectionType.Variable:
                    {
                        tmpDataValid = !string.IsNullOrEmpty(variable);
                        break;
                    }
                case ESelectionType.Selection:
                    {
                        tmpDataValid = true;
                        break;
                    }
                default:
                    {
                        tmpDataValid = base.DataValidity();
                        break;
                    }
            }
            return tmpDataValid || base.DataValidity();
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            string selectionTypeName = CommonFun.Name(selectionType);
            string addStr = "";
            switch (selectionType)
            {
                case ESelectionType.GameObject:
                    {
                        addStr = base.ToFriendlyString();
                        break;
                    }
                case ESelectionType.Tag:
                    {
                        addStr = selectedTag;
                        break;
                    }
                case ESelectionType.Layer:
                    {
                        addStr = LayerMask.LayerToName(selectedLayer);
                        break;
                    }
                case ESelectionType.Name:
                    {
                        addStr = findName;
                        break;
                    }
                case ESelectionType.TagAndLayer:
                    {
                        addStr = selectedTag + "和" + LayerMask.LayerToName(selectedLayer);
                        break;
                    }
                case ESelectionType.TagAndName:
                    {
                        addStr = selectedTag + "和" + findName;
                        break;
                    }
                case ESelectionType.LayerAndName:
                    {
                        addStr = LayerMask.LayerToName(selectedLayer) + "和" + findName;
                        break;
                    }
                case ESelectionType.Variable:
                    {
                        addStr = variable;
                        break;
                    }
                case ESelectionType.GameObjectChildren:
                default:
                    {
                        addStr = base.ToFriendlyString();
                        break;
                    }
            }
            return selectionTypeName + ":" + addStr;
        }

        private GameObject[] FindGameObjectWithRule()
        {
            switch (selectionType)
            {
                case ESelectionType.GameObject: break;
                case ESelectionType.Tag:
                    {
                        return FindGameObjectsWithFunc(go => go.tag == selectedTag, includeInactive);
                    }
                case ESelectionType.Layer:
                    {
                        return FindGameObjectsWithLayer(selectedLayer, includeInactive);
                    }
                case ESelectionType.Name:
                    {
                        if (GetFindName(out string tmpName))
                        {
                            return FindGameObjectsWithName(tmpName, compareNameRule, includeInactive);
                        }
                        break;
                    }
                case ESelectionType.TagAndLayer:
                    {
                        return FindGameObjectsWithFunc(go => go.tag == selectedTag && go.layer == selectedLayer, includeInactive);
                    }
                case ESelectionType.TagAndName:
                    {
                        if (GetFindName(out string tmpName))
                        {
                            return FindGameObjectsWithTagAndName(selectedTag, tmpName, compareNameRule, includeInactive);
                        }
                        break;
                    }
                case ESelectionType.LayerAndName:
                    {
                        if (GetFindName(out string tmpName))
                        {
                            return FindGameObjectsWithLayerAndName(selectedLayer, tmpName, compareNameRule, includeInactive);
                        }
                        break;
                    }
                case ESelectionType.Variable:
                    {
                        if (ScriptManager.TryGetGlobalVariableValue(variable, out var variableValue))
                        {
                            var go = CommonFun.StringToGameObject(variableValue.ObjectToString());
                            if (go)
                            {
                                return new GameObject[1] { go };
                            }
                        }
                        break;
                    }
                case ESelectionType.GameObjectChildren:
                    {
                        return FindGameObjectChildren(_objects, includeInactive);
                    }
                case ESelectionType.GameObjectAllChildren:
                    {
                        var list = new List<GameObject>();
                        foreach (var go in _objects)
                        {
                            if (go) list.AddRange(CommonFun.GetAllChildrenGameObject(go, false));
                        }
                        return list.ToArray();
                    }
                case ESelectionType.GameObjectAndAllParents:
                    {
                        var list = new List<GameObject>();
                        foreach (var go in _objects)
                        {
                            if (go) list.AddRange(CommonFun.GetParentsGameObject(go, true));
                        }
                        return list.ToArray();
                    }
                case ESelectionType.Selection:
                    {
                        return Selection.selections;
                    }
                default: break;
            }
            return Empty<GameObject>.Array;
        }

        private bool GetFindName(out string tmpName)
        {
            tmpName = findName;
            if (!nameIsVariable) return true;
            if (ScriptManager.TryGetGlobalVariableValue(findName, out var varValue))
            {
                tmpName = varValue.ToScriptParamString();
                return true;
            }
            return false;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying && findObjectInRuntime) CacheObjects();
#endif 
        }


        #region 查找

        /// <summary>
        /// 查找游戏对象子级
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectChildren(IEnumerable<GameObject> gameObjects, bool includeInactive = false)
        {
            var gos = new List<GameObject>();
            foreach (var go in gameObjects)
            {
                gos.AddRange(CommonFun.GetChildGameObjects(go.transform));
            }
            return gos.ToArray();
        }

        /// <summary>
        /// 查找游戏对象通过层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithLayer(int layer, bool includeInactive = false)
        {
            return FindGameObjectsWithFunc(go => go.layer == layer ? true : false, includeInactive);
        }

        /// <summary>
        /// 查找游戏对象通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compareNameRule"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithName(string name, ECompareNameRule compareNameRule, bool includeInactive = false)
        {
            switch (compareNameRule)
            {
                case ECompareNameRule.Equal: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) == 0, includeInactive);
                case ECompareNameRule.NotEqual: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) != 0, includeInactive);
                case ECompareNameRule.Less: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) < 0, includeInactive);
                case ECompareNameRule.LessEqual: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) <= 0, includeInactive);
                case ECompareNameRule.Greater: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) > 0, includeInactive);
                case ECompareNameRule.GreaterEqual: return FindGameObjectsWithFunc(go => string.Compare(go.name, name) >= 0, includeInactive);
                case ECompareNameRule.Contains: return FindGameObjectsWithFunc(go => go.name.Contains(name), includeInactive);
                case ECompareNameRule.NotContains: return FindGameObjectsWithFunc(go => !go.name.Contains(name), includeInactive);
                case ECompareNameRule.Contained: return FindGameObjectsWithFunc(go => name.Contains(go.name), includeInactive);
                case ECompareNameRule.NotContained: return FindGameObjectsWithFunc(go => !name.Contains(go.name), includeInactive);
                case ECompareNameRule.RegexMatch:
                    return FindGameObjectsWithFunc(go => (new Regex(@name)).IsMatch(go.name), includeInactive);
                case ECompareNameRule.RegexNotMatch:
                    return FindGameObjectsWithFunc(go => !(new Regex(@name)).IsMatch(go.name), includeInactive);
            }
            return FindGameObjectsWithFunc(go => go.name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0, includeInactive);
        }

        /// <summary>
        /// 查找游戏对象通过标签与层
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="layer"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithTagAndLayer(string tag, int layer, bool includeInactive = false)
        {
            return FindGameObjectsWithLayer(layer, includeInactive).FindAll(go => go.tag == tag).ToArray();
        }

        /// <summary>
        /// 查找游戏对象通过标签与名称
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="name"></param>
        /// <param name="compareNameRule"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithTagAndName(string tag, string name, ECompareNameRule compareNameRule, bool includeInactive = false)
        {
            return FindGameObjectsWithName(name, compareNameRule, includeInactive).FindAll(go => go.tag == tag).ToArray();
        }

        /// <summary>
        /// 查找游戏对象通过层与名称
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="name"></param>
        /// <param name="compareNameRule"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithLayerAndName(int layer, string name, ECompareNameRule compareNameRule, bool includeInactive = false)
        {
            return FindGameObjectsWithName(name, compareNameRule, includeInactive).FindAll(go => go.layer == layer).ToArray();
        }

        /// <summary>
        /// 查找所有游戏对象（包含非激活）
        /// </summary>
        /// <param name="fun">匹配函数</param>
        /// <returns>返回游戏对象</returns>
        public static GameObject[] FindGameObjectsWithFunc(Func<GameObject, bool> fun, bool includeInactive = false)
        {
            var goArray = includeInactive ? Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[] :
                FindObjectsOfType(typeof(GameObject)) as GameObject[];
            var goList = new List<GameObject>();
            goArray.Foreach(go =>
            {
                if (fun(go))
                {
                    goList.Add(go);
                }
            });
            return goList.ToArray();
        }

        /// <summary>
        /// 选择类型
        /// </summary>
        [Name("选择类型")]
        public enum ESelectionType
        {
            /// <summary>
            /// 游戏对象
            /// </summary>
            [Name("游戏对象")]
            GameObject,

            /// <summary>
            /// 标签
            /// </summary>
            [Name("标签")]
            Tag,

            /// <summary>
            /// 层
            /// </summary>
            [Name("层")]
            Layer,

            /// <summary>
            /// 名称
            /// </summary>
            [Name("名称")]
            Name,

            /// <summary>
            /// 标签和层
            /// </summary>
            [Name("标签和层")]
            TagAndLayer,

            /// <summary>
            /// 标签和名称
            /// </summary>
            [Name("标签和名称")]
            TagAndName,

            /// <summary>
            /// 层和名称
            /// </summary>
            [Name("层和名称")]
            LayerAndName,

            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            Variable,

            /// <summary>
            /// 游戏对象成员
            /// </summary>
            [Name("游戏对象成员")]
            GameObjectChildren,

            /// <summary>
            /// 游戏对象所有成员
            /// </summary>
            [Name("游戏对象所有子成员")]
            [Tip("包含子和孙对象", "Contains children and grandchildren")]
            GameObjectAllChildren,

            /// <summary>
            /// 游戏对象及父级对象
            /// </summary>
            [Name("游戏对象及所有父级对象")]
            GameObjectAndAllParents,

            /// <summary>
            /// 选择集
            /// </summary>
            [Name("选择集")]
            Selection,
        }

        /// <summary>
        /// 比较名称规则
        /// </summary>
        [Name("比较名称规则")]
        public enum ECompareNameRule
        {
            /// <summary>
            /// 等于
            /// </summary>
            [Name("等于")]
            Equal = 0,

            /// <summary>
            /// 不等于
            /// </summary>
            [Name("不等于")]
            NotEqual,

            /// <summary>
            /// 小于
            /// </summary>
            [Name("小于")]
            Less,

            /// <summary>
            /// 小于等于
            /// </summary>
            [Name("小于等于")]
            LessEqual,

            /// <summary>
            /// 大于
            /// </summary>
            [Name("大于")]
            Greater,

            /// <summary>
            /// 大于等于
            /// </summary>
            [Name("大于等于")]
            GreaterEqual,

            /// <summary>
            /// 包含
            /// </summary>
            [Name("包含")]
            Contains,

            /// <summary>
            /// 不包含
            /// </summary>
            [Name("不包含")]
            NotContains,

            /// <summary>
            /// 被包含
            /// </summary>
            [Name("被包含")]
            Contained,

            /// <summary>
            /// 不被包含
            /// </summary>
            [Name("不被包含")]
            NotContained,

            /// <summary>
            /// 正则表达式匹配
            /// </summary>
            [Name("正则表达式匹配")]
            RegexMatch,

            /// <summary>
            /// 正则表达式不匹配
            /// </summary>
            [Name("正则表达式不匹配")]
            RegexNotMatch,
        }

        #endregion
    }
}
