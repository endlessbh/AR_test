using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginsCameras.UI
{
    /// <summary>
    /// 相机列表
    /// </summary>
    [Name("相机列表")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(CameraManager))]
    public class CameraList : TableProcessor
    {
        #region 属性

        /// <summary>
        /// 相机切换时间
        /// </summary>
        [Name("相机切换时间")]
        [Min(0)]
        public float _duration = 1f;

        /// <summary>
        /// 相机视图尺寸
        /// </summary>
        [Name("相机视图尺寸")]
        public Vector2Int _viewSize = new Vector2Int(256, 256);

        /// <summary>
        /// 相机表数据规则
        /// </summary>
        [Name("相机表数据规则")]
        [EnumPopup]
        public ECameraTableDataRule _cameraTableDataRule = ECameraTableDataRule.All;

        /// <summary>
        /// 相机视图项数据查找规则
        /// </summary>
        public enum ECameraTableDataRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 全部
            /// </summary>
            [Name("全部")]
            All,

            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom,

            /// <summary>
            /// 除自定义外全部
            /// </summary>
            [Name("除自定义外全部")]
            AllWithoutCustom,
        }

        /// <summary>
        /// 自定义相机表数据
        /// </summary>
        [Name("自定义相机表数据")]
        [HideInSuperInspector(nameof(_cameraTableDataRule), EValidityCheckType.NotEqual | EValidityCheckType.And, ECameraTableDataRule.Custom, nameof(_cameraTableDataRule), EValidityCheckType.NotEqual, ECameraTableDataRule.AllWithoutCustom)]
        public List<CameraTableData_Model> _customCameraTableDatas = new List<CameraTableData_Model>();

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CameraControllerEvent.onEndSwitch += RenderCameraView;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            CameraControllerEvent.onEndSwitch -= RenderCameraView;
        }

        #endregion

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            return base.CanInteract(interactData) && (interactData as TableInteractData).tableData_Model is IComponentGetter;
        }

        /// <summary>
        /// 预加载数据
        /// </summary>
        protected override IEnumerable<TableData_Model> prefabModels
        {
            get 
            {  
                var list = new List<CameraTableData_Model>();
                switch (_cameraTableDataRule)
                {
                    case ECameraTableDataRule.All:
                        {
                            foreach (var c in ComponentCache.GetComponents<BaseCameraMainController>(true))
                            {
                                list.Add(new CameraTableData_Model(c, c.ownerGameObject ? c.ownerGameObject.name : "", ToTexture2D(c)));
                            }
                            break;
                        }
                    case ECameraTableDataRule.Custom:
                        {
                            foreach (var model in _customCameraTableDatas)
                            {
                                if (model.unityObject)
                                {
                                    if(!model.texture2D)
                                    {
                                        model.texture2D = ToTexture2D(model.unityObject);
                                    }
                                    list.Add(model);
                                }
                            }
                            break;
                        }
                    case ECameraTableDataRule.AllWithoutCustom:
                        {
                            foreach (var c in ComponentCache.GetComponents<BaseCameraMainController>(true))
                            {
                                if (!_customCameraTableDatas.Exists(item => item.unityObject == c))
                                {
                                    list.Add(new CameraTableData_Model(c, c.ownerGameObject ? c.ownerGameObject.name : "", ToTexture2D(c)));
                                }
                            }
                            break;
                        }
                }
                return list;
            }
        }

        /// <summary>
        /// 视图项数据点击
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnClick(TableInteractData tableInteractData)
        {
            if ((tableInteractData.tableData_Model as IComponentGetter).component is BaseCameraMainController camController && camController)
            {
                if (CameraManager.instance)
                {
                    CameraManager.instance.GetProvider().SwitchCameraController(camController, _duration, null, true);
                }
            }
        }

        private void RenderCameraView(BaseCameraMainController from, BaseCameraMainController to)
        {
            // 重新渲染相机图片
            foreach (var item in table.models)
            {
                if (item is CameraTableData_Model cameraData)
                {
                    cameraData.texture2D = ToTexture2D(cameraData.baseCameraMainController);
                }
            }
            table.TryReflashTableData();
        }

        private Texture2D ToTexture2D(BaseCameraMainController cameraController) => cameraController.Render(_viewSize).ToTexture2D();
    }

    /// <summary>
    /// 相机表数据
    /// </summary>
    [Name("相机表数据")]
    [Serializable]
    public class CameraTableData_Model : ComponentTableData_Model<BaseCameraMainController>
    {
        /// <summary>
        /// 基础相机主控制器
        /// </summary>
        public BaseCameraMainController baseCameraMainController => unityObject;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public CameraTableData_Model(BaseCameraMainController component) : base(component) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="texture2D"></param>
        public CameraTableData_Model(BaseCameraMainController component, string title, Texture2D texture2D) : base(component, title, texture2D) { }

        /// <summary>
        /// 标题:
        /// 1、首先获取输入的标题。
        /// 2、1为空时，使用相机控制器拥有者所在游戏对象的名称
        /// 3、2为空时，使用相机控制器所在游戏对象的名称
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(base.title) && baseCameraMainController && baseCameraMainController.ownerGameObject)
                {
                    base.title = baseCameraMainController.ownerGameObject.name;
                }
                return base.title;
            }
            set => base.title = value;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public override bool selected => unityObject ? unityObject == CameraManager.instance.GetCurrentCameraController() : base.selected;
    }
}
