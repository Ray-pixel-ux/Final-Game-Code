using Godot;
using System.IO;

public static class SaveHelper
{
	private static readonly string SavePath =
		ProjectSettings.GlobalizePath("user://savegame.dat");

	public static void SaveLastLevel(string levelName)
{
	File.WriteAllText(SavePath, levelName);
	GD.Print("SAVE: ", levelName);   // <-- add this
}

public static string LoadLastLevel()
{
	if (!File.Exists(SavePath)) return "TutorialStage";
	string level = File.ReadAllText(SavePath).Trim();
	GD.Print("LOAD: ", level);       // <-- add this
	return level;
}
}
