using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteArea
{
	public BackgroundRegion Region;

	public float Expansion;

	public float Padding;

	public bool OverrideAllowedSprites;

	public List<Sprite> AllowedSprites;
}
