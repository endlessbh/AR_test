using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键设置组手：层级变量中的层级键扩展设置机制
    /// </summary>
    public static class HierarchyKeySetHelper
    {
        /// <summary>
        /// 数量
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("数量")]
        [Tip("尝试设置子级变量数目：仅针对列表（数组）类型时有效;末尾操作，多删除，少补空字符串；", "Try to set the number of child variables: valid only for list (array) types; Final operation: delete more and fill in fewer empty strings;")]
        [HierarchyKey(EHierarchyKeyMode.Set, "数量", nameof(Count), "Array_Length", "List_Count", "Dictionary_Count", "String_Length")]
        public static bool Count(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, object varValue)
        {            
            return Converter.instance.TryConvertTo<int>(varValue, out var count) && hierarchyVar.TrySetChildCount(count);
        }

        /// <summary>
        /// X
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("X")]
        [Tip("尝试设置子级变量值中形如'X/Y/Z/W'格式的第1个元素X的信息", "Try to set the information of the first element X in the child variable value in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Set, "x", "X", "XYZW_X", "Color_R", "Rect_X")]
        public static bool X(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, object varValue)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        return hierarchyVar.TryGetChild(0, false, out var dstVar) && dstVar.TrySetValue(varValue, EVarType.String);
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("X", out var dstVar1)) return dstVar1.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("x", out var dstVar2)) return dstVar2.TrySetValue(varValue, EVarType.String);
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 0)
                        {
                            array[0] = varValue.ToScriptParamString();
                            return hierarchyVar.TrySetValue(array.ToStringDirect("/"), EVarType.String);
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// Y
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("Y")]
        [Tip("尝试设置子级变量值中形如'X/Y/Z/W'格式的第2个元素Y的信息", "Try to set the information of the second element Y in the child variable value in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Set, "y", "Y", "XYZW_Y", "Color_G", "Rect_Y")]
        public static bool Y(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, object varValue)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        return hierarchyVar.TryGetChild(1, false, out var dstVar) && dstVar.TrySetValue(varValue, EVarType.String);
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("Y", out var dstVar1)) return dstVar1.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("y", out var dstVar2)) return dstVar2.TrySetValue(varValue, EVarType.String);
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 1)
                        {
                            array[1] = varValue.ToScriptParamString();
                            return hierarchyVar.TrySetValue(array.ToStringDirect("/"), EVarType.String);
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// Z
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("Z")]
        [Tip("尝试设置子级变量值中形如'X/Y/Z/W'格式的第3个元素Z的信息", "Try to set the information of the third element Z in the child variable value in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Set, "z", "Z", "XYZW_Z", "Color_B", "Rect_W")]
        public static bool Z(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, object varValue)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        return hierarchyVar.TryGetChild(2, false, out var dstVar) && dstVar.TrySetValue(varValue, EVarType.String);
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("Z", out var dstVar1)) return dstVar1.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("z", out var dstVar2)) return dstVar2.TrySetValue(varValue, EVarType.String);
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 2)
                        {
                            array[2] = varValue.ToScriptParamString();
                            return hierarchyVar.TrySetValue(array.ToStringDirect("/"), EVarType.String);
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// W
        /// </summary>
        /// <param name="varContext"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="extensionHierarchyKey"></param>
        /// <returns></returns>
        [Name("W")]
        [Tip("尝试设置子级变量值中形如'X/Y/Z/W'格式的第4个元素W的信息", "Try to set the information of the fourth element w in the child variable value in the form of 'x / Y / Z / W'")]
        [HierarchyKey(EHierarchyKeyMode.Set, "w", "W", "XYZW_W", "Color_A", "Rect_H")]
        public static bool W(IVarContext varContext, IHierarchyVar hierarchyVar, string extensionHierarchyKey, object varValue)
        {
            switch (hierarchyVar.varType)
            {
                case EVarType.Array:
                    {
                        return hierarchyVar.TryGetChild(3, false, out var dstVar) && dstVar.TrySetValue(varValue, EVarType.String);
                    }
                case EVarType.Dictionary:
                    {
                        if (hierarchyVar.TryGetChild(extensionHierarchyKey, out var dstVar)) return dstVar.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("W", out var dstVar1)) return dstVar1.TrySetValue(varValue, EVarType.String);
                        if (hierarchyVar.TryGetChild("w", out var dstVar2)) return dstVar2.TrySetValue(varValue, EVarType.String);
                        break;
                    }
                case EVarType.String:
                    {
                        var array = hierarchyVar.stringValue.GetSplitArray("/");
                        if (array != null && array.Length > 3)
                        {
                            array[3] = varValue.ToScriptParamString();
                            return hierarchyVar.TrySetValue(array.ToStringDirect("/"), EVarType.String);
                        }
                        break;
                    }
            }
            return false;
        }
    }
}
