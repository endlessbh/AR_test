using UnityEngine;
using System.Collections;
using XCSJ.EditorCommonUtils;
using System.Collections.Generic;
using UnityEditor;
using XCSJ.EditorExtension;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.EditorAssetsExporter.FBXExporter5_0;
using XCSJ.Tools;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;

namespace XCSJ.EditorAssetsExporter
{
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class FBXExporterWindow : XEditorWindowWithScrollView<FBXExporterWindow>
    {
        public const string Title = "FBX导出工具";

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        [Name("文件路径")]
        public string folderPathForFBX = "";

        [Name("文件名")]
        public string fileName = "fbx";

        [Name("添加层级对象")]
        public bool addObjectInHierarchy = true;

        [Name("对象列表")]
        public bool displayObjs = true;

        public List<GameObject> meshObjs = new List<GameObject>();

        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.Separator();

            folderPathForFBX = UICommonFun.DrawSaveFolder(CommonFun.NameTooltip(this, nameof(folderPathForFBX)), CommonFun.TempContent("选择"), Application.dataPath + folderPathForFBX, GUILayout.Width(80));
            if (folderPathForFBX.StartsWith(Application.dataPath))
            {
                folderPathForFBX = folderPathForFBX.Replace(Application.dataPath, "");
            }
            else
            {
                folderPathForFBX = "";
            }

            fileName = EditorGUILayout.DelayedTextField(CommonFun.NameTooltip(this, nameof(fileName)), fileName);
            addObjectInHierarchy = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(addObjectInHierarchy)), addObjectInHierarchy);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("导出操作");
            if (GUILayout.Button("导出FBX"))
            {
                FBXExporter.ExportFBX(folderPathForFBX, fileName, meshObjs.Where(o => o).ToArray(), addObjectInHierarchy);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("对象列表操作");
            if (GUILayout.Button("添加对象", EditorStyles.miniButtonLeft))
            {
                meshObjs.Add(null);
            }
            if (GUILayout.Button("移除空对象", EditorStyles.miniButtonRight))
            {
                Debug.Log(Application.dataPath);
                meshObjs = meshObjs.Where(o => o).ToList();
            }
            EditorGUILayout.EndHorizontal();

            if (displayObjs = UICommonFun.Foldout(displayObjs, CommonFun.NameTooltip(this, nameof(displayObjs)), true))
            {
                CommonFun.BeginLayout();
                for (int i = 0; i < meshObjs.Count; ++i)
                {
                    meshObjs[i] = (GameObject)EditorGUILayout.ObjectField((i + 1).ToString(), meshObjs[i], typeof(GameObject), true);
                }
                CommonFun.EndLayout();
            }          
        }
    }
}

