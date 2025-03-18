#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// Base class used to perform 'ground' detection.
    /// This can be extended to add game specific features.
    /// </summary>  
    [Name("基础地面检测")]
    [RequireComponent(typeof(CapsuleCollider))]
    [DisallowMultipleComponent]
    public abstract class BaseGroundDetection : BaseCharacterMB
    {
        #region EDITOR EXPOSED FIELDS

        [Name("地面层")]
        [Tip("被视为‘地面’的层（可行走）。", "A layer considered 'ground' (walkable). ")]
        [SerializeField]
        public LayerMask _groundMask = 1;

        [Name("地面限定")]
        [Tip("将被视为‘地面’的最大角度（单位：角度）；任何大于本值的都被当做墙。", "The maximum angle (in angles) that will be considered 'ground'; Anything greater than this value is treated as a wall.")]
        [SerializeField]
        public float _groundLimit = 60.0f;

        [Name("台阶偏移")]
        [Tip("有效台阶的最大高度（单位：米）；当角色离地面距离接近本值时，角色会踏上楼梯；根据经验,本值为当前角色碰撞器的半径。")]
        [SerializeField]
        public float _stepOffset = 0.5f;

        [Name("突出偏移")]
        [Tip("角色可以站立在窗台上而不向下滑动的最大水平距离（单位：米）", "The maximum horizontal distance (in meters) that a character can stand on a windowsill without sliding down")]
        [SerializeField]
        public float _ledgeOffset = 0;

        [Name("投射距离")]
        [Tip("检测投射的最大距离；根据经验,本值为当前角色碰撞器的半径。", "Detect the maximum distance of projection; As a rule of thumb, this value is the radius of the current character collider.")]
        [SerializeField]
        public float _castDistance = 0.5f;

        [Name("触发器交互")]
        [Tip("标识触发器是否应该作为地面?", "Should the identification trigger be used as the ground?")]
        [SerializeField]
        public QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;

        #endregion

        #region FIELDS

        private static readonly Collider[] OverlappedColliders = new Collider[16];

        protected const float kBackstepDistance = 0.05f;
        protected const float kMinCastDistance = 0.01f;
        protected const float kMinLedgeDistance = 0.05f;
        protected const float kMinStepOffset = 0.10f;
        protected const float kHorizontalOffset = 0.001f;

        private CapsuleCollider _capsuleCollider;

        protected GroundHit _groundHitInfo;

        private LayerMask _overlapMask = -1;

        private int _ignoreRaycastLayer = 2;
        private int _cachedLayer;
        
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Layers to be considered as 'ground' (walkables).
        /// </summary>

        public LayerMask groundMask
        {
            get { return _groundMask; }
            set { _groundMask = value; }
        }

        /// <summary>
        /// The maximum angle that will be accounted as 'ground', anything above is treated as a 'wall'.
        /// </summary>

        public float groundLimit
        {
            get { return _groundLimit; }
            set { _groundLimit = Mathf.Clamp(value, 0.0f, 89.0f); }
        }

        /// <summary>
        /// The maximum height (in meters) for a valid step.
        /// This will detect a step only if it is closer to the ground than this value.
        /// As rule of thumb, configure it to your character's collider radius.
        /// </summary>

        public float stepOffset
        {
            get { return _stepOffset; }
            set { _stepOffset = Mathf.Clamp(value, kMinStepOffset, capsuleCollider.radius); }
        }

        /// <summary>
        /// The maximum horizontal distance (in meters) a character can stand on a ledge without slide down.
        /// </summary>

        public float ledgeOffset
        {
            get { return _ledgeOffset; }
            set { _ledgeOffset = Mathf.Clamp(value, 0.0f, capsuleCollider.radius); }
        }

        /// <summary>
        /// Determines the max length of the cast.
        /// As rule of thumb, configure it to your character's collider radius.
        /// </summary>

        public float castDistance
        {
            get { return _castDistance; }
            set { _castDistance = Mathf.Max(kMinCastDistance, value); }
        }

        /// <summary>
        /// Specifies whether casts should hit Triggers.
        /// </summary>

        public QueryTriggerInteraction triggerInteraction
        {
            get { return _triggerInteraction; }
            set { _triggerInteraction = value; }
        }

        /// <summary>
        /// Cached capsule collider.
        /// </summary>

        public CapsuleCollider capsuleCollider
        {
            get
            {
                if (_capsuleCollider == null)
                    _capsuleCollider = GetComponent<CapsuleCollider>();

                return _capsuleCollider;
            }
        }

        /// <summary>
        /// Is this character standing on ANY 'ground'?
        /// </summary>

        public bool isOnGround
        {
            get { return _groundHitInfo.isOnGround; }
        }

        /// <summary>
        /// Is this character standing on VALID 'ground'?
        /// </summary>

        public bool isValidGround
        {
            get { return _groundHitInfo.isValidGround; }
        }

        /// <summary>
        /// Is this character standing on the 'solid' side of a ledge?
        /// </summary>

        public bool isOnLedgeSolidSide
        {
            get { return _groundHitInfo.isOnLedgeSolidSide; }
        }

        /// <summary>
        /// Is this character standing on the 'empty' side of a ledge?
        /// </summary>

        public bool isOnLedgeEmptySide
        {
            get { return _groundHitInfo.isOnLedgeEmptySide; }
        }

        /// <summary>
        /// The horizontal distance from the character's bottom position to the ledge contact point.
        /// </summary>

        public float ledgeDistance
        {
            get { return _groundHitInfo.ledgeDistance; }
        }

        /// <summary>
        /// Is this character standing on a step?
        /// </summary>

        public bool isOnStep
        {
            get { return _groundHitInfo.isOnStep; }
        }

        /// <summary>
        /// The current step height.
        /// </summary>

        public float stepHeight
        {
            get { return _groundHitInfo.stepHeight; }
        }

        /// <summary>
        /// Is this character standing on a slope?
        /// </summary>

        public bool isOnSlope
        {
            get { return !Mathf.Approximately(groundAngle, 0.0f); }
        }

        /// <summary>
        /// The contact point (in world space) where the cast hit the 'ground' collider.
        /// </summary>

        public Vector3 groundPoint
        {
            get { return _groundHitInfo.groundPoint; }
        }

        /// <summary>
        /// The normal of the 'ground' surface.
        /// </summary>

        public Vector3 groundNormal
        {
            get { return _groundHitInfo.groundNormal; }
        }

        /// <summary>
        /// The distance from the ray's origin to the impact point.
        /// </summary>

        public float groundDistance
        {
            get { return _groundHitInfo.groundDistance; }
        }

        /// <summary>
        /// The Collider that was hit.
        /// This is null if the cast hit nothing.
        /// </summary>

        public Collider groundCollider
        {
            get { return _groundHitInfo.groundCollider; }
        }

        /// <summary>
        /// The Rigidbody of the collider that was hit.
        /// If the collider is not attached to a rigidbody then it is null.
        /// </summary>

        public Rigidbody groundRigidbody
        {
            get { return _groundHitInfo.groundRigidbody; }
        }

        /// <summary>
        /// The 'ground' angle (in degrees) the character is standing on.
        /// </summary>

        public float groundAngle
        {
            get { return !isOnGround ? 0.0f : Vector3.Angle(surfaceNormal, transform.up); }
        }

        /// <summary>
        /// The real surface normal.
        /// 
        /// This cab be different from groundNormal, because when SphereCast contacts the edge of a collider
        /// (rather than a face directly on) the hit.normal that is returned is the interpolation of the two normals
        /// of the faces that are joined to that edge.
        /// </summary>

        public Vector3 surfaceNormal
        {
            get { return _groundHitInfo.surfaceNormal; }
        }

        /// <summary>
        /// The current GroundHit info.
        /// </summary>

        public GroundHit groundHit
        {
            get { return _groundHitInfo; }
        }

        /// <summary>
        /// The previous frame GroundHit info.
        /// </summary>

        public GroundHit prevGroundHit { get; private set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize overlap mask, this uses the rigidbody's layer's collision matrix.
        /// </summary>

        protected virtual void InitializeOverlapMask()
        {
            var layer = gameObject.layer;

            _overlapMask = 0;
            for (var i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                    _overlapMask |= 1 << i;
            }
        }

        /// <summary>
        /// Helper method.
        /// Check the character's capsule against the physics world and return all overlapping colliders.
        /// </summary>
        /// <param name="position">The probing position.</param>
        /// <param name="rotation">The probing rotation.</param>
        /// <param name="overlapCount">The amount of entries written to the buffer.</param>

        public Collider[] OverlapCapsule(Vector3 position, Quaternion rotation, out int overlapCount)
        {
            var center = capsuleCollider.center;
            var radius = capsuleCollider.radius;

            var height = capsuleCollider.height * 0.5f - radius;

            var topSphereCenter = center + Vector3.up * height;
            var bottomSphereCenter = center - Vector3.up * height;

            var top = position + rotation * topSphereCenter;
            var bottom = position + rotation * bottomSphereCenter;

            var colliderCount = Physics.OverlapCapsuleNonAlloc(bottom, top, radius, OverlappedColliders, _overlapMask,
                triggerInteraction);

            overlapCount = colliderCount;
            for (var i = 0; i < colliderCount; i++)
            {
                var overlappedCollider = OverlappedColliders[i];
                if (overlappedCollider != null && overlappedCollider != capsuleCollider)
                    continue;

                if (i < --overlapCount)
                    OverlappedColliders[i] = OverlappedColliders[overlapCount];
            }

            return OverlappedColliders;
        }

        /// <summary>
        /// Raycast helper method.
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        
        protected bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance,
            float backstepDistance = kBackstepDistance)
        {
            origin = origin - direction * backstepDistance;

            var hit = Physics.Raycast(origin, direction, out hitInfo, distance + backstepDistance, groundMask,
                triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }

        /// <summary>
        /// SphereCast helper method.
        /// </summary>
        /// <param name="origin">The center of the sphere at the start of the sweep.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="direction">The direction into which to sweep the sphere.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        
        protected bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo,
            float distance, float backstepDistance = kBackstepDistance)
        {
            origin = origin - direction * backstepDistance;

            var hit = Physics.SphereCast(origin, radius, direction, out hitInfo, distance + backstepDistance,
                groundMask, triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }

        /// <summary>
        /// CapsuleCast helper method.
        /// </summary>
        /// <param name="bottom">The center of the sphere at the start of the capsule.</param>
        /// <param name="top">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="direction">The direction into which to sweep the sphere.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when the intersects any 'ground' collider, otherwise false.</returns>
        
        protected bool CapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 direction, out RaycastHit hitInfo,
            float distance, float backstepDistance = kBackstepDistance)
        {
            top = top - direction * backstepDistance;
            bottom = bottom - direction * backstepDistance;

            var hit = Physics.CapsuleCast(bottom, top, radius, direction, out hitInfo, distance + backstepDistance,
                groundMask, triggerInteraction);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }

        /// <summary>
        /// Sweep helper method.
        /// Tests if a character would collide with anything, if it was moved through the scene.
        /// </summary>
        /// <param name="position">Character's potision, this can be different from character's current rotation.</param>
        /// <param name="rotation">Character's rotation, this can be different from character's current rotation.</param>
        /// <param name="direction">The direction into which to sweep the character's collider.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The max length of the sweep.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>

        public virtual bool SweepTest(Vector3 position, Quaternion rotation, Vector3 direction, out RaycastHit hitInfo,
            float distance = Mathf.Infinity, float backstepDistance = kBackstepDistance)
        {
            var radius = capsuleCollider.radius;
            var height = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            
            var bottomSphereCenter = capsuleCollider.center - Vector3.up * height;
            var topSphereCenter = capsuleCollider.center + Vector3.up * height;

            var bottom = position + rotation * bottomSphereCenter;
            var top = position + rotation * topSphereCenter;

            return CapsuleCast(bottom, top, radius, direction, out hitInfo, distance, backstepDistance);
        }

        /// <summary>
        /// Temporaly moves the character's collider to 'Ignore Raycast' layer to
        /// prevent any collision against it during 'grounding' tests.
        /// </summary>
        
        protected virtual void DisableRaycastCollisions()
        {
            _cachedLayer = gameObject.layer;
            gameObject.layer = _ignoreRaycastLayer;
        }

        /// <summary>
        /// Restore character's collider layer.
        /// </summary>
        
        protected virtual void EnableRaycastCollisions()
        {
            gameObject.layer = _cachedLayer;
        }

        /// <summary>
        /// Reset the 'ground' hit info.
        /// </summary>
        
        public virtual void ResetGroundInfo()
        {
            var up = transform.up;

            prevGroundHit = new GroundHit(_groundHitInfo);
            _groundHitInfo = new GroundHit
            {
                groundPoint = transform.position,
                groundNormal = up,
                surfaceNormal = up
            };
        }

        /// <summary>
        /// Compute 'ground' hit info casting downwards the character's volume,
        /// if found any 'ground' groundHitInfo will contain additional information about it.
        /// Returns true when intersects any 'ground' collider, otherwise false.
        /// </summary>
        /// <param name="position">A probing position. This can be different from character's position.</param>
        /// <param name="rotation">A probing rotation. This can be different from character's rotation.</param>
        /// <param name="groundHitInfo">If true is returned, will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the cast.</param>
        
        public abstract bool ComputeGroundHit(Vector3 position, Quaternion rotation, ref GroundHit groundHitInfo,
            float distance = Mathf.Infinity);

        /// <summary>
        /// Perform ground detection.
        /// </summary>

        public void DetectGround()
        {
            // Prevent hit character's collider during 'grounding' tests

            DisableRaycastCollisions();

            // Detect ground

            ComputeGroundHit(transform.position, transform.rotation, ref _groundHitInfo, castDistance);

            // Re-enable character's collisions

            EnableRaycastCollisions();
        }

        /// <summary>
        /// Test if would collide with 'ground', if it was moved through the scene (eg: sweep).
        /// Returns true when intersects any 'ground' collider, otherwise false.
        /// </summary>
        /// <param name="direction">The direction into which to sweep.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the sweep.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        
        public abstract bool FindGround(Vector3 direction, out RaycastHit hitInfo, float distance = Mathf.Infinity,
            float backstepDistance = kBackstepDistance);

        /// <summary>
        /// Draw gizmos (on editor only).
        /// </summary>

        protected virtual void DrawGizmos()
        {
#if UNITY_EDITOR
            
            if (!Application.isPlaying)
                return;

            if (!isOnGround)
                return;

            // Ground point

            var color = new Color(0.0f, 1.0f, 0.0f, 0.25f);
            if (!isValidGround)
                color = new Color(0.0f, 0.0f, 1.0f, 0.25f);

            Handles.color = color;
            Handles.DrawSolidDisc(groundPoint, surfaceNormal, 0.1f);

            // Surface normal

            Gizmos.color = isValidGround ? Color.green : Color.blue;
            Gizmos.DrawRay(groundPoint, surfaceNormal);

            // Ground normal

            if (groundNormal != surfaceNormal)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(groundPoint, groundNormal);
            }

            // Step height

            if (!isOnStep)
                return;

            var stepPoint = groundPoint - transform.up * stepHeight;

            Gizmos.color = Color.black;
            Gizmos.DrawLine(groundPoint, stepPoint);

            Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            Handles.DrawSolidDisc(stepPoint, transform.up, 0.1f);

#endif
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Validate this editor exposed fields.
        /// If you overrides it, be sure to call base.OnValidate to fully validate base class.
        /// </summary>

        protected virtual void OnValidate()
        {
            groundLimit = _groundLimit;
            stepOffset = _stepOffset;
            ledgeOffset = _ledgeOffset;
            castDistance = _castDistance;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>

        protected virtual void Awake()
        {
            InitializeOverlapMask();

            _ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        }

        /// <summary>
        /// Draw gizmos.
        /// </summary>

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        #endregion
    }
}
