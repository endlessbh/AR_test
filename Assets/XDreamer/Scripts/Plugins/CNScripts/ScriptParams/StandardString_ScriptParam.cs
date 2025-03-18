using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.ScriptParams
{
    /// <summary>
    /// 标准字符串脚本参数
    /// </summary>
    [ScriptParamType(EParamType.StandardString)]
    [ScriptParamType(EParamType.Combo)]
    [ScriptParamType(EParamType.ScriptEventType)]
    [ScriptParamType(EParamType.UserDefineFun)]
    [ScriptParamType(EParamType.File)]
    [ScriptParamType(EParamType.SaveFile)]
    [ScriptParamType(EParamType.OpenFolder)]
    [ScriptParamType(EParamType.SaveFolder)]
    [ScriptParamType(EParamType.GlobalVariableName)]
    [ScriptParamType(EParamType.Variable)]
    [ScriptParamType(EParamType.Array)]
    [ScriptParamType(EParamType.Dictionary)]
    public class StandardString_ScriptParam : StringScriptParam { }

    /// <summary>
    /// 字符串脚本参数
    /// </summary>
    [ScriptParamType(EParamType.String)]
    public class String_ScriptParam : StringScriptParam
    {
        static Dictionary<string, string> formatCache = new Dictionary<string, string>();

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject)
        {
            if(paramObject is string str)
            {
                if (formatCache.TryGetValue(str, out var value)) return value;

                value = ScriptHelper.SymbolFormat(str);
                formatCache[str] = value;
                return value;
            }
            return "";
        }

        static Dictionary<string, string> unformatCache = new Dictionary<string, string>();

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString)
        {
            if (unformatCache.TryGetValue(paramString, out var value)) return value;

            value = ScriptHelper.UnSymbolFormat(paramString);
            unformatCache[paramString] = value;
            return value;
        }
    }

    /// <summary>
    /// 键码脚本参数
    /// </summary>
    [ScriptParamType(EParamType.KeyCode)]
    public class KeyCode_ScriptParam : EnumScriptParam<KeyCode>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override KeyCode DefaultParamObject() => KeyCode.None;
    }

    /// <summary>
    /// 布尔脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Bool)]
    public class EBool_ScriptParam : EnumScriptParam<EBool>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override EBool DefaultParamObject() => EBool.No;
    }

    /// <summary>
    /// 布尔2脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Bool2)]
    public class EBool2_ScriptParam : EnumScriptParam<EBool2>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override EBool2 DefaultParamObject() => EBool2.No;
    }

    /// <summary>
    /// 坐标轴类型脚本参数
    /// </summary>
    [ScriptParamType(EParamType.CoordinateType)]
    public class ECoordinateType_ScriptParam : EnumScriptParam<ECoordinateType>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override ECoordinateType DefaultParamObject() => ECoordinateType.World;
    }

    /// <summary>
    /// 文本锚点脚本参数
    /// </summary>
    [ScriptParamType(EParamType.TextAnchor)]
    public class TextAnchor_ScriptParam : EnumScriptParam<TextAnchor>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override TextAnchor DefaultParamObject() => TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// 运行时平台脚本参数
    /// </summary>
    [ScriptParamType(EParamType.RuntimePlatform)]
    public class RuntimePlatform_ScriptParam : EnumScriptParam<RuntimePlatform>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override RuntimePlatform DefaultParamObject() => RuntimePlatform.OSXEditor;
    }

    /// <summary>
    /// 鼠标按钮脚本参数
    /// </summary>
    [ScriptParamType(EParamType.MouseButton)]
    public class EMouseButtonType_ScriptParam : EnumScriptParam<EMouseButtonType>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override EMouseButtonType DefaultParamObject() => EMouseButtonType.Left;
    }

    /// <summary>
    /// Unity资产对象脚本参数
    /// </summary>
    [ScriptParamType(EParamType.UnityAssetObjectType)]
    public class EUnityAssetObjectType_ScriptParam : EnumScriptParam<EUnityAssetObjectType>
    {
        /// <summary>
        /// 默认对象
        /// </summary>
        /// <returns></returns>
        public override EUnityAssetObjectType DefaultParamObject() => EUnityAssetObjectType.Object;
    }

    /// <summary>
    /// 组件类型脚本参数
    /// </summary>
    [ScriptParamType(EParamType.ComponentType)]
    public class ComponentType_ScriptParam : ScriptParam<Type>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.ComponentTypeToString(paramObject as Type);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToComponentType(paramString);
    }

    /// <summary>
    /// 矩形脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Rect)]
    public class Rect_ScriptParam : ScriptParam<Rect>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.RectToString((Rect)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToRect(paramString);
    }

    /// <summary>
    /// 整型矩形脚本参数
    /// </summary>
    [ScriptParamType(EParamType.RectInt)]
    public class RectInt_ScriptParam : ScriptParam<RectInt>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.RectIntToString((RectInt)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToRectInt(paramString);
    }

    /// <summary>
    /// 二维向量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Vector2)]
    [ScriptParamType(EParamType.MinMaxSlider)]
    public class Vector2_ScriptParam : ScriptParam<Vector2>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.Vector2ToString((Vector2)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToVector2(paramString);
    }

    /// <summary>
    /// 整型二维向量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Vector2Int)]
    public class Vector2Int_ScriptParam : ScriptParam<Vector2Int>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.Vector2IntToString((Vector2Int)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToVector2Int(paramString);
    }

    /// <summary>
    /// 三维向量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Vector3)]
    public class Vector3_ScriptParam : ScriptParam<Vector3>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.Vector3ToString((Vector3)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToVector3(paramString);
    }

    /// <summary>
    /// 整型三维向量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Vector3Int)]
    public class Vector3Int_ScriptParam : ScriptParam<Vector3Int>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.Vector3IntToString((Vector3Int)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToVector3Int(paramString);
    }

    /// <summary>
    /// 四维向量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Vector4)]
    public class Vector4_ScriptParam : ScriptParam<Vector4>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.Vector4ToString((Vector4)(paramObject));

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToVector4(paramString);
    }

    /// <summary>
    /// 整型脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Int)]
    [ScriptParamType(EParamType.IntSlider)]
    [ScriptParamType(EParamType.IntPopup)]
    public class Int_ScriptParam : ScriptParam<int>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => paramObject?.ToString() ?? "";

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToInt(paramString);
    }

    /// <summary>
    /// 长整型脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Long)]
    public class Long_ScriptParam : ScriptParam<long>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => paramObject?.ToString() ?? "";

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToLong(paramString);
    }

    /// <summary>
    /// 浮点数脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Float)]
    [ScriptParamType(EParamType.FloatSlider)]
    public class Float_ScriptParam : ScriptParam<float>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => paramObject?.ToString() ?? "";

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToFloat(paramString);
    }

    /// <summary>
    /// 双精度浮点数脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Double)]
    public class Double_ScriptParam : ScriptParam<double>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => paramObject?.ToString() ?? "";

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToDouble(paramString);
    }

    /// <summary>
    /// 游戏对象脚本参数
    /// </summary>
    [ScriptParamType(EParamType.GameObject)]
    public class GameObject_ScriptParam : UnityObjectScriptParam<GameObject>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => CommonFun.GameObjectToString(paramObject as GameObject) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.GameObjectToString(paramObject as GameObject);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToGameObject(paramString);
    }

    /// <summary>
    /// 游戏对象组件脚本参数
    /// </summary>
    [ScriptParamType(EParamType.GameObjectComponent)]
    [ScriptParamType(EParamType.ComponentGameObject)]
    public class GameObjectComponent_ScriptParam : UnityObjectScriptParam<Component>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => CommonFun.GameObjectComponentToString(paramObject as Component) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.GameObjectComponentToString(paramObject as Component);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToGameObjectComponent(paramString);
    }

    /// <summary>
    /// 游戏对象脚本事件脚本参数
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEvent)]
    public class GameObjectSciptEvent_ScriptParam : UnityObjectScriptParam<Component>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => CommonFun.GameObjectScriptEventToString(paramObject as Component) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.GameObjectScriptEventToString(paramObject as Component);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToGameObjectScriptEvent(paramString);
    }

    /// <summary>
    /// 游戏对象脚本事件函数脚本参数
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEventFunction)]
    public class GameObjectSciptEventFunction_ScriptParam : ScriptParam<Function>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => CommonFun.GameObjectScriptEventFunctionToString(paramObject as Function) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.GameObjectScriptEventFunctionToString(paramObject as Function);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToGameObjectScriptEventFunction(paramString);
    }

    /// <summary>
    /// 游戏对象脚本事件变量脚本参数
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEventVariable)]
    public class GameObjectSciptEventVariable_ScriptParam : ScriptParam<GameObjectScriptEventVariableData>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => ParamObjectToString(paramObject) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.GameObjectScriptEventVariableDataToString(paramObject as GameObjectScriptEventVariableData);

        public override object StringToParamObject(string paramString) => CommonFun.StringToGameObjectScriptEventVariableData(paramString);
    }

    /// <summary>
    /// 包围盒脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Bounds)]
    public class Bounds_ScriptParam : ScriptParam<Bounds>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject)=> CommonFun.BoundsToString((Bounds)paramObject);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToBounds(paramString);
    }

    /// <summary>
    /// 整型包围盒脚本参数
    /// </summary>
    [ScriptParamType(EParamType.BoundsInt)]
    public class BoundsInt_ScriptParam : ScriptParam<BoundsInt>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.BoundsIntToString((BoundsInt)paramObject);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToBoundsInt(paramString);
    }

    /// <summary>
    /// 颜色脚本参数
    /// </summary>
    [ScriptParamType(EParamType.Color)]
    public class Color_ScriptParam : ScriptParam<Color>
    {
        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.ColorToString((Color)paramObject);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToColor(paramString);
    }

    /// <summary>
    /// Unity资源对象脚本参数
    /// </summary>
    [ScriptParamType(EParamType.UnityAssetObject)]
    public class UnityAssetObject_ScriptParam : UnityObjectScriptParam<UnityEngine.Object>
    {
        /// <summary>
        /// 检查类型对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override bool CheckParamObject(string paramString, object paramObject) => CommonFun.UnityAssetObjectToString(paramObject as UnityEngine.Object) == paramString;

        /// <summary>
        /// 参数对象转字符串
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public override string ParamObjectToString(object paramObject) => CommonFun.UnityAssetObjectToString(paramObject as UnityEngine.Object);

        /// <summary>
        /// 字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => CommonFun.StringToUnityAssetObject(paramString);
    }
}
