using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 渲染顺序:渲染顺序组件是用于设置游戏对象的渲染顺序的执行体
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RenderQueue))]
    [Tip("渲染顺序组件是用于设置游戏对象的渲染顺序的执行体", "The render order component is the executor used to set the render order of game objects")]
    [XCSJ.Attributes.Icon(index = 33636)]
    [RequireComponent(typeof(GameObjectSet))]
    public class RenderQueue : StateComponent<RenderQueue>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染顺序";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(RenderQueue))]
        [Tip("渲染顺序组件是用于设置游戏对象的渲染顺序的执行体", "The render order component is the executor used to set the render order of game objects")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateRenderQueue(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("渲染模式")]
        public enum ERenderingMode
        {
            [Tip("无操作", "无操作")]
            None,

            [Tip("默认")]
            Default,

            [Tip("不透明", "Opaque")]
            Opaque,

            [Tip("镂空", "Cutout")]
            Cutout,

            [Tip("隐现", "Fade")]
            Fade,

            [Tip("透明", "Transparent")]
            Transparent,
        }

        //[Name("渲染顺序")]
        //public ERenderingMode renderingMode = ERenderingMode.Opaque;

        //[Name("进入时切换")]
        //[Tip("勾选,进入时切换渲染顺序;不勾,退出时切换渲染顺序;")]
        //public bool switchWhenStart = false;

        [Name("进入时渲染顺序")]
        [EnumPopup]
        public ERenderingMode enterRenderingMode = ERenderingMode.None;

        [Name("退出时渲染顺序")]
        [EnumPopup]
        public ERenderingMode exitRenderingMode = ERenderingMode.None;

        [Name("包含成员")]
        public bool includeChildren = true;

        private List<Recorder> recorders = new List<Recorder>();

        public override bool Init(StateData data)
        {
            List<GameObject> objects = gameObjectSet.objects;
            foreach (var go in objects)
            {
                Recorder recorder = new Recorder();
                recorder.gameObject = go;
                recorder.includeChildren = includeChildren;
                recorder.Record();
                recorders.Add(recorder);
            }
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            
            base.OnEntry(data);
            if(enterRenderingMode == ERenderingMode.None) return;
            if(enterRenderingMode == ERenderingMode.Default)
            {
                foreach (var recoder in recorders)
                {
                    recoder.ResetRecorder();
                }
            }
            else
            {
                foreach (var recoder in recorders)
                {
                    recoder.SetRenderingMode(enterRenderingMode);
                }
            }
        }

        public override bool Finished()
        {
            return true;
        }

        public override void OnExit(StateData data)
        {
            if (exitRenderingMode != ERenderingMode.None)
            {
                if(exitRenderingMode == ERenderingMode.Default && enterRenderingMode != ERenderingMode.Default)
                {
                    foreach (var recoder in recorders)
                    {
                        recoder.ResetRecorder();
                    }
                }
                else if(exitRenderingMode != ERenderingMode.Default)
                {
                    foreach (var recoder in recorders)
                    {
                        recoder.SetRenderingMode(exitRenderingMode);
                    }
                }
            }
            base.OnExit(data);
        }

        public static void SetMaterialRenderingMode(Material material, ERenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case ERenderingMode.Opaque:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;
                        break;
                    }
                case ERenderingMode.Cutout:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 2450;
                        break;
                    }
                case ERenderingMode.Fade:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    }
                case ERenderingMode.Transparent:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    }
                default:
                    break;
            }
        }

        public static void SetMaterialsRenderingMode(Material[] materials, ERenderingMode renderingMode)
        {
            foreach (Material mat in materials)
                SetMaterialRenderingMode(mat, renderingMode);
        }

        public class Recorder
        {
            public GameObject gameObject;

            public bool includeChildren;

            public List<MaterialRenderQuene> renderQuenes = new List<MaterialRenderQuene>();

            public void Record()
            {
                if (!gameObject) return;
                renderQuenes.Clear();
                if (includeChildren)
                {
                    Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer ren in renderers)
                    {
                        Record(ren);
                    }
                }
                else
                {
                    Record(gameObject.GetComponent<Renderer>());
                }
            }

            private void Record(Renderer renderer)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (!materials[i].shader.name.Contains("Standard")) continue;
                    ERenderingMode renderingMode = ERenderingMode.Opaque;
                    if (materials[i].renderQueue == 2000)
                    {
                        renderingMode = ERenderingMode.Opaque;
                    }
                    else if (materials[i].renderQueue == 2450)
                    {
                        renderingMode = ERenderingMode.Cutout;
                    }
                    else if (materials[i].renderQueue == 3000 && materials[i].GetInt("_SrcBlend") == (int)UnityEngine.Rendering.BlendMode.SrcAlpha)
                    {
                        renderingMode = ERenderingMode.Fade;
                    }
                    else if (materials[i].renderQueue == 3000)
                    {
                        renderingMode = ERenderingMode.Transparent;
                    }
                    renderQuenes.Add(new MaterialRenderQuene(materials[i], renderingMode));
                }
            }

            public void ResetRecorder()
            {
                for(int i=0;i<renderQuenes.Count;i++)
                {
                    SetMaterialRenderingMode(renderQuenes[i].material, renderQuenes[i].renderingMode); 
                }
            }

            public void SetRenderingMode(ERenderingMode renderingMode)
            {
                for (int i = 0; i < renderQuenes.Count; i++)
                {
                    SetMaterialRenderingMode(renderQuenes[i].material, renderingMode);
                }
            }
        }

        public class MaterialRenderQuene
        {
            public Material material;

            [EnumPopup]
            public ERenderingMode renderingMode;

            public MaterialRenderQuene(Material mat, ERenderingMode eRenderingMode)
            {
                this.material = mat;
                this.renderingMode = eRenderingMode;
            }
        }

    }
}
