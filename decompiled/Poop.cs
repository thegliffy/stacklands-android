using UnityEngine;

public class Poop : CardData
{
	public float SickChance = 20f;

	public AudioClip PoopSound;

	public bool CanMakeSick
	{
		get
		{
			bool result = true;
			if (MyGameCard.Parent != null && MyGameCard.Parent.CardData is Cesspool)
			{
				result = false;
			}
			if (MyGameCard.CardData.CreationMonth == WorldManager.instance.CurrentMonth)
			{
				result = false;
			}
			return result;
		}
	}

	public override void OnInitialCreate()
	{
		CardData nearestCardMatchingPred = WorldManager.instance.GetNearestCardMatchingPred(MyGameCard, (GameCard x) => x.CardData.Id == "sewer");
		if (nearestCardMatchingPred != null)
		{
			WorldManager.instance.StackSendTo(MyGameCard, nearestCardMatchingPred.MyGameCard);
		}
		base.OnInitialCreate();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Humans && otherCard.MyCardType != CardType.Food && !(otherCard.Id == Id))
		{
			if (otherCard.MyCardType == CardType.Structures)
			{
				return !otherCard.IsBuilding;
			}
			return false;
		}
		return true;
	}

	public override void UpdateCardText()
	{
		if (WorldManager.instance.CurseIsActive(CurseType.Death))
		{
			descriptionOverride = SokLoc.Translate(DescriptionTerm) + "\n\n<i>" + SokLoc.Translate("card_poop_cant_sell") + "</i>";
		}
		else
		{
			descriptionOverride = null;
		}
		base.UpdateCardText();
	}

	public override void UpdateCard()
	{
		if (WorldManager.instance.CurseIsActive(CurseType.Death))
		{
			Value = -1;
		}
		else
		{
			Value = 1;
		}
		base.UpdateCard();
	}
}
