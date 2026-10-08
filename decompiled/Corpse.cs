public class Corpse : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Corpse;
	}

	public override void UpdateCardText()
	{
		string text = SokLoc.Translate(NameTerm);
		if (!string.IsNullOrEmpty(CustomName))
		{
			text = SokLoc.Translate("card_corpse_name_long", LocParam.Create("name", CustomName));
		}
		nameOverride = text;
	}
}
