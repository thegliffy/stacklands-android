using System;

public class ConfigEntry<T> : ConfigEntryBase
{
	internal object _cachedValue;

	internal bool _cached;

	public Action<T> OnChanged;

	public T Value
	{
		get
		{
			return (T)(_cached ? _cachedValue : ((object)Config.GetValue<T>(Name)));
		}
		set
		{
			_cached = true;
			_cachedValue = value;
			Config.SetValue(Name, value);
			OnChanged?.Invoke(value);
		}
	}

	public override object BoxedValue
	{
		get
		{
			return Value;
		}
		set
		{
			Value = (T)value;
		}
	}

	public ConfigEntry(string name, ConfigFile config, object defaultValue = null, ConfigUI ui = null)
	{
		Name = name;
		Config = config;
		BoxedValue = Config.GetValue(Name, typeof(T)) ?? defaultValue;
		ValueType = typeof(T);
		_cached = true;
		if (ui != null)
		{
			UI = ui;
		}
		Config.Entries.Add(this);
	}
}
