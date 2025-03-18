using static UnityEngine.UI.InputField;
using static UnityEngine.UI.Selectable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS;

namespace UnityEngine.UI.States
{
    /// <summary>
    /// 输入框属性设置: 输入框属性设置
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(InputFieldPropertySet))]
    [Tip("输入框属性设置", "Input field property set")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class InputFieldPropertySet : BasePropertySet<InputFieldPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入框属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(InputFieldPropertySet))]
        [Tip("输入框属性设置", "Input field property set")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// Input Field:Input Field
        /// </summary>
        [Name("Input Field")]
        [Tip("Input Field")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InputField _InputField;
        
        /// <summary>
        /// Input Field:Input Field
        /// </summary>
        public InputField InputField => this.XGetComponentInGlobal(ref _InputField, true);
        
        /// <summary>
        /// Input Field列表:Input Field列表
        /// </summary>
        [Name("Input Field列表")]
        [Tip("Input Field列表")]
        public List<InputField> _InputFields = new List<InputField>();
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.None;
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName 
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("无")]
            [EnumFieldName("无")]
            None,
            
            #region 属性
            
            /// <summary>
            /// Caret Blink Rate(属性):Caret Blink Rate
            /// </summary>
            [Name("Caret Blink Rate(属性)")]
            [Tip("Caret Blink Rate")]
            [EnumFieldName("属性/Caret Blink Rate")]
            Property_caretBlinkRate = 1000,
            
            /// <summary>
            /// Caret Color(属性):Caret Color
            /// </summary>
            [Name("Caret Color(属性)")]
            [Tip("Caret Color")]
            [EnumFieldName("属性/Caret Color")]
            Property_caretColor,
            
            /// <summary>
            /// Caret Position(属性):Caret Position
            /// </summary>
            [Name("Caret Position(属性)")]
            [Tip("Caret Position")]
            [EnumFieldName("属性/Caret Position")]
            Property_caretPosition,
            
            /// <summary>
            /// Caret Width(属性):Caret Width
            /// </summary>
            [Name("Caret Width(属性)")]
            [Tip("Caret Width")]
            [EnumFieldName("属性/Caret Width")]
            Property_caretWidth,
            
            /// <summary>
            /// Character Limit(属性):Character Limit
            /// </summary>
            [Name("Character Limit(属性)")]
            [Tip("Character Limit")]
            [EnumFieldName("属性/Character Limit")]
            Property_characterLimit,
            
            /// <summary>
            /// Character Validation(属性):Character Validation
            /// </summary>
            [Name("Character Validation(属性)")]
            [Tip("Character Validation")]
            [EnumFieldName("属性/Character Validation")]
            Property_characterValidation,
            
            /// <summary>
            /// Content Type(属性):Content Type
            /// </summary>
            [Name("Content Type(属性)")]
            [Tip("Content Type")]
            [EnumFieldName("属性/Content Type")]
            Property_contentType,
            
            /// <summary>
            /// Custom Caret Color(属性):Custom Caret Color
            /// </summary>
            [Name("Custom Caret Color(属性)")]
            [Tip("Custom Caret Color")]
            [EnumFieldName("属性/Custom Caret Color")]
            Property_customCaretColor,
            
            /// <summary>
            /// 启用(属性):启用
            /// </summary>
            [Name("启用(属性)")]
            [Tip("启用")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled,
            
            /// <summary>
            /// 隐藏标识(属性):隐藏标识
            /// </summary>
            [Name("隐藏标识(属性)")]
            [Tip("隐藏标识")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// Image(属性):Image
            /// </summary>
            [Name("Image(属性)")]
            [Tip("Image")]
            [EnumFieldName("属性/Image")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_image,
            
            /// <summary>
            /// Input Type(属性):Input Type
            /// </summary>
            [Name("Input Type(属性)")]
            [Tip("Input Type")]
            [EnumFieldName("属性/Input Type")]
            Property_inputType,
            
            /// <summary>
            /// Interactable(属性):Interactable
            /// </summary>
            [Name("Interactable(属性)")]
            [Tip("Interactable")]
            [EnumFieldName("属性/Interactable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_interactable,
            
            /// <summary>
            /// Keyboard Type(属性):Keyboard Type
            /// </summary>
            [Name("Keyboard Type(属性)")]
            [Tip("Keyboard Type")]
            [EnumFieldName("属性/Keyboard Type")]
            Property_keyboardType,
            
            /// <summary>
            /// Line Type(属性):Line Type
            /// </summary>
            [Name("Line Type(属性)")]
            [Tip("Line Type")]
            [EnumFieldName("属性/Line Type")]
            Property_lineType,
            
            /// <summary>
            /// 名称(属性):名称
            /// </summary>
            [Name("名称(属性)")]
            [Tip("名称")]
            [EnumFieldName("属性/名称")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// Placeholder(属性):Placeholder
            /// </summary>
            [Name("Placeholder(属性)")]
            [Tip("Placeholder")]
            [EnumFieldName("属性/Placeholder")]
            Property_placeholder,
            
            /// <summary>
            /// Read Only(属性):Read Only
            /// </summary>
            [Name("Read Only(属性)")]
            [Tip("Read Only")]
            [EnumFieldName("属性/Read Only")]
            Property_readOnly,
            
            /// <summary>
            /// Selection Anchor Position(属性):Selection Anchor Position
            /// </summary>
            [Name("Selection Anchor Position(属性)")]
            [Tip("Selection Anchor Position")]
            [EnumFieldName("属性/Selection Anchor Position")]
            Property_selectionAnchorPosition,
            
            /// <summary>
            /// Selection Color(属性):Selection Color
            /// </summary>
            [Name("Selection Color(属性)")]
            [Tip("Selection Color")]
            [EnumFieldName("属性/Selection Color")]
            Property_selectionColor,
            
            /// <summary>
            /// Selection Focus Position(属性):Selection Focus Position
            /// </summary>
            [Name("Selection Focus Position(属性)")]
            [Tip("Selection Focus Position")]
            [EnumFieldName("属性/Selection Focus Position")]
            Property_selectionFocusPosition,
            
            /// <summary>
            /// Should Hide Mobile Input(属性):Should Hide Mobile Input
            /// </summary>
            [Name("Should Hide Mobile Input(属性)")]
            [Tip("Should Hide Mobile Input")]
            [EnumFieldName("属性/Should Hide Mobile Input")]
            Property_shouldHideMobileInput,
            
            /// <summary>
            /// 标签(属性):标签
            /// </summary>
            [Name("标签(属性)")]
            [Tip("标签")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// Target Graphic(属性):Target Graphic
            /// </summary>
            [Name("Target Graphic(属性)")]
            [Tip("Target Graphic")]
            [EnumFieldName("属性/Target Graphic")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_targetGraphic,
            
            /// <summary>
            /// Text(属性):Text
            /// </summary>
            [Name("Text(属性)")]
            [Tip("Text")]
            [EnumFieldName("属性/Text")]
            Property_text,
            
            /// <summary>
            /// Text Component(属性):Text Component
            /// </summary>
            [Name("Text Component(属性)")]
            [Tip("Text Component")]
            [EnumFieldName("属性/Text Component")]
            Property_textComponent,
            
            /// <summary>
            /// Transition(属性):Transition
            /// </summary>
            [Name("Transition(属性)")]
            [Tip("Transition")]
            [EnumFieldName("属性/Transition")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transition,
            
            /// <summary>
            /// 使用GUI布局(属性):使用GUI布局
            /// </summary>
            [Name("使用GUI布局(属性)")]
            [Tip("使用GUI布局")]
            [EnumFieldName("属性/使用GUI布局")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// Activate Input Field(方法):Activate Input Field
            /// </summary>
            [Name("Activate Input Field(方法)")]
            [Tip("Activate Input Field")]
            [EnumFieldName("方法/Activate Input Field")]
            Method_ActivateInputField = 10000,
            
            /// <summary>
            /// 广播消息(方法名)(方法):广播消息(方法名)
            /// </summary>
            [Name("广播消息(方法名)(方法)")]
            [Tip("广播消息(方法名)")]
            [EnumFieldName("方法/广播消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String,
            
            /// <summary>
            /// 广播消息(方法名+发送消息选项)(方法):广播消息(方法名+发送消息选项)
            /// </summary>
            [Name("广播消息(方法名+发送消息选项)(方法)")]
            [Tip("广播消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/广播消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,
            
            /// <summary>
            /// Calculate Layout Input Horizontal(方法):Calculate Layout Input Horizontal
            /// </summary>
            [Name("Calculate Layout Input Horizontal(方法)")]
            [Tip("Calculate Layout Input Horizontal")]
            [EnumFieldName("方法/Calculate Layout Input Horizontal")]
            Method_CalculateLayoutInputHorizontal,
            
            /// <summary>
            /// Calculate Layout Input Vertical(方法):Calculate Layout Input Vertical
            /// </summary>
            [Name("Calculate Layout Input Vertical(方法)")]
            [Tip("Calculate Layout Input Vertical")]
            [EnumFieldName("方法/Calculate Layout Input Vertical")]
            Method_CalculateLayoutInputVertical,
            
            /// <summary>
            /// 取消调用(方法):取消调用
            /// </summary>
            [Name("取消调用(方法)")]
            [Tip("取消调用")]
            [EnumFieldName("方法/取消调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke,
            
            /// <summary>
            /// 取消调用(方法名)(方法):取消调用(方法名)
            /// </summary>
            [Name("取消调用(方法名)(方法)")]
            [Tip("取消调用(方法名)")]
            [EnumFieldName("方法/取消调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke_String,
            
            /// <summary>
            /// 比较标签(标签)(方法):比较标签(标签)
            /// </summary>
            [Name("比较标签(标签)(方法)")]
            [Tip("比较标签(标签)")]
            [EnumFieldName("方法/比较标签(标签)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,
            
            /// <summary>
            /// Deactivate Input Field(方法):Deactivate Input Field
            /// </summary>
            [Name("Deactivate Input Field(方法)")]
            [Tip("Deactivate Input Field")]
            [EnumFieldName("方法/Deactivate Input Field")]
            Method_DeactivateInputField,
            
            /// <summary>
            /// Find Selectable(方法):Find Selectable
            /// </summary>
            [Name("Find Selectable(方法)")]
            [Tip("Find Selectable")]
            [EnumFieldName("方法/Find Selectable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectable_Vector3,
            
            /// <summary>
            /// Find Selectable On Down(方法):Find Selectable On Down
            /// </summary>
            [Name("Find Selectable On Down(方法)")]
            [Tip("Find Selectable On Down")]
            [EnumFieldName("方法/Find Selectable On Down")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnDown,
            
            /// <summary>
            /// Find Selectable On Left(方法):Find Selectable On Left
            /// </summary>
            [Name("Find Selectable On Left(方法)")]
            [Tip("Find Selectable On Left")]
            [EnumFieldName("方法/Find Selectable On Left")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnLeft,
            
            /// <summary>
            /// Find Selectable On Right(方法):Find Selectable On Right
            /// </summary>
            [Name("Find Selectable On Right(方法)")]
            [Tip("Find Selectable On Right")]
            [EnumFieldName("方法/Find Selectable On Right")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnRight,
            
            /// <summary>
            /// Find Selectable On Up(方法):Find Selectable On Up
            /// </summary>
            [Name("Find Selectable On Up(方法)")]
            [Tip("Find Selectable On Up")]
            [EnumFieldName("方法/Find Selectable On Up")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnUp,
            
            /// <summary>
            /// Force Label Update(方法):Force Label Update
            /// </summary>
            [Name("Force Label Update(方法)")]
            [Tip("Force Label Update")]
            [EnumFieldName("方法/Force Label Update")]
            Method_ForceLabelUpdate,
            
            /// <summary>
            /// 获取组件(类型)(方法):获取组件(类型)
            /// </summary>
            [Name("获取组件(类型)(方法)")]
            [Tip("获取组件(类型)")]
            [EnumFieldName("方法/获取组件(类型)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// 获取哈希码(方法):获取哈希码
            /// </summary>
            [Name("获取哈希码(方法)")]
            [Tip("获取哈希码")]
            [EnumFieldName("方法/获取哈希码")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// 获取实例ID(方法):获取实例ID
            /// </summary>
            [Name("获取实例ID(方法)")]
            [Tip("获取实例ID")]
            [EnumFieldName("方法/获取实例ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// 获取类型(方法):获取类型
            /// </summary>
            [Name("获取类型(方法)")]
            [Tip("获取类型")]
            [EnumFieldName("方法/获取类型")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// Graphic Update Complete(方法):Graphic Update Complete
            /// </summary>
            [Name("Graphic Update Complete(方法)")]
            [Tip("Graphic Update Complete")]
            [EnumFieldName("方法/Graphic Update Complete")]
            Method_GraphicUpdateComplete,
            
            /// <summary>
            /// 是否调用(方法名+时间)(方法):是否调用(方法名+时间)
            /// </summary>
            [Name("是否调用(方法名+时间)(方法)")]
            [Tip("是否调用(方法名+时间)")]
            [EnumFieldName("方法/是否调用(方法名+时间)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Invoke_String_Single,
            
            /// <summary>
            /// 重复调用(方法):重复调用
            /// </summary>
            [Name("重复调用(方法)")]
            [Tip("重复调用")]
            [EnumFieldName("方法/重复调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_InvokeRepeating_String_Single_Single,
            
            /// <summary>
            /// Is Active(方法):Is Active
            /// </summary>
            [Name("Is Active(方法)")]
            [Tip("Is Active")]
            [EnumFieldName("方法/Is Active")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsActive,
            
            /// <summary>
            /// Is Destroyed(方法):Is Destroyed
            /// </summary>
            [Name("Is Destroyed(方法)")]
            [Tip("Is Destroyed")]
            [EnumFieldName("方法/Is Destroyed")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsDestroyed,
            
            /// <summary>
            /// Is Interactable(方法):Is Interactable
            /// </summary>
            [Name("Is Interactable(方法)")]
            [Tip("Is Interactable")]
            [EnumFieldName("方法/Is Interactable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInteractable,
            
            /// <summary>
            /// 是否调用(方法):是否调用
            /// </summary>
            [Name("是否调用(方法)")]
            [Tip("是否调用")]
            [EnumFieldName("方法/是否调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// 是否调用(方法名)(方法):是否调用(方法名)
            /// </summary>
            [Name("是否调用(方法名)(方法)")]
            [Tip("是否调用(方法名)")]
            [EnumFieldName("方法/是否调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// Layout Complete(方法):Layout Complete
            /// </summary>
            [Name("Layout Complete(方法)")]
            [Tip("Layout Complete")]
            [EnumFieldName("方法/Layout Complete")]
            Method_LayoutComplete,
            
            /// <summary>
            /// Move Text End(方法):Move Text End
            /// </summary>
            [Name("Move Text End(方法)")]
            [Tip("Move Text End")]
            [EnumFieldName("方法/Move Text End")]
            Method_MoveTextEnd_Boolean,
            
            /// <summary>
            /// Move Text Start(方法):Move Text Start
            /// </summary>
            [Name("Move Text Start(方法)")]
            [Tip("Move Text Start")]
            [EnumFieldName("方法/Move Text Start")]
            Method_MoveTextStart_Boolean,
            
            /// <summary>
            /// Rebuild(方法):Rebuild
            /// </summary>
            [Name("Rebuild(方法)")]
            [Tip("Rebuild")]
            [EnumFieldName("方法/Rebuild")]
            Method_Rebuild_CanvasUpdate,
            
            /// <summary>
            /// Select(方法):Select
            /// </summary>
            [Name("Select(方法)")]
            [Tip("Select")]
            [EnumFieldName("方法/Select")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Select,

            /// <summary>
            /// 发送消息(方法名)(方法):发送消息(方法名)
            /// </summary>
            [Name("发送消息(方法名)(方法)")]
            [Tip("发送消息(方法名)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,

            /// <summary>
            /// 发送消息(方法名+发送消息选项)(方法):发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)(方法)")]
            [Tip("发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,

            /// <summary>
            /// 向上发送消息(方法名)(方法):向上发送消息(方法名)
            /// </summary>
            [Name("向上发送消息(方法名)(方法)")]
            [Tip("向上发送消息(方法名)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,

            /// <summary>
            /// 向上发送消息(方法名+发送消息选项)(方法):向上发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)(方法)")]
            [Tip("向上发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,
            
            /// <summary>
            /// 启动协程(方法):启动协程
            /// </summary>
            [Name("启动协程(方法)")]
            [Tip("启动协程")]
            [EnumFieldName("方法/启动协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// 停止全部协程(方法):停止全部协程
            /// </summary>
            [Name("停止全部协程(方法)")]
            [Tip("停止全部协程")]
            [EnumFieldName("方法/停止全部协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopAllCoroutines,
            
            /// <summary>
            /// 停止协程(方法):停止协程
            /// </summary>
            [Name("停止协程(方法)")]
            [Tip("停止协程")]
            [EnumFieldName("方法/停止协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopCoroutine_String,
            
            /// <summary>
            /// 转字符串(方法):转字符串
            /// </summary>
            [Name("转字符串(方法)")]
            [Tip("转字符串")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            #endregion
            
        }
        
        #region 属性
        
        /// <summary>
        /// Caret Blink Rate:Caret Blink Rate
        /// </summary>
        [Name("Caret Blink Rate")]
        [Tip("Caret Blink Rate")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_caretBlinkRate)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_caretBlinkRate = new FloatPropertyValue();
        
        /// <summary>
        /// Caret Color:Caret Color
        /// </summary>
        [Name("Caret Color")]
        [Tip("Caret Color")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_caretColor)]
        [OnlyMemberElements]
        public ColorPropertyValue _Property_caretColor = new ColorPropertyValue();
        
        /// <summary>
        /// Caret Position:Caret Position
        /// </summary>
        [Name("Caret Position")]
        [Tip("Caret Position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_caretPosition)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_caretPosition = new IntPropertyValue();
        
        /// <summary>
        /// Caret Width:Caret Width
        /// </summary>
        [Name("Caret Width")]
        [Tip("Caret Width")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_caretWidth)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_caretWidth = new IntPropertyValue();
        
        /// <summary>
        /// Character Limit:Character Limit
        /// </summary>
        [Name("Character Limit")]
        [Tip("Character Limit")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_characterLimit)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_characterLimit = new IntPropertyValue();
        
        /// <summary>
        /// Character Validation属性值
        /// </summary>
        [Serializable]
        public class CharacterValidationPropertyValue : EnumPropertyValue<CharacterValidation>
        {
        }
        
        /// <summary>
        /// Character Validation:Character Validation
        /// </summary>
        [Name("Character Validation")]
        [Tip("Character Validation")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_characterValidation)]
        [OnlyMemberElements]
        public CharacterValidationPropertyValue _Property_characterValidation = new CharacterValidationPropertyValue();
        
        /// <summary>
        /// Content Type属性值
        /// </summary>
        [Serializable]
        public class ContentTypePropertyValue : EnumPropertyValue<ContentType>
        {
        }
        
        /// <summary>
        /// Content Type:Content Type
        /// </summary>
        [Name("Content Type")]
        [Tip("Content Type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_contentType)]
        [OnlyMemberElements]
        public ContentTypePropertyValue _Property_contentType = new ContentTypePropertyValue();
        
        /// <summary>
        /// Custom Caret Color:Custom Caret Color
        /// </summary>
        [Name("Custom Caret Color")]
        [Tip("Custom Caret Color")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_customCaretColor)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_customCaretColor = new BoolPropertyValue();
        
        /// <summary>
        /// 启用:启用
        /// </summary>
        [Name("启用")]
        [Tip("启用")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_enabled)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_enabled = new BoolPropertyValue();
        
        /// <summary>
        /// 隐藏标识:隐藏标识
        /// </summary>
        [Name("隐藏标识")]
        [Tip("隐藏标识")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_hideFlags)]
        [OnlyMemberElements]
        public HideFlagsPropertyValue _Property_hideFlags = new HideFlagsPropertyValue();
        
        /// <summary>
        /// Image:Image
        /// </summary>
        [Name("Image")]
        [Tip("Image")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_image)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Property_image = new UnityObjectPropertyValue();
        
        /// <summary>
        /// Input Type属性值
        /// </summary>
        [Serializable]
        public class InputTypePropertyValue : EnumPropertyValue<InputType>
        {
        }
        
        /// <summary>
        /// Input Type:Input Type
        /// </summary>
        [Name("Input Type")]
        [Tip("Input Type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_inputType)]
        [OnlyMemberElements]
        public InputTypePropertyValue _Property_inputType = new InputTypePropertyValue();
        
        /// <summary>
        /// Interactable:Interactable
        /// </summary>
        [Name("Interactable")]
        [Tip("Interactable")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_interactable)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_interactable = new BoolPropertyValue();
        
        /// <summary>
        /// Touch Screen Keyboard Type属性值
        /// </summary>
        [Serializable]
        public class TouchScreenKeyboardTypePropertyValue : EnumPropertyValue<TouchScreenKeyboardType>
        {
        }
        
        /// <summary>
        /// Keyboard Type:Keyboard Type
        /// </summary>
        [Name("Keyboard Type")]
        [Tip("Keyboard Type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_keyboardType)]
        [OnlyMemberElements]
        public TouchScreenKeyboardTypePropertyValue _Property_keyboardType = new TouchScreenKeyboardTypePropertyValue();
        
        /// <summary>
        /// Line Type属性值
        /// </summary>
        [Serializable]
        public class LineTypePropertyValue : EnumPropertyValue<LineType>
        {
        }
        
        /// <summary>
        /// Line Type:Line Type
        /// </summary>
        [Name("Line Type")]
        [Tip("Line Type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_lineType)]
        [OnlyMemberElements]
        public LineTypePropertyValue _Property_lineType = new LineTypePropertyValue();
        
        /// <summary>
        /// 名称:名称
        /// </summary>
        [Name("名称")]
        [Tip("名称")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_name)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_name = new StringPropertyValue();
        
        /// <summary>
        /// Placeholder:Placeholder
        /// </summary>
        [Name("Placeholder")]
        [Tip("Placeholder")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_placeholder)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Property_placeholder = new UnityObjectPropertyValue();
        
        /// <summary>
        /// Read Only:Read Only
        /// </summary>
        [Name("Read Only")]
        [Tip("Read Only")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_readOnly)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_readOnly = new BoolPropertyValue();
        
        /// <summary>
        /// Selection Anchor Position:Selection Anchor Position
        /// </summary>
        [Name("Selection Anchor Position")]
        [Tip("Selection Anchor Position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_selectionAnchorPosition)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_selectionAnchorPosition = new IntPropertyValue();
        
        /// <summary>
        /// Selection Color:Selection Color
        /// </summary>
        [Name("Selection Color")]
        [Tip("Selection Color")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_selectionColor)]
        [OnlyMemberElements]
        public ColorPropertyValue _Property_selectionColor = new ColorPropertyValue();
        
        /// <summary>
        /// Selection Focus Position:Selection Focus Position
        /// </summary>
        [Name("Selection Focus Position")]
        [Tip("Selection Focus Position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_selectionFocusPosition)]
        [OnlyMemberElements]
        public IntPropertyValue _Property_selectionFocusPosition = new IntPropertyValue();
        
        /// <summary>
        /// Should Hide Mobile Input:Should Hide Mobile Input
        /// </summary>
        [Name("Should Hide Mobile Input")]
        [Tip("Should Hide Mobile Input")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_shouldHideMobileInput)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_shouldHideMobileInput = new BoolPropertyValue();
        
        /// <summary>
        /// 标签:标签
        /// </summary>
        [Name("标签")]
        [Tip("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_tag)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_tag = new StringPropertyValue();
        
        /// <summary>
        /// Target Graphic:Target Graphic
        /// </summary>
        [Name("Target Graphic")]
        [Tip("Target Graphic")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_targetGraphic)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Property_targetGraphic = new UnityObjectPropertyValue();
        
        /// <summary>
        /// Text:Text
        /// </summary>
        [Name("Text")]
        [Tip("Text")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_text)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_text = new StringPropertyValue();
        
        /// <summary>
        /// Text Component:Text Component
        /// </summary>
        [Name("Text Component")]
        [Tip("Text Component")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_textComponent)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Property_textComponent = new UnityObjectPropertyValue();
        
        /// <summary>
        /// Transition属性值
        /// </summary>
        [Serializable]
        public class TransitionPropertyValue : EnumPropertyValue<Selectable.Transition>
        {
        }
        
        /// <summary>
        /// Transition:Transition
        /// </summary>
        [Name("Transition")]
        [Tip("Transition")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_transition)]
        [OnlyMemberElements]
        public TransitionPropertyValue _Property_transition = new TransitionPropertyValue();
        
        /// <summary>
        /// 使用GUI布局:使用GUI布局
        /// </summary>
        [Name("使用GUI布局")]
        [Tip("使用GUI布局")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_useGUILayout)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_useGUILayout = new BoolPropertyValue();
        
        #endregion
        
        #region 方法
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CancelInvoke_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CancelInvoke_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 标签:标签
        /// </summary>
        [Name("标签")]
        [Tip("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// dir:dir
        /// </summary>
        [Name("dir")]
        [Tip("dir")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_FindSelectable_Vector3)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_FindSelectable_Vector3__dir = new Vector3PropertyValue();
        
        /// <summary>
        /// 类型:类型
        /// </summary>
        [Name("类型")]
        [Tip("类型")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_Invoke_String_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 时间:时间
        /// </summary>
        [Name("时间")]
        [Tip("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_Invoke_String_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_InvokeRepeating_String_Single_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 时间:时间
        /// </summary>
        [Name("时间")]
        [Tip("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// 重复率:重复率
        /// </summary>
        [Name("重复率")]
        [Tip("重复率")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__repeatRate = new FloatPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// shift:shift
        /// </summary>
        [Name("shift")]
        [Tip("shift")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_MoveTextEnd_Boolean)]
        [OnlyMemberElements]
        public BoolPropertyValue _Method_MoveTextEnd_Boolean__shift = new BoolPropertyValue();
        
        /// <summary>
        /// shift:shift
        /// </summary>
        [Name("shift")]
        [Tip("shift")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_MoveTextStart_Boolean)]
        [OnlyMemberElements]
        public BoolPropertyValue _Method_MoveTextStart_Boolean__shift = new BoolPropertyValue();
        
        /// <summary>
        /// Canvas Update属性值
        /// </summary>
        [Serializable]
        public class CanvasUpdatePropertyValue : EnumPropertyValue<CanvasUpdate>
        {
        }
        
        /// <summary>
        /// update:update
        /// </summary>
        [Name("update")]
        [Tip("update")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Rebuild_CanvasUpdate)]
        [OnlyMemberElements]
        public CanvasUpdatePropertyValue _Method_Rebuild_CanvasUpdate__update = new CanvasUpdatePropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StopCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StopCoroutine_String__methodName = new StringPropertyValue();
        
        #endregion
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_caretBlinkRate:
                    {
                        var value = _Property_caretBlinkRate.GetValue();
                        if (_InputField != null) _InputField.caretBlinkRate = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.caretBlinkRate = value;
                        }
                        break;
                    }
                case EPropertyName.Property_caretColor:
                    {
                        var value = _Property_caretColor.GetValue();
                        if (_InputField != null) _InputField.caretColor = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.caretColor = value;
                        }
                        break;
                    }
                case EPropertyName.Property_caretPosition:
                    {
                        var value = _Property_caretPosition.GetValue();
                        if (_InputField != null) _InputField.caretPosition = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.caretPosition = value;
                        }
                        break;
                    }
                case EPropertyName.Property_caretWidth:
                    {
                        var value = _Property_caretWidth.GetValue();
                        if (_InputField != null) _InputField.caretWidth = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.caretWidth = value;
                        }
                        break;
                    }
                case EPropertyName.Property_characterLimit:
                    {
                        var value = _Property_characterLimit.GetValue();
                        if (_InputField != null) _InputField.characterLimit = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.characterLimit = value;
                        }
                        break;
                    }
                case EPropertyName.Property_characterValidation:
                    {
                        var value = _Property_characterValidation.GetValue();
                        if (_InputField != null) _InputField.characterValidation = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.characterValidation = value;
                        }
                        break;
                    }
                case EPropertyName.Property_contentType:
                    {
                        var value = _Property_contentType.GetValue();
                        if (_InputField != null) _InputField.contentType = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.contentType = value;
                        }
                        break;
                    }
                case EPropertyName.Property_customCaretColor:
                    {
                        var value = _Property_customCaretColor.GetValue();
                        if (_InputField != null) _InputField.customCaretColor = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.customCaretColor = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_InputField != null) _InputField.enabled = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_InputField != null) _InputField.hideFlags = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_image:
                    {
                        var value = _Property_image.GetValue() as Image;
                        if (_InputField != null) _InputField.image = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.image = value;
                        }
                        break;
                    }
                case EPropertyName.Property_inputType:
                    {
                        var value = _Property_inputType.GetValue();
                        if (_InputField != null) _InputField.inputType = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.inputType = value;
                        }
                        break;
                    }
                case EPropertyName.Property_interactable:
                    {
                        var value = _Property_interactable.GetValue();
                        if (_InputField != null) _InputField.interactable = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.interactable = value;
                        }
                        break;
                    }
                case EPropertyName.Property_keyboardType:
                    {
                        var value = _Property_keyboardType.GetValue();
                        if (_InputField != null) _InputField.keyboardType = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.keyboardType = value;
                        }
                        break;
                    }
                case EPropertyName.Property_lineType:
                    {
                        var value = _Property_lineType.GetValue();
                        if (_InputField != null) _InputField.lineType = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.lineType = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_InputField != null) _InputField.name = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_placeholder:
                    {
                        var value = _Property_placeholder.GetValue() as Graphic;
                        if (_InputField != null) _InputField.placeholder = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.placeholder = value;
                        }
                        break;
                    }
                case EPropertyName.Property_readOnly:
                    {
                        var value = _Property_readOnly.GetValue();
                        if (_InputField != null) _InputField.readOnly = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.readOnly = value;
                        }
                        break;
                    }
                case EPropertyName.Property_selectionAnchorPosition:
                    {
                        var value = _Property_selectionAnchorPosition.GetValue();
                        if (_InputField != null) _InputField.selectionAnchorPosition = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.selectionAnchorPosition = value;
                        }
                        break;
                    }
                case EPropertyName.Property_selectionColor:
                    {
                        var value = _Property_selectionColor.GetValue();
                        if (_InputField != null) _InputField.selectionColor = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.selectionColor = value;
                        }
                        break;
                    }
                case EPropertyName.Property_selectionFocusPosition:
                    {
                        var value = _Property_selectionFocusPosition.GetValue();
                        if (_InputField != null) _InputField.selectionFocusPosition = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.selectionFocusPosition = value;
                        }
                        break;
                    }
                case EPropertyName.Property_shouldHideMobileInput:
                    {
                        var value = _Property_shouldHideMobileInput.GetValue();
                        if (_InputField != null) _InputField.shouldHideMobileInput = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.shouldHideMobileInput = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_InputField != null) _InputField.tag = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_targetGraphic:
                    {
                        var value = _Property_targetGraphic.GetValue() as Graphic;
                        if (_InputField != null) _InputField.targetGraphic = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.targetGraphic = value;
                        }
                        break;
                    }
                case EPropertyName.Property_text:
                    {
                        var value = _Property_text.GetValue();
                        if (_InputField != null) _InputField.text = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.text = value;
                        }
                        break;
                    }
                case EPropertyName.Property_textComponent:
                    {
                        var value = _Property_textComponent.GetValue() as Text;
                        if (_InputField != null) _InputField.textComponent = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.textComponent = value;
                        }
                        break;
                    }
                case EPropertyName.Property_transition:
                    {
                        var value = _Property_transition.GetValue();
                        if (_InputField != null) _InputField.transition = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.transition = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_InputField != null) _InputField.useGUILayout = value;
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Method_ActivateInputField:
                    {
                        if (_InputField != null) _InputField.ActivateInputField();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.ActivateInputField();
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_InputField != null) _InputField.BroadcastMessage(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_InputField != null) _InputField.BroadcastMessage(methodName, options);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CalculateLayoutInputHorizontal:
                    {
                        if (_InputField != null) _InputField.CalculateLayoutInputHorizontal();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.CalculateLayoutInputHorizontal();
                        }
                        break;
                    }
                case EPropertyName.Method_CalculateLayoutInputVertical:
                    {
                        if (_InputField != null) _InputField.CalculateLayoutInputVertical();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.CalculateLayoutInputVertical();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_InputField != null) _InputField.CancelInvoke();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_InputField != null) _InputField.CancelInvoke(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_InputField != null) _InputField.CompareTag(tag);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_DeactivateInputField:
                    {
                        if (_InputField != null) _InputField.DeactivateInputField();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.DeactivateInputField();
                        }
                        break;
                    }
                case EPropertyName.Method_FindSelectable_Vector3:
                    {
                        var dir = _Method_FindSelectable_Vector3__dir.GetValue();
                        if (_InputField != null) _InputField.FindSelectable(dir);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.FindSelectable(dir);
                        }
                        break;
                    }
                case EPropertyName.Method_FindSelectableOnDown:
                    {
                        if (_InputField != null) _InputField.FindSelectableOnDown();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.FindSelectableOnDown();
                        }
                        break;
                    }
                case EPropertyName.Method_FindSelectableOnLeft:
                    {
                        if (_InputField != null) _InputField.FindSelectableOnLeft();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.FindSelectableOnLeft();
                        }
                        break;
                    }
                case EPropertyName.Method_FindSelectableOnRight:
                    {
                        if (_InputField != null) _InputField.FindSelectableOnRight();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.FindSelectableOnRight();
                        }
                        break;
                    }
                case EPropertyName.Method_FindSelectableOnUp:
                    {
                        if (_InputField != null) _InputField.FindSelectableOnUp();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.FindSelectableOnUp();
                        }
                        break;
                    }
                case EPropertyName.Method_ForceLabelUpdate:
                    {
                        if (_InputField != null) _InputField.ForceLabelUpdate();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.ForceLabelUpdate();
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_InputField != null) _InputField.GetComponent(type);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_InputField != null) _InputField.GetHashCode();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_InputField != null) _InputField.GetInstanceID();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_InputField != null) _InputField.GetType();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_GraphicUpdateComplete:
                    {
                        if (_InputField != null) _InputField.GraphicUpdateComplete();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.GraphicUpdateComplete();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_InputField != null) _InputField.Invoke(methodName, time);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.Invoke(methodName, time);
                        }
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        var methodName = _Method_InvokeRepeating_String_Single_Single__methodName.GetValue();
                        var time = _Method_InvokeRepeating_String_Single_Single__time.GetValue();
                        var repeatRate = _Method_InvokeRepeating_String_Single_Single__repeatRate.GetValue();
                        if (_InputField != null) _InputField.InvokeRepeating(methodName, time, repeatRate);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsActive:
                    {
                        if (_InputField != null) _InputField.IsActive();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.IsActive();
                        }
                        break;
                    }
                case EPropertyName.Method_IsDestroyed:
                    {
                        if (_InputField != null) _InputField.IsDestroyed();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.IsDestroyed();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInteractable:
                    {
                        if (_InputField != null) _InputField.IsInteractable();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.IsInteractable();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_InputField != null) _InputField.IsInvoking();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_InputField != null) _InputField.IsInvoking(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_LayoutComplete:
                    {
                        if (_InputField != null) _InputField.LayoutComplete();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.LayoutComplete();
                        }
                        break;
                    }
                case EPropertyName.Method_MoveTextEnd_Boolean:
                    {
                        var shift = _Method_MoveTextEnd_Boolean__shift.GetValue();
                        if (_InputField != null) _InputField.MoveTextEnd(shift);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.MoveTextEnd(shift);
                        }
                        break;
                    }
                case EPropertyName.Method_MoveTextStart_Boolean:
                    {
                        var shift = _Method_MoveTextStart_Boolean__shift.GetValue();
                        if (_InputField != null) _InputField.MoveTextStart(shift);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.MoveTextStart(shift);
                        }
                        break;
                    }
                case EPropertyName.Method_Rebuild_CanvasUpdate:
                    {
                        var update = _Method_Rebuild_CanvasUpdate__update.GetValue();
                        if (_InputField != null) _InputField.Rebuild(update);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.Rebuild(update);
                        }
                        break;
                    }
                case EPropertyName.Method_Select:
                    {
                        if (_InputField != null) _InputField.Select();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.Select();
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_InputField != null) _InputField.SendMessage(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_InputField != null) _InputField.SendMessage(methodName, options);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_InputField != null) _InputField.SendMessageUpwards(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_InputField != null) _InputField.SendMessageUpwards(methodName, options);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_InputField != null) _InputField.StartCoroutine(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_InputField != null) _InputField.StopAllCoroutines();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_InputField != null) _InputField.StopCoroutine(methodName);
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_InputField != null) _InputField.ToString();
                        foreach(var obj in _InputFields)
                        {
                            if (obj != null) obj.ToString();
                        }
                        break;
                    }
            }
        }
        
        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_caretBlinkRate:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_caretBlinkRate.ToFriendlyString();
                    }
                case EPropertyName.Property_caretColor:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_caretColor.ToFriendlyString();
                    }
                case EPropertyName.Property_caretPosition:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_caretPosition.ToFriendlyString();
                    }
                case EPropertyName.Property_caretWidth:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_caretWidth.ToFriendlyString();
                    }
                case EPropertyName.Property_characterLimit:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_characterLimit.ToFriendlyString();
                    }
                case EPropertyName.Property_characterValidation:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_characterValidation.ToFriendlyString();
                    }
                case EPropertyName.Property_contentType:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_contentType.ToFriendlyString();
                    }
                case EPropertyName.Property_customCaretColor:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_customCaretColor.ToFriendlyString();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_enabled.ToFriendlyString();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_hideFlags.ToFriendlyString();
                    }
                case EPropertyName.Property_image:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_image.ToFriendlyString();
                    }
                case EPropertyName.Property_inputType:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_inputType.ToFriendlyString();
                    }
                case EPropertyName.Property_interactable:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_interactable.ToFriendlyString();
                    }
                case EPropertyName.Property_keyboardType:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_keyboardType.ToFriendlyString();
                    }
                case EPropertyName.Property_lineType:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_lineType.ToFriendlyString();
                    }
                case EPropertyName.Property_name:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_name.ToFriendlyString();
                    }
                case EPropertyName.Property_placeholder:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_placeholder.ToFriendlyString();
                    }
                case EPropertyName.Property_readOnly:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_readOnly.ToFriendlyString();
                    }
                case EPropertyName.Property_selectionAnchorPosition:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_selectionAnchorPosition.ToFriendlyString();
                    }
                case EPropertyName.Property_selectionColor:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_selectionColor.ToFriendlyString();
                    }
                case EPropertyName.Property_selectionFocusPosition:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_selectionFocusPosition.ToFriendlyString();
                    }
                case EPropertyName.Property_shouldHideMobileInput:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_shouldHideMobileInput.ToFriendlyString();
                    }
                case EPropertyName.Property_tag:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_tag.ToFriendlyString();
                    }
                case EPropertyName.Property_targetGraphic:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_targetGraphic.ToFriendlyString();
                    }
                case EPropertyName.Property_text:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_text.ToFriendlyString();
                    }
                case EPropertyName.Property_textComponent:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_textComponent.ToFriendlyString();
                    }
                case EPropertyName.Property_transition:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_transition.ToFriendlyString();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_useGUILayout.ToFriendlyString();
                    }
                case EPropertyName.Method_ActivateInputField:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CalculateLayoutInputHorizontal:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CalculateLayoutInputVertical:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_DeactivateInputField:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_FindSelectable_Vector3:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_FindSelectableOnDown:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_FindSelectableOnLeft:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_FindSelectableOnRight:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_FindSelectableOnUp:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_ForceLabelUpdate:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetType:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GraphicUpdateComplete:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsActive:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsDestroyed:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInteractable:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_LayoutComplete:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_MoveTextEnd_Boolean:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_MoveTextStart_Boolean:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Rebuild_CanvasUpdate:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Select:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_ToString:
                    {
                        return CommonFun.Name(_propertyName);
                    }
            }
            return base.ToFriendlyString();
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_caretBlinkRate:
                    {
                        return _InputField && _Property_caretBlinkRate.DataValidity();
                    }
                case EPropertyName.Property_caretColor:
                    {
                        return _InputField && _Property_caretColor.DataValidity();
                    }
                case EPropertyName.Property_caretPosition:
                    {
                        return _InputField && _Property_caretPosition.DataValidity();
                    }
                case EPropertyName.Property_caretWidth:
                    {
                        return _InputField && _Property_caretWidth.DataValidity();
                    }
                case EPropertyName.Property_characterLimit:
                    {
                        return _InputField && _Property_characterLimit.DataValidity();
                    }
                case EPropertyName.Property_characterValidation:
                    {
                        return _InputField && _Property_characterValidation.DataValidity();
                    }
                case EPropertyName.Property_contentType:
                    {
                        return _InputField && _Property_contentType.DataValidity();
                    }
                case EPropertyName.Property_customCaretColor:
                    {
                        return _InputField && _Property_customCaretColor.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _InputField && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _InputField && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_image:
                    {
                        return _InputField && _Property_image.DataValidity();
                    }
                case EPropertyName.Property_inputType:
                    {
                        return _InputField && _Property_inputType.DataValidity();
                    }
                case EPropertyName.Property_interactable:
                    {
                        return _InputField && _Property_interactable.DataValidity();
                    }
                case EPropertyName.Property_keyboardType:
                    {
                        return _InputField && _Property_keyboardType.DataValidity();
                    }
                case EPropertyName.Property_lineType:
                    {
                        return _InputField && _Property_lineType.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _InputField && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_placeholder:
                    {
                        return _InputField && _Property_placeholder.DataValidity();
                    }
                case EPropertyName.Property_readOnly:
                    {
                        return _InputField && _Property_readOnly.DataValidity();
                    }
                case EPropertyName.Property_selectionAnchorPosition:
                    {
                        return _InputField && _Property_selectionAnchorPosition.DataValidity();
                    }
                case EPropertyName.Property_selectionColor:
                    {
                        return _InputField && _Property_selectionColor.DataValidity();
                    }
                case EPropertyName.Property_selectionFocusPosition:
                    {
                        return _InputField && _Property_selectionFocusPosition.DataValidity();
                    }
                case EPropertyName.Property_shouldHideMobileInput:
                    {
                        return _InputField && _Property_shouldHideMobileInput.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _InputField && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_targetGraphic:
                    {
                        return _InputField && _Property_targetGraphic.DataValidity();
                    }
                case EPropertyName.Property_text:
                    {
                        return _InputField && _Property_text.DataValidity();
                    }
                case EPropertyName.Property_textComponent:
                    {
                        return _InputField && _Property_textComponent.DataValidity();
                    }
                case EPropertyName.Property_transition:
                    {
                        return _InputField && _Property_transition.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _InputField && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Method_ActivateInputField:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _InputField && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if(!_InputField) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CalculateLayoutInputHorizontal:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_CalculateLayoutInputVertical:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _InputField && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _InputField && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_DeactivateInputField:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_FindSelectable_Vector3:
                    {
                        return _InputField && _Method_FindSelectable_Vector3__dir.DataValidity();
                    }
                case EPropertyName.Method_FindSelectableOnDown:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_FindSelectableOnLeft:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_FindSelectableOnRight:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_FindSelectableOnUp:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_ForceLabelUpdate:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _InputField && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_GraphicUpdateComplete:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if(!_InputField) return false;
                        if(!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if(!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if(!_InputField) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsActive:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_IsDestroyed:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_IsInteractable:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _InputField && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_LayoutComplete:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_MoveTextEnd_Boolean:
                    {
                        return _InputField && _Method_MoveTextEnd_Boolean__shift.DataValidity();
                    }
                case EPropertyName.Method_MoveTextStart_Boolean:
                    {
                        return _InputField && _Method_MoveTextStart_Boolean__shift.DataValidity();
                    }
                case EPropertyName.Method_Rebuild_CanvasUpdate:
                    {
                        return _InputField && _Method_Rebuild_CanvasUpdate__update.DataValidity();
                    }
                case EPropertyName.Method_Select:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _InputField && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if(!_InputField) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _InputField && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if(!_InputField) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _InputField && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _InputField;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _InputField && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _InputField;
                    }
            }
            return base.DataValidity();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
            if (InputField) { }
        }
        
    }
}
