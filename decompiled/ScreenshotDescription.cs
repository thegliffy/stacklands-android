using System;

public class ScreenshotDescription
{
	public int Width;

	public int Height;

	public bool ShowUI = true;

	public ControlScheme? ControlSchemeOverride;

	public string Language;

	public string Description;

	public DateTime TakenAt;

	public bool IncludeInScreenshots = true;

	public bool AlphaBackground;

	public ScreenshotDescription(int w, int h)
	{
		Width = w;
		Height = h;
	}
}
