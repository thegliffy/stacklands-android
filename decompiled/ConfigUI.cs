using System;
using System.Collections.Generic;

public class ConfigUI
{
	public string Name;

	public string NameTerm;

	public string Tooltip;

	public string TooltipTerm;

	public string PlaceholderText;

	public bool RestartAfterChange;

	public Action<ConfigEntryBase> OnUI;

	public bool Hidden;

	public Dictionary<string, string> ExtraData = new Dictionary<string, string>();

	public string GetName()
	{
		if (string.IsNullOrEmpty(NameTerm))
		{
			return Name;
		}
		return SokLoc.Translate(NameTerm);
	}

	public string GetTooltip()
	{
		if (string.IsNullOrEmpty(TooltipTerm))
		{
			return Tooltip;
		}
		return SokLoc.Translate(TooltipTerm);
	}
}
