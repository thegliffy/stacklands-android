using System.Collections.Generic;

public class ModBlueprint
{
	public string Id;

	public string NameTerm;

	public string NameOverride;

	public string Group;

	public string Icon;

	public int Value = 1;

	public bool HideFromCardopedia;

	public bool HideFromIdeasTab;

	public bool IsInvention;

	public bool NeedsExactMatch = true;

	public List<ModSubprint> Subprints;

	public string Script = "Blueprint";
}
