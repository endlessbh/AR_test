using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    /// <summary>
    /// 点集合
    /// </summary>
    public interface IPointSet
    {
        /// <summary>
        /// 有效
        /// </summary>
        bool valid { get; }

        /// <summary>
        /// 点数量
        /// </summary>
        int count { get; }

        /// <summary>
        /// 获取点坐标
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        bool TryGetPoint(int index, out Vector3 point);

        /// <summary>
        /// 设置点坐标
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        bool TrySetPoint(int index, Vector3 point);
    }

    /// <summary>
    /// 点击点拾取器
    /// </summary>
    [Name("点击点拾取器")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IPointSet))]
    public class ClickPointPicker : InteractProvider
    {
        [Name("启用执行")]
        public bool pickOnEnable = true;

        /// <summary>
        /// 执行一次
        /// </summary>
        [Name("执行一次")]
        public bool executeOnce = false;

        [Name("拾取完成回调函数")]
        [UserDefineFun()]
        [HideInSuperInspector(nameof(executeOnce), EValidityCheckType.Equal, false)]
        public string finishUserScriptCallback;

        private IPointSet _pointSet;
        private int _index = 0;
        private bool _pointSetValid = false;

        private bool _finished = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _pointSet = GetComponent<IPointSet>();

            if (pickOnEnable)
            {
                BeginPick();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (!_finished)
            {
                EndPick();
            }
        }

        private void Update()
        {
            if (!_finished && _pointSetValid && TryGetClickPoint(out Vector3 point))
            {
                if (_index < _pointSet.count)
                {
                    _pointSet.TrySetPoint(_index, point);
                    ++_index;
                }

                if (_index== _pointSet.count)
                {
                    EndPick();
                }
            }
        }

        public void BeginPick()
        {
            _index = 0;
            _finished = false;
            _pointSetValid = _pointSet != null && _pointSet.valid && _pointSet.count > 0;
        }

        public void EndPick()
        {
            if (executeOnce)
            {
                _finished = true;
                ScriptManager.CallUDF(finishUserScriptCallback);
            }
        }

        /// <summary>
        /// 获取鼠标点击点位置
        /// </summary>
        /// <param name="point">点击点三维</param>
        /// <returns>点击是否有效</returns>
        private bool TryGetClickPoint(out Vector3 point)
        {
            if (Input.touchCount == 0 && Input.GetMouseButtonDown(0))
            {
                var cam = CameraHelperExtension.currentCamera;

                if (cam && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                {
                    point = hit.point;
                    return true;
                }
            }

            point = Vector3.zero;
            return false;
        }
    }
}