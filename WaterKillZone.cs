using Godot;

public partial class WaterKillZone : Area2D
{
	public override void _Ready()
		=> BodyEntered += OnEnter;   // hook signal once

	private void OnEnter(Node2D body)
{
	if (!body.IsInGroup("player")) return;
	
	// shield blocks one hit
if (body.GetNode<PlayerPowerUpManager>("PowerUpManager").ConsumeShield())
{
	// optional break sound
	return;   // do NOT kill the player
}

	var gm = GameManager.Instance;
	gm.Reset();
	gm.LastLevel = GetTree().CurrentScene.Name;
	SaveHelper.SaveLastLevel(gm.LastLevel);

	// defer the scene swap
	Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn"))
			.CallDeferred();
}
}
