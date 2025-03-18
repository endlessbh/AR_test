using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 射线目标命令枚举
    /// </summary>
    public enum ERayTargetCmd
    {
        /// <summary>
        /// 增加射线长度
        /// </summary>
        [Name("增加射线长度")]
        IncreaseRayLength,

        /// <summary>
        /// 减少射线长度
        /// </summary>
        [Name("减少射线长度")]
        DecreaseRayLength,
    }

    /// <summary>
    /// 射线目标命令
    /// </summary>
    [Serializable]
    public class RayTargetCmd : Cmd<ERayTargetCmd> { }

    /// <summary>
    /// 射线目标命令列表
    /// </summary>
    [Serializable]
    public class RayTargetCmds : Cmds<ERayTargetCmd, RayTargetCmd> { }

    /// <summary>
    /// 射线碰撞检测器:
    /// 1、射线原点、朝向构建一个焦点
    /// 2、当射线与场景内游戏对象有碰撞点时，碰撞点为焦点；当射线与场景游戏对象没有碰撞点时，使用射线原点、朝向和长度值构建的虚拟点作为焦点
    /// 3、可使用交互命令来控制射线长度值
    /// </summary>
    [Name("射线碰撞检测器")]
    [Tip("1、射线原点、朝向构建一个目标点\n2、当射线与场景有碰撞点时，目标点为碰撞点；当射线与场景没有碰撞点时，使用射线原点、朝向和长度值构建目标点\n3、使用交互器命令来控制射线长度值", "1. Construct a target point with the origin and orientation of the ray  n2. When there is a collision point between the ray and the scene, the target point is the collision point; When there is no collision point between the ray and the scene, use the ray origin, orientation, and length values to construct the target point.  n3. Use the interactive command to control the ray length value")]
    [Tool(InteractionCategory.InteractorInput, rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public class RayHitDetector : Interactor
    {
        /// <summary>
        /// 射线目标命令列表
        /// </summary>
        [Name("射线目标命令列表")]
        [OnlyMemberElements]
        public RayTargetCmds _rayTargetCmds = new RayTargetCmds();

        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _rayTargetCmds.cmdNames;

        /// <summary>
        /// 射线碰撞器
        /// </summary>
        [Name("射线碰撞器")]
        public RayHitter _rayHitter = new RayHitter();

        /// <summary>
        /// 射线长度
        /// </summary>
        [Name("射线长度")]
        [Min(0)]
        public float _rayLength = 1000;

        /// <summary>
        /// 射线长度步长
        /// </summary>
        [Name("射线长度步长")]
        [Min(0)]
        public float _rayLenghtOffset = 0.1f;

        /// <summary>
        /// 焦点：当射线与场景内游戏对象有碰撞点时，碰撞点为焦点；当射线与场景游戏对象没有碰撞点时，使用射线原点、朝向和长度值构建的虚拟点作为焦点
        /// </summary>
        public Vector3 focusPoint
        {
            get
            {
                if (_rayHitter.TryGetHit(out var ray, out var raycastHit))
                {
                    return raycastHit.point;
                }
                return ray.origin + ray.direction * _rayLength;
            }
        }

        /// <summary>
        /// 获取射线碰撞数据
        /// </summary>
        public RaycastHit? raycastHit
        {
            get
            {
                if(_rayHitter.TryGetHit(out var raycastHit))
                {
                    return raycastHit;
                }
                return default;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => _rayTargetCmds.Reset();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => true;

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_rayTargetCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case ERayTargetCmd.IncreaseRayLength:
                        {
                            _rayLength += _rayLenghtOffset;
                            break;
                        }
                    case ERayTargetCmd.DecreaseRayLength:
                        {
                            _rayLength -= _rayLenghtOffset;
                            if (_rayLength < 0)
                            {
                                _rayLength = 0;
                            }
                            break;
                        }
                }
            }
            return base.OnInteract(interactData);
        }
    }
}
