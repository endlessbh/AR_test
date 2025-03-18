using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    [Serializable]
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [Tip("场景(资源)打包工具(编译管线)", "Scene (resource) packaging tool (compilation pipeline)")]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class BuildAssetBundleWindow : XEditorWindowWithScrollView<BuildAssetBundleWindow>
    {
        public const string Title = "场景(资源)打包工具";

        //[MenuItem(XDreamerMenu.Name + "/" + Title, false, 345)]
        [MenuItem(XDreamerMenu.EditorWindow + Title)]
        public static void Init() => OpenAndFocus();

        [Name("编译类型")]
        [Tip("待编译的资源类型;", "Resource type to be compiled;")]
        [EnumPopup]
        public EBuildType buildType = EBuildType.Scene;

        [Name("编译后目标类型")]
        [Tip("打包资源后适用的平台目标类型;具体的详见Unity BuildTarget 定义；", "Applicable platform target type after resource packaging; See the definition of Unity BuildTarget for details;")]
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;

        [Name("编译选项")]
        [Tip("编译选项;具体的详见Unity BuildOptions 定义；", "Compile options; See the definition of Unity BuildOptions for details;")]
        public BuildOptions buildOptions = BuildOptions.BuildAdditionalStreamedScenes;

        [Name("编译资源包选项")]
        [Tip("编译资源包选项;具体的详见Unity BuildAssetBundleOptions 定义；", "Compile resource package options; See the definition of Unity BuildAssetBundleOptions for details;")]
        public BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

        /// <summary>
        /// 待打包的场景文件全路径信息；
        /// </summary>
        [Name("待编译场景文件(.unity)")]
        [Tip("待编译的场景文件全路径,即.unity后缀文件", "The full path of the scene file to be compiled, i.e Unity suffix file")]
        public string sceneFileBeforeBuild = "";

        /// <summary>
        /// 场景文件打包后的输出文件全路径信息；
        /// </summary>
        [Name("编译后场景文件(.unity3d)")]
        [Tip("编译场景完成后输出文件的全路径，即打包输出.unity3d后缀文件", "After compiling the scene, the full path of the output file, that is, packaged output Unity3d suffix file")]
        public string sceneFileAfterBuild = "";

        [Name("待编译场景文件夹")]
        [Tip("会递归遍历出所有.unity后缀的文件，作为待编译文件", "Will recursively iterate through all The file with the suffix unity is used as the file to be compiled")]
        public string sceneFolderBeforeBuild = "";

        [Name("编译后场景文件夹")]
        [Tip("编译场景完成后输出文件夹的全路径，即打包输出.unity3d后缀文件", "After compiling the scene, output the full path of the folder, that is, package output Unity3d suffix file")]
        public string sceneFolderAfterBuild = "";

        /// <summary>
        /// 资源包打包后的输出文件夹目录路径信息；
        /// </summary>
        [Name("编译后资源包文件夹")]
        [Tip("编译资源包完成后输出文件的文件夹目录信息", "After compiling the resource package, the folder and directory information of the output file")]
        public string assentBundleFolderAfterBuild = "";

        [Name("批量打包")]
        public bool useBatch = false;

        public bool selectAll = true;

        public bool selectNone = false;

        public class SceneFileInfo
        {
            public string path = "";

            public bool needBuild = true;
        }

        public List<SceneFileInfo> sceneFiles = new List<SceneFileInfo>();

        public bool expandBuildSettings = true;

        public BuildAssetBundleWindow()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetInfoWithCurrentScene();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (needBuild) RunBuild();
        }

        [Name("打开")]
        [Tip("打开", "Open")]
        [XCSJ.Attributes.Icon(EIcon.Open)]
        public bool open;

        [Name("保存")]
        [Tip("保存", "Save")]
        [XCSJ.Attributes.Icon(EIcon.Save)]
        public bool save;

        [Name("刷新")]
        [Tip("刷新信息", "Refresh information")]
        [XCSJ.Attributes.Icon(EIcon.Reset)]
        public bool refresh;

        public override void OnGUIWithScrollView()
        {
            #region 编译设置

            expandBuildSettings = UICommonFun.Foldout(expandBuildSettings, new GUIContent("编译设置"), () =>
            {
                if (GUILayout.Button(TrLabel(nameof(refresh)), GUILayout.Width(60), UICommonOption.Height16))
                {
                    SetInfoWithCurrentScene();
                }
            });

            if (expandBuildSettings)
            {
                CommonFun.BeginLayout();

                #region 资源编译类型

                var buildTypeTmp = (EBuildType)UICommonFun.EnumPopup(TrLabel(nameof(buildType)), buildType);
                if (buildTypeTmp != buildType)
                {
                    buildType = buildTypeTmp;
                    SetInfoWithCurrentScene();
                }

                #endregion

                EditorGUILayout.Separator();

                #region 编译后目标平台类型

                var buildTargetTmp = (BuildTarget)UICommonFun.EnumPopup(TrLabel(nameof(buildTarget)), buildTarget);
                if (buildTarget != buildTargetTmp)
                {
                    if (sceneFileAfterBuild.Contains(GetBuildTargetInfo(buildTarget)))
                    {
                        sceneFileAfterBuild = sceneFileAfterBuild.Replace(GetBuildTargetInfo(buildTarget), GetBuildTargetInfo(buildTargetTmp));
                    }
                    else
                    {
                        sceneFileAfterBuild = sceneFileAfterBuild.Insert(sceneFileAfterBuild.LastIndexOf("."), GetBuildTargetInfo(buildTargetTmp));
                    }
                    buildTarget = buildTargetTmp;
                }

                #endregion

                EditorGUILayout.Separator();

                switch (buildType)
                {
                    case EBuildType.Scene:
                        {
                            //编译选项
                            buildOptions = (BuildOptions)UICommonFun.EnumPopup(TrLabel(nameof(buildOptions)), buildOptions);
                            //是否批量打包
                            var useBatchTmp = EditorGUILayout.Toggle(TrLabel(nameof(useBatch)), useBatch);
                            if (useBatchTmp != useBatch)
                            {
                                useBatch = useBatchTmp;
                                sceneFiles.Clear();
                            }

                            if (useBatch)
                            {
                                #region 打开

                                EditorGUILayout.BeginHorizontal();

                                var folderTmp1 = EditorGUILayout.TextField(TrLabel(nameof(sceneFolderBeforeBuild)), GetValidDirectory(sceneFolderBeforeBuild));
                                if (sceneFolderBeforeBuild != folderTmp1)
                                {
                                    sceneFolderBeforeBuild = folderTmp1;
                                    sceneFiles.Clear();
                                }
                                if (GUILayout.Button(TrLabel(nameof(open)), UICommonOption.Width60, UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    var folderTmp2 = EditorUtility.OpenFolderPanel("打开", sceneFolderBeforeBuild, "");
                                    if (sceneFolderBeforeBuild != folderTmp2)
                                    {
                                        sceneFolderBeforeBuild = folderTmp2;
                                        sceneFiles.Clear();
                                    }
                                }

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region 全选/全不选/反选

                                if (sceneFiles.Count == 0 && !string.IsNullOrEmpty(sceneFolderBeforeBuild))
                                {
                                    foreach (var f in Directory.GetFiles(sceneFolderBeforeBuild, "*.unity", SearchOption.AllDirectories))
                                    {
                                        sceneFiles.Add(new SceneFileInfo()
                                        {
                                            path = Path.GetFullPath(f)
                                        });
                                    }
                                }

                                CommonFun.BeginLayout(true, false);

                                //全选/取消全选/反选
                                EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));

                                GUILayout.Label("操作:", GUILayout.Width(50));

                                if (GUILayout.Toggle(selectAll, "全选", GUILayout.Width(60)))
                                {
                                    selectNone = false;
                                    foreach (var fi in sceneFiles) fi.needBuild = true;
                                }

                                if (GUILayout.Toggle(selectNone, "全不选", GUILayout.Width(60)))
                                {
                                    selectAll = false;
                                    foreach (var fi in sceneFiles) fi.needBuild = false;
                                }

                                if (GUILayout.Button("反选", GUILayout.Width(60)))
                                {
                                    foreach (var fi in sceneFiles) fi.needBuild = !fi.needBuild;
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginVertical("box");
                                var selectAllTmp = true;
                                var selectNoneTmp = true;
                                foreach (var fi in sceneFiles)
                                {
                                    fi.needBuild = EditorGUILayout.ToggleLeft(fi.path, fi.needBuild);
                                    if (fi.needBuild) selectNoneTmp = false;
                                    else selectAllTmp = false;
                                }
                                selectAll = selectAllTmp;
                                selectNone = selectNoneTmp;
                                EditorGUILayout.EndVertical();

                                CommonFun.EndLayout();

                                #endregion

                                #region 保存

                                EditorGUILayout.BeginHorizontal();

                                sceneFolderAfterBuild = EditorGUILayout.TextField(TrLabel(nameof(sceneFolderAfterBuild)), GetValidDirectory(sceneFolderAfterBuild));

                                if (GUILayout.Button(TrLabel(nameof(save)), UICommonOption.Width60, UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    string directoryPath = "";
                                    //string defaultName = "";

                                    if (string.IsNullOrEmpty(sceneFolderAfterBuild))
                                    {
                                        var scene = SceneManager.GetActiveScene();
                                        if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                                        {
                                            directoryPath = Path.GetDirectoryName(scene.path);
                                            //defaultName = Path.GetFileNameWithoutExtension(scene.name);
                                        }
                                    }
                                    else
                                    {
                                        directoryPath = Path.GetDirectoryName(sceneFolderAfterBuild);
                                        //defaultName = Path.GetFileNameWithoutExtension(sceneFolderAfterBuild);
                                    }

                                    if (string.IsNullOrEmpty(directoryPath)) directoryPath = Application.dataPath;

                                    //弹出文件保存对话框
                                    sceneFolderAfterBuild = EditorUtility.SaveFolderPanel("保存", directoryPath, "");
                                }
                                EditorGUILayout.EndHorizontal();


                                #endregion
                            }
                            else
                            {
                                #region 打开

                                EditorGUILayout.BeginHorizontal();

                                sceneFileBeforeBuild = EditorGUILayout.TextField(TrLabel(nameof(sceneFileBeforeBuild)), sceneFileBeforeBuild);
                                if (GUILayout.Button(TrLabel(nameof(open)), UICommonOption.Width60, UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    sceneFileBeforeBuild = EditorUtility.OpenFilePanel("打开", GetValidDirectory(sceneFileBeforeBuild), "unity");
                                }

                                EditorGUILayout.EndHorizontal();

                                #endregion

                                #region 保存

                                EditorGUILayout.BeginHorizontal();

                                sceneFileAfterBuild = EditorGUILayout.TextField(TrLabel(nameof(sceneFileAfterBuild)), sceneFileAfterBuild);

                                if (GUILayout.Button(TrLabel(nameof(save)), UICommonOption.Width60, UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    string directoryPath = "";
                                    string defaultName = "";

                                    if (string.IsNullOrEmpty(sceneFileAfterBuild))
                                    {
                                        var scene = SceneManager.GetActiveScene();
                                        if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                                        {
                                            directoryPath = Path.GetDirectoryName(scene.path);
                                            defaultName = Path.GetFileNameWithoutExtension(scene.name);
                                        }
                                    }
                                    else
                                    {
                                        directoryPath = Path.GetDirectoryName(sceneFileAfterBuild);
                                        defaultName = Path.GetFileNameWithoutExtension(sceneFileAfterBuild);
                                    }

                                    if (string.IsNullOrEmpty(directoryPath)) directoryPath = Application.dataPath;

                                    //弹出文件保存对话框
                                    sceneFileAfterBuild = EditorUtility.SaveFilePanel("保存", directoryPath, defaultName, "unity3d");
                                }
                                EditorGUILayout.EndHorizontal();

                                #endregion
                            }
                            break;
                        }
                    case EBuildType.AssetBundleAll:
                        {
                            BuildAssetBundleGUI();
                            break;
                        }
                }

                CommonFun.EndLayout();
            }
            #endregion               

            #region 执行打包按钮

            EditorGUILayout.Separator();
            if (GUILayout.Button(string.Format("执行打包 - {0}", CommonFun.Name(typeof(EBuildType), this.buildType.ToString()))))
            {
                GUI.FocusControl("");
                needBuild = true;
            }//end if button

            #endregion

            EditorGUILayout.Separator();
        }

        private string GetValidDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var scene = SceneManager.GetActiveScene();
                if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                {
                    path = Path.GetDirectoryName(scene.path);
                }
            }
            else
            {
                if (path.EndsWith(".unity") || path.EndsWith(".unity3d")) path = Path.GetDirectoryName(path);
            }

            return string.IsNullOrEmpty(path) ? Application.dataPath : path;
        }

        private void BuildAssetBundleGUI()
        {
            //编译资源包选项
            buildAssetBundleOptions = (BuildAssetBundleOptions)UICommonFun.EnumPopup(TrLabel(nameof(buildAssetBundleOptions)), buildAssetBundleOptions);

            #region 编译后输出路径

            EditorGUILayout.BeginHorizontal();
            assentBundleFolderAfterBuild = EditorGUILayout.TextField(TrLabel(nameof(assentBundleFolderAfterBuild)), assentBundleFolderAfterBuild);
            if (GUILayout.Button("保存", GUILayout.Width(80)))
            {
                string directoryPath = "";
                string defaultName = "";
                if (string.IsNullOrEmpty(assentBundleFolderAfterBuild))
                {
                    Scene scene = SceneManager.GetActiveScene();
                    if (scene.IsValid())
                    {
                        directoryPath = Path.GetDirectoryName(scene.path);
                        defaultName = Path.GetFileNameWithoutExtension(scene.name);
                    }
                }
                else
                {
                    directoryPath = Path.GetDirectoryName(assentBundleFolderAfterBuild);
                    defaultName = Path.GetFileNameWithoutExtension(assentBundleFolderAfterBuild);
                }
                assentBundleFolderAfterBuild = EditorUtility.SaveFolderPanel("保存资产包", directoryPath, defaultName);
                if (!string.IsNullOrEmpty(assentBundleFolderAfterBuild)) assentBundleFolderAfterBuild = Path.GetFullPath(assentBundleFolderAfterBuild);
            }
            EditorGUILayout.EndHorizontal();

            #endregion
        }

        private void SetInfoWithCurrentScene()
        {
            switch (buildType)
            {
                case EBuildType.Scene:
                    {
                        Scene scene = SceneManager.GetActiveScene();
                        if (scene.IsValid())
                        {
                            if (string.IsNullOrEmpty(scene.path) || string.IsNullOrEmpty(scene.name))
                            {
                                //发送本地组件版本，unity版本
                                if (EditorUtility.DisplayDialog("注意！",
                                    "当前场景未保存！请先保存场景，再执行打包！", "保存场景",
                                    "关闭"))
                                {
                                    if (!EditorSceneManager.SaveScene(scene))
                                    {
                                        Debug.LogError("当前场景保存失败!");
                                    }
                                }
                                break;
                            }
                            buildTarget = EditorUserBuildSettings.activeBuildTarget;
                            buildOptions = BuildOptions.BuildAdditionalStreamedScenes;
                            sceneFileAfterBuild = Path.GetDirectoryName(scene.path) + "/" + scene.name + GetBuildTargetInfo(buildTarget) + ".unity3d";
                            sceneFileAfterBuild = Path.GetFullPath(sceneFileAfterBuild);
                            sceneFolderAfterBuild = Path.GetDirectoryName(sceneFileAfterBuild);

                            sceneFileBeforeBuild = Path.GetDirectoryName(scene.path) + "/" + scene.name + ".unity";
                            sceneFileBeforeBuild = Path.GetFullPath(sceneFileBeforeBuild);
                            sceneFolderBeforeBuild = Path.GetDirectoryName(sceneFileBeforeBuild);

                        }
                        break;
                    }
                case EBuildType.AssetBundleAll:
                    {
                        buildTarget = EditorUserBuildSettings.activeBuildTarget;
                        buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
                        if (string.IsNullOrEmpty(assentBundleFolderAfterBuild) && !string.IsNullOrEmpty(sceneFileAfterBuild))
                        {
                            assentBundleFolderAfterBuild = Path.GetDirectoryName(sceneFileAfterBuild);
                            assentBundleFolderAfterBuild = Path.GetFullPath(assentBundleFolderAfterBuild);
                        }
                        break;
                    }
            }
        }

        private string GetBuildTargetInfo(BuildTarget bt)
        {
            return "(" + bt.ToString() + ")";
        }

        public static void BuildScene(string unityFile, string unity3dFile, BuildTarget buildTarget, BuildOptions buildOptions)
        {
            if (!unityFile.EndsWith(".unity") || !File.Exists(unityFile))
            {
                Debug.LogWarning("待编译的场景文件: " + unityFile + " 无效！");
                return;
            }
            if (!unity3dFile.EndsWith(".unity3d") || !Directory.Exists(Path.GetDirectoryName(unity3dFile)))
            {
                Debug.LogWarning("场景: " + unityFile + " 的编译(打包)后输出路径: " + unity3dFile + " 无效！");
                return;
            }
            //清空一下缓存
            Caching.ClearCache();
            //打包场景
            var buildReport = BuildPipeline.BuildPlayer(new string[] { unityFile }, unity3dFile, buildTarget, buildOptions);
            if (buildReport.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("场景:" + unityFile + " 编译(打包)完成!");
            }
            else
            {
                Debug.LogError("执行场景:" + unityFile + " 编译(打包)发生错误!");
            }
        }

        private bool needBuild = false;

        private void RunBuild()
        {
            needBuild = false;
            string buildTypeName = CommonFun.Name(this.GetType(), nameof(buildType));
            switch (buildType)
            {
                case EBuildType.Scene:
                    {
                        if (useBatch)
                        {
                            if (!Directory.Exists(sceneFolderAfterBuild))
                            {
                                Debug.LogWarning(buildTypeName + "编译(打包)后输出文件夹(目录): " + sceneFolderAfterBuild + " 无效！");
                                break;
                            }
                            foreach (var fi in sceneFiles)
                            {
                                if (fi.needBuild) BuildScene(fi.path, string.Format("{0}/{1}{2}.unity3d", sceneFolderAfterBuild, Path.GetFileNameWithoutExtension(fi.path), GetBuildTargetInfo(buildTarget)), buildTarget, buildOptions);
                            }
                        }
                        else
                        {
                            BuildScene(sceneFileBeforeBuild, sceneFileAfterBuild, buildTarget, buildOptions);
                        }
                        break;
                    }
                case EBuildType.AssetBundleAll:
                    {
                        if (!Directory.Exists(sceneFolderAfterBuild))
                        {
                            Debug.LogWarning(buildTypeName + "编译(打包)后输出文件夹(目录): " + sceneFolderAfterBuild + " 无效！");
                            break;
                        }
                        //清空一下缓存
                        Caching.ClearCache();
                        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(assentBundleFolderAfterBuild, buildAssetBundleOptions, buildTarget);
                        if (abm) Debug.Log(buildTypeName + "编译完成!");
                        else Debug.LogError("执行" + buildTypeName + "编译发生错误!");
                        break;
                    }
            }
            //资源数据库刷新
            AssetDatabase.Refresh();
        }
    }

    public enum EBuildType
    {
        [Name("场景")]
        [Tip("编译类型：场景；将场景文件（.unity 后缀名文件）编译生成 .unity3d 后缀的执行文件；", "Compilation type: scene; Compile the scene file (. Unit suffix file) to generate Execution file of unity3d suffix;")]
        Scene = 0,

        [Name("资源包-全部")]
        [Tip("编译类型：资源包；将当前工程中所有已编辑资源包名称的文件，编译生成对应的资源包压缩文件以及 .manifest 后缀的清单文件；", "Compilation type: resource package; Compile all the files with edited resource package names in the current project to generate the corresponding resource package compressed files and Manifest file of manifest suffix;")]
        AssetBundleAll,
    }
}