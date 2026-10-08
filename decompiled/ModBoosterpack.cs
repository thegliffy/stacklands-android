using System.Collections.Generic;

public class ModBoosterpack
{
	public string Id;

	public string NameTerm;

	public string NameOverride;

	public int MinQuestCount;

	public int Cost;

	public string Icon;

	public string Location = "Mainland";

	public List<ModCardBag> CardBags = new List<ModCardBag>();
}
