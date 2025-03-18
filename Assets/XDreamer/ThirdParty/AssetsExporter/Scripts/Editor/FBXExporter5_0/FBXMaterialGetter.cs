using System.Text;
using UnityEngine;

namespace XCSJ.EditorAssetsExporter.FBXExporter5_0
{
    public class FBXMaterialGetter
    {
        public static void GetAllMaterialsToString(GameObject gameObj, string newPath, bool copyMaterials, bool copyTextures, Material[] materials, out string matObjects, out string connections)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (Material material in materials)
            {
                string name = material.name;
                int num = Mathf.Abs(material.GetInstanceID());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\tMaterial: " + num + ", \"Material::" + name + "\", \"\" {");
                stringBuilder.AppendLine("\t\tVersion: 102");
                stringBuilder.AppendLine("\t\tShadingModel: \"phong\"");
                stringBuilder.AppendLine("\t\tMultiLayer: 0");
                stringBuilder.AppendLine("\t\tProperties70:  {");
                if (material.HasProperty("_Color"))
                {
                    stringBuilder.AppendFormat("\t\t\tP: \"Diffuse\", \"Vector3D\", \"Vector\", \"\",{0},{1},{2}", material.color.r, material.color.g, material.color.b);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",{0},{1},{2}", material.color.r, material.color.g, material.color.b);
                }
                stringBuilder.AppendLine();
                if (material.HasProperty("_SpecColor"))
                {
                    Color color = material.GetColor("_SpecColor");
                    stringBuilder.AppendFormat("\t\t\tP: \"Specular\", \"Vector3D\", \"Vector\", \"\",{0},{1},{2}", color.r, color.g, color.r);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("\t\t\tP: \"SpecularColor\", \"ColorRGB\", \"Color\", \" \",{0},{1},{2}", color.r, color.g, color.b);
                    stringBuilder.AppendLine();
                }
                if (material.HasProperty("_Mode"))
                {
                    Color white = Color.white;
                    switch ((int)material.GetFloat("_Mode"))
                    {
                        case 2:
                            white = material.GetColor("_Color");
                            stringBuilder.AppendFormat("\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",{0},{1},{2}", white.r, white.g, white.b);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",{0}", white.a);
                            stringBuilder.AppendLine();
                            break;
                        case 3:
                            white = material.GetColor("_Color");
                            stringBuilder.AppendFormat("\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",{0},{1},{2}", white.r, white.g, white.b);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendFormat("\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",{0}", white.a);
                            stringBuilder.AppendLine();
                            break;
                    }
                }
                if (material.HasProperty("_EmissionColor"))
                {
                    Color color2 = material.GetColor("_EmissionColor");
                    stringBuilder.AppendFormat("\t\t\tP: \"Emissive\", \"Vector3D\", \"Vector\", \"\",{0},{1},{2}", color2.r, color2.g, color2.b);
                    stringBuilder.AppendLine();
                    float num2 = (color2.r + color2.g + color2.b) / 3f;
                    stringBuilder.AppendFormat("\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",{0}", num2);
                    stringBuilder.AppendLine();
                }
                stringBuilder.AppendLine("\t\t}");
                stringBuilder.AppendLine("\t}");
                string value = default(string);
                string value2 = default(string);
                FBXMaterialGetter.SerializedTextures(gameObj, newPath, material, name, copyTextures, out value, out value2);
                stringBuilder.Append(value);
                stringBuilder2.Append(value2);
            }
            matObjects = stringBuilder.ToString();
            connections = stringBuilder2.ToString();
        }

        private static void SerializedTextures(GameObject gameObj, string newPath, Material material, string materialName, bool copyTextures, out string objects, out string connections)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            int materialId = Mathf.Abs(material.GetInstanceID());
            Texture texture = material.GetTexture("_MainTex");
            string value = null;
            string value2 = null;
            if ((Object)texture != (Object)null)
            {
                FBXMaterialGetter.SerializeOneTexture(gameObj, newPath, material, materialName, materialId, copyTextures, "_MainTex", "DiffuseColor", out value, out value2);
                stringBuilder.AppendLine(value);
                stringBuilder2.AppendLine(value2);
            }
            if (FBXMaterialGetter.SerializeOneTexture(gameObj, newPath, material, materialName, materialId, copyTextures, "_BumpMap", "NormalMap", out value, out value2))
            {
                stringBuilder.AppendLine(value);
                stringBuilder2.AppendLine(value2);
            }
            connections = stringBuilder2.ToString();
            objects = stringBuilder.ToString();
        }

        private static bool SerializeOneTexture(GameObject gameObj, string newPath, Material material, string materialName, int materialId, bool copyTextures, string unityExtension, string textureType, out string objects, out string connections)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            if ((Object)material.GetTexture(unityExtension) == (Object)null)
            {
                objects = "";
                connections = "";
                return false;
            }
            objects = stringBuilder.ToString();
            connections = stringBuilder2.ToString();
            return true;
        }
    }
}
