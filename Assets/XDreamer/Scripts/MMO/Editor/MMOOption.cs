using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.ComponentModel;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;

namespace XCSJ.EditorMMO
{
    [XDreamerPreferences]
    [Name(MMOHelper.CategoryName)]
    [Import]
    public class MMOOption : XDreamerOption<MMOOption>
    {
        [Name("同步变量高亮")]
        public bool syncVarHighlight = true;

        [Name("同步变量高亮颜色")]
        [Json(exportString = true)]
        public Color syncVarHighlightColor = new Color(0, 0.5490196f, 0.8117647f, 1f);
    }
}
