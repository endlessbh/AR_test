using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XCSJ.EditorAssetsExporter.FBXExporter5_0
{
    public class FBXMeshGetter
    {
        public static long GetMeshToString(GameObject gameObj, Material[] materials, ref StringBuilder objects, ref StringBuilder connections, GameObject parentObject = null, long parentModelId = 0L)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            long randomFBXId = FBXWriter.GetRandomFBXId();
            long randomFBXId2 = FBXWriter.GetRandomFBXId();
            MeshFilter component = gameObj.GetComponent<MeshFilter>();
            string name = gameObj.name;
            string text = "Null";
            if ((Object)component != (Object)null)
            {
                name = component.sharedMesh.name;
                text = "Mesh";
            }
            if (parentModelId == 0L)
            {
                stringBuilder2.AppendLine("\t;Model::" + name + ", Model::RootNode");
            }
            else
            {
                stringBuilder2.AppendLine("\t;Model::" + name + ", Model::USING PARENT");
            }
            stringBuilder2.AppendLine("\tC: \"OO\"," + randomFBXId2 + "," + parentModelId);
            stringBuilder2.AppendLine();
            stringBuilder.AppendLine("\tModel: " + randomFBXId2 + ", \"Model::" + gameObj.name + "\", \"" + text + "\" {");
            stringBuilder.AppendLine("\t\tVersion: 232");
            stringBuilder.AppendLine("\t\tProperties70:  {");
            stringBuilder.AppendLine("\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",4");
            stringBuilder.AppendLine("\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",1");
            stringBuilder.AppendLine("\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
            stringBuilder.AppendLine("\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            stringBuilder.AppendLine("\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            Vector3 localPosition = gameObj.transform.localPosition;
            stringBuilder.Append("\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A+\",");
            stringBuilder.AppendFormat("{0},{1},{2}", localPosition.x * -1f, localPosition.y, localPosition.z);
            stringBuilder.AppendLine();
            Vector3 localEulerAngles = gameObj.transform.localEulerAngles;
            stringBuilder.AppendFormat("\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A+\",{0},{1},{2}", localEulerAngles.x, localEulerAngles.y * -1f, -1f * localEulerAngles.z);
            stringBuilder.AppendLine();
            Vector3 localScale = gameObj.transform.localScale;
            stringBuilder.AppendFormat("\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",{0},{1},{2}", localScale.x, localScale.y, localScale.z);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("\t\t\tP: \"currentUVSet\", \"KString\", \"\", \"U\", \"map1\"");
            stringBuilder.AppendLine("\t\t}");
            stringBuilder.AppendLine("\t\tShading: T");
            stringBuilder.AppendLine("\t\tCulling: \"CullingOff\"");
            stringBuilder.AppendLine("\t}");
            if ((Object)component != (Object)null)
            {
                Mesh sharedMesh = component.sharedMesh;
                stringBuilder.AppendLine("\tGeometry: " + randomFBXId + ", \"Geometry::\", \"Mesh\" {");
                Vector3[] vertices = sharedMesh.vertices;
                stringBuilder.AppendLine("\t\tVertices: *" + sharedMesh.vertexCount * 3 + " {");
                stringBuilder.Append("\t\t\ta: ");
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.AppendFormat("{0},{1},{2}", vertices[i].x * -1f, vertices[i].y, vertices[i].z);
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\t\t} ");
                int num = sharedMesh.triangles.Length;
                int[] triangles = sharedMesh.triangles;
                stringBuilder.AppendLine("\t\tPolygonVertexIndex: *" + num + " {");
                stringBuilder.Append("\t\t\ta: ");
                for (int j = 0; j < num; j += 3)
                {
                    if (j > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.AppendFormat("{0},{1},{2}", triangles[j], triangles[j + 2], triangles[j + 1] * -1 - 1);
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\t\t} ");
                stringBuilder.AppendLine("\t\tGeometryVersion: 124");
                stringBuilder.AppendLine("\t\tLayerElementNormal: 0 {");
                stringBuilder.AppendLine("\t\t\tVersion: 101");
                stringBuilder.AppendLine("\t\t\tName: \"\"");
                stringBuilder.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                stringBuilder.AppendLine("\t\t\tReferenceInformationType: \"Direct\"");
                Vector3[] normals = sharedMesh.normals;
                stringBuilder.AppendLine("\t\t\tNormals: *" + num * 3 + " {");
                stringBuilder.Append("\t\t\t\ta: ");
                for (int k = 0; k < num; k += 3)
                {
                    if (k > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    Vector3 vector = normals[triangles[k]];
                    stringBuilder.AppendFormat("{0},{1},{2},", vector.x * -1f, vector.y, vector.z);
                    vector = normals[triangles[k + 2]];
                    stringBuilder.AppendFormat("{0},{1},{2},", vector.x * -1f, vector.y, vector.z);
                    vector = normals[triangles[k + 1]];
                    stringBuilder.AppendFormat("{0},{1},{2}", vector.x * -1f, vector.y, vector.z);
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t}");
                int num2 = sharedMesh.uv.Length;
                Vector2[] uv = sharedMesh.uv;
                stringBuilder.AppendLine("\t\tLayerElementUV: 0 {");
                stringBuilder.AppendLine("\t\t\tVersion: 101");
                stringBuilder.AppendLine("\t\t\tName: \"map1\"");
                stringBuilder.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                stringBuilder.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                stringBuilder.AppendLine("\t\t\tUV: *" + num2 * 2 + " {");
                stringBuilder.Append("\t\t\t\ta: ");
                for (int l = 0; l < num2; l++)
                {
                    if (l > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.AppendFormat("{0},{1}", uv[l].x, uv[l].y);
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\t\t\t\t}");
                stringBuilder.AppendLine("\t\t\tUVIndex: *" + num + " {");
                stringBuilder.Append("\t\t\t\ta: ");
                for (int m = 0; m < num; m += 3)
                {
                    if (m > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    int num3 = triangles[m];
                    int num4 = triangles[m + 2];
                    int num5 = triangles[m + 1];
                    stringBuilder.AppendFormat("{0},{1},{2}", num3, num4, num5);
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t}");
                if (sharedMesh.uv2.Length != 0)
                {
                    num2 = sharedMesh.uv2.Length;
                    uv = sharedMesh.uv2;
                    stringBuilder.AppendLine("\t\tLayerElementUV: 1 {");
                    stringBuilder.AppendLine("\t\t\tVersion: 101");
                    stringBuilder.AppendLine("\t\t\tName: \"map2\"");
                    stringBuilder.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                    stringBuilder.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                    stringBuilder.AppendLine("\t\t\tUV: *" + num2 * 2 + " {");
                    stringBuilder.Append("\t\t\t\ta: ");
                    for (int n = 0; n < num2; n++)
                    {
                        if (n > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        stringBuilder.AppendFormat("{0},{1}", uv[n].x, uv[n].y);
                    }
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("\t\t\t\t}");
                    stringBuilder.AppendLine("\t\t\tUVIndex: *" + num + " {");
                    stringBuilder.Append("\t\t\t\ta: ");
                    for (int num6 = 0; num6 < num; num6 += 3)
                    {
                        if (num6 > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        int num7 = triangles[num6];
                        int num8 = triangles[num6 + 2];
                        int num9 = triangles[num6 + 1];
                        stringBuilder.AppendFormat("{0},{1},{2}", num7, num8, num9);
                    }
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("\t\t\t}");
                    stringBuilder.AppendLine("\t\t}");
                }
                stringBuilder.AppendLine("\t\tLayerElementMaterial: 0 {");
                stringBuilder.AppendLine("\t\t\tVersion: 101");
                stringBuilder.AppendLine("\t\t\tName: \"\"");
                stringBuilder.AppendLine("\t\t\tMappingInformationType: \"ByPolygon\"");
                stringBuilder.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
                int num10 = 0;
                int subMeshCount = sharedMesh.subMeshCount;
                StringBuilder stringBuilder3 = new StringBuilder();
                if (subMeshCount == 1)
                {
                    int num11 = triangles.Length / 3;
                    for (int num12 = 0; num12 < num11; num12++)
                    {
                        stringBuilder3.Append("0,");
                        num10++;
                    }
                }
                else
                {
                    List<int[]> list = new List<int[]>();
                    for (int num13 = 0; num13 < subMeshCount; num13++)
                    {
                        list.Add(sharedMesh.GetIndices(num13));
                    }
                    for (int num14 = 0; num14 < triangles.Length; num14 += 3)
                    {
                        for (int num15 = 0; num15 < list.Count; num15++)
                        {
                            bool flag = false;
                            int num16 = 0;
                            while (num16 < list[num15].Length)
                            {
                                if (triangles[num14] != list[num15][num16] || triangles[num14 + 1] != list[num15][num16 + 1] || triangles[num14 + 2] != list[num15][num16 + 2])
                                {
                                    if (flag)
                                    {
                                        break;
                                    }
                                    num16 += 3;
                                    continue;
                                }
                                stringBuilder3.Append(num15.ToString());
                                stringBuilder3.Append(",");
                                num10++;
                                break;
                            }
                        }
                    }
                }
                stringBuilder.AppendLine("\t\t\tMaterials: *" + num10 + " {");
                stringBuilder.Append("\t\t\t\ta: ");
                stringBuilder.AppendLine(stringBuilder3.ToString());
                stringBuilder.AppendLine("\t\t\t} ");
                stringBuilder.AppendLine("\t\t}");
                stringBuilder.AppendLine("\t\tLayer: 0 {");
                stringBuilder.AppendLine("\t\t\tVersion: 100");
                stringBuilder.AppendLine("\t\t\tLayerElement:  {");
                stringBuilder.AppendLine("\t\t\t\tType: \"LayerElementNormal\"");
                stringBuilder.AppendLine("\t\t\t\tTypedIndex: 0");
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t\tLayerElement:  {");
                stringBuilder.AppendLine("\t\t\t\tType: \"LayerElementMaterial\"");
                stringBuilder.AppendLine("\t\t\t\tTypedIndex: 0");
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t\tLayerElement:  {");
                stringBuilder.AppendLine("\t\t\t\tType: \"LayerElementTexture\"");
                stringBuilder.AppendLine("\t\t\t\tTypedIndex: 0");
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t\tLayerElement:  {");
                stringBuilder.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
                stringBuilder.AppendLine("\t\t\t\tTypedIndex: 0");
                stringBuilder.AppendLine("\t\t\t}");
                stringBuilder.AppendLine("\t\t}");
                if (sharedMesh.uv2.Length != 0)
                {
                    stringBuilder.AppendLine("\t\tLayer: 1 {");
                    stringBuilder.AppendLine("\t\t\tVersion: 100");
                    stringBuilder.AppendLine("\t\t\tLayerElement:  {");
                    stringBuilder.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
                    stringBuilder.AppendLine("\t\t\t\tTypedIndex: 1");
                    stringBuilder.AppendLine("\t\t\t}");
                    stringBuilder.AppendLine("\t\t}");
                }
                stringBuilder.AppendLine("\t}");
                stringBuilder2.AppendLine("\t;Geometry::, Model::" + sharedMesh.name);
                stringBuilder2.AppendLine("\tC: \"OO\"," + randomFBXId + "," + randomFBXId2);
                stringBuilder2.AppendLine();
                MeshRenderer component2 = gameObj.GetComponent<MeshRenderer>();
                if ((Object)component2 != (Object)null)
                {
                    Material[] sharedMaterials = component2.sharedMaterials;
                    foreach (Material material in sharedMaterials)
                    {
                        int num18 = Mathf.Abs(material.GetInstanceID());
                        if ((Object)material == (Object)null)
                        {
                            Debug.LogError("ERROR: the game object " + gameObj.name + " has an empty material on it. This will export problematic files. Please fix and reexport");
                        }
                        else
                        {
                            stringBuilder2.AppendLine("\t;Material::" + material.name + ", Model::" + sharedMesh.name);
                            stringBuilder2.AppendLine("\tC: \"OO\"," + num18 + "," + randomFBXId2);
                            stringBuilder2.AppendLine();
                        }
                    }
                }
            }
            for (int num19 = 0; num19 < gameObj.transform.childCount; num19++)
            {
                FBXMeshGetter.GetMeshToString(gameObj.transform.GetChild(num19).gameObject, materials, ref stringBuilder, ref stringBuilder2, gameObj, randomFBXId2);
            }
            objects.Append(stringBuilder.ToString());
            connections.Append(stringBuilder2.ToString());
            return randomFBXId2;
        }
    }
}
