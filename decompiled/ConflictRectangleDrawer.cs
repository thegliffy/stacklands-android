using System;
using UnityEngine;

public class ConflictRectangleDrawer : ShapeDrawer
{
	public MeshRenderer Renderer;

	private MaterialPropertyBlock propBlock;

	public ConflictRectangle Rectangle => (ConflictRectangle)(object)base.MyShape;

	public override Type DrawingType => typeof(ConflictRectangle);

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		base.transform.position = Rectangle.Center;
		Renderer.transform.localScale = new Vector3(Rectangle.Size.x, Rectangle.Size.y, 1f) + Vector3.one;
		Renderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Size", new Vector4(Rectangle.Size.x, Rectangle.Size.y));
		Renderer.SetPropertyBlock(propBlock);
	}
}
