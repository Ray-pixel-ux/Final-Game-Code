using Godot;

[GlobalClass]
public partial class TrashBin : Area2D
{
	[Export] private float flySpeed      = 600f;   // px/s
	[Export] private float flyAngleMin   = -30f;   // degrees upward
	[Export] private float flyAngleMax   =  30f;

	private AnimatedSprite2D sprite;
	private bool launched = false;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("idle");
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
{
	if (!body.IsInGroup("player") || launched) return;
	launched = true;
	SetDeferred("monitoring", false);

	// ------ bounce the cat away ------
	if (body is CharacterBody2D cat)
	{
		Vector2 dir = (cat.GlobalPosition - GlobalPosition).Normalized();
		cat.Velocity = dir * 200f;   // bounce speed (tweak to taste)
		cat.MoveAndSlide();          // apply immediately
	}

	sprite.Play("launch");
	sprite.AnimationFinished += StartFlying;
}

	private void StartFlying()
	{
		// random upward direction
		float angleRad = Mathf.DegToRad((float)GD.RandRange(flyAngleMin, flyAngleMax));
		Vector2 dir = new Vector2(Mathf.Sin(angleRad), -Mathf.Cos(angleRad));

		// convert to rigid-like motion
		var tween = CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);
		tween.TweenProperty(this, "global_position", GlobalPosition + dir * flySpeed * 2f, 1.5f)
			 .SetTrans(Tween.TransitionType.Linear);

		// free when off-screen
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
