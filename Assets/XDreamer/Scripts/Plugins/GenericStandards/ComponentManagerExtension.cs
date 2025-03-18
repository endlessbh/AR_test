using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards
{
    /// <summary>
    /// 组件管理器扩展：用于处理运行时因Unity代码剥离导致无法做类型反射查找的问题；
    /// </summary>
    public class ComponentManagerExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.IPhonePlayer:
                    {
                        ComponentManager.Init(SubclassOfComponent);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Unity内置组件的所有子类
        /// </summary>
        public readonly static HashSet<Type> SubclassOfComponent = new HashSet<Type>
        {            
            typeof(UnityEngine.AI.NavMeshAgent),
            typeof(UnityEngine.AI.NavMeshObstacle),
            typeof(UnityEngine.AI.OffMeshLink),
            typeof(UnityEngine.AnchoredJoint2D),
            typeof(UnityEngine.Animation),
            typeof(UnityEngine.Animations.AimConstraint),
            typeof(UnityEngine.Animations.LookAtConstraint),
            typeof(UnityEngine.Animations.ParentConstraint),
            typeof(UnityEngine.Animations.PositionConstraint),
            typeof(UnityEngine.Animations.RotationConstraint),
            typeof(UnityEngine.Animations.ScaleConstraint),
            typeof(UnityEngine.Animator),
            typeof(UnityEngine.AreaEffector2D),
#if UNITY_2020_1_OR_NEWER
            typeof(UnityEngine.ArticulationBody),
#endif
            typeof(UnityEngine.AudioBehaviour),
            typeof(UnityEngine.AudioChorusFilter),
            typeof(UnityEngine.AudioDistortionFilter),
            typeof(UnityEngine.AudioEchoFilter),
            typeof(UnityEngine.AudioHighPassFilter),
            typeof(UnityEngine.AudioListener),
            typeof(UnityEngine.AudioLowPassFilter),
            typeof(UnityEngine.AudioReverbFilter),
            typeof(UnityEngine.AudioReverbZone),
            typeof(UnityEngine.AudioSource),
            typeof(UnityEngine.Behaviour),
            typeof(UnityEngine.BillboardRenderer),
            typeof(UnityEngine.BoxCollider),
            typeof(UnityEngine.BoxCollider2D),
            typeof(UnityEngine.BuoyancyEffector2D),
            typeof(UnityEngine.Camera),
            typeof(UnityEngine.Canvas),
            typeof(UnityEngine.CanvasGroup),
            typeof(UnityEngine.CanvasRenderer),
            typeof(UnityEngine.CapsuleCollider),
            typeof(UnityEngine.CapsuleCollider2D),
            typeof(UnityEngine.CharacterController),
            typeof(UnityEngine.CharacterJoint),
            typeof(UnityEngine.CircleCollider2D),
            typeof(UnityEngine.Cloth),
            typeof(UnityEngine.Collider),
            typeof(UnityEngine.Collider2D),
            typeof(UnityEngine.Component),
            typeof(UnityEngine.CompositeCollider2D),
            typeof(UnityEngine.ConfigurableJoint),
            typeof(UnityEngine.ConstantForce),
            typeof(UnityEngine.ConstantForce2D),
            typeof(UnityEngine.DistanceJoint2D),
            typeof(UnityEngine.EdgeCollider2D),
            typeof(UnityEngine.Effector2D),
            typeof(UnityEngine.EventSystems.BaseInput),
            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.EventSystems.EventTrigger),
            typeof(UnityEngine.EventSystems.Physics2DRaycaster),
            typeof(UnityEngine.EventSystems.PhysicsRaycaster),
            typeof(UnityEngine.EventSystems.StandaloneInputModule),
#if !UNITY_2019_3_OR_NEWER
            typeof(UnityEngine.Experimental.U2D.SpriteShapeRenderer),
            typeof(UnityEngine.Experimental.VFX.VisualEffect),
#endif
            typeof(UnityEngine.FixedJoint),
            typeof(UnityEngine.FixedJoint2D),
            typeof(UnityEngine.FlareLayer),
            typeof(UnityEngine.FrictionJoint2D),
            typeof(UnityEngine.Grid),
            typeof(UnityEngine.GridLayout),
#if UNITY_2019_3_OR_NEWER
#else
            typeof(UnityEngine.GUIElement),
#endif
            typeof(UnityEngine.HingeJoint),
            typeof(UnityEngine.HingeJoint2D),
            typeof(UnityEngine.Joint),
            typeof(UnityEngine.Joint2D),
            typeof(UnityEngine.LensFlare),
            typeof(UnityEngine.Light),
            typeof(UnityEngine.LightProbeGroup),
            typeof(UnityEngine.LightProbeProxyVolume),
            typeof(UnityEngine.LineRenderer),
            typeof(UnityEngine.LODGroup),
            typeof(UnityEngine.MeshCollider),
            typeof(UnityEngine.MeshFilter),
            typeof(UnityEngine.MeshRenderer),
            typeof(UnityEngine.MonoBehaviour),
            typeof(UnityEngine.OcclusionArea),
            typeof(UnityEngine.OcclusionPortal),
            typeof(UnityEngine.ParticleSystem),
            typeof(UnityEngine.ParticleSystemForceField),
            typeof(UnityEngine.ParticleSystemRenderer),
            typeof(UnityEngine.PhysicsUpdateBehaviour2D),
            typeof(UnityEngine.PlatformEffector2D),
            typeof(UnityEngine.Playables.PlayableDirector),
            typeof(UnityEngine.PointEffector2D),
            typeof(UnityEngine.PolygonCollider2D),
            typeof(UnityEngine.Projector),
            typeof(UnityEngine.RectTransform),
            typeof(UnityEngine.ReflectionProbe),
            typeof(UnityEngine.RelativeJoint2D),
            typeof(UnityEngine.Renderer),
            typeof(UnityEngine.Rendering.SortingGroup),
            typeof(UnityEngine.Rigidbody),
            typeof(UnityEngine.Rigidbody2D),
            typeof(UnityEngine.SkinnedMeshRenderer),
            typeof(UnityEngine.Skybox),
            typeof(UnityEngine.SliderJoint2D),
            typeof(UnityEngine.SphereCollider),
            typeof(UnityEngine.SpringJoint),
            typeof(UnityEngine.SpringJoint2D),
            typeof(UnityEngine.SpriteMask),
            typeof(UnityEngine.SpriteRenderer),
            typeof(UnityEngine.StreamingController),
            typeof(UnityEngine.SurfaceEffector2D),
            typeof(UnityEngine.TargetJoint2D),
            typeof(UnityEngine.Terrain),
            typeof(UnityEngine.TerrainCollider),
            typeof(UnityEngine.TextMesh),
            typeof(UnityEngine.Tilemaps.Tilemap),
            typeof(UnityEngine.Tilemaps.TilemapCollider2D),
            typeof(UnityEngine.Tilemaps.TilemapRenderer),
#if UNITY_2020_1_OR_NEWER
            typeof(UnityEngine.Timeline.SignalReceiver),
#endif
            typeof(UnityEngine.TrailRenderer),
            typeof(UnityEngine.Transform),
            typeof(UnityEngine.Tree),
#if UNITY_2019_3_OR_NEWER
            typeof(UnityEngine.U2D.SpriteShapeRenderer),
#endif
            typeof(UnityEngine.UI.AspectRatioFitter),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.CanvasScaler),
            typeof(UnityEngine.UI.ContentSizeFitter),
            typeof(UnityEngine.UI.Dropdown),
            typeof(UnityEngine.UI.GraphicRaycaster),
            typeof(UnityEngine.UI.GridLayoutGroup),
            typeof(UnityEngine.UI.HorizontalLayoutGroup),
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.InputField),
            typeof(UnityEngine.UI.LayoutElement),
            typeof(UnityEngine.UI.Mask),
            typeof(UnityEngine.UI.Outline),
            typeof(UnityEngine.UI.PositionAsUV1),
            typeof(UnityEngine.UI.RawImage),
            typeof(UnityEngine.UI.RectMask2D),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.ScrollRect),
            typeof(UnityEngine.UI.Selectable),
            typeof(UnityEngine.UI.Shadow),
            typeof(UnityEngine.UI.Slider),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.Toggle),
            typeof(UnityEngine.UI.ToggleGroup),
            typeof(UnityEngine.UI.VerticalLayoutGroup),
#if UNITY_2019_3_OR_NEWER
            typeof(UnityEngine.VFX.VisualEffect),
#endif
            typeof(UnityEngine.Video.VideoPlayer),
            typeof(UnityEngine.WheelCollider),
            typeof(UnityEngine.WheelJoint2D),
            typeof(UnityEngine.WindZone),
#if UNITY_EDITOR || UNITY_WSA
#if !UNITY_2020_2_OR_NEWER
            typeof(UnityEngine.XR.WSA.WorldAnchor)
#endif
#endif
        };
    }
}

