using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class PreviewHelper
    {
        /// <summary>
        /// 获取预览图
        /// </summary>
        /// <param name="astPath">文件完整路径</param>
        /// <returns></returns>
        public static Texture2D GetPreviewTex(string astPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(astPath);
            var texPath = "Library/metadata/" + guid.Substring(0, 2) + "/" + guid + ".info";
            var texN = LoadTexture(texPath);
            return texN;
        }

        public static string GetPreviewTexPath(string astPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(astPath);
            if (string.IsNullOrEmpty(guid)) return null;
            var texPath = "/Library/metadata/" + guid.Substring(0, 2) + "/" + guid + ".info";
            return texPath;
        }

        /// <summary>
        /// 加载贴图
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static Texture2D LoadTexture(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.anisoLevel = 8;
                tex.LoadImage(fileData);
            }
            return tex;
        }
    }

    public class SelectionHelper
    {
        /// <summary>
        /// 在窗口中选择对象
        /// </summary>
        /// <param name="path">对象所在的路径</param>
        public static void OnAssetSelected(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj == null) return;
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        /// <summary>
        /// 导入unitypackage包
        /// </summary>
        /// <param name="path">unitypackage包路径</param>
        public static void OnPackageSelected(string path)
        {
            AssetDatabase.ImportPackage(path, true);
        }

        /// <summary>
        /// 打开场景
        /// </summary>
        /// <param name="path">场景路径</param>
        public static void OnSceneSelected(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }
}
