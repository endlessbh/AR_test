using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XCSJ.EditorAssetsExporter.FBXExporter5_0
{
    public class FBXExporter
    {
        private static void BuildAndSaveFBX(string folderPathForFBX, string fbxName, GameObject topParent, Material[] materials)
        {
            string text = Application.dataPath + folderPathForFBX + "/" + fbxName + ".fbx";
            string text2 = FBXWriter.MeshToString(topParent, text, materials, true, true);
            Debug.Log(text2);
            File.WriteAllText(text, text2);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static GameObject DuplicateObject(GameObject objectToDuplicate)
        {
            GameObject obj = Object.Instantiate((Object)objectToDuplicate) as GameObject;
            obj.transform.SetParent(objectToDuplicate.transform.parent);
            obj.transform.localPosition = objectToDuplicate.transform.localPosition;
            obj.transform.localEulerAngles = objectToDuplicate.transform.localEulerAngles;
            obj.transform.localScale = objectToDuplicate.transform.localScale;
            obj.transform.SetParent(null);
            obj.name = objectToDuplicate.name;
            return obj;
        }

        private static int ErrorCheckings(GameObject[] selectedObjects, bool addObjectInHierarchy)
        {
            int result = 0;
            if (selectedObjects == null)
            {
                Debug.LogError("Nothing Selected!!!");
                return -1;
            }
            if (selectedObjects.Length == 0)
            {
                Debug.LogError("Nothing Selected!!!");
                return -1;
            }
            if (addObjectInHierarchy && selectedObjects.Length > 1)
            {
                Debug.LogError("Select only one Gameobject if using heirarchy");
                result = -1;
            }
            return result;
        }

        public static void ExportFBX(string folderPathForFBX, string fbxName, GameObject[] selectedObjects, bool addObjectInHierarchy)
        {
            fbxName = FBXExporter.GetUniqueName(fbxName, folderPathForFBX, ".fbx");
            Debug.Log(fbxName);
            if (FBXExporter.ErrorCheckings(selectedObjects, addObjectInHierarchy) <= 0)
            {
                GameObject[] objectsToCombine = FBXExporter.GetObjectsToCombine(selectedObjects, addObjectInHierarchy);
                FBXExporter.SetDirectories(folderPathForFBX);
                FBXExporter.ExportFBXForTheseObjects(objectsToCombine, folderPathForFBX, fbxName, addObjectInHierarchy, selectedObjects[0]);
            }
            else
            {
                Debug.Log("no");
            }
        }

        private static void ExportFBXForTheseObjects(GameObject[] objectsToCombine, string folderPathForFBX, string fbxName, bool addObjectInHierarchy, GameObject parentGameobject)
        {
            int num = 0;
            List<Material> list = new List<Material>();
            list = FBXExporter.GetUniqueMaterials(objectsToCombine);
            GameObject gameObject = null;
            if (addObjectInHierarchy)
            {
                gameObject = FBXExporter.DuplicateObject(parentGameobject);
            }
            else if (objectsToCombine.Length == 1)
            {
                gameObject = FBXExporter.DuplicateObject(objectsToCombine[0]);
            }
            else
            {
                gameObject = new GameObject(fbxName);
                for (num = 0; num < objectsToCombine.Length; num++)
                {
                    FBXExporter.DuplicateObject(objectsToCombine[num]).transform.SetParent(gameObject.transform);
                }
            }
            gameObject.name = fbxName;
            FBXExporter.BuildAndSaveFBX(folderPathForFBX, fbxName, gameObject, list.ToArray());
            Object.DestroyImmediate(gameObject);
        }

        public static GameObject[] GetObjectsToCombine(GameObject[] selectedObjects, bool addObjectInHierarchy)
        {
            int num = 0;
            int num2 = 0;
            List<MeshFilter> list = new List<MeshFilter>();
            MeshFilter[] array = null;
            for (num = 0; num < selectedObjects.Length; num++)
            {
                if (addObjectInHierarchy)
                {
                    array = selectedObjects[num].GetComponentsInChildren<MeshFilter>();
                    for (num2 = 0; num2 < array.Length; num2++)
                    {
                        if ((Object)array[num2].sharedMesh != (Object)null && (Object)((Component)array[num2]).GetComponent<Renderer>() != (Object)null && (Object)((Component)array[num2]).GetComponent<Renderer>().sharedMaterial.mainTexture != (Object)null)
                        {
                            list.Add(array[num2]);
                        }
                    }
                }
                else if ((Object)selectedObjects[num].GetComponent<MeshFilter>() != (Object)null && (Object)selectedObjects[num].GetComponent<MeshFilter>().sharedMesh != (Object)null && (Object)selectedObjects[num].GetComponent<Renderer>() != (Object)null && (Object)selectedObjects[num].GetComponent<Renderer>().sharedMaterial.mainTexture != (Object)null)
                {
                    list.Add(selectedObjects[num].GetComponent<MeshFilter>());
                }
            }
            List<MeshFilter> list2 = new List<MeshFilter>();
            foreach (MeshFilter item in list)
            {
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                }
            }
            GameObject[] array2 = new GameObject[list2.Count];
            for (num = 0; num < list2.Count; num++)
            {
                array2[num] = list2[num].gameObject;
            }
            return array2;
        }

        private static List<Material> GetUniqueMaterials(GameObject[] objectsToCombine)
        {
            List<Material> list = new List<Material>();
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                Material[] sharedMaterials = objectsToCombine[i].GetComponent<Renderer>().sharedMaterials;
                foreach (Material item in sharedMaterials)
                {
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private static string GetUniqueName(string name, string path, string extension)
        {
            string path2 = Application.dataPath + path + "/" + name + extension;
            int num = 1;
            if (!File.Exists(path2))
            {
                return name;
            }
            while (File.Exists(Application.dataPath + path + "/" + name + " " + num + extension))
            {
                num++;
            }
            name = name + " " + num;
            return name;
        }

        private static void SetDirectories(string folderPathForFBX)
        {
            if (!Directory.Exists(Application.dataPath + folderPathForFBX))
            {
                Directory.CreateDirectory(Application.dataPath + folderPathForFBX);
            }
        }
    }
}
