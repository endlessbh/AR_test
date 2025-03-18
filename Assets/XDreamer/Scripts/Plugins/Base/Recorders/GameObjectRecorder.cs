using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 游戏对象记录器
    /// </summary>
    public class GameObjectRecorder : Recorder<GameObject, GameObjectRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<GameObject>
        {
            /// <summary>
            /// 游戏对象
            /// </summary>
            public GameObject gameObject;

            /// <summary>
            /// 激活记录
            /// </summary>
            private bool _active;

            private int _layer;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="gameObject"></param>
            public void Record(GameObject gameObject)
            {
                if (!gameObject) return;

                this.gameObject = gameObject;
                _active = gameObject.activeSelf;
                _layer = gameObject.layer;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (gameObject)
                {
                    gameObject.SetActive(_active);
                    gameObject.layer = _layer;
                }
            }

            /// <summary>
            /// 设置激活
            /// </summary>
            /// <param name="active"></param>
            public void SetActive(bool active)
            {
                if (gameObject) gameObject.SetActive(active);
            }

            /// <summary>
            /// 激活
            /// </summary>
            /// <param name="active"></param>
            public void SetActive(EBool active)
            {
                if (gameObject) gameObject.SetActive(CommonFun.BoolChange(gameObject.activeInHierarchy, active));
            }

            /// <summary>
            /// 设置层
            /// </summary>
            /// <param name="layer"></param>
            public void SetLayer(int layer)
            {
                if (gameObject)
                {
                    gameObject.layer = layer;
                }
            }
        }
    }
}
