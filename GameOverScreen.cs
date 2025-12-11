using Godot;

public partial class GameOverScreen : Control
{
	public string LastLevel => NextSceneData.LastLevel;

	[Export] public Texture2D GoatedImage;
	[Export] public Texture2D MehImage;
	[Export] public Texture2D CookedImage;

	private TextureRect _imageDisplay;
	private GameManager _gm;

	public override void _Ready()
	{
		GD.Print("GameOverScreen opened. LastLevel = ", NextSceneData.LastLevel);

		_imageDisplay = GetNode<TextureRect>("ImageDisplay");
		_gm = GameManager.Instance;

		// -------------  FIXED PICTURE RULES  -------------
		int total = _gm.GrandTotal();
		_imageDisplay.Texture =
			_gm.ForceCookedImage ? CookedImage :
			total <= 2           ? MehImage   :
			GoatedImage;

		// Wire buttons
		GetNode<Button>("PlayAgainBtn").Pressed += OnPlayAgain;
		GetNode<Button>("MainMenuBtn").Pressed += OnMainMenu;

		var continueBtn = GetNode<Button>("ContinueBtn");
		string next = NextLevelName();
		continueBtn.Visible = next != "";
		continueBtn.Text = "Continue";
		continueBtn.Pressed += () => OnContinue(next);
	}

	/* ---------- helpers ---------- */
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

	/* ---------- button callbacks ---------- */
	private void OnPlayAgain()
	{
		GameManager.Instance.ForceCookedImage = false;
		
		_gm.Reset();
		GetTree().ChangeSceneToFile(ScenePath(LastLevel));
	}

	private void OnMainMenu()
	{
		_gm.Reset();
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}

	private void OnContinue(string nextLevel)
	{
		GameManager.Instance.ForceCookedImage = false;
		
		_gm.Reset();
		GetTree().ChangeSceneToFile(ScenePath(nextLevel));
	}
}
