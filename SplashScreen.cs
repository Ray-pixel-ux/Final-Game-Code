using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class SplashScreen : Control
{
	[Export] private float fadeDuration = 1.2f;
	[Export] private float holdDuration = 2.5f;
	[Export] private string nextScene = "res://scenes/TitleScreen.tscn";
	[Export] private FontFile comicateFont;

	private TextureRect backdrop;
	private Label label;
	private Label skipLabel; // bottom-centre skip text

	private readonly Color[] topColors =
	{
		new(0.11f, 0.22f, 0.55f), new(0.93f, 0.30f, 0.48f),
		new(0.20f, 0.85f, 0.45f), new(0.98f, 0.74f, 0.16f)
	};
	private readonly Color[] bottomColors =
	{
		new(0.93f, 0.30f, 0.48f), new(0.20f, 0.85f, 0.45f),
		new(0.98f, 0.74f, 0.16f), new(0.11f, 0.22f, 0.55f)
	};

	private readonly List<string> lines = new()
	{
		"Developed by BSEMC-DAT 2A Students\nfrom Bukidnon State University\n\nJanrey Jandayan\nKurt Nicholai Bolislis\nIvor Paypa",
		"Subject: Intro to Game Design & Development\n\nInstructor: Mark Daniel G. Dacer",
		"In collaboration with Subject:\nData Structures & Algorithm\n\nInstructor: Rov Japheth G. Oracion",
        "S.Y. 2025-2026"
	};

	public override void _Ready()
{
	/* colourful gradient backdrop */
	backdrop = new TextureRect
	{
		Texture = MakeGradient(topColors[0], bottomColors[0]),
		StretchMode = TextureRect.StretchModeEnum.Scale,
		AnchorsPreset = (int)Control.LayoutPreset.FullRect
	};
	AddChild(backdrop);

	/* 32 px label with Comicate font */
	label = new Label
	{
		AnchorsPreset = (int)Control.LayoutPreset.Center,
		AnchorLeft = 0.5f, AnchorTop = 0.5f,
		AnchorRight = 0.5f, AnchorBottom = 0.5f,
		OffsetLeft = -300, OffsetRight = 300,
		OffsetTop = -50, OffsetBottom = 50,
		HorizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment = VerticalAlignment.Center,
		AutowrapMode = TextServer.AutowrapMode.WordSmart,
		Modulate = Colors.Transparent
	};
	var theme = new Theme();
	theme.SetFont("font", "Label", comicateFont);
	theme.SetFontSize("font_size", "Label", 32);
	label.Theme = theme;
	AddChild(label);

	/* centre-bottom skip label – font size 40 */
skipLabel = new Label
{
	Text = "Press Anywhere To Skip",
	AnchorsPreset = (int)Control.LayoutPreset.CenterBottom,
	AnchorLeft = 0.5f, AnchorRight = 0.5f,
	AnchorTop = 1f, AnchorBottom = 1f,
	OffsetTop = -80, OffsetBottom = -20,
	HorizontalAlignment = HorizontalAlignment.Center,
	Modulate = Colors.White // visible now
};
skipLabel.AddThemeFontSizeOverride("font_size", 30);
AddChild(skipLabel);

/* ---------- fade in/out loop (same tween as splash) ---------- */
var tween = CreateTween()
	.SetLoops()
	.SetTrans(Tween.TransitionType.Sine)
	.SetEase(Tween.EaseType.InOut)
	.BindNode(this);

tween.TweenProperty(skipLabel, "modulate", Colors.White, 1f)
	 .From(Colors.Transparent);
tween.TweenProperty(skipLabel, "modulate", Colors.Transparent, 1f);

	// ---------- debug prints ----------
	GD.Print("skip label text = ", skipLabel.Text);
	GD.Print("skip global pos = ", skipLabel.GlobalPosition);
	GD.Print("skip modulate = ", skipLabel.Modulate);

	ShowSequence();
}
	
	// ---------- click anywhere to skip ----------
public override void _Input(InputEvent @event)
{
	if (@event is InputEventMouseButton mb && mb.Pressed)
		GetTree().ChangeSceneToFile(nextScene);
}

	private void OnAnyClick(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.Pressed)
			GetTree().ChangeSceneToFile(nextScene);
	}

	private async void ShowSequence()
	{
		await Fade(backdrop, Colors.Black, Colors.Transparent, fadeDuration);

		for (int i = 0; i < lines.Count; i++)
		{
			backdrop.Texture = MakeGradient(topColors[i], bottomColors[i]);
			label.Text = lines[i];
			await Fade(label, Colors.Transparent, Colors.White, fadeDuration);
			await ToSignal(GetTree().CreateTimer(holdDuration), SceneTreeTimer.SignalName.Timeout);
			await Fade(label, Colors.White, Colors.Transparent, fadeDuration);
		}

		await Fade(backdrop, Colors.Transparent, Colors.Black, fadeDuration);
		GetTree().ChangeSceneToFile(nextScene);
	}

	/* bottom-centre skip label fade loop */
	private async Task Fade(CanvasItem node, Color from, Color to, float duration)
	{
		var tween = CreateTween()
				   .SetTrans(Tween.TransitionType.Linear)
				   .SetEase(Tween.EaseType.InOut)
				   .BindNode(this);

		node.Modulate = from;
		tween.TweenProperty(node, "modulate", to, duration);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	private GradientTexture2D MakeGradient(Color top, Color bottom)
	{
		var g = new Gradient();
		g.AddPoint(0f, top);
		g.AddPoint(1f, bottom);

		return new GradientTexture2D
		{
			Gradient = g,
			Width = 1920,
			Height = 1080
		};
	}
}
