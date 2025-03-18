using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.UGUI.InputFields
{
    /// <summary>
    /// 输入框属性获取: 输入框属性获取
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(InputFieldPropertyGet))]
    [Tip("输入框属性获取", "Input field property get")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class InputFieldPropertyGet : BasePropertyGet<InputFieldPropertyGet>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入框属性获取";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(InputFieldPropertyGet))]
        [Tip("输入框属性获取", "Input field property get")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// 输入框:输入框对象
        /// </summary>
        [Name("输入框")]
        [Tip("输入框对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InputField _InputField;
        
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
            
            /// <summary>
            /// Should Hide Mobile Input:Should Hide Mobile Input
            /// </summary>
            [Name("Should Hide Mobile Input")]
            [Tip("Should Hide Mobile Input")]
            [EnumFieldName("属性/Should Hide Mobile Input")]
            Property_shouldHideMobileInput = 1000,
            
            /// <summary>
            /// Text:Text
            /// </summary>
            [Name("Text")]
            [Tip("Text")]
            [EnumFieldName("属性/Text")]
            Property_text,
            
            /// <summary>
            /// Is Focused:Is Focused
            /// </summary>
            [Name("Is Focused")]
            [Tip("Is Focused")]
            [EnumFieldName("属性/Is Focused")]
            Property_isFocused,
            
            /// <summary>
            /// Caret Blink Rate:Caret Blink Rate
            /// </summary>
            [Name("Caret Blink Rate")]
            [Tip("Caret Blink Rate")]
            [EnumFieldName("属性/Caret Blink Rate")]
            Property_caretBlinkRate,
            
            /// <summary>
            /// Caret Width:Caret Width
            /// </summary>
            [Name("Caret Width")]
            [Tip("Caret Width")]
            [EnumFieldName("属性/Caret Width")]
            Property_caretWidth,
            
            /// <summary>
            /// Text Component:Text Component
            /// </summary>
            [Name("Text Component")]
            [Tip("Text Component")]
            [EnumFieldName("属性/Text Component")]
            Property_textComponent,
            
            /// <summary>
            /// Placeholder:Placeholder
            /// </summary>
            [Name("Placeholder")]
            [Tip("Placeholder")]
            [EnumFieldName("属性/Placeholder")]
            Property_placeholder,
            
            /// <summary>
            /// Caret Color:Caret Color
            /// </summary>
            [Name("Caret Color")]
            [Tip("Caret Color")]
            [EnumFieldName("属性/Caret Color")]
            Property_caretColor,
            
            /// <summary>
            /// Custom Caret Color:Custom Caret Color
            /// </summary>
            [Name("Custom Caret Color")]
            [Tip("Custom Caret Color")]
            [EnumFieldName("属性/Custom Caret Color")]
            Property_customCaretColor,
            
            /// <summary>
            /// Selection Color:Selection Color
            /// </summary>
            [Name("Selection Color")]
            [Tip("Selection Color")]
            [EnumFieldName("属性/Selection Color")]
            Property_selectionColor,
            
            /// <summary>
            /// Character Limit:Character Limit
            /// </summary>
            [Name("Character Limit")]
            [Tip("Character Limit")]
            [EnumFieldName("属性/Character Limit")]
            Property_characterLimit,
            
            /// <summary>
            /// Content Type:Content Type
            /// </summary>
            [Name("Content Type")]
            [Tip("Content Type")]
            [EnumFieldName("属性/Content Type")]
            Property_contentType,
            
            /// <summary>
            /// Line Type:Line Type
            /// </summary>
            [Name("Line Type")]
            [Tip("Line Type")]
            [EnumFieldName("属性/Line Type")]
            Property_lineType,
            
            /// <summary>
            /// Input Type:Input Type
            /// </summary>
            [Name("Input Type")]
            [Tip("Input Type")]
            [EnumFieldName("属性/Input Type")]
            Property_inputType,
            
            /// <summary>
            /// Keyboard Type:Keyboard Type
            /// </summary>
            [Name("Keyboard Type")]
            [Tip("Keyboard Type")]
            [EnumFieldName("属性/Keyboard Type")]
            Property_keyboardType,
            
            /// <summary>
            /// Character Validation:Character Validation
            /// </summary>
            [Name("Character Validation")]
            [Tip("Character Validation")]
            [EnumFieldName("属性/Character Validation")]
            Property_characterValidation,
            
            /// <summary>
            /// Read Only:Read Only
            /// </summary>
            [Name("Read Only")]
            [Tip("Read Only")]
            [EnumFieldName("属性/Read Only")]
            Property_readOnly,
            
            /// <summary>
            /// Multi Line:Multi Line
            /// </summary>
            [Name("Multi Line")]
            [Tip("Multi Line")]
            [EnumFieldName("属性/Multi Line")]
            Property_multiLine,
            
            /// <summary>
            /// Was Canceled:Was Canceled
            /// </summary>
            [Name("Was Canceled")]
            [Tip("Was Canceled")]
            [EnumFieldName("属性/Was Canceled")]
            Property_wasCanceled,
            
            /// <summary>
            /// Caret Position:Caret Position
            /// </summary>
            [Name("Caret Position")]
            [Tip("Caret Position")]
            [EnumFieldName("属性/Caret Position")]
            Property_caretPosition,
            
            /// <summary>
            /// Selection Anchor Position:Selection Anchor Position
            /// </summary>
            [Name("Selection Anchor Position")]
            [Tip("Selection Anchor Position")]
            [EnumFieldName("属性/Selection Anchor Position")]
            Property_selectionAnchorPosition,
            
            /// <summary>
            /// Selection Focus Position:Selection Focus Position
            /// </summary>
            [Name("Selection Focus Position")]
            [Tip("Selection Focus Position")]
            [EnumFieldName("属性/Selection Focus Position")]
            Property_selectionFocusPosition,
            
            /// <summary>
            /// Min Width:Min Width
            /// </summary>
            [Name("Min Width")]
            [Tip("Min Width")]
            [EnumFieldName("属性/Min Width")]
            Property_minWidth,
            
            /// <summary>
            /// Preferred Width:Preferred Width
            /// </summary>
            [Name("Preferred Width")]
            [Tip("Preferred Width")]
            [EnumFieldName("属性/Preferred Width")]
            Property_preferredWidth,
            
            /// <summary>
            /// Flexible Width:Flexible Width
            /// </summary>
            [Name("Flexible Width")]
            [Tip("Flexible Width")]
            [EnumFieldName("属性/Flexible Width")]
            Property_flexibleWidth,
            
            /// <summary>
            /// Min Height:Min Height
            /// </summary>
            [Name("Min Height")]
            [Tip("Min Height")]
            [EnumFieldName("属性/Min Height")]
            Property_minHeight,
            
            /// <summary>
            /// Preferred Height:Preferred Height
            /// </summary>
            [Name("Preferred Height")]
            [Tip("Preferred Height")]
            [EnumFieldName("属性/Preferred Height")]
            Property_preferredHeight,
            
            /// <summary>
            /// Flexible Height:Flexible Height
            /// </summary>
            [Name("Flexible Height")]
            [Tip("Flexible Height")]
            [EnumFieldName("属性/Flexible Height")]
            Property_flexibleHeight,
            
            /// <summary>
            /// Layout Priority:Layout Priority
            /// </summary>
            [Name("Layout Priority")]
            [Tip("Layout Priority")]
            [EnumFieldName("属性/Layout Priority")]
            Property_layoutPriority,
            
            /// <summary>
            /// Transition:Transition
            /// </summary>
            [Name("Transition")]
            [Tip("Transition")]
            [EnumFieldName("属性/Transition")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transition,
            
            /// <summary>
            /// Target Graphic:Target Graphic
            /// </summary>
            [Name("Target Graphic")]
            [Tip("Target Graphic")]
            [EnumFieldName("属性/Target Graphic")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_targetGraphic,
            
            /// <summary>
            /// Interactable:Interactable
            /// </summary>
            [Name("Interactable")]
            [Tip("Interactable")]
            [EnumFieldName("属性/Interactable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_interactable,
            
            /// <summary>
            /// Image:Image
            /// </summary>
            [Name("Image")]
            [Tip("Image")]
            [EnumFieldName("属性/Image")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_image,
            
            /// <summary>
            /// Animator:Animator
            /// </summary>
            [Name("Animator")]
            [Tip("Animator")]
            [EnumFieldName("属性/Animator")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_animator,
            
            /// <summary>
            /// Use GUI Layout:Use GUI Layout
            /// </summary>
            [Name("Use GUI Layout")]
            [Tip("Use GUI Layout")]
            [EnumFieldName("属性/Use GUI Layout")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            /// <summary>
            /// Enabled:Enabled
            /// </summary>
            [Name("Enabled")]
            [Tip("Enabled")]
            [EnumFieldName("属性/Enabled")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled,
            
            /// <summary>
            /// Is Active And Enabled:Is Active And Enabled
            /// </summary>
            [Name("Is Active And Enabled")]
            [Tip("Is Active And Enabled")]
            [EnumFieldName("属性/Is Active And Enabled")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_isActiveAndEnabled,
            
            /// <summary>
            /// Transform:Transform
            /// </summary>
            [Name("Transform")]
            [Tip("Transform")]
            [EnumFieldName("属性/Transform")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transform,
            
            /// <summary>
            /// Game Object:Game Object
            /// </summary>
            [Name("Game Object")]
            [Tip("Game Object")]
            [EnumFieldName("属性/Game Object")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_gameObject,
            
            /// <summary>
            /// Tag:Tag
            /// </summary>
            [Name("Tag")]
            [Tip("Tag")]
            [EnumFieldName("属性/Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// Name:Name
            /// </summary>
            [Name("Name")]
            [Tip("Name")]
            [EnumFieldName("属性/Name")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// Hide Flags:Hide Flags
            /// </summary>
            [Name("Hide Flags")]
            [Tip("Hide Flags")]
            [EnumFieldName("属性/Hide Flags")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// Compare Tag:Compare Tag
            /// </summary>
            [Name("Compare Tag")]
            [Tip("Compare Tag")]
            [EnumFieldName("方法/Compare Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String = 10000,
            
            /// <summary>
            /// Find Selectable:Find Selectable
            /// </summary>
            [Name("Find Selectable")]
            [Tip("Find Selectable")]
            [EnumFieldName("方法/Find Selectable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectable_Vector3,
            
            /// <summary>
            /// Find Selectable On Down:Find Selectable On Down
            /// </summary>
            [Name("Find Selectable On Down")]
            [Tip("Find Selectable On Down")]
            [EnumFieldName("方法/Find Selectable On Down")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnDown,
            
            /// <summary>
            /// Find Selectable On Left:Find Selectable On Left
            /// </summary>
            [Name("Find Selectable On Left")]
            [Tip("Find Selectable On Left")]
            [EnumFieldName("方法/Find Selectable On Left")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnLeft,
            
            /// <summary>
            /// Find Selectable On Right:Find Selectable On Right
            /// </summary>
            [Name("Find Selectable On Right")]
            [Tip("Find Selectable On Right")]
            [EnumFieldName("方法/Find Selectable On Right")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnRight,
            
            /// <summary>
            /// Find Selectable On Up:Find Selectable On Up
            /// </summary>
            [Name("Find Selectable On Up")]
            [Tip("Find Selectable On Up")]
            [EnumFieldName("方法/Find Selectable On Up")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_FindSelectableOnUp,
            
            /// <summary>
            /// Get Component:Get Component
            /// </summary>
            [Name("Get Component")]
            [Tip("Get Component")]
            [EnumFieldName("方法/Get Component")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// Get Hash Code:Get Hash Code
            /// </summary>
            [Name("Get Hash Code")]
            [Tip("Get Hash Code")]
            [EnumFieldName("方法/Get Hash Code")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// Get Instance ID:Get Instance ID
            /// </summary>
            [Name("Get Instance ID")]
            [Tip("Get Instance ID")]
            [EnumFieldName("方法/Get Instance ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// Get Type:Get Type
            /// </summary>
            [Name("Get Type")]
            [Tip("Get Type")]
            [EnumFieldName("方法/Get Type")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// Is Active:Is Active
            /// </summary>
            [Name("Is Active")]
            [Tip("Is Active")]
            [EnumFieldName("方法/Is Active")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsActive,
            
            /// <summary>
            /// Is Destroyed:Is Destroyed
            /// </summary>
            [Name("Is Destroyed")]
            [Tip("Is Destroyed")]
            [EnumFieldName("方法/Is Destroyed")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsDestroyed,
            
            /// <summary>
            /// Is Interactable:Is Interactable
            /// </summary>
            [Name("Is Interactable")]
            [Tip("Is Interactable")]
            [EnumFieldName("方法/Is Interactable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInteractable,
            
            /// <summary>
            /// Is Invoking:Is Invoking
            /// </summary>
            [Name("Is Invoking")]
            [Tip("Is Invoking")]
            [EnumFieldName("方法/Is Invoking")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// Is Invoking:Is Invoking
            /// </summary>
            [Name("Is Invoking")]
            [Tip("Is Invoking")]
            [EnumFieldName("方法/Is Invoking")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// Start Coroutine:Start Coroutine
            /// </summary>
            [Name("Start Coroutine")]
            [Tip("Start Coroutine")]
            [EnumFieldName("方法/Start Coroutine")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// To String:To String
            /// </summary>
            [Name("To String")]
            [Tip("To String")]
            [EnumFieldName("方法/To String")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
        }
        
        /// <summary>
        /// tag:tag
        /// </summary>
        [Name("tag")]
        [Tip("tag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// dir:dir
        /// </summary>
        [Name("dir")]
        [Tip("dir")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_FindSelectable_Vector3)]
        public Vector3PropertyValue _Method_FindSelectable_Vector3__dir = new Vector3PropertyValue();
        
        /// <summary>
        /// type:type
        /// </summary>
        [Name("type")]
        [Tip("type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 变量名:将获取到的属性值存储在变量名对应的变量中
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的变量中", "Store the obtained attribute value in the variable corresponding to the variable name")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variableName;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variableName);
        }

        #endregion

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(object value)
        {
            _variableName.TrySetOrAddSetHierarchyVarValue(value);
        }
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Property_shouldHideMobileInput:
                {
                    SetToVariable(_InputField.shouldHideMobileInput);
                    break;
                }
                case EPropertyName.Property_text:
                {
                    SetToVariable(_InputField.text);
                    break;
                }
                case EPropertyName.Property_isFocused:
                {
                    SetToVariable(_InputField.isFocused);
                    break;
                }
                case EPropertyName.Property_caretBlinkRate:
                {
                    SetToVariable(_InputField.caretBlinkRate);
                    break;
                }
                case EPropertyName.Property_caretWidth:
                {
                    SetToVariable(_InputField.caretWidth);
                    break;
                }
                case EPropertyName.Property_textComponent:
                {
                    SetToVariable(_InputField.textComponent);
                    break;
                }
                case EPropertyName.Property_placeholder:
                {
                    SetToVariable(_InputField.placeholder);
                    break;
                }
                case EPropertyName.Property_caretColor:
                {
                    SetToVariable(_InputField.caretColor);
                    break;
                }
                case EPropertyName.Property_customCaretColor:
                {
                    SetToVariable(_InputField.customCaretColor);
                    break;
                }
                case EPropertyName.Property_selectionColor:
                {
                    SetToVariable(_InputField.selectionColor);
                    break;
                }
                case EPropertyName.Property_characterLimit:
                {
                    SetToVariable(_InputField.characterLimit);
                    break;
                }
                case EPropertyName.Property_contentType:
                {
                    SetToVariable(_InputField.contentType);
                    break;
                }
                case EPropertyName.Property_lineType:
                {
                    SetToVariable(_InputField.lineType);
                    break;
                }
                case EPropertyName.Property_inputType:
                {
                    SetToVariable(_InputField.inputType);
                    break;
                }
                case EPropertyName.Property_keyboardType:
                {
                    SetToVariable(_InputField.keyboardType);
                    break;
                }
                case EPropertyName.Property_characterValidation:
                {
                    SetToVariable(_InputField.characterValidation);
                    break;
                }
                case EPropertyName.Property_readOnly:
                {
                    SetToVariable(_InputField.readOnly);
                    break;
                }
                case EPropertyName.Property_multiLine:
                {
                    SetToVariable(_InputField.multiLine);
                    break;
                }
                case EPropertyName.Property_wasCanceled:
                {
                    SetToVariable(_InputField.wasCanceled);
                    break;
                }
                case EPropertyName.Property_caretPosition:
                {
                    SetToVariable(_InputField.caretPosition);
                    break;
                }
                case EPropertyName.Property_selectionAnchorPosition:
                {
                    SetToVariable(_InputField.selectionAnchorPosition);
                    break;
                }
                case EPropertyName.Property_selectionFocusPosition:
                {
                    SetToVariable(_InputField.selectionFocusPosition);
                    break;
                }
                case EPropertyName.Property_minWidth:
                {
                    SetToVariable(_InputField.minWidth);
                    break;
                }
                case EPropertyName.Property_preferredWidth:
                {
                    SetToVariable(_InputField.preferredWidth);
                    break;
                }
                case EPropertyName.Property_flexibleWidth:
                {
                    SetToVariable(_InputField.flexibleWidth);
                    break;
                }
                case EPropertyName.Property_minHeight:
                {
                    SetToVariable(_InputField.minHeight);
                    break;
                }
                case EPropertyName.Property_preferredHeight:
                {
                    SetToVariable(_InputField.preferredHeight);
                    break;
                }
                case EPropertyName.Property_flexibleHeight:
                {
                    SetToVariable(_InputField.flexibleHeight);
                    break;
                }
                case EPropertyName.Property_layoutPriority:
                {
                    SetToVariable(_InputField.layoutPriority);
                    break;
                }
                case EPropertyName.Property_transition:
                {
                    SetToVariable(_InputField.transition);
                    break;
                }
                case EPropertyName.Property_targetGraphic:
                {
                    SetToVariable(_InputField.targetGraphic);
                    break;
                }
                case EPropertyName.Property_interactable:
                {
                    SetToVariable(_InputField.interactable);
                    break;
                }
                case EPropertyName.Property_image:
                {
                    SetToVariable(_InputField.image);
                    break;
                }
                case EPropertyName.Property_animator:
                {
                    SetToVariable(_InputField.animator);
                    break;
                }
                case EPropertyName.Property_useGUILayout:
                {
                    SetToVariable(_InputField.useGUILayout);
                    break;
                }
                case EPropertyName.Property_enabled:
                {
                    SetToVariable(_InputField.enabled);
                    break;
                }
                case EPropertyName.Property_isActiveAndEnabled:
                {
                    SetToVariable(_InputField.isActiveAndEnabled);
                    break;
                }
                case EPropertyName.Property_transform:
                {
                    SetToVariable(_InputField.transform);
                    break;
                }
                case EPropertyName.Property_gameObject:
                {
                    SetToVariable(_InputField.gameObject);
                    break;
                }
                case EPropertyName.Property_tag:
                {
                    SetToVariable(_InputField.tag);
                    break;
                }
                case EPropertyName.Property_name:
                {
                    SetToVariable(_InputField.name);
                    break;
                }
                case EPropertyName.Property_hideFlags:
                {
                    SetToVariable(_InputField.hideFlags);
                    break;
                }
                case EPropertyName.Method_CompareTag_String:
                {
                    SetToVariable(_InputField.CompareTag(_Method_CompareTag_String__tag.GetValue()));
                    break;
                }
                case EPropertyName.Method_FindSelectable_Vector3:
                {
                    SetToVariable(_InputField.FindSelectable(_Method_FindSelectable_Vector3__dir.GetValue()));
                    break;
                }
                case EPropertyName.Method_FindSelectableOnDown:
                {
                    SetToVariable(_InputField.FindSelectableOnDown());
                    break;
                }
                case EPropertyName.Method_FindSelectableOnLeft:
                {
                    SetToVariable(_InputField.FindSelectableOnLeft());
                    break;
                }
                case EPropertyName.Method_FindSelectableOnRight:
                {
                    SetToVariable(_InputField.FindSelectableOnRight());
                    break;
                }
                case EPropertyName.Method_FindSelectableOnUp:
                {
                    SetToVariable(_InputField.FindSelectableOnUp());
                    break;
                }
                case EPropertyName.Method_GetComponent_String:
                {
                    SetToVariable(_InputField.GetComponent(_Method_GetComponent_String__type.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetHashCode:
                {
                    SetToVariable(_InputField.GetHashCode());
                    break;
                }
                case EPropertyName.Method_GetInstanceID:
                {
                    SetToVariable(_InputField.GetInstanceID());
                    break;
                }
                case EPropertyName.Method_GetType:
                {
                    SetToVariable(_InputField.GetType());
                    break;
                }
                case EPropertyName.Method_IsActive:
                {
                    SetToVariable(_InputField.IsActive());
                    break;
                }
                case EPropertyName.Method_IsDestroyed:
                {
                    SetToVariable(_InputField.IsDestroyed());
                    break;
                }
                case EPropertyName.Method_IsInteractable:
                {
                    SetToVariable(_InputField.IsInteractable());
                    break;
                }
                case EPropertyName.Method_IsInvoking:
                {
                    SetToVariable(_InputField.IsInvoking());
                    break;
                }
                case EPropertyName.Method_IsInvoking_String:
                {
                    SetToVariable(_InputField.IsInvoking(_Method_IsInvoking_String__methodName.GetValue()));
                    break;
                }
                case EPropertyName.Method_StartCoroutine_String:
                {
                    SetToVariable(_InputField.StartCoroutine(_Method_StartCoroutine_String__methodName.GetValue()));
                    break;
                }
                case EPropertyName.Method_ToString:
                {
                    SetToVariable(_InputField.ToString());
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
            return _variableName + " = " + CommonFun.Name(_propertyName);
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity();
        }
        
    }
}
