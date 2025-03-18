using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorExtension.Base
{
    [XDreamerPreferences]
    [Name("场景图形绘制")]
    [Import]
    public class SceneGraphOption : XDreamerOption<SceneGraphOption>
    {
        [Name("文字尺寸")]
        public int labelSize = 16;

        [Name("文字颜色")]
        [Json(exportString = true)]
        public Color labelColor = Color.white;

        [Name("线颜色")]
        [Json(exportString = true)]
        public Color lineColor = Color.cyan;

        [Name("面颜色")]
        [Json(exportString = true)]
        public Color planeColor = new Color(0, 1, 1, 0.2f);

        [Name("关键点颜色")]
        [Json(exportString = true)]
        public Color keyPointColor = Color.magenta;

        [Name("大球体尺寸")]
        public float bigSphereRadius = 0.4f;

        [Name("正常球体尺寸")]
        public float sphereRadius = 0.2f;

        [Name("小球体尺寸")]
        public float smallSphereRadius = 0.1f;

        [Name("半径")]
        public float radius = 1f;

        [Name("箭头颜色")]
        [Json(exportString = true)]
        public Color arrowColor = Color.yellow;

        [Name("箭头长度")]
        public float arrowLength = 2f;
    }
}
