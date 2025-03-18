using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    ///// <summary>
    ///// 游戏对象插槽：用于归类的组对象，不支持组嵌套组
    ///// </summary>
    //[ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    //[Name(Title, nameof(GameObjectSocket))]
    //[XCSJ.Attributes.Icon(EIcon.GameObject)]
    //[Tip("被用于匹配对象位置的数据结构")]
    //public class GameObjectSocket : StateComponent<GameObjectSocket>
    //{
    //    /// <summary>
    //    /// 标题
    //    /// </summary>
    //    public const string Title = "游戏对象插槽";

    //    /// <summary>
    //    /// Title
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <returns></returns>
    //    [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
    //    [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    //    [Name(Title, nameof(SingleGroup))]
    //    [Tip("创建组")]
    //    [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
    //    public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

    //    /// <summary>
    //    /// 游戏对象插槽
    //    /// </summary>
    //    [Name("游戏对象插槽")]
    //    public GameObject _socket;

    //    /// <summary>
    //    /// 完成
    //    /// </summary>
    //    /// <returns></returns>
    //    public override bool Finished() => true;
    //}
}
