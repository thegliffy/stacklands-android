using System;
using UnityEngine;

public class TransportArrowMainlandDrawer : ShapeDrawer
{
	public Renderer Renderer;

	private MaterialPropertyBlock propBlock;

	public Material FrontMaterial;

	public Material BehindMaterial;

	public TransportArrowMainland Cable => (TransportArrowMainland)(object)base.MyShape;

	public override Type DrawingType => typeof(TransportArrowMainland);

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		Renderer.sharedMaterial = ((WorldManager.instance.CurrentView == ViewType.Transport) ? FrontMaterial : BehindMaterial);
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Cable.Start.x, Cable.Start.z));
		propBlock.SetVector("_End", new Vector4(Cable.End.x, Cable.End.z));
		propBlock.SetVector("_Middle", new Vector4(Cable.Middle.x, Cable.Middle.z));
		Renderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Cable.Start, Cable.End, 0.5f);
		position.y = Mathf.Min(Cable.Start.y, Cable.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Transport)
		{
			position.y = 0f;
		}
		Vector3 vector = new Vector3(Mathf.Abs(Cable.End.x - Cable.Start.x), 1f, Mathf.Abs(Cable.End.z - Cable.Start.z));
		base.transform.position = position;
		Renderer.transform.localScale = new Vector3(vector.x + 1.5f, vector.z + 1.5f, 1f);
	}
}
