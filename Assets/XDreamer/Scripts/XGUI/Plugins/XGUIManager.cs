using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.CNScripts;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows.ImageBrowers;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI
{
    /// <summary>
    /// XGUI:用于调度、维护、管理GUI的插件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Base)]
    [ComponentOption(EComponentOption.Recommended)]
    [Name("XGUI")]
    [Tip("用于调度、维护、管理GUI的插件", "Plug in for scheduling, maintenance and management GUI")]
    [Guid("80A39A8F-C7AB-44BB-BC92-E27BA56707FC")]
    [Version("23.428")]
    [ExecuteInEditMode]
    public sealed class XGUIManager : BaseManager<XGUIManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch ((EScriptID)id)
            {
                case EScriptID.ImageBrowerControl:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var imageBrower = go.GetComponent<ImageBrower>();
                        if (!imageBrower) break;
                        switch (param[2] as string)
                        {
                            case "全屏":
                                {
                                    imageBrower.FullscreenImage();
                                    break;
                                }
                            case "取消全屏":
                                {
                                    imageBrower.HideImageView();
                                    break;
                                }
                            case "上一张":
                                {
                                    var scrollPage = imageBrower.scrollPage;
                                    if (!scrollPage) return ReturnValue.No;
                                    scrollPage.PreviousPage();
                                    break;
                                }
                            case "下一张":
                                {
                                    var scrollPage = imageBrower.scrollPage;
                                    if (!scrollPage) return ReturnValue.No;
                                    scrollPage.NextPage();
                                    break;
                                }
                            case "跳转至指定页":
                                {
                                    var scrollPage = imageBrower.scrollPage;
                                    if (!scrollPage) return ReturnValue.No;

                                    if (int.TryParse(param[3] as string, out int index))
                                    {
                                        scrollPage.GotoPage(index, true);
                                    }
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }
            }
            return ReturnValue.No;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            this.XGetOrAddComponent<XGUICommonAssets>();
        }
    }
}
