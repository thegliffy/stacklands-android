using System;
using UnityEngine;

public abstract class ShapeDrawer : MonoBehaviour
{
	public IShape MyShape { get; set; }

	public abstract Type DrawingType { get; }

	public abstract void UpdateShape();
}
