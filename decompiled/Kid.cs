public class Kid : CardData
{
	public override void UpdateCardText()
	{
		string text = SokLoc.Translate(NameTerm);
		if (!string.IsNullOrEmpty(CustomName))
		{
			text = text + " " + CustomName;
		}
		nameOverride = text;
	}
}
