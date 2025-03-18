using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginRepairman.States;

namespace XCSJ.PluginRepairman.States.Study
{
    /// <summary>
    /// 学习选择游戏对象颜色
    /// </summary>
    [Name("学习选择游戏对象颜色")]
    public class StudySelectGameObjectColor : RepairStudyListener
    {
        [Name("选择正确色彩")]
        public Color rightColor = new Color(1,0.6f,0);

        [Name("选择错误色彩")]
        public Color wrongColor = Color.red;

        protected override void OnPartSelected(GameObject selectedGO, bool right)
        {
            SetGameObjectColor(selectedGO, right ? rightColor : wrongColor);
        }

        private void SetGameObjectColor(GameObject go, Color color)
        {
            if (!go) return;

            var renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                foreach (var m in r.materials)
                {
                    if (m) m.color = color;
                }
            }
        }

        protected override void OnToolSelected(Tool tool, bool right) { }
    }
}
