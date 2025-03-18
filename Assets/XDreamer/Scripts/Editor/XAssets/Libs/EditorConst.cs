using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public static class EditorConst
    {
        public static string[] TextureExts = { ".tga", ".png", ".jpg", ".tif", ".psd", ".exr" };
        public static string[] AudioExts = { ".mp3", ".wma", ".rm", ".wav", ".midi", ".ape", ".flac" };
        public static string[] MaterialExts = { ".mat" };
        public static string[] ModelExts = { ".fbx", ".asset", ".obj" };
        public static string[] AnimationExts = { ".anim" };
        public static string[] MetaExts = { ".meta" };
        public static string[] ShaderExts = { ".shader" };
        public static string[] ScriptExts = { ".cs" };
        public static string[] PrefabExts = { ".prefab" };
        public static string[] PhysicMaterialExts = { ".physicMaterial" };
        public static string[] PhysicMaterial2DExts = { ".physicMaterial2D" };
        public static string[] AssetExts = { ".asset" };
        public static string[] AudioMixerExts = { ".mixer" };
        public static string[] FlareExts = { ".flare" };
        public static string[] RenderTextureExts = { ".renderTexture" };
        public static string[] LightmapParametersExts = { ".giparams" };
        public static string[] SpriteAtlasExts = { ".spriteatlas" };
        public static string[] AnimatorControllerExts = { ".controller" }; 
        public static string[] AnimatorOverrideControllerExts = { ".overrideController" };
        public static string[] AvatarMaskExts = { ".mask" };
        public static string[] TimelineExts = { ".playable" };
        public static string[] GUISkinExts = { ".guiskin" };
        public static string[] FontExts = { ".fontsettings", ".TTF" };
        public static string[] CubemapExts = { ".cubemap" };
        public static string[] BrushExts = { ".brush" };
        public static string[] TerrainLayerExts = { ".terrainlayer" };
        public static string[] UnityPackageExts = { ".unitypackage" };
        public static string[] SceneExts = { ".unity" };

        public static string PlatformAndroid = "Android";
        public static string PlatformIos = "iPhone";
        public static string PlatformStandalones = "Standalones";

        public static string EDITOR_ANICLIP_NAME = "__preview__Take 001";
        public static string[] EDITOR_CONTROL_NAMES = {"AnimatorStateMachine",
            "AnimatorStateTransition", "AnimatorState", "AnimatorTransition", "BlendTree" };
    }
}
