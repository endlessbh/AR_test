using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 模型数据类
    /// </summary>
    [Import]
    public class Model : INamePath, IDelete, INavigationData
    {
        #region 序列化信息-会输出到JSON的信息

        [Name("唯一编号")]
        public string guid = "";

        [Name("名称")]
        public string name = "";

        [Name("提示")]
        public string tip = "";

        [Name("描述")]
        public string description = "";

        [Name("可见")]
        public bool visable = true;

        [Name("资产类型")]
        public string assetType = "";

        [Name("索引")]
        [Tip("分类内部排序排序使用", "Classification internal sorting")]
        public int index = 0;

        #endregion

        #region 资产对象

        /// <summary>
        /// 判断资产是否存在;
        /// </summary>
        [Json(false)]
        public virtual bool assetExist => !string.IsNullOrEmpty(assetPath);

        /// <summary>
        /// 资产路径
        /// </summary>
        [Json(false)]
        public string assetPath => AssetDatabase.GUIDToAssetPath(guid);

        private UnityEngine.Object _asset = null;

        /// <summary>
        /// 资产对象
        /// </summary>
        [Json(false)]
        public UnityEngine.Object asset
        {
            get
            {
                if (!_asset)
                {
                    if (string.IsNullOrEmpty(guid)) return null;
                    _asset = UICommonFun.LoadFromAssets(assetPath, default(UnityEngine.Object));
                }
                return _asset;
            }
            set
            {
                _asset = value;
                if (_asset)
                {
                    this.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_asset));
                }
                else
                {
                    this.guid = "";
                }
            }
        }

        #endregion

        #region 缩略图--未实现 -- 不需要

        [Name("图片路径")]
        public string imagePath = "";
        #endregion

        #region 备份缩略图

        private Texture _thumbnail = null;

        /// <summary>
        /// 缩略图
        /// </summary>
        [Json(false)]
        public Texture thumbnail
        {
            get
            {
                if (_thumbnail == null || isDirty)
                {
                    //重新构建缓存
                    _thumbnail = default;
                    var imageURL = UICommonFun.ToFullPath(imagePath);
                    if (string.IsNullOrEmpty(imagePath) || !FileHelper.Exists(imageURL))
                    {
                        //资产无效
                        if (!asset) return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Object));

                        //还在加载预览图中
                        if (AssetPreview.IsLoadingAssetPreview(asset.GetInstanceID()))
                        {
                            //Debug.Log(assetPath + " ==> 加载预览图中");
                            //那么使用迷你缩略图或迷你类型缩略图
                            var tex = AssetPreview.GetMiniThumbnail(asset);
                            return tex ? tex : AssetPreview.GetMiniTypeThumbnail(asset.GetType());
                        }

                        //优先使用预览图
                        Texture2D texture = AssetPreview.GetAssetPreview(asset);

                        //预览图无效时，使用迷你缩略图
                        if (texture == null) texture = AssetPreview.GetMiniThumbnail(asset);
                        _thumbnail = texture;
                    }
                    else
                    {
                        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                        ImageConversion.LoadImage(texture, File.ReadAllBytes(imageURL), false);
                        _thumbnail = texture;
                    }
                }
                return _thumbnail;
            }
        }

        private Texture _typeIcon = null;

        /// <summary>
        /// 类型的缩略图
        /// </summary>
        [Json(false)]
        public Texture typeIcon
        {
            get
            {
                if (_typeIcon == null || isDirty)
                {
                    //重新构建缓存
                    _typeIcon = default;

                    //资产无效
                    if (!asset) return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Object));

                    //使用迷你类型缩略图
                    _typeIcon = AssetPreview.GetMiniTypeThumbnail(asset.GetType());
                }
                return _typeIcon;
            }
        }

        #endregion

        #region INamePath

        [Json(false)]
        public Model parent;

        INamePath INamePath.parent => parent;

        string IName.name { get => name; set => name = value; }

        [Json(false)]
        public string namePath => (parent == null ? "" : parent.namePath) + Define.Delimiter + this.name;

        #endregion

        #region 物理磁盘关联的JSON文件信息

        /// <summary>
        /// JSON文件路径；以Assets开头的路径
        /// </summary>
        [Json(false)]
        public string jsonPath = "";

        /// <summary>
        /// 判断JSON文件是否在磁盘上真实存在
        /// </summary>
        [Json(false)]
        public bool jsonFileExists => File.Exists(jsonPath);

        /// <summary>
        /// JSON文件名称；含文件扩展名；
        /// </summary>
        [Json(false)]
        public string jsonFileName => Path.GetFileName(jsonPath);

        /// <summary>
        /// JSON文件所在的目录名称; 以Assets开头的文件夹路径；
        /// </summary>
        [Json(false)]
        public string jsonDirectoryName => string.IsNullOrEmpty(jsonPath) ? "" : Path.GetDirectoryName(jsonPath);

        /// <summary>
        /// 所在的文件夹名称;
        /// </summary>
        [Json(false)]
        public string jsonFolderName => string.IsNullOrEmpty(jsonDirectoryName) ? "" : Path.GetFileName(jsonDirectoryName);

        #endregion

        #region 界面编辑使用

        /// <summary>
        /// 深度
        /// </summary>
        [Json(false)]
        public int depth => parent == null ? 0 : parent.depth + 1;

        /// <summary>
        /// 是否脏；即标识数据是否发生了修改；
        /// </summary>
        [Json(false)]
        public bool isDirty { get; private set; } = false;

        /// <summary>
        /// 标记脏
        /// </summary>
        /// <param name="isDirty"></param>
        public void MarkDirty(bool isDirty = true) => this.isDirty = isDirty;

        /// <summary>
        /// 判断是否需要保存
        /// </summary>
        /// <param name="onlyDirty"></param>
        /// <returns></returns>
        public bool NeedSave(bool onlyDirty) => onlyDirty ? isDirty : true;

        #endregion

        #region 删除

        /// <summary>
        /// 删除当前对象
        /// </summary>
        /// <param name="deleteObject"></param>
        /// <returns></returns>
        public bool Delete(bool deleteObject = false)
        {
            if (parent != null)
            {
                parent.OnDelete(this, deleteObject);
            }
            return true;
        }

        /// <summary>
        /// 删除时的回调
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deleteObject"></param>
        public virtual void OnDelete(Model model, bool deleteObject)
        {
            MarkDirty();
        }

        #endregion

        #region INavigationData

        [Json(false)]
        public List<object> navigationItems
        {
            get
            {
                var list = parent == null ? new List<object>() : parent.navigationItems;
                list.Add(this);
                return list;
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="item"></param>
        public void OnClickNavigationItem(object item)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 获取导航图项的GUIContent
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public GUIContent GetNavigationItemGUIContent(object item)
        {
            return CommonFun.TempContent(NavigationBar.DefaultObjectToName(item));
        }

        #endregion

        [Json(false)]
        public bool visiableInPanel => parent == null ? visable : (visable ? parent.visiableInPanel : visable);
    }

    /// <summary>
    /// 项；对应 Data
    /// </summary>
    public class Item : Model
    {
        #region 序列化信息-会输出到JSON的信息

        [Name("标签")]
        public List<string> tags = new List<string>();

        [Name("来源")]
        public string source = "";

        #endregion

        #region 基础信息

        [Json(false)]
        public Category category => parent as Category;

        #endregion

        #region 构造函数

        public Item() { }

        public Item(string name, string jsonPath, Category parent, UnityEngine.Object asset)
        {
            this.name = name;
            this.jsonPath = jsonPath;
            this.parent = parent;
            this.asset = asset;
        }

        #endregion
    }

    /// <summary>
    /// 目录；对应 CategoryData
    /// </summary>
    public class Category : Model
    {
        #region 基础信息

        /// <summary>
        /// 判断资产是否存在;对于目录一直返回True；
        /// </summary>
        [Json(false)]
        public override bool assetExist => true;

        [Json(false)]
        public bool expand = false;

        [Json(false)]
        public List<Category> categories = new List<Category>();

        [Json(false)]
        public List<Item> items = new List<Item>();

        private int _itemTotalCount = -1;
        [Json(false)]
        public int itemTotalCount
        {
            get
            {
                if (_itemTotalCount == -1 || isDirty)
                    _itemTotalCount = itemTotalCountRealTime;
                return _itemTotalCount;
            }
        }

        [Json(false)]
        public int itemTotalCountRealTime => categories.Sum(c => c.itemTotalCount) + items.Count;

        private int _visibleItemTotalCount = -1;
        [Json(false)]
        public int visibleItemTotalCount
        {
            get
            {
                if (_visibleItemTotalCount == -1 || isDirty)
                    _visibleItemTotalCount = visibleItemTotalCountRealTime;
                return _visibleItemTotalCount;
            }
        }

        [Json(false)]
        public int visibleItemTotalCountRealTime => visable ? items.Count(i => i.visable) + categories.Sum(c => c.visibleItemTotalCount) : 0;

        [Json(false)]
        public int categoryTotalCount => categories.Sum(c => c.categoryTotalCount) + categories.Count;

        //[Json(false)]
        //public int expandCategoryTotalCount => expand ? categories.Sum(c => c.expandCategoryTotalCount) + 1 : 1;

        //[Json(false)]
        //public int expandVisibleCategoryTotalCount => visiableInPanel? (expand ? categories.Sum(c => c.expandVisibleCategoryTotalCount) + 1 : 1) : 0;

        [Json(false)]
        public Vector2 scrollPosition = Vector2.zero;

        #endregion

        #region 构造函数

        public Category() { }

        public Category(string name, string jsonPath, Category parent)
        {
            this.name = name;
            this.jsonPath = jsonPath;
            this.parent = parent;
        }

        #endregion

        #region 输出JSON文件

        public void Output(bool onlyDirty = true, bool children = true)
        {
            OuputCategory(onlyDirty, children);
            OuputItem(onlyDirty, children);
        }

        public void OuputCategory(bool onlyDirty = true, bool children = true)
        {
            if (NeedSave(onlyDirty)) File<Category>.SaveFile(jsonPath, this);
            if (children) categories.ForEach(c => c.OuputCategory(onlyDirty, children));
        }

        public void OuputItem(bool onlyDirty = true, bool children = true)
        {
            items.GroupBy(i => i.jsonPath).Foreach(g =>
            {
                if (g.Any(i1 => i1.NeedSave(onlyDirty)))
                {
                    File<Item>.SaveFile(g.Key, g.ToList());
                }
            });
            if (children) categories.ForEach(c => c.OuputItem(onlyDirty, children));
        }

        #endregion

        #region 查找与搜索

        /// <summary>
        /// 通过名称路径查找目录
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public Category FindCategory(string namePath) => this.Find(namePath, c => c.categories, Define.Delimiter);

        public List<Item> SearchItem(string searchText, ESearchType searchType, bool checkVisible = false) => SearchItem(this, Define.SearchPredicate(searchText, searchType, checkVisible));

        public static void SearchItem(List<Category> categories, List<Item> items, Func<Item, bool> predicate)
        {
            categories.ForEach(c => SearchItem(c, items, predicate));
        }

        public static void SearchItem(Category category, List<Item> items, Func<Item, bool> predicate)
        {
            items.AddRange(category.items.Where(predicate));
            SearchItem(category.categories, items, predicate);
        }

        public static List<Item> SearchItem(List<Category> categories, Func<Item, bool> predicate)
        {
            List<Item> items = new List<Item>();
            SearchItem(categories, items, predicate);
            return items;
        }

        public static List<Item> SearchItem(Category category, Func<Item, bool> predicate)
        {
            List<Item> items = new List<Item>();
            SearchItem(category, items, predicate);
            return items;
        }

        #endregion

        #region 添加项

        /// <summary>
        /// 分析文件的类型
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public bool CheckFileExt(string assetPath)
        {
            //分析文件的类型
            if (EditorPath.PathEndWithExt(assetPath, Define.MetaExt)) return false;
            if (EditorPath.PathEndWithExt(assetPath, Define.FileExt)) return false;

            //空类型，表示不限定类型
            if (string.IsNullOrEmpty(assetType))
            {
                return true;
            }
            else
            {
                var assetExts = AssetLibWindowOption.weakInstance.assetExtensionConfig.GetExtensions(assetType);
                if (assetExts == null)
                {
                    return true;
                }
                else
                {
                    return EditorPath.PathEndWithExt(assetPath, assetExts);
                }
            }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public Item AddItem(string assetPath)
        {
            //分析文件的类型
            if (!CheckFileExt(assetPath)) return null;

            //转为基于Assets的相对路径
            assetPath = UICommonFun.ToAssetsPath(assetPath);

            //判断资产是否已经添加
            var item = items.FirstOrDefault(i => i.assetPath == assetPath);
            if (item != null) return item;

            var asset = UICommonFun.LoadFromAssets(assetPath, default(UnityEngine.Object));
            if (!asset) return null;

            item = new Item(asset.name, Define.CombinePath(Path.GetDirectoryName(assetPath), Define.ItemFileName), this, asset);
            items.Add(item);

            return item;
        }

        /// <summary>
        /// 添加项
        /// </summary>
        public void AddItems(IEnumerable<Item> items)
        {
            items.Foreach(i =>
            {
                i.parent = this;
                this.items.Add(i);
            });
        }

        #endregion

        #region 遍历

        public void ForeachCategory(Action<Category> action, bool children = true)
        {
            action(this);
            if (children) categories.ForEach(c => c.ForeachCategory(action, children));
        }

        public void ForeachItem(Action<Item> action, bool children = true)
        {
            items.ForEach(action);
            if (children) categories.ForEach(c => c.ForeachItem(action, children));
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除时的回调
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deleteObject"></param>
        public override void OnDelete(Model model, bool deleteObject)
        {
            base.OnDelete(model, deleteObject);
            if (model is Item item)
            {
                items.Remove(item);
            }
            else if (model is Category category)
            {
                categories.Remove(category);
            }
        }

        #endregion
    }

    /// <summary>
    /// 基础定义
    /// </summary>
    public class Define
    {
        #region 基础定义

#pragma warning disable IDE1006 // 命名样式

        public const string CategoryFileName = "category.json";

        public const string ItemFileName = "item.json";

        public const string FileExt = ".json";

        public const string MetaExt = ".meta";

        public const string Delimiter = "/";

#pragma warning restore IDE1006 // 命名样式

        #endregion

        #region 搜索

        private static bool SearchMatch(string value, string searchText) => value.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0;

        public static Func<Item, bool> SearchPredicate(string searchText, ESearchType searchType, bool checkVisible = false)
        {
            Func<Item, bool> predicate = i => (checkVisible ? i.visiableInPanel : true);
            if (string.IsNullOrEmpty(searchText)) return predicate;

            switch (searchType)
            {
                case ESearchType.Name:
                    {
                        predicate = i => SearchMatch(i.name, searchText) && (checkVisible ? i.visiableInPanel : true);
                        break;
                    }
                case ESearchType.Tag:
                    {
                        predicate = i => i.tags.Any(t => SearchMatch(t, searchText)) && (checkVisible ? i.visiableInPanel : true);
                        break;
                    }
                case ESearchType.Source:
                    {
                        predicate = i => SearchMatch(i.source, searchText) && (checkVisible ? i.visiableInPanel : true);
                        break;
                    }
                case ESearchType.Fuzzy:
                    {
                        predicate = i => (SearchMatch(i.name, searchText) || i.tags.Any(t => SearchMatch(t, searchText)) || SearchMatch(i.source, searchText)) && (checkVisible ? i.visiableInPanel : true);
                        break;
                    }
            }

            return predicate;
        }

        #endregion

        /// <summary>
        /// 合并路径
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(params string[] paths) => PathHelper.Format(paths.ToStringDirect("/")).Replace("//", "/");
    }

    /// <summary>
    /// 文件泛型类；用于处理JSON文件的加载与保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class File<T> where T : Model
    {
        #region 加载

        public static List<T> LoadFile(string path)
        {
            if (!FileHelper.Exists(path)) return new List<T>();
            var list = Load(FileHelper.InputFile(path));
            list.Foreach(o => o.jsonPath = path);
            return list;
        }

        private static List<T> Load(string data)
        {
            if (string.IsNullOrEmpty(data)) return new List<T>();

            if (JsonHelper.ToObject<T>(data) is T model)
            {
                return model.assetExist ? new List<T>() { model } : new List<T>();
            }
            if (JsonHelper.ToObject<List<T>>(data) is List<T> models)
            {
                return models.Where(o => o.assetExist).ToList();
            }

            return new List<T>();
        }

        #endregion

        #region 保存

        public static void SaveFile(string path, List<T> models)
        {
            if (string.IsNullOrEmpty(path) || models == null) return;
            switch (models.Count)
            {
                case 0: break;
                case 1:
                    {
                        Save(path, JsonHelper.ToJson(models[0], true));
                        break;
                    }
                default:
                    {
                        Save(path, JsonHelper.ToJson(models, true));
                        break;
                    }
            }
            MarkSave(models);
        }

        public static void SaveFile(string path, params T[] models)
        {
            if (string.IsNullOrEmpty(path) || models == null) return;
            switch (models.Length)
            {
                case 0: break;
                case 1:
                    {
                        Save(path, JsonHelper.ToJson(models[0], true));
                        break;
                    }
                default:
                    {
                        Save(path, JsonHelper.ToJson(models, true));
                        break;
                    }
            }
            MarkSave(models);
        }

        private static void MarkSave(IEnumerable<T> models)
        {
            models.Foreach(m => m.MarkDirty(false));
        }

        private static void Save(string path, string data)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(data)) return;
            FileHelper.OutputFile(path, data);
        }

        #endregion
    }

    /// <summary>
    /// 加载规则
    /// </summary>
    [Name("加载规则")]
    public enum ELoadRule
    {
        /// <summary>
        /// 加载，只读取配置文件存在的
        /// </summary>
        Load,

        /// <summary>
        /// 加载并生成，优先加载配置文件，再分析本地磁盘情况生成
        /// </summary>
        LoadAndGenerate,

        /// <summary>
        /// 生成，不读取配置文件，直接分析本地磁盘情况生成
        /// </summary>
        Generate,
    }

    /// <summary>
    /// XDreamer资产数据库
    /// </summary>
    public class XAssetsDatabase : InstanceClass<XAssetsDatabase>
    {
        #region 基础信息

        /// <summary>
        /// 目录列表
        /// </summary>
        public List<Category> categories = new List<Category>();

        /// <summary>
        /// 目录总数
        /// </summary>
        public int categoryTotalCount => categories.Sum(c => c.categoryTotalCount) + categories.Count;

        /// <summary>
        /// 项的总数
        /// </summary>
        public int itemTotalCount => categories.Sum(c => c.itemTotalCount);

        #endregion

        #region 加载   

        private static readonly char[] separator = { '\\', '/' };

        public void Load(ELoadRule loadRule = ELoadRule.Load, bool sort = true)
        {
            categories.Clear();
            var paths = AssetLibWindowOption.weakInstance.assetPathConfigs;
            foreach (var path in paths)
            {
                var jsonPath = Define.CombinePath(path.path, Define.CategoryFileName);
                var list = File<Category>.LoadFile(jsonPath);
                if (list == null || list.Count == 0)
                {
                    //不存在有效的目录文件--执行创建
                    var category = new Category(path.name, jsonPath, null);
                    categories.Add(category);
                }
                else
                {
                    categories.AddRange(list);
                }
            }

            //加载所有分类信息
            categories.Foreach(c => Load(c, loadRule));

            //排序
            if (sort) Sort();
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort()
        {
            Sort((x, y) => SortCompare(x, y), (x, y) => SortCompare(x, y));
        }

        private int SortCompare(Model x, Model y)
        {
            var c = x.index - y.index;
            if (c == 0)
            {
                return string.Compare(x.name, y.name);
            }
            return c < 0 ? -1 : 1;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="categoryComparison">Category比较函数</param>
        /// <param name="itemComparison">Item比较函数</param>
        public void Sort(Comparison<Category> categoryComparison, Comparison<Item> itemComparison)
        {
            if (categoryComparison == null) throw new ArgumentNullException(nameof(categoryComparison));
            if (itemComparison == null) throw new ArgumentNullException(nameof(itemComparison));

            categories.ForEach(c =>
            {
                c.ForeachCategory(c1 =>
                {
                    c1.categories.Sort(categoryComparison);
                    c1.items.Sort(itemComparison);
                });
            });
        }

        private Category FindOrCreateCategory(Category category, string filePath)
        {
            var categoryDirectory = Path.GetDirectoryName(category.jsonPath);
            var directory = Path.GetDirectoryName(filePath);

            if (categoryDirectory == directory) return category;

            var array1 = categoryDirectory.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var array2 = directory.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            Category last = category;
            for (int i = array1.Length; i < array2.Length; i++)
            {
                last = last.categories.FirstOrNew(c => c.jsonFolderName == array2[i], () =>
                {
                    var jsonPath = Define.CombinePath(last.jsonDirectoryName, array2[i], Define.CategoryFileName);
                    var c = File<Category>.LoadFile(jsonPath).FirstOrDefault();
                    if (c == null)
                    {
                        c = new Category(array2[i], jsonPath, last);
                    }
                    else
                    {
                        c.parent = last;
                    }
                    last.categories.Add(c);
                    return c;
                });
            }
            return last;
        }

        private void Load(Category category, ELoadRule loadRule)
        {
            switch (loadRule)
            {
                case ELoadRule.Load:
                    {
                        var files = Directory.GetFiles(Path.GetDirectoryName(category.jsonPath), Define.ItemFileName, SearchOption.AllDirectories).ToList(UICommonFun.ToAssetsPath);
                        files.Sort();

                        foreach (var f in files)
                        {
                            var items = File<Item>.LoadFile(f);
                            if (items.Count > 0)
                            {
                                FindOrCreateCategory(category, f)?.AddItems(items);
                            }
                        }
                        break;
                    }
                case ELoadRule.LoadAndGenerate:
                    {
                        Load(category, ELoadRule.Load);
                        Load(category, ELoadRule.Generate);
                        break;
                    }
                case ELoadRule.Generate:
                    {
                        var files = Directory.GetFiles(Path.GetDirectoryName(category.jsonPath), "*.*", SearchOption.AllDirectories).ToList(UICommonFun.ToAssetsPath);
                        files.Sort();

                        foreach (var f in files)
                        {
                            FindOrCreateCategory(category, f)?.AddItem(f);
                        }

                        break;
                    }
            }
        }

        #endregion

        #region 保存

        public void Save(bool onlyDirty = true, bool children = true) => categories.Foreach(c => c.Output(onlyDirty, children));

        #endregion

        #region 查找与搜索

        /// <summary>
        /// 通过名称路径查找目录
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public Category FindCategory(string namePath) => NamePathExtension.Find(categories, namePath, c => c.categories, Define.Delimiter);

        /// <summary>
        /// 通过名称路径查找项
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public Item FindItem(string namePath)
        {
            if (namePath.TryParseNamePath(out string parentNamePath, out string name, Define.Delimiter))
            {
                return FindCategory(parentNamePath)?.items.FirstOrDefault(i => i.name == name);
            }
            return null;
        }

        public List<Item> SearchItem(string searchText, ESearchType searchType) => Category.SearchItem(categories, Define.SearchPredicate(searchText, searchType));

        #endregion
    }
}