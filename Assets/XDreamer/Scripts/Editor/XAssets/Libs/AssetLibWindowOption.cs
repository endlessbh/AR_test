using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    [XDreamerPreferences(index = IDRange.Begin + 60)]
    [Name(Product.Name + "-本地资源库")]
    [Import]
    public class AssetLibWindowOption : XDreamerOption<AssetLibWindowOption>
    {
        protected override int newVersion => base.newVersion;

        protected override void OnVersionChanged(int lastVersion)
        {
            base.OnVersionChanged(lastVersion);
        }

        [Name("文字颜色")]
        [Json(exportString = true)]
        public Color ColorText = Color.white;

        [Name("列表选项选中颜色")]
        [Json(exportString = true)]
        public Color ColorCategoryHover = new Color(0f, 0.734375f, 0.9453125f, 1f);

        [Name("资源面板最小尺寸")]
        public int AssetPanelMinSize = 200;

        [Name("单行元素数目")]
        public int ColumnNumberPerGrid = 3;

        [Name("单页行数")]
        public int RowNumberPerGrid = 10;

        [Name("同意声明")]
        public bool RecieveDeclaration = false;

        [Name("资源路径配置")]
        public List<SuperAssetPath> assetPathConfigs = new List<SuperAssetPath> { new SuperAssetPath("资源",  "Assets/XDreamer-Assets/" , ConstVariable.WholeTypeName, new List<AssetPath> {}),
                new SuperAssetPath("案例", "Assets/XDreamer-Examples/" , "场景", new List<AssetPath>()),
                new SuperAssetPath("第三方库", "Assets/XDreamer-ThirdPartyUnityPackage/" , "资源包", new List<AssetPath>()),
                };

        [Name("资源类型配置")]
        public AssetExtensionConfig assetExtensionConfig = new AssetExtensionConfig();
    }

    [CommonEditor(typeof(AssetLibWindowOption))]
    public class AssetLibWindowOptionEditor : XDreamerOptionEditor<AssetLibWindowOption>
    {
        private const int NameWidth = UICommonOption._120;

        private void DrawAssetPathConfig(AssetPath assetPath, AssetLibWindowOption option)
        {
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetPath, nameof(assetPath.name)), GUILayout.Width(NameWidth));
                assetPath.name = EditorGUILayout.TextField(assetPath.name);
            }
            UICommonFun.EndHorizontal();
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetPath, nameof(assetPath.path)), GUILayout.Width(NameWidth));
                assetPath.path = EditorGUILayout.TextField(assetPath.path);
            }
            UICommonFun.EndHorizontal();
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetPath, nameof(assetPath.assetType)), GUILayout.Width(NameWidth));
                int assetIndex = option.assetExtensionConfig.GetAssetIndex(assetPath.assetType);
                if (assetIndex == -1) assetIndex = 0;
                assetIndex = EditorGUILayout.Popup(assetIndex, option.assetExtensionConfig.GetAssetTypes(), new GUILayoutOption[0]);
                if (assetIndex == 0) assetPath.assetType = "全部类型";
                else assetPath.assetType = option.assetExtensionConfig.assetExtensions[assetIndex - 1].assetType;
            }
            UICommonFun.EndHorizontal();
        }

        private void DrawSuperAssetPathConfig(SuperAssetPath superAssetPath, AssetLibWindowOption option)
        {
            DrawAssetPathConfig(superAssetPath, option);
            GUILayout.Label(CommonFun.NameTip(superAssetPath, nameof(superAssetPath.assetPaths)), GUILayout.Width(NameWidth));
            CommonFun.BeginLayout();
            if (superAssetPath.assetPaths.Count > 0)
            {
                for (int i = 0; i < superAssetPath.assetPaths.Count; i++)
                {
                    var item = superAssetPath.assetPaths[i];
                    bool iconSetsChanged = false;
                    if (item.expand = UICommonFun.Foldout(item.expand, CommonFun.TempContent(item.name), true, EditorStyles.foldout, null, () =>
                    {
                        if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                        {
                            var newSet = item;
                            superAssetPath.assetPaths.Insert(i + 1, newSet);
                            iconSetsChanged = true;
                        }

                        EditorGUI.BeginDisabledGroup(superAssetPath.assetPaths.Count == 0);
                        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
                        {
                            superAssetPath.assetPaths.RemoveAt(i);
                            iconSetsChanged = true;
                        }
                        EditorGUI.EndDisabledGroup();
                    }))

                    if (!iconSetsChanged)
                    {
                        CommonFun.BeginLayout();
                        DrawAssetPathConfig(item, option);
                        CommonFun.EndLayout();
                    }

                }
            }
            else
            {
                var item = new AssetPath();
                if (item.expand = UICommonFun.Foldout(item.expand, CommonFun.TempContent(item.name), true, EditorStyles.foldout, null, () =>
                {
                    if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(25)))
                    {
                        var newSet = item;
                        superAssetPath.assetPaths.Add(newSet);
                    }
                })) { }
            }
            CommonFun.EndLayout();
        }

        private void DrawAssetPathConfigs(AssetLibWindowOption option)
        {
            
            GUILayout.Label(CommonFun.NameTip(option, nameof(option.assetPathConfigs)), GUILayout.Width(NameWidth));
            CommonFun.BeginLayout();
            for (int i = 0; i < option.assetPathConfigs.Count; i++)
            {
                var item = option.assetPathConfigs[i];
                bool assetPathSetsChanged = false;
                if (item.expand = UICommonFun.Foldout(item.expand, CommonFun.TempContent(item.name), true, EditorStyles.foldout, null, () =>
                {
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                    {
                        var newSet = item;
                        option.assetPathConfigs.Insert(i + 1, newSet);
                        assetPathSetsChanged = true;
                    }

                    EditorGUI.BeginDisabledGroup(option.assetPathConfigs.Count == 0);
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
                    {
                        option.assetPathConfigs.RemoveAt(i);
                        assetPathSetsChanged = true;
                    }
                    EditorGUI.EndDisabledGroup();
                }))
                {
                    if (!assetPathSetsChanged)
                    {
                        CommonFun.BeginLayout();
                        DrawSuperAssetPathConfig(item, option);
                        CommonFun.EndLayout();
                    }
                }
            }
            CommonFun.EndLayout();
        }

        private void DrawAssetExtension(AssetExtension assetExtension)
        {
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetExtension, nameof(assetExtension.assetType)), GUILayout.Width(NameWidth));
                assetExtension.assetType = EditorGUILayout.TextField(assetExtension.assetType);
            }
            UICommonFun.EndHorizontal();
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetExtension, nameof(assetExtension.extensions)), GUILayout.Width(NameWidth));
                var extensions = EditorGUILayout.TextField(string.Join("|", assetExtension.extensions.ToArray()));
                assetExtension.extensions = new List<string>(extensions.Split(new string[] { "，", ",", "|" }, StringSplitOptions.RemoveEmptyEntries));
            }
            UICommonFun.EndHorizontal();
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetExtension, nameof(assetExtension.haveReview)), GUILayout.Width(NameWidth));
                assetExtension.haveReview = EditorGUILayout.Toggle(assetExtension.haveReview);
            }
            UICommonFun.EndHorizontal();
            UICommonFun.BeginHorizontal(true);
            {
                GUILayout.Label(CommonFun.NameTip(assetExtension, nameof(assetExtension.typeImgPath)), GUILayout.Width(NameWidth));
                assetExtension.typeImgPath = EditorGUILayout.TextField(assetExtension.typeImgPath);
            }
            UICommonFun.EndHorizontal();
        }

        private void DrawAssetExtensionConfig(AssetLibWindowOption option)
        {

            GUILayout.Label(CommonFun.NameTip(option, nameof(option.assetExtensionConfig)), GUILayout.Width(NameWidth));
            CommonFun.BeginLayout();
            for (int i = 0; i < option.assetExtensionConfig.assetExtensions.Count; i++)
            {
                var item = option.assetExtensionConfig.assetExtensions[i];
                bool assetExtensionSetsChanged = false;
                if (item.expand = UICommonFun.Foldout(item.expand, CommonFun.TempContent(item.assetType), true, EditorStyles.foldout, null, () =>
                {
                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                    {
                        var newSet = item;
                        option.assetExtensionConfig.assetExtensions.Insert(i + 1, newSet);
                        assetExtensionSetsChanged = true;
                    }

                    EditorGUI.BeginDisabledGroup(option.assetExtensionConfig.assetExtensions.Count == 0);
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
                    {
                        option.assetExtensionConfig.assetExtensions.RemoveAt(i);
                        assetExtensionSetsChanged = true;
                    }
                    EditorGUI.EndDisabledGroup();
                }))
                {
                    if (!assetExtensionSetsChanged)
                    {
                        CommonFun.BeginLayout();
                        DrawAssetExtension(item);
                        CommonFun.EndLayout();
                    }
                }
            }
            CommonFun.EndLayout();
        }

        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.ColumnNumberPerGrid):
                    {
                        UICommonFun.BeginHorizontal(true);
                        {
                            GUILayout.Label(CommonFun.NameTip(preference, nameof(preference.ColumnNumberPerGrid)), GUILayout.Width(NameWidth + 20));
                            preference.ColumnNumberPerGrid = EditorGUILayout.IntSlider(preference.ColumnNumberPerGrid, 1, 12, new GUILayoutOption[0]);
                        }
                        UICommonFun.EndHorizontal();
                        
                        return true;
                    }
                case nameof(preference.RowNumberPerGrid):
                    {
                        UICommonFun.BeginHorizontal(true);
                        {
                            GUILayout.Label(CommonFun.NameTip(preference, nameof(preference.RowNumberPerGrid)), GUILayout.Width(NameWidth + 20));
                            preference.RowNumberPerGrid = EditorGUILayout.IntSlider(preference.RowNumberPerGrid, 1, 15, new GUILayoutOption[0]);
                        }
                        UICommonFun.EndHorizontal();

                        return true;
                    }
                case nameof(preference.RecieveDeclaration):
                    {
                        return true;
                    }
                case nameof(preference.assetPathConfigs):
                    {
                        
                        DrawAssetPathConfigs(preference);
                        return true;
                    }
                case nameof(preference.assetExtensionConfig):
                    {
                        DrawAssetExtensionConfig(preference);
                        return true;
                    }
            }

            return base.OnGUI(obj, fieldInfo);
        }
    }

    /// <summary>
    /// 文件路径配置
    /// </summary>
    [Import]
    [Serializable]
    [Name("资源路径")]
    public class AssetPath
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        [Name("类别名称")]
        public string name;

        /// <summary>
        /// 路径
        /// </summary>
        [Name("文件路径")]
        public string path;

        /// <summary>
        /// 资源类型
        /// </summary>
        [Name("资源类型")]
        public string assetType;

        [Name("是否折叠")]
        public bool expand = false;

        public AssetPath()
        { }

        public AssetPath(string name, string path, string eAssetType)
        {
            this.name = name;
            this.path = path;
            this.assetType = eAssetType;
        }
    }

    [Import]
    [Serializable]
    [Name("超级资源路径")]
    public class SuperAssetPath : AssetPath
    {
        /// <summary>
        /// 子类别路径
        /// </summary>
        [Name("子类别路径列表")]
        public List<AssetPath> assetPaths = new List<AssetPath>();

        public SuperAssetPath()
        { }

        public SuperAssetPath(string name, string path, string eAssetType, List<AssetPath> assetPaths)
        {
            this.name = name;
            this.path = path;
            this.assetType = eAssetType;
            this.assetPaths = assetPaths;
        }
    }

    [Serializable]
    [Name("资源扩展配置")]
    public class AssetExtensionConfig
    {
        public List<AssetExtension> assetExtensions = new List<AssetExtension>() { new AssetExtension("模型", new List<string>() { ".fbx", ".asset", ".obj" }, true, "/XDreamer/EditorResources/XAssetLib/Model.png"),
            new AssetExtension("贴图", new List<string>() { ".tga", ".png", ".jpg", ".tif", ".psd", ".exr" }, true, "/XDreamer/EditorResources/XAssetLib/Texture.png"),
            new AssetExtension("材质", new List<string>() { ".meta" }, true, "/XDreamer/EditorResources/XAssetLib/Material.png"),
            new AssetExtension("音频", new List<string>() { ".mp3", ".wma", ".rm", ".wav", ".midi", ".ape", ".flac" }, true, "/XDreamer/EditorResources/XAssetLib/Audio.png"),
            new AssetExtension("资源包", new List<string>() { ".unitypackage" }),
            new AssetExtension("预制体", new List<string>() { ".prefab" }),
            new AssetExtension("场景", new List<string>() { ".unity" }),
            new AssetExtension("物理材质", new List<string>() { ".physicMaterial" }),
            new AssetExtension("2D物理材质", new List<string>() { ".physicMaterial2D" }),
            new AssetExtension("渲染贴图", new List<string>() { ".renderTexture" }),
            new AssetExtension("字体", new List<string>() { ".fontsettings", ".TTF" }),
            new AssetExtension("立方体贴图", new List<string>() { ".cubemap" })
        };

        /// <summary>
        /// 获取资源类型列表数组
        /// </summary>
        /// <returns>资源类型数组</returns>
        public string[] GetAssetTypes()
        {
            string[] assetTypes = new string[assetExtensions.Count + 1];
            assetTypes[0] = ConstVariable.WholeTypeName;
            for (int i = 0; i < assetExtensions.Count; i++)
                assetTypes[i + 1] = assetExtensions[i].assetType;
            return assetTypes;
        }

        /// <summary>
        /// 获取后缀数组
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <returns>后缀数组</returns>
        public string[] GetExtensions(string assetType)
        {
            for (int i = 0; i < assetExtensions.Count; i++)
            {
                if (assetExtensions[i].assetType == assetType) return assetExtensions[i].extensions.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 获取资源索引
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <returns>资源索引，用于界面显示</returns>
        public int GetAssetIndex(string assetType)
        {
            if (assetType == ConstVariable.WholeTypeName) return 0;
            for (int i = 0; i < assetExtensions.Count; i++)
            {
                if (assetExtensions[i].assetType == assetType) return i+1;
            }
            return -1;
        }
    }

    /// <summary>
    /// 资源扩展：用于处理不同类型扩展名
    /// </summary>
    [Serializable]
    [Name("资源扩展")]
    public class AssetExtension
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        [Name("资源类型")]
        public string assetType = "";

        /// <summary>
        /// 扩展名列表
        /// </summary>
        [Name("扩展名列表")]
        public List<string> extensions = new List<string>();

        /// <summary>
        /// 是否有资源缩略图
        /// </summary>
        [Name("是否有资源缩略图")]
        public bool haveReview = false;

        /// <summary>
        /// 类型缩略图路径
        /// </summary>
        [Name("类型缩略图路径")]
        public string typeImgPath = "";

        [Name("是否折叠")]
        public bool expand = false;

        public AssetExtension()
        { }

        public AssetExtension(string assetType, List<string> extensions, bool haveReview = false, string imgPath = "")
        {
            this.assetType = assetType;
            this.extensions = extensions;
            this.haveReview = haveReview;
            this.typeImgPath = imgPath;
        }

        public AssetExtension(string assetType, string[] extensions, bool haveReview = false, string imgPath = "")
        {
            this.assetType = assetType;
            this.extensions = extensions.ToList<string>();
            this.haveReview = haveReview;
            this.typeImgPath = imgPath;
        }

        public AssetExtension(string assetType, string extention, bool haveReview = false, string imgPath = "")
        {
            List<string> list = new List<string>(extention.Split(new string[] { "，", "," }, StringSplitOptions.RemoveEmptyEntries));
            this.assetType = assetType;
            this.extensions = list;
            this.haveReview = haveReview;
            this.typeImgPath = imgPath;
        }
    }

    /// <summary>
    /// 资源库所有的常量
    /// </summary>
    [Serializable]
    [Name("常量变量")]
    public class ConstVariable
    {
        /// <summary>
        /// 全部类型名称
        /// </summary>
        public const string WholeTypeName = "全部类型";

        /// <summary>
        /// 资源所在路径
        /// </summary>
        public const string CommonPath = "Assets/XDreamer-Assets/";

        /// <summary>
        /// 第三方库路径
        /// </summary>
        public const string ThirdPath = "Assets/XDreamer-ThirdPartyUnityPackage/";
    }
}
