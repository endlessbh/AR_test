using UnityEngine;
using System.Collections;
using System.Reflection;
using System.IO;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// 图片目标类，用于添加图片识别的marker<br />
    /// 特别注意: 本类对应的Inspector必须使用 EasyAR默认的 Inspector 进行渲染，因为有些私有的成员需要通过该界面进行信息填充；
    /// </summary>
    [Name("图片目标")]
    [Serializable]
#if XDREAMER_EASYAR_4_0_1
    public class ImageTargetMB : ImageTargetController
#elif XDREAMER_EASYAR_3_0_1
    public class ImageTargetMB : ExtendImageTargetController
#elif XDREAMER_EASYAR_2_3_0
    public class ImageTargetMB : ImageTargetBaseBehaviour
#else
    public class ImageTargetMB : BaseEasyARMB
#endif
    {
        [Name("识别图")]
        [Tip("AR相机识别使用的Marker（识别图）;如果本参数有效会使用本参数的信息覆盖path、name、size等信息；", "Marker used for AR camera identification; If this parameter is valid, the information of this parameter will be used to overwrite path, name, size and other information;")]
        public Texture2D marker;

#if XDREAMER_EASYAR_4_0_1
        void Awake()
        {
            if (marker)
            {
                ImageFileSource.PathType = PathType.Absolute;
                if (string.IsNullOrEmpty(ImageFileSource.Name)) ImageFileSource.Name = marker.name;

                //将图片输出磁盘并获取绝对路径
                ImageFileSource.Path = string.Format("{0}/{1}.jpg", Application.persistentDataPath, marker.name);
                //Debug.Log(this.Path);
                File.WriteAllBytes(ImageFileSource.Path, marker.EncodeToJPG());
            }

            if (Tracker == null) Tracker = FindObjectOfType<ImageTrackerFrameFilter>();
        }
#elif XDREAMER_EASYAR_3_0_1

        void Awake()
        {
            if (marker)
            {
                this.Type = PathType.Absolute;
                if (string.IsNullOrEmpty(this.TargetName)) this.TargetName = marker.name;

                //将图片输出磁盘并获取绝对路径
                this.TargetPath = string.Format("{0}/{1}.jpg", Application.persistentDataPath, marker.name);
                //Debug.Log(this.Path);
                File.WriteAllBytes(this.TargetPath, marker.EncodeToJPG());
            }

            if (ImageTracker == null) ImageTracker = GameObject.FindObjectOfType<ExtendImageTrackerBehaviour>();
        }
#elif XDREAMER_EASYAR_2_3_0
        protected override void Start()
        {
            if (marker)
            {
                if (string.IsNullOrEmpty(this.Name)) this.Name = marker.name;

                //将图片输出磁盘并获取绝对路径
                this.Path = string.Format("{0}/{1}.jpg", Application.persistentDataPath, marker.name);
                //Debug.Log(this.Path);
                File.WriteAllBytes(this.Path, marker.EncodeToJPG());
            }

            this.Storage = StorageType.Absolute;

            if (this.GameObjectActiveControl)
            {
                base.gameObject.SetActive(false);
            }

            if (!this.SetupWithImage(this.Path, this.Storage, this.Name, this.Size))
            {
                Log.ErrorFormat("EasyAR使用 Marker(识别图) 启动失败!");
            }
        }
#else
        protected void Start() { }
#endif

        #region 回调消息 -- 已注释 -- 回调事件由ScriptEasyAREvent组件完成

        /*
        protected override void Awake()
        {
            base.Awake();
            TargetFound += OnTargetFound;
            TargetLost += OnTargetLost;
            TargetLoad += OnTargetLoad;
            TargetUnload += OnTargetUnload;
        }

        void OnTargetFound(TargetAbstractBehaviour behaviour)
        {
            Debug.Log("Found: " + Target.Id);
        }

        void OnTargetLost(TargetAbstractBehaviour behaviour)
        {
            Debug.Log("Lost: " + Target.Id);
        }

        void OnTargetLoad(ImageTargetBaseBehaviour behaviour, ImageTrackerBaseBehaviour tracker, bool status)
        {
            Debug.Log("Load target (" + status + "): " + Target.Id + " (" + Target.Name + ") " + " -> " + tracker);
        }

        void OnTargetUnload(ImageTargetBaseBehaviour behaviour, ImageTrackerBaseBehaviour tracker, bool status)
        {
            Debug.Log("Unload target (" + status + "): " + Target.Id + " (" + Target.Name + ") " + " -> " + tracker);
        }
         */

        #endregion
    }
}
