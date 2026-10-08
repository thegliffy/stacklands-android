using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_PanCamera : CutsceneStep
{
	public float Duration = 5f;

	public float CameraDistance = 12f;

	public override IEnumerator Process()
	{
		GameCamera.instance.TargetCardOverride = null;
		float y = GameCamera.instance.MaxZoom + WorldManager.instance.CurrentBoard.WorldSizeIncrease;
		Bounds worldBounds = WorldManager.instance.CurrentBoard.WorldBounds;
		Vector3 left = new Vector3(worldBounds.center.x - worldBounds.extents.x * 0.8f, y, worldBounds.center.z);
		Vector3 right = new Vector3(worldBounds.center.x + worldBounds.extents.x * 0.8f, y, worldBounds.center.z);
		GameCamera.instance.TargetPositionOverride = left;
		GameCamera.instance.CameraPositionDistanceOverride = CameraDistance;
		yield return new WaitForSeconds(1f);
		float timer = 0f;
		while (timer <= Duration)
		{
			timer += Time.deltaTime;
			GameCamera.instance.TargetPositionOverride = Vector3.Lerp(left, right, timer / Duration);
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		GameCamera.instance.TargetPositionOverride = null;
		GameCamera.instance.CameraPositionDistanceOverride = null;
	}
}
