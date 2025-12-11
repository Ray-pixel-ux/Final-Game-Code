using Godot;

public partial class BossFollower1 : CharacterBody2D
{
	[Export] private float moveSpeed   = 60.0f;
	[Export] private float detection   = 250.0f;

	private AnimatedSprite2D sprite;
	private Area2D hitArea;
	private Player player;
	private Vector2 startPos;

	public override void _Ready()
	{
		sprite  = GetNode<AnimatedSprite2D>("Sprite");
		hitArea = GetNode<Area2D>("HitArea");
		hitArea.BodyEntered += OnBodyEntered;

		player   = GetTree().GetFirstNodeInGroup("player") as Player;
		startPos = GlobalPosition;
		sprite.Play("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) return;

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (dist < detection)
{
	Vector2 dir = (player.GlobalPosition - GlobalPosition).Normalized();
	Velocity = dir * moveSpeed;
	MoveAndSlide();

	// choose animation by direction, then flip if necessary
	if (dir.X < 0)          // going left
	{
		sprite.Play("walkLeft");
		sprite.FlipH = false;   // use left frames as-is
	}
	else if (dir.X > 0)     // going right
	{
		sprite.Play("walkRight");
		sprite.FlipH = false;   // use right frames as-is
	}
	else                    // vertical only – keep facing
	{
		sprite.Play("idle");
	}
}
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

		// silent insta-kill → Cooked picture
		var gm = GameManager.Instance;
gm.Reset();
gm.LastLevel = GetTree().CurrentScene.Name;
SaveHelper.SaveLastLevel(gm.LastLevel);

// Force the "Iori got Slimed" image
gm.ForceCookedImage = true;

Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn"))
	.CallDeferred();
	}
}
