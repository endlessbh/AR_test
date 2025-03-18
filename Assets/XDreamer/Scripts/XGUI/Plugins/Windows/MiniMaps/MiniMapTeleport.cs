using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图传送
    /// </summary>
    [Name("导航图传送")]
    [RequireComponent(typeof(MiniMap))]
    [DisallowMultipleComponent]
    [Tool("导航图", nameof(MiniMap), rootType = typeof(XGUIManager))]
    [XCSJ.Attributes.Icon(EIcon.MiniMap)]
    public class MiniMapTeleport : View
    {
        /// <summary>
        /// 传送规则
        /// </summary>
        [Name("传送规则")]
        [Tip("在导航图中点击，可将玩家移动到对应的三维空间上的地点的规则", "Click in the navigation map to move the player to the place in the corresponding three-dimensional space")]
        [EnumPopup]
        public ETeleportRule _teleportRule = ETeleportRule.NeedRayHit;

        /// <summary>
        /// 传送规则
        /// </summary>
        [Name("传送规则")]
        public enum ETeleportRule
        {
            [Name("无条件传送")]
            None,

            [Name("传送需射线撞击")]
            [Tip("从地图上方垂直发射一条射线，与地面物体有碰撞后才能传送", "从地图上方垂直发射一条射线，与地面物体有碰撞后才能传送")]
            NeedRayHit,
        }

        [Name("偏移高度")]
        [Tip("传送到目标点的偏移高度", "Offset height transmitted to target point")]
        public float _heightOffset = 0.1f;

        private MiniMap miniMap = null;

        private bool _onUIRootWhenDown = false;

        private CameraManagerProvider cameraManagerProvider;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            miniMap = GetComponent<MiniMap>();

            if (CameraManager.instance)
            {
                cameraManagerProvider = CameraManager.instance.cameraManagerProvider as CameraManagerProvider;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (miniMap.player && miniMap.miniMapCamera && miniMap.miniMapCamera.linkCamera)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _onUIRootWhenDown = miniMap.TryGetClickPointInMap(Input.mousePosition, out _);
                }
                if (Input.GetMouseButtonUp(0) && _onUIRootWhenDown
                    && miniMap.TryGetClickPointInMap(Input.mousePosition, out Vector2 uiLocalPointNormalized))
                {
                    HandleClickPoint(uiLocalPointNormalized, _teleportRule == ETeleportRule.NeedRayHit);
                }
            }
        }

        private void HandleClickPoint(Vector2 clickUIPoint, bool needRayHit)
        {
            var pointWorld = miniMap.miniMapCamera.linkCamera.ViewportToWorldPoint(clickUIPoint);

            var hit = Physics.Raycast(new Ray(pointWorld, Vector3.down), out RaycastHit hitInfo);
            if (needRayHit && !hit)
            {
                return;
            }
            var clickPoint = hit ? hitInfo.point : pointWorld;

            // 新版相机控制器
            if (miniMap.player == cameraManagerProvider.currentCameraController.cameraTransformer)
            {
                cameraManagerProvider.currentCameraController.cameraTransformer.SetPosition(GetMovePosition(cameraManagerProvider.currentCameraController.transform, clickPoint), Space.World);
            }
            else
            {
                miniMap.player.position = GetMovePosition(miniMap.player.transform, clickPoint);
            }
        }

        private Vector3 GetMovePosition(Transform moveTransform, Vector3 clickPoint)
        {
            float heightOffset = _heightOffset;
            if (CommonFun.GetBounds(out Bounds bounds, moveTransform))
            {
                heightOffset += bounds.size.y / 2;
            }
            if (Physics.Raycast(new Ray(moveTransform.position, Vector3.down), out RaycastHit hitInfo))
            {
                heightOffset += moveTransform.position.y - hitInfo.point.y;
            }

            return new Vector3(clickPoint.x, clickPoint.y + heightOffset, clickPoint.z);
        }
    }
}
