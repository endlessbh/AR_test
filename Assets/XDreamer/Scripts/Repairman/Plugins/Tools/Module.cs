using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 模块：
    /// 1、零件的集合
    ///    1.1、零件不为实体零件，而是零件分类名称信息
    ///    1.2、记录零件插槽信息。包括在设备空间下的位置与旋转信息（本地）和零件运动约束（移动或旋转）
    ///    1.3、零件必须以模块为直接父级，否则中间层级也需要转化为一个模块
    /// 2、模块与模块之间可嵌套
    /// 3、记录零件之间的装配约束关系
    /// 4、零件管理。
    ///    4.1、当零件装配到模块中时，零件为模块的子级；当零件从模块中移除时，零件父级变为空（即场景根级）；
    ///    4.2、拖拽模式管理。当点击到模块下的零件运动受限时，拖拽的是整个模块，零件运动不受限时，拖拽的是零件
    ///    4.3、零件在模块中时为装配态；零件不在模块中时为拆卸态（即游离态），并且零件本身不再能查询到模块信息
    /// </summary>
    [Name("模块")]
    [DisallowMultipleComponent]
    public class Module : Part
    {
        /// <summary>
        /// 关联状态组件
        /// </summary>
        [Group("零件设置", textEN = "Part Settings")]
        [Name("关联状态组件")]
        [StateComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public States.Module _moduleSC = null;

        /// <summary>
        /// 关联状态组件
        /// </summary>
        public States.Module moduleSC
        {
            get
            {
                if (!_moduleSC || _moduleSC.gameObject != gameObject)
                {
                    _moduleSC = SMSHelper.GetStateComponents<States.Module>().Find(msc => msc.gameObject == gameObject);
                }
                return _moduleSC;
            }
        }

        /// <summary>
        /// 可拖拽
        /// </summary>
        [Name("可拖拽")]
        public bool _canDrag = false;

        /// <summary>
        /// 能否拖拽
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected override bool CanGrabbed(Dragger dragger) => base.CanGrabbed(dragger) && _canDrag;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            _partSockets.ForEach(ps => ps.module = this);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // 给每个零件对象注入装配约束关系
            foreach (var item in _partAssemblyConstraints)
            {
                item.AddConstraint();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // 将装配约束关系从零件中移除
            foreach (var item in _partAssemblyConstraints)
            {
                item.RemoveConstraint();
            }
        }

        /// <summary>
        /// 模块在父级的插槽中位置是匹配的，子级插槽数据也是满的情况下才是装配态
        /// </summary>
        public override EAssembleState assembleState 
        {
            get
            {
                bool allFull = _partSockets.All(ps => ps.full);
                if (!allFull)
                {
                    return EAssembleState.Disassembled;
                }
                return base.assembleState;
            }
            internal set => base.assembleState = value; 
        }

        #region 零件插槽

        /// <summary>
        /// 零件插槽数据
        /// </summary>
        [Name("零件插槽数据")]
        public List<PartSocket> _partSockets = new List<PartSocket>();

        /// <summary>
        /// 查找零件数据
        /// </summary>
        /// <param name="part"></param>
        /// <param name="partData"></param>
        /// <returns></returns>
        protected bool TryGetPartSocket(Part part, out PartSocket partData)
        {
            if (part)
            {
                partData = _partSockets.Find(d => d.IsCategoryMatch(part.replacePartTypeTag));
                return partData != null;
            }
            partData = default;
            return false;
        }

        /// <summary>
        /// 获取分类名称匹配的零件数据列表
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<PartSocket> GetPartSockets(string categoryName) => new List<PartSocket>(_partSockets.Where(d => d.IsCategoryMatch(categoryName)));

        /// <summary>
        /// 获取分类名称匹配的零件数据列表
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<PartSocket> GetEmptyPartSockets(string categoryName) => new List<PartSocket>(_partSockets.Where(d => d.empty && d.IsCategoryMatch(categoryName)));

        /// <summary>
        /// 查找离输入点最近的零件插槽
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public PartSocket FindNearestEmptyPartSocket(string categoryName, Vector3 position)
        {
            PartSocket result = null;
            float nearestDistance = Mathf.Infinity;
            foreach (var data in GetEmptyPartSockets(categoryName))
            {
                float distance = Vector3.SqrMagnitude(position - data.GetWorldPosition());// 使用平方代替距离，减少开方运算
                if (distance < nearestDistance)
                {
                    result = data;
                    nearestDistance = distance;
                }
            }
            return result;
        }

        /// <summary>
        /// 创建零件插槽
        /// </summary>
        public static void CreateSockets(Module module)
        {
            module.XModifyProperty(() =>
            {
                foreach (var part in module.GetComponentsInChildren<Part>(true))
                {
                    if (part != module && part.module == module)
                    {
                        var ps = new PartSocket(part);
                        ps.module = module;
                        module._partSockets.Add(ps);

                        if (part is Module m)
                        {
                            CreateSockets(m);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 清除零件插槽
        /// </summary>
        /// <param name="module"></param>
        public static void ClearSockets(Module module)
        {
            module.XModifyProperty(() =>
            {
                module._partSockets.Clear();

                foreach (var part in module.GetComponentsInChildren<Part>(true))
                {
                    if (part != module && part.module == module)
                    {
                        if (part is Module m)
                        {
                            ClearSockets(m);
                        }
                    }
                }
            });
        }

        #endregion

        #region 零件装配约束

        /// <summary>
        /// 零件装配约束数据
        /// </summary>
        [Name("零件装配约束数据")]
        public List<PartAssemblyConstraint> _partAssemblyConstraints = new List<PartAssemblyConstraint>();

        public List<PartAssemblyConstraint> partAssemblyConstraints => _partAssemblyConstraints;

        public bool TryGetAssemblyConstraint(Part fromPart, out List<Part> toParts)
        {
            var resultConstraints = partAssemblyConstraints.FindAll(pc => pc._fromPart == fromPart);
            toParts = new List<Part>();
            foreach (var constraint in resultConstraints)
            {
                toParts.Add(constraint._toPart);
            }
            return toParts.Count > 0;
        }

        /// <summary>
        /// 获取可自由装配零件列表
        /// </summary>
        /// <param name="includeAssembled">包含已装配零件</param>
        /// <returns></returns>
        public IEnumerable<Part> GetCanAssemblyParts(bool includeAssembled = false)
        {
            return new List<Part>();
        }

        /// <summary>
        /// 获取可自由拆卸零件列表
        /// </summary>
        /// <param name="includeDisassembled">包含已拆卸零件</param>
        /// <returns></returns>
        public IEnumerable<Part> GetCanDisassemblyParts(bool includecannotDisassembled = false)
        {
            return _partSockets.Where(data => data.assembledPart && data.assembledPart.canDisassembly).Cast(d => d.assembledPart);
        }

        /// <summary>
        /// 获取装配顺序：并行零件也被串行化
        /// </summary>
        /// <param name="includeAssembled">包含已装配零件</param>
        /// <returns>返回零件与装配深度元组列表</returns>
        public List<(Part, int)> GetAssemblyOrder(bool includeAssembled = false) => BreadthSearchPart(GetCanAssemblyParts(includeAssembled), true);

        /// <summary>
        /// 获取拆卸顺序：并行零件也被串行化
        /// </summary>
        /// <param name="includeDisassembled">包含已拆卸零件</param>
        /// <returns>返回零件与装配深度元组列表</returns>
        public List<(Part, int)> GetDisassemblyOrder(bool includeDisassembled = false) => BreadthSearchPart(GetCanDisassemblyParts(includeDisassembled), false);

        /// <summary>
        /// 广度遍历零件约束关系，输出顺序化结果
        /// </summary>
        /// <param name="inParts"></param>
        /// <param name="isAssembly"></param>
        /// <returns>返回零件与装配深度元组列表</returns>
        protected List<(Part, int)> BreadthSearchPart(IEnumerable<Part> inParts, bool isAssembly)
        {
            var result = new List<(Part, int)>();
            var visitedParts = new HashSet<Part>();

            var queue = new Queue<(Part, int)>();
            inParts.Foreach(p => queue.Enqueue((p, 1)));
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentPart = current.Item1;
                var currentDeep = current.Item2;
                if (!visitedParts.Contains(currentPart))
                {
                    visitedParts.Add(currentPart);
                    result.Add((currentPart, currentDeep));

                    List<Part> searchParts = isAssembly ? currentPart.disassemblyParts : currentPart.assemblyParts;

                    searchParts.ForEach(p =>
                    {
                        if (!visitedParts.Contains(p))
                        {
                            queue.Enqueue((currentPart, currentDeep + 1));
                        }
                    });
                }
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// 零件插槽
    /// </summary>
    [Serializable]
    public class PartSocket
    {
        #region 零件属性插槽

        /// <summary>
        /// 替换零件种类标签
        /// </summary>
        public string replacePartTypeTag => _partPrototype ? _partPrototype.replacePartTypeTag : "";

        /// <summary>
        /// 在设备完全装配好的情况下指向的零件原型对象
        /// </summary>
        [Name("零件原型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Part _partPrototype;

        [Name("本地位置")]
        public Vector3 _localPosition = Vector3.zero;

        [Name("本地旋转")]
        public Quaternion _localRotation = Quaternion.identity;

        [Name("包围盒大小")]
        public Bounds _bounds = new Bounds();

        [Name("吸附距离")]
        [Min(0)]
        public float _snapDistance = 1;

        #endregion

        /// <summary>
        /// 已装配到模块上的零件对象
        /// </summary>
        public Part assembledPart
        {
            get => _assembledPart;
            set
            {
                if (_assembledPart != value)
                {
                    if (value)// 新的装配零件
                    {
                        TrySnapPart(value);
                        value.assembleState = EAssembleState.Assembled;
                        value.partSocket = this;
                    }
                    else if(_assembledPart)// 旧装配零件
                    {
                        _assembledPart.assembleState = EAssembleState.Disassembled;
                        _assembledPart.partSocket = null;
                    }
                    _assembledPart = value;
                }
            }
        }
        public Part _assembledPart;

        /// <summary>
        /// 更新装配状态
        /// </summary>
        public void UpdateAssembleState()
        {
            if (assembledPart)
            {
                if (InSnapDistance(assembledPart))
                {
                    TrySnapPart(assembledPart);
                }
                else
                {
                    assembledPart = null;
                }
            }
        }

        /// <summary>
        /// 装配
        /// </summary>
        public bool full => assembledPart;

        /// <summary>
        /// 未装配
        /// </summary>
        public bool empty => !full;

        /// <summary>
        /// 所属模块
        /// </summary>
        internal Module module;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        /// <param name="positionOffset"></param>
        /// <param name="rotationOffset"></param>
        public PartSocket(Part part)
        {
            _partPrototype = part;
            _localPosition = part.transform.localPosition;
            _localRotation = part.transform.localRotation;

            CommonFun.GetBounds(out _bounds, part.gameObject);

            var extents = _bounds.extents;
            _snapDistance = Mathf.Max(extents.x, extents.y, extents.z);
        }

        /// <summary>
        /// 分类相同
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public bool IsCategoryMatch(string replacePartTypeTag) => _partPrototype && !string.IsNullOrEmpty(replacePartTypeTag) && _partPrototype.replacePartTypeTag == replacePartTypeTag;

        /// <summary>
        /// 获取零件到当前存储姿态的距离
        /// </summary>
        /// <param name="module"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public float GetDistance(Part part) => part ? Vector3.Distance(GetWorldPosition(), part.transform.position) : 0;

        /// <summary>
        /// 在零件吸附距离内
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool InSnapDistance(Part part)
        {
            if (part)
            {
                var snapDistance = _snapDistance;
                var device = part.device;
                if (device)
                {
                    snapDistance *= device._partSnapDistanceScaleCoefficient;
                }
                return GetDistance(part) <= _snapDistance;
            }
            return false;
        }

        private void TrySnapPart(Part part)
        {
            if (!part) return;

            var dev = part.device;
            if (!dev) return;

            switch (dev._partHandleRule)
            {
                case Device.EPartHandleRule.AutoSnap:
                    {
                        if (InSnapDistance(part))
                        {
                            SetWorldPose(part);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置世界姿态
        /// </summary>
        /// <param name="parent"></param>
        public void SetWorldPose(Part part)
        {
            if (part)
            {
                part.transform.position = GetWorldPosition();
                part.transform.rotation = GetWorldRotation();
            }
        }

        /// <summary>
        /// 获取世界姿态
        /// </summary>
        /// <returns></returns>
        public Pose GetWorldPose() => new Pose(GetWorldPosition(), GetWorldRotation());

        /// <summary>
        /// 获取位置
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition() => module.transform.position + _localPosition;

        /// <summary>
        /// 获取旋转量
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Quaternion GetWorldRotation() => module.transform.rotation * _localRotation;
    }

    /// <summary>
    /// 零件装配关系
    /// </summary>
    [Serializable]
    public class PartAssemblyConstraint
    {
        [Name("先装配")]
        public Part _fromPart;

        [Name("后装配")]
        public Part _toPart;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fromPart"></param>
        /// <param name="toPart"></param>
        public PartAssemblyConstraint(Part fromPart, Part toPart)
        {
            _fromPart = fromPart;
            _toPart = toPart;
        }

        /// <summary>
        /// 增加约束关系
        /// </summary>
        internal void AddConstraint()
        {
            if (!_fromPart || !_toPart) return;
            _fromPart.disassemblyParts.Add(_toPart);
            _toPart.assemblyParts.Add(_fromPart);
        }

        /// <summary>
        /// 移除约束关系
        /// </summary>
        internal void RemoveConstraint()
        {
            if (!_fromPart || !_toPart) return;
            _fromPart.disassemblyParts.Remove(_toPart);
            _toPart.assemblyParts.Remove(_fromPart);
        }
    }

}
