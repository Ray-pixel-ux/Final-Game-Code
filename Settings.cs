using Godot;
using System;

public static class Settings
{
	private const string PATH = "user://settings.cfg";

	public static void SaveSoundVol(float vol) => Save("audio", "sound", vol);
	public static float LoadSoundVol() => LoadFloat("audio", "sound", 100f);

	private static void Save(string section, string key, Variant value)
	{
		var cf = new ConfigFile();
		cf.Load(PATH);
		cf.SetValue(section, key, value);
		cf.Save(PATH);
	}

	private static float LoadFloat(string section, string key, float @default)
	{
		var cf = new ConfigFile();
		return cf.Load(PATH) == Error.Ok ? (float)cf.GetValue(section, key, @default) : @default;
	}
}
