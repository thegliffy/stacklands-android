public class CombatCircleElement : Hoverable
{
	public GameCard ParentCard;

	public override string GetTitle()
	{
		if (ParentCard.CardData is Combatable combatable)
		{
			return combatable.GetCombatTypeTitle();
		}
		return "";
	}

	public override string GetDescription()
	{
		if (ParentCard.CardData is Combatable combatable)
		{
			return "<i>" + combatable.GetCombatTypeLore() + "</i>\n\n" + combatable.GetCombatTypeDescription();
		}
		return "";
	}
}
