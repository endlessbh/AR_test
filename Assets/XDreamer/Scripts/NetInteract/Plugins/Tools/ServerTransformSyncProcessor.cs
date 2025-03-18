using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 服务器变换同步处理器
    /// </summary>
    [Name("服务器变换同步处理器")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class ServerTransformSyncProcessor : ServerProcessor
    {
        /// <summary>
        /// 网络变换同步数据
        /// </summary>
        [Name("网络变换同步数据")]
        public NetTransformDatas _netTransformDatas = new NetTransformDatas();

        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="netQuestion"></param>
        protected override void OnReceived(Server server, NetQuestion netQuestion)
        {
            if (!(netQuestion is NetTransformQuestion question)) return;
            if (!ValidServer(server)) return;

            if (question.data._guid != _netTransformDatas._guid) return;

            foreach(var data0 in _netTransformDatas._datas)
            {
                foreach(var data1 in question.data._datas)
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
