public class Dollar : Resource, ICurrency
{
	public int DollarValue;

	public CardData Card => this;

	public int CurrencyValue
	{
		get
		{
			return DollarValue;
		}
		set
		{
			DollarValue = value;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Dollar) && !(otherCard is Worker))
		{
			return otherCard is Resource;
		}
		return true;
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		nameOverride = SokLoc.Translate(NameTerm, LocParam.Create("icon", Icons.Dollar));
		descriptionOverride = SokLoc.Translate(DescriptionTerm, LocParam.Create("icon", Icons.Dollar));
	}

	public void UseCurrency(int currencyAmount, bool spawnSmoke = false)
	{
		if (spawnSmoke)
		{
			WorldManager.instance.CreateSmoke(base.Position);
		}
		MyGameCard.DestroyCard();
	}
}
