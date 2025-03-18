using System;
using UnityEngine;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 3x3矩阵：使用列矩阵方式存储数据；
    /// 作为旋转矩阵时，其存储数据内容如下(x、y、z代表对应轴的分量；ABR对应<see cref="ERotateMatrixType"/>中的解释):
    /// A.x  B.x  R.x
    /// A.y  B.y  R.y
    /// A.z  B.z  R.z
    /// </summary>
    public struct Matrix3x3
	{
		/// <summary>
		/// A.x
		/// </summary>
		public float m00;

		/// <summary>
		/// B.x
		/// </summary>
		public float m01;

		/// <summary>
		/// R.x
		/// </summary>
		public float m02;

		/// <summary>
		/// A.y
		/// </summary>
		public float m10;

		/// <summary>
		/// B.y
		/// </summary>
		public float m11;

		/// <summary>
		/// R.y
		/// </summary>
		public float m12;

		/// <summary>
		/// A.z
		/// </summary>
		public float m20;

		/// <summary>
		/// B.z
		/// </summary>
		public float m21;

		/// <summary>
		/// C.z
		/// </summary>
		public float m22;

		/// <summary>
		/// 转置矩阵
		/// </summary>
		public Matrix3x3 transpose => Transpose(this);

		/// <summary>
		/// 先行后列；即3x3程序数组的索引方式；
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return this.m00;
					case 1: return this.m01;
					case 2: return this.m02;
					case 3: return this.m10;
					case 4: return this.m11;
					case 5: return this.m12;
					case 6: return this.m20;
					case 7: return this.m21;
					case 8: return this.m22;
					default: throw new IndexOutOfRangeException("无效索引!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						this.m00 = value;
						break;
					case 1:
						this.m01 = value;
						break;
					case 2:
						this.m02 = value;
						break;
					case 3:
						this.m10 = value;
						break;
					case 4:
						this.m11 = value;
						break;
					case 5:
						this.m12 = value;
						break;
					case 6:
						this.m20 = value;
						break;
					case 7:
						this.m21 = value;
						break;
					case 8:
						this.m22 = value;
						break;
					default:
						throw new IndexOutOfRangeException("无效索引!");
				}
			}
		}

		/// <summary>
		/// 先行后列；即3x3程序数组的索引方式；
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public float this[int row, int col]
		{
			get => this[row * 3 + col];
		}

		/// <summary>
		/// 转数组
		/// </summary>
		/// <param name="columnWise">优先列</param>
		/// <returns></returns>
		public float[] ToArray(bool columnWise = false)
		{
			if (columnWise)
			{
				return new float[] { m00, m10, m20, m01, m11, m21, m02, m12, m22 };
			}
			else
			{
				return new float[] { m00, m01, m02, m10, m11, m12, m20, m21, m22 };
			}
		}

		/// <summary>
		/// 零矩阵
		/// </summary>
		public static Matrix3x3 zero => _zero;

		/// <summary>
		/// 零矩阵
		/// </summary>
		public static Matrix3x3 _zero = new Matrix3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

		/// <summary>
		/// 单位矩阵
		/// </summary>
		public static Matrix3x3 identity => _identity;

		/// <summary>
		/// 单位矩阵
		/// </summary>
		public static Matrix3x3 _identity => new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

		/// <summary>
		/// 转置矩阵
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static Matrix3x3 Transpose(Matrix3x3 m)
		{
			var nm = new Matrix3x3()
			{
				m00 = m.m00,
				m10 = m.m01,
				m20 = m.m02,

				m01 = m.m10,
				m11 = m.m11,
				m21 = m.m12,

				m02 = m.m20,
				m12 = m.m21,
				m22 = m.m22
			};

			return nm;
		}

		/// <summary>
		/// 创建矩阵
		/// </summary>
		/// <param name="m"></param>
		/// <param name="columnWise">优先列</param>
		/// <returns></returns>
		public static Matrix3x3 Create(float[] m, bool columnWise = false)
		{
			if (m == null || m.Length < 9) throw new ArgumentException("参数无效", nameof(m));
			if (columnWise)
			{
				return new Matrix3x3() { m00 = m[0], m10 = m[1], m20 = m[2], m01 = m[3], m11 = m[4], m21 = m[5], m02 = m[6], m12 = m[7], m22 = m[8] };
			}
			else
			{
				return new Matrix3x3() { m00 = m[0], m01 = m[1], m02 = m[2], m10 = m[3], m11 = m[4], m12 = m[5], m20 = m[6], m21 = m[7], m22 = m[8] };
			}
		}

		/// <summary>
		/// 创建矩阵
		/// </summary>
		/// <param name="m"></param>
		/// <param name="columnWise">优先列</param>
		/// <returns></returns>
		public static Matrix3x3 Create(double[] m, bool columnWise = false)
        {
			if (m == null || m.Length < 9) throw new ArgumentException("参数无效", nameof(m));
			if (columnWise)
			{
				return new Matrix3x3() { m00 = (float)m[0], m10 = (float)m[1], m20 = (float)m[2], m01 = (float)m[3], m11 = (float)m[4], m21 = (float)m[5], m02 = (float)m[6], m12 = (float)m[7], m22 = (float)m[8] };
			}
			else
			{
				return new Matrix3x3() { m00 = (float)m[0], m01 = (float)m[1], m02 = (float)m[2], m10 = (float)m[3], m11 = (float)m[4], m12 = (float)m[5], m20 = (float)m[6], m21 = (float)m[7], m22 = (float)m[8] };
			}
		}

		/// <summary>
		/// 创建矩阵
		/// </summary>
		/// <param name="m"></param>
		/// <param name="columnWise">优先列</param>
		/// <returns></returns>
		public static Matrix3x3 Create(Matrix4x4 matrix4X4) => new Matrix3x3(matrix4X4.m00, matrix4X4.m10, matrix4X4.m20, matrix4X4.m01, matrix4X4.m11, matrix4X4.m21, matrix4X4.m02, matrix4X4.m12, matrix4X4.m22);

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="m00"></param>
		/// <param name="m10"></param>
		/// <param name="m20"></param>
		/// <param name="m01"></param>
		/// <param name="m11"></param>
		/// <param name="m21"></param>
		/// <param name="m02"></param>
		/// <param name="m12"></param>
		/// <param name="m22"></param>
		public Matrix3x3(float m00, float m10, float m20, float m01, float m11, float m21, float m02, float m12, float m22)
		{
			this.m00 = m00;
			this.m10 = m10;
			this.m20 = m20;

			this.m01 = m01;
			this.m11 = m11;
			this.m21 = m21;

			this.m02 = m02;
			this.m12 = m12;
			this.m22 = m22;
		}

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="column0"></param>
		/// <param name="column1"></param>
		/// <param name="column2"></param>
		public Matrix3x3(Vector3 column0, Vector3 column1, Vector3 column2)
		{
			m00 = column0.x;
			m10 = column0.y;
			m20 = column0.z;

			m01 = column1.x;
			m11 = column1.y;
			m21 = column1.z;

			m02 = column2.x;
			m12 = column2.y;
			m22 = column2.z;
		}

		/// <summary>
		/// 获取列
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Vector3 GetColumn(int index)
		{
			switch (index)
			{
				case 0:
					{
						return new Vector3(m00, m10, m20);
					}
				case 1:
					{
						return new Vector3(m01, m11, m21);
					}
				case 2:
					{
						return new Vector3(m02, m12, m22);
					}
				default: return Vector3.zero;
			}
		}

		/// <summary>
		/// 转为4x4矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4x4 ToMatrix4x4() => new Matrix4x4(GetColumn(0), GetColumn(1), GetColumn(2), new Vector4(0, 0, 0, 1));

		/// <summary>
		/// 隐式转换为<see cref="Matrix4x4"/>
		/// </summary>
		/// <param name="matrix3X3"></param>
		public static implicit operator Matrix4x4(Matrix3x3 matrix3X3) => matrix3X3.ToMatrix4x4();

		/// <summary>
		/// 从<see cref="Matrix4x4"/>隐式转换
		/// </summary>
		/// <param name="matrix4X4"></param>
		public static implicit operator Matrix3x3(Matrix4x4 matrix4X4) => Create(matrix4X4);

		#region 旋转矩阵

		/// <summary>
		/// 默认旋转矩阵类型
		/// </summary>
		public const ERotateMatrixType DefaultRotateMatrixType = ERotateMatrixType.YXZ;

		/// <summary>
		/// 构建旋转矩阵
		/// </summary>
		/// <param name="eulerAngles">欧拉角度</param>
		/// <param name="rotateMatrixType"></param>
		/// <param name="degreeOrRadians">欧拉角度的单位是度还是弧度</param>
		/// <returns></returns>
		public static Matrix3x3 Rotate(Vector3 eulerAngles, ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType, bool degreeOrRadians = true)
		{
			var (a, b, r) = ToABR(eulerAngles, rotateMatrixType);
			return Rotate(a, b, r, rotateMatrixType, degreeOrRadians);
		}

		/// <summary>
		/// 构建旋转矩阵
		/// </summary>
		/// <param name="a">对应第1元素旋转的角度</param>
		/// <param name="b">对应第2元素旋转的角度</param>
		/// <param name="r">对应第3元素旋转的角度</param>
		/// <param name="rotateMatrixType">旋转矩阵类型</param>
		/// <param name="degreeOrRadians">角度的单位是度还是弧度</param>
		/// <returns></returns>
		public static Matrix3x3 Rotate(float a, float b, float r, ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType, bool degreeOrRadians = true)
		{
			if (degreeOrRadians)
			{
				a = Mathf.Deg2Rad * a;
				b = Mathf.Deg2Rad * b;
				r = Mathf.Deg2Rad * r;
			}

			var c1 = Mathf.Cos(a);
			var c2 = Mathf.Cos(b);
			var c3 = Mathf.Cos(r);

			var s1 = Mathf.Sin(a);
			var s2 = Mathf.Sin(b);
			var s3 = Mathf.Sin(r);

			var rotateMatrix = new Matrix3x3();
			switch (rotateMatrixType)
			{
				case ERotateMatrixType.XZX:
					{
						rotateMatrix.m00 = c2; rotateMatrix.m01 = -c3 * s2; rotateMatrix.m02 = s2 * s3;
						rotateMatrix.m10 = c1 * s2; rotateMatrix.m11 = c1 * c2 * c3 - s1 * s3; rotateMatrix.m12 = -c3 * s1 - c1 * c2 * s3;
						rotateMatrix.m20 = s1 * s2; rotateMatrix.m21 = c1 * s3 + c2 * c3 * s1; rotateMatrix.m22 = c1 * c3 - c2 * s1 * s3;
						break;
					}
				case ERotateMatrixType.XYX:
					{
						rotateMatrix.m00 = c2; rotateMatrix.m01 = s2 * s3; rotateMatrix.m02 = c3 * s2;
						rotateMatrix.m10 = s1 * s2; rotateMatrix.m11 = c1 * c3 - c2 * s1 * s3; rotateMatrix.m12 = -c1 * s3 - c2 * c3 * s1;
						rotateMatrix.m20 = -c1 * s2; rotateMatrix.m21 = c3 * s1 + c1 * c2 * s3; rotateMatrix.m22 = c1 * c2 * c3 - s1 * s3;
						break;
					}
				case ERotateMatrixType.YXY:
					{
						rotateMatrix.m00 = c1 * c3 - c2 * s1 * s3; rotateMatrix.m01 = s1 * s2; rotateMatrix.m02 = c1 * s3 + c2 * c3 * s1;
						rotateMatrix.m10 = s2 * s3; rotateMatrix.m11 = c2; rotateMatrix.m12 = -c3 * s2;
						rotateMatrix.m20 = -c3 * s1 - c1 * c2 * s3; rotateMatrix.m21 = c1 * s2; rotateMatrix.m22 = c1 * c2 * c3 - s1 * s3;
						break;
					}
				case ERotateMatrixType.YZY:
					{
						rotateMatrix.m00 = c1 * c2 * c3 - s1 * s3; rotateMatrix.m01 = -c1 * s2; rotateMatrix.m02 = c3 * s1 + c1 * c2 * s3;
						rotateMatrix.m10 = c3 * s2; rotateMatrix.m11 = c2; rotateMatrix.m12 = s2 * s3;
						rotateMatrix.m20 = -c1 * s3 - c2 * c3 * s1; rotateMatrix.m21 = s1 * s2; rotateMatrix.m22 = c1 * c3 - c2 * s1 * s3;
						break;
					}
				case ERotateMatrixType.ZYZ:
					{
						rotateMatrix.m00 = c1 * c2 * c3 - s1 * s3; rotateMatrix.m01 = -c3 * s1 - c1 * c2 * s3; rotateMatrix.m02 = c1 * s2;
						rotateMatrix.m10 = c1 * s3 + c2 * c3 * s1; rotateMatrix.m11 = c1 * c3 - c2 * s1 * s3; rotateMatrix.m12 = s1 * s2;
						rotateMatrix.m20 = -c3 * s2; rotateMatrix.m21 = s2 * s3; rotateMatrix.m22 = c2;
						break;
					}
				case ERotateMatrixType.ZXZ:
					{
						rotateMatrix.m00 = c1 * c3 - c2 * s1 * s3; rotateMatrix.m01 = -c1 * s3 - c2 * c3 * s1; rotateMatrix.m02 = s1 * s2;
						rotateMatrix.m10 = c3 * s1 + c1 * c2 * s3; rotateMatrix.m11 = c1 * c2 * c3 - s1 * s3; rotateMatrix.m12 = -c1 * s2;
						rotateMatrix.m20 = s2 * s3; rotateMatrix.m21 = c3 * s2; rotateMatrix.m22 = c2;
						break;
					}
				case ERotateMatrixType.XZY:
					{
						rotateMatrix.m00 = c2 * c3; rotateMatrix.m01 = -s2; rotateMatrix.m02 = c2 * s3;
						rotateMatrix.m10 = s1 * s3 + c1 * c3 * s2; rotateMatrix.m11 = c1 * c2; rotateMatrix.m12 = c1 * s2 * s3 - c3 * s1;
						rotateMatrix.m20 = c3 * s1 * s2 - c1 * s3; rotateMatrix.m21 = c2 * s1; rotateMatrix.m22 = c1 * c3 + s1 * s2 * s3;
						break;
					}
				case ERotateMatrixType.XYZ:
					{
						rotateMatrix.m00 = c2 * c3; rotateMatrix.m01 = -c2 * s3; rotateMatrix.m02 = s2;
						rotateMatrix.m10 = c1 * s3 + c3 * s1 * s2; rotateMatrix.m11 = c1 * c3 - s1 * s2 * s3; rotateMatrix.m12 = -c2 * s1;
						rotateMatrix.m20 = s1 * s3 - c1 * c3 * s2; rotateMatrix.m21 = c3 * s1 + c1 * s2 * s3; rotateMatrix.m22 = c1 * c2;
						break;
					}
				case ERotateMatrixType.YXZ:
					{
						rotateMatrix.m00 = c1 * c3 + s1 * s2 * s3; rotateMatrix.m01 = c3 * s1 * s2 - c1 * s3; rotateMatrix.m02 = c2 * s1;
						rotateMatrix.m10 = c2 * s3; rotateMatrix.m11 = c2 * c3; rotateMatrix.m12 = -s2;
						rotateMatrix.m20 = c1 * s2 * s3 - c3 * s1; rotateMatrix.m21 = c3 * c3 * s2 + s1 * s3; rotateMatrix.m22 = c1 * c2;
						break;
					}
				case ERotateMatrixType.YZX:
					{
						rotateMatrix.m00 = c1 * c2; rotateMatrix.m01 = s1 * s3 - c1 * c3 * s2; rotateMatrix.m02 = c3 * s1 + c1 * s2 * s3;
						rotateMatrix.m10 = s2; rotateMatrix.m11 = c2 * c3; rotateMatrix.m12 = -c2 * s3;
						rotateMatrix.m20 = -c2 * s1; rotateMatrix.m21 = c1 * s3 + c3 * s1 * s2; rotateMatrix.m22 = c1 * c3 - s1 * s2 * s3;
						break;
					}
				case ERotateMatrixType.ZYX:
					{
						rotateMatrix.m00 = c1 * c2; rotateMatrix.m01 = c1 * s2 * s3 - c3 * s1; rotateMatrix.m02 = s1 * s3 + c1 * c3 * s2;
						rotateMatrix.m10 = c2 * s1; rotateMatrix.m11 = c1 * c3 + s1 * s2 * s3; rotateMatrix.m12 = c3 * s1 * s2 - c1 * s3;
						rotateMatrix.m20 = -s2; rotateMatrix.m21 = c2 * s3; rotateMatrix.m22 = c2 * c3;
						break;
					}
				case ERotateMatrixType.ZXY:
					{
						rotateMatrix.m00 = c1 * c3 - s1 * s2 * s3; rotateMatrix.m01 = -c2 * s1; rotateMatrix.m02 = c1 * s3 + c3 * s1 * s2;
						rotateMatrix.m10 = s3 * s1 + c1 * s2 * s3; rotateMatrix.m11 = c1 * c2; rotateMatrix.m12 = s1 * s3 - c1 * c3 * s2;
						rotateMatrix.m20 = -c2 * s3; rotateMatrix.m21 = s2; rotateMatrix.m22 = c2 * c3;
						break;
					}
				default:
					{
						//throw new ArgumentException("无效参数值[" + rotateMatrixType + "]！", nameof(rotateMatrixType));
						return Rotate(a, b, r, DefaultRotateMatrixType, false);
					}
			}
			return rotateMatrix;
		}

		/// <summary>
		/// 获取欧拉角度
		/// </summary>
		/// <param name="rotateMatrixType"></param>
		/// <param name="degreeOrRadians"></param>
		/// <returns></returns>
		public Vector3 GetEulerAngles(ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType, bool degreeOrRadians = true)
		{
			var (a, b, r) = GetABR(rotateMatrixType, degreeOrRadians);
			return FromABR(a, b, r, rotateMatrixType);
		}

		/// <summary>
		/// 获取ABR旋转顺序角度
		/// </summary>
		/// <param name="rotateMatrixType">旋转矩阵类型</param>
		/// <param name="degreeOrRadians">期望返回欧拉角度的单位是度还是弧度</param>
		/// <returns></returns>
		public (float a, float b, float r) GetABR(ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType, bool degreeOrRadians = true)
		{
			float a;
			float b;
			float r;
			switch (rotateMatrixType)
			{
				case ERotateMatrixType.XZX:
					{
						a = Mathf.Atan2(m20, m10);
						b = Mathf.Acos(m00);
						r = Mathf.Atan2(m02, -m01);
						break;
					}
				case ERotateMatrixType.XYX:
					{
						a = Mathf.Atan2(m10, -m20);
						b = Mathf.Acos(m00);
						r = Mathf.Atan2(m01, m02);
						break;
					}
				case ERotateMatrixType.YXY:
					{
						a = Mathf.Atan2(m01, m21);
						b = Mathf.Acos(m11);
						r = Mathf.Atan2(m10, -m12);
						break;
					}
				case ERotateMatrixType.YZY:
					{
						a = Mathf.Atan2(m21, -m01);
						b = Mathf.Acos(m11);
						r = Mathf.Atan2(m12, m10);
						break;
					}
				case ERotateMatrixType.ZYZ:
					{
						a = Mathf.Atan2(m12, m02);
						b = Mathf.Acos(m22);
						r = Mathf.Atan2(m21, -m20);
						break;
					}
				case ERotateMatrixType.ZXZ:
					{
						a = Mathf.Atan2(m02, -m12);
						b = Mathf.Acos(m22);
						r = Mathf.Atan2(m20, m21);
						break;
					}
				case ERotateMatrixType.XZY:
					{
						a = Mathf.Atan2(m21, m11);
						b = Mathf.Asin(-m01);
						r = Mathf.Atan2(m02, m00);
						break;
					}
				case ERotateMatrixType.XYZ:
					{
						a = Mathf.Atan2(-m12, m22);
						b = Mathf.Asin(m02);
						r = Mathf.Atan2(-m01, m00);
						break;
					}
				case ERotateMatrixType.YXZ:
					{
						a = Mathf.Atan2(m02, m22);
						b = Mathf.Asin(-m12);
						r = Mathf.Atan2(m10, m11);
						break;
					}
				case ERotateMatrixType.YZX:
					{
						a = Mathf.Atan2(-m20, m00);
						b = Mathf.Asin(m10);
						r = Mathf.Atan2(-m12, m11);
						break;
					}
				case ERotateMatrixType.ZYX:
					{
						a = Mathf.Atan2(m10, m00);
						b = Mathf.Asin(-m20);
						r = Mathf.Atan2(m21, m22);
						break;
					}
				case ERotateMatrixType.ZXY:
					{
						a = Mathf.Atan2(-m01, m11);
						b = Mathf.Asin(m21);
						r = Mathf.Atan2(-m20, m22);
						break;
					}
				default:
					{
						//throw new ArgumentException("无效参数值[" + rotateMatrixType + "]！", nameof(rotateMatrixType));
						return GetABR(DefaultRotateMatrixType, degreeOrRadians);
					}
			}
			return degreeOrRadians ? (a * Mathf.Rad2Deg, b * Mathf.Rad2Deg, r * Mathf.Rad2Deg) : (a, b, r);
		}

		/// <summary>
		/// 将ABR旋转顺序角度转为<see cref="Vector3"/>类型欧拉角度（用户直观认知的XYZ角度）：即调整ABR与XYZ的对应关系；
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="r"></param>
		/// <param name="rotateMatrixType"></param>
		/// <returns></returns>
		public static Vector3 FromABR(float a, float b, float r, ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType)
		{
			switch (rotateMatrixType)
			{
				case ERotateMatrixType.ZXY:
				case ERotateMatrixType.ZXZ:
					{
						return new Vector3(b, r, a);
					}
				case ERotateMatrixType.ZYZ:
				case ERotateMatrixType.ZYX:
					{
						return new Vector3(r, b, a);
					}
				case ERotateMatrixType.YXY:
				case ERotateMatrixType.YXZ:
					{
						return new Vector3(b, a, r);
					}
				case ERotateMatrixType.YZY:
				case ERotateMatrixType.YZX:
					{
						return new Vector3(r, a, b);
					}
				case ERotateMatrixType.XZX:
				case ERotateMatrixType.XZY:
					{
						return new Vector3(a, r, b);
					}
				case ERotateMatrixType.XYX:
				case ERotateMatrixType.XYZ:
					{
						return new Vector3(a, b, r);
					}
				default: return FromABR(a, b, r, DefaultRotateMatrixType);
			}
		}

		/// <summary>
		/// 将<see cref="Vector3"/>类型欧拉角度（用户直观认知的XYZ角度）转为ABR旋转顺序角度：即调整ABR与XYZ的对应关系；
		/// </summary>
		/// <param name="eulerAngles"></param>
		/// <param name="rotateMatrixType"></param>
		/// <returns></returns>
		public static (float a, float b, float r) ToABR(Vector3 eulerAngles, ERotateMatrixType rotateMatrixType = DefaultRotateMatrixType)
		{
			switch (rotateMatrixType)
			{
				case ERotateMatrixType.XZX: return (eulerAngles.x, eulerAngles.z, eulerAngles.y);
				case ERotateMatrixType.XYX: return (eulerAngles.x, eulerAngles.y, eulerAngles.z);
				case ERotateMatrixType.YXY: return (eulerAngles.y, eulerAngles.x, eulerAngles.z);
				case ERotateMatrixType.YZY: return (eulerAngles.y, eulerAngles.z, eulerAngles.x);
				case ERotateMatrixType.ZYZ: return (eulerAngles.z, eulerAngles.y, eulerAngles.x);
				case ERotateMatrixType.ZXZ: return (eulerAngles.z, eulerAngles.x, eulerAngles.y);
				case ERotateMatrixType.XZY: return (eulerAngles.x, eulerAngles.z, eulerAngles.y);
				case ERotateMatrixType.XYZ: return (eulerAngles.x, eulerAngles.y, eulerAngles.z);
				case ERotateMatrixType.YXZ: return (eulerAngles.y, eulerAngles.x, eulerAngles.z);
				case ERotateMatrixType.YZX: return (eulerAngles.y, eulerAngles.z, eulerAngles.x);
				case ERotateMatrixType.ZYX: return (eulerAngles.z, eulerAngles.y, eulerAngles.x);
				case ERotateMatrixType.ZXY: return (eulerAngles.z, eulerAngles.x, eulerAngles.y);
				default: return ToABR(eulerAngles, DefaultRotateMatrixType);//throw new ArgumentException("无效参数值[" + rotateMatrixType + "]！", nameof(rotateMatrixType));
			}
		}

		#endregion
	}
}
