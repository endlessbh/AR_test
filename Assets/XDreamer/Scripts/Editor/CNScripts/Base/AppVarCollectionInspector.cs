using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// App变量集合检查器
    /// </summary>
    [Name("App变量集合检查器")]
    [CustomEditor(typeof(AppVarCollection), true)]
    public class AppVarCollectionInspector: BaseVarCollectionInspector
    {
        /// <summary>
        /// 当绘制成员之后
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("Save All App Variables", "保存所有App变量")]
        [LanguageTuple("Reload All App Variables", "重新加载所有App变量")]
        protected override void OnAfterDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnAfterDrawMember(serializedProperty, propertyData);
            switch (serializedProperty.name)
            {
                case nameof(AppVarCollection._varCollection):
                    {
                        if (GUILayout.Button(Tr("Save All App Variables")))
                        {
                            (target as AppVarCollection).Save();
                            UICommonFun.MarkSceneDirty();
                        }
                        if (GUILayout.Button(Tr("Reload All App Variables")))
                        {
                            (target as AppVarCollection).Reload();
                            UICommonFun.MarkSceneDirty();
                        }
                        break;
                    }
            }
        }
    }
}
