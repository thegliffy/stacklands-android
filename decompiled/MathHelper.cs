using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
	public static Color DebugColor;

	public static bool ContainsPoint(List<Vector2> polyPoints, Vector2 p)
	{
		int index = polyPoints.Count - 1;
		bool flag = false;
		int num = 0;
		while (num < polyPoints.Count)
		{
			if (((polyPoints[num].y <= p.y && p.y < polyPoints[index].y) || (polyPoints[index].y <= p.y && p.y < polyPoints[num].y)) && p.x < (polyPoints[index].x - polyPoints[num].x) * (p.y - polyPoints[num].y) / (polyPoints[index].y - polyPoints[num].y) + polyPoints[num].x)
			{
				flag = !flag;
			}
			index = num++;
		}
		return flag;
	}

	public static bool IsNan(this Vector3 vec)
	{
		if (!float.IsNaN(vec.x) && !float.IsNaN(vec.y))
		{
			return float.IsNaN(vec.z);
		}
		return true;
	}

	public static bool LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, out Vector3 intersection)
	{
		Vector3 lhs = linePoint2 - linePoint1;
		Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
		Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
		if (Mathf.Abs(Vector3.Dot(lhs, rhs)) < 0.0001f && rhs.sqrMagnitude > 0.0001f)
		{
			float num = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
			intersection = linePoint1 + lineVec1 * num;
			return true;
		}
		intersection = Vector3.zero;
		return false;
	}

	public static bool IsSameDirection(Vector2 a, Vector2 a_dir, Vector2 b, Vector2 b_dir)
	{
		Vector2 vector = b - a;
		if (Vector2.Dot(a_dir, vector) < 0f)
		{
			vector = -vector;
		}
		return Vector2.Dot(vector, b_dir) >= 0f;
	}

	public static bool IsSameDirectionProjected(Vector3 a, Vector3 a_dir, Vector3 b, Vector3 b_dir)
	{
		return IsSameDirection(To2D(a), To2D(a_dir), To2D(b), To2D(b_dir));
	}

	private static Vector2 To2D(Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	private static Vector3 From2D(Vector2 v)
	{
		return new Vector3(v.x, 0f, v.y);
	}

	public static float PointLineDistance(Vector2 start, Vector2 end, Vector2 point)
	{
		Vector2 vector = ProjectPointOnLine(start, end, point);
		return (point - vector).magnitude;
	}

	public static float PointLineDistanceSqr(Vector2 start, Vector2 end, Vector2 point)
	{
		Vector2 vector = ProjectPointOnLine(start, end, point);
		return (point - vector).sqrMagnitude;
	}

	public static Vector2 ClosestPointOnRectangle(Rect rect, Vector2 point)
	{
		Vector2[] array = new Vector2[4]
		{
			new Vector2(rect.xMin, rect.yMin),
			new Vector2(rect.xMax, rect.yMin),
			new Vector2(rect.xMax, rect.yMax),
			new Vector2(rect.xMin, rect.yMax)
		};
		float num = float.MaxValue;
		Vector2 result = rect.center;
		for (int i = 0; i < array.Length; i++)
		{
			Vector2 start = array[i];
			Vector2 end = array[(i + 1) % 4];
			Vector2 vector = ProjectPointOnLine(start, end, point);
			float magnitude = (point - vector).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				result = vector;
			}
		}
		return result;
	}

	public static Vector2 ProjectPointOnLine(Vector2 start, Vector2 end, Vector2 point)
	{
		Vector2 vector = end - start;
		Vector2 normalized = vector.normalized;
		float value = Vector2.Dot(point - start, normalized);
		value = Mathf.Clamp(value, 0f, vector.magnitude);
		return start + normalized * value;
	}

	public static float ProjectedPointLineDistance(Vector3 start, Vector3 end, Vector3 point)
	{
		return PointLineDistance(To2D(start), To2D(end), To2D(point));
	}

	public static bool GetProjectedLineSegmentIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
	{
		Vector2 p5 = To2D(p1);
		Vector2 p6 = To2D(p2);
		Vector2 p7 = To2D(p3);
		Vector2 p8 = To2D(p4);
		Vector2 intersection2;
		float u;
		bool result = LineSegmentsIntersection(p5, p6, p7, p8, out intersection2, out u);
		intersection = From2D(intersection2);
		return result;
	}

	public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection, out float u)
	{
		intersection = Vector2.zero;
		float num = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
		u = 0f;
		if (num == 0f)
		{
			return false;
		}
		u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / num;
		float num2 = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / num;
		if (u < 0f || u > 1f || num2 < 0f || num2 > 1f)
		{
			return false;
		}
		intersection.x = p1.x + u * (p2.x - p1.x);
		intersection.y = p1.y + u * (p2.y - p1.y);
		return true;
	}

	public static float Angle(Vector2 pos1, Vector2 pos2)
	{
		Vector2 vector = pos2 - pos1;
		Vector2 vector2 = new Vector2(1f, 0f);
		float num = Vector2.Angle(vector, vector2);
		if (Vector3.Cross(vector, vector2).z > 0f)
		{
			num = 360f - num;
		}
		return num;
	}

	public static float SqrDistance(Vector3 a, Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3;
	}

	public static float EvaluateFunctionWithControlPoints(float x1, float y1, float x2, float y2, float t)
	{
		return CubicBezier(Vector2.zero, new Vector2(x1, y1), new Vector2(x2, y2), Vector2.one, t).y;
	}

	public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		return (1f - t) * QuadBezier(p0, p1, p2, t) + t * QuadBezier(p1, p2, p3, t);
	}

	public static Vector2 QuadBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
	{
		t = Mathf.Clamp01(t);
		return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
	}
}
