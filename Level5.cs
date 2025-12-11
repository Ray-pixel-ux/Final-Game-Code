using Godot;

public partial class Level5 : Node2D
{
	[Export] private AudioStream titleMusic; // drag your music file here
	
	private AudioStreamPlayer music;
	
	public override void _Ready()
	{
		// 2. music (optional – no error if you leave export empty)
	if (titleMusic != null)
	{
		music = new AudioStreamPlayer { Stream = titleMusic, Autoplay = true };
		AddChild(music);
	}
		
		GameManager.Instance.LoadPersistedLastLevel(); // <-- ADD
		GameManager.Instance.SetCurrentLevel("Level5"); // save immediately
		//  REMOVE any SaveHelper call here
		// hook up the water kill-zone
		GetNode<Area2D>("Background/WaterKillZone").BodyEntered += OnBodyEnteredWater;
	}
	private void OnBodyEnteredWater(Node2D body)
{
	if (!body.IsInGroup("player")) return;

	var gm = GameManager.Instance;
	gm.Reset();
	gm.LastLevel = "Level5";
	SaveHelper.SaveLastLevel("Level5");

	SceneTree tree = GetTree();                       // grab while we still exist
	Callable.From(() =>
		tree.ChangeSceneToFile("res://scenes/GameOverScreen.tscn"))
	   .CallDeferred();
}
}
