using System.Collections.Generic;
using UnityEngine;

public class ProfanityChecker
{
	private class LanguageProfanities
	{
		public string Language;

		public List<string> List = new List<string>();

		public HashSet<string> Set = new HashSet<string>();

		public LanguageProfanities(string language)
		{
			Language = language;
		}

		public void AddWord(string s)
		{
			if (!string.IsNullOrWhiteSpace(s))
			{
				s = s.ToLower();
				List.Add(s);
				Set.Add(s);
			}
		}
	}

	private SokSheet profanitySheet;

	private Dictionary<string, LanguageProfanities> languageProfanities = new Dictionary<string, LanguageProfanities>();

	public ProfanityChecker()
	{
		profanitySheet = Resources.Load<SokSheet>("Sheets/ProfanitySheet");
		Load();
	}

	public bool IsProfanityInLanguage(string language, string word)
	{
		if (string.IsNullOrWhiteSpace(word))
		{
			return false;
		}
		if (!languageProfanities.ContainsKey(language))
		{
			return false;
		}
		return languageProfanities[language].Set.Contains(word);
	}

	public bool ContainsProfanityInLanguage(string language, string word)
	{
		if (!this.languageProfanities.ContainsKey(language))
		{
			return false;
		}
		LanguageProfanities languageProfanities = this.languageProfanities[language];
		word = word.ToLower();
		for (int i = 0; i < languageProfanities.List.Count; i++)
		{
			if (word.Contains(languageProfanities.List[i]))
			{
				return true;
			}
		}
		return false;
	}

	private void Load()
	{
		for (int i = 0; i < profanitySheet.Table[0].GetLength(0); i++)
		{
			string text = profanitySheet.Table[0][i];
			LanguageProfanities languageProfanities = new LanguageProfanities(text);
			for (int j = 1; j < profanitySheet.Table.GetLength(0); j++)
			{
				string[] array = profanitySheet.Table[j][i].Split(',');
				foreach (string s in array)
				{
					languageProfanities.AddWord(s);
				}
			}
			this.languageProfanities.Add(text, languageProfanities);
		}
	}
}
