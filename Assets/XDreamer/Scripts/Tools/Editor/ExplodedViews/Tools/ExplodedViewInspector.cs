using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.PluginTools.ExplodedViews;
using XCSJ.PluginTools.ExplodedViews.Tools;

namespace XCSJ.EditorTools.ExplodedViews.Tools
{
    /// <summary>
    /// 爆炸图检查器
    /// </summary>
    [Name("爆炸图检查器")]
    [CustomEditor(typeof(ExplodedView))]
    public class ExplodedViewInspector : PlayableContentInspector<ExplodedView>
    {
        private bool inSimulation = false;

        private float percent = 0;

        /// <summary>
        /// 禁用时
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopSimulate();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying) return;

            if (GUILayout.Button("启用爆炸图模拟并记录数据"))
            {
                StartSimulate();
            }

            EditorGUI.BeginDisabledGroup(!inSimulation);
            {
                if (GUILayout.Button("停止爆炸图模拟并还原(清除)数据"))
                {
                    StopSimulate();
                }

                EditorGUI.BeginDisabledGroup(mb.datas.Count == 0);
                {
                    if (GUILayout.Button("点爆"))
                    {
                        Explode(EExplodeType.Point);
                    }

                    if (GUILayout.Button("线爆"))
                    {
                        Explode(EExplodeType.Line);
                    }

                    if (GUILayout.Button("面爆"))
                    {
                        Explode(EExplodeType.Plane);
                    }

                    EditorGUI.BeginChangeCheck();
                    percent = EditorGUILayout.Slider(percent, 0, (float)mb.explodeMultiple);
                    if (EditorGUI.EndChangeCheck() && inSimulation)
                    {
                        mb.UpdatePercent(percent);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void StartSimulate()
        {
            if (inSimulation) return;
            inSimulation = true;
            if (mb)
            {
                mb.SetSimulation(true);
                mb.SetDrawInfo(mb.center, mb.direction);
                mb.Record();
            }         
            percent = 0;
        }

        private void StopSimulate()
        {
            inSimulation = false;
            percent = 0;
            if (mb)
            {
                mb.Clear();
                mb.SetSimulation(false);
            }
        }

        private void Explode(EExplodeType explodeType)
        {
            if (mb)
            {
                mb.Recovry();
                //mb.explodeType = explodeType;
                var center = mb.center;
                var direction = mb.direction;
                mb.SetDrawInfo(center, direction);
                mb.datas = ExplodedViewHelper.Explode(explodeType, mb.datas, center, direction, mb.deltaIntervalValue, mb.minIntervalValue, mb._sortRule);
                mb.UpdateTranforms();
            }       
            percent = 1;
        }
    }
}
