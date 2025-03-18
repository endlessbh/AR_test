using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.Base;
using XCSJ.PluginMMO.CNScripts;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.Scripts;

namespace XCSJ.PluginMMO.Kernel
{
    /// <summary>
    /// 默认MMO处理器
    /// </summary>
    public class DefaultMMOHandler : InstanceClass<DefaultMMOHandler>, IMMOHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            MMOHandler.handler = instance;
        }

        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public IMMOClient CreateClient(MMOManager manager) => new MMOClient(manager);

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(MMOManager manager) => Script.GetScriptsOfEnum<EMMOScriptID>(manager);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue ExecuteScript(MMOManager manager, int id, ScriptParamList param) => ExecuteScript(manager, (EMMOScriptID)id, param);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private ReturnValue ExecuteScript(MMOManager manager, EMMOScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                case EMMOScriptID.MMOGameObjectClone:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var netIdentity = go.GetComponent<NetIdentity>();
                        if (!netIdentity) break;
                        var newNetIdentity = MMOHelper.Clone(netIdentity, CommonFun.BoolChange(netIdentity.hasAccess, (EBool)param[2]));
                        if (!newNetIdentity) break;
                        return ReturnValue.True(CommonFun.GameObjectToString(newNetIdentity.gameObject));
                    }
                case EMMOScriptID.MMOGameObjectDestory:
                    {
                        return ReturnValue.Create(MMOHelper.Destory(param[1] as GameObject));
                    }
                case EMMOScriptID.MMOGetNetProperty:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var mb = go.GetComponent<NetProperty>();
                        if (!mb) break;
                        if (mb.GetProperty(param[2] as string) is Property property)
                        {
                            return ReturnValue.True(property.value);
                        }
                        break;
                    }
                case EMMOScriptID.MMOSetNetProperty:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        var mb = go.GetComponent<NetProperty>();
                        if (!mb) break;
                        return ReturnValue.Create(mb.SetProperty(param[2] as string, param[3] as string, EBool2.Yes == (EBool2)param[4]) != null);
                    }
                case EMMOScriptID.MMOGetNetState:
                    {
                        return ReturnValue.True(manager.netState.ToString());
                    }
                case EMMOScriptID.MMOGetPing:
                    {
                        return ReturnValue.True(manager.ping.ToString());
                    }
                case EMMOScriptID.MMOControl:
                    {
                        var mmo = MMOManager.instance;
                        if (!mmo) break;
                        switch(param[1] as string)
                        {
                            case "初始化网络": mmo.InitNet(); break;
                            case "连接": mmo.Connect(); break;
                            case "关闭": mmo.Close(); break;
                            case "登入": mmo.Login(); break;
                            case "登出": mmo.Logout(); break;
                            case "加入房间": mmo.JoinRoom(); break;
                            case "退出房间": mmo.QuitRoom(); break;
                            case "清理登录信息": mmo.ClearLoginInfo(); break;
                            case "清理房间信息": mmo.ClearRoomInfo(); break;
                            case "完全关闭":
                            case "强制初始化":
                                {
                                    mmo.ForceInit();
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EMMOScriptID.MMOGetCurrentRoomInfo:
                    {
                        var mmo = MMOManager.instance;
                        if (!mmo) break;
                        switch (param[1] as string)
                        {
                            case "App名": return ReturnValue.True(manager.roomInfo.appName);
                            case "App编号": return ReturnValue.True(manager.roomInfo.appGuid);
                            case "App版本": return ReturnValue.True(manager.roomInfo.appVersion);
                            case "房间编号": return ReturnValue.True(manager.roomInfo.roomGuid);
                            case "房间名": return ReturnValue.True(manager.roomInfo.name);
                            case "是否需要密码": return ReturnValue.True(manager.roomInfo.pwd);
                            case "在线人数": return ReturnValue.True(manager.roomInfo.userCount);
                            case "总人数": return ReturnValue.True(manager.roomInfo.limitCount);
                            default: return ReturnValue.No;
                        }
                    }
                case EMMOScriptID.MMOCreatePlayer:
                    {
                        var creater = param[1] as MMOPlayerCreater;
                        if (creater)
                        {
                            creater.CreatePlayer(param[2] as string);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EMMOScriptID.MMOSendMsg:
                    {
                        var netChat = param[1] as NetChat;
                        if (netChat)
                        {
                            switch (param[2] as string)
                            {
                                case "文本":
                                    {
                                        var str = param[3] as string;
                                        if (!string.IsNullOrEmpty(str))
                                        {
                                            netChat.Send(str); 
                                            return ReturnValue.Yes;
                                        }
                                        break;
                                    }
                                case "音频":
                                    {
                                        var audioSource = param[4] as AudioSource;
                                        if (audioSource)
                                        {
                                            netChat.Send(audioSource);
                                            return ReturnValue.Yes;
                                        }
                                        break;
                                    }
                                default: break;
                            }
                            
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }       
    }
}
