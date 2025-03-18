using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Windows.TreeViews;

namespace XCSJ.EditorXGUI.Windows
{

    public static class UITreeViewEditor
    {
        public const string treeOpenIcon = "ArrowDown.png";
        public const string treeCloseIcon = "ArrowRight.png";

        public static GameObject CreateUITreeView(DefaultControls.Resources resources)
        {
            try
            {
                // 创建树            
                var root = EditorXGUIHelper.CreateScrollViewWithGridLayoutGroup();
                root.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 400);// XGUIOption.instance.treeViewSize;
                root.name = nameof(UITreeView);
                var uiTreeView = root.AddComponent<UITreeView>();
                uiTreeView._expandIcon = UICommonFun.LoadFromXDreamer<Sprite>(EFolder.EditorResources_Common_Icon, treeOpenIcon);
                uiTreeView._unexpandIcon = UICommonFun.LoadFromXDreamer<Sprite>(EFolder.EditorResources_Common_Icon, treeCloseIcon);
                uiTreeView.scrollbar = root.GetComponent<ScrollRect>().verticalScrollbar;

                // 设置网格布局组件
                var glg = root.GetComponentInChildren<GridLayoutGroup>();
                glg.cellSize = new Vector2(200, 40);// XGUIOption.instance.treeNodeSize;
                glg.spacing = new Vector2(0, 2/*XGUIOption.instance.treeNodeRowSpace*/);
                glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                glg.constraintCount = 1;

                // 设置内容适配组件
                var csf = root.GetComponentInChildren<ContentSizeFitter>();
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                //创建树节点
                uiTreeView.templateNode = CreateUITreeNode(resources);
                GameObjectUtility.SetParentAndAlign(uiTreeView.templateNode.gameObject, csf.gameObject);
                return root;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static UITreeNode CreateUITreeNode(DefaultControls.Resources resources)
        {
            try
            {
                var node = DefaultControls.CreateButton(resources);
                RectTransform treeNodeRect = node.GetComponent<RectTransform>();
                treeNodeRect.anchorMin = new Vector2(0, 1);
                treeNodeRect.anchorMax = new Vector2(0, 1);
                treeNodeRect.sizeDelta = new Vector2(200, 40);// XGUIOption.instance.treeNodeSize;
                UITreeNode uiTreeNode = node.AddComponent<UITreeNode>();
                uiTreeNode.backgroundButton = node.GetComponent<Button>();

                uiTreeNode.dsplayInfoText = node.GetComponentInChildren<Text>();
                uiTreeNode.dsplayInfoText.alignment = TextAnchor.MiddleLeft;
                uiTreeNode.dsplayInfoText.GetComponent<RectTransform>().offsetMin = new Vector3(30, 0);

                // 创建树的展开控件
                var toggle = DefaultControls.CreateToggle(resources);
                GameObjectUtility.SetParentAndAlign(toggle, node);
                RectTransform toggleRect = toggle.GetComponent<RectTransform>();
                toggleRect.anchorMin = new Vector2(0, 0.5f);
                toggleRect.anchorMax = new Vector2(0, 0.5f);
                toggleRect.pivot = new Vector2(0.5f, 0.5f);
                toggleRect.sizeDelta = new Vector2(20, 20);
                toggleRect.offsetMin = new Vector2(10, 0);

                uiTreeNode.expandedToggle = toggle.GetComponent<Toggle>();
                // 销毁多余的游戏对象
                GameObject.DestroyImmediate(uiTreeNode.expandedToggle.graphic.gameObject);
                GameObject.DestroyImmediate(uiTreeNode.expandedToggle.GetComponentInChildren<Text>().gameObject);

                return node.GetComponent<UITreeNode>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}
