using System.Collections.Generic;

public class CardStringSplitter
{
	private static CardStringSplitter _instance;

	private Dictionary<string, string[]> stringToSplit = new Dictionary<string, string[]>();

	public static CardStringSplitter me
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CardStringSplitter();
			}
			return _instance;
		}
	}

	public string[] Split(string s)
	{
		if (stringToSplit.TryGetValue(s, out var value))
		{
			return value;
		}
		string[] array = s.Split('|');
		stringToSplit[s] = array;
		return array;
	}
}
