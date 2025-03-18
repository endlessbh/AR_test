using System.Collections.Generic;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Libraries.Tools
{
    /// <summary>
    /// 基础库
    /// </summary>
    public abstract class BaseLibrary : InteractProvider
    {

    }

    public abstract class BaseLibrary<TObject>: BaseLibrary
    {
        public List<TObject> _objects = new List<TObject>();
    }
}
