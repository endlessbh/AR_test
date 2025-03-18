using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI;
using XCSJ.EditorXGUI.Windows;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginART;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginRepairman.States.Study;
using XCSJ.PluginRepairman.UI;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 分类名
        /// </summary>
        public const string CategoryName = RepairmanHelper.PluginName;

        #region 拆装步骤树

        public const string RepairStepTreeViewName = "拆装步骤列表";

        /// <summary>
        /// 创建拆装步骤树
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(RepairStepTreeViewName)]
        [XCSJ.Attributes.Icon(EIcon.Task)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairStepGroup))]
        public static void CreateUITaskWorkTreeView(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("拆装/拆装步骤列表.prefab"));
        }

        /// <summary>
        /// 创建计划步骤树
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("计划步骤树")]
        [XCSJ.Attributes.Icon(EIcon.Task)]
        [RequireManager(typeof(RepairmanManager))]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true)]
        [Manual(typeof(Plan))]
        public static void CreateUIPlanTreeView(ToolContext toolContext)
        {
            var go = CreateUITreeView("计划步骤树界面");
            if (go)
            {
                go.XAddComponent<UITreeViewPlanData>();
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateUITreeView(string name)
        {
            var go = UITreeViewEditor.CreateUITreeView(EditorXGUIHelper.defaultResources);
            go.XSetName(name);
            return go;
        }

        #endregion

        #region 零件列表

        public const string PartListName = "零件列表";

        /// <summary>
        /// 创建零件列表
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(PartListName)]
        [XCSJ.Attributes.Icon(nameof(Part))]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairTaskWorkPartView))]
        public static void CreatePartList(ToolContext toolContext)
        {
            var go = CreatePartScrollView(EditorXGUIHelper.defaultResources);
            go.XSetName(PartListName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreatePartScrollView(DefaultControls.Resources resources)
        {
            try
            {
                var repairmanOption = RepairmanExtentionsOption.instance;
                var partList = EditorXGUIHelper.CreateScrollView<GUIPartList>(resources,
                    repairmanOption.partListSize, repairmanOption.partItemSize, repairmanOption.CellSpaceSize);

                //创建零件单元
                partList.itemButtonPrefab = CreatePartCell(resources);
                GameObjectUtility.SetParentAndAlign(partList.itemButtonPrefab.gameObject,
                    partList.GetComponentInChildren<ContentSizeFitter>().gameObject);
                return partList.gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static GameObject CreatePartCell(DefaultControls.Resources resources)
        {
            try
            {
                return DefaultControls.CreateButton(resources).AddComponent<GUIPartButton>().gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region 工具包

        public const string ToolBagName = "工具包";

        /// <summary>
        /// 创建工具包
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ToolBagName)]
        [XCSJ.Attributes.Icon(nameof(Bag))]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(PluginRepairman.States.Tool))]
        public static void CreateToolBag(ToolContext toolContext)
        {
            var go = CreateToolBagScrollView(EditorXGUIHelper.defaultResources);
            go.XSetName(ToolBagName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateToolBagScrollView(DefaultControls.Resources resources)
        {
            try
            {
                var repairmanOption = RepairmanExtentionsOption.instance;
                var toolList = EditorXGUIHelper.CreateScrollView<GUIToolList>(resources,
                    repairmanOption.toolBagSize, repairmanOption.toolItemSize, repairmanOption.CellSpaceSize);

                //创建工具单元
                toolList.itemButtonPrefab = CreateToolCell(resources);
                GameObjectUtility.SetParentAndAlign(toolList.itemButtonPrefab.gameObject,
                    toolList.GetComponentInChildren<ContentSizeFitter>().gameObject);
                return toolList.gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static GameObject CreateToolCell(DefaultControls.Resources resources)
        {
            try
            {
                var toggleGO = DefaultControls.CreateToggle(resources);
                toggleGO.AddComponent<GUIItemToggle>();

                // 设置图标向四周扩展
                var bgRectTransform = toggleGO.transform.Find("Background").transform as RectTransform;
                bgRectTransform.XStretchHV();

                // 设置选中框向四周扩展
                var selectedRectTransform = toggleGO.transform.Find("Background/Checkmark").transform as RectTransform;
                selectedRectTransform.XStretchHV();

                // 设置文字属性
                var textRectTransform = toggleGO.GetComponentInChildren<Text>().transform as RectTransform;
                textRectTransform.offsetMin = new Vector2(0f, -20f);
                textRectTransform.offsetMax = new Vector2(0f, -60f);

                return toggleGO;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region 学习提示信息

        public const string StudyTipInfoName = "学习提示信息";

        /// <summary>
        /// 创建学习提示信息
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(StudyTipInfoName)]
        [XCSJ.Attributes.Icon(EIcon.Study)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIStudyTipInfo))]
        public static void CreateStudyTipInfo(ToolContext toolContext)
        {
            var go = DefaultControls.CreateText(EditorXGUIHelper.defaultResources);
            go.XAddComponent<UIStudyTipInfo>();
            go.XSetName(StudyTipInfoName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        #endregion

        #region 答题表格

        public const string QuestionTableName = "答题表格";

        /// <summary>
        /// 创建答题表格
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(QuestionTableName)]
        [XCSJ.Attributes.Icon(EIcon.AnswerQuestion)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIQuestionTableRepairExamData))]
        public static void CreateExamUIQuestionTable(ToolContext toolContext)
        {
            var go = CreateUIQuestionTable(EditorXGUIHelper.defaultResources);
            go.XSetName(QuestionTableName);
            go.XAddComponent<UIQuestionTableRepairExamData>();

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateUIQuestionTable(DefaultControls.Resources resources)
        {
            try
            {
                var repairmanOption = RepairmanTeachingOption.instance;
                var table = EditorXGUIHelper.CreateScrollView<UIQuestionTable>(resources,
                    repairmanOption.questionTableSize, repairmanOption.questionCellSize, repairmanOption.CellSpaceSize);

                //创建答题单元格
                table.questionCellTemplate = CreateUIQuestionCell(resources);
                GameObjectUtility.SetParentAndAlign(table.questionCellTemplate.gameObject,
                    table.GetComponentInChildren<ContentSizeFitter>().gameObject);
                return table.gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static UIQuestionCell CreateUIQuestionCell(DefaultControls.Resources resources)
        {
            try
            {
                GameObject btn = DefaultControls.CreateButton(resources);
                UIQuestionCell questionCell = btn.AddComponent<UIQuestionCell>();
                questionCell.questionText = btn.GetComponentInChildren<Text>();
                return questionCell;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region 考试提示信息

        public const string ExamTipInfoName = "考试提示信息";

        /// <summary>
        /// 创建考试提示
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ExamTipInfoName)]
        [XCSJ.Attributes.Icon(EIcon.Exam)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIExamInfo))]
        public static void CreateExamTipInfo(ToolContext toolContext)
        {
            var go = DefaultControls.CreateText(EditorXGUIHelper.defaultResources);
            go.XAddComponent<UIExamInfo>();
            go.XSetName(ExamTipInfoName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        #endregion

        #region 考试结果

        public const string ExamResultName = "考试结果";

        /// <summary>
        /// 创建考试结果
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(category = CategoryName, purposes = new string[] { nameof(RepairmanManager), nameof(XGUIManager) }, rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ExamResultName)]
        [XCSJ.Attributes.Icon(EIcon.Exam)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairExam))]
        public static void CreateExamResult(ToolContext toolContext)
        {
            var go = CreateUIRepairExamResult();
            go.XSetName(ExamResultName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateUIRepairExamResult()
        {
            // 背景框
            var root = DefaultControls.CreatePanel(EditorXGUIHelper.defaultResources);
            root.name = ExamResultName;
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = rootRect.anchorMin;
            rootRect.pivot = rootRect.anchorMin;
            rootRect.sizeDelta = new Vector2(300, 300);
            var examResult = root.AddComponent<UIExamResult>();

            // 主标题
            var title = DefaultControls.CreateText(EditorXGUIHelper.defaultResources).GetComponent<Text>();
            GameObjectUtility.SetParentAndAlign(title.gameObject, root);
            title.name = "主标题";
            title.text = "考试结果";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 120);
            title.alignment = TextAnchor.MiddleCenter;

            // 得分
            var getScore = DefaultControls.CreateText(EditorXGUIHelper.defaultResources).GetComponent<Text>();
            GameObjectUtility.SetParentAndAlign(getScore.gameObject, root);
            getScore.name = "得分";
            getScore.text = "得分";
            getScore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90);
            examResult.getScore = getScore;

            // 总分
            var totalScore = DefaultControls.CreateText(EditorXGUIHelper.defaultResources).GetComponent<Text>();
            GameObjectUtility.SetParentAndAlign(totalScore.gameObject, root);
            totalScore.name = "总分";
            totalScore.text = "总分";
            totalScore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60);
            examResult.totalScore = totalScore;

            // 考试判断结果
            var result = DefaultControls.CreateText(EditorXGUIHelper.defaultResources).GetComponent<Text>();
            GameObjectUtility.SetParentAndAlign(result.gameObject, root);
            result.name = "考试结果";
            result.text = "考试结果";
            result.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            examResult.result = result;

            // 错误步骤列表
            var scrollViewGo = DefaultControls.CreateScrollView(EditorXGUIHelper.defaultResources);
            GameObjectUtility.SetParentAndAlign(scrollViewGo, root);
            scrollViewGo.name = "错误步骤列表";
            var scrollView = scrollViewGo.GetComponent<RectTransform>();
            scrollView.sizeDelta = new Vector2(200, 160);
            scrollView.anchoredPosition = new Vector2(0, -60);
            Transform content = scrollViewGo.transform.Find("Viewport/Content");
            var sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            var errorText = DefaultControls.CreateText(EditorXGUIHelper.defaultResources).GetComponent<Text>();
            GameObjectUtility.SetParentAndAlign(errorText.gameObject, content.gameObject);
            examResult.errorsDetailDescription = errorText;

            return root;
        }

        #endregion
    }
}

