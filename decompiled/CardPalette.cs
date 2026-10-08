using System;
using UnityEngine;

[Serializable]
public class CardPalette
{
	public Color Color;

	public Color Color2;

	public Color Icon;

	public CardPalette(Color color, Color color2, Color icon)
	{
		Color = color;
		Color2 = color2;
		Icon = icon;
	}
}
