using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BootSequence : Control
{
	[Export] private string nextScene = "res://scenes/SplashScreen.tscn";
	[Export] private float fadeTime   = 0.5f;
	[Export] private float holdTime   = 1.0f;

	/* logo */
	[Export] private Texture2D logoTexture;
	[Export] private float logoScalePop = 1.15f;
	[Export] private float logoGlowCycle = 1.2f;
	[Export] private float bounceSpeed = 280f;

	private Label cardLabel;
	private TextureRect logoDisplay;
	private Label promptLabel;          // you can drag one in or we create it
	private CpuParticles2D sparkle;

	private readonly List<string> cards = new()
	{
		"Publisher\nBSEMC Studios",
		"Developer\n3 Fleas Team",
        "Powered by\nGodot 4"
	};

	private Vector2 velocity;
	private readonly Color[] bounceColors = {
		Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow,
		Colors.Magenta, Colors.Cyan, Colors.Orange, Colors.SpringGreen
	};
	private int colorIndex = 0;

	/* ---------- life-cycle ---------- */
	public override void _Ready()
	{
		cardLabel = GetNode<Label>("CardLabel");
		CreateLogoNode();
		ProvidePromptLabel();   // <-- changed
		_ = RunSequence();
	}

	/* ---------- prompt label (created only if you didn't add one) ---------- */
	private void ProvidePromptLabel()
	{
		// if you already put a Label named "PromptLabel" in scene, use it
		promptLabel = GetNodeOrNull<Label>("PromptLabel");
		if (promptLabel == null)
		{
			promptLabel = new Label
			{
				Name = "PromptLabel",
				Text = "Press Space to continue",
				AnchorsPreset = (int)LayoutPreset.BottomRight,
				AnchorLeft = 1f, AnchorTop = 1f,
				AnchorRight = 1f, AnchorBottom = 1f,
				OffsetLeft = -220, OffsetTop = -40,
				OffsetRight = -10, OffsetBottom = -10,
				HorizontalAlignment = HorizontalAlignment.Right,
				Modulate = Colors.Transparent   // start invisible
			};
			promptLabel.AddThemeFontSizeOverride("font_size", 15);
			AddChild(promptLabel);
		}

		// blinking loop
		var tween = CreateTween().SetLoops().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(promptLabel, "modulate:a", 0.3f, 0.9f);
		tween.TweenProperty(promptLabel, "modulate:a", 1f, 0.9f);
	}

	/* ---------- logo ---------- */
	private void CreateLogoNode()
	{
		logoDisplay = new TextureRect
		{
			Texture = logoTexture,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			AnchorsPreset = (int)LayoutPreset.TopLeft,
			OffsetLeft = 100, OffsetTop = 100,
			OffsetRight = 100 + 256, OffsetBottom = 100 + 128,
			Modulate = Colors.White
		};
		AddChild(logoDisplay);

		var rng = new Random();
		velocity = new Vector2(
			bounceSpeed * (rng.NextSingle() * 0.4f + 0.8f),
			bounceSpeed * (rng.NextSingle() * 0.4f + 0.8f)
		).Rotated(rng.NextSingle() * Mathf.Tau);
	}

	/* ---------- sequence ---------- */
	private async Task RunSequence()
	{
		// 1. text cards
		foreach (var txt in cards)
		{
			cardLabel.Text = txt;
			await Fade(cardLabel, 0f, 1f, fadeTime);
			await ToSignal(GetTree().CreateTimer(holdTime), SceneTreeTimer.SignalName.Timeout);
			await Fade(cardLabel, 1f, 0f, fadeTime);
		}

		// 2. dvd bounce
		await Fade(logoDisplay, 0f, 1f, fadeTime);
		await Fade(promptLabel, 0f, 1f, fadeTime);   // show prompt
		SetProcess(true);
		// <-- removed forced 4-s timer; now waits for player
	}

	/* ---------- dvd bounce ---------- */
	public override void _Process(double delta)
	{
		float dt = (float)delta;
		Rect2 vp = GetViewportRect();
		Vector2 pos = logoDisplay.Position;
		Vector2 size = logoDisplay.Size;

		pos += velocity * dt;

		bool bounced = false;
		if (pos.X <= 0) { velocity.X =  Math.Abs(velocity.X); pos.X = 0; bounced = true; }
		if (pos.X + size.X >= vp.Size.X) { velocity.X = -Math.Abs(velocity.X); pos.X = vp.Size.X - size.X; bounced = true; }
		if (pos.Y <= 0) { velocity.Y =  Math.Abs(velocity.Y); pos.Y = 0; bounced = true; }
		if (pos.Y + size.Y >= vp.Size.Y) { velocity.Y = -Math.Abs(velocity.Y); pos.Y = vp.Size.Y - size.Y; bounced = true; }

		if (bounced)
		{
			colorIndex = (colorIndex + 1) % bounceColors.Length;
			logoDisplay.Modulate = bounceColors[colorIndex];
		}

		logoDisplay.Position = pos;
	}

	/* ---------- input ---------- */
	public override void _Input(InputEvent evt)
	{
		if (evt.IsPressed() && 
			(evt is InputEventKey kb && kb.Keycode == Key.Space ||
			 evt is InputEventMouseButton))
		{
			GetViewport().SetInputAsHandled();
			Finish();
		}
	}

	/* ---------- utils ---------- */
	private async Task Fade(CanvasItem node, float fromA, float toA, float duration)
	{
		var tween = CreateTween()
				   .SetTrans(Tween.TransitionType.Linear)
				   .SetEase(Tween.EaseType.InOut)
				   .BindNode(this);
		node.Modulate = new Color(1, 1, 1, fromA);
		tween.TweenProperty(node, "modulate:a", toA, duration);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	private void Finish()
		=> GetTree().ChangeSceneToFile(nextScene);
}
