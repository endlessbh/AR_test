using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键获取组手：层级变量中的层级键扩展获取机制
    /// </summary>
    public static class HierarchyKeyGetHelper
    {
        /// <summary>
        /// 数量
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("数量")]
        [Tip("尝试获取子级变量数目：针对列表（数组）、字典（对象）类型时返回其元素数量；针对字符串，返回字符串的长度信息；", "Try to get the number of child variables: return the number of elements for list (array) and Dictionary (object) types; For a string, return the length information of the string;")]
        [HierarchyKey(EHierarchyKeyMode.Get, "数量", nameof(Count), "Array_Length", "List_Count", "Dictionary_Count", "String_Length")]
        public static object Count(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChildCount(out var count)) return count;
                        break;
                    }
                case EVarType.String:
                    {
                        return hierarchyVar.stringValue.Length;
                    }
            }
            return default;
        }

        /// <summary>
        /// X
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("X")]
        [Tip("尝试获取子级变量值中形如'X/Y/Z/W'格式的第1个元素X的信息", "Try to get the information of the first element X in the value of the child variable in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Get, "x", "X", "XYZW_X", "Color_R", "Rect_X")]
        public static object X(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        if (hierarchyVar.TryGetChild(0, false, out var dstVar)) return dstVar?.stringValue;
                        break;
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar?.stringValue;
                        if (hierarchyVar.TryGetChild("X", out var dstVar1)) return dstVar1?.stringValue;
                        if (hierarchyVar.TryGetChild("x", out var dstVar2)) return dstVar2?.stringValue;
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 0) return array[0];
                        break;
                    }
            }
            return default;
        }

        /// <summary>
        /// Y
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("Y")]
        [Tip("尝试获取子级变量值中形如'X/Y/Z/W'格式的第2个元素Y的信息", "Try to get the information of the second element Y in the value of the child variable in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Get, "y", "Y", "XYZW_Y", "Color_G", "Rect_Y")]
        public static object Y(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        if (hierarchyVar.TryGetChild(1, false, out var dstVar)) return dstVar?.stringValue;
                        break;
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar?.stringValue;
                        if (hierarchyVar.TryGetChild("Y", out var dstVar1)) return dstVar1?.stringValue;
                        if (hierarchyVar.TryGetChild("y", out var dstVar2)) return dstVar2?.stringValue;
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 1) return array[1];
                        break;
                    }
            }
            return default;
        }

        /// <summary>
        /// Z
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("Z")]
        [Tip("尝试获取子级变量值中形如'X/Y/Z/W'格式的第3个元素Z的信息", "Try to get the information of the third element Z in the child variable value in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Get, "z", "Z", "XYZW_Z", "Color_B", "Rect_W")]
        public static object Z(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        if (hierarchyVar.TryGetChild(2, false, out var dstVar)) return dstVar?.stringValue;
                        break;
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar?.stringValue;
                        if (hierarchyVar.TryGetChild("Z", out var dstVar1)) return dstVar1?.stringValue;
                        if (hierarchyVar.TryGetChild("z", out var dstVar2)) return dstVar2?.stringValue;
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 2) return array[2];
                        break;
                    }
            }
            return default;
        }

        /// <summary>
        /// W
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("W")]
        [Tip("尝试获取子级变量值中形如'X/Y/Z/W'格式的第4个元素W的信息", "Try to get the information of the fourth element w in the value of the child variable in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Get, "w", "W", "XYZW_W", "Color_A", "Rect_H")]
        public static object W(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        if (hierarchyVar.TryGetChild(3, false, out var dstVar)) return dstVar?.stringValue;
                        break;
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar?.stringValue;
                        if (hierarchyVar.TryGetChild("W", out var dstVar1)) return dstVar1?.stringValue;
                        if (hierarchyVar.TryGetChild("w", out var dstVar2)) return dstVar2?.stringValue;
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 3) return array[3];
                        break;
                    }
            }
            return default;
        }
    }
}
