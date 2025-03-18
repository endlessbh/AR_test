using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    [Name("网格线")]
    public abstract class MeshLine
    {
        /// <summary>
        /// 转换
        /// </summary>
        [Name("转换")]
        [SerializeField]
        protected Transform _transform;

        /// <summary>
        /// 转换
        /// </summary>
        public Transform transform { get => _transform; set => Update(value); }

        /// <summary>
        /// 渲染器
        /// </summary>
        [Name("渲染器")]
        [Readonly]
        public Renderer renderer;

        /// <summary>
        /// 宽度
        /// </summary>
        [Name("宽度")]
        [Range(0.001f, 1)]
        public float width = 0.08f;

        /// <summary>
        /// 更新颜色
        /// </summary>
        [Name("更新颜色")]
        public bool updateColor = true;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [Tip("在编辑器的非运行态时,不支持渲染器材质的颜色调整", "Color adjustment of renderer materials is not supported when the editor is not running")]
        [HideInSuperInspector(nameof(updateColor), EValidityCheckType.Equal, false)]
        public Color color = Color.white;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="parentTransform"></param>
        /// <param name="name"></param>
        public abstract void Create(MB mb, Transform parentTransform, string name);

        /// <summary>
        /// 更新转换
        /// </summary>
        /// <param name="transform"></param>
        public void Update(Transform transform)
        {
            this._transform = transform;
            renderer = transform ? transform.GetComponent<Renderer>() : null;
        }

        /// <summary>
        /// 更新线的起点终点
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Update(Vector3 from, Vector3 to)
        {
            if (!_transform) return;
            _transform.position = (from + to) / 2;
            var dir = to - from;
            if (dir == Vector3.zero)
            {
                _transform.localScale = new Vector3(width, width, 0);
            }
            else
            {
                _transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                _transform.localScale = new Vector3(width, width, dir.magnitude);
            }
            if (updateColor) UpdateColor();
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        /// <param name="color"></param>
        public void Update(Color color)
        {
            this.color = color;
            UpdateColor();
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        public void UpdateColor()
        {
            if (renderer)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return;
                }
#endif
                renderer.material.color = color;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="mb"></param>
        public void OnEnable(MB mb)
        {
            if (_transform) _transform.gameObject.SetActive(true);
            if (updateColor) UpdateColor();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="mb"></param>
        public void OnDisable(MB mb)
        {
            if (_transform) _transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// 可见与否
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if(renderer) renderer.enabled = isVisible;
        }
    }

    [Serializable]
    [Name("立方线")]
    public class CubeLine : MeshLine
    {
        public override void Create(MB mb, Transform parentTransform, string name)
        {
            if (!_transform)
            {
                _transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                _transform.name = string.IsNullOrEmpty(name) ? nameof(MeshLine) : name;
                renderer = _transform.GetComponent<Renderer>();

                if (parentTransform) _transform.SetParent(parentTransform);

                UpdateColor();
            }
        }
    }
}
