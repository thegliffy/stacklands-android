using UnityEngine;

public static class PerformanceHelper
{
	public static void SetActive(GameObject obj, bool active)
	{
		if (active && !obj.activeSelf)
		{
			obj.SetActive(active);
		}
		else if (!active && obj.activeSelf)
		{
			obj.SetActive(active);
		}
	}

	public static void SetActiveFast(this GameObject obj, bool active)
	{
		if (active && !obj.activeSelf)
		{
			obj.SetActive(active);
		}
		else if (!active && obj.activeSelf)
		{
			obj.SetActive(active);
		}
	}
}
