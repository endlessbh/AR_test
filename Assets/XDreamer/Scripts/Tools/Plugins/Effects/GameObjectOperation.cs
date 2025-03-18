using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 游戏对象操作
    /// </summary>
    [Name("游戏对象操作")]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class GameObjectOperation : BaseEffect
    {
        public enum EOperationRule
        {
            [Name("无")]
            None,

            [Name("激活游戏对象")]
            ActiveGameObject,
        }

        /// <summary>
        /// 操作规则
        /// </summary>
        [Name("操作规则")]
        [EnumPopup]
        public EOperationRule _operationRule = EOperationRule.ActiveGameObject;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        public GameObject[] _gameObjects = new GameObject[0];

        /// <summary>
        /// 启用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject) => Operate(true);

        /// <summary>
        /// 禁用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject) => Operate(false);

        /// <summary>
        /// 操作
        /// </summary>
        private void Operate(bool enable)
        {
            foreach (var go in _gameObjects)
            {
                if (!go) continue;

                switch (_operationRule)
                {
                    case EOperationRule.ActiveGameObject:
                        {
                            go.XSetActive(enable);
                            break;
                        }
                }
            }
        }
    }
}
