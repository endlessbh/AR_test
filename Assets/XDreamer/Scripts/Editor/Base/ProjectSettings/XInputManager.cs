using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.ProjectSettings
{
    /// <summary>
    /// 输入管理器；用于关联Unity的InputManager.asset资产
    /// </summary>
    public class XInputManager
    {
        /// <summary>
        /// 输入管理器资产路径
        /// </summary>
        public const string AssetPath = "ProjectSettings/InputManager.asset";

        /// <summary>
        /// 输入管理器资产对象
        /// </summary>
        public static UnityEngine.Object asset => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetPath);

        /// <summary>
        /// 输入管理器序列化对象
        /// </summary>
        public static SerializedObject axesSerializedObject => new SerializedObject(asset);

        /// <summary>
        /// 获取轴列表序列化属性对象
        /// </summary>
        public static SerializedProperty axesSerializedProperty => axesSerializedObject.FindProperty(nameof(m_Axes));

        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="axesSerializedObject"></param>
        /// <param name="axesSerializedProperty"></param>
        public static void TryGet(out UnityEngine.Object asset, out SerializedObject axesSerializedObject, out SerializedProperty axesSerializedProperty)
        {
            asset = XInputManager.asset; new SerializedObject(asset);
            axesSerializedObject = new SerializedObject(asset);
            axesSerializedProperty = axesSerializedObject.FindProperty(nameof(m_Axes));
        }

        /// <summary>
        /// 获取轴对象列表
        /// </summary>
        public static List<Axis> m_Axes => GetAxes();

        /// <summary>
        /// 获取轴对象列表
        /// </summary>
        /// <returns></returns>
        public static List<Axis> GetAxes()
        {
            var axes = new List<Axis>();
            var axesProperty = axesSerializedProperty;

            for (int i = 0, l = axesProperty.arraySize; i < l; i++)
            {
                axes.Add(new Axis(axesProperty.GetArrayElementAtIndex(i)));
            }

            return axes;
        }

        /// <summary>
        /// 获取轴的名称列表，名称可能会出现重复
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAxisNames()
        {
            var axes = new List<string>();
            var axesProperty = axesSerializedProperty;

            for (int i = 0, l = axesProperty.arraySize; i < l; i++)
            {
                axes.Add(Axis.GetName(axesProperty.GetArrayElementAtIndex(i)));
            }
            return axes;
        }

        /// <summary>
        /// 获取轴的名称列表，名称不会重复
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetAxisNamesDistinct()
        {
            var axes = new HashSet<string>();
            var axesProperty = axesSerializedProperty;

            for (int i = 0, l = axesProperty.arraySize; i < l; i++)
            {
                axes.Add(Axis.GetName(axesProperty.GetArrayElementAtIndex(i)));
            }
            return axes;
        }

        /// <summary>
        /// 添加新轴
        /// </summary>
        /// <param name="axis"></param>
        public static void AddAxis(Axis axis)
        {
            if (axis == null) return;

            var axesProperty = axesSerializedProperty;
            var property = SerializedObjectHelper.AddArrayElement(axesProperty);
            axis.SerializeTo(property);
            axesProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 轴
        /// </summary>
        public class Axis
        {
            public string m_Name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;
            public float gravity;
            public float dead;
            public float sensitivity;
            public bool snap;
            public bool invert;
            public int type;  // Enum
            public int axis;  // Enum
            public int joyNum;    // Enum

            /// <summary>
            /// 构造
            /// </summary>
            public Axis() { }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="axis"></param>
            /// <param name="sensitivity"></param>
            public Axis(string name, int type, int axis, float sensitivity)
            {
                this.m_Name = name;
                this.descriptiveName = "";
                this.descriptiveNegativeName = "";
                this.negativeButton = "";
                this.positiveButton = "";
                this.altNegativeButton = "";
                this.altPositiveButton = "";
                this.gravity = 0.0f;
                this.dead = 0.001f;
                this.sensitivity = sensitivity;
                this.snap = false;
                this.invert = false;
                this.type = type;
                this.axis = axis;
                this.joyNum = 0;
            }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="device"></param>
            /// <param name="analog"></param>
            public Axis(int device, int analog)
            {
                this.m_Name = string.Format("joystick {0} analog {1}", device, analog);
                this.descriptiveName = "";
                this.descriptiveNegativeName = "";
                this.negativeButton = "";
                this.positiveButton = "";
                this.altNegativeButton = "";
                this.altPositiveButton = "";
                this.gravity = 0.0f;
                this.dead = 0.001f;
                this.sensitivity = 1.0f;
                this.snap = false;
                this.invert = false;
                this.type = 2;
                this.axis = analog;
                this.joyNum = device;
            }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="name"></param>
            /// <param name="id"></param>
            /// <param name="isButton"></param>
            public Axis(string name, int id, bool isButton)
            {
                this.m_Name = name;
                this.descriptiveName = "";
                this.descriptiveNegativeName = "";
                this.negativeButton = "";
                if (isButton) this.positiveButton = string.Format("joystick button {0}", id);
                else this.positiveButton = "";
                this.altNegativeButton = "";
                this.altPositiveButton = "";
                this.gravity = 1000.0f;
                this.dead = 0.001f;
                this.sensitivity = 1000.0f;
                this.snap = false;
                this.invert = false;
                this.type = 2;
                if (isButton) this.axis = 0;
                else this.axis = id;
                this.joyNum = 0;
            }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="axisSP"></param>
            public Axis(SerializedProperty axisSP)
            {
                SerializeFrom(axisSP);
            }
            
            /// <summary>
            /// 保留的名称
            /// </summary>
            public bool ReservedName
            {
                get
                {
                    if (Regex.Match(m_Name, @"^joystick \d+ analog \d+$").Success ||
                        Regex.Match(m_Name, @"^mouse (x|y|z)$").Success)
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// 获取名称
            /// </summary>
            /// <param name="axisSP"></param>
            /// <returns></returns>
            public static string GetName(SerializedProperty axisSP) => axisSP.FindPropertyRelative(nameof(m_Name)).stringValue;

            /// <summary>
            /// 从序列化属性对象序列化到当前对象
            /// </summary>
            /// <param name="axisSP"></param>
            public void SerializeFrom(SerializedProperty axisSP)
            {
                m_Name = axisSP.FindPropertyRelative(nameof(m_Name)).stringValue;
                descriptiveName = axisSP.FindPropertyRelative(nameof(descriptiveName)).stringValue;
                descriptiveNegativeName = axisSP.FindPropertyRelative(nameof(descriptiveNegativeName)).stringValue;
                negativeButton = axisSP.FindPropertyRelative(nameof(negativeButton)).stringValue;
                positiveButton = axisSP.FindPropertyRelative(nameof(positiveButton)).stringValue;
                altNegativeButton = axisSP.FindPropertyRelative(nameof(altNegativeButton)).stringValue;
                altPositiveButton = axisSP.FindPropertyRelative(nameof(altPositiveButton)).stringValue;
                gravity = axisSP.FindPropertyRelative(nameof(gravity)).floatValue;
                dead = axisSP.FindPropertyRelative(nameof(dead)).floatValue;
                sensitivity = axisSP.FindPropertyRelative(nameof(sensitivity)).floatValue;
                snap = axisSP.FindPropertyRelative(nameof(snap)).boolValue;
                invert = axisSP.FindPropertyRelative(nameof(invert)).boolValue;
                type = axisSP.FindPropertyRelative(nameof(type)).intValue;
                axis = axisSP.FindPropertyRelative(nameof(axis)).intValue;
                joyNum = axisSP.FindPropertyRelative(nameof(joyNum)).intValue;
            }

            /// <summary>
            /// 当前对象序列化到序列属性化对象
            /// </summary>
            /// <param name="axisSP"></param>
            public void SerializeTo(SerializedProperty axisSP)
            {
                axisSP.FindPropertyRelative(nameof(m_Name)).stringValue = m_Name;
                axisSP.FindPropertyRelative(nameof(descriptiveName)).stringValue = descriptiveName;
                axisSP.FindPropertyRelative(nameof(descriptiveNegativeName)).stringValue = descriptiveNegativeName;
                axisSP.FindPropertyRelative(nameof(negativeButton)).stringValue = negativeButton;
                axisSP.FindPropertyRelative(nameof(positiveButton)).stringValue = positiveButton;
                axisSP.FindPropertyRelative(nameof(altNegativeButton)).stringValue = altNegativeButton;
                axisSP.FindPropertyRelative(nameof(altPositiveButton)).stringValue = altPositiveButton;
                axisSP.FindPropertyRelative(nameof(gravity)).floatValue = gravity;
                axisSP.FindPropertyRelative(nameof(dead)).floatValue = dead;
                axisSP.FindPropertyRelative(nameof(sensitivity)).floatValue = sensitivity;
                axisSP.FindPropertyRelative(nameof(snap)).boolValue = snap;
                axisSP.FindPropertyRelative(nameof(invert)).boolValue = invert;
                axisSP.FindPropertyRelative(nameof(type)).intValue = type;
                axisSP.FindPropertyRelative(nameof(axis)).intValue = axis;
                axisSP.FindPropertyRelative(nameof(joyNum)).intValue = joyNum;
            }

            /// <summary>
            /// 与序列化属性对象比较相等性
            /// </summary>
            /// <param name="axisSP"></param>
            /// <returns></returns>
            public bool EqualTo(SerializedProperty axisSP)
            {
                if (axisSP.FindPropertyRelative(nameof(m_Name)).stringValue != m_Name) return false;
                if (axisSP.FindPropertyRelative(nameof(descriptiveName)).stringValue != descriptiveName) return false;
                if (axisSP.FindPropertyRelative(nameof(descriptiveNegativeName)).stringValue != descriptiveNegativeName) return false;
                if (axisSP.FindPropertyRelative(nameof(negativeButton)).stringValue != negativeButton) return false;
                if (axisSP.FindPropertyRelative(nameof(positiveButton)).stringValue != positiveButton) return false;
                if (axisSP.FindPropertyRelative(nameof(altNegativeButton)).stringValue != altNegativeButton) return false;
                if (axisSP.FindPropertyRelative(nameof(altPositiveButton)).stringValue != altPositiveButton) return false;
                if (!Mathf.Approximately(axisSP.FindPropertyRelative(nameof(gravity)).floatValue, gravity)) return false;
                if (!Mathf.Approximately(axisSP.FindPropertyRelative(nameof(dead)).floatValue, dead)) return false;
                if (!Mathf.Approximately(axisSP.FindPropertyRelative(nameof(sensitivity)).floatValue, this.sensitivity)) return false;
                if (axisSP.FindPropertyRelative(nameof(snap)).boolValue != snap) return false;
                if (axisSP.FindPropertyRelative(nameof(invert)).boolValue != invert) return false;
                if (axisSP.FindPropertyRelative(nameof(type)).intValue != type) return false;
                if (axisSP.FindPropertyRelative(nameof(axis)).intValue != axis) return false;
                if (axisSP.FindPropertyRelative(nameof(joyNum)).intValue != joyNum) return false;

                return true;
            }
        }
    }
}
