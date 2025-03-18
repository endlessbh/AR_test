using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [Attributes.Icon]
    [DisallowMultipleComponent]
    [Name("网络动画")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    [RequireComponent(typeof(Animator))]
    public sealed class NetAnimator : NetMB
    {
        [Group("动画")]
        [Name("动画")]
        [SerializeField]
        private Animator _animator;

        [Name("数据")]
        [SyncVar(sync = false)]
        public AnimatorData data = new AnimatorData();

        [Readonly]
        [Name("目标数据")]
        public AnimatorData targetData = new AnimatorData();

        [Readonly]
        [Name("上一次数据")]
        public AnimatorData prevData = new AnimatorData();

        [Readonly]
        [Name("原始数据")]
        public AnimatorData originalData = new AnimatorData();

        public Animator animator
        {
            get
            {
                if (!_animator)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
            set => _animator = value;
        }

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();
            if (data.SetData(animator))
            {
                prevData = new AnimatorData();//保证至少同步一次
                targetData.SetData(data);
                originalData.SetData(data);
            }
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();
            if (!animator) return;
            originalData.SetAnimator(animator);
        }

        HashSet<int> waitReceive = new HashSet<int>();

        protected override bool OnTimedCheckChange()
        {
            if (waitReceive.Contains(version)) return false;
            return data.SetData(animator) && HasChange(prevData, data);
        }

        protected override string OnSerializeData()
        {
            waitReceive.Add(version);
            return data.ToJson();
        }

        protected override void OnDeserializeData(string data, Data dataObj)
        {
            if (animator && AnimatorData.FromJson(data) is AnimatorData animatorData)
            {
                waitReceive.RemoveWhere(i => i <= dataObj.version);
                if (CanOptimizable(dataObj))
                {
                    if (!HasChange(this.data, animatorData))
                    {
                        prevData.SetData(this.data);
                    }
                    return;
                }
                Debug.LogFormat("{0}->{1}", dataObj.userGuid, transform.parent.name);

                targetData = animatorData;

                targetData.SetAnimator(animator);
            }
        }

        private bool HasChange(AnimatorData lData, AnimatorData rData)
        {
            if (lData == rData) return false;
            if (lData.parameters.Count != rData.parameters.Count) return true;

            for (int i = 0; i < lData.parameters.Count; i++)
            {
                if (lData.parameters[i].HasChange(rData.parameters[i])) return true;
            }

            return false;
        }

        /// <summary>
        /// 动画数据
        /// </summary>
        [Name("动画数据")]
        [Serializable]
        public class AnimatorData : JsonObject<AnimatorData>
        {
            [Name("参数列表")]
            [ArrayElement(EArrayElementHandleRule.CanDelete)]
            public List<Parameter> parameters = new List<Parameter>();

            public void SetData(AnimatorData data)
            {
                parameters.Clear();
                foreach (var p in data.parameters)
                {
                    this.parameters.Add(new Parameter(p));
                }
            }

            public bool SetData(Animator animator)
            {
                //Debug.LogWarning("animator:" + CommonFun.GameObjectToString(animator.gameObject) + "==" + animator.isActiveAndEnabled);
                if (!animator || !animator.isActiveAndEnabled) return false;

                foreach (var p in parameters)
                {
                    p.SetValue(animator);
                }

                return true;
            }

            public void SetAnimator(Animator animator)
            {
                if (!animator || !animator.isActiveAndEnabled) return;
                foreach (var p in parameters)
                {
                    p.SetAnimator(animator);
                }
            }
        }

        /// <summary>
        /// 参数
        /// </summary>
        [Name("参数")]
        [Serializable]
        [Import]
        public class Parameter
        {
            [Readonly]
            [Name("类型")]
            public AnimatorControllerParameterType type;

            [Readonly]
            [Name("名称")]
            public string name;

            [Readonly]
            [Name("值")]
            public string value;

            [Json(false)]
            public bool boolValue => Converter.instance.ConvertTo<bool>(value);

            [Json(false)]
            public int intValue => Converter.instance.ConvertTo<int>(value);

            [Json(false)]
            public float floatValue => Converter.instance.ConvertTo<float>(value);

            public Parameter() { }
            public Parameter(Parameter parameter)
            {
                SetValue(parameter);
            }

            public void SetValue(Parameter parameter)
            {
                this.type = parameter.type;
                this.name = parameter.name;
                this.value = parameter.value;
            }

            public void SetValue(Animator animator)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Bool:
                        {
                            value = animator.GetBool(name).ToString();
                            break;
                        }
                    case AnimatorControllerParameterType.Int:
                        {
                            value = animator.GetInteger(name).ToString();
                            break;
                        }
                    case AnimatorControllerParameterType.Float:
                        {
                            value = animator.GetFloat(name).ToString();
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException("不支持" + type.ToString() + "参数类型");
                        }
                }
            }

            public void SetAnimator(Animator animator)
            {
                switch (type)
                {
                    case AnimatorControllerParameterType.Bool:
                        {
                            animator.SetBool(name, boolValue);
                            break;
                        }
                    case AnimatorControllerParameterType.Int:
                        {
                            animator.SetBool(name, boolValue);
                            break;
                        }
                    case AnimatorControllerParameterType.Float:
                        {
                            animator.SetBool(name, boolValue);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException("不支持" + type.ToString() + "参数类型");
                        }
                }
            }

            public bool HasChange(Parameter parameter)
            {
                if (this == parameter) return false;
                if (this.type != parameter.type) return true;
                if (this.name != parameter.name) return true;
                if (this.value != parameter.value) return true;
                return false;
            }
        }
    }
}
