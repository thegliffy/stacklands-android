public class RadarStation : CardData
{
	public override void UpdateCard()
	{
		int currentMonth = WorldManager.instance.CurrentMonth;
		int nextConflictMonth = CitiesManager.instance.NextConflictMonth;
		if (currentMonth >= nextConflictMonth - 3 && currentMonth < nextConflictMonth)
		{
			descriptionOverride = SokLoc.Translate(DescriptionTerm) + ". " + SokLoc.Translate("statuseffect_radar_description", LocParam.Create("amount", (CitiesManager.instance.NextConflictMonth - 1).ToString()));
			AddStatusEffect(new StatusEffect_Radar());
		}
		else
		{
			RemoveStatusEffect<StatusEffect_Radar>();
		}
		base.UpdateCard();
	}
}
