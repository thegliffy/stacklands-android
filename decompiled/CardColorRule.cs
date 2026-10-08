using System;

public class CardColorRule
{
	public Predicate<CardData> Predicate;

	public CardPalette Palette;

	public CardColorRule(CardPalette palette, Predicate<CardData> pred)
	{
		Palette = palette;
		Predicate = pred;
	}
}
