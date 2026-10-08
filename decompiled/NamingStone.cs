public class NamingStone : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is BaseVillager) && !(otherCard is Animal))
		{
			return otherCard is Kid;
		}
		return true;
	}

	public override void UpdateCard()
	{
		CardData card = null;
		CardData card2 = null;
		if ((HasCardOnTop(out card) || IsOnCard<CardData>(out card2)) && !GameCanvas.instance.ModalIsOpen)
		{
			CardData bs = ((card != null) ? card : card2);
			if (CanHaveCard(bs))
			{
				GameCanvas.instance.ShowNameCombatableModal(bs, delegate
				{
					if (bs is BaseVillager)
					{
						QuestManager.instance.SpecialActionComplete("name_villager");
					}
					bs.MyGameCard.RemoveFromStack();
					bs.MyGameCard.SendIt();
				});
			}
			else
			{
				bs.MyGameCard.RemoveFromStack();
			}
		}
		base.UpdateCard();
	}
}
