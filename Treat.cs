using Godot;

public partial class Treat : Area2D
{
	[Export] float collectDuration = 0.15f;

	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite2d");
		sprite.Play("spin");
		BodyEntered += OnBodyEntered;
	}

	private async void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player")) return;

		var gm = GameManager.Instance;
		gm.AddCoin();
		SetDeferred("monitoring", false);

		var tween = CreateTween().SetParallel();
		tween.TweenProperty(sprite, "scale", Vector2.Zero, collectDuration)
			 .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "modulate:a", 0f, collectDuration);

		await ToSignal(tween, "finished");
		QueueFree();
	}
}
