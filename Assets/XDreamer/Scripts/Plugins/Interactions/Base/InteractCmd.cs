using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;

namespace XCSJ.Extension.Interactions.Base
{
    /// <summary>
    /// 命令名称接口
    /// </summary>
    public interface ICmdName 
    { 
        string cmdName { get; }
    }

    /// <summary>
    /// 命令
    /// </summary>
    public class Cmd : ICmdName
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        public string _cmdName;

        /// <summary>
        /// 命令名称
        /// </summary>
        public virtual string cmdName => _cmdName;
    }

    /// <summary>
    /// 命令：用于交互器与被交互对象做交互的动作描述
    /// </summary>
    /// <typeparam name="TECmd"></typeparam>
    public class Cmd<TECmd> : Cmd where TECmd : Enum
    {
        [Name("命令")]
        [EnumPopup]
        public TECmd _cmd;
    }

    /// <summary>
    /// 命令列表
    /// </summary>
    public abstract class Cmds { }

    /// <summary>
    /// 枚举命令列表
    /// </summary>
    /// <typeparam name="TCmd"></typeparam>
    public class Cmds<TCmd> : Cmds where TCmd : Cmd, new()
    {
        /// <summary>
        /// 命令列表
        /// </summary>
        [Name("命令列表")]
        public List<TCmd> _cmds = new List<TCmd>();

        /// <summary>
        /// 命令名称
        /// </summary>
        public List<string> cmdNames => _cmds.ToList(item => item._cmdName);

        /// <summary>
        /// 是否存在命令名称
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public bool Exists(string cmdName)
        {
            return cmdNames.Exists(name => name == cmdName);
        }
    }

    /// <summary>
    /// 枚举命令列表
    /// </summary>
    /// <typeparam name="TECmd"></typeparam>
    /// <typeparam name="TCmd"></typeparam>
    public class Cmds<TECmd, TCmd> : Cmds<TCmd>
        where TECmd : Enum
        where TCmd : Cmd<TECmd>, new()
    {
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            var names = EnumTypeData.GetEnumTypeData(typeof(TECmd)).displayNames;
            var array = EnumCache<TECmd>.Array;
            for (int i = 0; i < array.Length; i++)
            {
                _cmds.Add(new TCmd() { _cmdName = names[i], _cmd = array[i] });
            }
        }

        /// <summary>
        /// 获取命令名称列表
        /// </summary>
        /// <param name="eCmds"></param>
        /// <returns></returns>
        public List<string> GetCmdNames(params TECmd[] eCmds)
        {
            return _cmds.Where(c => eCmds.Contains(c._cmd)).ToList(item => item._cmdName);
        }

        /// <summary>
        /// 获取命令名称
        /// </summary>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public string GetCmdName(TECmd eCmd)
        {
            return _cmds.FirstOrDefault(cmd => cmd._cmd.Equals(eCmd))?._cmdName;
        }

        /// <summary>
        /// 尝试获取命令枚举
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public bool TryGetECmd(string cmdName, out TECmd eCmd)
        {
            var cmd = _cmds.FirstOrDefault(c => c._cmdName == cmdName);
            if (cmd != null)
            {
                eCmd = cmd._cmd;
                return true;
            }
            eCmd = default;
            return false;
        }

        /// <summary>
        /// 是否存在命令枚举
        /// </summary>
        /// <param name="eCmd"></param>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public bool Exists(TECmd eCmd, string cmdName)
        {
            return _cmds.Exists(cmd => cmd._cmd.Equals(eCmd) && cmd._cmdName == cmdName);
        }
    }
}
