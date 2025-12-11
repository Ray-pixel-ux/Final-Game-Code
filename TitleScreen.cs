using Godot;

public partial class TitleScreen : Control
{
	[Export] private string nextScene = "res://scenes/MainMenu.tscn";
	[Export] private AudioStream titleMusic;   // drag music file here

	private Label prompt;
	private AudioStreamPlayer music;
	private AnimatedSprite2D bgAnimator;

	public override void _Ready()
	{
		// 1. animated background
		bgAnimator = GetNode<AnimatedSprite2D>("BgAnimator");
		bgAnimator.Play("idle");

		// 2. blinking prompt
		prompt = GetNode<Label>("Label");
		var tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(prompt, "modulate:a", 0.3f, 0.8f);
		tween.TweenProperty(prompt, "modulate:a", 1.0f, 0.8f);

		// 3. optional music
		if (titleMusic != null)
		{
			music = new AudioStreamPlayer { Stream = titleMusic, Autoplay = true };
			AddChild(music);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey or InputEventMouseButton && @event.IsPressed())
		{
			GetViewport().SetInputAsHandled();
			GetTree().ChangeSceneToFile(nextScene);
		}
	}
}
