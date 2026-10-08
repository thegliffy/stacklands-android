public class BlueprintAltar : Blueprint
{
	public override bool CanCurrentlyBeMade
	{
		get
		{
			if (!(WorldManager.instance.CurrentBoard.Id == "main"))
			{
				return WorldManager.instance.CurrentBoard.Id == "island";
			}
			return true;
		}
	}
}
