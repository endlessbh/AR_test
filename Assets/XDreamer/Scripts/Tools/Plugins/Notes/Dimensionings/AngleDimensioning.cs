using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    /// <summary>
    /// 角度尺寸标注
    /// </summary>
    [Name("角度尺寸标注")]
    //[Tool("标注", rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Note)]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public class AngleDimensioning : Dimensioning, IPointSet
    {
        /// <summary>
        /// 起点转换
        /// </summary>
        [Name("起点转换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform beginTransform;

        /// <summary>
        /// 终点转换
        /// </summary>
        [Name("终点转换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform endTransform;

        /// <summary>
        /// 中心转换
        /// </summary>
        [Name("中心转换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform centerTransform;

        /// <summary>
        /// 限定平面
        /// </summary>
        [Name("限定平面")]
        public bool limitPlane = false;

        /// <summary>
        /// 法向量
        /// </summary>
        [Name("法向量")]
        [HideInSuperInspector(nameof(limitPlane), EValidityCheckType.Equal, false)]
        public Vector3 normal = Vector3.forward;

        /// <summary>
        /// 偏移距离
        /// </summary>
        [Name("偏移距离")]
        public float offsetDistance = 2;

        /// <summary>
        /// 起点尺寸界线
        /// </summary>
        [Name("起点尺寸界线")]
        [SerializeField]
        protected CubeLine _beginExtensionLine = new CubeLine();

        /// <summary>
        /// 起点尺寸界线
        /// </summary>
        public override MeshLine beginExtensionLine => _beginExtensionLine;

        /// <summary>
        /// 终点尺寸界线
        /// </summary>
        [Name("终点尺寸界线")]
        [SerializeField]
        protected CubeLine _endExtensionLine = new CubeLine();

        /// <summary>
        /// 终点尺寸界线
        /// </summary>
        public override MeshLine endExtensionLine => _endExtensionLine;

        /// <summary>
        /// 尺寸线
        /// </summary>
        [Name("尺寸线")]
        [SerializeField]
        protected CubeLine _dimensionLine = new CubeLine();

        /// <summary>
        /// 尺寸线
        /// </summary>
        public override MeshLine dimensionLine => _dimensionLine;

        /// <summary>
        /// 尺寸数字
        /// </summary>
        [Name("尺寸数字")]
        [SerializeField]
        protected AngleSizeDigital _sizeDigital = new AngleSizeDigital();

        /// <summary>
        /// 尺寸数字
        /// </summary>
        public override SizeDigital sizeDigital => _sizeDigital;

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            if (valid)
            {
                beginExtensionLine.SetVisible(true);
                endExtensionLine.SetVisible(true);
                dimensionLine.SetVisible(true);
                sizeDigital.SetVisible(true);
            }
            else
            {
                beginExtensionLine.SetVisible(false);
                endExtensionLine.SetVisible(false);
                dimensionLine.SetVisible(false);
                sizeDigital.SetVisible(false);
                return;
            }

            var pos0 = beginTransform.position;
            var pos1 = endTransform.position;
            var center = centerTransform.position;

            Vector3 p0;
            Vector3 p1;
            float angle;
            if (limitPlane)
            {
                DimensioningHelper.Angle(pos0, pos1, center, normal, offsetDistance, out p0, out p1, out angle);
            }
            else
            {
                DimensioningHelper.Angle(pos0, pos1, center, offsetDistance, out p0, out p1, out angle);
            }

            //更新尺寸界线
            _beginExtensionLine.Update(center, p0);
            _endExtensionLine.Update(center, p1);

            //更新尺寸线
            _dimensionLine.Update(p0, p1);

            //更新尺数字
            _sizeDigital.Update(p0, p1, center, angle);
        }

        #region IPointSet

        public bool valid => beginTransform && endTransform && centerTransform;

        public int count => 3;

        public bool TryGetPoint(int index, out Vector3 point)
        {
            point = Vector3.zero;
            var result = false;

            switch (index)
            {
                case 0:
                    {
                        result = beginTransform;
                        if (beginTransform)
                        {
                            point = beginTransform.position;
                        }
                        break;
                    }
                case 1:
                    {
                        result = endTransform;
                        if (endTransform)
                        {
                            point = endTransform.position;
                        }
                        break;
                    }

                case 2:
                    {
                        result = centerTransform;
                        if (centerTransform)
                        {
                            point = centerTransform.position;
                        }
                        break;
                    }
            }

            return result;
        }

        public bool TrySetPoint(int index, Vector3 point)
        {
            var result = false;

            switch (index)
            {
                case 0:
                    {
                        result = beginTransform;
                        if (beginTransform)
                        {
                            beginTransform.position = point;
                        }
                        break;
                    }
                case 1:
                    {
                        result = endTransform;
                        if (endTransform)
                        {
                            endTransform.position = point;
                        }
                        break;
                    }
                case 2:
                    {
                        result = centerTransform;
                        if (centerTransform)
                        {
                            centerTransform.position = point;
                        }
                        break;
                    }
            }

            return result;
        }
        #endregion
    }
}
