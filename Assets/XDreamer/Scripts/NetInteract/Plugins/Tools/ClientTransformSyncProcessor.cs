using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 客户端变换同步处理器
    /// </summary>
    [Name("客户端变换同步处理器")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class ClientTransformSyncProcessor : ClientProcessor
    {
        /// <summary>
        /// 网络变换同步数据
        /// </summary>
        [Name("网络变换同步数据")]
        public NetTransformDatas _netTransformDatas = new NetTransformDatas();

        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netAnswer"></param>
        protected override void OnReceived(Client client, NetAnswer netAnswer)
        {
            if (!(netAnswer is NetTransformAnswer answer)) return;
            if (!ValidClient(client)) return;

            if (answer.data._guid != _netTransformDatas._guid) return;

            foreach (var data0 in _netTransformDatas._datas)
            {
                foreach (var data1 in answer.data._datas)
                {
                    if (data1.TryApplyTo(data0)) break;
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            var data = new TransformData();
            data._transform = this.transform;
            _netTransformDatas._datas.Add(data);
        }
    }
}
