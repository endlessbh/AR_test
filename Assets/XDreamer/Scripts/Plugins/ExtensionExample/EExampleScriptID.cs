using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.ExtensionExample
{
    /// <summary>
    /// 案例ID区间
    /// </summary>
    public static class ExampleIDRange
    {
        public const int Begin = (int)EExtensionID._0xa;//34048
        public const int End = (int)EExtensionID._0xb - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//34048
        public const int MonoBehaviour = Begin + Fragment * 1;//34072
        public const int StateLib = Begin + Fragment * 2;//34096
        public const int Tools = Begin + Fragment * 3;//34120
        public const int Editor = Begin + Fragment * 4;//34144
    }

    /// <summary>
    /// 案例脚本ID
    /// </summary>
    [Name("案例脚本ID")]
    public enum EExampleScriptID
    {
        /// <summary>
        /// 最小脚本ID
        /// </summary>
        MinScriptID = EExtensionID._0xa,

        [ScriptName("扩展案例", nameof(ExtensionExample), EGrammarType.Category)]
        ExtensionExample,

        [ScriptName("输出Hello World", nameof(OutputHelloWorld))]
        OutputHelloWorld,

        [ScriptName("测试脚本参数字符串", nameof(TestScriptParamString))]
        [ScriptParams(1, EParamType.String, "字符串String:")]
        [ScriptParams(2, EParamType.String, "字符串String:", defaultObject = "默认字符串")]
        TestScriptParamString,

        [ScriptName("测试脚本参数标准字符串", nameof(TestScriptParamStandardString))]
        [ScriptParams(1, EParamType.StandardString, "标准字符串StandardString:")]
        [ScriptParams(2, EParamType.StandardString, "标准字符串StandardString:", defaultObject = "默认标准字符串")]
        TestScriptParamStandardString,

        [ScriptName("测试脚本参数组合", nameof(TestScriptParamCombo))]
        [ScriptParams(1, EParamType.Combo, "组合Combo:", "Combo0", "Combo1", "Combo2")]
        [ScriptParams(2, EParamType.Combo, "组合Combo:", "Combo0", "Combo1", "Combo2", defaultObject = "Combo1")]
        TestScriptParamCombo,

        [ScriptName("测试脚本参数文件", nameof(TestScriptParamFile))]
        [ScriptParams(1, EParamType.File, "打开文件File:")]
        [ScriptParams(2, EParamType.File, "打开文件File:", defaultObject = "C://")]
        TestScriptParamFile,

        [ScriptName("测试脚本参数保存文件", nameof(TestScriptParamSaveFile))]
        [ScriptParams(1, EParamType.SaveFile, "保存文件SaveFile:")]
        [ScriptParams(2, EParamType.SaveFile, "保存文件SaveFile:", defaultObject = "C://")]
        TestScriptParamSaveFile,

        [ScriptName("测试脚本参数打开文件夹", nameof(TestScriptParamOpenFolder))]
        [ScriptParams(1, EParamType.OpenFolder, "打开文件夹OpenFolder:")]
        [ScriptParams(2, EParamType.OpenFolder, "打开文件夹OpenFolder:", defaultObject = "C://")]
        TestScriptParamOpenFolder,

        [ScriptName("测试脚本参数保存文件夹", nameof(TestScriptParamSaveFolder))]
        [ScriptParams(1, EParamType.SaveFolder, "保存文件夹SaveFolder:")]
        [ScriptParams(2, EParamType.SaveFolder, "保存文件夹SaveFolder:", defaultObject = "C://")]
        TestScriptParamSaveFolder,

        [ScriptName("测试脚本参数用户自定义函数", nameof(TestScriptParamUserDefineFun))]
        [ScriptParams(1, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:")]
        [ScriptParams(2, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:", defaultObject = "无")]
        [ScriptParams(3, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:", defaultObject = 1)]
        TestScriptParamUserDefineFun,

        [ScriptName("测试脚本参数全局变量名", nameof(TestScriptParamGlobalVariableName))]
        [ScriptParams(1, EParamType.GlobalVariableName, "全局变量值GlobalVariableName:")]
        [ScriptParams(2, EParamType.GlobalVariableName, "全局变量值GlobalVariableName:", defaultObject = "全局变量名")]
        TestScriptParamGlobalVariableName,

        [ScriptName("测试脚本参数变量", nameof(TestScriptParamVariable))]
        [ScriptParams(1, EParamType.Variable, "变量Variable:")]
        [ScriptParams(2, EParamType.Variable, "变量Variable:", defaultObject = "_GO")]
        TestScriptParamVariable,

        [ScriptName("测试脚本参数布尔2类型", nameof(TestScriptParamBool2))]
        [ScriptParams(1, EParamType.Bool2, "布尔2Bool2:")]
        [ScriptParams(2, EParamType.Bool2, "布尔2Bool2:", defaultObject = EBool2.Yes)]
        [ScriptParams(3, EParamType.Bool2, "布尔2Bool2:", defaultObject = "Switch")]
        [ScriptParams(4, EParamType.Bool2, "布尔2Bool2:", defaultObject = true)]
        TestScriptParamBool2,

        [ScriptName("测试脚本参数布尔类型", nameof(TestScriptParamBool))]
        [ScriptParams(1, EParamType.Bool, "布尔Bool:")]
        [ScriptParams(2, EParamType.Bool, "布尔Bool:", defaultObject = EBool.Yes)]
        [ScriptParams(3, EParamType.Bool, "布尔Bool:", defaultObject = "Switch")]
        [ScriptParams(4, EParamType.Bool, "布尔Bool:", defaultObject = true)]
        TestScriptParamBool,

        [ScriptName("测试脚本参数整形", nameof(TestScriptParamInt))]
        [ScriptParams(1, EParamType.Int, "整形Int:")]
        [ScriptParams(2, EParamType.Int, "整形Int:", defaultObject = 1)]
        [ScriptParams(3, EParamType.Int, "整形Int:", defaultObject = "2")]
        TestScriptParamInt,

        [ScriptName("测试脚本参数整形滑动条", nameof(TestScriptParamIntSlider))]
        [ScriptParams(1, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10)]
        [ScriptParams(2, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10, defaultObject = 1)]
        [ScriptParams(3, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10, defaultObject = "2")]
        TestScriptParamIntSlider,

        [ScriptName("测试脚本参数整形弹出框", nameof(TestScriptParamIntPopup))]
        [ScriptParams(1, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "XXXXXX", 2, "name3", 3)]
        [ScriptParams(2, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "name2", 2, "name3", 3, defaultObject = 2)]
        [ScriptParams(3, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "name2", 2, "name3", 3, defaultObject = "3")]
        TestScriptParamIntPopup,

        [ScriptName("测试脚本参数长整型", nameof(TestScriptParamLong))]
        [ScriptParams(1, EParamType.Long, "长整型Long:")]
        [ScriptParams(2, EParamType.Long, "长整型Long:", defaultObject = 1L)]
        [ScriptParams(3, EParamType.Long, "长整型Long:", defaultObject = "1")]
        TestScriptParamLong,

        [ScriptName("测试脚本参数浮点数", nameof(TestScriptParamFloat))]
        [ScriptParams(1, EParamType.Float, "浮点数Float:")]
        [ScriptParams(2, EParamType.Float, "浮点数Float:", defaultObject = 1f)]
        [ScriptParams(3, EParamType.Float, "浮点数Float:", defaultObject = "2")]
        TestScriptParamFloat,

        [ScriptName("测试脚本参数浮点数滑动条", nameof(TestScriptParamFloatSlider))]
        [ScriptParams(1, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f)]
        [ScriptParams(2, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f, defaultObject = 1f)]
        [ScriptParams(3, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f, defaultObject = "2")]
        TestScriptParamFloatSlider,

        [ScriptName("测试脚本参数双精度浮点数", nameof(TestScriptParamDouble))]
        [ScriptParams(1, EParamType.Double, "双精度浮点数Double:")]
        [ScriptParams(2, EParamType.Double, "双精度浮点数Double:", defaultObject = 1.0)]
        [ScriptParams(3, EParamType.Double, "双精度浮点数Double:", defaultObject = "2")]
        TestScriptParamDouble,

        [ScriptName("测试脚本参数游戏对象", nameof(TestScriptParamGameObject))]
        [ScriptParams(1, EParamType.GameObject, "游戏对象GameObject:")]
        [ScriptParams(2, EParamType.GameObject, "游戏对象GameObject(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamGameObject,

        [ScriptName("测试脚本参数组件类型", nameof(TestScriptParamComponentType))]
        [ScriptParams(1, EParamType.ComponentType, "组件ComponentType:")]
        [ScriptParams(2, EParamType.ComponentType, "组件ComponentType(默认对象为UnityEngine.Transform):", defaultObject = typeof(Transform))]
        TestScriptParamComponentType,

        [ScriptName("测试脚本参数游戏对象组件", nameof(TestScriptParamGameObjectComponent))]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件GameObjectComponent:")]
        [ScriptParams(2, EParamType.GameObjectComponent, "游戏对象组件GameObjectComponent(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamGameObjectComponent,

        [ScriptName("测试脚本参数组件游戏对象", nameof(TestScriptParamComponentGameObject))]
        [ScriptParams(1, EParamType.ComponentGameObject, "组件游戏对象ComponentGameObject:")]
        [ScriptParams(2, EParamType.ComponentGameObject, "组件游戏对象ComponentGameObject(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamComponentGameObject,

        [ScriptName("测试脚本参数游戏对象脚本事件", nameof(TestScriptParamGameObjectSciptEvent))]
        [ScriptParams(1, EParamType.GameObjectSciptEvent, "游戏对象脚本事件GameObjectSciptEvent:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEvent, "游戏对象脚本事件GameObjectSciptEvent(限定类型UGUIButtonScriptEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEvent,

        [ScriptName("测试脚本参数游戏对象脚本事件函数", nameof(TestScriptParamGameObjectSciptEventFunction))]
        [ScriptParams(1, EParamType.GameObjectSciptEventFunction, "游戏对象脚本事件函数GameObjectSciptEventFunction:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEventFunction, "游戏对象脚本事件函数GameObjectSciptEventFunction(限定类型UGUIButtonScriptEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEventFunction,

        [ScriptName("测试脚本参数游戏对象脚本事件变量", nameof(TestScriptParamGameObjectSciptEventVariable))]
        [ScriptParams(1, EParamType.GameObjectSciptEventVariable, "游戏对象脚本事件变量GameObjectSciptEventVariable:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEventVariable, "游戏对象脚本事件变量(限定类型UGUIButtonScriptEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEventVariable,       

        [ScriptName("测试脚本参数脚本事件", nameof(TestScriptParamScriptEventType))]
        [ScriptParams(1, EParamType.ScriptEventType, "脚本事件ScriptEventType:")]
        [ScriptParams(2, EParamType.ScriptEventType, "脚本事件ScriptEventType:", defaultObject = "UGUI按钮脚本事件")]
        [ScriptParams(3, EParamType.ScriptEventType, "脚本事件ScriptEventType:", defaultObject = 2)]
        [ScriptParams(4, EParamType.ScriptEventType, "脚本事件ScriptEventType:", defaultObject = typeof(UGUIButtonScriptEvent))]
        [ScriptParams(5, EParamType.ScriptEventType, "脚本事件ScriptEventType:", typeof(UGUIButtonScriptEvent))]
        TestScriptParamScriptEventType,

        [ScriptName("测试脚本参数矩形", nameof(TestScriptParamRect))]
        [ScriptParams(1, EParamType.Rect, "矩形Rect:")]
        [ScriptParams(2, EParamType.Rect, "矩形Rect:", defaultObject = "1/2/3/4")]
        TestScriptParamRect,

        [ScriptName("测试脚本参数二维向量", nameof(TestScriptParamVector2))]
        [ScriptParams(1, EParamType.Vector2, "二维向量Vector2:")]
        [ScriptParams(2, EParamType.Vector2, "二维向量Vector2:", defaultObject = "1/2")]
        TestScriptParamVector2,

        [ScriptName("测试脚本参数三维向量", nameof(TestScriptParamVector3))]
        [ScriptParams(1, EParamType.Vector3, "三维向量Vector3:")]
        [ScriptParams(2, EParamType.Vector3, "三维向量Vector3:", defaultObject = "1/2/3")]
        TestScriptParamVector3,

        [ScriptName("测试脚本参数四维向量", nameof(TestScriptParamVector4))]
        [ScriptParams(1, EParamType.Vector4, "四维向量Vector4:")]
        [ScriptParams(2, EParamType.Vector4, "四维向量Vector4:", defaultObject = "1/2/3/4")]
        TestScriptParamVector4,

        [ScriptName("测试脚本参数最小最大滑动条", nameof(TestScriptParamMinMaxSlider))]
        [ScriptParams(1, EParamType.MinMaxSlider, "最小最大滑动条MinMaxSlider:", -10f, 10f)]
        [ScriptParams(2, EParamType.MinMaxSlider, "最小最大滑动条MinMaxSlider:", -10f, 10f, defaultObject = "-1/1")]
        TestScriptParamMinMaxSlider,

        [ScriptName("测试脚本参数颜色", nameof(TestScriptParamColor))]
        [ScriptParams(1, EParamType.Color, "颜色Color:")]
        [ScriptParams(2, EParamType.Color, "颜色Color:", defaultObject = "255/0/0/4")]
        TestScriptParamColor,

        [ScriptName("测试脚本参数界限", nameof(TestScriptParamBounds))]
        [ScriptParams(1, EParamType.Bounds, "界限Bounds:")]
        [ScriptParams(2, EParamType.Bounds, "界限Bounds:", defaultObject = "1/2/3/4/5/6")]
        TestScriptParamBounds,

        [ScriptName("测试脚本参数键码", nameof(TestScriptParamKeyCode))]
        [ScriptParams(1, EParamType.KeyCode, "键码KeyCode:")]
        [ScriptParams(2, EParamType.KeyCode, "键码KeyCode:", defaultObject = KeyCode.A)]
        [ScriptParams(3, EParamType.KeyCode, "键码KeyCode:", defaultObject = "B")]
        TestScriptParamKeyCode,

        [ScriptName("测试脚本参数鼠标按钮", nameof(TestScriptParamMouseButton))]
        [ScriptParams(1, EParamType.MouseButton, "鼠标按钮MouseButton:")]
        [ScriptParams(2, EParamType.MouseButton, "鼠标按钮MouseButton:", defaultObject = EMouseButtonType.Middle)]
        [ScriptParams(3, EParamType.MouseButton, "鼠标按钮MouseButton:", defaultObject = "Right")]
        TestScriptParamMouseButton,

        [ScriptName("测试脚本参数运行时平台", nameof(TestScriptParamRuntimePlatform))]
        [ScriptParams(1, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:")]
        [ScriptParams(2, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:", defaultObject = RuntimePlatform.WindowsPlayer)]
        [ScriptParams(3, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:", defaultObject = "Android")]
        TestScriptParamRuntimePlatform,

        [ScriptName("测试脚本参数Unity资源对象", nameof(TestScriptParamUnityAssetObject))]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "Unity资源对象UnityAssetObject:")]
        [ScriptParams(2, EParamType.UnityAssetObject, "Unity资源对象UnityAssetObject(限定类型Material):", typeof(Material))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamUnityAssetObject,

        [ScriptName("测试脚本参数Unity资源对象类型", nameof(TestScriptParamUnityAssetObjectType))]
        [ScriptParams(1, EParamType.UnityAssetObjectType, "Unity资源对象UnityAssetObjectType:")]
        TestScriptParamUnityAssetObjectType,

        [ScriptName("测试脚本参数坐标系类型", nameof(TestScriptParamCoordinateType))]
        [ScriptParams(1, EParamType.CoordinateType, "坐标系类型CoordinateType:")]
        [ScriptParams(2, EParamType.CoordinateType, "坐标系类型CoordinateType:", defaultObject = ECoordinateType.Screen)]
        [ScriptParams(3, EParamType.CoordinateType, "坐标系类型CoordinateType:", defaultObject = "ViewPort")]
        TestScriptParamCoordinateType,

        [ScriptName("测试脚本参数文本锚点", nameof(TestScriptParamTextAnchor))]
        [ScriptParams(1, EParamType.TextAnchor, "文本锚点TextAnchor:")]
        [ScriptParams(2, EParamType.TextAnchor, "文本锚点TextAnchor:", defaultObject = TextAnchor.MiddleCenter)]
        [ScriptParams(3, EParamType.TextAnchor, "文本锚点TextAnchor:", defaultObject = "LowerRight")]
        [ScriptParams(4, EParamType.TextAnchor, "文本锚点TextAnchor:", defaultObject = 7)]
        TestScriptParamTextAnchor,

        [ScriptName("测试脚本参数矩形Int", nameof(TestScriptParamRectInt))]
        [ScriptParams(1, EParamType.RectInt, "矩形RectInt:")]
        [ScriptParams(2, EParamType.RectInt, "矩形RectInt:", defaultObject = "1/2/3/4")]
        TestScriptParamRectInt,

        [ScriptName("测试脚本参数二维向量Int", nameof(TestScriptParamVector2Int))]
        [ScriptParams(1, EParamType.Vector2Int, "二维向量Vector2Int:")]
        [ScriptParams(2, EParamType.Vector2Int, "二维向量Vector2Int:", defaultObject = "1/2")]
        TestScriptParamVector2Int,

        [ScriptName("测试脚本参数三维向量Int", nameof(TestScriptParamVector3Int))]
        [ScriptParams(1, EParamType.Vector3Int, "三维向量Vector3Int:")]
        [ScriptParams(2, EParamType.Vector3Int, "三维向量Vector3Int:", defaultObject = "1/2/3")]
        TestScriptParamVector3Int,

        [ScriptName("测试脚本参数界限Int", nameof(TestScriptParamBoundsInt))]
        [ScriptParams(1, EParamType.BoundsInt, "界限BoundsInt:")]
        [ScriptParams(2, EParamType.BoundsInt, "界限BoundsInt:", defaultObject = "1/2/3/4/5/6")]
        TestScriptParamBoundsInt,

        MaxCurrent,
    }
}

