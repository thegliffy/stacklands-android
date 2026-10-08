public class HappinessSpirit : Spirit
{
	public override void UpdateCard()
	{
		base.UpdateCard();
	}

	public override void Clicked()
	{
		if (WorldManager.instance.CurrentRunVariables.CompletedHappinessSpirit)
		{
			WorldManager.instance.QueueCutscene(Cutscenes.ReturnToBoardCutscene());
		}
		base.Clicked();
	}
}
