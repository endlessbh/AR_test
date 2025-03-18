using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.AI.Autos;
using XCSJ.PluginTools.CNScripts;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Notes.Dimensionings;
using XCSJ.PluginTools.Others;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXGUI.Windows.ColorPickers;
using XCSJ.PluginXGUI.Windows.MiniMaps;
using XCSJ.Scripts;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Kernel
{
    /// <summary>
    /// 默认工具处理器
    /// </summary>
    public class DefaultToolsHandler : InstanceClass<DefaultToolsHandler>, IToolsHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            ToolsHandler.handler = instance;
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(ToolsManager manager) => Script.GetScriptsOfEnum<EScriptID>(manager);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue ExecuteScript(ToolsManager manager, int id, ScriptParamList param)
        {

            switch ((EScriptID)id)
            {
                case EScriptID.TrackGyro:
                    {
                        var trackGyro = GameObject.FindObjectOfType<TrackGyro>();
                        if (!trackGyro) break;
                        trackGyro.SetTracked(CommonFun.BoolChange(trackGyro.enabled, (EBool)param[1]));
                        break;
                    }
                case EScriptID.TrackGyroWithGameObject:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var trackGyro = go.GetComponent<TrackGyro>();
                        if (!trackGyro)
                        {
                            trackGyro = go.AddComponent<TrackGyro>();
                        }
                        trackGyro.SetTracked(CommonFun.BoolChange(trackGyro.enabled, (EBool)param[2]));
                        return ReturnValue.Yes;
                    }
                case EScriptID.GLWireFrameRendererControl:
                    {
                        var cam = CameraHelperExtension.currentCamera;
                        if (cam)
                        {
                            var renderer = CommonFun.GetOrAddComponent<GLWireFrameRenderer>(cam.gameObject);
                            renderer.enabled = (param[1] as string) == "启动" ? true : false;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.BeginNotePointPicker:
                    {
                        var picker = param[1] as ClickPointPicker;
                        if (picker)
                        {
                            picker.BeginPick();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.EndNotePointPicker:
                    {
                        var picker = param[1] as ClickPointPicker;
                        if (picker)
                        {
                            picker.EndPick();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.GetColorPickerColor:
                    {
                        var picker = param[1] as ColorPicker;
                        if (picker)
                        {
                            return ReturnValue.True(picker.color);
                        }
                        break;
                    }
                case EScriptID.SetColorPickerColor:
                    {
                        var picker = param[1] as ColorPicker;
                        if (picker)
                        {
                            picker.color = (Color)param[2];
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.SetRendererToColorPicker:
                    {
                        var renderer = param[1] as Renderer;
                        var picker = param[2] as ColorPicker;
                        if (picker && renderer && renderer.material)
                        {
                            picker.color = renderer.material.color;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.SetColorPickerBinderMode:
                    {
                        var binder = param[1] as ColorPickerBinder;
                        if (binder)
                        {
                            switch (param[2] as string)
                            {
                                case "游戏对象列表": binder.bindType = EColorPickerBindType.GameObject; return ReturnValue.Yes;
                                case "选择集": binder.bindType = EColorPickerBindType.Selection; return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EScriptID.SetColorPickerBinderToColorPicker:
                    {
                        var binder = param[1] as ColorPickerBinder;
                        if (binder)
                        {
                            binder.BinderToColorPicker();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.ColorPickerBinderGameObjectOperation:
                    {
                        var binder = param[1] as ColorPickerBinder;
                        if (binder)
                        {
                            var go = param[3] as GameObject;
                            switch (param[2] as string)
                            {
                                case "添加绑定游戏对象":
                                    if (go)
                                    {
                                        binder.AddGameObject(go, false);
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                case "移除绑定游戏对象":
                                    if (go)
                                    {
                                        binder.RemoveGameObject(go);
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                case "清空所有绑定游戏对象": binder.ClearGameObject(); return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EScriptID.MiniMapItemOperation:
                    {
                        var miniMap = param[1] as MiniMap;
                        if (!miniMap) break;

                        var go = param[3] as GameObject;
                        if (!go) break;

                        switch (param[2] as string)
                        {
                            case "添加":
                                {
                                    var rt = param[4] as RectTransform;
                                    if (!rt) break;

                                    miniMap.CreateItem(rt, go.transform);
                                    return ReturnValue.Yes;
                                }
                            case "移除":
                                {
                                    miniMap.DestroyItem(go.transform);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EScriptID.SetMiniMapTeleportRule:
                    {
                        var miniMap = param[1] as MiniMapTeleport;
                        if (!miniMap) break;

                        switch (param[2] as string)
                        {
                            case "无":
                                {
                                    miniMap._teleportRule = MiniMapTeleport.ETeleportRule.None;
                                    return ReturnValue.Yes;
                                }
                            case "传送需射线撞击":
                                {
                                    miniMap._teleportRule = MiniMapTeleport.ETeleportRule.NeedRayHit;
                                    return ReturnValue.Yes;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EScriptID.SetMiniMapTeleportHeightOffset:
                    {
                        var miniMap = param[1] as MiniMapTeleport;
                        if (!miniMap) break;

                        miniMap._heightOffset = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EScriptID.ExcuteInteract:
                    {
                        var interactor = param[1] as InteractObject;
                        if (!interactor) break;

                        return interactor.TryInteract(param[2] as string, out _, param[3] as InteractableVirtual) ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.SetInteractableEntitySelected:
                    {
                        var interactableEntity = param[1] as InteractableEntity;
                        if (!interactableEntity) break;

                        interactableEntity.isSelected = CommonFun.BoolChange(interactableEntity.isSelected, (EBool)param[2]);

                        return ReturnValue.Yes;
                    }
                case EScriptID.SetInteractableEntityActived:
                    {
                        var interactableEntity = param[1] as InteractableEntity;
                        if (!interactableEntity) break;

                        interactableEntity.isActived = CommonFun.BoolChange(interactableEntity.isActived, (EBool)param[2]);

                        return ReturnValue.Yes;
                    }
            }
            return ReturnValue.No;
        }
    }
}
