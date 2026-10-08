using System;
using UnityEngine;

public class ConveyorArrowDrawer : ShapeDrawer
{
	public Color OutlineColor = Color.black;

	public float Length = 0.1f;

	public float Thickness = 0.1f;

	public float OutlineThickness = 0.05f;

	public Renderer ArrowRenderer;

	private MaterialPropertyBlock propBlock;

	public ConveyorArrow Arrow => (ConveyorArrow)(object)base.MyShape;

	public override Type DrawingType => typeof(ConveyorArrow);

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		ArrowRenderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Arrow.Start.x, Arrow.Start.z));
		propBlock.SetVector("_End", new Vector4(Arrow.End.x, Arrow.End.z));
		propBlock.SetColor("_OutlineColor", OutlineColor);
		ArrowRenderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f);
		Vector3 vector = new Vector3(Mathf.Abs(Arrow.End.x - Arrow.Start.x), 1f, Mathf.Abs(Arrow.End.z - Arrow.Start.z));
		base.transform.position = position;
		ArrowRenderer.transform.localScale = new Vector3(vector.x + 1f, vector.z + 1f, 1f);
	}
}
