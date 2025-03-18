using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 角色移动通过XBox
    /// </summary>
    [Name("角色移动通过XBox")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Move, /*nameof(XCharacterController),*/ nameof(CharacterTransformer))]
    [Tool(XBoxHelper.Title)]
    [RequireManager(typeof(XBoxManager))]
    [Owner(typeof(XBoxManager))]
    public class CharacterMoveByXBox : BaseCharacterToolController, IXBox
    {
        /// <summary>
        /// 移动模式
        /// </summary>
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.Local;

        /// <summary>
        /// 控制数据
        /// </summary>
        [Name("控制数据")]
        public XBoxControlData _controlData = new XBoxControlData();

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var moveDirection = _controlData.GetOffset();

            moveDirection.y = moveDirection.y >= _controlData._pyDeadZone.x ? CharacterTransformer.JumpValue : 0;

            if (moveDirection != Vector3.zero)
            {
                mainController.Move(moveDirection, (int)_moveMode);
            }
        }
    }
}
