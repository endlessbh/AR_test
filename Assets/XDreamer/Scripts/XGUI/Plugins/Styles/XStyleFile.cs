using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles
{
    /// <summary>
    /// XStyle资产文件
    /// </summary>
    [Name("样式文件")]
    public class XStyleFile : AssetFile<XStyleFile, XStyle>
    {
        /// <summary>
        /// 样式名
        /// </summary>
        public string styleName { get => asset.name; set => asset.XSetName(value); }

        /// <summary>
        /// XStyle -> XStyleFile 隐式转换
        /// </summary>
        /// <param name="node"></param>
        public static implicit operator XStyleFile(XStyle node)
        {
            return XStyleFile.Create(node);
        }

        /// <summary>
        /// XStyleFile -> XStyle 隐式转换
        /// </summary>
        /// <param name="node"></param>
        public static implicit operator XStyle(XStyleFile styleFile)
        {
            return styleFile ? styleFile.asset : null;
        }

        /// <summary>
        /// 中间名
        /// </summary>
        public const string Middle = ".style";

        /// <summary>
        /// 扩展名
        /// </summary>
        public const string Extension = Middle + AssetHelper.DefaultExtension;

        /// <summary>
        /// 扩展名
        /// </summary>
        public override string extension => Extension;

        /// <summary>
        /// 保存资产回调
        /// </summary>
        protected override void OnSaveAsset()
        {
            SaveElement(asset);
        }

        private void SaveElement(StyleElementCollection styleElementCollection)
        {
            styleElementCollection._elements.ForEach(e =>
            {
                if (e is StyleElementCollection collection)
                {
                    SaveElement(collection);
                }
                else
                {
                    AddAsset(e);
                }
            });
        }

        /// <summary>
        /// 将样式名纠正为文件名
        /// </summary>
        public void MakeSameName()
        {
            int index = name.LastIndexOf(Middle);
            if (index>=0)
            {
                var tmp = name.Substring(0, index);
                if (tmp!=styleName)
                {
                    styleName = tmp;
                }
            }
        }

        /// <summary>
        /// 添加元素资产
        /// </summary>
        /// <param name="styleElement"></param>
        public void AddAssetAndSave(BaseStyleElement styleElement)
        {
            AddAsset(styleElement);
            TrySave();
        }

        /// <summary>
        /// 删除元素资产
        /// </summary>
        public void RemoveAssetAndSave(BaseStyleElement styleElement)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(styleElement);
#endif
            TrySave();
        }

        /// <summary>
        /// 保存元素
        /// </summary>
        /// <param name="styleElement"></param>
        private void TrySave()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public static string GetFileName(string styleName) => styleName + Extension;
    }
}
