using System.Collections.Generic;
using UnityEngine;
using XCSJ.LitJson;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 变换记录器:用于记录变换的PRS信息；
    /// </summary>
    public class TransformRecorder : Recorder<Transform, TransformRecorder.Info>, ITransformRecorder
    {
        /// <summary>
        /// 变换记录列表
        /// </summary>
        [Json(false)]
        public ITransformRecord[] transformRecords => _records.ToArray();

        /// <summary>
        /// 记录游戏对象的变换
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.transform);
        }

        /// <summary>
        /// 批量记录游戏对象的变换
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
        /// 恢复
        /// </summary>
        /// <param name="space"></param>
        public virtual void Recover(Space space)
        {
            foreach (var i in _records)
            {
                try
                {
                    i.Recover(space);
                }
                catch { }
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ITransformRecord
        {
            /// <summary>
            /// 变换
            /// </summary>
            [Json(false)]
            public Transform transform { get; set; }

            /// <summary>
            /// 组件
            /// </summary>
            [Json(exportString = true)]
            public Component component { get => transform; set => transform = value as Transform; }

            /// <summary>
            /// 本地位置
            /// </summary>
            [Json(exportString = true)]
            public Vector3 localPosition { get; set; }

            /// <summary>
            /// 本地旋转
            /// </summary>
            [Json(exportString = true)]
            public Quaternion localRotation { get; set; }

            /// <summary>
            /// 本地缩放
            /// </summary>
            [Json(exportString = true)]
            public Vector3 localScale { get; set; }

            /// <summary>
            /// 世界位置
            /// </summary>
            [Json(exportString = true)]
            public Vector3 worldPosition { get; set; }

            /// <summary>
            /// 世界旋转
            /// </summary>
            [Json(exportString = true)]
            public Quaternion worldRotation { get; set; }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public virtual void Record(Transform transform)
            {
                this.transform = transform;

                localPosition = transform.localPosition;
                localRotation = transform.localRotation;
                localScale = transform.localScale;

                worldPosition = transform.position;
                worldRotation = transform.rotation;
            }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="arg1"></param>
            public void Record(GameObject arg1) => Record(arg1.transform);

            /// <summary>
            /// 恢复：基于本地信息执行恢复
            /// </summary>
            public virtual void Recover()
            {
                transform.localPosition = localPosition;
                transform.localRotation = localRotation;
                transform.localScale = localScale;

                //transform.position = worldPosition;
                //transform.rotation = worldRotation;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            /// <param name="space"></param>
            public void Recover(Space space) => RecoverTo(transform, space);

            /// <summary>
            /// 恢复到
            /// </summary>
            /// <param name="targetTransform"></param>
            /// <param name="space">本地或时间</param>
            public void RecoverTo(Transform targetTransform, Space space)
            {
                if (!targetTransform) return;
                switch(space)
                {
                    case Space.Self:
                        {
                            targetTransform.localPosition = localPosition;
                            targetTransform.localRotation = localRotation;
                            targetTransform.localScale = localScale;
                            break;
                        }
                    case Space.World:
                        {
                            targetTransform.position = worldPosition;
                            targetTransform.rotation = worldRotation;
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// 层级变换记录器:用于记录变换的父级与PRS信息；
    /// </summary>
    public class HierarchyTransformRecorder : Recorder<Transform, HierarchyTransformRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : TransformRecorder.Info
        {
            /// <summary>
            /// 父级
            /// </summary>
            [Json(exportString = true)]
            public Transform parent { get; set; }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public override void Record(Transform transform)
            {
                parent = transform.parent;
                base.Record(transform);
            }

            /// <summary>
            /// 恢复：基于本地信息执行恢复
            /// </summary>
            public override void Recover()
            {
                transform.parent = parent;
                base.Recover();
            }
        }
    }

    /// <summary>
    /// 变换记录接口
    /// </summary>
    public interface ITransformRecord : ISingleRecord<Transform>, ISingleRecord<GameObject>
    {
        Transform transform { get; set; }

        Vector3 localPosition { get; set; }

        Quaternion localRotation { get; set; }

        Vector3 localScale { get; set; }

        Vector3 worldPosition { get; set; }

        Quaternion worldRotation { get; set; }
    }

    /// <summary>
    /// 变换记录器接口
    /// </summary>
    public interface ITransformRecorder : IBatchRecorder<Transform>, IBatchRecorder<GameObject>
    {
        /// <summary>
        /// 记录列表
        /// </summary>
        ITransformRecord[] transformRecords { get; }
    }
}
