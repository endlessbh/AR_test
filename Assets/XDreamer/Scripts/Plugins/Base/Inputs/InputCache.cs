using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入缓存
    /// </summary>
    public class InputCache : TICache<InputCache, EInput, IInput>
    {
        /// <summary>
        /// 构建值
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        protected override KeyValuePair<bool, IInput> CreateValue(EInput key1)
        {
            return new KeyValuePair<bool, IInput>(true, new InputList(key1));
        }
    }
}
