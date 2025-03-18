using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginEasyAR.States
{
    [ComponentMenu("Easy AR/图像识别触发", typeof(EasyARManager))]
    [Name("图像识别触发", nameof(ImageRecognition))]
    [Attributes.Icon]
    [Tip("图像识别触发组件是摄像头识别特定图片的触发器。使用摄像头对着设定的图片，识别后组件切换为完成态。", "The image recognition trigger component is a trigger for the camera to recognize a specific picture. Use the camera to face the set picture and switch the component to the completed state after recognition.")]
    [Owner(typeof(EasyARManager))]
    public class ImageRecognition : Trigger<ImageRecognition>
    {
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("Easy AR", typeof(EasyARManager))]
        [StateComponentMenu("Easy AR/图像识别触发", typeof(EasyARManager))]
#endif
        [Name("图像识别触发", nameof(ImageRecognition))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("图像识别触发组件是摄像头识别特定图片的触发器。使用摄像头对着设定的图片，识别后组件切换为完成态。", "The image recognition trigger component is a trigger for the camera to recognize a specific picture. Use the camera to face the set picture and switch the component to the completed state after recognition.")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("识别图像列表")]
        public List<Texture2D> textures = new List<Texture2D>();
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1 || XDREAMER_EASYAR_2_3_0
        GameObject recognizedGo;
#endif

        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="stateData">状态数据</param>
        public override void OnEntry(StateData stateData)
        {
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1 || XDREAMER_EASYAR_2_3_0
            if (textures.Count > 0)
            {
                if (recognizedGo == null)
                {
                    recognizedGo = new GameObject("图片识别集_temp");
                    recognizedGo.transform.position = Vector3.zero;
                    recognizedGo.transform.localScale = Vector3.one;
                    recognizedGo.SetActive(false);
                }   
            }
            for(int i=0;i<textures.Count;i++)
            {
                ImageTargetMB imageTargetMB = recognizedGo.AddComponent<ImageTargetMB>();
                imageTargetMB.marker = textures[i];
                imageTargetMB.TargetFound += OnTargetFound;
            }
            recognizedGo.SetActive(true);
#endif
            base.OnEntry(stateData);
        }

#if XDREAMER_EASYAR_4_0_1
        void OnTargetFound()
        {
            finished = true;
        }
#elif XDREAMER_EASYAR_3_0_1
        void OnTargetFound(ExtendImageTargetController controller)
        {
            finished = true;
        }
#elif XDREAMER_EASYAR_2_3_0
        protected void OnTargetFound(TargetAbstractBehaviour behaviour)
        {
            finished = true;
        }
#else
        public override bool Finished()
        {
            return true;
        }
#endif

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="stateData">状态数据</param>
        public override void OnExit(StateData stateData)
        {
#if XDREAMER_EASYAR_3_0_1 || XDREAMER_EASYAR_2_3_0
            if (recognizedGo) Destroy(recognizedGo);
#endif
            base.OnExit(stateData);
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && textures.Count != 0;
        }
    }
}
