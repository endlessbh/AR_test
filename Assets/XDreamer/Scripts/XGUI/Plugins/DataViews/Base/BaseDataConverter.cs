using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    [RequireManager(typeof(XGUIManager))]
    public abstract class BaseDataConverter : InteractProvider
    {
        public virtual bool TryConvertTo(object input, Type outputType, out object output)
        {
            return Converter.instance.TryConvertTo(input, outputType, out output);
        }
    }

    public interface IConverter<TInput, TOutput> { }
}
