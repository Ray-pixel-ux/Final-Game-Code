using Godot;

public partial class MainMenu : Control
{
	[Export] private AudioStream titleMusic; // drag your music file here
	
	private AudioStreamPlayer music;
	
	public override void _Ready()
	{
		// Initialize fullscreen
		InitializeFullscreen();
		
		// 2. music (optional – no error if you leave export empty)
		if (titleMusic != null)
		{
			music = new AudioStreamPlayer { Stream = titleMusic, Autoplay = true };
			AddChild(music);
		}
		
		GetNode<Button>("ButtonContainer/StartGameBtn").Pressed += OnStartGame;
		GetNode<Button>("ButtonContainer/LoadGameBtn").Pressed += OnLoadGame;
		GetNode<Button>("ButtonContainer/OptionsBtn").Pressed += OnOptions;
		GetNode<Button>("ButtonContainer/QuitBtn").Pressed   += OnQuit;   // ← new
		//  hook other buttons if you want
	}
	
	private void InitializeFullscreen()
	{
		// Simple solution: Just force fullscreen
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		
		// Or if you want windowed for testing:
		// DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
	}
	
	private void OnStartGame()
	{
		SaveHelper.SaveLastLevel("TutorialStage");   // still save first level
		GetTree().ChangeSceneToFile("res://scenes/StoryScene.tscn"); // <-- new
	}
	
	private void OnLoadGame()
	{
		string last = SaveHelper.LoadLastLevel();
		GD.Print("LoadGame pressed -> ", last);
		string path = last switch
		{
			"Level2" => "res://scenes/Level2.tscn",
			"Level3" => "res://scenes/Level3.tscn",
			"Level4" => "res://scenes/Level4.tscn",
			"Level5" => "res://scenes/Level5.tscn",
			"Level6" => "res://scenes/Level6.tscn",
			"Level7" => "res://scenes/Level7.tscn",
			"Level8" => "res://scenes/Level8.tscn",
			"Level9" => "res://scenes/Level9.tscn",
			"Level10" => "res://scenes/Level10.tscn",
			"Level11" => "res://scenes/Level11.tscn",
			"Level12" => "res://scenes/Level12.tscn",
			"TutorialStage" => "res://scenes/TutorialStage.tscn", // Add this line
			_ => "res://scenes/TutorialStage.tscn" // Default case handles empty string, null, or other values
		};
		GetTree().ChangeSceneToFile(path);
	}
	
	private void OnOptions()
	{
		var pop = ResourceLoader.Load<PackedScene>("res://ui/OptionsPopup.tscn").Instantiate<OptionsPopup>();
		AddChild(pop);
	}
	
	private void OnQuit()
	{
		GetTree().Quit();
	}
}
