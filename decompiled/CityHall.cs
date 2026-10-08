using System.Linq;
using UnityEngine;

public class CityHall : Landmark
{
	[HideInInspector]
	[ExtraData("dollar_amount")]
	public int DollarAmount = 100;

	public static int DollarPerCardcap = 5;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Dollar) && !(otherCard is Worker))
		{
			return otherCard is CitiesCombatable;
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && HasEnergyInput())
		{
			if (AllChildrenMatchPredicate((CardData x) => x is Dollar))
			{
				int num = (from x in MyGameCard.GetChildCards()
					select x.CardData into x
					where x is Dollar
					select x).Cast<Dollar>().Sum((Dollar x) => x.DollarValue);
				DollarAmount += num;
				DestroyChildrenMatchingPredicateAndRestack((CardData x) => x is Dollar, ChildrenMatchingPredicateCount((CardData x) => x is Dollar));
				QuestManager.instance.SpecialActionComplete("card_cap_increased");
				if (MyGameCard.HasChild)
				{
					GameCard child = MyGameCard.Child;
					child.RemoveFromParent();
					child.SendIt();
				}
			}
			else if (MyGameCard.GetChildCount() == 1 && (MyGameCard.Child.CardData is Worker || MyGameCard.Child.CardData is CitiesCombatable))
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
							bs.MyGameCard.RemoveFromStack();
							bs.MyGameCard.SendIt();
						});
					}
					else
					{
						bs.MyGameCard.RemoveFromStack();
					}
				}
			}
		}
		if (DollarAmount > 0)
		{
			descriptionOverride = SokLoc.Translate("card_city_hall_description_long", LocParam.Create("amount", DollarAmount.ToString()));
		}
		base.UpdateCard();
	}
}
