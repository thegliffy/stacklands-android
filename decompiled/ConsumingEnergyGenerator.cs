using System.Collections.Generic;
using System.Linq;

public class ConsumingEnergyGenerator : EnergyGenerator
{
	public List<CardAmountPair> CardsToConsume = new List<CardAmountPair>();

	public int PollutionAmount;

	public float CycleTime = 15f;

	[ExtraData("has_energy")]
	public bool hasEnergy;

	private bool prevHasEnergy;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (CardsToConsume.Select((CardAmountPair x) => x.CardId).Contains(otherCard.Id))
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && (!MyGameCard.TimerRunning || MyGameCard.TimerActionId == GetActionId("StopEnergy")) && CardsToConsume.All((CardAmountPair pair) => CardsInStackMatchingPredicate((CardData x) => x.Id == pair.CardId).Count >= pair.Amount))
		{
			if (!IsDamaged)
			{
				MyGameCard.StartTimer(CycleTime, GenerateEnergy, SokLoc.Translate("card_energy_status_0"), GetActionId("GenerateEnergy"));
			}
			if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("GenerateEnergy"))
			{
				MyGameCard.CancelTimer(GetActionId("StopEnergy"));
				AudioManager.me.PlaySound2D(AudioManager.me.CardDestroy, 1f, 0.4f);
				foreach (CardAmountPair pair2 in CardsToConsume)
				{
					DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.Id == pair2.CardId, pair2.Amount);
				}
			}
		}
		if (MyGameCard.TimerRunning)
		{
			hasEnergy = true;
		}
		if (!MyGameCard.TimerRunning && hasEnergy)
		{
			MyGameCard.StartTimer(5f, StopEnergy, SokLoc.Translate("card_energy_status_0"), GetActionId("StopEnergy"), withStatusBar: false, skipWorkerEnergyCheck: true, skipDamageOnOffCheck: true);
		}
		if (hasEnergy != prevHasEnergy)
		{
			NotifyEnergyConsumers();
		}
		prevHasEnergy = hasEnergy;
		base.UpdateCard();
	}

	public override bool HasEnergyOutput(CardConnector connectedNode, List<CardConnector> nodeTracker)
	{
		return hasEnergy;
	}

	[TimedAction("generate_energy")]
	public void GenerateEnergy()
	{
		if (PollutionAmount > 0)
		{
			WorldManager.instance.CreateCardStack(base.Position, PollutionAmount, "pollution", checkAddToStack: false);
		}
	}

	[TimedAction("stop_energy")]
	public void StopEnergy()
	{
		hasEnergy = false;
		AudioManager.me.PlaySound2D(AudioManager.me.PowerOutage, 1f, 0.4f);
	}
}
