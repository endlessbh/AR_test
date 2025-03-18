using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 设备：
    /// 1、比模块更大的集合，但不允许嵌套（树层级顶层）
    /// 2、管理其子级零件和模块的分组信息
    /// 3、监控拖拽事件，对比零件与插槽位置，然后修正零件的拆装状态
    /// </summary>
    [Name("设备")]
    [DisallowMultipleComponent]
    public sealed class Device : Module
    {
        #region 交互输入

        /// <summary>
        /// 输入交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (interactData.interactor is Grabbable grabbable && grabbable)
            {
                if (grabbable._grabResultCmds.TryGetECmd(interactData.cmdName, out var grabCmd))
                {
                    var part = grabbable.GetComponent<Part>();
                    if (!part) return;

                    switch (grabCmd)
                    {
                        case EGrabResultCmd.Grab:
                            {
                                var partSocket = part.partSocket;
                                if (partSocket == null)
                                {
                                    if (FindNearestEmptyPartSocket(part, out partSocket))
                                    {
                                    }
                                }
                                break;
                            }
                        case EGrabResultCmd.Hold:
                            {
                                var partSocket = part.partSocket;
                                if (partSocket != null || FindNearestEmptyPartSocket(part, out partSocket))
                                {
                                    Debug.DrawLine(partSocket.GetWorldPosition(), part.transform.position, partSocket.InSnapDistance(part) ? Color.green : Color.magenta);
                                }
                                break;
                            }
                        case EGrabResultCmd.Release:
                            {
                                CheckPartAssemblyState(part);
                                break;
                            }
                    }
                }
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 零件处理规则
        /// </summary>
        public enum EPartHandleRule
        {
            [Name("无")]
            None = 0,

            [Name("启用零件吸附")]
            AutoSnap,
        }

        /// <summary>
        /// 零件处理规则
        /// </summary>
        [Name("零件处理规则")]
        [EnumPopup]
        public EPartHandleRule _partHandleRule = EPartHandleRule.None;

        /// <summary>
        /// 零件吸附距离缩放系数
        /// </summary>
        [Name("零件吸附距离缩放系数")]
        [Min(0)]
        [HideInSuperInspector(nameof(_partHandleRule), EValidityCheckType.NotEqual, EPartHandleRule.AutoSnap)]
        public float _partSnapDistanceScaleCoefficient = 1f;

        private List<Module> modules = new List<Module>();

        private List<Part> parts = new List<Part>();

        /// <summary>
        /// 零件分类字典：参数1=分类名称，参数2=零件列表
        /// </summary>
        public Dictionary<string, List<Part>> partCategoryMap { get; private set; } = new Dictionary<string, List<Part>>();

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            modules.AddRange(GetComponentsInChildren<Module>());

            CreatePartCategoryMap();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            modules.Clear();
            parts.Clear();

            DestoryCategory();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            CheckPartAssemblyState();
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 创建零件分类图
        /// </summary>
        public void CreatePartCategoryMap()
        {
            foreach (var ps in module._partSockets)
            {
                if (ps._partPrototype)
                {
                    parts.Add(ps._partPrototype);
                }
            }

            CreateCategory(parts);
        }

        /// <summary>
        /// 查找零件
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<Part> FindParts(string categoryName)
        {
            partCategoryMap.TryGetValue(categoryName, out var list);
            return list;
        }

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="parts"></param>
        public void CreateCategory(IEnumerable<Part> parts)
        {
            foreach (var part in parts)
            {
                var tag = part.replacePartTypeTag;
                if (string.IsNullOrEmpty(tag))
                {
                    tag = part.name;
                }
                if (!partCategoryMap.TryGetValue(tag, out var list))
                {
                    partCategoryMap[tag] = list = new List<Part>();
                }
                list.Add(part);
            }
        }

        /// <summary>
        /// 清除组
        /// </summary>
        public void DestoryCategory()
        {
            partCategoryMap.Clear();
        }

        /// <summary>
        /// 查找离零件最近的插槽
        /// </summary>
        /// <param name="part"></param>
        /// <param name="partSocket"></param>
        /// <returns></returns>
        private bool FindNearestEmptyPartSocket(Part part, out PartSocket partSocket)
        {
            partSocket = default;
            if (!part) return false;
            var categoryName = part.replacePartTypeTag;
            var partPosition = part.transform.position;

            float nearestDistance = Mathf.Infinity;
            foreach (var m in modules)
            {
                var data = m.FindNearestEmptyPartSocket(categoryName, partPosition);

                if (data != null)
                {
                    float distance = Vector3.SqrMagnitude(partPosition - data.GetWorldPosition());
                    if (distance < nearestDistance)
                    {
                        partSocket = data;
                        nearestDistance = distance;
                    }
                }
            }
            return partSocket != default;
        }

        /// <summary>
        /// 检测所有零件在设备下的装配情况
        /// </summary>
        public void CheckPartAssemblyState()
        {
            foreach (var part in parts)
            {
                if (modules.Contains(part)) continue;

                CheckPartAssemblyState(part);
            }
        }

        /// <summary>
        /// 零件装配状态检查
        /// </summary>
        /// <param name="part"></param>
        private void CheckPartAssemblyState(Part part)
        {
            if (!part) return;

            if (!part.gameObject.activeInHierarchy)
            {
                if (part.partSocket != null)
                {
                    part.partSocket.assembledPart = null;
                }
                else
                {
                    part.assembleState = Base.EAssembleState.Disassembled;
                }
                return;
            }

            if (part.partSocket != null)
            {
                part.partSocket.UpdateAssembleState();
            }

            if (FindNearestEmptyPartSocket(part, out var partData) && partData.InSnapDistance(part))
            {
                partData.assembledPart = part;
            }
        }

        #endregion
    }
}
