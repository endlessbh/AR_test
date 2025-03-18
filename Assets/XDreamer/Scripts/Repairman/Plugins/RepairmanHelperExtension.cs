using System.Collections.Generic;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginRepairman
{
    /// <summary>
    /// 拆装助手扩展
    /// </summary>
    public static class RepairmanHelperExtension
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public const string CategoryName = "拆装";

        /// <summary>
        /// 数据模型名称
        /// </summary>
        public const string DataModelName = "模型";

        /// <summary>
        /// 数据模型路径
        /// </summary>
        public const string DataModelPath = CategoryName + "/" + DataModelName;

        /// <summary>
        /// 数据模型状态库名称
        /// </summary>
        public const string DataModelStateLibName = CategoryName + "-" + DataModelName;

        /// <summary>
        /// 步骤名称
        /// </summary>
        public const string StepName = "步骤";

        /// <summary>
        /// 步骤路径
        /// </summary>
        public const string StepPath = CategoryName + "-" + StepName;

        /// <summary>
        /// 步骤状态库名称
        /// </summary>
        public const string StepStateLibName = CategoryName + "-" + StepName;

        public static T GetStateComponent<T>(bool includeDisable = true) where T : StateComponent
        {
            List<T> list = SMSHelper.GetStateComponents<T>();
            return list.Find(t =>
            {
                return includeDisable ? true : t.parent.active == true;
            });
        }

        public static void RemoveNullObject<T>(List<T> objects) where T : UnityEngine.Object
        {
            for (int i = objects.Count - 1; i >= 0; --i)
            {
                if (!objects[i]) objects.RemoveAt(i);
            }
        }
    }
}
