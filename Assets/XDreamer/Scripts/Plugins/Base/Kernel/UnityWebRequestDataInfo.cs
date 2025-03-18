using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    public class UnityWebRequestDataInfo : IDataInfo
    {
        public DataRequest dataRequest { get; private set; } = null;

        public UnityWebRequest unityWebRequest { get; private set; } = null;

        public Texture2D texture
        {
            get
            {
                try
                {
                    return DownloadHandlerTexture.GetContent(unityWebRequest);
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        var tex = new Texture2D(2, 2);
                        tex.name = Path.GetFileNameWithoutExtension(unityWebRequest.url);
                        tex.LoadImage(unityWebRequest.downloadHandler.data, false);
                        return tex;
                    }
                }
                catch { }

                return null;
            }
        }

        public AudioClip audioClip
        {
            get
            {
                try
                {
#if CSHARP_7_3_OR_NEWER
                    if (DownloadHandlerAudioClip.GetContent(unityWebRequest) is AudioClip audio && audio)
                    {
                        return audio;
                    }
#else
                    AudioClip audio = DownloadHandlerAudioClip.GetContent(unityWebRequest) as AudioClip;
                    if (audio)
                    {
                        return audio;
                    }   

#endif
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        return AudioClipHelper.LoadFromMemory(unityWebRequest.downloadHandler.data, Path.GetFileNameWithoutExtension(unityWebRequest.url));
                    }
                }
                catch { }

                return null;
            }
        }

        public AssetBundle assetBundle
        {
            get
            {
                try
                {
                    return DownloadHandlerAssetBundle.GetContent(unityWebRequest);
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        return AssetBundle.LoadFromMemory(unityWebRequest.downloadHandler.data);
                    }
                }
                catch { }

                return null;
            }
        }

        public string fullPath => unityWebRequest.url; //unityWebRequest.uri.AbsolutePath;

        public string url => unityWebRequest.url;

        public string text => unityWebRequest.downloadHandler.text;

        public byte[] bytes => unityWebRequest.downloadHandler.data;

#if UNITY_2020_2_OR_NEWER
        public bool isError => unityWebRequest.result == UnityWebRequest.Result.ConnectionError;
#else
        public bool isError => unityWebRequest.isNetworkError;
#endif

        public string error => unityWebRequest.error;

        public float progress => unityWebRequest.downloadProgress;

        public bool isDone => unityWebRequest.isDone;

        public UnityWebRequestDataInfo(UnityWebRequest unityWebRequest, DataRequest dataRequest)
        {
#if CSHARP_7_3_OR_NEWER
            this.unityWebRequest = unityWebRequest ?? throw new ArgumentNullException(nameof(unityWebRequest));
            this.dataRequest = dataRequest ?? throw new ArgumentNullException(nameof(dataRequest));
#else
            if(unityWebRequest == null) throw new ArgumentNullException(nameof(unityWebRequest));
            if (dataRequest == null) throw new ArgumentNullException(nameof(dataRequest));
            this.unityWebRequest = unityWebRequest;
            this.dataRequest = dataRequest;
#endif
        }
    }
}
