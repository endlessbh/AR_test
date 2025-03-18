using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginXGUI.Windows.TreeViews
{
    /// <summary>
    /// 树节点
    /// </summary>
    [Name("树节点")]
    [DisallowMultipleComponent]
    public class UITreeNode : View, ITreeNodeGraph
    {
        [Name("折叠切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle expandedToggle;

        [Name("显示文本信息")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text dsplayInfoText;

        [Name("背景点击按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button backgroundButton;

        #region 树相关数据

        private UITreeView treeView;

        private ITreeNodeGraph data;

        private UITreeNode parentNode;

        private List<UITreeNode> childrenNodes = new List<UITreeNode>();

        public int index { get; set; } = 0;

        #endregion

        protected void Start()
        {
            if (parentNode)
            {
                parentNode.childrenNodes.Add(this);
            }

            //if (dsplayInfoText)
            //{
            //    dsplayInfoText.text = displayName;
            //}

            if (backgroundButton)
            {
                backgroundButton.onClick.AddListener(OnClick);
            }

            if (expandedToggle)
            {
                expandedToggle.onValueChanged.AddListener(OnExpanded);
            }
        }

        public void Update()
        {
            if (data == null || treeView==null ) return;

            if (dsplayInfoText)
            {
                dsplayInfoText.text = displayName;
            }

            // 树项选择状态不一致
            if (backgroundButton && selected != data.selected)
            {
                selected = data.selected;
                if (selected)
                {
                    XGUIHelper.SetColor(backgroundButton, treeView.selectedColor);

                    // 只响应叶子节点滚动
                    if (childrenNodes.Count == 0)
                    {
                        treeView.AutoMoveScrollBar(transform.GetSiblingIndex());
                    }                    
                }
                else
                {
                    XGUIHelper.SetColor(backgroundButton, treeView.unselectedColor);
                }
            }

            // 检查接口数据和toggle状态，让它们保持一致
            if (expandedToggle && this.expanded != expandedToggle.isOn)
            {
                expandedToggle.isOn = this.expanded;
            }

            // 设置精灵
            if (data is ITexture2D texture2D && texture2D.texture2D)
            {
                var img = backgroundButton.GetComponent<Image>();
                if (img)
                {
                    img.sprite = texture2D.texture2D.ToSprite();
                }
            }
        }

        public void ResetData()
        {
            XGUIHelper.SetColor(backgroundButton, treeView.unselectedColor);
        }

        public void InitData(UITreeView treeView, ITreeNodeGraph data, UITreeNode parentNode, int index)
        {
            this.treeView = treeView;
            this.data = data;
            this.parentNode = parentNode;
            this.index = index;

            if (expandedToggle)
            {
                MoveRight(expandedToggle.transform);
                if (data.children.Length==0)
                {
                    expandedToggle.gameObject.SetActive(false);
                }
            }

            if (dsplayInfoText)
            {
                MoveRight(dsplayInfoText.transform);
            }

            OnExpanded(true);
        }     
        
        /// <summary>
        /// 向右缩进
        /// </summary>
        private void MoveRight(Transform moveTransform)
        {
            float indent = treeView ? treeView.indent : 0;
            moveTransform.Translate(indent * (depth - 1), 0, 0);
        }

        public string GetIndex()
        {
            return (parentNode ? parentNode.GetIndex() + "." : "") + index;
        }

        public void OnExpanded(bool expanded)
        {
            this.expanded = expanded;
            if (expandedToggle && expandedToggle.image && treeView)
            {
                expandedToggle.image.sprite = this.expanded ? treeView._expandIcon : treeView._unexpandIcon;
            }
            SetChildExpanded(this.expanded);
        }

        public void SetChildExpanded(bool expanded)
        {
            childrenNodes.ForEach(n =>
            {
                // 父展开，子随着自己属性控制孙节点
                if (expanded)
                {
                    n.SetChildExpanded(n.expanded);
                }
                // 父折叠，子孙必须折叠
                else
                {
                    n.SetChildExpanded(expanded);
                }
                n.gameObject.SetActive(expanded);
            });
        }

        #region 树节点接口

        public GUIContent display => null;

        public bool enable { get; set; } = true;

        public bool visible { get; set; } = true;

        public int depth => data.depth;

        public bool expanded { get { return data.expanded;} set { data.expanded = value; } }

        public string displayName => (treeView.useIndex) ? (GetIndex()+ treeView.separatorString + data?.displayName) : data?.displayName;

        ITreeNodeGraph ITreeNodeGraph.parent => parentNode;

        ITreeNodeGraph[] ITreeNodeGraph.children => childrenNodes.ToArray();

        ITreeNode ITreeNode.parent => parentNode;

        ITreeNode[] ITreeNode.children => childrenNodes.ToArray();

        public bool selected { get; set; } = false;

        public void OnClick()
        {
            data?.OnClick();
        }

        #endregion
    }
}

