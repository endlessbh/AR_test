using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Tools;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;

namespace XCSJ.EditorAssetsExporter
{
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class OBJExporterWindow : XEditorWindowWithScrollView<OBJExporterWindow>
    {
        public const string Title = "OBJ导出工具";

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        [Name("使用选择集")]
        public bool useSelection = true;

        [Name("文件路径")]
        public string folderPathForFBX = "";

        [Name("对象列表")]
        public bool displayObjs = true;

        public List<Transform> transforms = new List<Transform>();

        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.Separator();
            useSelection = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(useSelection)), useSelection);    
            folderPathForFBX = UICommonFun.DrawSaveFolder(CommonFun.NameTooltip(this, nameof(folderPathForFBX)), CommonFun.TempContent("选择"), folderPathForFBX, GUILayout.Width(80));
            if(string.IsNullOrEmpty(folderPathForFBX))
            {
                folderPathForFBX = Application.dataPath;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("导出操作");
            EditorGUI.BeginDisabledGroup(transforms.Count == 0);
            if (GUILayout.Button(CommonFun.TempContent("独立导出", "将对象列表中每个对象的Mesh信息导出,导出" + transforms.Count + "个OBJ文件"), EditorStyles.miniButtonLeft))
            {
                ExportToSeparate(folderPathForFBX, transforms.ToArray());
            }
            if (GUILayout.Button(CommonFun.TempContent("合并导出", "将对象列表中所有对象的Mesh信息合并导出为1个OBJ文件"), EditorStyles.miniButtonMid))
            {
                ExportWholeToSingle(folderPathForFBX, transforms.ToArray());
            }
            if (GUILayout.Button(CommonFun.TempContent("分别导出", "将对象列表中每个对象以及子级对象的Mesh信息分别导出,至少导出" + transforms.Count + "个OBJ文件"), EditorStyles.miniButtonRight))
            {
                ExportEachToSingle(folderPathForFBX, transforms.ToArray());
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("对象列表操作");
            EditorGUI.BeginDisabledGroup(useSelection);
            if (GUILayout.Button("添加对象", EditorStyles.miniButtonLeft))
            {
                transforms.Add(null);
            }
            if (GUILayout.Button("移除空对象", EditorStyles.miniButtonRight))
            {
                Debug.Log(Application.dataPath);
                transforms = transforms.Where(o => o).ToList();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (displayObjs = UICommonFun.Foldout(displayObjs, CommonFun.NameTooltip(this, nameof(displayObjs)), true))
            {
                CommonFun.BeginLayout();
                for (int i = 0; i < transforms.Count; ++i)
                {
                    transforms[i] = (Transform)EditorGUILayout.ObjectField((i + 1).ToString(), transforms[i], typeof(Transform), true);
                }
                CommonFun.EndLayout();
            }                   
        }

        private void OnSelectionChange()
        {
            if (useSelection)
            {
                transforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab).ToList();
            }
            Repaint();
        }

        #region 导出OBJ核心实现

        private static int vertexOffset = 0;
        private static int normalOffset = 0;
        private static int uvOffset = 0;

        struct ObjMaterial
        {
            public string name;
            public string textureName;
        }

        private static string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList)
        {
            Mesh m = mf.sharedMesh;
            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n");
            foreach (Vector3 lv in m.vertices)
            {
                Vector3 wv = mf.transform.TransformPoint(lv);

                //This is sort of ugly - inverting x-component since we're in
                //a different coordinate system than "everyone" is "used to".
                sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            sb.Append("\n");

            foreach (Vector3 lv in m.normals)
            {
                Vector3 wv = mf.transform.TransformDirection(lv);

                sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            sb.Append("\n");

            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                //See if this material is already in the materiallist.
                try
                {
                    ObjMaterial objMaterial = new ObjMaterial();

                    objMaterial.name = mats[material].name;

                    if (mats[material].mainTexture)
                    {
                        //objMaterial.textureName = EditorUtility.GetAssetPath(mats[material].mainTexture);
                        objMaterial.textureName = AssetDatabase.GetAssetPath(mats[material].mainTexture);
                    }
                    else
                        objMaterial.textureName = null;

                    materialList.Add(objMaterial.name, objMaterial);
                }
                catch (ArgumentException)
                {
                    //Already in the dictionary
                }


                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    //Because we inverted the x-component, we also needed to alter the triangle winding.
                    sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
                }
            }

            vertexOffset += m.vertices.Length;
            normalOffset += m.normals.Length;
            uvOffset += m.uv.Length;

            return sb.ToString();
        }

        private static void Clear()
        {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }

        private static Dictionary<string, ObjMaterial> PrepareFileWrite()
        {
            Clear();
            return new Dictionary<string, ObjMaterial>();
        }

        private static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
        {
            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl"))
            {
                foreach (KeyValuePair<string, ObjMaterial> kvp in materialList)
                {
                    sw.Write("\n");
                    sw.Write("newmtl {0}\n", kvp.Key);
                    sw.Write("Ka  0.6 0.6 0.6\n");
                    sw.Write("Kd  0.6 0.6 0.6\n");
                    sw.Write("Ks  0.9 0.9 0.9\n");
                    sw.Write("d  1.0\n");
                    sw.Write("Ns  0.0\n");
                    sw.Write("illum 2\n");

                    if (kvp.Value.textureName != null)
                    {
                        string destinationFile = kvp.Value.textureName;


                        int stripIndex = destinationFile.LastIndexOf('/');//FIXME: Should be Path.PathSeparator;

                        if (stripIndex >= 0)
                            destinationFile = destinationFile.Substring(stripIndex + 1).Trim();


                        string relativeFile = destinationFile;

                        destinationFile = folder + "/" + destinationFile;

                        Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                        try
                        {
                            //Copy the source file
                            File.Copy(kvp.Value.textureName, destinationFile);
                        }
                        catch
                        {

                        }


                        sw.Write("map_Kd {0}", relativeFile);
                    }

                    sw.Write("\n\n\n");
                }
            }
        }

        private static void MeshToFile(MeshFilter mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                sw.Write(MeshToString(mf, materialList));
            }

            MaterialsToFile(materialList, folder, filename);
        }

        private static void MeshesToFile(MeshFilter[] mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                for (int i = 0; i < mf.Length; i++)
                {
                    sw.Write(MeshToString(mf[i], materialList));
                }
            }

            MaterialsToFile(materialList, folder, filename);
        }

        public static void ExportToSeparate(string folder, Transform[] selection)
        {
            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("OBJ导出失败!", "请选择一个或多个目标对象!", "");
                return;
            }

            int exportedObjects = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    MeshToFile((MeshFilter)meshfilter[m], folder, selection[i].name + "_" + i + "_" + m);
                }
            }
            if (exportedObjects > 0)
            {
                EditorUtility.DisplayDialog("OBJ导出成功", "导出 " + exportedObjects + " 对象!", "");
            }
            else
            {
                EditorUtility.DisplayDialog("OBJ导出失败", "确保至少一个对象具有网格过滤器(MeshFilter)!", "");
            }
        }

        public static void ExportWholeToSingle(string folder, Transform[] selection)
        {
            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("OBJ导出失败!", "请选择一个或多个目标对象!", "");
                return;
            }

            int exportedObjects = 0;

            ArrayList mfList = new ArrayList();

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects > 0)
            {
                MeshFilter[] mf = new MeshFilter[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (MeshFilter)mfList[i];
                }

                //string filename = EditorApplication.currentScene + "_" + exportedObjects;
                string filename = SceneManager.GetActiveScene().path + "_" + exportedObjects;

                int stripIndex = filename.LastIndexOf('/');//FIXME: Should be Path.PathSeparator

                if (stripIndex >= 0)
                    filename = filename.Substring(stripIndex + 1).Trim();

                MeshesToFile(mf, folder, filename);


                EditorUtility.DisplayDialog("OBJ导出成功", "导出 " + exportedObjects + " 对象到" + filename + "!", "");
            }
            else
            {
                EditorUtility.DisplayDialog("OBJ导出失败", "确保至少一个对象具有网格过滤器(MeshFilter)!", "");
            }                
        }

        public static void ExportEachToSingle(string folder, Transform[] selection)
        {
            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("OBJ导出失败", "请选择一个或多个目标对象!", "");
                return;
            }

            int exportedObjects = 0;
            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));

                MeshFilter[] mf = new MeshFilter[meshfilter.Length];

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mf[m] = (MeshFilter)meshfilter[m];
                }

                MeshesToFile(mf, folder, selection[i].name + "_" + i);
            }

            if (exportedObjects > 0)
            {
                EditorUtility.DisplayDialog("OBJ导出成功", "导出 " + exportedObjects + " 对象!", "");
            }
            else
            {
                EditorUtility.DisplayDialog("OBJ导出失败", "确保至少一个对象具有网格过滤器(MeshFilter)!", "");
            }
        }

        #endregion
    }
}