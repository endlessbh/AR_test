using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// XDreamer扩展配置类
    /// </summary>
    [XDreamerPreferences(index = IDRange.Begin + 1)]
    [Name(Product.Name + "-扩展")]
    [Import]
    public class XDreamerExtensionOption : XDreamerOption<XDreamerExtensionOption>
    {
        /// <summary>
        /// 版本，修改时会自动修改编译宏,所有代码重新编译;
        /// </summary>
        [Name("用户版本")]
        [Tip("针对不同层级XDreamer用户定制不同界面的版本;修改后,会自动修改编译宏,所有代码重新编译;", "Customized versions of different interfaces for different levels of xdreamer users; After modification, the compilation macro will be modified automatically and all codes will be recompiled;")]
        [EnumPopup]
        public EXDreamerEdition edition = EXDreamerEdition.Developer;

        [Name("在文件夹图标上绘制XDreamer")]
        [Tip("在文件夹图标上绘制XDreamer", "Draw XDreamer on the folder icon")]
        public bool drawXDreamerIconOnFoulder = true;

        protected override int newVersion => 2;

        protected override void OnVersionChanged(int lastVersion)
        {
            XDreamerEdition.UpdateEditionMacro();
        }


        /// <summary>
        /// 当配置修改时
        /// </summary>
        public override void OnModified()
        {
            base.OnModified();
            XDreamerEdition.UpdateEditionMacro();
        }

        #region 修改CS文件图标--待优化


        //[Name("修改CS文件图标-待实现")]
        //[Tip("勾选后，会将CS文件图标尝试替换为" + Product.Name + "专用图标;不勾选,将CS文件图标设置为原始图标;")]
        //public bool modifyCSFileIcon = false;

        //[Name("使用Icon特性-待实现")]
        //[Tip("勾选后，会根据CS文件中主体类的Icon特性将件图标尝试替换为特性指定的专用图标;不勾选,则使用" + Product.Name + "专用图标;")]
        //public bool useIconAttribute = true;

        private static void InitIcon()
        {
            foreach (var ms in MonoImporter.GetAllRuntimeMonoScripts())
            {
                SetIcon(ms);
            }
        }

        private static void SetIcon(MonoScript monoScript)
        {
            var type = monoScript.GetClass();
            if (type == null || !type.FullName.StartsWith("XCSJ.")) return;
            //Debug.Log(type);
            if (!typeof(MonoBehaviour).IsAssignableFrom(type) && !typeof(ScriptableObject).IsAssignableFrom(type)) return;
            Debug.Log(type + ": " + EditorIconHelper.GetIconInLib(type));
            //EditorHelper.SetIcon(monoScript, EditorIconHelper.GetIconInLib(type));
        }

        #endregion

        #region 文件夹图标

        [InitializeOnLoadMethod]
        private static void InitFolderIcon()
        {
            EditorApplication.projectWindowItemOnGUI += DrawXDreamerIcon;
        }        

        private static void DrawXDreamerIcon(string guid, Rect rect)
        {
            if (!weakInstance.drawXDreamerIconOnFoulder) return;
            var icon = UICommonFun.GizmosDefaultIcon32x32;
            if (!icon) return;

            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;

            var index = path.LastIndexOf('/');
            if (index<0) return;

            var lastStr = path.Substring(index+1);
            if (!lastStr.StartsWith(Product.Name)) return;

            DrawCustomIcon(guid, rect, icon);
        }

        private static void DrawCustomIcon(string guid, Rect rect, Texture texture)
        {
            if (texture)
            {
                if (IsTreeView(rect))
                {
                    rect = new Rect(rect.x + 3, rect.y + rect.height - treeViewIconSize, treeViewIconSize, treeViewIconSize);
                }
                else
                {
                    var size = new Vector2(texture.width, texture.height);
                    rect = new Rect(rect.center - size / 2, size);
                }
                GUI.DrawTexture(rect, texture);
            }
        }

        private static bool IsTreeView(Rect rect)
        {
            return rect.width > rect.height || rect.height == unityEditorUIDefaultHeight;
        }

        private const float treeViewIconSize = 12;
        private const float unityEditorUIDefaultHeight = 16;

        #endregion

        #region 层级变量编辑器

        [Name("层级变量编辑器显示模式")]
        [Tip("层级变量编辑器的变量信息的显示模式", "Display mode of variable information in hierarchical variable editing window")]
        public EDisplayMode hierarchyVarEditorWindowDisplayMode = EDisplayMode.List;

        #endregion

        #region 

        /// <summary>
        /// 当创建管理器时询问导入包
        /// </summary>
        [Name("当创建管理器时询问导入包")]
        public bool askImportPackageOnCreatedManager = true;

        /// <summary>
        /// 当场景打开时询问导入包
        /// </summary>
        [Name("当场景打开时询问导入包")]
        public bool askImportPackageOnSceneOpened = true;

        #endregion
    }

    [CommonEditor(typeof(XDreamerExtensionOption))]
    public class XDreamerExtensionOptionEditor : XDreamerOptionEditor<XDreamerExtensionOption>
    {
    }
}
