using System.Collections.Generic;

public class BlueprintDeathCurseOver : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		base.BlueprintComplete(rootCard, involvedCards, print);
		if (WorldManager.instance.CurrentBoard.Id == "death")
		{
			WorldManager.instance.CurrentRunVariables.CompletedDeathSpirit = true;
			WorldManager.instance.CurrentSave.FinishedDeath = true;
			WorldManager.instance.QueueCutscene(Cutscenes.SpiritOutro(CurseType.Death));
		}
	}
}
