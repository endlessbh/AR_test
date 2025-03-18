using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;

namespace XCSJ.Extension.Base.Maths
{
	/// <summary>
	/// 旋转矩阵类型:使用A、B、R分别代表第1，第2，第3元素旋转顺序及其对应元素的旋转角度；
	/// </summary>
	[Name("旋转矩阵类型")]
	public enum ERotateMatrixType
	{
		/// <summary>
		/// 未知
		/// </summary>
		[Name("未知")]
		Unknow = -1,

		#region Proper Euler angles

		XZX,

		XYX,

		YXY,

		YZY,

		ZYZ,

		ZXZ,

		#endregion

		#region Tait-Bryan angles

		XZY,

		XYZ,

		/// <summary>
		/// Unity外旋顺序-世界旋转矩阵顺序;Unity中类型<see cref="Matrix4x4"/>使用本顺序构建旋转组件；
		/// </summary>
		YXZ,

		YZX,

		ZYX,

		/// <summary>
		/// Unity内旋顺序-本地旋转矩阵顺序；
		/// </summary>
		ZXY,

		#endregion
	}

	/// <summary>
	/// 旋转矩阵组手
	/// </summary>
	public static class RotateMatrixHelper
	{
		private static Dictionary<ECoordinateSystem, bool> testCache = new Dictionary<ECoordinateSystem, bool>();

		/// <summary>
		/// 有有效将源坐标系转与Unity坐标系的相互转换的转换矩阵
		/// </summary>
		/// <param name="srcCoordinateSystem"></param>
		/// <returns></returns>
		public static bool HasValidConvertMatrixWithUnity(this ECoordinateSystem srcCoordinateSystem)
		{
			if (testCache.TryGetValue(srcCoordinateSystem, out var valid)) return valid;

			valid = false;
			var cs = new CoordinateSystem(srcCoordinateSystem);
			if (cs.isValid)
			{
				var attributes = AttributeCache<CoordinateSystemConvertTestAttribute>.GetsOfField(srcCoordinateSystem);
				if (attributes != null && attributes.Length > 0)
				{
					valid = attributes.All(a =>
					{
						var up = new Vector3(a.ux, a.uy, a.uz);
						var p = new Vector3(a.x, a.y, a.z);
						return cs.ConvertPositionToUnity(p) == up && cs.ConvertPositionFromUnity(up) == p;
					});
				}
			}

			testCache[srcCoordinateSystem] = valid;
			return valid;
		}

		private static Dictionary<ECoordinateSystem, Matrix4x4> cache = new Dictionary<ECoordinateSystem, Matrix4x4>();

		/// <summary>
		/// 获取将源坐标系转为Unity坐标系的转换矩阵
		/// </summary>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 GetConvertMatrixToUnity(this ECoordinateSystem srcCoordinateSystem)
		{
			if (cache.TryGetValue(srcCoordinateSystem, out var mat)) return mat;

			var attribute = AttributeCache<CoordinateSystemConvertAttribute>.GetOfField(srcCoordinateSystem);
			if (attribute == null)
			{
                if (srcCoordinateSystem.IsValidCoordinateSystem())
                {
					throw new ArgumentException(string.Format("有效坐标系: {0}, 值: {1} 未定义有效的转换矩阵!", srcCoordinateSystem.ToCoordinateSystemString(), srcCoordinateSystem.ToString()), nameof(srcCoordinateSystem));
				}
                else
				{
					throw new ArgumentException(string.Format("无效坐标系: {0}, 值: {1}", srcCoordinateSystem.ToCoordinateSystemString(), srcCoordinateSystem.ToString()), nameof(srcCoordinateSystem));
				}
			}

            if (attribute.needScale)
            {
                if (attribute.needRotate)
				{
					var scale = Matrix4x4.Scale(new Vector3(attribute.xScale, attribute.yScale, attribute.zScale));
					var rotate = Matrix4x4.Rotate(Quaternion.Euler(attribute.xRotate, attribute.yRotate, attribute.zRotate));
					mat = attribute.scaleLeft ? (scale * rotate) : (rotate * scale);
				}
                else
                {
					mat = Matrix4x4.Scale(new Vector3(attribute.xScale, attribute.yScale, attribute.zScale));
				}
            }
            else
            {
				if (attribute.needRotate)
				{
					mat = Matrix4x4.Rotate(Quaternion.Euler(attribute.xRotate, attribute.yRotate, attribute.zRotate));
				}
				else
				{
					mat = Matrix4x4.identity;
				}
			}

			cache[srcCoordinateSystem] = mat;
			return mat;
		}

		/// <summary>
		/// 将源坐标系的旋转矩阵转为默认坐标系的旋转矩阵
		/// </summary>
		/// <param name="srcRotateMatrix">源坐标系的旋转矩阵</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 ToUnityCoordinateSystem(this Matrix3x3 srcRotateMatrix, ECoordinateSystem srcCoordinateSystem) => ConvertRotateMatrixToUnity(srcRotateMatrix.ToMatrix4x4(), srcCoordinateSystem);

		/// <summary>
		/// 将指定坐标系的旋转矩阵转为默认坐标系的旋转矩阵
		/// </summary>
		/// <param name="srcRotateMatrix">源坐标系的旋转矩阵</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 ConvertRotateMatrixToUnity(this Matrix4x4 srcRotateMatrix, ECoordinateSystem srcCoordinateSystem)
		{
			switch (srcCoordinateSystem)
			{
				case ECoordinateSystem.XR_YU_ZF: return Matrix4x4.identity;
				default:
                    {
						var r = srcCoordinateSystem.GetConvertMatrixToUnity();
						return r * srcRotateMatrix * r.inverse;
					}
			}
		}

		/// <summary>
		/// 获取将源坐标系的源位置点转为默认坐标系的位置点
		/// </summary>
		/// <param name="srcPosition">源位置点</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Vector3 ConvertPositionToUnity(this Vector3 srcPosition, ECoordinateSystem srcCoordinateSystem) => srcCoordinateSystem.GetConvertMatrixToUnity().MultiplyPoint3x4(srcPosition);
	}
}
