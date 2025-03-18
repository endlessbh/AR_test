using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 图形记录器
    /// </summary>
    public class GraphicRecorder : Recorder<Graphic, GraphicRecorder.Info>, IRecord<GameObject>, IRecord<IEnumerable<GameObject>>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GraphicRecorder() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="graphic"></param>
        public GraphicRecorder(Graphic graphic) 
        {
            Record(graphic);
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.GetComponent<Graphic>());
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 信息类
        /// </summary>
        public class Info : ISingleRecord<Graphic>
        {
            /// <summary>
            /// 图形
            /// </summary>
            public Graphic graphic;

            /// <summary>
            /// 颜色
            /// </summary>
            public Color color;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="graphic"></param>
            public void Record(Graphic graphic)
            {
                this.graphic = graphic;
                if(graphic)
                {
                    this.color = graphic.color;
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                RecoverColor();
            }

            /// <summary>
            /// 恢复颜色
            /// </summary>
            public void RecoverColor()
            {
                if (graphic)
                {
                    graphic.color = color;
                }
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            /// <param name="color"></param>
            public void SetPercent(Percent percent, Color color)
            {
                if (graphic)
                {
                    graphic.color = Color.Lerp(this.color, color, (float)percent.percent01OfWorkCurve);
                }
            }

            /// <summary>
            /// 设置颜色
            /// </summary>
            /// <param name="color"></param>
            public void SetColor(Color color)
            {
                if (graphic) graphic.color = color;
            }

            /// <summary>
            /// 设置透明度
            /// </summary>
            /// <param name="alpha"></param>
            public void SetAlpha(float alpha)
            {
                if (graphic)
                {
                    var c = graphic.color;
                    c.a = alpha;
                    graphic.color = c;
                }
            }
        }
    }
}
