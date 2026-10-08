using System.Collections.Generic;

public class BlueprintGreedCurseOver : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		base.BlueprintComplete(rootCard, involvedCards, print);
		if (WorldManager.instance.CurrentBoard.Id == "greed")
		{
			WorldManager.instance.CurrentRunVariables.CompletedGreedSpirit = true;
			WorldManager.instance.CurrentSave.FinishedGreed = true;
			WorldManager.instance.QueueCutscene(Cutscenes.SpiritOutro(CurseType.Greed));
		}
	}
}
