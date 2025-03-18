using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Controllers;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机主控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraMainController), true)]
    [Name("基础相机主控制器检查器")]
    public class BaseCameraMainControllerInspector : BaseCameraMainControllerInspector<BaseCameraMainController>
    {
    }

    /// <summary>
    /// 基础相机主控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraMainControllerInspector<T> : BaseMainControllerInspector<T>
       where T : BaseCameraMainController
    {
        [Name("主相机")]
        [Tip("本参数对应相机实体控制器中主相机", "This parameter corresponds to the main camera in the camera entity controller")]
        [Readonly]
        public bool mainCamera;

        [Name("速度系数")]
        [Tip("速度系数会影响所有使用速度的组件；运算时与将本系数乘到对应组件的速度上；本参数对应相机（移动/旋转）控制器中速度系数；", "The speed coefficient will affect all components using speed; When calculating, multiply the coefficient by the speed of the corresponding component; This parameter corresponds to the speed coefficient in the camera (move / rotate) controller;")]
        public Vector3 speedCoefficient;

        [Name("阻尼系数")]
        [Tip("阻尼系数会影响所有使用阻尼的组件；运算时与将本系数乘到对应组件的阻尼系数上；本参数对应相机（移动/旋转）控制器中阻尼系数；", "The damping coefficient will affect all components using damping; When calculating, multiply the coefficient by the damping coefficient of the corresponding component; This parameter corresponds to the damping coefficient in the camera (move / rotate) controller;")]
        public float dampingCoefficient;

        [Name("使用阻尼")]
        [Tip("所有控制组件的使用阻尼属性统一启用或禁用；所有有效控制组件均使用阻尼时，本参数才为True;如无有效控制组件或是任意一个组件不使用阻尼，则本参数为False；本参数对应相机（移动/旋转）控制器上所有具有使用阻尼属性的组件；", "The damping properties of all control components are enabled or disabled uniformly; This parameter is true only when all effective control components use damping; If there is no effective control component or any component does not use damping, this parameter is false; This parameter corresponds to all components with damping attribute on the camera (move / rotate) controller;")]
        public bool useDamping;

        [Name("主目标")]
        [Tip("本参数对应相机目标控制器中主目标", "This parameter corresponds to the main target in the camera target controller")]
        public UnityEngine.Object mainTarget;

        [Name("主目标距离")]
        [Tip("相机目标控制器与其主目标之间的距离", "The distance between the camera target controller and its main target")]
        public bool mainTargetDistance;

        [Name("距离限定")]
        [Tip("本参数对应相机移动控制器游戏对象上相机位置限定器组件", "This parameter corresponds to the camera position limiter component on the game object of the camera movement controller")]
        public Vector2 rangeLimit = new Vector2(0.001f, 10000);

        private bool moveFoldout = true;
        private bool rotateFoldout = true;

        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);

            switch (serializedProperty.name)
            {
                case nameof(BaseCameraMainController._cameraEntityController):
                    {
                        var controller = targetObject;
                        {
                            EditorGUILayout.ObjectField(TrLabel(nameof(mainCamera)), controller.cameraEntityController.mainCamera, typeof(Camera), true);
                        }
                        break;
                    }
                case nameof(BaseCameraMainController._cameraTransformer):
                    {
                        var controller = targetObject;

                        #region 移动

                        if (moveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(moveFoldout, "移动"))
                        {
                            {
                                EditorGUI.BeginChangeCheck();
                                speedCoefficient = EditorGUILayout.Vector3Field(TrLabel(nameof(speedCoefficient)), controller.cameraTransformer.moveSpeedCoefficient);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    controller.cameraTransformer.moveSpeedCoefficient = speedCoefficient;
                                }
                            }
                            {
                                var cameras = controller.GetComponentsInChildren<ICameraMoveDamping>();
                                EditorGUI.BeginChangeCheck();
                                useDamping = EditorGUILayout.Toggle(TrLabel(nameof(useDamping)), cameras.Length > 0 && cameras.All(c => c.useDamping));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    cameras.Foreach(c => c.useDamping = useDamping);
                                }

                                EditorGUI.BeginChangeCheck();
                                dampingCoefficient = EditorGUILayout.Slider(TrLabel(nameof(dampingCoefficient)), controller.cameraTransformer.moveDampingCoefficient, 0, CameraHelperExtension.MaxDampingCoefficient);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    controller.cameraTransformer.moveDampingCoefficient = dampingCoefficient;
                                }
                            }
                            {
                                if (controller.TryGetPositionLimiter(out CameraPositionLimiter min, out CameraPositionLimiter max))
                                {
                                    Vector2 range = new Vector2(min._sphereLimiter._radius, max._sphereLimiter._radius);
                                    EditorGUI.BeginChangeCheck();
                                    range = UICommonFun.MinMaxSliderLayout(TrLabel(nameof(rangeLimit)), range, rangeLimit);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        min.XModifyProperty(ref min._sphereLimiter._radius, range.x);
                                        max.XModifyProperty(ref max._sphereLimiter._radius, range.y);
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        #endregion

                        #region 旋转

                        if (rotateFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(rotateFoldout, "旋转"))
                        {
                            {
                                EditorGUI.BeginChangeCheck();
                                speedCoefficient = EditorGUILayout.Vector3Field(TrLabel(nameof(speedCoefficient)), controller.cameraTransformer.rotateSpeedCoefficient);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    controller.cameraTransformer.rotateSpeedCoefficient = speedCoefficient;
                                }
                            }
                            {
                                var cameras = controller.GetComponentsInChildren<ICameraRotateDamping>();
                                EditorGUI.BeginChangeCheck();
                                useDamping = EditorGUILayout.Toggle(TrLabel(nameof(useDamping)), cameras.Length > 0 && cameras.All(c => c.useDamping));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    cameras.Foreach(c => c.useDamping = useDamping);
                                }

                                EditorGUI.BeginChangeCheck();
                                dampingCoefficient = EditorGUILayout.Slider(TrLabel(nameof(dampingCoefficient)), controller.cameraTransformer.rotateDampingCoefficient, 0, CameraHelperExtension.MaxDampingCoefficient);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    controller.cameraTransformer.rotateDampingCoefficient = dampingCoefficient;
                                }
                            }
                        }
                        EditorGUILayout.EndFoldoutHeaderGroup();

                        #endregion

                        break;
                    }
                case nameof(BaseCameraMainController._cameraTargetController):
                    {
                        var controller = targetObject;
                        var targetController = controller.cameraTargetController;
                        var mainTargetTransform = targetController.mainTarget;
                        var bak = GUI.backgroundColor;
                        if (!mainTargetTransform) GUI.backgroundColor = Color.red;
                        EditorGUI.BeginChangeCheck();
                        mainTarget = EditorGUILayout.ObjectField(TrLabel(nameof(mainTarget)), mainTargetTransform, typeof(Transform), true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            targetController.mainTarget = mainTarget as Transform;
                        }
                        GUI.backgroundColor = bak;
                        EditorGUI.BeginChangeCheck();
                        var targetPosition = targetController.targetPosition;
                        var dir = controller.cameraTransformer.position - targetPosition;
                        var distance = dir.magnitude;
                        if (controller.TryGetPositionLimiter(out CameraPositionLimiter min, out CameraPositionLimiter max))
                        {
                            distance = EditorGUILayout.Slider(TrLabel(nameof(mainTargetDistance)), distance, min._sphereLimiter._radius, max._sphereLimiter._radius);
                        }
                        else
                        {
                            distance = EditorGUILayout.FloatField(TrLabel(nameof(mainTargetDistance)), distance);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            controller.cameraTransformer.position = dir.normalized * distance + targetPosition;
                        }
                        break;
                    }
            }
        }

#if UNITY_2018_1_OR_NEWER //相机预览

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestroy()
        {
            if (_previewCamera != null)
            {
                DestroyImmediate(this._previewCamera.gameObject, true);
                _previewCamera = null;
            }
        }

        /// <summary>
        /// 绘制场景GUI
        /// </summary>
        public void OnSceneGUI()
        {
            Camera camera = targetObject.cameraEntityController.mainCamera;
            if (!camera) return;
            if (CameraEditorUtils.IsViewportRectValidToRender(camera.rect))
            {
                if (_delegate == null)
                {
                    _delegate = GetType().GetMethod(nameof(OnOverlayGUI)).CreateDelegate(SceneViewOverlay_LinkType.WindowFunction_Type, this);
                }

#if UNITY_2019_1_OR_NEWER
                Vector2 mainPlayModeViewTargetSize = PlayModeView_LinkType.GetMainPlayModeViewTargetSize();
                if (s_PreviousMainPlayModeViewTargetSize != mainPlayModeViewTargetSize)
                {
                    base.Repaint();
                    s_PreviousMainPlayModeViewTargetSize = mainPlayModeViewTargetSize;
                }
                SceneViewOverlay_LinkType.Window(EditorGUIUtility.TrTextContent("Camera Preview"), _delegate, -100, camera, 1, null);
#else
                SceneViewOverlay_LinkType.Window(EditorGUIUtility.TrTextContent("Camera Preview"), _delegate, -100, camera, 1);
#endif
                CameraEditorUtils.HandleFrustum(camera, this.referenceTargetIndex);
            }
        }

        private Camera _previewCamera;

        private Camera previewCamera
        {
            get
            {
                if (_previewCamera == null)
                {
                    this._previewCamera = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave, typeof(Camera), typeof(Skybox)).GetComponent<Camera>();
                }
                this._previewCamera.enabled = false;
                return this._previewCamera;
            }
        }

        private int _referenceTargetIndex = 0;

        private int referenceTargetIndex
        {
            get
            {
                return Mathf.Clamp(this._referenceTargetIndex, 0, targets.Length - 1);
            }
            set
            {
                this._referenceTargetIndex = (Math.Abs(value * targets.Length) + value) % targets.Length;
            }
        }

        private RenderTexture previewTexture;

        private Delegate _delegate;

        SceneView_LinkType sceneView_LinkType = new SceneView_LinkType();

        /// <summary>
        /// 绘制覆盖的GUI
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sceneView"></param>
        public void OnOverlayGUI(UnityEngine.Object target, SceneView sceneView)
        {
            var camera = target as Camera;
            if (!camera) return;
            {
                sceneView_LinkType.SetObject(sceneView);
                var customScene = sceneView_LinkType.customScene;
                StageHandle stageHandle = StageUtility.GetStageHandle(camera.gameObject);
                StageHandle stageHandle2 = StageUtility.GetStageHandle(customScene);
                if (!(stageHandle != stageHandle2))
                {
#if UNITY_2019_1_OR_NEWER
                    Vector2 vector = camera.targetTexture ? new Vector2(camera.targetTexture.width, camera.targetTexture.height) : PlayModeView_LinkType.GetMainPlayModeViewTargetSize();
#else
                    Vector2 vector = camera.targetTexture ? new Vector2(camera.targetTexture.width, camera.targetTexture.height) : GameView_LinkType.GetMainGameViewTargetSize();
#endif
                    var position = sceneView.position;
                    if (vector.x < 0f)
                    {
                        vector.x = position.width;
                        vector.y = position.height;
                    }
                    Rect rect = camera.rect;
                    rect.xMin = Math.Max(rect.xMin, 0f);
                    rect.yMin = Math.Max(rect.yMin, 0f);
                    rect.xMax = Math.Min(rect.xMax, 1f);
                    rect.yMax = Math.Min(rect.yMax, 1f);
                    vector.x *= Mathf.Max(rect.width, 0f);
                    vector.y *= Mathf.Max(rect.height, 0f);
                    if (!(vector.x < 1f) && !(vector.y < 1f))
                    {
                        float num = vector.x / vector.y;
                        vector.y = 0.2f * position.height;
                        vector.x = vector.y * num;
                        if (vector.y > position.height * 0.5f)
                        {
                            vector.y = position.height * 0.5f;
                            vector.x = vector.y * num;
                        }
                        if (vector.x > position.width * 0.5f)
                        {
                            vector.x = position.width * 0.5f;
                            vector.y = vector.x / num;
                        }
                        Rect rect2 = GUILayoutUtility.GetRect(vector.x, vector.y);
                        if (Event.current.type == EventType.Repaint)
                        {
                            this.previewCamera.CopyFrom(camera);
                            this.previewCamera.scene = customScene;
                            Skybox component = ((Component)this.previewCamera).GetComponent<Skybox>();
                            if ((bool)component)
                            {
                                Skybox component2 = ((Component)camera).GetComponent<Skybox>();
                                if ((bool)component2 && component2.enabled)
                                {
                                    component.enabled = true;
                                    component.material = component2.material;
                                }
                                else
                                {
                                    component.enabled = false;
                                }
                            }

#if UNITY_2019_1_OR_NEWER
                            RenderTexture previewTextureWithSize = this.GetPreviewTextureWithSizeAndAA((int)rect2.width, (int)rect2.height);
#else
                            RenderTexture previewTextureWithSize = this.GetPreviewTextureWithSize((int)rect2.width, (int)rect2.height);
                            previewTextureWithSize.antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);
#endif
                            this.previewCamera.targetTexture = previewTextureWithSize;
                            this.previewCamera.pixelRect = new Rect(0f, 0f, rect2.width, rect2.height);
                            Handles_LinkType.EmitGUIGeometryForCamera(camera, this.previewCamera);
                            if (camera.usePhysicalProperties)
                            {
                                RenderTexture active = RenderTexture.active;
                                RenderTexture.active = previewTextureWithSize;
                                GL.Clear(false, true, Color.clear);
                                RenderTexture.active = active;
                            }
                            this.previewCamera.Render();
                            Graphics.DrawTexture(rect2, previewTextureWithSize, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility_LinkType.GUITextureBlit2SRGBMaterial);
                        }
                    }
                }
            }
        }


#if UNITY_2019_1_OR_NEWER

        private static Vector2 s_PreviousMainPlayModeViewTargetSize;

        private RenderTexture GetPreviewTextureWithSizeAndAA(int width, int height)
        {
            int num = Mathf.Max(1, QualitySettings.antiAliasing);
            if (previewTexture == null || this.previewTexture.width != width || this.previewTexture.height != height || this.previewTexture.antiAliasing != num)
            {
                this.previewTexture = new RenderTexture(width, height, 24, SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR));
                this.previewTexture.antiAliasing = num;
            }
            return this.previewTexture;
        }
#else
        private RenderTexture GetPreviewTextureWithSize(int width, int height)
        {
            if (this.previewTexture == null || this.previewTexture.width != width || this.previewTexture.height != height)
            {
                this.previewTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            }
            return this.previewTexture;
        }
#endif



#endif
    }
}
