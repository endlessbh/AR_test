using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.ReferencedObjects
{
    /// <summary>
    /// 引用的目录
    /// </summary>
    [Name("引用的目录")]
    [DisallowMultipleComponent]
    public class ReferencedCategory : ReferencedObject
    {
        /// <summary>
        /// 获取本目录中属于拥有者的引用的对象
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public List<ReferencedObject> GetReferencedObjects(UnityEngine.Object owner)
        {
            if (!owner) return null;
            return GetComponentsInChildren<ReferencedObject>().Where(to => to.HasOwner(owner)).ToList();
        }

        /// <summary>
        /// 删除引用的目录中成员；如果成员被其他对象引用中，则不作处理；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="owner">拥有者</param>
        /// <param name="child">子变换</param>
        public void Delete(UnityEngine.Object owner, Transform child)
        {
            if (!owner || !child) return;

            //是当前对象变换的子级
            if (child.parent = transform)
            {
                var temp = child.GetComponent<ReferencedObject>();
                if (temp)
                {
                    temp.RemoveOwner(owner);
                    if (!temp.HasAnyOnwer())//未记录着任何拥有者信息
                    {
                        //直接删除游戏对象
                        child.gameObject.XDestoryObject();
                    }
                }
                else
                {
                    //直接删除游戏对象
                    child.gameObject.XDestoryObject();
                }
            }
        }
    }
}
