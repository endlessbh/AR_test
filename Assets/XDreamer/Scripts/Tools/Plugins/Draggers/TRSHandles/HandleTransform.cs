using UnityEngine;
using System.Runtime.Serialization;
using XCSJ.Attributes;
//using GILES.Serialization;

namespace XCSJ.PluginTools.Draggers.TRSHandles
{
	/// <summary>
	/// 处理变换，用于构建拖拽控制
	/// </summary>
	[Name("处理变换")]
	[System.Serializable]
	public class HandleTransform : System.IEquatable<HandleTransform>, ISerializable
	{
		// If the matrix needs rebuilt, this will be true.  Used to delay expensive
		// matrix construction til necessary (since t/r/s can change a lot before a
		// matrix is needed).
		private bool dirty = true;

		[SerializeField] private Vector3 _position;

		[SerializeField] private Quaternion _rotation;

		[SerializeField] private Vector3 _scale;

        public Vector3 position { get { return _position; } set { dirty = true; _position = value; } }

        public Quaternion rotation { get { return _rotation; } set { dirty = true; _rotation = value; } }

        public Vector3 scale { get { return _scale; } set { dirty = true; _scale = value; } }

        private Matrix4x4 matrix;

		public static readonly HandleTransform identity = new HandleTransform(Vector3.zero, Quaternion.identity, Vector3.one);

		public HandleTransform() : this(Vector3.zero, Quaternion.identity, Vector3.one)
		{
			this.matrix = Matrix4x4.identity;
		}

		public HandleTransform(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position 	= position;
			this.rotation 	= rotation;
			this.scale		= scale;

			this.matrix 	= Matrix4x4.TRS(position, rotation, scale);
			this.dirty 	= false;
		}

		public HandleTransform(Transform transform) : this(transform.position, transform.localRotation, transform.localScale)
        {
        }

        public HandleTransform(HandleTransform transform) : this(transform.position, transform.rotation, transform.scale)
		{
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("position", (Vector3)_position, typeof(Vector3));
			info.AddValue("rotation", (Quaternion)_rotation, typeof(Quaternion));
			info.AddValue("scale", (Vector3)_scale, typeof(Vector3));
		}

		public HandleTransform(SerializationInfo info, StreamingContext context)
		{
			this._position = (Vector3) info.GetValue("position", typeof(Vector3));
			this._rotation = (Quaternion) info.GetValue("rotation", typeof(Quaternion));
			this._scale = (Vector3) info.GetValue("scale", typeof(Vector3));
			this.dirty = true;
		}

		public void SetTRS(Transform transform)
		{
			this.position 	= transform.position;
			this.rotation 	= transform.rotation;
			this.scale		= transform.localScale;
			this.dirty 		= true;
		}

		bool Approx(Vector3 lhs, Vector3 rhs)
		{
			return 	Mathf.Abs(lhs.x - rhs.x) < Mathf.Epsilon && Mathf.Abs(lhs.y - rhs.y) < Mathf.Epsilon && Mathf.Abs(lhs.z - rhs.z) < Mathf.Epsilon;
		}

		bool Approx(Quaternion lhs, Quaternion rhs)
		{
			return 	Mathf.Abs(lhs.x - rhs.x) < Mathf.Epsilon && Mathf.Abs(lhs.y - rhs.y) < Mathf.Epsilon && Mathf.Abs(lhs.z - rhs.z) < Mathf.Epsilon && Mathf.Abs(lhs.w - rhs.w) < Mathf.Epsilon;
		}

		public bool Equals(HandleTransform rhs)
		{
			return 	Approx(this.position, rhs.position) && Approx(this.rotation, rhs.rotation) && Approx(this.scale, rhs.scale);
		}

		public override bool Equals(System.Object rhs) => rhs is HandleTransform && this.Equals((HandleTransform)rhs);

		public override int GetHashCode() => position.GetHashCode() ^ rotation.GetHashCode() ^ scale.GetHashCode();

		public Matrix4x4 GetMatrix()
		{
            if (this.dirty)
            {
                this.dirty = false;
                matrix = Matrix4x4.TRS(position, rotation, scale);
            }
            return matrix;
        }

		public static HandleTransform operator - (HandleTransform lhs, HandleTransform rhs)
        {
			return new HandleTransform(lhs.position - rhs.position, Quaternion.Inverse(rhs.rotation) * lhs.rotation,
				new Vector3(lhs.scale.x / rhs.scale.x, lhs.scale.y / rhs.scale.y, lhs.scale.z / rhs.scale.z));
        }

        public static HandleTransform operator +(HandleTransform lhs, HandleTransform rhs)
        {
            return new HandleTransform(lhs.position + rhs.position, lhs.rotation * rhs.rotation, new Vector3(lhs.scale.x * rhs.scale.x, lhs.scale.y * rhs.scale.y, lhs.scale.z * rhs.scale.z));
        }

        public static HandleTransform operator +(Transform lhs, HandleTransform rhs)
        {
			return new HandleTransform(lhs.position + rhs.position, lhs.localRotation * rhs.rotation, new Vector3(lhs.localScale.x * rhs.scale.x, lhs.localScale.y * rhs.scale.y, lhs.localScale.z * rhs.scale.z));
        }

		public static bool operator == (HandleTransform lhs, HandleTransform rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs) || lhs.Equals(rhs);
		}

		public static bool operator != (HandleTransform lhs, HandleTransform rhs)
		{
			return !(lhs == rhs);
		}

		public Vector3 up { get { return rotation * Vector3.up; }	}

		public Vector3 forward { get { return rotation * Vector3.forward; }	}

		public Vector3 right { get { return rotation * Vector3.right; }	}

		public override string ToString()
		{
			return position.ToString("F2") + "\n" + rotation.ToString("F2") + "\n" + scale.ToString("F2");
		}
	}
}