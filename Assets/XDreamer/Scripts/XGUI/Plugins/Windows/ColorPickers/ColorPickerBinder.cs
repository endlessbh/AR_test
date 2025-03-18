using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色拾取绑定对象类型
    /// </summary>
    [Name("颜色拾取绑定对象类型")]
    public enum EColorPickerBindType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        GameObject,

        /// <summary>
        /// 选择集
        /// </summary>
        [Name("选择集")]
        Selection,

        /// <summary>
        /// 灯光
        /// </summary>
        [Name("灯光")]
        Light,

        /// <summary>
        /// 图形
        /// </summary>
        [Name("图形")]
        Graphic,
    }

    /// <summary>
    /// 颜色拾取绑定器
    /// </summary>

    [Name("颜色拾取绑定器")]
    public class ColorPickerBinder : ColorPickerObserver, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 包含子对象
        /// </summary>
        [Group("绑定配置")]
        [Name("绑定对象包含子对象")]
        public bool _includeChildren = true;

        /// <summary>
        /// 颜色作用选择集
        /// </summary>
        [Name("绑定对象类型")]
        [Tip("当颜色拾取器启动时，会使用绑定对象的一个材质颜色初始化颜色拾取器的颜色", "When the color picker starts, the color of the color picker is initialized with one of the material colors of the bound object")]
        [EnumPopup]
        public EColorPickerBindType _bindType = EColorPickerBindType.GameObject;

        /// <summary>
        /// 绑定类型
        /// </summary>
        public EColorPickerBindType bindType
        {
            get => _bindType;
            set 
            {
                _bindType = value;
                BinderToColorPicker();
            }
        }

        /// <summary>
        /// 作用游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        [HideInSuperInspector(nameof(_bindType), EValidityCheckType.NotEqual, EColorPickerBindType.GameObject)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [Readonly(EEditorMode.Runtime)]
        public List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// 灯光列表
        /// </summary>
        [Name("灯光列表")]
        [HideInSuperInspector(nameof(_bindType), EValidityCheckType.NotEqual, EColorPickerBindType.Light)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [Readonly(EEditorMode.Runtime)]
        public List<Light> lights = new List<Light>();

        /// <summary>
        /// 图形列表
        /// </summary>
        [Name("图形列表")]
        [HideInSuperInspector(nameof(_bindType), EValidityCheckType.NotEqual, EColorPickerBindType.Graphic)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [Readonly(EEditorMode.Runtime)]
        public List<Graphic> graphics = new List<Graphic>();

        /// <summary>
        /// 材质名称过滤器 ：当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色
        /// </summary>
        [Name("材质名称过滤器")]
        [Tip("当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色", "When an object has multiple materials, you can use filtering rules to accurately locate the material color that needs to be modified")]
        public XStringComparer _comparer = new XStringComparer();

        /// <summary>
        /// 变量
        /// </summary>
        [Group("中文脚本配置")]
        [Name("变量")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variable);
        }

        #endregion

        /// <summary>
        /// 中文回调函数
        /// </summary>
        [Name("回调中文脚本函数")]
        [UserDefineFun]
        public string _cnFunction;

        private Dictionary<GameObject, Renderer[]> _goRendererMap = new Dictionary<GameObject, Renderer[]>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void Start()
        {
            BinderToColorPicker();
            gameObjects.ForEach(go => GetAndAddRendererMap(go));
        }

        /// <summary>
        /// 响应选择集变化,并设置颜色拾取器的颜色
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        private void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            BinderToColorPicker();
        }

        /// <summary>
        /// 颜色变化回调函数
        /// </summary>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            _variable.TrySetOrAddSetHierarchyVarValue(CommonFun.ColorToString(color)); 
            ScriptManager.CallUDF(_cnFunction);

            switch (bindType)
            {
                case EColorPickerBindType.None: break;
                case EColorPickerBindType.GameObject:
                    {
                        SetColor(color, gameObjects.ToArray());
                        break;
                    }
                case EColorPickerBindType.Selection:
                    {
                        SetColor(color, Selection.selections.Where(obj => obj is GameObject).Cast(o => (GameObject)o).ToArray());
                        break;
                    }
                case EColorPickerBindType.Light:
                    {
                        lights.ForEach(l =>
                        {
                            if (l)
                            {
                                l.color = color;
                            }
                        });
                        break;
                    }
                case EColorPickerBindType.Graphic:
                    {
                        graphics.ForEach(g =>
                        {
                            if (g)
                            {
                                g.color = color;
                            }
                        });
                        break;
                    }
                default: break;
            }
        }

        private void SetColor(Color color, params GameObject[] gos)
        {
            gos.Foreach(go =>
            {
                if (go)
                {
                    SetMaterialColor(color, GetAndAddRendererMap(go));
                }
            });
        }

        private void SetMaterialColor(Color color, params Renderer[] renderers)
        {
            renderers.Foreach( r =>
            {
                r.materials.Foreach(m =>
                {
                    if (m && _comparer.IsMatch(m.name))
                    {
                        m.color = color;
                    }
                });
            });
        }

        private bool TryGetMaterialColor(Renderer renderer, out Color color)
        {
            foreach (var m in renderer.materials)
            {
                if (m && _comparer.IsMatch(m.name))
                {
                    color = m.color;
                    return true;
                }
            }
            color = Color.white;
            return false;
        }

        /// <summary>
        /// 游戏对象材质颜色同步至颜色拾取器
        /// </summary>
        public void BinderToColorPicker()
        {
            if (_colorPicker)
            {
                switch (bindType)
                {
                    case EColorPickerBindType.None: break;
                    case EColorPickerBindType.GameObject: RendererToColorPicker(gameObjects); break;
                    case EColorPickerBindType.Selection:
                        {
                            if (Selection.countValid > 0)
                            {
                                RendererToColorPicker(Selection.selections);
                            }
                            break;
                        }
                    case EColorPickerBindType.Light:
                        {
                            var light = lights.Find(l => l);
                            if (light)
                            {
                                _colorPicker.color = light.color;
                            }
                            break;
                        }
                    case EColorPickerBindType.Graphic:
                        {
                            var graphic = graphics.Find(g => g);
                            if (graphic)
                            {
                                _colorPicker.color = graphic.color;
                            }
                            break;
                        }
                    default: break;
                }
            }            
        }

        [LanguageTuple("The palette failed to pick the renderer material color according to the matching rule", "调色板未能按匹配规则拾取到渲染器材质颜色")]
        private bool RendererToColorPicker(IEnumerable<GameObject> gameObjectSet)
        {
            foreach (var go in gameObjectSet)
            {
                var r = go.GetComponentInChildren<Renderer>();
                if (r && TryGetMaterialColor(r, out var color))
                {
                    _colorPicker.color = color;
                    return true;
                }
            }
            Debug.LogWarning("The palette failed to pick the renderer material color according to the matching rule".Tr(this.GetType()));
            return false;
        }

        /// <summary>
        /// 添加绑定游戏对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="removeOtherObject"></param>
        public void AddGameObject(GameObject go, bool removeOtherObject)
        {
            if (go)
            {
                if (removeOtherObject)
                {
                    ClearGameObject();
                }
                gameObjects.Add(go);
                BinderToColorPicker();
            }
        }

        /// <summary>
        /// 移除绑定游戏对象
        /// </summary>
        /// <param name="go"></param>
        public void RemoveGameObject(GameObject go)
        {
            if (go)
            {
                _goRendererMap.Remove(go);
                gameObjects.Remove(go);
            }
        }

        /// <summary>
        /// 清除绑定游戏对象
        /// </summary>
        public void ClearGameObject()
        {
            foreach (var go in gameObjects)
            {
                _goRendererMap.Remove(go);
            }
            gameObjects.Clear();
        }

        /// <summary>
        /// 获取游戏对象渲染器，并记录到图中
        /// </summary>
        /// <param name="go"></param>
        private Renderer[] GetAndAddRendererMap(GameObject go)
        {
            if (!_goRendererMap.TryGetValue(go, out Renderer[] renderers))
            {
                _goRendererMap[go] = renderers = _includeChildren ? go.GetComponentsInChildren<Renderer>() : go.GetComponents<Renderer>();
            }
            return renderers;
        }
    }
}
