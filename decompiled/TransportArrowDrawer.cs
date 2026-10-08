using System;
using UnityEngine;

public class TransportArrowDrawer : ShapeDrawer
{
	public Renderer Renderer;

	private MaterialPropertyBlock propBlock;

	public Material FrontMaterial;

	public Material BehindMaterial;

	private int start = Shader.PropertyToID("_Start");

	private int end = Shader.PropertyToID("_End");

	private int middle = Shader.PropertyToID("_Middle");

	public override Type DrawingType => typeof(TransportArrow);

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		TransportArrow transportArrow = (TransportArrow)(object)base.MyShape;
		Renderer.sharedMaterial = ((WorldManager.instance.CurrentView == ViewType.Transport) ? FrontMaterial : BehindMaterial);
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector(start, new Vector4(transportArrow.Start.x, transportArrow.Start.z));
		propBlock.SetVector(end, new Vector4(transportArrow.End.x, transportArrow.End.z));
		propBlock.SetVector(middle, new Vector4(transportArrow.Middle.x, transportArrow.Middle.z));
		Renderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(transportArrow.Start, transportArrow.End, 0.5f);
		position.y = Mathf.Min(transportArrow.Start.y, transportArrow.End.y);
		if (WorldManager.instance.CurrentView != ViewType.Transport)
		{
			position.y = 0f;
		}
		Vector3 vector = new Vector3(Mathf.Abs(transportArrow.End.x - transportArrow.Start.x), 1f, Mathf.Abs(transportArrow.End.z - transportArrow.Start.z));
		base.transform.position = position;
		Renderer.transform.localScale = new Vector3(vector.x + 1.5f, vector.z + 1.5f, 1f);
	}
}
