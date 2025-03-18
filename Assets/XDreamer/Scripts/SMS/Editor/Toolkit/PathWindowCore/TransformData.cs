using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    [Name("变换数据")]
    public class TransformData
    {
        public virtual Vector3 keyPoint
        {
            get
            {
                return pathInfo.offsetValue + position;
            }
            set
            {
                position = value - pathInfo.offsetValue;
            }
        }

        public PathInfo pathInfo = null;

        public Vector3 position = Vector3.zero;

        public Quaternion rotation = Quaternion.Euler(0, 0, 0);

        public Vector3 scale = Vector3.one;
        
        public Vector3 recordPosition = Vector3.zero;

        public static TransformData Clone(TransformData transformData)
        {
            var newObj = new TransformData(transformData.pathInfo);
            newObj.pathInfo = transformData.pathInfo;
            newObj.position = transformData.position;
            newObj.rotation = transformData.rotation;
            newObj.scale = transformData.scale;
            return newObj;
        }

        public TransformData(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            if (pathInfo.firstTransform!=null)
            {
                this.recordPosition = pathInfo.firstTransform.position;
                this.rotation = pathInfo.firstTransform.localRotation;
                this.scale = pathInfo.firstTransform.localScale;
            }
        }

        public TransformData(PathInfo pathInfo, Vector3 offset) : this(pathInfo)
        {
            this.position = offset;
        }

        public void Recover()
        {
            if (pathInfo.firstTransform != null)
            {
                pathInfo.firstTransform.position = recordPosition;
                pathInfo.firstTransform.localRotation = rotation;
                pathInfo.firstTransform.localScale = scale;
            }
        }
    }
}
