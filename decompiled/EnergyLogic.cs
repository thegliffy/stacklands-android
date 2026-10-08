using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyLogic : CardData
{
	[ExtraData("has_energy")]
	[HideInInspector]
	public bool HasEnergy;

	private bool prevHasEnergy;

	public override bool HasEnergyOutput(CardConnector connectedNode, List<CardConnector> nodeTracker)
	{
		if (nodeTracker.Contains(connectedNode))
		{
			return false;
		}
		nodeTracker.Add(connectedNode);
		if (MyGameCard.CardConnectorChildren.Where((CardConnector x) => x.CardDirection == CardDirection.input).Count() <= 0)
		{
			HasEnergy = false;
		}
		if (MyGameCard.CardConnectorChildren.Where((CardConnector x) => x.CardDirection == CardDirection.input).All((CardConnector x) => x.ConnectedNode != null && x.ConnectedNode.Parent.CardData.HasEnergyOutput(x.ConnectedNode, nodeTracker)))
		{
			HasEnergy = true;
		}
		else
		{
			HasEnergy = false;
		}
		if (HasEnergy != prevHasEnergy)
		{
			NotifyEnergyConsumers();
		}
		prevHasEnergy = HasEnergy;
		return HasEnergy;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}
}
