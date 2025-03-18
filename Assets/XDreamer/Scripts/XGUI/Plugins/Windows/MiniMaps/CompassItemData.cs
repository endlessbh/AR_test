using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 指南针项数据 
    /// </summary>
    [Name("指南针项数据")]
    [Tip("通过设置地图中的三维参考对象来指示方向", "Indicates the direction by setting up 3D reference objects in the map")]
    [XCSJ.Attributes.Icon(EIcon.MiniMap)]
    [Tool("导航图", nameof(MiniMap), rootType = typeof(XGUIManager))]
    [RequireComponent(typeof(RectTransform))]
    public class CompassItemData : View
    {
        /// <summary>
        /// 导航图
        /// </summary>
        [Name("导航图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MiniMap _miniMap;

        /// <summary>
        /// 导航图
        /// </summary>
        public MiniMap miniMap => this.XGetComponentInParentOrGlobal(ref _miniMap);

        /// <summary>
        /// 关联UI
        /// </summary>
        [Name("关联UI")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform _ui = null;

        #region 位置设定

        /// <summary>
        /// 位于导航图边沿
        /// </summary>
        [Group("位置设定", textEN = "Position Settings")]
        [Name("位于导航图边沿")]
        public bool _onMapEdge = true;

        /// <summary>
        /// 导航图类型
        /// </summary>
        [Name("导航图类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_onMapEdge), EValidityCheckType.False)]
        public EMiniMapType _minimapType = EMiniMapType.Circle;

        /// <summary>
        /// 导航图类型
        /// </summary>
        [Name("导航图类型")]
        public enum EMiniMapType
        {
            [Name("圆形")]
            Circle,

            [Name("矩形")]
            Rect,
        }

        #endregion

        #region 旋转设定

        /// <summary>
        /// 旋转设定
        /// </summary>
        [Group("旋转设定", textEN = "Rotation Settings")]
        [Name("自转")]
        public bool _selfRotation = false;

        /// <summary>
        /// UI Z轴旋转偏移量
        /// </summary>
        [Name("旋转偏移量")]
        [HideInSuperInspector(nameof(_selfRotation), EValidityCheckType.False)]
        [LimitRange(0, 360)]
        public float _rotationOffset = 0f;

        #endregion

        #region Unity生命周期函数

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (miniMap) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        private void Awake()
        {
            if (!_ui)
            {
                _ui = GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        [XCSJ.Languages.LanguageTuple("Missing navigation map component", "缺少导航图组件")]
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!miniMap)
            {
                Debug.LogError("Missing navigation map component".Tr());
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (!_ui || !miniMap.uiRoot) return;

            var uiSize = miniMap.uiRoot.sizeDelta;

            if (_selfRotation)
            {
                _ui.rotation = Quaternion.Euler(0, 0, miniMap.miniMapCamera.linkCamera.transform.eulerAngles.y + _rotationOffset + miniMap.GetDirectionYAngle());
            }

            // 更新位置
            if (_onMapEdge)
            {
                var dir = Quaternion.Euler(0, -miniMap.miniMapCamera.linkCamera.transform.eulerAngles.y, 0) * miniMap.GetDirection();

                var point = new Vector2(dir.x, dir.z);
                point.Normalize();

                switch (_minimapType)
                {
                    case EMiniMapType.Circle:
                        {
                            point.Scale(uiSize);
                            _ui.anchoredPosition = point / 2;
                            break;
                        }
                    case EMiniMapType.Rect:
                        {
                            if (point.x == 0 || point.y == 0)
                            {
                                point.Scale(uiSize);
                            }
                            else
                            {
                                // 比较斜率, 当前向量斜率大于地图矩形斜率
                                var k = point.y / point.x;
                                if (Mathf.Abs(k) > Mathf.Abs(uiSize.y / uiSize.x))
                                {
                                    point.y = point.y > 0 ? uiSize.y : -uiSize.y;
                                    point.x = point.y / k;
                                }
                                else
                                {
                                    point.x = point.x > 0 ? uiSize.x : -uiSize.x;
                                    point.y = point.x * k;
                                }
                            }
                            _ui.anchoredPosition = point / 2;
                            break;
                        }
                }
            }
        } 

        #endregion
    }
}
