using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Museum : CardData
{
	[ExtraData("given_card_ids")]
	[HideInInspector]
	public string SavedGivenCardIds;

	private List<string> _givenCards;

	private List<string> givenCards
	{
		get
		{
			if (_givenCards == null)
			{
				_givenCards = SavedGivenCardIds.Split(',').ToList();
			}
			return _givenCards;
		}
	}

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public void GiveCard(CardData card)
	{
		if (!CardWasGiven(card))
		{
			givenCards.Add(card.Id);
			UpdateData();
		}
	}

	public bool CardWasGiven(CardData card)
	{
		return givenCards.Contains(card.Id);
	}

	private void UpdateData()
	{
		SavedGivenCardIds = string.Join(",", givenCards);
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
		if (HasCardOnTop(out Food _))
		{
			MyGameCard.StartTimer(5f, ResearchedItem, SokLoc.Translate("card_tavern_status_0"), GetActionId("ResearchedItem"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("ResearchedItem"));
		}
	}

	[TimedAction("research_food")]
	public void ResearchedItem()
	{
		if (HasCardOnTop(out CardData card) && !CardWasGiven(card))
		{
			RemoveFirstChildFromStack();
			card.MyGameCard.DestroyCard();
			WorldManager.instance.TryCreateHappiness(base.transform.position, 2);
			GiveCard(card);
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Resource && !CardWasGiven(otherCard))
		{
			return true;
		}
		return false;
	}
}
