using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTimelines.Tools;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginTimelines.UI
{
    /// <summary>
    /// 播放内容列表
    /// </summary>
    [Name("播放内容列表")]
    [RequireManager(typeof(TimelineManager))]
    public class PlayableContentList : TableProcessor
    {
        /// <summary>
        /// 播放器控制器        
        /// </summary>
        [Name("播放器控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlayerController _playerController;

        /// <summary>
        /// 播放控制器
        /// </summary>
        public PlayerController playerController => this.XGetComponentInChildrenOrGlobal(ref _playerController);

        /// <summary>
        /// 可播放内容列表
        /// </summary>
        [Name("可播放内容列表")] 
        public List<PlayableContentTableData_Model> _playableContentTableDatas = new List<PlayableContentTableData_Model>();

        /// <summary>
        /// 预加载数据
        /// </summary>
        protected override IEnumerable<TableData_Model> prefabModels => _playableContentTableDatas;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (playerController) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!playerController)
            {
                enabled = false;
                Debug.LogErrorFormat("[{0}]未找到有效的播放器控制器!", CommonFun.ObjectToString(this));
            }
        }

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
        /// 视图项点击
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnClick(TableInteractData tableInteractData)
        {
            base.OnClick(tableInteractData);

            if (playerController) 
            {
                playerController.PlayContent((tableInteractData.tableData_Model as IComponentGetter).component as PlayableContent);
            }
        }
    }

    /// <summary>
    /// 可播放内容表数据
    /// </summary>
    [Serializable]
    public class PlayableContentTableData_Model : ComponentTableData_Model<PlayableContent>
    {
        public PlayableContentTableData_Model(PlayableContent component) : base(component)
        {
        }
    }
}
