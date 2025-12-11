using Godot;
using System.Threading.Tasks;

public partial class TrashbingoldCollectible : Area2D
{
	[Export] public CollectibleType Type;   // pick in inspector
	[Export] float collectDuration = 0.15f;

	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		sprite.Play("idle");          // every item MUST have an "idle" animation
		BodyEntered += OnBodyEntered;
	}

	private async void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player")) return;

		GameManager.Instance.AddCollectible(Type);   // see §3
		SetDeferred("monitoring", false);

		var tween = CreateTween().SetParallel();
		tween.TweenProperty(sprite, "scale", Vector2.Zero, collectDuration)
			 .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "modulate:a", 0f, collectDuration);

		await ToSignal(tween, "finished");
		QueueFree();
	}
}
