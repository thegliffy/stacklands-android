using System;

public abstract class ConfigEntryBase
{
	public string Name;

	public Type ValueType;

	public ConfigFile Config;

	public ConfigUI UI = new ConfigUI();

	public abstract object BoxedValue { get; set; }
}
