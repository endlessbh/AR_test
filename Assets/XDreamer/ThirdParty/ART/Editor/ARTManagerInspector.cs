using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginART.Base;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART
{
    /// <summary>
    /// ART管理器检查器
    /// </summary>
    [Name("ART管理器检查器")]
    [CustomEditor(typeof(ARTManager))]
    public class ARTManagerInspector : BaseManagerInspector<ARTManager>
    {
        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<ARTStreamClient>();
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Create [ARTStreamClient]", "创建[ART流客户端]")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Tr("Create [ARTStreamClient]")))
            {
                manager.XGetOrAddComponent<ARTStreamClient>();
            }

            DrawDetailInfos();
            DrawDetailInfos_Flystick();
        }

        /// <summary>
        /// ART刚体关联列表
        /// </summary>
        [Name("ART刚体关联列表", "ART Rigidbody Link List")]
        [Tip("当前场景中所有与ART刚体关联的对象", "All objects associated with art rigid bodies in the current scene")]
        public bool _display = true;

        /// <summary>
        /// ART关联对象
        /// </summary>
        [Name("ART关联对象", "ART Link Object")]
        [Tip("ART刚体关联的组件；本项只读；", "Components associated with art rigid body; This item is read-only;")]
        public bool linkObject = true;

        /// <summary>
        /// 关联对象拥有者
        /// </summary>
        [Name("关联对象拥有者", "Link Object Owner")]
        [Tip("ART刚体关联对象拥有者所在的游戏对象；本项只读；", "The game object of the owner of the art rigid body association object; This item is read-only;")]
        public bool linkObjectOwner = true;

        /// <summary>
        /// 激活启用
        /// </summary>
        [Name("激活启用")]
        [Tip("ART刚体关联对象是否处于激活并启用的状态；本项只读；", "Whether the art rigid body association object is active and enabled; This item is read-only;")]
        public bool activeEnable = true;

        /// <summary>
        /// 数据类型
        /// </summary>
        [Name("数据类型")]
        [Tip("用于与ART软件进行数据流通信的数据类型；", "Data type for data flow communication with art software;")]
        public bool dateType = true;

        /// <summary>
        /// 刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与ART软件进行数据流通信的刚体ID；", "Rigid body ID for data flow communication with art software;")]
        public bool rigidbodyID = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, TrLabel(nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(linkObject)));
            GUILayout.Label(TrLabel(nameof(linkObjectOwner)));
            GUILayout.Label(TrLabel(nameof(activeEnable)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(dateType)), UICommonOption.Width80);
            GUILayout.Label(TrLabel(nameof(rigidbodyID)), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IARTObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IARTObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ART关联对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //ART关联对象
                var owner = component.GetARTObjectOwnerGameObject();
                EditorGUILayout.ObjectField(owner, typeof(GameObject), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //数据类型
                EditorGUI.BeginChangeCheck();
                var dataType = UICommonFun.EnumPopup(link.dataType, UICommonOption.Width80);
                if (EditorGUI.EndChangeCheck())
                {
                    link.dataType = (EDataType)dataType;
                }

                //刚体ID
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// ARTFlystick手柄关联列表
        /// </summary>
        [Name("ART Flystick手柄关联列表", "Art Flystick Handle Association List")]
        [Tip("当前场景中所有与ART Flystick手柄关联的对象", "All objects associated with art flystick handles in the current scene")]
        public bool _displayFlystick = true;

        /// <summary>
        /// Flystick对象
        /// </summary>
        [Name("Flystick对象")]
        [Tip("Flystick关联的组件；本项只读；", "Components associated with flystick; This item is read-only;")]
        public bool flystickObject = true;

        /// <summary>
        /// 编号
        /// </summary>
        [Name("编号")]
        [Tip("用于与ART软件进行数据流通信的Flystick编号；", "Flystick number for data flow communication with art software;")]
        public bool number = true;

        /// <summary>
        /// 手柄
        /// </summary>
        [Name("手柄")]
        [Tip("用于与ART软件进行数据流通信的Flystick手柄类型；", "The type of flystick handle used for data flow communication with art software;")]
        public bool handle = true;

        private void DrawDetailInfos_Flystick()
        {
            _displayFlystick = UICommonFun.Foldout(_displayFlystick, TrLabel(nameof(_displayFlystick)));
            if (!_displayFlystick) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(flystickObject)));
            GUILayout.Label(TrLabel(nameof(activeEnable)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(number)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(handle)), UICommonOption.Width80);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IARTFlystickObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IARTFlystickObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ART关联对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //Flystick编号
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                //Flystick手柄
                EditorGUI.BeginChangeCheck();
                var flystick = UICommonFun.EnumPopup(link.flystick, UICommonOption.Width80);
                if (EditorGUI.EndChangeCheck())
                {
                    link.flystick = (EFlystick)flystick;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}