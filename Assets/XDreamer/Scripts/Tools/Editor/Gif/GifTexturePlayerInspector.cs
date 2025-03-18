using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Gif;

namespace XCSJ.EditorTools.Gif
{
    /// <summary>
    /// GIF纹理播放器检查器
    /// </summary>
    [Name("GIF纹理播放器检查器")]
    [CustomEditor(typeof(GifTextureUpdater), true)]
    public class GifTexturePlayerInspector : PlayableContentInspector<GifTextureUpdater>
    {
        private static string[] imageExtends = new string[] { ".png", ".jpg", ".jpeg" };

        protected override bool OnDrawGenericMemberHead(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {

                case nameof(targetObject._gifTexture):
                    {
                        if (!Application.isPlaying && targetObject._gifTexture._gifSource != EGifSource.SequenceFrames) break;

                        return DrawGenericMemberHeadIfAllow(serializedProperty, propertyData, null, () =>
                        {
                            if (GUILayout.Button(CommonFun.NameTip(EIcon.Update), UICommonOption.WH24x16))
                            {
                                targetObject.XModifyProperty(() =>
                                {
                                    targetObject._gifTexture.sequenceFrames.MarkDirty();
                                    var ttl = targetObject._gifTexture.sequenceFrames.GetTotalTimeLength();
                                    targetObject.timeLength = ttl;
                                    targetObject._onceTimeLength = ttl;
                                });
                            }
                        });
                    }
                case nameof(targetObject._gifTexture._gifSequenceFrames):
                    {
                        return DrawGenericMemberHeadIfAllow(serializedProperty, propertyData, null, () =>
                        {
                            if (GUILayout.Button(CommonFun.NameTip(EIcon.Folder), UICommonOption.WH24x16))
                            {
                                string path = EditorUtility.OpenFolderPanel("选择导入图片文件夹", Application.dataPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    string[] filePaths = Directory.GetFiles(path, "*.*").Where(s => imageExtends.ToList().Exists(i => s.EndsWith(i))).ToArray();

                                    targetObject.XModifyProperty(() =>
                                    {
                                        foreach (var fp in filePaths)
                                        {
                                            var relativePath = fp.Substring(fp.IndexOf("Assets"));
                                            var rawImg = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
                                            targetObject._gifTexture._gifSequenceFrames._frameTextures.Add(new FrameTexture(rawImg, 0.04f));// 按一秒25张图的效果
                                        }
                                        targetObject._gifTexture._gifSequenceFrames.MarkDirty();
                                    });
                                }
                            }
                        });
                    }
            }
            return base.OnDrawGenericMemberHead(serializedProperty, propertyData);
        }
    }
}
