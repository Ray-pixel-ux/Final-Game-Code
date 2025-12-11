using Godot;

public partial class RandFrameSlime : Node2D
{
	[Export] float minPause = 1.0f;
	[Export] float maxPause = 10.0f;

	private AnimatedSprite2D _sprite;
	private Timer _pauseTimer;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		_sprite.AnimationFinished += OnAnimFinished;

		_pauseTimer = new Timer { OneShot = true };
		AddChild(_pauseTimer);
		_pauseTimer.Timeout += PlayAgain;

		PlayAgain();          // first start
	}

	private void PlayAgain()
	{
		_sprite.Play("default");
	}

	private void OnAnimFinished()
	{
		// animation just ended – schedule random pause
		_pauseTimer.WaitTime = GD.RandRange(minPause, maxPause);
		_pauseTimer.Start();
	}
}
