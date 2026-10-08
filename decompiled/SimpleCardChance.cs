using System;

[Serializable]
public class SimpleCardChance
{
	[Card]
	public string CardId;

	public int Chance = 1;

	public SimpleCardChance(string cardId, int chance)
	{
		CardId = cardId;
		Chance = chance;
	}

	public SimpleCardChance()
	{
	}
}
