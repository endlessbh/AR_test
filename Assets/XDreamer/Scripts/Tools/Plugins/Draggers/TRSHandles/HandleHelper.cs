using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XCSJ.PluginsCameras;

namespace XCSJ.PluginTools.Draggers.TRSHandles
{
    /// <summary>
    /// 辅助平移旋转缩放工具静态函数
    /// </summary>
    public static class HandleHelper
    {
        private static Vector3 Mask(Vector3 vec)
        {
            return new Vector3(vec.x > 0f ? 1f : -1f, vec.y > 0f ? 1f : -1f, vec.z > 0f ? 1f : -1f);
        }

        private static float Mask(float val)
        {
            return val > 0f ? 1f : -1f;
        }

        public static Vector3 DirectionMask(Transform target, Vector3 rayDirection)
        {
            Vector3 viewDir = -Mask(new Vector3(Vector3.Dot(rayDirection, target.right), Vector3.Dot(rayDirection, target.up), Vector3.Dot(rayDirection, target.forward)));
            return viewDir;
        }

        /**
		 * When dragging in 3d space, this returns the signed delta based on handle orientation.
		 */
        public static float CalcMouseDeltaSignWithAxes(Camera cam, Vector3 origin, Vector3 upDir, Vector3 rightDir, Vector2 mouseDelta)
        {
            if (Mathf.Abs(mouseDelta.magnitude) < .0001f) return 1f;

            Vector2 or = cam.WorldToScreenPoint(origin);
            Vector2 ud = cam.WorldToScreenPoint(origin + upDir);
            Vector2 rd = cam.WorldToScreenPoint(origin + rightDir);

            float mouseDotUp = Vector2.Dot(mouseDelta, ud - or);
            float mouseDotRight = Vector2.Dot(mouseDelta, rd - or);

            if (Mathf.Abs(mouseDotUp) > Mathf.Abs(mouseDotRight)) return Mathf.Sign(mouseDotUp);
            else return Mathf.Sign(mouseDotRight);
        }

        /**
		 * Calculates a signed float delta from a current and previous mouse position.
		 * @param lhs Current mouse position.
		 * @param rhs Previous mouse position.
		 */
        public static float CalcSignedMouseDelta(Vector2 lhs, Vector2 rhs)
        {
            float delta = Vector2.Distance(lhs, rhs);
            float scale = 1f / Mathf.Min(Screen.width, Screen.height);

            // If horizontal movement is greater than vertical movement, use the X axis for sign.
            if (Mathf.Abs(lhs.x - rhs.x) > Mathf.Abs(lhs.y - rhs.y)) return delta * scale * ((lhs.x - rhs.x) > 0f ? 1f : -1f);
            else return delta * scale * ((lhs.y - rhs.y) > 0f ? 1f : -1f);
        }

        /// <summary>
        /// 返回屏幕与世界空间的比例
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static float GetScreenAndWorldRatio(Vector3 worldPosition)
        {
            Camera cam = CameraHelperExtension.currentCamera;
            if (!cam) return 1f;
            Transform t = cam.transform;
            float z = Vector3.Dot(worldPosition - t.position, cam.transform.forward);
            Vector3 lhs = cam.WorldToScreenPoint(t.position + (t.forward * z));
            Vector3 rhs = cam.WorldToScreenPoint(t.position + (t.right + t.forward * z));
            return 1f / (lhs - rhs).magnitude;
        }

        /**
       * Snap a value to the nearest increment.
       */
        public static float Snap(float value, float increment)
        {
            return Mathf.Round(value / increment) * increment;
        }

        public static Vector2 Snap(Vector2 value, float increment)
        {
            return new Vector2(Snap(value.x, increment), Snap(value.y, increment));
        }

        public static Vector3 Snap(Vector3 value, float increment)
        {
            return new Vector3(Snap(value.x, increment), Snap(value.y, increment), Snap(value.z, increment));
        }
    }

}
