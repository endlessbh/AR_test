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
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// MMO组件检查器
    /// </summary>
    [Name("MMO组件检查器")]
    [CustomEditor(typeof(MMOMB), true)]
    public class MMOMBInspector : MMOMBInspector<MMOMB> { }

    /// <summary>
    /// MMO组件检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MMOMBInspector<T> : MBInspector<T> where T : MMOMB
    {
        private static CategoryList _categoryList = null;
        public static CategoryList categoryList
        {
            get
            {
                if (_categoryList == null)
                {
                    _categoryList = EditorToolsHelper.GetWithPurposes(MMOHelper.ToolPurpose);
                }
                return _categoryList;
            }
        }
    }
}
