using System.Collections.Generic;

public class ModManifest
{
	public string Id;

	public string Name;

	public string Version = "1.0.0";

	internal string Folder;

	public List<string> Dependencies = new List<string>();

	public List<string> OptionalDependencies = new List<string>();

	public string Assembly;
}
