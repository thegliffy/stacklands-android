using UnityEngine;

public class GridQuad : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	private void Update()
	{
		Vector3 position = GameCamera.instance.ScreenPosToWorldPos(new Vector3(Screen.width, Screen.height) * 0.5f);
		position.y = 0.2f;
		base.transform.position = position;
		meshRenderer.enabled = WorldManager.instance.gridAlpha >= 0.001f;
	}
}
