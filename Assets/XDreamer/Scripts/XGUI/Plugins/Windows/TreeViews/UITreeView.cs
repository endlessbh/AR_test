using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.TreeViews
{
    /// <summary>
    /// 树视图
    /// </summary>
    [Name("树视图")]
    [DisallowMultipleComponent]
    public class UITreeView : View
    {
        /// <summary>
        /// 节点模版
        /// </summary>
        [Group("节点配置")]
        [Name("节点模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UITreeNode templateNode;

        /// <summary>
        /// 节点展开图标
        /// </summary>
        [Name("节点展开图标")]
        public Sprite _expandIcon;

        /// <summary>
        /// 节点折叠图标
        /// </summary>
        [Name("节点折叠图标")]
        public Sprite _unexpandIcon;

        [Name("自动滚动")]
        [Tip("为了使选中项总是可见，需自动设定树型控件垂直滚动条！", "In order to make the selected item always visible, you need to automatically set the tree control vertical scroll bar!")]
        public bool autoScrollToShowSelectedNode = true;

        [Name("垂直滚动条")]
        [HideInSuperInspector(nameof(autoScrollToShowSelectedNode), EValidityCheckType.Equal, false)]
        [ComponentPopup]
        public Scrollbar scrollbar;

        [Group("样式")]
        [Name("使用编号")]
        public bool useIndex = true;

        [Name("编号分隔符")]
        [HideInSuperInspector(nameof(useIndex), EValidityCheckType.False)]
        public string separatorString = ".";

        [Name("缩进")]
        [Range(0,1000)]
        public float indent = 20;

        [Name("选中颜色")]
        public Color selectedColor = new Color(1, 0.6f, 0, 1);

        [Name("未选中颜色")]
        public Color unselectedColor = Color.white;

        public ITreeNodeGraph data;

        protected WorkObjectPool<UITreeNode> uiTreeNodePool = new WorkObjectPool<UITreeNode>();

        protected GridLayoutGroup gridLayoutGroup = null;

        protected int showChildrenCount = 0 ;

        protected void Awake()
        {
            if (templateNode) templateNode.gameObject.SetActive(false);

            uiTreeNodePool.Init(
                () => {
                    var newGO = GameObject.Instantiate(templateNode.gameObject, templateNode.transform.parent) as GameObject;
                    newGO.transform.localScale = templateNode.transform.localScale;
                    return newGO.GetComponent<UITreeNode>();
                },
                uiTreeNode => uiTreeNode.gameObject.SetActive(true),
                uiTreeNode => uiTreeNode.ResetData(),
                uiTreeNode => uiTreeNode
                );

            gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        }

        protected void Start() { }

        public void Create()
        {
            if (data == null)
            {
                Debug.LogWarning("树形控件没有数据源!");
                return;
            }

            Create(data.children);

            if (gridLayoutGroup)
            {
                showChildrenCount = 0;
                for (int i = 0; i < gridLayoutGroup.transform.childCount; ++i)
                {
                    if (gridLayoutGroup.transform.GetChild(i).gameObject.activeSelf)
                    {
                        ++showChildrenCount;
                    }
                }
            }
        }

        protected void Create(ITreeNodeGraph[] treeNodes, UITreeNode parent = null)
        {
            for (int i = 0; i < treeNodes.Length; ++i)
            {
                Create(treeNodes[i], parent, i + 1);
            }
        }

        protected void Create(ITreeNodeGraph node, UITreeNode parent, int index)
        {
            // 绘制根
            var uiTreeNode = uiTreeNodePool.Alloc();
            uiTreeNode.InitData(this, node, parent, index);
            uiTreeNode.transform.SetAsLastSibling();

            // 绘制子节点
            for (int i = 0; i < node.children.Length; ++i)
            {
                Create(node.children[i], uiTreeNode, i + 1);
            }
        }

        public void AutoMoveScrollBar(int index)
        {
            if (autoScrollToShowSelectedNode && gridLayoutGroup && scrollbar)
            {
                // 为了显示步骤滚动条的值， 当滚动条比值大，才滚动， 滚动条值从1到0变化
                //XGUIHelper.GetScrollBarRange(index, showChildrenCount, rectTransform.sizeDelta.y, 
                //    gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y, out float maxValue, out float minValue);

                //float value = Mathf.Clamp(scrollbar.value, minValue, maxValue);
                var value = 1.0f * (index - 1) / (showChildrenCount-1);

                if (scrollbar.direction == Scrollbar.Direction.BottomToTop)
                {
                    scrollbar.value = 1 - value;
                }
                else
                {
                    scrollbar.value = value;
                }
            }
        }
    }
}