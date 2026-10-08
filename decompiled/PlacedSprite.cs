using System;
using UnityEngine;

[Serializable]
public class PlacedSprite
{
	public Vector2 Position;

	public Vector2 Size;

	public Sprite Sprite;

	public bool IsVisible;

	public Transform Transform;

	public float Left => Position.x - Size.x * 0.5f;

	public float Right => Position.x + Size.x * 0.5f;

	public float Top => Position.y + Size.y * 0.5f;

	public float Bottom => Position.y - Size.y * 0.5f;
}
