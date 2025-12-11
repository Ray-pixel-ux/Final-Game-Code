using Godot;
using System;

public partial class PlayerPowerUpManager : Node
{
	[Export] public float SpeedBoostMultiplier = 2f;
	[Export] public NodePath ShieldAnimatorPath;
	[Export] public AudioStream ShieldBreakSound;
	[Export] public NodePath BoostAnimatorPath;

	private Player _player;
	private AnimatedSprite2D _shieldSprite;
	private AnimatedSprite2D _boostSprite;
	private bool _hasSpeedBoost;
	private bool _hasShield;

	public bool HasSpeedBoost => _hasSpeedBoost;
	public bool HasShield     => _hasShield;
	
	public void ShowBoostFlare()
{
	if (!_hasSpeedBoost || _boostSprite == null) return;

	_boostSprite.Show();
	_boostSprite.Play("flare");

	// auto-hide when animation finishes
	float len = (float)(_boostSprite.SpriteFrames.GetFrameCount("flare") /
					  _boostSprite.SpriteFrames.GetAnimationSpeed("flare"));
	GetTree().CreateTimer(len).Timeout += () => _boostSprite.Hide();
}

	public override void _Ready()
	{
		_player = GetOwner<Player>();
		if (ShieldAnimatorPath != null)
			_shieldSprite = GetNode<AnimatedSprite2D>(ShieldAnimatorPath);
			
			_player = GetOwner<Player>();
	if (ShieldAnimatorPath != null)
		_shieldSprite = GetNode<AnimatedSprite2D>(ShieldAnimatorPath);
	if (BoostAnimatorPath != null)
		_boostSprite = GetNode<AnimatedSprite2D>(BoostAnimatorPath);
	}

public void AddSpeedBoost()
{
	bool wasActive = _hasSpeedBoost;
	_hasSpeedBoost = true;
	GameManager.Instance.EmitSignal(GameManager.SignalName.PowerUpChanged);

	if (!wasActive)               // first pickup only
		ShowBoostFlare();
}
	public void AddShield()
	{
		_hasShield = true;
		_shieldSprite?.Show();
		_shieldSprite?.Play("idle");
		GameManager.Instance.EmitSignal(GameManager.SignalName.PowerUpChanged);
	}

	public float ModifyLaunchPower(float original)
		=> _hasSpeedBoost ? original * SpeedBoostMultiplier : original;

	public bool ConsumeShield()
	{
		if (!_hasShield) return false;
		_hasShield = false;
		_shieldSprite?.Hide();
		GameManager.Instance.EmitSignal(GameManager.SignalName.PowerUpChanged);
		return true;
	}
}
