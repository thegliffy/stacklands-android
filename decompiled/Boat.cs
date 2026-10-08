using System.Collections.Generic;
using UnityEngine;

public class Boat : CardData
{
	public int MaxCapacity = 5;

	public float TravelTime = 30f;

	public bool InSailOffPrompt;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public bool IsSailingOff
	{
		get
		{
			if (MyGameCard.TimerRunning)
			{
				return MyGameCard.TimerActionId == GetActionId("SailOff");
			}
			return false;
		}
	}

	public bool InSailOff => InSailOffPrompt;

	public override bool CanBeDragged
	{
		get
		{
			if (IsSailingOff)
			{
				return false;
			}
			return true;
		}
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (GetChildCount() + (otherCard.GetChildCount() + 1) > MaxCapacity)
		{
			return false;
		}
		if (CardIsAllowedOnBoat(otherCard))
		{
			return otherCard.AllChildrenMatchPredicate((CardData x) => CardIsAllowedOnBoat(x));
		}
		return false;
	}

	private bool CardIsAllowedOnBoat(CardData otherCard)
	{
		if (otherCard.Id == "trained_monkey")
		{
			return false;
		}
		if (otherCard.MyCardType != CardType.Food && otherCard.MyCardType != CardType.Humans)
		{
			return otherCard.MyCardType == CardType.Resources;
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			int num = ChildrenMatchingPredicateCount((CardData x) => x is BaseVillager);
			if (num > 0)
			{
				if (!WorldManager.instance.CurrentBoard.BoardOptions.CanTravelToIsland)
				{
					GameCanvas.instance.ShowCantChangeBoardSpirit();
					Stay();
					return;
				}
				RemoveTrainedMonkeys();
				int cardCount = WorldManager.instance.GetCardCount((CardData x) => x is BaseVillager);
				int requiredFoodCount = WorldManager.instance.GetRequiredFoodCount();
				if (WorldManager.instance.GetFoodCount() < requiredFoodCount)
				{
					MyGameCard.Child.RemoveFromParent();
					GameCanvas.instance.NotEnoughFoodToSailOffPrompt();
				}
				else if (WorldManager.instance.CurrentBoard.Id == "main" && num == cardCount)
				{
					MyGameCard.CancelTimer(GetActionId("SailOff"));
					GameCanvas.instance.OneVillagerNeedsToStayPrompt("label_sailing_off_title");
					RemoveLastVillagerOnBoat();
				}
				else
				{
					MyGameCard.StartTimer(TravelTime, SailOff, SokLoc.Translate("card_boat_status"), GetActionId("SailOff"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("SailOff"));
			}
		}
		base.UpdateCard();
	}

	private void RemoveTrainedMonkeys()
	{
		List<GameCard> allCardsInStack = MyGameCard.GetAllCardsInStack();
		for (int num = allCardsInStack.Count - 1; num >= 0; num--)
		{
			if (allCardsInStack[num].CardData.Id == "trained_monkey")
			{
				allCardsInStack.RemoveAt(num);
				break;
			}
		}
		WorldManager.instance.Restack(allCardsInStack);
	}

	private void RemoveLastVillagerOnBoat()
	{
		List<GameCard> allCardsInStack = MyGameCard.GetAllCardsInStack();
		for (int num = allCardsInStack.Count - 1; num >= 0; num--)
		{
			if (allCardsInStack[num].CardData is BaseVillager)
			{
				allCardsInStack.RemoveAt(num);
				break;
			}
		}
		WorldManager.instance.Restack(allCardsInStack);
	}

	[TimedAction("sail_off")]
	public void SailOff()
	{
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			GameCanvas.instance.ChangeLocationPrompt(GoAway, Stay, "island");
		}
	}

	private void Stay()
	{
		GameCard parent = MyGameCard.Parent;
		RestackChildrenMatchingPredicate((CardData c) => c is BaseVillager);
		if (parent != null && parent.CardData is HeavyFoundation)
		{
			MyGameCard.SetParent(parent);
		}
	}

	private void RemoveStacksFromAllBoats()
	{
		foreach (Boat card in WorldManager.instance.GetCards<Boat>())
		{
			if (!(card == this) && card.MyGameCard.HasChild)
			{
				card.MyGameCard.Child.RemoveFromParent();
			}
		}
	}

	private void GoAway()
	{
		EndOfMonthParameters endOfMonthParameters = new EndOfMonthParameters();
		endOfMonthParameters.SkipSpecialEvents = true;
		endOfMonthParameters.CutsceneTitle = SokLoc.Translate("label_sailing_off_full");
		endOfMonthParameters.SkipEndConfirmation = true;
		InSailOffPrompt = true;
		RemoveStacksFromAllBoats();
		endOfMonthParameters.OnDone = delegate
		{
			InSailOffPrompt = false;
			GameCanvas.instance.SetScreen<CutsceneScreen>();
			string id = ((!(WorldManager.instance.CurrentBoard.Id == "main")) ? "main" : "island");
			GameBoard targetBoard = WorldManager.instance.GetBoardWithId(id);
			WorldManager.instance.GoToBoard(targetBoard, delegate
			{
				GameCanvas.instance.SetScreen<GameScreen>();
				WorldManager.instance.SendStackToBoard(MyGameCard, targetBoard, new Vector2(0.4f, 0.5f));
				RestackChildrenMatchingPredicate((CardData v) => v is BaseVillager);
			});
		};
		WorldManager.instance.ForceEndOfMoon(endOfMonthParameters);
	}
}
