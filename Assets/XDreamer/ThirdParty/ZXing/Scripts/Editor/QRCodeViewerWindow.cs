using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginZXing;
using XCSJ.Tools;

namespace XCSJ.EditorZXing
{
    [Name(Title)]
    [Tip("二维码生成与解码编辑窗口", "QR code generation and decoding editing window")]
    [XCSJ.Attributes.Icon(EIcon.QRCode)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class QRCodeViewerWindow : XEditorWindow<QRCodeViewerWindow>
    {
        public const string Title = "二维码查看器";

        public static readonly Macro XDREAMER_ZXING = new Macro(nameof(XDREAMER_ZXING), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            if (TypeHelper.ExistsAndAssemblyFileExists("ZXing.BarcodeWriter"))
            {
                XDREAMER_ZXING.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_ZXING.UndefineWithSelectedBuildTargetGroup();
            }
        }

        /// <summary>
        /// 静态主入口函数
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        public int toolbarIndex = 0;

        public Vector2 contextArea = new Vector2();

        #region EncodeViewer

        public string encodeString = "";

        public string encodeTexturePath = "";

        public Texture2D encodeTexture = null;

        public int encodeTextureWidth = 256;

        public int encodeTextureHeight = 256;

        #endregion

        #region DecodeViewer

        public string decodeString = "";

        public string decodeTexturePath = "";

        public Texture2D decodeTexture = null;

        #endregion

        #region VideoDecodeViewer

        public WebCamTexture webCamTexture = null;

        public string webCamDecodeString = "";

        public int count = 0;

        #endregion

        protected override void OnDisable()
        {
            //EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;
            //Debug.Log("OnDisable");
            if (decodeTexture)
            {
                DestroyImmediate(decodeTexture);
                decodeTexture = null;
            }
            if (encodeTexture)
            {
                DestroyImmediate(encodeTexture);
                encodeTexture = null;
            }
            ReleaseVideoWhenNotInPlay();
        }

        protected void OnFocus()
        {
            if (webCamTexture)
            {
                webCamTexture.Play();
            }
        }

        protected void OnLostFocus()
        {
            //Debug.Log("OnLostFocus ReleaseVideo");
            //ReleaseVideoWhenNotInPlay();
            if (webCamTexture)
            {
                webCamTexture.Stop();
            }
        }

        protected override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(25));
            if (GUILayout.Toggle(toolbarIndex == 0, "扫描图片二维码", EditorStyles.miniButtonLeft, GUILayout.ExpandHeight(true)))
            {
                toolbarIndex = 0;
                ReleaseVideoWhenNotInPlay();
            }
            if (GUILayout.Toggle(toolbarIndex == 1, "生成二维码", EditorStyles.miniButtonMid, GUILayout.ExpandHeight(true)))
            {
                toolbarIndex = 1;
                ReleaseVideoWhenNotInPlay();

            }
            if (GUILayout.Toggle(toolbarIndex == 2, "扫描二维码", EditorStyles.miniButtonMid, GUILayout.ExpandHeight(true)))
            {
                toolbarIndex = 2;
            }
            if (GUILayout.Toggle(toolbarIndex == 3, "关于", EditorStyles.miniButtonRight, GUILayout.ExpandHeight(true)))
            {
                toolbarIndex = 3;
                ReleaseVideoWhenNotInPlay();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical("box");
            contextArea = EditorGUILayout.BeginScrollView(contextArea, false, false);
            switch (toolbarIndex)
            {
                case 1:
                    {
                        EncodeViewer();
                        break;
                    }
                case 2:
                    {
                        VideoDecodeViewer();
                        break;
                    }
                case 3:
                    {
                        AboutWindow();
                        break;
                    }
                default:
                    {
                        DecodeViewer();
                        break;
                    }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        public void EncodeViewer()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel("编码信息:", GUILayout.Width(60));
            string es = EditorGUILayout.TextArea(encodeString);
            if (es != encodeString)
            {
                encodeString = es;
                EncodeToTexture();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel("图片信息:", GUILayout.Width(60));
            EditorGUILayout.SelectableLabel(string.Format(" 宽: {0}, 高: {1}", encodeTextureWidth, encodeTextureHeight));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel("路径信息:", GUILayout.Width(60));
            EditorGUILayout.SelectableLabel(encodeTexturePath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel("编码信息:", GUILayout.Width(60));
            EditorGUILayout.SelectableLabel(QRCode.Decode(encodeTexture));
            EditorGUILayout.EndHorizontal();

            //             int w = EditorGUILayout.IntSlider(encodeTextureWidth, 256, 1024);
            //             if (w != encodeTextureWidth)
            //             {
            //                 encodeTextureWidth = w;
            //                 EncodeToTexture();
            //             }
            //             int h = EditorGUILayout.IntSlider(encodeTextureHeight, 128, 1024);
            //             if (h != encodeTextureHeight)
            //             {
            //                 encodeTextureHeight = h;
            //                 EncodeToTexture();
            //             }

            if (GUILayout.Button("保存图片") && encodeTexture)
            {
                if (string.IsNullOrEmpty(encodeTexturePath)) encodeTexturePath = Application.dataPath;
                encodeTexturePath = EditorUtility.SaveFilePanel("选择图片", System.IO.Path.GetDirectoryName(encodeTexturePath), "qrcode", "jpg");
                if (!string.IsNullOrEmpty(encodeTexturePath) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(encodeTexturePath)))
                {
                    //encodeTexturePath = encodeTexturePath.Replace("/", "\\");
                    Debug.Log("保存图片: " + encodeTexturePath);
                    System.IO.File.WriteAllBytes(encodeTexturePath, encodeTexture.EncodeToJPG());
                }
            }
            //EditorGUILayout.Separator();
            if (encodeTexture)
            {
                GUILayout.Box(encodeTexture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
            else
            {
                EncodeToTexture();
                GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
        }

        public void EncodeToTexture()
        {
            if (encodeTexture) UnityEngine.Object.DestroyImmediate(encodeTexture);
            //Debug.Log("EncodeToTexture: " + encodeTextureWidth + ", " + encodeTextureHeight);
            try
            {
                encodeTexture = QRCode.EncodeToTexture2D(encodeString, encodeTextureWidth, encodeTextureHeight);
            }
            catch// (Exception ex)
            {
                encodeTexture = null;
            }
        }

        public void DecodeViewer()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel("扫描结果:", GUILayout.Width(60));
            EditorGUILayout.SelectableLabel(decodeString);
            if (GUILayout.Button("重新扫描", GUILayout.Width(70)) && decodeTexture)
            {
                QRCode.Decode(decodeTexture, out decodeString);
            }
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.Separator();
            if (GUILayout.Button("加载图片"))
            {
                if (string.IsNullOrEmpty(decodeTexturePath)) decodeTexturePath = Application.dataPath;
                decodeTexturePath = EditorUtility.OpenFilePanelWithFilters("选择图片", System.IO.Path.GetDirectoryName(decodeTexturePath), UICommonOption.pictureFileFilters);
                if (System.IO.File.Exists(decodeTexturePath))
                {
                    decodeTexturePath = decodeTexturePath.Replace("/", "\\");
                    //Debug.Log("加载图片: " + decodeTexturePath);
                    if (decodeTexture) UnityEngine.Object.DestroyImmediate(decodeTexture);
                    decodeTexture = CommonFun.LoadTextureFromLocalDisk(decodeTexturePath);
                    if (decodeTexture) QRCode.Decode(decodeTexture, out decodeString);
                }
            }
            //EditorGUILayout.Separator();
            if (decodeTexture)
            {
                GUILayout.Box(decodeTexture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
            else
            {
                GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
        }

        public void AboutWindow()
        {
            UICommonFun.NotificationLayout("使用ZXing解析与生成二维码");
        }

        public void VideoDecodeViewer()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("运行态不可使用！");
                ReleaseVideo();
                return;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("扫描结果:", GUILayout.Width(70));
            EditorGUILayout.SelectableLabel(webCamDecodeString);
            if (GUILayout.Button("重新扫描", GUILayout.Width(70)))
            {
                webCamDecodeString = "";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            if (webCamTexture)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("扫描状态:", GUILayout.Width(70));
                if (!webCamTexture.isPlaying)
                {
                    EditorGUILayout.SelectableLabel("摄像头未启动...");
                }
                else if (string.IsNullOrEmpty(webCamDecodeString))
                {
                    EditorGUILayout.SelectableLabel("扫描中...");
                    if (QRCode.Decode(webCamTexture, out webCamDecodeString))
                    {
                        Debug.Log("二维码查看器 扫描结果: " + webCamDecodeString);
                    }
                }
                else
                {
                    EditorGUILayout.SelectableLabel("扫描完成！");
                }
                //
                EditorGUILayout.EndHorizontal();
                GUILayout.Box(webCamTexture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
            else if (UICommonFun.NotificationButtonLayout("启动摄像头"))
            {
                //                 if (webCamTexture)
                //                 {
                //                     Debug.Log("webCamTexture： " + webCamTexture.didUpdateThisFrame);
                //                 }
                //Debug.Log("启动摄像头");
                ReleaseVideo();
                CreateAndPlayVideo();
            }
        }

        public void CreateAndPlayVideo()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
            else
            {
                if (WebCamTexture.devices != null && WebCamTexture.devices.Length > 0)
                {
                    foreach (var tex in UnityEngine.Object.FindObjectsOfType<WebCamTexture>())
                    {
                        Debug.Log(tex.deviceName);
                        if (tex.deviceName == WebCamTexture.devices[0].name)
                        {
                            //Debug.Log("find webCamTexture： " + webCamTexture.deviceName + " , " + WebCamTexture.devices[0].name);
                            webCamTexture = tex;
                            webCamTexture.Play();
                            WebCamManager.devices[tex.deviceName] = tex;
                            return;
                        }
                    }
                    webCamTexture = WebCamManager.CreateOrGetWebCamTexture(WebCamTexture.devices[0].name);
                    if (webCamTexture)
                    {
                        //Debug.Log("webCamTexture： " + webCamTexture.deviceName + " , " + WebCamTexture.devices[0].name);
                        webCamTexture.requestedHeight = 320;
                        webCamTexture.requestedWidth = 640;
                        webCamTexture.requestedFPS = 24;
                        webCamTexture.Play();
                    }
                }
            }
        }

        public void ReleaseVideo()
        {
            webCamDecodeString = "";
            //Debug.Log("ReleaseVideo");
            if (webCamTexture)
            {
                //Debug.Log("ReleaseVideo -->" + webCamTexture.deviceName);
                WebCamManager.Release();
                webCamTexture = null;
            }
        }

        public void ReleaseVideoWhenNotInPlay()
        {
            //非运行态才执行销毁操作~~放置是运行态脚本创建的导致互相冲突！！！
            if (!Application.isPlaying)
            {
                ReleaseVideo();
            }
        }

        protected void Update()
        {
            if (webCamTexture && ++count > 2)
            {
                count = 0;
                Repaint();
            }
        }

    }
}
