using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;

namespace XCSJ.EditorExtension
{
    [XDreamerPreferences(index = IDRange.Begin + 55)]
    [Name(Product.Name + "-图标库")]
    [Import]
    public class XDreamerIconOption : XDreamerOption<XDreamerIconOption>
    {
        protected override int newVersion => -41;

        [Name("专业版皮肤图标标识")]
        public string professionalSkinIconMarker = "Pro";

        [Name("个人版皮肤图标标识")]
        public string personalSkinIconMarker = "Per";

        [Name("当前图集")]
        public string iconSetName = "";


        [Name("图标集")]
        public List<IconSet> iconSets = new List<IconSet>();

        private IconSet _currentIconSet;

        [Json(false)]
        public IconSet CurrentIconSet
        {
            get
            {
                InitIconSetsIfNeed();

                if (_currentIconSet == null)
                {
                    _currentIconSet = GetIconSet(iconSetName);
                    iconSetName = _currentIconSet?.name;
                }

                return _currentIconSet;
            }
            set
            {
                _currentIconSet = value;
                iconSetName = _currentIconSet?.name;
            }
        }

        [InitializeOnLoadMethod]
        public static void InitOption()
        {
            XDreamerEvents.onProjectAnyAssetsOrOptionChangedInEditor += ClearCache;
            UICommonFun.DelayCall(() => weakInstance.iconSets.ForEach(set => set.UpdateSearch()));
        }

        public static void ClearCache()
        {
            IconPathCache.Cache.Clear();
            weakInstance?.InitIconSetsIfNeed();
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitIconSetsIfNeed();
            UpdateIconSetName();
        }

        public override void OnModified()
        {
            base.OnModified();
            IconPathCache.Cache.Clear();
            UpdateIconSetName();
        }

        protected override void OnVersionChanged(int lastVersion)
        {
            //Debug.Log("OnVersionChanged: " + force);
            // 更新图标库
            foreach (var item in iconSets)
            {
                var infos = CreateIconInfos(item.name);

                infos.ForEach(info =>
                {
                    item.UpdateOrAddIconInfo(info, newVersion < 0);
                });
            }

            MarkDirty();
        }

        #region 图集数据操作

        public const string DefaultIconSetName = "默认";

        internal string[] iconSetNames = { };

        public void InitIconSetsIfNeed()
        {
            if (iconSets.Count == 0)
            {
                CreateIconSet();
            }
        }

        internal void CreateIconSet(string name = DefaultIconSetName)
        {
            _currentIconSet = new IconSet(name, CreateIconInfos(name));
            iconSets.Add(_currentIconSet);
            iconSetName = _currentIconSet.name;
        }

        internal IconSet GetIconSet(string name)
        {
            var tmp = iconSets.Find(i => i.name == name);
            if (tmp == null)
            {
                tmp = iconSets.FirstOrDefault();
            }

            return tmp;
        }

        private void UpdateIconSetName()
        {
            iconSetNames = iconSets.Cast(s => s.name).ToArray();
        }

        #endregion

        #region IconInfo创建

        public static string EditorResourcesPath { get; } = UICommonFun.GetAssetsPath(EFolder.EditorResources) + "/";

        public static string IconLibPath { get; } = UICommonFun.GetAssetsPath(EFolder.EditorResources_IconLib);

        public static List<IconInfo> CreateIconInfos(string iconSetName = DefaultIconSetName)
        {
            var infos = new List<IconInfo>();

            var commonInfos = CreateCommonIconInfos(iconSetName);
            infos.AddRange(commonInfos);

            var attInfos = CreateIconInfosWithAttribute(iconSetName);
            attInfos = attInfos.Where(a => !commonInfos.Exists(c => c.name == a.name)).ToList();
            infos.AddRange(attInfos);

            return infos;
        }

        private static List<IconInfo> CreateCommonIconInfos(string iconSetName = DefaultIconSetName)
        {
            // 创建文件夹
            var iconDirectory = typeof(EIcon).FullName.Replace('.', '/');
            var fullDirectory = ToFullPath(iconSetName, iconDirectory);

            // 枚举通用UI
            var infos = new List<IconInfo>();
            EnumHelper.Enums<EIcon>().ForEach(item =>
            {
                if (item != EIcon.None)
                {
                    string keyword = item.ToString();
                    string path = ToXDreamerPath(iconSetName, iconDirectory) + "/" + keyword + EditorIconHelper.DefaultIconExt;
                    infos.Add(new IconInfo((int)item, keyword, CommonFun.Name(item), path, fullDirectory));
                }
            });
            return infos;
        }

        private static List<IconInfo> CreateIconInfosWithAttribute(string iconSetName = DefaultIconSetName)
        {
            var infos = new List<IconInfo>();
            var iconMemberInfos = FindIconAttribute();
            foreach (var item in iconMemberInfos)
            {
                string relativeDirectory = item.relativeDirectory;
                if (!string.IsNullOrEmpty(relativeDirectory))
                {
                    string path = ToXDreamerPath(iconSetName, relativeDirectory) + "/" + item.fileName;
                    infos.Add(new IconInfo(item.iconAttribute.index, item.keyword, CommonFun.Name(item.memberInfo), path, ToFullPath(iconSetName, relativeDirectory)));
                }
            }
            return infos;
        }

        public static List<IconMemberInfo> FindIconAttribute()
        {
            var infos = new List<IconMemberInfo>();
            TypeHelper.GetTypes(t => t.IsInterface || (t.IsClass && t.IsPublic && !t.IsAbstract && !t.IsGenericType)).ForEach(type =>
            {
                var typeAttr = AttributeHelper.GetAttribute<Attributes.IconAttribute>(type);
                if (typeAttr != null)
                {
                    infos.Add(new IconMemberInfo(type, typeAttr));
                }

                type.GetMembers().Foreach(memberInfo =>
                {
                    var memberAttr = AttributeHelper.GetAttribute<Attributes.IconAttribute>(memberInfo);
                    if (memberAttr != null)
                    {
                        infos.Add(new IconMemberInfo(memberInfo, memberAttr));
                    }
                });
            });

            infos.Sort();

            infos = infos.Distinct(new IconMemberInfoCompareByKeyword()).ToList();

            infos.Sort();

            return infos;
        }

        public static string ToFullPath(string iconSetName, string relativeDirectory)
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + ToXDreamerPath(iconSetName, relativeDirectory);
        }

        public static string ToXDreamerPath(string iconSetName, string relativeDirectory)
        {
            return ToAssetsPath(relativeDirectory, IconLibPath + "/" + iconSetName + "/");
        }

        public static string ToAssetsPath(string path)
        {
            return ToAssetsPath(path, EditorResourcesPath);
        }

        public static string ToAssetsPath(string path, string prefix)
        {
            if (path.IndexOf('/') >= 0)
            {
                if (path.StartsWith(UICommonFun.Assets + "/") || path.StartsWith("/" + UICommonFun.Assets + "/"))
                {
                    return path;
                }
                else if (path.StartsWith(Product.Name + "/") || path.StartsWith("/" + Product.Name + "/"))
                {
                    return UICommonFun.Assets + "/" + path;
                }
            }
            return prefix + path;
        }

        public class IconMemberInfo : IComparable
        {
            public MemberInfo memberInfo;
            public Attributes.IconAttribute iconAttribute;

            public string keyword { get; private set; } = "";
            public string relativeDirectory { get; private set; } = "";
            public string fileName { get; private set; } = "";

            public IconMemberInfo(MemberInfo memberInfo, Attributes.IconAttribute iconAttribute)
            {
                this.memberInfo = memberInfo;
                this.iconAttribute = iconAttribute;

                Init();
            }

            private void Init()
            {
                // 关键字
                keyword = Attributes.IconAttribute.GetDefaultIcon(memberInfo);

                var icon = iconAttribute.icon;
                if (!string.IsNullOrEmpty(icon))
                {
                    if (Path.HasExtension(icon))
                    {
                        int index = icon.LastIndexOf('/');
                        if (index >= 0)
                        {
                            relativeDirectory = icon.Substring(0, index);
                        }
                        fileName = Path.GetFileName(keyword);
                        return;
                    }
                    else if (icon.IndexOf('/') >= 0)
                    {
                        Debug.LogError(memberInfo.ToString() + "使用Icon特性修饰的路径字符串必须包含后缀");
                        return;
                    }
                }
                var fullName = GetFullName();
                var length = fullName.LastIndexOf('.');
                if (length >= 0)
                {
                    relativeDirectory = fullName.Substring(0, length).Replace('.', '/');
                }
                else
                {
                    Debug.LogError("全名称:[" + fullName + "]对应图标初始化异常！");
                }

                fileName = keyword + EditorIconHelper.DefaultIconExt;
            }

            private string GetFullName()
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                    case MemberTypes.Method:
                        {
                            switch (iconAttribute.memberRule)
                            {
                                case EMemberRule.None: return memberInfo.ReflectedType.FullName + "." + memberInfo.Name;
                                case EMemberRule.DeclaringType:
                                case EMemberRule.DeclaringType_MemberName:
                                    {
                                        return memberInfo.DeclaringType.FullName;
                                    }
                                case EMemberRule.ReflectedType:
                                case EMemberRule.ReflectedType_MemberName:
                                    {
                                        return memberInfo.ReflectedType.FullName;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            if (memberInfo is Type type)
                            {
                                return type.FullName;
                            }
                            break;
                        }
                }
                return null;
            }

            public int CompareTo(object obj)
            {
                if (null == obj || !(obj is IconMemberInfo y))
                {
                    return 1;//空值比较大，返回1
                }

                IconMemberInfo x = this;

                // 比较关键字
                if (x.keyword != y.keyword)
                {
                    return x.keyword.CompareTo(y.keyword);
                }

                // 比较icon
                if (x.iconAttribute.icon != y.iconAttribute.icon)
                {
                    return x.iconAttribute.icon.CompareTo(y.iconAttribute.icon);
                }

                // 都是类型比较相对路径
                if (x.memberInfo is Type && y.memberInfo is Type)
                {
                    return x.relativeDirectory.CompareTo(y.relativeDirectory);
                }

                // 谁是类型，优先返回谁
                if (x.memberInfo is Type)
                {
                    return -1;
                }
                else if (y.memberInfo is Type)
                {
                    return 1;
                }
                else
                {
                    // 都不是类型，比较相对路径
                    return x.relativeDirectory.CompareTo(y.relativeDirectory);
                }
            }
        }

        public class IconMemberInfoCompareByKeyword : IEqualityComparer<IconMemberInfo>
        {
            public bool Equals(IconMemberInfo x, IconMemberInfo y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                if (x.keyword == y.keyword)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(IconMemberInfo obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    return obj.keyword.GetHashCode();
                }
            }
        }

        #endregion

        #region IconSet 定义

        public class IconSet
        {
            [Name("名称")]
            public string name = "";

            [Name("图标信息")]
            [Tip("图标信息列表", "Icon information list")]
            public List<IconInfo> iconInfos = new List<IconInfo>();

            private List<IconInfo> _drawIconInfos;
            [Json(false)]
            public List<IconInfo> drawIconInfos
            {
                get
                {
                    if (_drawIconInfos == null) _drawIconInfos = iconInfos;
                    return _drawIconInfos;
                }
                set => _drawIconInfos = value;
            }

            public bool expand = false;

            public IconInfo newIconInfo = new IconInfo();

            public IconInfo searchIconInfo = new IconInfo();

            public IconSet() { }
            public IconSet(string name, List<IconInfo> infos)
            {
                this.name = name;
                SetIconInfo(infos);
            }

            public void SetIconInfo(List<IconInfo> infos)
            {
                iconInfos.Clear();
                infos.ForEach(i => AddIconInfo(i));
                iconInfos.Sort((x, y) => x.name.CompareTo(y.name));
            }

            public void UpdateOrAddIconInfo(IconInfo iconInfo, bool updatePath)
            {
                var info = AddIconInfo(iconInfo);
                if (info != null)
                {
                    info.description = iconInfo.description;
                    info.id = iconInfo.id;
                    if (updatePath)
                    {
                        info.iconPath = iconInfo.iconPath;
                        DirectoryHelper.Create(iconInfo.fullDirectory);
                    }
                }
            }

            public IconInfo AddIconInfo(IconInfo iconInfo)
            {
                IconInfo newInfo = null;
                if (iconInfo.Valid)
                {
                    newInfo = iconInfos.Find(i => i.name == iconInfo.name);

                    if (newInfo == null)
                    {
                        // 创建目录
                        DirectoryHelper.Create(iconInfo.fullDirectory);
                        newInfo = new IconInfo(iconInfo);
                        iconInfos.Add(newInfo);
                    }
                }
                return newInfo;
            }

            public IconSet Clone()
            {
                return new IconSet(name, iconInfos) { expand = expand };
            }

            public void UpdateSearch()
            {
                drawIconInfos = iconInfos.Where(info => info.name.IndexOf(searchIconInfo.name, StringComparison.CurrentCultureIgnoreCase) >= 0
                 && info.description.IndexOf(searchIconInfo.description, StringComparison.CurrentCultureIgnoreCase) >= 0
                 && (searchIconInfo.id == 0 || info.id.ToString().IndexOf(searchIconInfo.id.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                 && info.iconPath.IndexOf(searchIconInfo.iconPath, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }

            public void ClearSearch()
            {
                searchIconInfo.id = 0;
                searchIconInfo.name = "";
                searchIconInfo.description = "";
                searchIconInfo.iconPath = "";
                _drawIconInfos = null;
            }
        }

        public class IconInfo
        {
            public int id = 0;
            public string name = "";
            public string description = "";
            public string iconPath = "";

            [Json(false)]
            public string fullDirectory { get; private set; } = "";

            public IconInfo() { }
            public IconInfo(IconInfo iconInfo) : this(iconInfo.id, iconInfo.name, iconInfo.description, iconInfo.iconPath, iconInfo.fullDirectory) { }

            public IconInfo(int id, string name, string description, string iconPath, string fullDirectory)
            {
                this.id = id;
                this.name = name;
                this.description = description;
                this.iconPath = iconPath;
                this.fullDirectory = fullDirectory;
            }

            public bool Valid
            {
                get
                {
                    return !(string.IsNullOrEmpty(description) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(iconPath));
                }
            }
        }

        public class IconPathCache : TICache<IconPathCache, string, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(string key1)
            {
                if (weakInstance?.CurrentIconSet?.iconInfos.FirstOrDefault(info => info.name == key1)?.iconPath is string iconPath)
                {
                    return new KeyValuePair<bool, string>(true, iconPath);
                }
                return new KeyValuePair<bool, string>(true, "");
            }

            public static string Get(string name)
            {
                if (string.IsNullOrEmpty(name)) return null;
                Cache.TryGetValue(name, out string iconPath, true);
                return iconPath;
            }
        }

        #endregion

        #region 皮肤图标标识

        public string GetSkinIconMarker()
        {
            return EditorGUIUtility.isProSkin ? professionalSkinIconMarker : personalSkinIconMarker;
        }

        #endregion
    }

    [CommonEditor(typeof(XDreamerIconOption))]
    public class XDreamerIconOptionEditor : XDreamerOptionEditor<XDreamerIconOption>
    {
        #region 界面绘制

        private const int IndexWidth = XDreamerInspector.IndexWidth;
        private const int NameWidth = UICommonOption._200;
        private const int DescriptionWidth = UICommonOption._100;
        private const int IDWidth = UICommonOption._60;
        private const int OperationWidth = UICommonOption._24 * 5;

        private string newIconSetName = "新图集";

        private void DrawCreateIconSetGUI(XDreamerIconOption option)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("图标集名称");

            newIconSetName = EditorGUILayout.DelayedTextField(newIconSetName, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("新建", GUILayout.Width(60)))
            {
                newIconSetName = GetUniqueNameInIconSets(option, newIconSetName);
                option.CreateIconSet(newIconSetName);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawIconSetDropdownList(XDreamerIconOption option)
        {
            EditorGUI.BeginChangeCheck();
            option.iconSetName = UICommonFun.Popup(CommonFun.NameTip(option, nameof(option.iconSetName)), option.iconSetName, option.iconSetNames, false);
            if (EditorGUI.EndChangeCheck())
            {
                option.CurrentIconSet = option.GetIconSet(option.iconSetName);
            }
        }

        private void DrawIconSets(XDreamerIconOption option)
        {
            //XEditorGUI.DrawList(iconSets,)
            for (int i = 0; i < option.iconSets.Count; i++)
            {
                var item = option.iconSets[i];
                bool iconSetsChanged = false;
                if (item.expand = UICommonFun.Foldout(item.expand, CommonFun.TempContent(item.name), true, EditorStyles.foldout, null, () =>
                {
                    if (GUILayout.Button(CommonFun.TempContent("R", "重置图标目录"), EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                    {
                        item.SetIconInfo(XDreamerIconOption.CreateIconInfos());
                        option.MarkDirty();
                    }

                    if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                    {
                        var newSet = item.Clone();
                        newSet.name = GetUniqueNameInIconSets(option, newSet.name);
                        option.iconSets.Insert(i + 1, newSet);
                        iconSetsChanged = true;
                    }

                    EditorGUI.BeginDisabledGroup(option.iconSets.Count == 1);
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
                    {
                        option.iconSets.RemoveAt(i);
                        iconSetsChanged = true;
                    }
                    EditorGUI.EndDisabledGroup();
                }))
                {
                    if (!iconSetsChanged)
                    {
                        CommonFun.BeginLayout();
                        DrawIconSet(item);
                        CommonFun.EndLayout();
                    }
                }
            }
        }

        private void DrawIconSet(XDreamerIconOption.IconSet iconSet)
        {
            UICommonFun.BeginHorizontal(false);
            {
                GUILayout.Label("搜索", GUILayout.Width(IndexWidth));

                EditorGUI.BeginChangeCheck();
                iconSet.searchIconInfo.name = EditorGUILayout.DelayedTextField(iconSet.searchIconInfo.name, GUILayout.Width(NameWidth));
                iconSet.searchIconInfo.description = EditorGUILayout.DelayedTextField(iconSet.searchIconInfo.description, GUILayout.Width(DescriptionWidth));
                iconSet.searchIconInfo.id = EditorGUILayout.DelayedIntField(iconSet.searchIconInfo.id, GUILayout.Width(IDWidth));
                iconSet.searchIconInfo.iconPath = EditorGUILayout.DelayedTextField(iconSet.searchIconInfo.iconPath);
                if (EditorGUI.EndChangeCheck())
                {
                    iconSet.UpdateSearch();
                }
                if (GUILayout.Button("清空搜索条件", EditorStyles.miniButtonRight, GUILayout.Width(OperationWidth)))
                {
                    GUI.FocusControl("");
                    iconSet.ClearSearch();
                }

            }
            UICommonFun.EndHorizontal();

            XEditorGUI.DrawList(iconSet.drawIconInfos,
                () =>
                {
                    EditorGUI.BeginChangeCheck();
                    if (GUILayout.Button("名称", EditorStyles.label, GUILayout.Width(NameWidth)))
                    {
                        iconSet.iconInfos.Sort((x, y) => x.name.CompareTo(y.name));
                    }
                    if (GUILayout.Button("描述", EditorStyles.label, GUILayout.Width(DescriptionWidth)))
                    {
                        iconSet.iconInfos.Sort((x, y) => x.description.CompareTo(y.description));
                    }
                    if (GUILayout.Button("ID", EditorStyles.label, GUILayout.Width(IDWidth)))
                    {
                        iconSet.iconInfos.Sort((x, y) => x.id.CompareTo(y.id));
                    }
                    if (GUILayout.Button("图标路径", EditorStyles.label))
                    {
                        iconSet.iconInfos.Sort((x, y) => x.iconPath.CompareTo(y.iconPath));
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        iconSet.UpdateSearch();
                    }
                },
                (info, i) =>
                {
                    EditorGUILayout.SelectableLabel(info.name, GUILayout.Width(NameWidth), UICommonOption.Height16);
                    EditorGUILayout.SelectableLabel(info.description, GUILayout.Width(DescriptionWidth), UICommonOption.Height16);
                    EditorGUILayout.SelectableLabel(info.id.ToString(), GUILayout.Width(IDWidth), UICommonOption.Height16);
                    info.iconPath = EditorGUILayout.DelayedTextField(info.iconPath);
                    SelectIcon(info);
                },
                IndexWidth,
                OperationWidth, UICommonOption.WH24x16);

            EditorGUILayout.Separator();

            UICommonFun.BeginHorizontal(false);
            {
                GUILayout.Label("信息", GUILayout.Width(IndexWidth));

                iconSet.newIconInfo.name = EditorGUILayout.DelayedTextField(iconSet.newIconInfo.name, GUILayout.Width(NameWidth));
                iconSet.newIconInfo.description = EditorGUILayout.DelayedTextField(iconSet.newIconInfo.description, GUILayout.Width(DescriptionWidth));
                iconSet.newIconInfo.id = EditorGUILayout.DelayedIntField(iconSet.newIconInfo.id, GUILayout.Width(IDWidth));
                iconSet.newIconInfo.iconPath = EditorGUILayout.DelayedTextField(iconSet.newIconInfo.iconPath);
                SelectIcon(iconSet.newIconInfo);

                if (GUILayout.Button("添加", EditorStyles.miniButtonRight, GUILayout.Width(OperationWidth)))
                {
                    iconSet.AddIconInfo(iconSet.newIconInfo);
                }
            }
            UICommonFun.EndHorizontal();
        }

        private void SelectIcon(XDreamerIconOption.IconInfo iconInfo)
        {
            UICommonFun.SelectImageButtonInAssets(ref iconInfo.iconPath, CommonFun.TempContent("...", "点击选择图标文件"), EditorStyles.miniButtonRight, GUILayout.Width(25));
        }

        private string GetUniqueNameInIconSets(XDreamerIconOption option, string name)
        {
            return CommonFun.GetUniqueName(name, n => !option.iconSets.Exists(i => i.name == n));
        }

        #endregion

        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.iconSetName):
                    {
                        DrawCreateIconSetGUI(preference);

                        DrawIconSetDropdownList(preference);

                        return true;
                    }
                case nameof(preference.iconSets):
                    {
                        DrawIconSets(preference);
                        return true;
                    }
            }

            return base.OnGUI(obj, fieldInfo);
        }
    }

}
