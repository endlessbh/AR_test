using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginPhysicses.Tools.Grounds
{
	/// <summary>
	/// 痕迹
	/// </summary>
	[Name("痕迹")]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
	public class Skidmark : BasePhysicsMB
	{
		/// <summary>
		/// 最大划痕数
		/// </summary>
		[Name("最大划痕数")]
	    public int maxMarks = 1024;

		/// <summary>
		/// 划痕宽度
		/// </summary>
		[Name("划痕宽度")]
	    [Tip("需与产生划痕的对象匹配，例如轮胎划痕与轮胎宽度相同", "It needs to match the object causing the scratch, for example, the tire scratch is the same as the tire width")]
	    public float markWidth = 0.275f;

		/// <summary>
		/// 划痕与地面高度差
		/// </summary>
		[Name("划痕与地面高度差")]
	    public float groundOffset = 0.02f;

		/// <summary>
		/// 划痕之间最小距离
		/// </summary>
		[Name("划痕之间最小距离")]
	    public float minDistance = 0.1f;
	
	    private MeshFilter meshFilter;
	    private Mesh mesh;
	
	    private int indexShift;
	    private int numMarks = 0;
	
	    /// <summary>
	    /// 划痕详细信息
	    /// </summary>
	    public class markSection
	    {
	        public Vector3 pos = Vector3.zero;
	        public Vector3 normal = Vector3.zero;
	        public Vector4 tangent = Vector4.zero;
	        public Vector3 posl = Vector3.zero;
	        public Vector3 posr = Vector3.zero;
	        public float intensity = 0.0f;
	        public int lastIndex = 0;
	    };
	
	    /// <summary>
	    /// 划痕数组
	    /// </summary>
	    public markSection[] skidmarks;
	
	    private bool updated = false;
	
	    /// <summary>
	    /// 唤醒
	    /// </summary>
	    protected void Awake()
	    {
	        skidmarks = new markSection[maxMarks];
	
	        for (int i = 0; i < maxMarks; i++) skidmarks[i] = new markSection();
	
	        meshFilter = GetComponent<MeshFilter>();
	        mesh = meshFilter.mesh;
	    }
	
	    /// <summary>
	    /// 开始
	    /// </summary>
	    protected void Start()
	    {
	        transform.position = Vector3.zero;
	    }
	
	    /// <summary>
	    /// 动态创建划痕
	    /// </summary>
	    /// <param name="pos"></param>
	    /// <param name="normal"></param>
	    /// <param name="intensity"></param>
	    /// <param name="lastIndex"></param>
	    /// <returns></returns>
	    public int CreateSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex)
	    {
	        if (intensity < 0f) return -1;
	        if (intensity > 1f) intensity = 1f;
	
	        if (lastIndex > 0 && (pos - skidmarks[lastIndex % maxMarks].pos).sqrMagnitude < minDistance)
	        {
	            return lastIndex;
	        }
	
	        markSection curr = skidmarks[numMarks % maxMarks];
	        curr.pos = pos + normal * groundOffset;
	        curr.normal = normal;
	        curr.intensity = intensity;
	        curr.lastIndex = lastIndex;
	
	        if (lastIndex != -1)
	        {
	            markSection last = skidmarks[lastIndex % maxMarks];
	            Vector3 dir = (curr.pos - last.pos);
	            Vector3 xDir = Vector3.Cross(dir, normal).normalized;
	
	            var w = xDir * markWidth * 0.5f;
	            curr.posl = curr.pos + w;
	            curr.posr = curr.pos - w;
	            curr.tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);
	
	            if (last.lastIndex == -1)
	            {
	                last.tangent = curr.tangent;
	                last.posl = curr.pos + w;
	                last.posr = curr.pos - w;
	            }
	        }
	        numMarks++;
	        updated = true;
	        return numMarks - 1;
	    }
	
	    /// <summary>
	    /// 延迟更新
	    /// </summary>
	    protected void LateUpdate()
	    {
	        if (!updated) return;
	
	        updated = false;
	
	        mesh.Clear();
	
	        int segmentCount = 0;
	        for (int j = 0; j < numMarks && j < maxMarks; j++)
	        {
	            if (skidmarks[j].lastIndex != -1 && skidmarks[j].lastIndex > numMarks - maxMarks) segmentCount++;
	        }
	
	        var length = segmentCount * 4;
	        Vector3[] vertices = new Vector3[length];
	        Vector3[] normals = new Vector3[length];
	        Vector4[] tangents = new Vector4[length];
	        Color[] colors = new Color[length];
	        Vector2[] uvs = new Vector2[length];
	
	        int[] triangles = new int[segmentCount * 6];
	        segmentCount = 0;
	
	        for (int i = 0; i < numMarks && i < maxMarks; i++)
	        {
	            if (skidmarks[i].lastIndex != -1 && skidmarks[i].lastIndex > numMarks - maxMarks)
	            {
	                markSection curr = skidmarks[i];
	                markSection last = skidmarks[curr.lastIndex % maxMarks];
	
	                int baseIndex = segmentCount * 4;
	                var index1 = baseIndex;
	                var index2 = baseIndex + 1;
	                var index3 = baseIndex + 2;
	                var index4 = baseIndex + 3;
	                vertices[index1] = last.posl;
	                vertices[index2] = last.posr;
	                vertices[index3] = curr.posl;
	                vertices[index4] = curr.posr;
	
	                normals[index1] = last.normal;
	                normals[index2] = last.normal;
	                normals[index3] = curr.normal;
	                normals[index4] = curr.normal;
	
	                tangents[index1] = last.tangent;
	                tangents[index2] = last.tangent;
	                tangents[index3] = curr.tangent;
	                tangents[index4] = curr.tangent;
	
	                colors[index1] = new Color(0, 0, 0, last.intensity);
	                colors[index2] = new Color(0, 0, 0, last.intensity);
	                colors[index3] = new Color(0, 0, 0, curr.intensity);
	                colors[index4] = new Color(0, 0, 0, curr.intensity);
	
	                uvs[index1] = new Vector2(0, 0);
	                uvs[index2] = new Vector2(1, 0);
	                uvs[index3] = new Vector2(0, 1);
	                uvs[index4] = new Vector2(1, 1);
	
	                triangles[segmentCount * 6 + 0] = index1;
	                triangles[segmentCount * 6 + 2] = index2;
	                triangles[segmentCount * 6 + 1] = index3;
	
	                triangles[segmentCount * 6 + 3] = index3;
	                triangles[segmentCount * 6 + 5] = index2;
	                triangles[segmentCount * 6 + 4] = index4;
	                segmentCount++;
	            }
	        }
	
	        mesh.vertices = vertices;
	        mesh.normals = normals;
	        mesh.tangents = tangents;
	        mesh.triangles = triangles;
	        mesh.colors = colors;
	        mesh.uv = uvs;
	    }
	}
}