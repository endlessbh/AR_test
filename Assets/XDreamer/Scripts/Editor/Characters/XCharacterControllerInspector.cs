using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.EditorTools;
using XCSJ.Extension.Characters;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginsCameras.Tools.Controllers;
using System.Linq;
using XCSJ.Extension.Base.Algorithms;

namespace XCSJ.EditorExtension.Characters
{
    /// <summary>
    /// 角色控制器检查器
    /// </summary>
    [Name("角色控制器检查器")]
    [CustomEditor(typeof(XCharacterController), true)]
    public class XCharacterControllerInspector : BaseMainControllerInspector<XCharacterController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(XCharacterController)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CategoryListExtension.DrawVertical(categoryList);
        }

        /// <summary>
        /// 左键相机与右键角色旋转控制模式
        /// </summary>
        [Name("左键相机与右键角色旋转控制模式")]
        public bool leftCameraRightCharacter = true;

        /// <summary>
        /// 左键角色与右键相机旋转控制模式
        /// </summary>
        [Name("左键角色与右键相机旋转控制模式")]
        public bool leftCharacterRightCamera = true;

        /// <summary>
        /// 左右键角色旋转控制模式
        /// </summary>
        [Name("左右键角色旋转控制模式")]
        public bool leftRightCharacter = true;

        /// <summary>
        /// 旋转速度基础值
        /// </summary>
        [Name("旋转速度基础值")]
        public bool rotateSpeedBaseValue = true;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);
            switch(serializedProperty.name)
            {
                case nameof(XCharacterController._characterTransformer):
                    {
                        var character = this.targetObject;
                        if (character.characterTransformer) { }

                        #region 移动

                        var characterMoveSpeedByInput = character.GetComponentInChildren<CharacterMoveSpeedByInput>();
                        if (characterMoveSpeedByInput && characterMoveSpeedByInput._overrideSpeed)
                        {
                            EditorGUI.BeginChangeCheck();
                            var walkSpeed = EditorGUILayout.DelayedFloatField(typeof(CharacterMoveSpeedByInput).TrLabel(nameof(characterMoveSpeedByInput._walkSpeed)), characterMoveSpeedByInput._walkSpeed);
                            var runSpeed = EditorGUILayout.DelayedFloatField(typeof(CharacterMoveSpeedByInput).TrLabel(nameof(characterMoveSpeedByInput._runSpeed)), characterMoveSpeedByInput._runSpeed);
                            if (EditorGUI.EndChangeCheck())
                            {
                                characterMoveSpeedByInput.XModifyProperty(() =>
                                {
                                    characterMoveSpeedByInput._walkSpeed = walkSpeed;
                                    characterMoveSpeedByInput._runSpeed = runSpeed;
                                });
                            }
                        }
                        else
                        {
                            var pd = targetPropertyCache.GetPropertyData(nameof(XCharacterController._speed));
                            EditorGUILayout.PropertyField(pd.serializedProperty, pd.trLabel);
                        }

                        #endregion

                        #region 旋转

                        var characterRotateByInput1 = character.GetComponentInChildren<CharacterRotateByInput>();
                        if (characterRotateByInput1)
                        {
                            EditorGUI.BeginChangeCheck();
                            var valueNew = EditorGUILayout.Vector3Field(TrLabel(nameof(rotateSpeedBaseValue)), characterRotateByInput1._speedInfo._value);
                            if (EditorGUI.EndChangeCheck())
                            {
                                characterRotateByInput1.XModifyProperty(ref characterRotateByInput1._speedInfo._value, valueNew);
                            }
                        }

                        #region 左键相机与右键角色旋转控制模式
                        if (GUILayout.Button(TrLabel(nameof(leftCameraRightCharacter))))
                        {
                            //左键相机
                            var cameraRotateByInput = this.targetObject.characterCameraController.cameraMainController.cameraTransformer.GetComponent<CameraRotateByInput>();
                            if (cameraRotateByInput)
                            {
                                cameraRotateByInput.XModifyProperty(() =>
                                {
                                    var has = false;
                                    for (int i = 0; i < cameraRotateByInput._yInputAxis._mouseButtons.Count; i++)
                                    {
                                        switch (cameraRotateByInput._yInputAxis._mouseButtons[i])
                                        {
                                            case EMouseButton.Left:
                                                {
                                                    has = true;
                                                    break;
                                                }
                                            case EMouseButton.Right:
                                                {
                                                    has = true;
                                                    cameraRotateByInput._yInputAxis._mouseButtons[i] = EMouseButton.Left;
                                                    break;
                                                }
                                        }
                                    }
                                    if (!has)
                                    {
                                        cameraRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Left);
                                    }
                                    //去重
                                    cameraRotateByInput._yInputAxis._mouseButtons = cameraRotateByInput._yInputAxis._mouseButtons.Distinct().ToList();
                                });
                            }

                            //右键角色
                            var characterRotateByInput = character.GetComponentInChildren<CharacterRotateByInput>();
                            if (characterRotateByInput)
                            {
                                characterRotateByInput.XModifyProperty(() =>
                                {
                                    var has = false;
                                    for (int i = 0; i < characterRotateByInput._yInputAxis._mouseButtons.Count; i++)
                                    {
                                        switch (characterRotateByInput._yInputAxis._mouseButtons[i])
                                        {
                                            case EMouseButton.Right:
                                                {
                                                    has = true;
                                                    break;
                                                }
                                            case EMouseButton.Left:
                                                {
                                                    has = true;
                                                    characterRotateByInput._yInputAxis._mouseButtons[i] = EMouseButton.Right;
                                                    break;
                                                }
                                        }
                                    }
                                    if (!has)
                                    {
                                        characterRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Right);
                                    }
                                    //去重
                                    characterRotateByInput._yInputAxis._mouseButtons = characterRotateByInput._yInputAxis._mouseButtons.Distinct().ToList();
                                });
                            }
                        }
                        #endregion

                        #region 左键角色与右键相机旋转控制模式
                        if (GUILayout.Button(TrLabel(nameof(leftCharacterRightCamera))))
                        {
                            //左键角色
                            var characterRotateByInput = character.GetComponentInChildren<CharacterRotateByInput>();
                            if (characterRotateByInput)
                            {
                                characterRotateByInput.XModifyProperty(() =>
                                {
                                    var has = false;
                                    for (int i = 0; i < characterRotateByInput._yInputAxis._mouseButtons.Count; i++)
                                    {
                                        switch (characterRotateByInput._yInputAxis._mouseButtons[i])
                                        {
                                            case EMouseButton.Right:
                                                {
                                                    has = true;
                                                    characterRotateByInput._yInputAxis._mouseButtons[i] = EMouseButton.Left;
                                                    break;
                                                }
                                            case EMouseButton.Left:
                                                {
                                                    has = true;
                                                    break;
                                                }
                                        }
                                    }
                                    if (!has)
                                    {
                                        characterRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Left);
                                    }
                                    //去重
                                    characterRotateByInput._yInputAxis._mouseButtons = characterRotateByInput._yInputAxis._mouseButtons.Distinct().ToList();
                                });
                            }

                            //右键相机
                            var cameraRotateByInput = this.targetObject.characterCameraController.cameraMainController.cameraTransformer.GetComponent<CameraRotateByInput>();
                            if (cameraRotateByInput)
                            {
                                cameraRotateByInput.XModifyProperty(() =>
                                {
                                    var has = false;
                                    for (int i = 0; i < cameraRotateByInput._yInputAxis._mouseButtons.Count; i++)
                                    {
                                        switch (cameraRotateByInput._yInputAxis._mouseButtons[i])
                                        {
                                            case EMouseButton.Right:
                                                {
                                                    has = true;
                                                    break;
                                                }
                                            case EMouseButton.Left:
                                                {
                                                    has = true;
                                                    cameraRotateByInput._yInputAxis._mouseButtons[i] = EMouseButton.Right;
                                                    break;
                                                }
                                        }
                                    }
                                    if (!has)
                                    {
                                        cameraRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Right);
                                    }
                                    //去重
                                    cameraRotateByInput._yInputAxis._mouseButtons = cameraRotateByInput._yInputAxis._mouseButtons.Distinct().ToList();
                                });
                            }
                        }
                        #endregion

                        #region 左右键角色旋转控制模式
                        if (GUILayout.Button(TrLabel(nameof(leftRightCharacter))))
                        {
                            //左右键角色
                            var characterRotateByInput = character.GetComponentInChildren<CharacterRotateByInput>();
                            if (characterRotateByInput)
                            {
                                characterRotateByInput.XModifyProperty(() =>
                                {
                                    characterRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Left);
                                    characterRotateByInput._yInputAxis._mouseButtons.Add(EMouseButton.Right);
                                    //去重
                                    characterRotateByInput._yInputAxis._mouseButtons = characterRotateByInput._yInputAxis._mouseButtons.Distinct().ToList();
                                });
                            }

                            //左右键相机取消
                            var cameraRotateByInput = this.targetObject.characterCameraController.cameraMainController.cameraTransformer.GetComponent<CameraRotateByInput>();
                            if (cameraRotateByInput)
                            {
                                cameraRotateByInput.XModifyProperty(() =>
                                {
                                    cameraRotateByInput._yInputAxis._mouseButtons.RemoveAll(mb => mb == EMouseButton.Left || mb == EMouseButton.Right);
                                });
                            }
                        }
                        #endregion

                        #endregion

                        break;
                    }
            }
        }
    }
}
