using UnityEngine;
using XCSJ.CommonUtils.EditorCharacters;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginsCameras.Tools.Controllers;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Characters
{
    /// <summary>
    /// 编辑器角色组手
    /// </summary>
    public static class EditorCharacterHelper
    {
        /// <summary>
        /// 角色的预制件目录
        /// </summary>
        public const string CharacterDir = "Assets/XDreamer-Assets/基础/Prefabs/人物/";

        /// <summary>
        /// 加载角色预制件:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <returns></returns>
        public static GameObject LoadCharacterPrefab(string nameWithoutExt)
        {
            var go = UICommonFun.LoadAndInstantiateFromAssets<GameObject>(CharacterDir + nameWithoutExt + ".prefab");
            go.XRename(nameWithoutExt);
            return go;
        }

        /// <summary>
        /// 加载Ethan:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <returns></returns>
        public static GameObject LoadEthan() => LoadCharacterPrefab("Ethan");

        /// <summary>
        /// 加载假人：胶囊小黄人:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <returns></returns>
        public static GameObject LoadDummy() => LoadCharacterPrefab("胶囊小黄人");
    }
}
