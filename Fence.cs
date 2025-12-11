using Godot;

public partial class Fence : CharacterBody2D
{
	[Export] private float bounceForce = 250f;   // how hard it pushes the player away

	private AnimatedSprite2D _sprite;
	private CollisionShape2D _solidShape;
	private Area2D _hitArea;
	private bool _alreadyShattered = false;

	public override void _Ready()
	{
		_sprite     = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_solidShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_hitArea    = GetNode<Area2D>("Area2D");

		_hitArea.BodyEntered += OnBodyEntered;
		_sprite.AnimationFinished += OnAnimationFinished;
		_sprite.Play("idle");
	}

	// Player touched the detector area
	private void OnBodyEntered(Node2D body)
	{
		if (_alreadyShattered) return;
		if (!body.IsInGroup("player")) return;   // make sure your player node is in group “player”

		_alreadyShattered = true;

		// 1.  push the player away
		if (body is CharacterBody2D pb)
		{
			Vector2 dir = (pb.GlobalPosition - GlobalPosition).Normalized();
			pb.Velocity = dir * bounceForce;
			pb.MoveAndSlide();
		}

		// 2.  start break animation
		_sprite.Play("shatter");
		_solidShape.SetDeferred("disabled", true); // stop further physics contacts
	}

	// When the break animation ends, delete the box
	private void OnAnimationFinished() => QueueFree();
}
