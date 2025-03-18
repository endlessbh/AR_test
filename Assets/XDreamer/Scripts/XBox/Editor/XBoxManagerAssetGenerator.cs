using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXBox;
using XCSJ.PluginXBox.Base;
using static XCSJ.EditorExtension.Base.ProjectSettings.XInputManager;

namespace XCSJ.EditorXBox
{
    /// <summary>
    /// <see cref="XBoxManager"/>资产生成器
    /// </summary>
    public class XBoxManagerAssetGenerator
    {
        /// <summary>
        /// 轴预设
        /// </summary>
        private static List<Axis> axisPresets = new List<Axis>();

        /// <summary>
        /// 生成输入管理器资产：即配置XBox手柄到输入管理器
        /// </summary>
        public static bool GenerateInputManagerAsset()
        {
            ApplyAxisPresets();
            var valid = CheckAxisPresets();
            if (valid)
            {
                Debug.Log("成功配置XBox手柄到输入管理器！");
            }
            else
            {
                Debug.LogError("配置XBox手柄到输入管理器失败！");
            }
            return valid;
        }

        /// <summary>
        /// 检查输入管理器资产：即检测XBox配置
        /// </summary>
        /// <returns></returns>
        public static bool CheckInputManagerAsset()
        {
            var valid = CheckAxisPresets();
            if (valid)
            {
                Debug.Log("XBox手柄已配置到输入管理器！");
            }
            else
            {
                Debug.LogError("XBox手柄未配置到输入管理器！");
            }
            return valid;
        }

        /// <summary>
        /// 检查轴预设
        /// </summary>
        /// <returns></returns>
        public static bool CheckAxisPresets()
        {
            SetupAxisPresets();

            var axisArray = axesSerializedProperty;

            if (axisArray.arraySize != axisPresets.Count)
            {
                return false;
            }

            for (int i = 0; i < axisPresets.Count; i++)
            {
                var axisEntry = axisArray.GetArrayElementAtIndex(i);
                if (!axisPresets[i].EqualTo(axisEntry))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 应用轴预设
        /// </summary>
        private static void ApplyAxisPresets()
        {
            SetupAxisPresets();

            TryGet(out _, out var serializedObject, out var axisArray);

            axisArray.arraySize = axisPresets.Count;
            for (int i = 0; i < axisPresets.Count; i++)
            {
                var axisEntry = axisArray.GetArrayElementAtIndex(i);
                axisPresets[i].SerializeTo(axisEntry);
            }
            serializedObject.ApplyModifiedProperties();

            XDreamerEvents.CallAnyAssetsChanged();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 设置轴预设，即填充<see cref="axisPresets"/>
        /// </summary>
        private static void SetupAxisPresets()
        {
            axisPresets.Clear();
            ImportExistingAxisPresets();
            CreateRequiredAxisPresets();
            CreateCompatibilityAxisPresets();
        }

        /// <summary>
        /// 创建XBox必须的轴预设
        /// </summary>
        private static void CreateRequiredAxisPresets()
        {
            foreach (var button in EnumCache<EXBoxButton>.Array)
            {
                var id = button.ToJoystickButtonID();
                var name = button.ToString();
                if (id >= 0 && !HasAxisPreset(name))
                {
                    axisPresets.Add(new Axis(name, id, true));
                }
            }

            foreach (var axis in EnumCache<EXBoxAxis>.Array)
            {
                var id = axis.ToJoystickButtonID();
                var name = axis.ToString();
                if (id >= 0 && !HasAxisPreset(name))
                {
                    axisPresets.Add(new Axis(name, id, false));
                }
            }
        }

        /// <summary>
        /// 导入已经存在的轴预设
        /// </summary>
        private static void ImportExistingAxisPresets() => axisPresets.AddRange(GetAxes().Where(axis => !axis.ReservedName));

        /// <summary>
        /// 创建兼容性轴预设
        /// </summary>
        private static void CreateCompatibilityAxisPresets()
        {
            if (!HasAxisPreset("Mouse ScrollWheel"))
            {
                axisPresets.Add(new Axis("Mouse ScrollWheel", 1, 2, 0.1f));
            }
            
            if (!HasAxisPreset("Horizontal"))
            {
                axisPresets.Add(new Axis()
                {
                    m_Name = "Horizontal",
                    negativeButton = "left",
                    positiveButton = "right",
                    altNegativeButton = "a",
                    altPositiveButton = "d",
                    gravity = 3.0f,
                    dead = 0.001f,
                    sensitivity = 3.0f,
                    snap = true,
                    type = 0,
                    axis = 0,
                    joyNum = 0
                });

                axisPresets.Add(new Axis()
                {
                    m_Name = "Horizontal",
                    gravity = 0.0f,
                    dead = 0.19f,
                    sensitivity = 1.0f,
                    type = 2,
                    axis = 0,
                    joyNum = 0
                });
            }

            if (!HasAxisPreset("Vertical"))
            {
                axisPresets.Add(new Axis()
                {
                    m_Name = "Vertical",
                    negativeButton = "down",
                    positiveButton = "up",
                    altNegativeButton = "s",
                    altPositiveButton = "w",
                    gravity = 3.0f,
                    dead = 0.001f,
                    sensitivity = 3.0f,
                    snap = true,
                    type = 0,
                    axis = 0,
                    joyNum = 0
                });

                axisPresets.Add(new Axis()
                {
                    m_Name = "Vertical",
                    gravity = 0.0f,
                    dead = 0.19f,
                    sensitivity = 1.0f,
                    type = 2,
                    axis = 0,
                    invert = true,
                    joyNum = 0
                });
            }

            if (!HasAxisPreset("Submit"))
            {
                axisPresets.Add(new Axis()
                {
                    m_Name = "Submit",
                    positiveButton = "return",
                    altPositiveButton = "joystick button 0",
                    gravity = 1000.0f,
                    dead = 0.001f,
                    sensitivity = 1000.0f,
                    type = 0,
                    axis = 0,
                    joyNum = 0
                });

                axisPresets.Add(new Axis()
                {
                    m_Name = "Submit",
                    positiveButton = "enter",
                    altPositiveButton = "space",
                    gravity = 1000.0f,
                    dead = 0.001f,
                    sensitivity = 1000.0f,
                    type = 0,
                    axis = 0,
                    joyNum = 0
                });
            }

            if (!HasAxisPreset("Cancel"))
            {
                axisPresets.Add(new Axis()
                {
                    m_Name = "Cancel",
                    positiveButton = "escape",
                    altPositiveButton = "joystick button 1",
                    gravity = 1000.0f,
                    dead = 0.001f,
                    sensitivity = 1000.0f,
                    type = 0,
                    axis = 0,
                    joyNum = 0
                });
            }
        }

        private static bool HasAxisPreset(string name) => axisPresets.Any(axis => axis.m_Name == name);
    }
}