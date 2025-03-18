using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Base.Recorders;
using XCSJ.LitJson;

namespace XCSJ.PluginPhysicses.Base.Recorders
{
    /// <summary>
    /// 刚体记录器
    /// </summary>
    public class RigidbodyRecorder : Recorder<Rigidbody, RigidbodyRecorder.Info>
    {
        /// <summary>
        /// 记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.GetComponent<Rigidbody>());
        }

        /// <summary>
        /// 批量记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 排除速度
        /// </summary>
        public void RecoverExcludeVelocity()
        {
            foreach (var i in _records)
            {
                try
                {
                    i.Recover(false);
                }
                catch { }
            }
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        public class Info : ISingleRecord<Rigidbody>
        {
            /// <summary>
            /// 刚体
            /// </summary>
            [Json(exportString = true)]
            public Rigidbody rigidbody;

            /// <summary>
            /// 组件
            /// </summary>
            [Json(exportString = true)]
            public Component component { get => rigidbody; set => rigidbody = value as Rigidbody; }

            [Json(exportString = true)]
            public Vector3 velocity;

            [Json(exportString = true)]
            public Vector3 angularVelocity;

            [Json(exportString = true)]
            public bool useGravity;

            [Json(exportString = true)]
            public bool isKinematic;

            [Json(exportString = true)]
            public float drag;

            [Json(exportString = true)]
            public float angularDrag;

            [Json(exportString = true)]
            public RigidbodyConstraints rigidbodyConstraints;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rigidbody"></param>
            public void Record(Rigidbody rigidbody)
            {
                this.rigidbody = rigidbody;

                velocity = rigidbody.velocity;
                angularVelocity = rigidbody.angularVelocity;
                useGravity = rigidbody.useGravity;
                isKinematic = rigidbody.isKinematic;
                drag = rigidbody.drag;
                angularDrag = rigidbody.angularDrag;
                rigidbodyConstraints = rigidbody.constraints;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover() => Recover(false);

            internal void Recover(bool recoverVelocity)
            {
                if (recoverVelocity)
                {
                    rigidbody.velocity = velocity;
                    rigidbody.angularVelocity = angularVelocity;
                }
                rigidbody.useGravity = useGravity;
                rigidbody.isKinematic = isKinematic;
                rigidbody.drag = drag;
                rigidbody.angularDrag = angularDrag;
                rigidbody.constraints = rigidbodyConstraints;
            }

            /// <summary>
            /// 设置IK属性
            /// </summary>
            /// <param name="isKinematic"></param>
            public void SetIsKinematic(bool isKinematic)
            {
                rigidbody.isKinematic = isKinematic;
            }
        }
    }
}
