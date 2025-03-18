using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.CNScripts;
using XCSJ.PluginTools.Gif;
using XCSJ.PluginTools.Points;
using XCSJ.Scripts;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具库扩展:基于Unity游戏对象与组件开发的各种功能的集合扩展库
    /// </summary>
    [Name("工具库扩展")]
    [Tip("基于Unity游戏对象与组件开发的各种功能的集合扩展库", "A collection extension library of various functions developed based on Unity game objects and components")]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOption.Optional)]
    [Guid("24E6703E-2B74-4F32-9D94-856A416F6934")]
    [Version("23.428")]
    [Index(index = 200)]
    public class ToolsExtensionManager : BaseManager<ToolsExtensionManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EExtensionScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch ((EExtensionScriptID)id)
            {
                case EExtensionScriptID.SegmentCreaterOperation:
                    {
                        var segmentCreater = param[1] as SegmentCreater;
                        if (!segmentCreater) break;

                        switch (param[2] as string)
                        {
                            case "开始拾取":
                                {
                                    segmentCreater.BeginPick();
                                    return ReturnValue.Yes;
                                }
                            case "记录":
                                {
                                    segmentCreater.RecordPoint();
                                    return ReturnValue.Yes;
                                }
                            case "结束拾取":
                                {
                                    segmentCreater.EndPick();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }

                case EExtensionScriptID.ReplaceGameObjectMainTextureToGifTexture:
                    {
                        #region ReplaceGameObjectMainTextureToGifTexture

                        var textAsset = param[2] as TextAsset;
                        if (!textAsset) break;

                        var gifTextureUpdater = GifHelper.GetOrCreateGifTextureUpdater(param[1] as GameObject);
                        if (!gifTextureUpdater) break;

                        gifTextureUpdater._gifTexture._gifTextAsset = textAsset;
                        gifTextureUpdater._gifTexture._gifSource = EGifSource.TextAsset;

                        return ReturnValue.Create(gifTextureUpdater._gifTexture.Load(gifTextureUpdater));
                        #endregion
                    }
                case EExtensionScriptID.ControlGameObjectGifTexture:
                    {
                        #region ControlGameObjectGifTexture
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var gifTextureUpdater = go.GetComponent<GifTextureUpdater>();
                        if (gifTextureUpdater)
                        {
                            switch (param[2] as string)
                            {
                                case "播放":
                                    {
                                        return ReturnValue.Create(gifTextureUpdater.Play());
                                    }
                                case "停止":
                                    {
                                        return ReturnValue.Create(gifTextureUpdater.Stop());
                                    }
                                case "暂停":
                                    {
                                        return ReturnValue.Create(gifTextureUpdater.Pause());
                                    }
                                case "继续":
                                    {
                                        return ReturnValue.Create(gifTextureUpdater.Resume());
                                    }
                                case "播放或继续":
                                    {
                                        return ReturnValue.Create(gifTextureUpdater.ResumeOrPlay());
                                    }
                            }
                        }
                        break;
                        #endregion
                    }
            }
            return ReturnValue.No;
        }
    }

    /// <summary>
    /// 工具分类
    /// </summary>
    public static class InteractionCategory
    {
        /// <summary>
        /// 交互
        /// </summary>
        public const string Interact = "交互";

        /// <summary>
        /// 交互器
        /// </summary>
        public const string InteractObject = "交互器";

        /// <summary>
        /// 可交互对象
        /// </summary>
        public const string InteractEvent = "交互事件";

        /// <summary>
        /// 交互输入
        /// </summary>
        public const string InteractorInput = "交互输入";

        /// <summary>
        /// 特效
        /// </summary>
        public const string Effect = "特效";

        /// <summary>
        /// 属性数据
        /// </summary>
        public const string InteractCommon = "交互常用";

        /// <summary>
        /// 拖拽
        /// </summary>
        public const string Draggable = "拖拽";

        /// <summary>
        /// 交互器工具分类索引
        /// </summary>
        public const int InteractorIndex = 1;

        /// <summary>
        /// 可交互对象工具分类索引
        /// </summary>
        public const int InteractableIndex = 20;

        /// <summary>
        /// 交互输入器工具分类索引
        /// </summary>
        public const int InteractInputIndex = 40;

        /// <summary>
        /// 可交互对象可视化工具分类索引
        /// </summary>
        public const int InteractableVisualIndex = 60;

        /// <summary>
        /// 交互数据工具分类索引
        /// </summary>
        public const int InteractDataIndex = 80;
    }
}
