using Godot;

public partial class Gem : Area2D
{
	[Export] float collectDuration = 0.15f;

	// Gem.cs
private AnimatedSprite2D sprite;   // was Sprite2D

public override void _Ready()
{
	sprite = GetNode<AnimatedSprite2D>("Sprite2D"); // name in tree
	sprite.Play("spin");          // if you added an animation
	BodyEntered += OnBodyEntered;
}

	private async void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player")) return;

		var gm = GameManager.Instance;
		gm.AddCoin();               // still uses same counter
		SetDeferred("monitoring", false);

		var tween = CreateTween().SetParallel();
		tween.TweenProperty(sprite, "scale", Vector2.Zero, collectDuration)
			 .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "modulate:a", 0f, collectDuration);

		await ToSignal(tween, "finished");
		QueueFree();
	}
}
