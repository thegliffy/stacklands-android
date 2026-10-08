public class WaterTreatmentPlant : EnergyHarvestable
{
	protected override bool CanStartHarvesting()
	{
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = MyGameCard.CardConnectorChildren[i];
			if (cardConnector != null && cardConnector.ConnectionType == ConnectionType.Sewer && cardConnector.ConnectedNode == null)
			{
				return false;
			}
		}
		return base.CanStartHarvesting();
	}
}
