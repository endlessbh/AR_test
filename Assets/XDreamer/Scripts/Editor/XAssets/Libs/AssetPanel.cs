using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class AssetPanel : BasePanel
    {

        #region 变量及构造函数

        /// <summary>
        /// 资源信息
        /// </summary>
        private Item asset;

        public Item Asset => asset;

        /// <summary>
        /// 文本框高度
        /// </summary>
        private float textBoxHeight = 30f;

        /// <summary>
        /// 标签文本
        /// </summary>
        private string tagText = "";

        GUIStyle titleSytle, tagStyle;

        private string source;

        private string name;

        private string nameTip;

        private string tagTip;

        private string iconTip;

        private string tip;

        private string assetType;

        private Texture typeIcon => asset.typeIcon;

        private Texture thumbnail => asset.thumbnail;

        public AssetPanel(Item asset)
        {
            this.asset = asset;
            ResetInfo();
        }

        public void ResetInfo()
        {
            tip = EditorPath.IsScene(asset.assetPath) || EditorPath.IsUnityPackage(asset.assetPath) ? "双击打开" : "单击选择";
            source = asset.source;
            name = asset.name;
            nameTip = string.Format("名称：{0}\n来源：{1}", asset.name, (string.IsNullOrEmpty(source) ? "无" : source));
            if (asset.tags != null && asset.tags.Count > 0) tagText = string.Join("、", this.asset.tags.ToArray());
            tagTip = string.Format("标签:{0}", this.tagText);
            iconTip = string.Format(" 资源类型：{0}", (string.IsNullOrEmpty(asset.assetType) ? asset.asset.GetType().ToString() : asset.assetType));
            assetType = asset.assetType;
        }

        #endregion 变量及构造函数

        #region BasePanel

        void InitStyle()
        {
            //标题Style
            titleSytle = new GUIStyle();
            titleSytle.normal.textColor = EditorStyles.label.normal.textColor;
            titleSytle.alignment = TextAnchor.MiddleLeft;
            titleSytle.padding = new RectOffset(3,0,0,0);
            titleSytle.fontSize = 16;
            titleSytle.stretchWidth = false;
            titleSytle.fontStyle = FontStyle.Bold;


            //标签Style
            tagStyle = new GUIStyle();
            tagStyle.normal.textColor = EditorStyles.label.normal.textColor;
            tagStyle.alignment = TextAnchor.MiddleLeft;
            tagStyle.padding = new RectOffset(5, 0, 0, 0);
        }

        /// <summary>
        /// 资源单元格渲染
        /// </summary>
        /// <param name="rPos"></param>
        public void Render(Rect rPos, bool recieveMouseEvent = true, float startHeight = 0, EPlaceType placeType = EPlaceType.Grid , GUIStyle backstyle = null, int index = 0)
        {
            if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) return;
            if (asset.tags != null && asset.tags.Count > 0) tagText = string.Join("、", this.asset.tags.ToArray());
            InitStyle();

            GUI.BeginGroup(rPos);

            if (placeType == EPlaceType.Grid)
            {
                DrawHorizontal(rPos);
            }
            else if(placeType == EPlaceType.List)
            {
                DrawVertical(rPos, backstyle, index);
            }

            GUI.EndGroup();

            Event events = Event.current;

            GUIStyle tipStyle = GUI.skin.box;
            tipStyle.alignment = TextAnchor.MiddleLeft;
            tipStyle.padding = new RectOffset(5,5,2,3);
            tipStyle.normal.textColor = EditorStyles.label.normal.textColor;

            if (!recieveMouseEvent) return;

            //拖拽资源事件处理
            EventType currentEventType = events.type;

            if (rPos.Contains(events.mousePosition) && events.mousePosition.y > startHeight) 
            {
                if (currentEventType == EventType.MouseUp && events.button == 0 && events.clickCount == 1)
                {
                    OnClick(events);
                }

                if (currentEventType == EventType.MouseDown && events.button == 0 && events.clickCount == 2)
                {
                    OnDoubleClick(events);
                }

                if (currentEventType == EventType.MouseUp && events.button == 1)
                {
                    OnMouseRightClick(events);
                }

                if(events.button == 0 && currentEventType == EventType.MouseDrag)
                {
                    OnDrag(events);
                }

                if (events.button == 0 && currentEventType == EventType.DragExited)
                {
                    OnDragEnd(events);
                }
            }
        }

        void DrawHorizontal(Rect rPos)
        {
            //项绘制窗口
            Rect rect = new Rect(rPos);
            rect.height = rect.height - 10;
            rect.width = rect.width - 10;
            rect.x = 5;
            rect.y = 5;

            //开始绘制，自动绘制
            GUILayout.BeginArea(rect, GUI.skin.box);
            GUILayout.BeginVertical();
            GUILayout.Box(CommonFun.TempContent("", tip), AssetLibWindowStyle.instance.transparentStyle, GUILayout.ExpandHeight(true), GUILayout.Width(rect.width));

            GUILayout.BeginHorizontal(GUILayout.Height(30));

            string tempName = CalculateText(this.name, titleSytle, titleSytle.fontSize,  (int)(rect.width - textBoxHeight - 5));
            GUILayout.Label(CommonFun.TempContent(tempName, nameTip), titleSytle, GUILayout.Width(rect.width - textBoxHeight - 5), GUILayout.Height(textBoxHeight));

            GUILayout.Label(CommonFun.TempContent(asset.typeIcon, iconTip), AssetLibWindowStyle.instance.transparentStyle, GUILayout.Height(textBoxHeight), GUILayout.Width(textBoxHeight-5));

            GUILayout.Space(2);
            GUILayout.EndHorizontal();

            GUILayout.Label(CommonFun.TempContent(this.tagText, tagTip), tagStyle, GUILayout.Height(20));

            GUILayout.EndVertical();

            GUILayout.EndArea();

            //绘制贴图，Layout布局，暂未找到缩放相应方法
            Rect rectImage = new Rect(10f, 10f, rect.width-10, rect.height - 60);
            GUI.DrawTexture(rectImage, thumbnail, ScaleMode.ScaleToFit);

            //绘制编辑按钮
            if (AssetLibWindow.instance.inEdit)
            {
                Rect topArea = new Rect(5, 5, rect.width, 30);
                GUILayout.BeginArea(topArea);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //json存在与否
                if (asset.jsonFileExists) GUILayout.Label(AssetLibWindowStyle.instance.jsonTexture, GUILayout.Width(30), GUILayout.Height(30));
                else GUILayout.Label(AssetLibWindowStyle.instance.jsonNonExistTexture, GUILayout.Width(30), GUILayout.Height(30));
                GUILayout.Space(5);
                //编辑按钮
                if (GUILayout.Button(GUIContent.none, AssetLibWindowStyle.instance.editStyle, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    CommonFun.FocusControl();
                    AssetLibWindow.instance.CallEditModel(asset);
                }
                //可见按钮
                GUILayout.Space(5);
                GUIStyle visiableStyle = asset.visable ? AssetLibWindowStyle.instance.visiableStyle : AssetLibWindowStyle.instance.nonVisiableStyle;
                if (GUILayout.Button(GUIContent.none, visiableStyle, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    CommonFun.FocusControl();
                    asset.MarkDirty(true);
                    asset.visable = !asset.visable;
                    AssetLibWindow.instance.CallVisibleModelChange(asset);
                }
                //关闭按钮
                GUILayout.Space(5);
                //if (GUILayout.Button(GUIContent.none, AssetLibWindowStyle.instance.closeStyle, GUILayout.Width(30), GUILayout.Height(30)))
                //{
                //    CommonFun.FocusControl();
                //    asset.MarkDirty(true);
                //    asset.Delete();
                //}
                GUILayout.Space(2);
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        void DrawVertical(Rect rPos, GUIStyle backstyle = null, int index = 0)
        {
            //项绘制窗口
            Rect rect = new Rect(rPos);
            rect.height = rect.height;
            rect.width = rect.width - 15;
            rect.x = 0;
            rect.y = 0;

            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal(backstyle);
            {
                GUILayout.Label(index.ToString(), GUILayout.Width(UICommonOption._32), UICommonOption.Height16);
                GUILayout.Label(CommonFun.TempContent(" ", iconTip), GUILayout.Width(UICommonOption._48));

                GUILayout.Label(CommonFun.TempContent(name, nameTip), GUILayout.Width(UICommonOption._200), UICommonOption.Height16);

                string tag = CalculateText(tagText, GUI.skin.label, GUI.skin.label.fontSize, (int)(rect.width - 480 >= 36? rect.width - 480 : 36));
                GUILayout.Label(CommonFun.TempContent(tag, tagTip), GUILayout.ExpandWidth(true), UICommonOption.Height16);

                GUILayout.Label(assetType, GUILayout.Width(UICommonOption._100), UICommonOption.Height16);
                GUILayout.Label(asset.source, GUILayout.Width(UICommonOption._100), UICommonOption.Height16);
                if (AssetLibWindow.instance.inEdit)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(60));
                    {
                        if (asset.jsonFileExists) GUILayout.Label(AssetLibWindowStyle.instance.jsonTexture, UICommonOption.WH20x16);
                        else GUILayout.Label(AssetLibWindowStyle.instance.jsonNonExistTexture, UICommonOption.WH20x16);

                        //编辑状态下绘制编辑按钮
                        if (GUILayout.Button("", AssetLibWindowStyle.instance.editStyle, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            AssetLibWindow.instance.CallEditModel(asset);
                        }
                        GUIStyle visiableStyle = asset.visable ? AssetLibWindowStyle.instance.visiableStyle : AssetLibWindowStyle.instance.nonVisiableStyle;
                        if (GUILayout.Button("", visiableStyle, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            asset.MarkDirty(true);
                            asset.visable = !asset.visable;
                            AssetLibWindow.instance.CallVisibleModelChange(asset);
                        }
                        //if (GUILayout.Button("", AssetLibWindowStyle.instance.closeStyle, UICommonOption.WH20x16))
                        //{
                        //    CommonFun.FocusControl();
                        //    asset.MarkDirty(true);
                        //    asset.Delete();
                        //}
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();


            Rect rectImage = new Rect(40, 5, 24, 16);
            GUI.DrawTexture(rectImage, thumbnail, ScaleMode.ScaleToFit);
        }

        #endregion BasePanel

        #region 鼠标响应函数

        public virtual void OnDrag(Event eve)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            DragAndDrop.objectReferences = new Object[] { asset.asset };
            DragAndDrop.StartDrag("开始拖拽");
        }

        public virtual void OnDragEnd(Event eve)
        {
            DragAndDrop.PrepareStartDrag();
        }

        public virtual void OnClick(Event eve)
        {
            SelectionHelper.OnAssetSelected(asset.assetPath);
        }

        public virtual void OnDoubleClick(Event eve)
        {
            if (EditorPath.IsScene(asset.assetPath))
            {
                SelectionHelper.OnSceneSelected(asset.assetPath);
            }
            else if (EditorPath.IsUnityPackage(asset.assetPath))
            {
                SelectionHelper.OnPackageSelected(asset.assetPath);
            }
            ResetInfo();
        }

        public virtual void OnMouseRightClick(Event eve)
        {
        }

        #endregion 鼠标响应函数

        public string CalculateText(string text, GUIStyle style, int fontSize, int fullWidth)
        {
            if (style.CalcSize(new GUIContent(text)).x < fullWidth) return text;

            for (int i = text.Length - 1; i > 0; i--)
            {
                string str = text.Substring(0, i) + "…";
                if (style.CalcSize(new GUIContent(str)).x < fullWidth) return str;
            }
            return text;
        }
    }
}