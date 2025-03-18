using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.ExportPackageItem")]
    public class ExportPackageItem : LinkType<ExportPackageItem>
    {
        public ExportPackageItem(object obj) : base(obj) { }

        #region assetPath

        public static XFieldInfo assetPath_FieldInfo { get; } = GetXFieldInfo(nameof(assetPath));

        public string assetPath
        {
            get => assetPath_FieldInfo.GetValue(obj) as string;
            set => assetPath_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region guid

        public static XFieldInfo guid_FieldInfo { get; } = GetXFieldInfo(nameof(guid));

        public string guid
        {
            get => guid_FieldInfo.GetValue(obj) as string;
            set => guid_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region isFolder

        public static XFieldInfo isFolder_FieldInfo { get; } = GetXFieldInfo(nameof(isFolder));

        public bool isFolder
        {
            get => (bool)isFolder_FieldInfo.GetValue(obj);
            set => isFolder_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region enabledStatus

        public static XFieldInfo enabledStatus_FieldInfo { get; } = GetXFieldInfo(nameof(enabledStatus));

        public int enabledStatus
        {
            get => (int)enabledStatus_FieldInfo.GetValue(obj);
            set => enabledStatus_FieldInfo.SetValue(obj, value);
        }

        #endregion
    }
}
