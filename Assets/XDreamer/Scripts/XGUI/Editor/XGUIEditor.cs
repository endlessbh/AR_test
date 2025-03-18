using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.Layouts;
using XCSJ.Extension.Base.Helpers;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorXGUI
{
    [Name(Title)]
    [Tip(Title, "XGUI Editor")]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    //[XDreamerEditorWindow(nameof(TrHelper.Common), index = 7)]
    public sealed class XGUIEditor : XEditorWindowWithScrollView<XGUIEditor>
    {
        public const string Title = "XGUI编辑器";

        [MenuItem(XDreamerMenu.EditorWindow + Title)]
        public static void Init()
        {
            OpenAndFocus();
            XGUIEditor.instance.minSize = new Vector2(50, 50);
        }

        #region Unity回调

        protected override void OnEnable()
        {
            base.OnEnable();

            InitData();

            Preferences.onOptionModified += OnOptionModified;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Preferences.onOptionModified -= OnOptionModified;
        }

        public override void OnSceneGUI(SceneView sceneView)
        {
            var color = Handles.color;
            if (standardRectTransform1)
            {
                Handles.color = LayoutOption.weakInstance.standardColor1;
                Handles.DrawWireCube(standardRectTransform1.position, standardRectTransform1.sizeDelta);
            }
            if (standardRectTransform2 && standardRectTransform1 != standardRectTransform2)
            {
                Handles.color = LayoutOption.weakInstance.standardColor2;
                Handles.DrawWireCube(standardRectTransform2.position, standardRectTransform2.sizeDelta);
            }
            Handles.color = color;

            SceneView.RepaintAll();
        }

        private void InitData()
        {
            _categoryList = CreateCategoryList();
        }

        public void OnSelectionChange()
        {
            if (Selection.activeTransform is RectTransform rt && rt)
            {
                standardRectTransform1 = rt;
            }
            else
            {
                standardRectTransform1 = null;
            }

            Repaint();
        }

        public void OnHierarchyChange()
        {
            Repaint();
        }

        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (buttonSizeOption == null)
                {
                    buttonSizeOption = new GUILayoutOption[] { GUILayout.Width(28), GUILayout.Height(28) };
                }

                //if (GUILayout.Button(CommonFun.NameTip(EIcon.List), buttonSizeOption))
                //{
                //    XGUILib.CreateWindow();
                //}

                if (GUILayout.Button(CommonFun.NameTip(EIcon.Config), buttonSizeOption))
                {
                    XDreamerPreferences.OpenWindow<XGUIOption>();
                }
            }
            EditorGUILayout.EndHorizontal();

            _categoryList.DrawVertical();
        }

        /// <summary>
        /// 选项修改回调
        /// </summary>
        /// <param name="option"></param>
        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);

        }

        #endregion

        #region 操作数据

        private CategoryList _categoryList = null;

        //private bool _displaySuperUI = false;

        [Name("使用完整选择集")]
        [Tip("勾选,对所有处于选择集中的游戏对象进行布局;不勾选，仅对选择集中激活游戏对象的子级游戏对象进行布局;", "Check to layout all game objects in the selection set; If unchecked, only the child game objects of the selected game objects will be laid out;")]
        public bool useFullSelection = true;

        [Name("当前矩形变换(只读)")]
        [Tip("当前游戏对象所属的矩形变换", "Rectangle transform to which the current GameObject belongs")]
        public RectTransform currentRectTransform = null;

        public RectTransform _standardRectTransform1 = null;

        [Name("起点矩形变换")]
        public RectTransform standardRectTransform1
        {
            get => _standardRectTransform1;
            set
            {
                _standardRectTransform1 = value;
            }
        }

        public RectTransform _standardRectTransform2 = null;

        [Name("终点矩形变换")]
        public RectTransform standardRectTransform2
        {
            get => _standardRectTransform2;
            set
            {
                _standardRectTransform2 = value;
            }
        }
        private List<RectTransform> selectedRectTransforms => Selection.transforms.Where(t => t is RectTransform).Cast(t => (RectTransform)t).ToList();

        private void ResetData()
        {
            standardRectTransform1 = standardRectTransform2 = null;
        }

        private GUILayoutOption[] buttonSizeOption = null;

        #endregion

        #region 对齐

        [Name("左对齐")]
        [Tip("以 标准矩形变换1 为基准进行 左对齐 操作", "Align left based on standard rectangle transform 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void LeftAlign()
        {
            selectedRectTransforms.LeftAlign(standardRectTransform1);
        }

        [Name("右对齐")]
        [Tip("以 标准矩形变换1 为基准进行 右对齐 操作", "Align right based on standard rectangle transform 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void RightAlign()
        {
            selectedRectTransforms.RightAlign(standardRectTransform1);
        }

        [Name("顶对齐")]
        [Tip("以 标准矩形变换1 为基准进行 顶对齐 操作", "The top alignment operation is based on the standard rectangular transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void TopAlign()
        {
            selectedRectTransforms.TopAlign(standardRectTransform1);
        }

        [Name("底对齐")]
        [Tip("以 标准矩形变换1 为基准进行 底对齐 操作", "The bottom alignment operation is based on the standard rectangular transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void BottomAlign()
        {
            selectedRectTransforms.BottomAlign(standardRectTransform1);
        }

        [Name("中心水平对齐")]
        [Tip("中心水平对齐,即中间对齐;以 标准矩形变换1 为基准进行 中心水平对齐 操作", "Center horizontal alignment, i.e. middle alignment; Align the center horizontally based on the standard rectangle transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void CenterHorizontalAlign()
        {
            selectedRectTransforms.CenterHorizontalAlign(standardRectTransform1);
        }

        [Name("中心垂直对齐")]
        [Tip("中心垂直对齐,即居中对齐;以 标准矩形变换1 为基准进行 中心垂直对齐 操作", "Align the center vertically, that is, align the center; Align the center vertically based on the standard rectangle transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("对齐")]
        public void CenterVerticalAlign()
        {
            selectedRectTransforms.CenterVerticalAlign(standardRectTransform1);
        }
        #endregion

        #region 间隔

        [Name("中心水平等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 中心水平等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the center horizontal equidistant linear interpolation operation")]
        [Attributes.Icon]
        [XCateogryAttribute("间隔")]
        public void CenterHorizontalSameSpace()
        {
            selectedRectTransforms.CenterHorizontalSameSpace(standardRectTransform1, standardRectTransform2);
        }

        [Name("中心垂直等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 中心垂直等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the center vertical equidistant linear interpolation operation")]
        [Attributes.Icon]
        [XCateogryAttribute("间隔")]
        public void CenterVerticalSameSpace()
        {
            selectedRectTransforms.CenterVerticalSameSpace(standardRectTransform1, standardRectTransform2);
        }

        [Name("边界水平等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 边界水平等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the boundary horizontal equidistant linear interpolation operation")]
        [Attributes.Icon]
        [XCateogryAttribute("间隔")]
        public void BoundsHorizontalSameSpace()
        {
            selectedRectTransforms.BoundsHorizontalSameSpace(standardRectTransform1, standardRectTransform2);
        }

        [Name("边界垂直等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 边界垂直等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the boundary vertical equidistant linear interpolation operation")]
        [Attributes.Icon]
        [XCateogryAttribute("间隔")]
        public void BoundsVerticalSameSpace()
        {
            selectedRectTransforms.BoundsVerticalSameSpace(standardRectTransform1, standardRectTransform2);
        }
      
        #endregion

        #region 尺寸

        [Name("等宽")]
        [Tip("以 标准矩形变换1 为基准进行 等宽 操作", "Equal width operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("尺寸")]
        public void SameWidth()
        {
            selectedRectTransforms.SameWidth(standardRectTransform1);
        }

        [Name("等高")]
        [Tip("以 标准矩形变换1 为基准进行 等高 操作", "Perform contour operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("尺寸")]
        public void SameHeight()
        {
            selectedRectTransforms.SameHeight(standardRectTransform1);
        }

        [Name("等尺寸")]
        [Tip("等尺寸,即等宽高;以 标准矩形变换1 为基准进行 等尺寸 操作", "Equal size, i.e. equal width and height; Equal size operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        [XCateogryAttribute("尺寸")]
        public void SameSize()
        {
            selectedRectTransforms.SameSize(standardRectTransform1);
        }

        //[Name("递增宽")]
        //[Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增宽 线性补间操作")]
        //[Attributes.Icon]
        //[XCateogryAttribute("尺寸")]
        //public void IncreaseWidth()
        //{
        //    selectedRectTransforms.IncreaseWidth(standardRectTransform1, standardRectTransform2);
        //}

        //[Name("递增高")]
        //[Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增高 线性补间操作")]
        //[Attributes.Icon]
        //[XCateogryAttribute("尺寸")]
        //public void IncreaseHeight()
        //{
        //    selectedRectTransforms.IncreaseHeight(standardRectTransform1, standardRectTransform2);
        //}

        //[Name("递增尺寸")]
        //[Tip("递增尺寸,即分别递增宽高;以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增尺寸 线性补间操作")]
        //[Attributes.Icon]
        //[XCateogryAttribute("尺寸")]
        //public void IncreaseSize()
        //{
        //    selectedRectTransforms.IncreaseSize(standardRectTransform1, standardRectTransform2);
        //}

        //[Name("方向重置")]
        //[Tip("将所有矩形变换的方向重置")]
        //[Attributes.Icon]
        //[XCateogryAttribute("尺寸")]
        //public void DirectionReset()
        //{
        //    selectedRectTransforms.DirectionReset();
        //}

        #endregion

        #region 高级布局

        //public List<IRectTransformLayoutWindow> layouts = new List<IRectTransformLayoutWindow>();

        public void ShowOtherLayout()
        {
            //CommonFun.BeginLayout();
            //var ts = effectiveRectTransforms;
            //foreach (var l in layouts)
            //{
            //    if (l.expanded = UICommonFun.Foldout(l.expanded, CommonFun.NameTooltip(l.GetType())))
            //    {
            //        try
            //        {
            //            CommonFun.BeginLayout();
            //            undo.Record(ts, () => l.OnGUI(effectiveRectTransforms, standardRectTransform1, standardRectTransform2));
            //        }
            //        finally
            //        {
            //            CommonFun.EndLayout();
            //        }
            //    }//end expanded
            //}//end foreach
            //CommonFun.EndLayout();
        }

        #endregion


        /// <summary>
        /// 获取当前类对象方法
        /// </summary>
        /// <returns></returns>
        private CategoryList CreateCategoryList()
        {
            var categoryList = new CategoryList();

            foreach (var methodInfo in this.GetType().GetMethods())
            {
                if (!methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
                {
                    if (Attribute.GetCustomAttribute(methodInfo, typeof(XCateogryAttribute)) is XCateogryAttribute xguiAttribute)
                    {
                        categoryList.AddItem(xguiAttribute.category, xguiAttribute.categoryIndex, new XGUIEditorWindowItem(xguiAttribute, methodInfo));
                    }
                }
            }

            return categoryList;
        }

        /// <summary>
        /// XGUI编辑器窗口项
        /// </summary>
        public class XGUIEditorWindowItem : BaseItem
        {
            /// <summary>
            /// XDreamer编辑器窗口项
            /// </summary>
            /// <param name="indexAttribute"></param>
            /// <param name="memberInfo"></param>
            public XGUIEditorWindowItem(IndexAttribute indexAttribute, MemberInfo memberInfo) : base(indexAttribute, memberInfo) 
            {
                _methodInfo = memberInfo as MethodInfo;
            }
            
            private MethodInfo _methodInfo;
            

            /// <summary>
            /// 点击
            /// </summary>
            public override void OnClick()
            {
                _methodInfo.Invoke(XGUIEditor.instance, null);
            }
        }
    }

 
}