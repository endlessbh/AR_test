using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;

namespace XCSJ.PluginRepairman.Base
{
    /// <summary>
    /// 装配状态
    /// </summary>
    public enum EAssembleState
    {
        [Name("无")]
        None,

        [Name("装配态")]
        Assembled,

        [Name("拆卸态")]
        Disassembled,

        [Name("装配中")]
        AssemblyInProgress,

        [Name("拆卸中")]
        DisassemblyInProgress
    }
}
