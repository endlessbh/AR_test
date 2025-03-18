using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.EditorXGUI.Styles
{
    /// <summary>
    /// 样式资产助手：用于管理风格资产文件在的创建、修改和删除
    /// </summary>
    public static class XStyleFileHelper
    {
        /// <summary>
        /// 路径
        /// </summary>
        public const string StylePath = "Assets/XDreamer-Assets/基础/GUI/样式/";

        /// <summary>
        /// 样式文件列表
        /// </summary>
        public static List<XStyleFile> styleFiles = new List<XStyleFile>();

        public static event Action<List<XStyleFile>> onLoadStyleFile;

        /// <summary>
        /// 加载所有风格
        /// </summary>
        /// <returns></returns>
        //[InitializeOnLoadMethod]
        public static void Init()
        {
            // 监听工程资产变动
            XDreamerEvents.onProjectAnyAssetsChangedInEditor += OnAssetsChanged;

            // 监听样式元素添加
            StyleElementCollection.onAddStyleElement += OnAddStyleElement;

            // 监听样式元素移除
            StyleElementCollection.onRemoveStyleElement += OnRemoveStyleElement;

            Load();
        }

        /// <summary>
        /// 资产变换回调函数
        /// </summary>
        private static void OnAssetsChanged()
        {
            // 存在无效对象，重新加载
            if (styleFiles.Exists(s => !s))
            {
                Load();
            }
        }

        private static void OnAddStyleElement(StyleElementCollection styleElementCollection, BaseStyleElement element)
        {
            OnStyleElementCountChanged(styleElementCollection, element, true);
        }

        private static void OnRemoveStyleElement(StyleElementCollection styleElementCollection, BaseStyleElement element)
        {
            OnStyleElementCountChanged(styleElementCollection, element, false);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private static bool OnStyleElementCountChanged(StyleElementCollection styleElementCollection, BaseStyleElement styleElement, bool addOrRemove)
        {
            if (!styleElementCollection) return styleElementCollection;

            foreach (var sf in styleFiles)
            {
                if (sf && (sf.asset == styleElementCollection || sf.asset.Contains(styleElementCollection)))
                {
                    if (addOrRemove)
                    {
                        sf.AddAssetAndSave(styleElement);
                    }
                    else
                    {
                        sf.RemoveAssetAndSave(styleElement);
                    }
                    // 应用样式修改
                    XStyleCache.UpdateStyle();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 从样式资产路径加载文件
        /// </summary>
        public static void Load()
        {
            if (Directory.Exists(StylePath))
            {
                styleFiles = EditorAssetHelper.Load<XStyleFile>(StylePath, XStyleFile.Extension, true);

                XStyleCache.Clear();
                styleFiles.ForEach(s =>
                {
                    s.MakeSameName();
                    XStyleCache.Register(s);
                });

                onLoadStyleFile?.Invoke(styleFiles);
            }
        }

        /// <summary>
        /// 创建UI风格对象
        /// </summary>
        public static XStyle Create(string name)
        {
            var dir = StylePath + name + "/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var style = XStyle.New(name);
            XStyleFile sf = style;
            styleFiles.Add(sf);
            sf.Save(dir + sf.name);
            return style;
        }

        /// <summary>
        /// 删除UI风格对象
        /// </summary>
        /// <param name="name"></param>
        public static bool Delete(string name)
        {
            var sf = styleFiles.Find(s => s.styleName == name);
            if (sf)
            {
                XStyleCache.Unregister(sf.asset);
                styleFiles.Remove(sf);
            }
            var fileName = XStyleFile.GetFileName(name);
            var dir = FindDirectory(fileName);
            bool rs = false;
            if (!string.IsNullOrEmpty(dir))
            {
                rs = AssetHelper.Delete(dir + fileName, null);
            }
            Debug.LogFormat("删除风格[{0}]{1}!", name, rs ? "成功" : "失败");
            return rs;
        }

        /// <summary>
        /// 复制风格
        /// </summary>
        /// <param name="uiStyle"></param>
        /// <returns></returns>
        public static XStyleFile Copy(XStyle style, string name)
        {
            if (string.IsNullOrEmpty(name) || styleFiles.Exists(sf => sf ? sf.styleName == name : false))
            {
                Debug.LogError("样式名称不能为空或与现有样式名重名!");
                return null;
            }
            var dir = FindDirectory(XStyleFile.GetFileName(style.name));
            if (!string.IsNullOrEmpty(dir))
            {
                return XStyleFile.CloneAndSave(dir + XStyleFile.GetFileName(name), style, sf => sf.styleName = name);
            }
            return null;
        }

        /// <summary>
        /// 查找样式文件对应目录
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FindDirectory(string fileName)
        {
            var dirList = new List<string>();
            dirList.Add(StylePath);
            dirList.AddRange(Directory.GetDirectories(StylePath));

            var fileExtension = "*" + XStyleFile.Extension;
            foreach (var dir in dirList)
            {
                foreach (var fp in Directory.GetFiles(dir, fileExtension))
                {
                    if (fp.Contains(fileName))
                    {
                        return dir + "/";
                    }
                }
            }
            return "";
        }
    }
}
