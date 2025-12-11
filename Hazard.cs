using Godot;

[GlobalClass]                       // appears in “Create Node” list
public partial class Hazard : Area2D
{
	[Export] private AudioStream hitSound;   // drag any thud / clang here

	private AudioStreamPlayer sfx;

	public override void _Ready()
{
	// optional audio – ignore if not present
	if (HasNode("AudioStreamPlayer"))
		sfx = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

	if (HasNode("AnimatedSprite2D"))
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");

	BodyEntered += OnBodyEntered;
}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player")) return;
		
		// shield blocks one hit
if (body.GetNode<PlayerPowerUpManager>("PowerUpManager").ConsumeShield())
{
	// optional break sound
	return;   // do NOT kill the player
}

		// play optional sound
if (hitSound != null && sfx != null)
{
	sfx.Stream = hitSound;
	sfx.Play();
}

		// insta-kill → Cooked picture
		var gm = GameManager.Instance;
		gm.Reset();          // coins = 0
		gm.LastLevel = GetTree().CurrentScene.Name;
		SaveHelper.SaveLastLevel(gm.LastLevel);

		Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn"))
				.CallDeferred();
	}
}
