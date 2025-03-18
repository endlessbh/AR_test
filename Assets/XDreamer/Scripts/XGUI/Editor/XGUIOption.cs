using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorXGUI.Styles;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// XGUI配置
    /// </summary>
    [XDreamerPreferences]
    [Name("XGUI")]
    [Import]
    public class XGUIOption : XDreamerOption<XGUIOption>
    {
        protected override int newVersion => -1;

        private const string DefaultStyleName = "科幻";

        /// <summary>
        /// 默认样式
        /// </summary>
        [Name("默认样式")]
        public string _defaultStyleName = DefaultStyleName;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            // 响应样式文件加载后，回调设置默认样式
            XStyleFileHelper.onLoadStyleFile += OnLoadStyleFile;

            // 如果样式文件加载回调事件已经发生了，则上面的函数无法扑捉到回调事件，这时候需主动设定
            if (!XStyleCache.defaultStyle)
            {
                SetDefaultStyle();
            }
        }        

        private void OnLoadStyleFile(List<XStyleFile> styleFile)
        {
            SetDefaultStyle();
        }

        /// <summary>
        /// 设置默认样式
        /// </summary>
        private void SetDefaultStyle()
        {
            XStyleCache.SetDefaultStyle(_defaultStyleName);
        }

        protected override void OnVersionChanged(int lastVersion)
        {
            if (newVersion < 0)
            {
                _defaultStyleName = DefaultStyleName;
            }
        }
    }

    /// <summary>
    /// XGUIOption编辑器
    /// </summary>
    [CommonEditor(typeof(XGUIOption))]
    public class XGUIOptionEditor : XDreamerOptionEditor<XGUIOption>
    {
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(XGUIOption._defaultStyleName):
                    {
                        EditorGUI.BeginChangeCheck();
                        preference._defaultStyleName = UICommonFun.Popup(CommonFun.NameTip(target.GetType(), nameof(XGUIOption._defaultStyleName)), preference._defaultStyleName, XStyleCache.GetNames(), false, GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                        }
                        return true;
                    }
            }
             return base.OnGUI(obj, fieldInfo);
        }
    }
}
