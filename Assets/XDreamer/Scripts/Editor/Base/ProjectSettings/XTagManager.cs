using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.ProjectSettings
{
    /// <summary>
    /// 标签管理器
    /// </summary>
    public static class XTagManager
    {
        /// <summary>
        /// 标签管理器资产路径
        /// </summary>
        public const string AssetPath = "ProjectSettings/TagManager.asset";

        /// <summary>
        /// 层属性名
        /// </summary>
        public const string LayerPropertyName = "layers";

        /// <summary>
        /// 添加层
        /// </summary>
        /// <param name="layerName"></param>
        public static void AddLayer(string layerName)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath(AssetPath);

            if ((asset != null) && (asset.Length > 0))
            {
                var serializedObject = new SerializedObject(asset[0]);
                var layers = serializedObject.FindProperty(LayerPropertyName);

                for (int i = 0; i < layers.arraySize; ++i)
                {
                    if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                    {
                        return;     // 层已存在
                    }
                }

                // 逆向遍历
                for (int i = layers.arraySize -1; i >= 0; --i)
                {
                    if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                    {
                        layers.GetArrayElementAtIndex(i).stringValue = layerName;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();

                        // 避免Unity锁死层
                        if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                        {
                            return;     
                        }
                    }
                }
            }
        }
    }
}
