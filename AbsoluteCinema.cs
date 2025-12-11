using Godot;

public partial class AbsoluteCinema : Control
{
	public string LastLevel => NextSceneData.LastLevel;

	public override void _Ready()
	{
		GetNode<Button>("RestartBtn").Pressed += OnRestart;
		GetNode<Button>("ContinueBtn").Pressed += OnContinue;
	}

	private void OnRestart()
	{
		GameManager.Instance.Reset();
		string path = ScenePath(LastLevel);
		if (!string.IsNullOrEmpty(path))
		{
			GetTree().ChangeSceneToFile(path);
		}
		else
		{
			GD.PrintErr("Invalid scene path for LastLevel: ", LastLevel);
			GetTree().ChangeSceneToFile("res://scenes/TutorialStage.tscn"); // Fallback
		}
	}

	private void OnContinue()
	{
		GameManager.Instance.Reset();
		string next = NextLevelName();
		string path = ScenePath(next);
		if (!string.IsNullOrEmpty(path))
		{
			GetTree().ChangeSceneToFile(path);
		}
		else
		{
			GD.PrintErr("Invalid next level: ", next);
			GetTree().ChangeSceneToFile("res://scenes/TutorialStage.tscn"); // Fallback
		}
	}

	private string NextLevelName() => LastLevel switch
	{
		"TutorialStage" => "Level2",
		"Level2"        => "Level3",
		"Level3"        => "Level4",
		"Level4"        => "Level5",
		"Level5"        => "Level6",
		"Level6"        => "Level7",
		"Level7"        => "Level8",
		"Level8"        => "Level9",
		"Level9"        => "Level10",
		"Level10"       => "Level11",
		"Level11"       => "Level12",
		"Level12"       => "LastScene",
		_ => "" // Default case for unexpected values
	};

	private string ScenePath(string level) => level switch
	{
		"TutorialStage" => "res://scenes/TutorialStage.tscn",
		"Level2"        => "res://scenes/Level2.tscn",
		"Level3"        => "res://scenes/Level3.tscn",
		"Level4"        => "res://scenes/Level4.tscn",
		"Level5"        => "res://scenes/Level5.tscn",
		"Level6"        => "res://scenes/Level6.tscn",
		"Level7"        => "res://scenes/Level7.tscn",
		"Level8"        => "res://scenes/Level8.tscn",
		"Level9"        => "res://scenes/Level9.tscn",
		"Level10"       => "res://scenes/Level10.tscn",
		"Level11"       => "res://scenes/Level11.tscn",
		"Level12"       => "res://scenes/Level12.tscn",
		"LastScene"     => "res://scenes/LastStoryScene.tscn",
		_ => "" // Default case for unexpected values
	};
}
