using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.Scripts;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具库扩展助手
    /// </summary>
    public static class ToolsExtensionHelper
    {
        /// <summary>
        /// 设置线渲染器样式
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="lineStyle"></param>
        public static void SetLineRendererStyle(this LineRenderer lineRenderer, LineStyle lineStyle)
        {
            if (!lineRenderer || !lineStyle) return;

            lineRenderer.material = lineStyle.mat;
            lineRenderer.material.color = lineStyle.color;
            lineRenderer.startWidth = lineStyle.width;
            lineRenderer.endWidth = lineStyle.width;
            lineRenderer.startColor = lineStyle.color;
            lineRenderer.endColor = lineStyle.color;
            lineRenderer.allowOcclusionWhenDynamic = lineStyle.occlusion;
        }

        /// <summary>
        /// 是否在选择集中
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsSelection(Component component) => component && IsSelection(component.transform);

        /// <summary>
        /// 是否在选择集中
        /// </summary>
        /// <param name="inTransform"></param>
        /// <returns></returns>
        public static bool IsSelection(Transform inTransform)
        {
            if (!inTransform) return false;

            foreach (var item in Selection.selections)
            {
                if (item && (item == inTransform.gameObject || inTransform.IsChildOf(item.transform)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加可抓交互组件
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="grabbable"></param>
        /// <param name="useGravity"></param>
        /// <param name="isKinematic"></param>
        /// <returns></returns>
        public static bool AddGrabbale(this GameObject gameObject, out Grabbable grabbable)
        {
            if (!gameObject.GetComponent<InteractableEntity>())
            {
                gameObject.XAddComponent<InteractableEntity>();
            }

            if (!gameObject.GetComponent<Grabbable>())
            {
                grabbable = gameObject.XAddComponent<Grabbable>();
                if (grabbable)
                {
                    grabbable.Reset();
                }
                return true;
            }

            grabbable = default;
            return false;
        }

        /// <summary>
        /// 创建一个与输入游戏对象同层级且变换数据相同的游戏对象
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="name"></param>
        /// <param name="isSynTransformData"></param>
        /// <returns></returns>
        public static GameObject CreateGameObjectAtSameSibling(GameObject gameObject, string name, bool isSynTransformData = true)
        {
            if (!gameObject) return null;

            var go = UnityObjectHelper.CreateGameObject(name);
            if (go)
            {
                var sourceTransform = gameObject.transform;
                var targetTransform = go.transform;

                targetTransform.SetParent(sourceTransform.parent);
                targetTransform.SetSiblingIndex(sourceTransform.GetSiblingIndex() + 1);

                if (isSynTransformData)
                {
                    targetTransform.localPosition = sourceTransform.localPosition;
                    targetTransform.localRotation = sourceTransform.localRotation;
                    targetTransform.localScale = sourceTransform.localScale;
                }
            }
            return go;
        }

        public static T GetInteractable<T>(this IInteractable interactable) where T : class, IInteractable
        {
            var result = interactable as T;
            if (result == null)
            {
                var entity = interactable as InteractableEntity;
                if (entity)
                {
                    result = entity.GetInteractable<T>();
                }

                if (result == null)
                {
                    var c = interactable as MonoBehaviour;
                    if (c)
                    {
                        result = c.GetComponent<T>();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 从交互数据中提取位置信息
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="position"></param>
        /// <param name="tryParent">尝试从父级数据上获取位置信息</param>
        /// <returns></returns>
        public static bool TryGetPosition(this InteractData interactData, out Vector3 position, bool tryParent = false)
        {
            if (interactData != null)
            {
                if (interactData is ColliderInteractData collisionData && collisionData.collision!=null)
                {
                    if (collisionData.collision.contactCount > 0)
                    {
                        position = collisionData.collision.GetContact(0).point;
                        return true;
                    }
                }
                else if (interactData is RayInteractData rayInteractData)
                {
                    if (rayInteractData.raycastHit.HasValue)
                    {
                        position = rayInteractData.raycastHit.Value.point;
                        return true;
                    }
                }
                else if (interactData.interactable)
                {
                    position = interactData.interactable.transform.position;
                    return true;
                }
            }

            if (tryParent)
            {
                TryGetPosition(interactData.parent, out position, tryParent);
            }

            position = Vector3.zero;
            return false;
        }
    }
}
