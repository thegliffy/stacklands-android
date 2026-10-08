using System.Linq;
using UnityEngine;

public class TimeMachine : Landmark
{
	public AudioClip TimeMachineChargingSound;

	public AudioClip TimeMachineUseSound;

	public AudioClip TimeMachineDoneSound;

	[ExtraData("is_charged")]
	public bool IsCharged;

	[ExtraData("used_once")]
	public bool UsedOnce;

	public override void UpdateCard()
	{
		if (IsCharged)
		{
			if (MyGameCard.HasChild && (MyGameCard.Child.CardData is Worker || MyGameCard.Child.CardData is BaseVillager))
			{
				if (WorldManager.instance.GetCardCount<BaseVillager>() <= 1 && MyGameCard.Child.CardData is BaseVillager)
				{
					GameCanvas.instance.OneVillagerNeedsToStayPrompt("label_take_time_machine");
					MyGameCard.Child.RemoveFromParent();
				}
				if (!MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(20f, UseTimeMachine, SokLoc.Translate("card_time_machine_status_1"), GetActionId("UseTimeMachine"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("UseTimeMachine"));
			}
		}
		else if (HasEnergyInput())
		{
			MyGameCard.StartTimer(240f, ChargeTimeMachine, SokLoc.Translate("card_time_machine_status_2"), GetActionId("ChargeTimeMachine"));
			AudioManager.me.PlaySound(TimeMachineChargingSound, base.transform, 1f, 0.5f);
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("ChargeTimeMachine"));
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		base.UpdateCardText();
		if (IsCharged)
		{
			nameOverride = SokLoc.Translate("card_time_machine_name_real");
			descriptionOverride = SokLoc.Translate("card_time_machine_desctiption_real");
		}
	}

	[TimedAction("start_time_machine")]
	public void ChargeTimeMachine()
	{
		AudioManager.me.PlaySound(TimeMachineDoneSound, base.transform, 1f, 0.5f);
		WorldManager.instance.QueueCutscene("cities_time_machine");
		IsCharged = true;
	}

	[TimedAction("use_time_machine")]
	public void UseTimeMachine()
	{
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			string destinationBoard = "main";
			if (WorldManager.instance.CurrentBoard.Id == "main")
			{
				destinationBoard = "cities";
			}
			GameCanvas.instance.ChangeLocationPrompt(GoAway, Stay, destinationBoard);
		}
	}

	public void Stay()
	{
		GameCard parent = MyGameCard.Parent;
		RestackChildrenMatchingPredicate((CardData c) => c is Worker || c is BaseVillager);
		if (parent != null && parent.CardData is HeavyFoundation)
		{
			MyGameCard.Parent = parent;
		}
	}

	private void GoAway()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameBoard targetBoard = WorldManager.instance.GetBoardWithId("main");
		if (WorldManager.instance.CurrentBoard.Id == "main")
		{
			targetBoard = WorldManager.instance.GetBoardWithId("cities");
		}
		AudioManager.me.PlaySound(TimeMachineUseSound, base.transform, 1f, 0.5f);
		WorldManager.instance.GoToBoard(targetBoard, delegate
		{
			GameCanvas.instance.SetScreen<GameScreen>();
			WorldManager.instance.SendToBoard(MyGameCard, targetBoard, new Vector2(0.4f, 0.5f));
			if (MyGameCard.GetChildCards().Any((GameCard x) => x.CardData is BaseVillager))
			{
				WorldManager.instance.QueueCutscene("cities_villager_timemachine");
			}
			UsedOnce = true;
			MyGameCard.RemoveFromStack();
		});
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!UsedOnce)
		{
			if (!(otherCard.Id == "genius"))
			{
				return otherCard.Id == "robot_genius";
			}
			return true;
		}
		if (otherCard is Worker || otherCard is BaseVillager)
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
