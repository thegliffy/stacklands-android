using System;
using System.Collections.Generic;

[Serializable]
public class SavedPreset
{
	public string SaveId;

	public string FullPath;

	public List<SavedCard> SavedCards;
}
