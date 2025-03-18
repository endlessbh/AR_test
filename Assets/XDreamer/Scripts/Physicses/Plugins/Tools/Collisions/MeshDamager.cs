using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
	/// <summary>
	/// 网格变形器
	/// </summary>
	[DisallowMultipleComponent]
	[Name("网格变形器")]
	[XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(PhysicsManager))]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class MeshDamager : ColliderTrigger
    {
		/// <summary>
		/// 不可变形对象
		/// </summary>
		[Name("不可变形对象列表")]
		[Tip("不可变形对象及子对象都不会因为碰撞而变形！", "Non deformable objects and sub objects will not deform due to collision!")]
		[Readonly(EEditorMode.Runtime)]
		public List<Transform> _cannotDeformableObjects = new List<Transform>();

		/// <summary>
		/// 随机破坏值
		/// </summary>
		[Name("随机破坏值")]
		public float _randomizeVertices = 1f;

		/// <summary>
		/// 碰撞影响半径
		/// </summary>
		[Name("碰撞影响半径")]
		public float _radius = .5f;

		/// <summary>
		/// 最小形变距离
		/// </summary>
		[Range(0, 1000)]
		[Name("最小形变距离")]
		public float _minDeform = .5f;

		/// <summary>
		/// 破坏系数
		/// </summary>
		[Name("破坏系数")]
		public float multiplier = 1f;

		#region 网格顶点数据

		/// <summary>
		/// 变形网格对象
		/// </summary>
		private MeshFilter[] deformableMeshFilters;
		private struct MeshVertices
		{
			public Vector3[] vertices;
		}
		private MeshVertices[] originalMeshData = new MeshVertices[0];

		#endregion

		#region 网格修理

		/// <summary>
		/// 修复顶点最小距离
		/// </summary>
		private float minimumVertDistanceForDamagedMesh = .002f;

		/// <summary>
		/// 修理中
		/// </summary>
		public bool repairing { get; private set; } = false;

		/// <summary>
		/// 修理完成
		/// </summary>
		public bool repaired { get; private set; } = true;

		/// <summary>
		/// 修复速度
		/// </summary>
		private float repairSpeed = 2;

		#endregion

		/// <summary>
		/// 唤醒
		/// </summary>
		protected virtual void Awake()
        {
			RecordMeshVertices();
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected virtual void Update()
		{
			if (repairing)
			{
				Repairing(Time.deltaTime * repairSpeed);
			}
		}

		private float _minVelocity = 0;

		/// <summary>
		/// 碰撞
		/// </summary>
		/// <param name="collision"></param>
		protected void OnCollisionEnterWithMinVelocity(Collision collision)
		{
			var contactNormal = collision.GetContact(0).normal;
			var colRelVel = collision.relativeVelocity * (1f - Mathf.Abs(Vector3.Dot(transform.up, contactNormal)));

			var cos = Mathf.Abs(Vector3.Dot(contactNormal, colRelVel.normalized));

			if (colRelVel.magnitude * cos >= _minVelocity)
			{
				var localVector = transform.InverseTransformDirection(colRelVel) * multiplier / 50f;

				for (int i = 0; i < deformableMeshFilters.Length; i++)
				{
					var mf = deformableMeshFilters[i];
					DeformMesh(mf, originalMeshData[i].vertices, collision, cos, localVector, Quaternion.identity);
				}
			}
		}

		private void RecordMeshVertices()
		{
			var properMeshFilters = new List<MeshFilter>();
			foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
			{
				if (!mf.mesh.isReadable)
				{
					Debug.LogError(mf.transform.name + "不可变形， 网格对象不可读写！");
					continue;
				}

				if (_cannotDeformableObjects.Exists(t => t && (t == mf.transform || mf.transform.IsChildOf(t))))
				{
					continue;
				}

				properMeshFilters.Add(mf);
			}
			deformableMeshFilters = properMeshFilters.ToArray();

			// 保持原有数据
			originalMeshData = new MeshVertices[deformableMeshFilters.Length];
			for (int i = 0; i < deformableMeshFilters.Length; i++)
			{
				originalMeshData[i].vertices = deformableMeshFilters[i].mesh.vertices;
			}
		}

		/// <summary>
		/// 开始修理
		/// </summary>
		public void BeginRepair(float repairSpeed)
		{
			repairing = true;
			this.repairSpeed = repairSpeed;
		}

		/// <summary>
		/// 停止修理
		/// </summary>
		public void StopRepair() => repairing = false;

		/// <summary>
		/// 修理破坏网格
		/// </summary>
		/// <param name="multiplier">修复进度倍速</param>
		protected void Repairing(float multiplier)
		{
			repaired = true;

			multiplier = Mathf.Clamp01(multiplier);

			for (int k = 0; k < deformableMeshFilters.Length; k++)
			{
				var vertices = deformableMeshFilters[k].mesh.vertices;

				for (int i = 0; i < vertices.Length; i++)
				{
					var offset = originalMeshData[k].vertices[i] - vertices[i];
					vertices[i] += offset * multiplier;
					if (offset.magnitude >= minimumVertDistanceForDamagedMesh)
					{
						repaired = false;
					}
				}

				SetMeshVertices(deformableMeshFilters[k].mesh, vertices);
			}

			if (repaired) repairing = false;
		}

		/// <summary>
		/// 变形模型
		/// </summary>
		/// <param name="meshFilter"></param>
		/// <param name="originalMesh"></param>
		/// <param name="collision"></param>
		/// <param name="cos"></param>
		/// <param name="localVector"></param>
		/// <param name="rot"></param>
		private void DeformMesh(MeshFilter meshFilter, Vector3[] originalMesh, Collision collision, float cos, Vector3 localVector, Quaternion rot)
		{
			var vertices = meshFilter.mesh.vertices;

			foreach (ContactPoint contact in collision.contacts)
			{
				var point = meshFilter.transform.InverseTransformPoint(contact.point);

				for (int i = 0; i < vertices.Length; i++)
				{
					var distance = (point - vertices[i]).magnitude;
					if (distance < _radius)
					{
						var v = vertices[i];
						var n = new Vector3(Mathf.Sin(v.y * 1000), Mathf.Sin(v.z * 1000), Mathf.Sin(v.x * 100)).normalized * _randomizeVertices / 500f;
						vertices[i] += rot * (localVector * (_radius - distance) / _radius * cos + n);

						var offset = vertices[i] - originalMesh[i];
						if (offset.magnitude > _minDeform)
						{
							vertices[i] = originalMesh[i] + offset.normalized * _minDeform;
						}
					}
				}
			}

			SetMeshVertices(meshFilter.mesh, vertices);
		}

		private void SetMeshVertices(Mesh mesh, Vector3[] vertices)
		{
			mesh.vertices = vertices;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
    }
}
