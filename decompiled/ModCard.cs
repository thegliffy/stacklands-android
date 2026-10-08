using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ModCard
{
	public string Id;

	public string DescriptionTerm;

	public string DescriptionOverride;

	public string NameTerm;

	public string NameOverride;

	public string Type = "Structures";

	public string Icon;

	public string PickupSound;

	public int Value;

	public bool HideFromCardopedia;

	public string Script = "CardData";

	[JsonExtensionData]
	public IDictionary<string, JToken> ExtraProps;
}
