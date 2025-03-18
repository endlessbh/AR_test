using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    public class DefaultPathHandler : InstanceClass<DefaultPathHandler>, IPathHandler
    {
        public string persistentDataPath
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                return Application.persistentDataPath + "/";
#else
                return Application.streamingAssetsPath + "/";
#endif
            }
        }

        public string fileProtocol
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                return "file://";
#else
                return "";
#endif
            }
        }
    }
}
