using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Interactions.Tools;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 碰撞悬停输入器：利用碰撞体进入、停留和退出事件来模拟悬停事件
    /// </summary>
    [Name("碰撞悬停输入器")]
    //[Tool(InteractionCategory.InteractableVisual, nameof(Interactable), rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public class CollisionHoverInput 
    {
        /// <summary>
        /// 碰撞器类型
        /// </summary>
        public enum EColliderType
        {
            [Name("无")]
            None,

            [Name("球体")]
            Sphere,

            [Name("盒体")]
            Box,

            [Name("胶囊体")]
            Capsule,
        }

        /// <summary>
        /// 碰撞器类型
        /// </summary>
        [Name("碰撞器类型")]
        [EnumPopup]
        public EColliderType _colliderType = EColliderType.Sphere;

        /// <summary>
        /// 球中心
        /// </summary>
        [Name("球中心")]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Sphere)]
        public Transform _sphereCenter;

        /// <summary>
        /// 球半径
        /// </summary>
        [Name("球半径")]
        [Min(0)]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Sphere)]
        public float _sphereRadius = 0.2f;

        /// <summary>
        /// 盒体
        /// </summary>
        [Name("盒体")]
        [Tip("盒体尺寸使用当前对象包围盒大小", "The box size uses the current object bounding box size")]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Box)]
        public Transform _box;

        /// <summary>
        /// 胶囊体端点1
        /// </summary>
        [Name("胶囊体端点1")]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Capsule)]
        public Transform _capsulePoint1;

        /// <summary>
        /// 胶囊体端点2
        /// </summary>
        [Name("胶囊体端点2")]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Capsule)]
        public Transform _capsulePoint2;

        /// <summary>
        /// 胶囊体半径
        /// </summary>
        [Name("胶囊体半径")]
        [Min(0)]
        [HideInSuperInspector(nameof(_colliderType), EValidityCheckType.NotEqual, EColliderType.Capsule)]
        public float _capsuleRadius = 0.2f;

        /// <summary>
        /// 悬停触发最大距离
        /// </summary>
        public float hoverMaxDistance
        {
            get
            {
                switch (_colliderType)
                {
                    case EColliderType.Sphere: return _sphereRadius;
                    case EColliderType.Box: return _boxBounds.size.magnitude;
                    case EColliderType.Capsule: return (_capsulePoint2.position - _capsulePoint1.position).magnitude + _capsuleRadius * 2;
                }
                return 0;
            }
        }

        private Bounds _boxBounds;

        /// <summary>
        /// 启用
        /// </summary>
        public void OnEnable()
        {

            if (_box)
            {
                CommonFun.GetBounds(out _boxBounds, _box);
            }
        }

        /// <summary>
        /// 获取悬停可交互对象
        /// </summary>
        /// <returns></returns>
        public ICollection<InteractableEntity> GetInputInteractables()
        {
            Collider[] colliders = new Collider[0];
            switch (_colliderType)
            {
                case EColliderType.Sphere:
                    {
                        colliders = Physics.OverlapSphere(_sphereCenter.position, _sphereRadius);
                        break;
                    }
                case EColliderType.Box:
                    {
                        colliders = Physics.OverlapBox(_box.position, _boxBounds.extents, _box.rotation);
                        break;
                    }
                case EColliderType.Capsule:
                    {
                        colliders = Physics.OverlapCapsule(_capsulePoint1.position, _capsulePoint2.position, _capsuleRadius);
                        break;
                    }
            }

            var list = new HashSet<InteractableEntity>();
            foreach (var c in colliders)
            {
                // 从碰撞体上查找交互组件，如果没有找到，则从碰撞体的父对象
                var interactables = c.GetComponents<InteractableEntity>();
                if (interactables == null || interactables.Length == 0)
                {
                    interactables = GetBaseInteractableInParentRigidbody(c.transform);
                }
                else
                {
                    foreach (var item in interactables)
                    {
                        if (item.canHover)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        private void OnDrawGizmos()
        {
            var orgColor = Gizmos.color;
            Gizmos.color = Color.red;

            switch (_colliderType)
            {
                case EColliderType.Sphere:
                    {
                        if(_sphereCenter) Gizmos.DrawWireSphere(_sphereCenter.position, _sphereRadius);
                        break;
                    }
                case EColliderType.Box:
                    {
                        if (_box) Gizmos.DrawWireCube(_box.position, _boxBounds.size);
                        break;
                    }
                case EColliderType.Capsule:
                    {
                        if (_capsulePoint1 && _capsulePoint2)
                        {
                            var pos = (_capsulePoint1.position + _capsulePoint2.position) / 2;
                            var height = Vector3.Distance(_capsulePoint1.position, _capsulePoint2.position) + _capsuleRadius * 2;
                            DrawWireCapsule(pos, Quaternion.identity, _capsuleRadius, height, Color.red);
                        }
                        break;
                    }
            }
            Gizmos.color = orgColor;
        }

        private InteractableEntity[] GetBaseInteractableInParentRigidbody(Transform currentTransform)
        {
            var rig = FindParentRigidbody(currentTransform);
            if (rig)
            {
                return rig.GetComponents<InteractableEntity>();
            }
            return null;
        }

        private Rigidbody FindParentRigidbody(Transform currentTransform)
        {
            while (currentTransform!=null)
            {
                var parent = currentTransform.parent;
                if (parent)
                {
                    var rig = parent.GetComponent<Rigidbody>();
                    if (rig)
                    {
                        return rig;
                    }
                }
                currentTransform = parent;
            }
            return null;
        }

        /// <summary>
        /// 绘制胶囊线框
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height, Color color = default(Color))
        {
#if UNITY_EDITOR
            if (color != default(Color)) Handles.color = color;

            Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
#endif
        }
    }
}
