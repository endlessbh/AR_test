using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图项 : 运行时注入导航图中，生成对应的项
    /// </summary>
    [Name("导航图项")]
    [Tip("运行时注入导航图中，生成对应的项", "The runtime is injected into the navigation diagram to generate corresponding items")]
    [XCSJ.Attributes.Icon(EIcon.MiniMap)]
    [Tool("导航图", nameof(MiniMap), rootType = typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public class MiniMapItem : InteractableVirtual
    {
        [Name("导航图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public MiniMap _miniMap = null;

        /// <summary>
        /// 查找规则
        /// </summary>
        [Name("自动查找导航图规则")]
        [Tip("导航图对象无效时查找规则", "Find rule when navigation map object is invalid")]
        [EnumPopup]
        public EFindRule _findRule = EFindRule.Global;

        /// <summary>
        /// 查找规则
        /// </summary>
        [Name("查找规则")]
        public enum EFindRule
        {
            [Name("无")]
            None,

            [Name("全局")]
            Global,

            [Name("全局第一个启用")]
            GlobalFirstEnable,
        }

        /// <summary>
        /// 绑定UI
        /// </summary>
        [Name("图标项数据")]
        public MiniMapItemData miniMapItemData = new MiniMapItemData();

        protected void Reset()
        {
            FindMiniMap();
        }

        protected override void Awake()
        {
            base.Awake();

            FindMiniMap();
        }

        private void FindMiniMap()
        {
            if (_miniMap) return;

            switch (_findRule)
            {
                case EFindRule.Global:
                    {
                        _miniMap = FindObjectOfType<MiniMap>();
                        break;
                    }
                case EFindRule.GlobalFirstEnable:
                    {
                        _miniMap = FindObjectsOfType<MiniMap>().Find(m => m.enabled);
                        break;
                    }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_miniMap)
            {
                _miniMap.CreateItem(miniMapItemData.ui, transform);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_miniMap)
            {
                _miniMap.DestroyItem(transform);
            }
        }
    }

    /// <summary>
    /// 小地图数据
    /// </summary>
    [Serializable]
    [Name("小地图数据")]
    public class MiniMapItemData
    {
        /// <summary>
        /// UI
        /// </summary>
        [Name("UI")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform ui = null;

        /// <summary>
        /// 移动偏移量 ：计算导航条UI位置时，使用追踪对象的位置+当前值作为最终的计算值
        /// </summary>
        [Name("移动偏移量")]
        [Tip("计算导航条UI位置时，使用追踪对象的位置+当前值作为最终的计算值", "When calculating the UI position of the navigation bar, the position of the tracking object + the current value is used as the final calculated value")]
        public Vector3 positionOffset = Vector3.zero;

        /// <summary>
        /// 追踪旋转
        /// </summary>
        [Name("追踪旋转")]
        [Tip("UI图标是否随着追踪对象旋转", "Does the UI icon rotate with the tracking object")]
        public bool followRotation = false;

        /// <summary>
        /// 旋转偏移量
        /// </summary>
        [Name("旋转偏移量")]
        [Tip("计算导航条UI旋转时，使用追踪对象的旋转量+当前值作为最终的计算值", "When calculating the rotation of the navigation bar UI, the rotation amount of the tracking object + the current value is used as the final calculation value")]
        [HideInSuperInspector(nameof(followRotation), EValidityCheckType.Equal, false)]
        public float rotationYOffset = 0f;

        public bool valid => ui;

        public MiniMapItemData() { }

        public MiniMapItemData(RectTransform ui, Vector3 positionOffset, bool followRotation = false, float rotationYOffset = 0)
        {
            this.ui = ui;
            this.positionOffset = positionOffset;
            this.followRotation = followRotation;
            this.rotationYOffset = rotationYOffset;
        }

        /// <summary>
        /// 更新UI项旋转 ： 设置旋转, 将追踪的3D游戏对象的Y负角度设置为UI的Z角度
        /// </summary>
        /// <param name="transform"></param>
        public void UpdateRotation(Transform transform)
        {
            if (followRotation && ui)
            {
                var angle = ui.eulerAngles;
                angle.z = -(transform.eulerAngles.y + rotationYOffset);
                ui.eulerAngles = angle;
            }
        }
    }
}
