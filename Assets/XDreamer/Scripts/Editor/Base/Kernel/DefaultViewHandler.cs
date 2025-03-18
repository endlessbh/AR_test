using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 缺省视图处理器
    /// </summary>
    public class DefaultViewHandler : InstanceClass<DefaultViewHandler>, IViewHandler
    {
        public GameObject Create(string name, Transform parent = null, params Type[] componentTypes)
        {
            GameObject newGO = null;

            try
            {
                // 创建画布类型的游戏对象比较复杂,单独进行创建操作
                if (componentTypes.Contains(typeof(Canvas)))
                {
                    newGO = EditorXGUIHelper.FindOrCreateRootCanvas();
                }
                else if (parent && parent.GetComponent<Canvas>())
                {
                    newGO = UnityObjectHelper.CreateGameObject(name);
                    newGO.XAddComponent<RectTransform>();
                }
                else
                {
                    newGO = UnityObjectHelper.CreateGameObject(name);
                }

                if (newGO)
                {
                    foreach (var item in componentTypes)
                    {
                        newGO.XGetOrAddComponent(item);
                    }

                    // 设置父子节点
                    if (parent)
                    {
                        newGO.XSetParent(parent);
                        newGO.XModifyProperty(()=> { GameObjectUtility.EnsureUniqueNameForSibling(newGO); });

                        // 如果游戏对象是RectTransform 则设置为随父节点缩放
                        if (newGO.GetComponent<RectTransform>() is RectTransform rect)
                        {
                            rect.XStretchHV();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }

            return newGO;
        }
    }
}
