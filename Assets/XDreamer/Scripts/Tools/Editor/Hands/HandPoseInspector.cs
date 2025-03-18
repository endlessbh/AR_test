using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Hands;

namespace XCSJ.EditorTools.Hands
{
    /// <summary>
    /// 可被手抓对象检查器
    /// </summary>
    [Name("可被手抓对象检查器")]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandPose))]
    public class HandPoseInspector : MBInspector<HandPose>
    {
        /// <summary>
        /// 编辑手
        /// </summary>
        public static Hand hand;

        /// <summary>
        /// 姿势数据索引
        /// </summary>
        public static int poseDataIndex = 0;

        /// <summary>
        /// 检查器绘制
        /// </summary>
        //[LanguageTuple("Open xlan File", "虚拟手")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            hand = (Hand)EditorGUILayout.ObjectField("虚拟手", hand, typeof(Hand), true);

            EditorGUI.BeginDisabledGroup(!hand);
            if (GUILayout.Button("保存手姿势"))
            {
                // 将手设置为可抓对象子对象以获取本地坐标系
                targetObject._handPoseDatas.Add(new HandPoseData(targetObject.transform, hand));
            }
            EditorGUI.EndDisabledGroup();

            poseDataIndex = EditorGUILayout.IntField("读取姿势索引", poseDataIndex);

            EditorGUI.BeginDisabledGroup(!hand || poseDataIndex < 0 || poseDataIndex >= targetObject._handPoseDatas.Count);
            if (GUILayout.Button("读取姿势"))
            {
                //targetObject._handPoseDatas[poseDataIndex].DataToHand(hand);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
