using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards.Managers
{
    /// <summary>
    /// 网络相机管理器
    /// </summary>
    public static class WebCamManager
    {
        /// <summary>
        /// 设备列表
        /// </summary>
        public static readonly Dictionary<string, WebCamTexture> devices = new Dictionary<string, WebCamTexture>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Init()
        {
        }

        /// <summary>
        /// 释放，把所有摄像机要释放掉！！
        /// </summary>
        public static void Release()
        {
            foreach (var kv in devices)
            {
                //Debug.Log("Release: " + kv.Key);
                kv.Value.Stop();
                //UnityEngine.Object.DestroyImmediate(kv.Value);
                kv.Value.XDestoryObject();
            }
            devices.Clear();
        }

        /// <summary>
        /// 创建或获取网络相机纹理
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static WebCamTexture CreateOrGetWebCamTexture(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return null;
            WebCamTexture webCamTexture;
            if (devices.TryGetValue(camName, out webCamTexture) && webCamTexture != null)
            {
                //
            }
            else
            {
                webCamTexture = new WebCamTexture(camName);
                if (webCamTexture)
                {
                    webCamTexture.name = camName;
                    devices[camName] = webCamTexture;
                }
            }
            return webCamTexture;
        }

        /// <summary>
        /// 获取网络相机纹理
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static WebCamTexture GetWebCamTexture(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return null;
            WebCamTexture webCamTexture;
            devices.TryGetValue(camName, out webCamTexture);
            return webCamTexture;
        }

        /// <summary>
        /// 播放所有
        /// </summary>
        public static void Play()
        {
            foreach (var kv in devices)
            {
                kv.Value.Play();
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static bool Play(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return false;
            WebCamTexture webCamTexture;
            if (devices.TryGetValue(camName, out webCamTexture) && webCamTexture != null)
            {
                webCamTexture.Play();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 停止所有
        /// </summary>
        public static void Stop()
        {
            foreach (var kv in devices)
            {
                kv.Value.Stop();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static bool Stop(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return false;
            WebCamTexture webCamTexture;
            if (devices.TryGetValue(camName, out webCamTexture) && webCamTexture != null)
            {
                webCamTexture.Stop();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 暂停所有
        /// </summary>
        public static void Pause()
        {
            foreach (var kv in devices)
            {
                kv.Value.Pause();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static bool Pause(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return false;
            WebCamTexture webCamTexture;
            if (devices.TryGetValue(camName, out webCamTexture) && webCamTexture != null)
            {
                webCamTexture.Pause();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="camName"></param>
        /// <returns></returns>
        public static bool Release(string camName)
        {
            if (string.IsNullOrEmpty(camName)) return false;
            WebCamTexture webCamTexture;
            if (devices.TryGetValue(camName, out webCamTexture))
            {
                //先确保相机停止工作了！
                if (webCamTexture != null) webCamTexture.Stop();

                //Debug.Log("Release x: " + camName);
                //销毁对象
                //UnityEngine.Object.DestroyImmediate(webCamTexture);
                webCamTexture.XDestoryObject();
                //再删除
                devices.Remove(camName);
            }
            return false;
        }
    }
}
