using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.ReferencedObjects
{
    /// <summary>
    /// 引用集
    /// </summary>
    [Name("引用集")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class ReferencedSet : InteractProvider
    {
        /// <summary>
        /// 获取第一个实例
        /// </summary>
        /// <returns></returns>
        public static ReferencedSet GetFirstInstance()
        {
            var components = ComponentCache.GetComponents<ReferencedSet>();
            if (components != null)
            {
                var set1 = components.FirstOrDefault(c => !c.transform.parent);
                if (set1) return set1;

                var set2 = components.FirstOrDefault();
                if (set2) return set2;
            }

            var name = CommonFun.GetUniqueName(CommonFun.Name(typeof(ReferencedSet)), n => !CommonFun.GetRootGameObjects().Any(go => go.name == n));
            var gameObject = UnityObjectHelper.CreateGameObject(name);
            return gameObject.XAddComponent<ReferencedSet>();
        }

        /// <summary>
        /// 获取第一个实例变换
        /// </summary>
        /// <returns></returns>
        public static Transform GetFirstInstanceTransform() => GetFirstInstance().transform;
    }
}
