using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 包围盒提供器
    /// </summary>
    [Name("包围盒提供器")]
    [RequireManager(typeof(ToolsManager))]
    public class BoundsProvider : InteractProvider
    {
        /// <summary>
        /// 获取包围盒
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBounds(out Bounds bounds)
        {
            switch (_boundsRule)
            {
                case EBoundsRule.CustomBounds:
                    {
                        bounds = _bounds;
                        return true;
                    }
                case EBoundsRule.CustomBoundsSize:
                    {
                        bounds = new Bounds(transform.position, _boundsSize);
                        return true;
                    }
                case EBoundsRule.ChildrenRenderer:
                    {
                        if (_includeChildren)
                        {
                            if (CommonFun.GetBounds(out bounds, GetValidGameObjects(CommonFun.GetAllChildrenGameObject(gameObject)), false, _includeInactiveGO, _includeDisableRenderer))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (!IsIgnore(gameObject))
                            {
                                if (CommonFun.GetBounds(out bounds, gameObject, false, _includeInactiveGO, _includeDisableRenderer))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                case EBoundsRule.OnlyInclude:
                    {
                        if (CommonFun.GetBounds(out bounds, GetValidGameObjects(_includeGameObjects), false, _includeInactiveGO, _includeDisableRenderer))
                        {
                            return true;
                        }
                        break;
                    }

            }
            bounds = default;
            return false;
        }

        public List<GameObject> GetIgnoreGameObjects()
        {
            var gameObjects = new List<GameObject>();
            switch (_boundsRule)
            {

                case EBoundsRule.ChildrenRenderer:
                    {
                        if (_includeChildren)
                        {
                            gameObjects.AddRange(GetIgnoreGameObjects(CommonFun.GetAllChildrenGameObject(gameObject)));
                        }
                        else
                        {
                            if (IsIgnore(gameObject))
                            {
                                gameObjects.Add(gameObject);
                            }
                        }
                        break;
                    }
                case EBoundsRule.OnlyInclude:
                    {
                        gameObjects.AddRange(GetIgnoreGameObjects(_includeGameObjects));
                        break;
                    }

            }
            return gameObjects;
        }

        /// <summary>
        /// 包围盒规则
        /// </summary>
        [Name("包围盒规则")]
        [EnumPopup]
        public EBoundsRule _boundsRule = EBoundsRule.ChildrenRenderer;

        [Name("包围盒规则")]
        public enum EBoundsRule
        {
            [Name("无")]
            None,

            [Name("包围盒")]
            CustomBounds,

            [Name("包围盒尺寸")]
            CustomBoundsSize,

            [Name("子游戏对象渲染器")]
            ChildrenRenderer,

            [Name("仅使用包含游戏对象列表")]
            OnlyInclude,
        }

        /// <summary>
        /// 包围盒
        /// </summary>
        [Name("包围盒")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.CustomBounds)]
        public Bounds _bounds;

        /// <summary>
        /// 包围盒尺寸
        /// </summary>
        [Name("包围盒尺寸")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.CustomBoundsSize)]
        public Vector3 _boundsSize = Vector3.one;

        /// <summary>
        /// 包含子级游戏对象
        /// </summary>
        [Name("包含子级游戏对象")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.ChildrenRenderer)]
        public bool _includeChildren = true;

        /// <summary>
        /// 包含非激活游戏对象
        /// </summary>
        [Name("包含非激活游戏对象")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.ChildrenRenderer)]
        public bool _includeInactiveGO = true;

        /// <summary>
        /// 包含禁用渲染器
        /// </summary>
        [Name("包含禁用渲染器")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.ChildrenRenderer)]
        public bool _includeDisableRenderer = true;

        /// <summary>
        /// 包含游戏对象列表
        /// </summary>
        [Name("包含游戏对象列表")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.OnlyInclude)]
        public List<GameObject> _includeGameObjects = new List<GameObject>();

        /// <summary>
        /// 忽略规则
        /// </summary>
        [Group("忽略设置", textEN = "Ignore Settings")]
        [Name("忽略规则")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual | EValidityCheckType.And, EBoundsRule.ChildrenRenderer, nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.OnlyInclude)]
        public EIgnoreRule _ignoreRule = EIgnoreRule.InIgnoreGameObjectList;

        /// <summary>
        /// 忽略游戏对象列表
        /// </summary>
        [Name("忽略游戏对象列表")]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual | EValidityCheckType.And, EBoundsRule.ChildrenRenderer, nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.OnlyInclude)]
        public List<GameObject> _ignoreGameObjects = new List<GameObject>();

        /// <summary>
        /// 忽略组件类型
        /// </summary>
        [Name("忽略组件类型")]
        [ComponentTypePopup]
        [HideInSuperInspector(nameof(_boundsRule), EValidityCheckType.NotEqual | EValidityCheckType.And, EBoundsRule.ChildrenRenderer, nameof(_boundsRule), EValidityCheckType.NotEqual, EBoundsRule.OnlyInclude)]
        public List<string> _ignoreComponentTypes = new List<string>();


        [Name("忽略规则")]
        [Flags]
        public enum EIgnoreRule
        {           
            [EnumFieldName("使用忽略游戏对象列表")]
            InIgnoreGameObjectList = 1<< 0,

            [EnumFieldName("组件类型")]
            ComponentType = 1 << 1,
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            CommonFun.GetBounds(out _bounds, gameObject);
            _boundsSize = _bounds.size;
        }

        private bool IsIgnore(GameObject inGameObject)
        {
            if ((_ignoreRule & EIgnoreRule.InIgnoreGameObjectList) == EIgnoreRule.InIgnoreGameObjectList)
            {
                if (_ignoreGameObjects.Contains(inGameObject))
                {
                    return true;
                }
            }

            if ((_ignoreRule & EIgnoreRule.ComponentType) == EIgnoreRule.ComponentType)
            {
                if (_ignoreComponentTypes.Exists(strType =>
                {
                    return inGameObject.GetComponent(TypeCache.Get(strType));
                }))
                {
                    return true;
                }
            }

            return false;
        }

        private List<GameObject> GetValidGameObjects(IEnumerable<GameObject> gameObjects)
        {
            var gameObjectList = new List<GameObject>();

            foreach (var go in gameObjects)
            {
                if (!IsIgnore(go))
                {
                    gameObjectList.Add(go);
                }
            }

            return gameObjectList;
        }

        private List<GameObject> GetIgnoreGameObjects(IEnumerable<GameObject> gameObjects)
        {
            var gameObjectList = new List<GameObject>();

            foreach (var go in gameObjects)
            {
                if (IsIgnore(go))
                {
                    gameObjectList.Add(go);
                }
            }

            return gameObjectList;
        }
    }
}
