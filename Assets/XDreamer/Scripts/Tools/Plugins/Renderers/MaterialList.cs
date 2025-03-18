using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// 材质列表
    /// </summary>
    [Name("材质列表")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(CameraManager))]
    public class MaterialList : TableProcessor
    {
        /// <summary>
        /// 材质列表
        /// </summary>
        [Name("材质列表")]
        public List<MaterialTableData_Model> _materials= new List<MaterialTableData_Model>();

        /// <summary>
        /// 材质列表
        /// </summary>
        protected override IEnumerable<TableData_Model> prefabModels => _materials;

        private Dragger dragger = null;

        private InteractableEntity lastHit;

        private RendererRecorder rendererRecorder = new RendererRecorder();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            return base.CanInteract(interactData) && (interactData as TableInteractData).tableData_Model is MaterialTableData_Model;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDragStart(TableInteractData tableInteractData)
        {
            base.OnDragStart(tableInteractData);

            SelectModel(tableInteractData);

            dragger = this.XGetComponentInParentOrGlobal<Dragger>(false);
            if (!dragger)
            {
                Debug.LogErrorFormat("[{0}]未找到有效的主动拖拽器!", CommonFun.ObjectToString(this));
            }

            lastHit = null;
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDrag(TableInteractData tableInteractData)
        {
            base.OnDrag(tableInteractData);

            var model = tableInteractData.tableData_Model as MaterialTableData_Model;

            //即使拖拽器没有在拖拽对象，也通过外部【鼠标输入】产生了【保持】数据
            if (dragger && dragger.holdData != null)
            {
                var entity = dragger.holdData.interactable as InteractableEntity;
                if (lastHit != entity)
                {
                    // 还原上次改变材质的对象
                    rendererRecorder.Recover();
                    rendererRecorder.Clear();

                    lastHit = entity;
                    if (lastHit)
                    {
                        rendererRecorder.Record(lastHit.gameObject, true);

                        foreach (var item in rendererRecorder.records)
                        {
                            item.FillMaterialSize(model.unityObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDragEnd(TableInteractData tableInteractData)
        {
            base.OnDragEnd(tableInteractData);

            dragger = null;
            rendererRecorder.Clear();
        }
    }

    /// <summary>
    /// 材质数据
    /// </summary>
    [Serializable]
    public class MaterialTableData_Model : UnityObjectTableData_Model<Material>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="material"></param>
        public MaterialTableData_Model(Material material) : base(material) { }
    }
}
