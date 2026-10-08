using System.Collections.Generic;

public class ModCardBag
{
	public string CardBagType;

	public int CardsInPack = 3;

	public List<CardChance> Chances;

	public List<string> SetPackCards;

	public string SetCardBag;

	public bool UseFallbackBag;

	public string FallbackBag;

	public string EnemyCardBag;

	public float StrengthLevel;
}
