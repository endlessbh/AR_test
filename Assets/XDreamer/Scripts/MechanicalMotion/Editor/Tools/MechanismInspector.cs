using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.Tools;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 机构检查器
    /// </summary>
    [CustomEditor(typeof(Mechanism), true)]
    public class MechanismInspector : MechanismInspector<Mechanism>
    {
        /// <summary>
        /// 绘制根机构
        /// </summary>
        public static void DrawRootMechanism() => DrawMechanisms(false, null, MechanicalMotionHelper.GetRootMechanisms());

        /// <summary>
        /// 绘制当前机构的父子级
        /// </summary>
        /// <param name="mechanism"></param>
        public static void DrawMechanism(Mechanism mechanism)
        {
            if (mechanism)
            {
                DrawMechanisms(true, mechanism.parent, mechanism.children);
            }
        }

        private static void DrawMechanisms(bool drawRoot, Mechanism parent, Mechanism[] children)
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(typeof(MechanismInspector), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();
            {
                // 根
                if (drawRoot)
                {
                    DrawRoot();
                }

                // 父级
                DrawParent(parent);

                // 绘制子级
                DrawChildren(children);
            }
            CommonFun.EndLayout();
        }

        private static void DrawRoot()
        {
            if (GUILayout.Button(CommonFun.Name(typeof(MechanicalMotionManager))))
            {
                Selection.activeObject = MechanicalMotionManager.instance;
            }
        }

        private static void DrawParent(Mechanism parent)
        {
            if (parent)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    //父级
                    EditorGUILayout.LabelField("父级", UICommonOption.Width32);

                    //父级机构名称
                    EditorGUILayout.LabelField(parent.mechanismName, UICommonOption.Width120);

                    //机构组件
                    EditorGUILayout.ObjectField(parent, typeof(Mechanism), true);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawChildren(Mechanism[] children)
        {
            // 标题
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("NO.", UICommonOption.Width32);
                GUILayout.Label("名称", UICommonOption.Width120);
                GUILayout.Label("运动机构");
                if (GUILayout.Button("全部选中", UICommonOption.Width60))
                {
                    Selection.objects = children.Cast(c => c.gameObject).ToArray();
                }
            }
            EditorGUILayout.EndHorizontal();

            // 子级内容
            for (int i = 0; i < children.Length; i++)
            {
                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                var m = children[i];

                //机构名称
                EditorGUILayout.LabelField(m.mechanismName, UICommonOption.Width120);

                //窗口组件
                EditorGUILayout.ObjectField(m, typeof(Mechanism), true);

                UICommonFun.EndHorizontal();
            }
        }
        
        /// <summary>
        /// 绘制场景辅助对象
        /// </summary>
        public override void DrawScene() { }

        /// <summary>
        /// 机构层级
        /// </summary>
        [Name("机构层级")]
        [Tip("当前对象的父级与子级", "")]
        public static bool _display = true;

    }

    /// <summary>
    /// 机构检查器
    /// </summary>
    [CanEditMultipleObjects]
    public abstract class MechanismInspector<T> : MBInspector<T> where T : Mechanism
    {
        /// <summary>
        /// 场景图像首选项
        /// </summary>
        protected SceneGraphOption sceneGraphOption { get; private set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            sceneGraphOption = SceneGraphOption.weakInstance;
        }

        /// <summary>
        /// 场景绘制
        /// </summary>
        public virtual void OnSceneGUI()
        {
            // 绘制自身
            DrawScene();
        }

        /// <summary>
        /// 绘制场景辅助对象
        /// </summary>
        public abstract void DrawScene();

        /// <summary>
        /// 绘制检查器UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MechanismInspector.DrawMechanism(targetObject);
        }

        /// <summary>
        /// 首选项修改回调方法
        /// </summary>
        /// <param name="option"></param>
        protected override void OnOptionModify(Option option)
        {
            base.OnOptionModify(option);

            if (option is SceneGraphOption sceneGraphOption)
            {
                this.sceneGraphOption = sceneGraphOption;
            }
        }
    }

}