using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 角色旋转通过XBox
    /// </summary>
    [Name("角色旋转通过XBox")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Rotate, /*nameof(XCharacterController),*/ nameof(CharacterTransformer))]
    [Tool(XBoxHelper.Title)]
    [RequireManager(typeof(XBoxManager))]
    [Owner(typeof(XBoxManager))]
    public class CharacterRotateByXBox : BaseCharacterRotateController, IXBox
    {
        /// <summary>
        /// 控制数据
        /// </summary>
        [Name("控制数据")]
        public XBoxControlData _controlData = new XBoxControlData();

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();
            var speedRealtime = _speedInfo.valueRealtime;

            _offset = _controlData.GetYOffset();
            _offset.y *= speedRealtime.y;

            if (_offset != Vector3.zero)
            {
                Rotate();
            }
        }
    }
}
