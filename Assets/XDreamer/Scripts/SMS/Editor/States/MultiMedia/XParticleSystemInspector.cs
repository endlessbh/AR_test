using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 粒子系统检查器
    /// </summary>
    [Name("粒子系统检查器")]
    [CustomEditor(typeof(XParticleSystem))]
    public class XParticleSystemInspector : WorkClipInspector<XParticleSystem>
    {
        #region 同步TL

        /// <summary>
        /// 标识是否有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected override bool HasSyncTLButton() => true;

        /// <summary>
        /// 获取同步时按钮内容
        /// </summary>
        /// <returns></returns>
        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = base.GetSyncTLButtonContent();
            content.tooltip += string.Format("\n将时长同步为时间轴时长");
            return content;
        }

        /// <summary>
        /// 获取预期的时长
        /// </summary>
        /// <param name="workRangeMemberProperty"></param>
        protected override double? GetExpectedTL()
        {
            var workClip = this.workClip;
            if (workClip && workClip.particleSystem)
            {
                return workClip.particleSystem.main.duration;
            }
            return default;
        }

        #endregion

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button("添加所有子粒子系统"))
            {
                UICommonFun.DelayCall(AddAllSubParticleSystems);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AddAllSubParticleSystems()
        {
            var workClip = this.workClip;
            if (workClip && workClip.particleSystem)
            {
                var xps = workClip.GetComponents<XParticleSystem>(); ;
                foreach (var ps in workClip.particleSystem.GetComponentsInChildren<ParticleSystem>())
                {
                    if (!xps.Any(c => c.particleSystem == ps))
                    {
                        if (workClip.componentCollection.AddComponent(typeof(XParticleSystem)) is XParticleSystem newXPS)
                        {
                            UndoHelper.RegisterCompleteObjectUndo(newXPS);
                            newXPS.particleSystem = ps;
                            newXPS.timeLength = ps.main.duration;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var workClip = this.workClip;
            if (!workClip) return info;

            info.Append("\n粒子系统:\t");
            if (workClip.particleSystem)
            {
                info.Append(CommonFun.GameObjectToString(workClip.particleSystem.gameObject));
                info.AppendFormat("\n时长: \t{0} 秒", workClip.particleSystem.main.duration);
            }
            else
            {
                return info.AppendFormat("<color=red>数据无效</color>");
            }

            return info;
        }
    }
}
