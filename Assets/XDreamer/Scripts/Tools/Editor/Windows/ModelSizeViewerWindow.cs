using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 模型尺寸查看器
    /// </summary>
    [Serializable]
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class ModelSizeViewerWindow : XEditorWindowWithScrollView<ModelSizeViewerWindow>
    {
        public const string Title = "模型尺寸查看器";

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        [Name("当前游戏对象")]
        public GameObject currentGO = null;

        [Name("使用选择集")]
        public bool useSelection = true;

        [Name("包围盒信息(基于世界坐标系)")]
        public bool expandOfBoundsInfos = true;

        [Name("辅助设置")]
        public bool expandOfHelpOption = true;

        [Name("层级")]
        [EnumPopup]
        public EHierarchyType hierarchyType = EHierarchyType.Son;

        [Name("辅助对象类型")]
        [EnumPopup]
        public EPrimitiveType primitiveType = EPrimitiveType.GizmoRenderer;

        [Name("随包围盒缩放")]
        public bool scaleWithBounds = true;

        [Name("缩放值")]
        public Vector3 scaleValue = new Vector3(1, 1, 1);

        [Name("包围盒锚点")]
        [EnumPopup]
        public EBoundsAnchor boundsAnchor = EBoundsAnchor.Center;

        [Name("位置值")]
        public Vector3 positionValue = new Vector3();

        [Name("重名时重命名")]
        public bool renameWhenHasSameName = true;

        [Name("激活")]
        public bool isActive = true;

        [Name("总是渲染")]
        public bool alwaysRender = false;

        [Name("包含成员")]
        public bool includeChildren = true;

        [Name("包含非激活游戏对象")]
        public bool includeInactiveGO = true;

        [Name("包含禁用的渲染器")]
        public bool includeDisableRenderer = true;

        [Name("渲染包围盒")]
        public bool renderbBounds = true;

        private Bounds bounds = new Bounds();

        public void OnSelectionChange() => Repaint();

        public override void OnGUIWithScrollView()
        {
            EditorGUI.BeginChangeCheck();

            #region 基础选项

            currentGO = EditorToolkitHelper.GameObjectField(CommonFun.NameTooltip(this, nameof(currentGO)), currentGO, CommonFun.NameTooltip(this, nameof(useSelection)), ref useSelection);

            includeChildren = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(includeChildren)), includeChildren);
            includeInactiveGO = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(includeInactiveGO)), includeInactiveGO);
            includeDisableRenderer = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(includeDisableRenderer)), includeDisableRenderer);            

            bool ok = CommonFun.GetBounds(out bounds, currentGO, includeChildren, includeInactiveGO, includeDisableRenderer);

            #endregion

            #region 包围盒信息

            expandOfBoundsInfos = UICommonFun.Foldout(expandOfBoundsInfos, CommonFun.NameTooltip(this, nameof(expandOfBoundsInfos)));

            if (expandOfBoundsInfos)
            {
                CommonFun.BeginLayout();

                renderbBounds = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(renderbBounds)), renderbBounds);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Min");
                EditorGUILayout.Vector3Field("", bounds.min);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Max");
                EditorGUILayout.Vector3Field("", bounds.max);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("半尺寸");
                EditorGUILayout.Vector3Field("", bounds.extents);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("尺寸");
                EditorGUILayout.Vector3Field("", bounds.size);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("中心");
                EditorGUILayout.Vector3Field("", bounds.center);
                EditorGUILayout.EndHorizontal();

                CommonFun.EndLayout();
            }

            #endregion

            #region 辅助设置

            expandOfHelpOption = UICommonFun.Foldout(expandOfHelpOption, CommonFun.NameTooltip(this, nameof(expandOfHelpOption)));

            if (expandOfHelpOption)
            {
                CommonFun.BeginLayout();

                hierarchyType = (EHierarchyType)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(hierarchyType)), hierarchyType);

                primitiveType = (EPrimitiveType)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(primitiveType)), primitiveType);

                scaleWithBounds = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(scaleWithBounds)), scaleWithBounds);

                if (scaleWithBounds) scaleValue = bounds.size;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.NameTooltip(this, nameof(scaleValue)));
                scaleValue = EditorGUILayout.Vector3Field("", scaleValue);
                if (GUILayout.Button("R", EditorStyles.miniButtonRight, GUILayout.Width(20))) scaleValue = new Vector3(1, 1, 1);
                EditorGUILayout.EndHorizontal();

                boundsAnchor = (EBoundsAnchor)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(boundsAnchor)), boundsAnchor);

                positionValue = AnchorHelper.GetAnchorPoition(bounds, boundsAnchor);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.NameTooltip(this, nameof(positionValue)));
                positionValue = EditorGUILayout.Vector3Field("", positionValue);
                if (GUILayout.Button("R", EditorStyles.miniButtonRight, GUILayout.Width(20))) positionValue = new Vector3();
                EditorGUILayout.EndHorizontal();

                renameWhenHasSameName = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(renameWhenHasSameName)), renameWhenHasSameName);

                isActive = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(isActive)), isActive);

                alwaysRender = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(alwaysRender)), alwaysRender);

                EditorGUI.BeginDisabledGroup(!ok);
                if (GUILayout.Button("添加辅助游戏对象"))
                {
                    GameObject helpGO;
                    switch (primitiveType)
                    {
                        case EPrimitiveType.Empty:
                            {
                                helpGO = UnityObjectHelper.CreateGameObject("");
                                helpGO.transform.XSetLocalScale(scaleValue);
                                break;
                            }
                        case EPrimitiveType.GizmoRenderer:
                            {
                                helpGO = UnityObjectHelper.CreateGameObject("");
                                var gizmos = helpGO.XAddComponent<GizmoRenderer>();
                                gizmos.XModifyProperty(() =>
                                {
                                    gizmos.gizmoShapeType = EGizmoShapeType.Cube;
                                    gizmos._sizeRule = GizmoRenderer.ESizeRule.Value;
                                    gizmos.sizeValue = scaleValue;
                                    gizmos.alwayShowGizmos = alwaysRender;
                                });
                                break;
                            }
                        default:
                            {                               
                                helpGO = UnityObjectHelper.CreateGameObject((PrimitiveType)(int)primitiveType);
                                helpGO.transform.XSetLocalScale(scaleValue);
                                helpGO.GetComponent<MeshRenderer>().XSetEnable(alwaysRender);
                                break;
                            }
                    }

                    if (helpGO)
                    {
                        helpGO.XSetActive(isActive);
                        helpGO.transform.XSetPosition(positionValue);
                        switch (hierarchyType)
                        {
                            case EHierarchyType.Son:
                                {
                                    helpGO.XSetParent(currentGO.transform);
                                    break;
                                }
                            case EHierarchyType.Parent:
                                {
                                    helpGO.XSetParent(currentGO.transform.parent);
                                    currentGO.XSetParent(helpGO.transform);
                                    break;
                                }
                            case EHierarchyType.Brother:
                                {
                                    helpGO.XSetParent(currentGO.transform.parent);
                                    break;
                                }
                            case EHierarchyType.Root:
                            default:
                                {
                                    break;
                                }
                        }

                        if (renameWhenHasSameName) helpGO.XSetUniqueName(currentGO.name);
                        else helpGO.XSetName(currentGO.name);
                    }
                }
                EditorGUI.EndDisabledGroup();

                CommonFun.EndLayout();
            }

            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        public override void OnSceneGUI(SceneView sceneView)
        {
            if (renderbBounds)
            {
                Handles.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }

    /// <summary>
    /// 基础类型
    /// </summary>
    [Name("基础类型")]
    public enum EPrimitiveType
    {
        /// <summary>
        /// 球体
        /// </summary>
        [Name("球体")]
        Sphere = 0,

        /// <summary>
        /// 胶囊体
        /// </summary>
        [Name("胶囊体")]
        Capsule = 1,

        /// <summary>
        /// 圆柱体
        /// </summary>
        [Name("圆柱体")]
        Cylinder = 2,

        /// <summary>
        /// 长方体
        /// </summary>
        [Name("长方体")]
        Cube = 3,

        /// <summary>
        /// 平面
        /// </summary>
        [Name("平面")]
        Plane = 4,

        /// <summary>
        /// 四角面
        /// </summary>
        [Name("四角面")]
        Quad = 5,

        /// <summary>
        /// 空
        /// </summary>
        [Name("空")]
        Empty,

        /// <summary>
        /// Gizmos渲染器
        /// </summary>
        [Name("Gizmos渲染器")]
        GizmoRenderer,
    }

    /// <summary>
    /// 层级类型
    /// </summary>
    [Name("层级类型")]
    public enum EHierarchyType
    {
        /// <summary>
        /// 根级
        /// </summary>
        [Name("根级")]
        Root,

        /// <summary>
        /// 子级
        /// </summary>
        [Name("子级")]
        Son,

        /// <summary>
        /// 父级
        /// </summary>
        [Name("父级")]
        Parent,

        /// <summary>
        /// 兄级
        /// </summary>
        [Name("兄级")]
        Brother,
    }
}
