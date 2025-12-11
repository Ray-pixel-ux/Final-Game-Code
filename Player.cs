using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float LaunchPower   = 1000f;
	[Export] public int   MaxLinePoints = 20;
	[Export] public float BounceDamping = 0.75f;

	private Vector2 _startDrag, _endDrag;
	private bool _isDragging;
	private Line2D _launchLine;
	private AnimatedSprite2D _animatedSprite;
	private GameManager _gm;
	private bool _waitingForStop = false;

	// drag / release audio
	private AudioStreamPlayer dragSfx;
	private AudioStreamPlayer releaseSfx;

	public override void _Ready()
	{
		_launchLine     = GetNode<Line2D>("LaunchLine");
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite");
		_animatedSprite.Play("idle");

		_gm = GameManager.Instance;
		GD.Print("Player found GameManager = ", _gm != null ? "YES" : "NO");

		dragSfx    = GetNode<AudioStreamPlayer>("DragSfx");
		releaseSfx = GetNode<AudioStreamPlayer>("ReleaseSfx");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
		{
			if (mb.Pressed)
			{
				_startDrag  = GetGlobalMousePosition();
				_isDragging = true;
				if (!dragSfx.Playing) dragSfx.Play();
			}
			else if (_isDragging)
			{
				_endDrag    = GetGlobalMousePosition();
				_isDragging = false;
				_launchLine.Points = System.Array.Empty<Vector2>();

				dragSfx.Stop();
				releaseSfx.Play();
				Launch();
			}
		}

		if (_isDragging)
			DrawLaunchPreview(GetGlobalMousePosition());
	}

	private void DrawLaunchPreview(Vector2 mouseGlobal)
	{
		Vector2 playerLocal = ToLocal(GlobalPosition);
		Vector2 mouseLocal  = ToLocal(mouseGlobal);
		Vector2 dirLocal    = (mouseLocal - playerLocal).Normalized();

		Vector2[] pts = new Vector2[MaxLinePoints];
		for (int i = 0; i < MaxLinePoints; i++)
		{
			float t = i / (float)(MaxLinePoints - 1);
			pts[i] = playerLocal + dirLocal * t * 120f;
		}
		_launchLine.Points = pts;
	}

	private void Launch()
	{
		if (_gm.Stamina <= 0)
		{
			GD.Print("NO STAMINA – abort");
			return;
		}

		_gm.SpendStamina();

		Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		float raw   = Mathf.Clamp((_endDrag - GlobalPosition).Length() * 2f, 0, LaunchPower);
		float power = GetNode<PlayerPowerUpManager>("PowerUpManager").ModifyLaunchPower(raw);

		// flare animation if speed boost active
		// GetNode<PlayerPowerUpManager>("PowerUpManager").ShowBoostFlare();

		Velocity = direction * power;
		_animatedSprite.Play(direction.X < 0 ? "jump_left" : "jump_right");
		_waitingForStop = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		Velocity *= Mathf.Clamp(1f - 0.05f * dt * 60f, 0f, 1f);

		var collision = MoveAndCollide(Velocity * dt);
		if (collision != null)
			Velocity = Velocity.Bounce(collision.GetNormal()) * BounceDamping;

		if (Velocity.Length() < 30f)
			_animatedSprite.Play("idle");

		if (_waitingForStop && Velocity.Length() < 30f)
		{
			_waitingForStop = false;
			GameManager.Instance.CheckGameOverNow();
		}
	}
}
