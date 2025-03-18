using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;

namespace XCSJ.EditorTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图列表检查器
    /// </summary>
    [Name("游戏对象视图列表检查器")]
    [CustomEditor(typeof(GrabbableList), true)]
    public class GrabbableListInspector : InteractorInspector<GrabbableList>
    {
        /// <summary>
        /// 游戏对象列表操作
        /// </summary>
        [Name("游戏对象列表操作")]
        public bool gameObjectListOperation;

        /// <summary>
        /// 添加选中游戏对象到列表中
        /// </summary>
        [Name("添加选中游戏对象到列表中")]
        public string addSelectedGameObjectsToList;

        private static int gameObjectCount = 3;

        [Languages.LanguageTuple("Count","数量")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(GrabbableList._cloneGrabbableTableDataMakers):
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(gameObjectListOperation)));
                        if (GUILayout.Button(TrLabel(nameof(addSelectedGameObjectsToList))))
                        {
                            targetObject.XModifyProperty(() =>
                            {
                                foreach (var go in Selection.gameObjects)
                                {
                                    var grabbable = go.XGetOrAddComponent<Grabbable>();
                                    if (grabbable)
                                    {
                                        targetObject._cloneGrabbableTableDataMakers.Add(new CloneGrabbableTableDataMaker(grabbable, gameObjectCount));
                                    }
                                }
                            });
                        }
                        GUILayout.Label(Tr("Count"));
                        gameObjectCount = EditorGUILayout.IntField(gameObjectCount, GUILayout.Width(32));
                        if (gameObjectCount<1)
                        {
                            gameObjectCount = 1;
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                default:
                    break;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var dragger = UnityObjectExtension.XGetComponentInGlobal<Dragger>();
            if (!dragger)
            {
                info.AppendLine("<color=red>场景中缺少【一键拖拽工具】，本组件需要依赖它才能工作！</color>");
            }
            return info;
        }
    }

}
