using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.PropertyHandler")]
    public class PropertyHandler : LinkType<PropertyHandler>
    {
        public PropertyHandler(object obj) : base(obj) { }

        #region hasPropertyDrawer

        public static XPropertyInfo hasPropertyDrawer_FieldInfo { get; } = GetXPropertyInfo(nameof(hasPropertyDrawer));

        public bool hasPropertyDrawer => (bool)hasPropertyDrawer_FieldInfo.GetValue(obj);

        #endregion

    }
}
