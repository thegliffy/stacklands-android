using System;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsExtensions
{
	private class AscendingDistanceComparer : IComparer<RaycastHit>
	{
		public int Compare(RaycastHit h1, RaycastHit h2)
		{
			if (!(h1.distance < h2.distance))
			{
				if (!(h1.distance > h2.distance))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}
	}

	private static AscendingDistanceComparer ascendDistance = new AscendingDistanceComparer();

	public static bool BoxCast(BoxCollider box, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool BoxCast(BoxCollider box, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] BoxCastAll(BoxCollider box, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int BoxCastNonAlloc(BoxCollider box, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckBox(BoxCollider box, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.CheckBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapBox(BoxCollider box, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
	}

	public static int OverlapBoxNonAlloc(BoxCollider box, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, queryTriggerInteraction);
	}

	public static int OverlapTwoBoxNonAlloc(BoxCollider box, BoxCollider box2, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		Bounds bounds = box.bounds;
		Bounds bounds2 = box2.bounds;
		bounds.Encapsulate(bounds2.min);
		bounds.Encapsulate(bounds2.max);
		return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results, Quaternion.identity, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceBox(this BoxCollider box, out Vector3 center, out Vector3 halfExtents, out Quaternion orientation)
	{
		orientation = box.transform.rotation;
		center = box.transform.TransformPoint(box.center);
		Vector3 a = AbsVec3(box.transform.lossyScale);
		halfExtents = Vector3.Scale(a, box.size) * 0.5f;
	}

	public static void ToWorldSpaceBox2(this BoxCollider box, out Vector3 halfExtents)
	{
		Vector3 lossyScale = box.transform.lossyScale;
		Vector3 a = new Vector3(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z));
		halfExtents = Vector3.Scale(a, box.size) * 0.5f;
	}

	public static bool SphereCast(SphereCollider sphere, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCast(center, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] SphereCastAll(SphereCollider sphere, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCastAll(center, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int SphereCastNonAlloc(SphereCollider sphere, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCastNonAlloc(center, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckSphere(SphereCollider sphere, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.CheckSphere(center, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapSphere(SphereCollider sphere, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.OverlapSphere(center, radius, layerMask, queryTriggerInteraction);
	}

	public static int OverlapSphereNonAlloc(SphereCollider sphere, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.OverlapSphereNonAlloc(center, radius, results, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceSphere(this SphereCollider sphere, out Vector3 center, out float radius)
	{
		center = sphere.transform.TransformPoint(sphere.center);
		radius = sphere.radius * MaxVec3(AbsVec3(sphere.transform.lossyScale));
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCast(point, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] CapsuleCastAll(CapsuleCollider capsule, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCastAll(point, point2, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int CapsuleCastNonAlloc(CapsuleCollider capsule, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCastNonAlloc(point, point2, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckCapsule(CapsuleCollider capsule, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CheckCapsule(point, point2, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapCapsule(CapsuleCollider capsule, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.OverlapCapsule(point, point2, radius, layerMask, queryTriggerInteraction);
	}

	public static int OverlapCapsuleNonAlloc(CapsuleCollider capsule, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.OverlapCapsuleNonAlloc(point, point2, radius, results, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceCapsule(this CapsuleCollider capsule, out Vector3 point0, out Vector3 point1, out float radius)
	{
		Vector3 vector = capsule.transform.TransformPoint(capsule.center);
		radius = 0f;
		float num = 0f;
		Vector3 vector2 = AbsVec3(capsule.transform.lossyScale);
		Vector3 vector3 = Vector3.zero;
		switch (capsule.direction)
		{
		case 0:
			radius = Mathf.Max(vector2.y, vector2.z) * capsule.radius;
			num = vector2.x * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.right);
			break;
		case 1:
			radius = Mathf.Max(vector2.x, vector2.z) * capsule.radius;
			num = vector2.y * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.up);
			break;
		case 2:
			radius = Mathf.Max(vector2.x, vector2.y) * capsule.radius;
			num = vector2.z * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.forward);
			break;
		}
		if (num < radius * 2f)
		{
			vector3 = Vector3.zero;
		}
		point0 = vector + vector3 * (num * 0.5f - radius);
		point1 = vector - vector3 * (num * 0.5f - radius);
	}

	public static void SortClosestToFurthest(RaycastHit[] hits, int hitCount = -1)
	{
		if (hitCount != 0)
		{
			if (hitCount < 0)
			{
				hitCount = hits.Length;
			}
			Array.Sort(hits, 0, hitCount, ascendDistance);
		}
	}

	private static Vector3 AbsVec3(Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	private static float MaxVec3(Vector3 v)
	{
		return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
	}
}
