using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ValidationCategory
{
	public static string CardTerms = "Card Terms";

	public static string Blueprints = "Blueprints";

	public static string BlueprintDuplicates = "Blueprint Duplicates";

	public static string SetCardBag = "Set CardBags";

	public static string CardReferences = "Card References";

	public static string CardBag = "Card Bags";

	public static string BoosterPacks = "Booster Packs";

	public static string Quests = "Quests";

	public static string ExpectedValues = "Expected Values";

	public static string CardClasses = "Card Classes";

	public static string CardAudio = "Card Audio";

	public static List<string> GetCategories()
	{
		return (from f in typeof(ValidationCategory).GetFields(BindingFlags.Static | BindingFlags.Public)
			where f.FieldType == typeof(string)
			select (string)f.GetValue(null)).ToList();
	}
}
