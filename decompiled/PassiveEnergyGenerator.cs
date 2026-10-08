using System.Collections.Generic;

public class PassiveEnergyGenerator : EnergyGenerator
{
	public int EnergyGeneratedPerCycle = 1;

	public float CycleTime = 30f;

	[ExtraData("has_energy")]
	public bool hasEnergy;

	private bool prevHasEnergy;

	public override void UpdateCard()
	{
		if (!MyGameCard.TimerRunning && !IsDamaged)
		{
			MyGameCard.StartTimer(CycleTime, EndCycle, SokLoc.Translate("card_energy_status_0"), GetActionId("EndCycle"));
		}
		base.UpdateCard();
	}

	public override bool HasEnergyOutput(CardConnector connectedNode, List<CardConnector> nodeTracker)
	{
		if (WorkerAmountMet())
		{
			hasEnergy = true;
		}
		else
		{
			hasEnergy = false;
		}
		if (hasEnergy != prevHasEnergy)
		{
			NotifyEnergyConsumers();
		}
		prevHasEnergy = hasEnergy;
		return hasEnergy;
	}

	[TimedAction("end_cycle")]
	public void EndCycle()
	{
	}
}
