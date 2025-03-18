using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 转换器扩展
    /// </summary>
    public static class ConverterExtension
    {
        static ConverterExtension() => Init();

        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            Converter.instance.Register<List<Color>, string>(ColorListToJson);
            Converter.instance.Register<string, List<Color>>(JsonToColorList);

            Converter.instance.Register<List<Material>, string>(MaterialListToJson);
            Converter.instance.Register<string, List<Material>>(JsonToMaterialList);

            Converter.instance.Register((Color32 c32) => string.Format("{0}/{1}/{2}/{3}", c32.r, c32.g, c32.b, c32.a));
            Converter.instance.Register((string s32) =>
            {
                var c32 = new Color32();
                var array = s32.GetSplitArray("/", StringSplitOptions.RemoveEmptyEntries);
                if (array != null && array.Length >= 4)
                {
                    byte.TryParse(array[0], out c32.r);
                    byte.TryParse(array[1], out c32.g);
                    byte.TryParse(array[2], out c32.b);
                    byte.TryParse(array[3], out c32.a);
                }
                return c32;
            });
        }

        private static string ColorListToJson(List<Color> colors)
        {
            var list = colors.ToList(c => CommonFun.ColorToString(c));
            return JsonHelper.ToJson(list);
        }

        private static List<Color> JsonToColorList(string jsonString)
        {
            var list = JsonHelper.ToObject<List<string>>(jsonString);
            return list?.ToList(str => CommonFun.StringToColor(str));
        }

        private static string MaterialListToJson(List<Material> colors)
        {
            var list = colors.ToList(c => c ? c.name : "");
            return JsonHelper.ToJson(list);
        }

        private static List<Material> JsonToMaterialList(string jsonString)
        {            
            var list = JsonHelper.ToObject<List<string>>(jsonString);
            return list?.ToList(str => UnityAssetObjectManager.instance.GetUnityAssetObject<Material>(str));
        }
    }
}
