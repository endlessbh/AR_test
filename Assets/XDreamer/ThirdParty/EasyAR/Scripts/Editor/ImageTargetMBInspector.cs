using UnityEngine;
using System.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginEasyAR;
using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.EditorEasyAR
{
    /// <summary>
    /// 图片目标检查器
    /// </summary>
    [Name("图片目标检查器")]
    /// <summary>
    /// 特别注意: 本类对应的Inspector必须使用 EasyAR默认的 Inspector 进行渲染，因为有些私有的成员需要通过该界面进行信息填充；
    /// </summary>
    [CustomEditor(typeof(ImageTargetMB))]
#if XDREAMER_EASYAR_4_0_1 
    public class ImageTargetMBInspector : ImageTargetControllerEditor
    {

        public ImageTargetMB targetObject { get { return target as ImageTargetMB; } }

        public ScriptImageTargetEvent imageTargetEvent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(ScriptImageTargetEvent)), (imageTargetEvent && imageTargetEvent.enabled)))
            {
                if (imageTargetEvent) imageTargetEvent.enabled = false;
            }
            else
            {
                if (!imageTargetEvent)
                {
                    imageTargetEvent = targetObject.gameObject.AddComponent<ScriptImageTargetEvent>();
                }
                else
                {
                    imageTargetEvent.enabled = true;
                }
            }
            if (Application.isPlaying) return;
            if(targetObject.marker == null)
            {
                targetObject.ImageFileSource.PathType = PathType.Absolute;
                targetObject.ImageFileSource.Path = "";
                targetObject.ImageFileSource.Name = "";
            }
            else
            {
                targetObject.ImageFileSource.PathType = PathType.Absolute;
                targetObject.ImageFileSource.Path = AssetDatabase.GetAssetPath(targetObject.marker);
                targetObject.ImageFileSource.Name = targetObject.marker.name;
            }
        }

        public new void OnEnable()
        {
            base.OnEnable();
            if (!imageTargetEvent)
            {
                imageTargetEvent = targetObject.GetComponent<ScriptImageTargetEvent>();
            }
        }
    }
#elif XDREAMER_EASYAR_3_0_1
    public class ImageTargetMBInspector : BaseInspector
    {

        public ImageTargetMB targetObject { get { return target as ImageTargetMB; } }

        public ScriptImageTargetEvent imageTargetEvent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(ScriptImageTargetEvent)), (imageTargetEvent && imageTargetEvent.enabled)))
            {
                if (imageTargetEvent) imageTargetEvent.enabled = false;
            }
            else
            {
                if (!imageTargetEvent)
                {
                    imageTargetEvent = targetObject.gameObject.AddComponent<ScriptImageTargetEvent>();
                }
                else
                {
                    imageTargetEvent.enabled = true;
                }
            }
        }

        protected override void OnEnable()
        {
            if (!imageTargetEvent)
            {
                imageTargetEvent = targetObject.GetComponent<ScriptImageTargetEvent>();
            }
        }
    }
#elif XDREAMER_EASYAR_2_3_0
    public class ImageTargetMBInspector : ImageTargetEditor
    {

        public ImageTargetMB targetObject { get { return target as ImageTargetMB; } }

        public ScriptImageTargetEvent imageTargetEvent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(ScriptImageTargetEvent)), (imageTargetEvent && imageTargetEvent.enabled)))
            {
                if (imageTargetEvent) imageTargetEvent.enabled = false;
            }
            else
            {
                if (!imageTargetEvent)
                {
                    imageTargetEvent = targetObject.gameObject.AddComponent<ScriptImageTargetEvent>();
                }
                else
                {
                    imageTargetEvent.enabled = true;
                }
            }
        }

        public void OnEnable()
        {
            var method = typeof(ImageTargetEditor).GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(this, null);
            }
            if (!imageTargetEvent)
            {
                imageTargetEvent = targetObject.GetComponent<ScriptImageTargetEvent>();
            }
        }
    }
#else 
    public class ImageTargetMBInspector : MBInspector<ImageTargetMB> { }
#endif
}
