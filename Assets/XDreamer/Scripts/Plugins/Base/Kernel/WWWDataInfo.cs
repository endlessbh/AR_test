using System;
using UnityEngine;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
#if UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中WWW类被标识过时,所以本类不推荐使用;")]
#endif
    public class WWWDataInfo : IDataInfo
    {
        public DataRequest dataRequest { get; private set; } = null;

        public WWW www { get; private set; } = null;

        public Texture2D texture => www.texture;

        public AudioClip audioClip => www.GetAudioClip();

        public AssetBundle assetBundle => www.assetBundle;

        public string fullPath => www.url;

        public string url => www.url;

        public string text => www.text;

        public byte[] bytes => www.bytes;

        public bool isError => !string.IsNullOrEmpty(www.error);

        public string error => www.error;

        public float progress => www == null ? 0 : www.progress;

        public bool isDone => www == null ? false : www.isDone;

        public WWWDataInfo(WWW www, DataRequest dataRequest)
        {
#if CSHARP_7_3_OR_NEWER
            this.www = www ?? throw new ArgumentNullException(nameof(www));
            this.dataRequest = dataRequest ?? throw new ArgumentNullException(nameof(dataRequest));
#else
            if(www == null) throw new ArgumentNullException(nameof(www));
            if(dataRequest == null) throw new ArgumentNullException(nameof(dataRequest));
            this.www = www;
            this.dataRequest = dataRequest;
#endif
        }
    }
}
