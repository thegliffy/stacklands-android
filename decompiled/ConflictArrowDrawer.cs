using System;
using Shapes;
using UnityEngine;

public class ConflictArrowDrawer : ShapeDrawer
{
	public Transform VeryEffectiveText;

	public Rectangle VeryEffectiveRect;

	public Color OutlineColor = Color.black;

	public float Length = 0.1f;

	public float Thickness = 0.1f;

	public float OutlineThickness = 0.05f;

	public Renderer ArrowRenderer;

	private MaterialPropertyBlock propBlock;

	public ConflictArrow Arrow => (ConflictArrow)(object)base.MyShape;

	public override Type DrawingType => typeof(ConflictArrow);

	private void Awake()
	{
		propBlock = new MaterialPropertyBlock();
	}

	public override void UpdateShape()
	{
		ArrowRenderer.GetPropertyBlock(propBlock);
		propBlock.SetVector("_Start", new Vector4(Arrow.Start.x, Arrow.Start.z));
		propBlock.SetVector("_End", new Vector4(Arrow.End.x, Arrow.End.z));
		propBlock.SetColor("_Color", Arrow.Color);
		propBlock.SetColor("_OutlineColor", OutlineColor);
		ArrowRenderer.SetPropertyBlock(propBlock);
		Vector3 position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f);
		Vector3 vector = new Vector3(Mathf.Abs(Arrow.End.x - Arrow.Start.x), 1f, Mathf.Abs(Arrow.End.z - Arrow.Start.z));
		base.transform.position = position;
		ArrowRenderer.transform.localScale = new Vector3(vector.x + 1f, vector.z + 1f, 1f);
		VeryEffectiveText.transform.position = Vector3.Lerp(Arrow.Start, Arrow.End, 0.5f) + Vector3.up * 0.03f;
		VeryEffectiveText.gameObject.SetActive(Arrow.VeryEffective);
		VeryEffectiveRect.gameObject.SetActive(Arrow.VeryEffective);
		VeryEffectiveText.transform.rotation = Camera.main.transform.rotation;
	}
}
