using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.Scripts;

namespace XCSJ.PluginMMO.CNScripts
{
    /// <summary>
    /// MMO脚本ID
    /// </summary>
    [Name("MMO脚本ID")]
    public enum EMMOScriptID
    {
        _Begin = IDRange.Begin,

        #region 多人在线MMO脚本-目录
        /// <summary>
        /// 多人在线MMO脚本
        /// </summary>
        [ScriptName("多人在线MMO脚本", nameof(MMOCategory), EGrammarType.Category)]
        [ScriptDescription("多人在线MMO脚本的相关脚本目录；本目录下的中文脚本命令会不定期的调整；功能稳定后可能将对应中文脚本命令转移到功能集合所在的DLL管理器中；")]
        #endregion
        MMOCategory,

        #region MMO游戏对象克隆
        /// <summary>
        /// MMO游戏对象克隆
        /// </summary>
        [ScriptName("MMO游戏对象克隆", nameof(MMOGameObjectClone))]
        [ScriptDescription("MMO游戏对象克隆；被克隆的游戏对象必须携带网络标识组件;会在待克隆游戏对象位置克隆一个包括子级游戏对象以及完备组件的新游戏对象；并执行网络同步；")]
        [ScriptReturn("成功返回克隆后的 游戏对象名称路径（即修改后的游戏对象描述字符串） ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象（限定游戏对象必须有NetIdentity组件）:", typeof(NetIdentity))]
        [ScriptParams(2, EParamType.Bool, "权限:", defaultObject = EBool.No)]
        #endregion
        MMOGameObjectClone,

        #region MMO游戏对象删除
        /// <summary>
        /// MMO游戏对象删除
        /// </summary>
        [ScriptName("MMO游戏对象删除", nameof(MMOGameObjectDestory))]
        [ScriptDescription("MMO游戏对象删除；被删除的游戏对象必须携带网络标识组件;会将游戏对象本身以及所属的子级游戏对象全部删除掉；并执行网络同步；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象（限定游戏对象必须有NetIdentity组件）:", typeof(NetIdentity))]
        #endregion
        MMOGameObjectDestory,

        #region MMO获取网络属性
        /// <summary>
        /// MMO获取网络属性
        /// </summary>
        [ScriptName("MMO获取网络属性", nameof(MMOGetNetProperty))]
        [ScriptDescription("MMO获取网络属性；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象（限定游戏对象必须有NetProperty组件）:", typeof(NetProperty))]
        [ScriptParams(2, EParamType.String, "属性名:")]
        #endregion
        MMOGetNetProperty,

        #region MMO设置网络属性
        /// <summary>
        /// MMO设置网络属性
        /// </summary>
        [ScriptName("MMO设置网络属性", nameof(MMOSetNetProperty))]
        [ScriptDescription("MMO设置网络属性；成功设置属性后会执行网络同步；")]
        [ScriptReturn("成功返回 对应的属性值 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象（限定游戏对象必须有NetProperty组件）:", typeof(NetProperty))]
        [ScriptParams(2, EParamType.String, "属性名:")]
        [ScriptParams(3, EParamType.String, "属性值:")]
        [ScriptParams(4, EParamType.Bool2, "强制设置:", defaultObject = EBool2.No)]
        #endregion
        MMOSetNetProperty,

        #region MMO获取网络状态
        /// <summary>
        /// MMO获取网络状态
        /// </summary>
        [ScriptName("MMO获取网络状态", nameof(MMOGetNetState))]
        [ScriptDescription("MMO获取网络状态；")]
        [ScriptReturn("成功返回 网络状态值 ; 失败返回 #False ;")]
        #endregion
        MMOGetNetState,

        #region MMO获取Ping
        /// <summary>
        /// MMO获取Ping
        /// </summary>
        [ScriptName("MMO获取Ping", nameof(MMOGetPing))]
        [ScriptDescription("MMO获取Ping；")]
        [ScriptReturn("成功返回 Ping值 ,单位毫秒; 失败返回 #False ;")]
        #endregion
        MMOGetPing,

        #region MMO控制
        /// <summary>
        /// MMO控制
        /// </summary>
        [ScriptName("MMO控制", nameof(MMOControl))]
        [ScriptDescription("MMO控制；使用已设置的各项信息执行对应命令；")]
        [ScriptParams(1, EParamType.Combo, "控制命令:", "初始化网络", "连接", "关闭", "登入", "登出", "加入房间", "退出房间", "清理登录信息", "清理房间信息", "完全关闭", "强制初始化")]
        [ScriptReturn("成功返回 #True; 失败返回 #False ;")]
        #endregion
        MMOControl,

        #region MMO获取当前房间信息
        /// <summary>
        /// MMO获取当前房间信息
        /// </summary>
        [ScriptName("MMO获取当前房间信息", nameof(MMOGetCurrentRoomInfo))]
        [ScriptDescription("MMO获取当前房间信息；获取房间的人数和总人数；")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "App名", "App编号", "App版本", "房间编号", "房间名", "是否需要密码", "在线人数", "总人数")]
        [ScriptReturn("成功返回 所需信息; 失败返回 #False ;")]
        #endregion
        MMOGetCurrentRoomInfo,

        #region MMO玩家
        /// <summary>
        /// 创建玩家
        /// </summary>
        [ScriptName("创建玩家", nameof(MMOCreatePlayer))]
        [ScriptDescription("创建玩家；使用名称创建玩家")]
        [ScriptParams(1, EParamType.ComponentGameObject, "玩家:", typeof(MMOPlayerCreater))]
        [ScriptParams(2, EParamType.String, "玩家昵称:")]
        [ScriptReturn("成功返回  #True; 失败返回 #False ;")]
        #endregion
        MMOCreatePlayer,

        #region 发送信息
        /// <summary>
        /// 发送信息
        /// </summary>
        [ScriptName("发送信息", nameof(MMOSendMsg))]
        [ScriptDescription("发送信息；使用名称创建玩家")]
        [ScriptParams(1, EParamType.ComponentGameObject, "网络聊天:", typeof(NetChat))]
        [ScriptParams(2, EParamType.Combo, "信息类型:", "文本", "音频")]
        [ScriptParams(3, EParamType.String, "文本:")]
        [ScriptParams(4, EParamType.GameObjectComponent, "音频:", typeof(AudioSource))]
        [ScriptReturn("成功返回  #True; 失败返回 #False ;")]
        #endregion
        MMOSendMsg
    }
}

