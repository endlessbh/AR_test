using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace XCSJ.EditorExtension.XAssets.Libs
{
    public static class EditorPath
    {
        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string FormatAssetPath(string path)
        {
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                path = path.Substring(index);
            }
            return NormalizePathSplash(path);
        }

        /// <summary>
        /// 判断是否是贴图
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsTexture(string path)
        {
            return PathEndWithExt(path, EditorConst.TextureExts);
        }

        /// <summary>
        /// 判断是否是材质
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsMaterial(string path)
        {
            return PathEndWithExt(path, EditorConst.MaterialExts);
        }

        /// <summary>
        /// 判断是否是模型
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsModel(string path)
        {
            return PathEndWithExt(path, EditorConst.ModelExts);
        }

        /// <summary>
        /// 判断是否是meta文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsMeta(string path)
        {
            return PathEndWithExt(path, EditorConst.MetaExts);
        }

        /// <summary>
        /// 判断是否是shader
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsShader(string path)
        {
            return PathEndWithExt(path, EditorConst.ShaderExts);
        }

        /// <summary>
        /// 是否是脚本文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsScript(string path)
        {
            return PathEndWithExt(path, EditorConst.ScriptExts);
        }

        /// <summary>
        /// 是否是音频文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAudio(string path)
        {
            return PathEndWithExt(path, EditorConst.AudioExts);
        }

        /// <summary>
        /// 是否是物理材质
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsPhysicMaterial(string path)
        {
            return PathEndWithExt(path, EditorConst.PhysicMaterialExts);
        }

        /// <summary>
        /// 是否是2D物理材质
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsPhysicMaterial2D(string path)
        {
            return PathEndWithExt(path, EditorConst.PhysicMaterial2DExts);
        }

        /// <summary>
        /// 是否是资源
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAsset(string path)
        {
            return PathEndWithExt(path, EditorConst.AssetExts);
        }

        /// <summary>
        /// 是否是音频混合器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAudioMixer(string path)
        {
            return PathEndWithExt(path, EditorConst.AudioMixerExts);
        }

        /// <summary>
        /// 是否是flare，做闪光的材质
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsFlare(string path)
        {
            return PathEndWithExt(path, EditorConst.FlareExts);
        }

        /// <summary>
        /// 是否是渲染贴图
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsRenderTexture(string path)
        {
            return PathEndWithExt(path, EditorConst.RenderTextureExts);
        }

        /// <summary>
        /// 是否烘焙贴图参数
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsLightmapParameters(string path)
        {
            return PathEndWithExt(path, EditorConst.LightmapParametersExts);
        }

        /// <summary>
        /// 是否是精灵图集
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsSpriteAtlas(string path)
        {
            return PathEndWithExt(path, EditorConst.SpriteAtlasExts);
        }

        /// <summary>
        /// 是否是动画控制器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAnimatorController(string path)
        {
            return PathEndWithExt(path, EditorConst.AnimatorControllerExts);
        }

        /// <summary>
        /// 是否是动画重载控制器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAnimatorOverrideController(string path)
        {
            return PathEndWithExt(path, EditorConst.AnimatorOverrideControllerExts);
        }

        /// <summary>
        /// 是否是动画遮罩
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsAvatarMask(string path)
        {
            return PathEndWithExt(path, EditorConst.AvatarMaskExts);
        }

        /// <summary>
        /// 是否是时间轴
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsTimeline(string path)
        {
            return PathEndWithExt(path, EditorConst.TimelineExts);
        }

        /// <summary>
        /// 是否是GUISkin
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsGUISkin(string path)
        {
            return PathEndWithExt(path, EditorConst.GUISkinExts);
        }

        /// <summary>
        /// 是否是字体
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsFont(string path)
        {
            return PathEndWithExt(path, EditorConst.FontExts);
        }

        /// <summary>
        /// 是否是立方体贴图
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsCubemap(string path)
        {
            return PathEndWithExt(path, EditorConst.CubemapExts);
        }

        /// <summary>
        /// 是否是笔刷
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsBrush(string path)
        {
            return PathEndWithExt(path, EditorConst.BrushExts);
        }

        /// <summary>
        /// 是否是地形层级
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsTerrainLayer(string path)
        {
            return PathEndWithExt(path, EditorConst.TerrainLayerExts);
        }

        /// <summary>
        /// 是否是unitypackage包
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsUnityPackage(string path)
        {
            return PathEndWithExt(path, EditorConst.UnityPackageExts);
        }

        /// <summary>
        /// 是否是unity场景
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsScene(string path)
        {
            return PathEndWithExt(path, EditorConst.SceneExts);
        }

        /// <summary>
        /// 是否包含动画
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAnimation(string path)
        {
            if (PathEndWithExt(path, EditorConst.ModelExts))
            {
                string assetPath = FormatAssetPath(path);
                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (modelImporter != null && modelImporter.importAnimation)
                {
                    return true;
                }
                return false;
            }
            return PathEndWithExt(path, EditorConst.AnimationExts);
        }

        /// <summary>
        /// 浏览文件夹下所有文件
        /// </summary>
        /// <param name="root">根目录</param>
        /// <param name="deep">是否含子目录</param>
        /// <param name="list">文件列表</param>
        public static void ScanDirectoryFile(string root, bool deep, List<string> list)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                Debug.LogWarning("scan directory file failed! " + root);
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(root);
            FileInfo[] files = dirInfo.GetFiles("*.*");
            for (int i = 0; i < files.Length; ++i)
            {
                list.Add(files[i].FullName);
            }

            if (deep)
            {
                DirectoryInfo[] dirs = dirInfo.GetDirectories("*.*");
                for (int i = 0; i < dirs.Length; ++i)
                {
                    ScanDirectoryFile(dirs[i].FullName, deep, list);
                }
            }
        }

        /// <summary>
        /// 浏览文件夹下所有文件
        /// </summary>
        /// <param name="root">根目录</param>
        /// <param name="deep">是否含子目录</param>
        /// <param name="list">文件列表</param>
        public static void ScanDirectory(string root, bool deep, List<string> list)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                Debug.LogWarning("scan directory file failed! " + root);
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(root);
            DirectoryInfo[] directories = dirInfo.GetDirectories();
            for(int i=0;i<directories.Length;i++)
            {
                list.Add(directories[i].FullName);
                if (deep)
                {
                    ScanDirectory(directories[i].FullName, deep, list);
                }
            }

        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static List<string> GetAssetPathList(string rootPath)
        {
            List<string> ret = new List<string>();
            ScanDirectoryFile(rootPath, true, ret);

            for (int i = 0; i < ret.Count; ++i)
            {
                ret[i] = FormatAssetPath(ret[i]);
            }

            return ret;
        }

        /// <summary>
        /// 规范化路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePathSplash(string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 判断是否带后缀
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="ext">后缀数组</param>
        /// <returns></returns>
        public static bool PathEndWithExt(string path, params string[] ext)
        {
            if (ext == null) return false;
            for (int i = 0; i < ext.Length; ++i)
            {
                if (path.EndsWith(ext[i], System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
