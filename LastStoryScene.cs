using System;
using System.Threading.Tasks;
using Godot;

public partial class LastStoryScene : Control
{
	[Export] private float fadeDuration = 1.5f;
	[Export] private float holdDuration = 3.0f;
	[Export] private string nextScene = "res://scenes/MainMenu.tscn";
	
	// Add your 9 story images here in the Inspector
	[Export] private Texture2D[] storyImages = new Texture2D[9];
	
	// Add your 9 story texts here in the Inspector (one for each image)
	[Export] private string[] storyTexts = new string[9];
	
	// Add your custom background texture here in the Inspector
	[Export] private Texture2D customBackground;
	
	private TextureRect backdrop;
	private TextureRect storyImage;
	private Label storyTextLabel;
	private Label skipLabel;
	
	private int currentImageIndex = 0;

	public override void _Ready()
	{
		// Create backdrop - use custom background if provided, otherwise use gradient
		backdrop = new TextureRect
		{
			StretchMode = TextureRect.StretchModeEnum.Scale,
			AnchorsPreset = (int)Control.LayoutPreset.FullRect
		};
		
		// Set the background texture - custom if available, otherwise gradient
		if (customBackground != null)
		{
			backdrop.Texture = customBackground;
			backdrop.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		}
		else
		{
			backdrop.Texture = MakeGradient(new Color(0.1f, 0.1f, 0.2f), new Color(0.05f, 0.05f, 0.1f));
		}
		
		AddChild(backdrop);

		// Create story image display
		storyImage = new TextureRect
		{
			AnchorsPreset = (int)Control.LayoutPreset.Center,
			AnchorLeft = 0.5f, AnchorTop = 0.5f,
			AnchorRight = 0.5f, AnchorBottom = 0.5f,
			OffsetLeft = -400, OffsetRight = 400,
			OffsetTop = -300, OffsetBottom = 300,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			Modulate = Colors.Transparent
		};
		AddChild(storyImage);

		// Create story text label
		storyTextLabel = new Label
		{
			Text = "",
			AnchorsPreset = (int)Control.LayoutPreset.BottomWide,
			AnchorLeft = 0.1f, AnchorRight = 0.9f,
			AnchorTop = 0.7f, AnchorBottom = 0.95f,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Modulate = Colors.Transparent,
			AutowrapMode = TextServer.AutowrapMode.Word,
			SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
		};
		storyTextLabel.AddThemeFontSizeOverride("font_size", 32);
		storyTextLabel.AddThemeColorOverride("font_color", Colors.White);
		storyTextLabel.AddThemeConstantOverride("line_spacing", 10);
		AddChild(storyTextLabel);

		// Optional: Add semi-transparent background behind text for better readability
		var textBackground = new ColorRect
		{
			AnchorsPreset = (int)Control.LayoutPreset.BottomWide,
			AnchorLeft = 0f, AnchorRight = 1f,
			AnchorTop = 0.7f, AnchorBottom = 1f,
			Color = new Color(0f, 0f, 0f, 0.6f),
			Modulate = Colors.Transparent
		};
		AddChild(textBackground);
		MoveChild(textBackground, GetChildCount() - 2); // Move behind text

		// Skip label
		skipLabel = new Label
		{
			Text = ".",
			AnchorsPreset = (int)Control.LayoutPreset.CenterBottom,
			AnchorLeft = 0.5f, AnchorRight = 0.5f,
			AnchorTop = 1f, AnchorBottom = 1f,
			OffsetTop = -80, OffsetBottom = -20,
			HorizontalAlignment = HorizontalAlignment.Center,
			Modulate = Colors.White
		};
		skipLabel.AddThemeFontSizeOverride("font_size", 1);
		AddChild(skipLabel);

		// Skip label fade animation
		var tween = CreateTween()
			.SetLoops()
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut)
			.BindNode(this);

		tween.TweenProperty(skipLabel, "modulate", Colors.White, 1f)
			 .From(Colors.Transparent);
		tween.TweenProperty(skipLabel, "modulate", Colors.Transparent, 1f);

		// Start the story sequence
		StartStorySequence();
	}

	// Input handling for skip
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.Pressed)
			GetTree().ChangeSceneToFile(nextScene);
	}

	private async void StartStorySequence()
	{
		// Fade in from black
		await Fade(backdrop, Colors.Black, Colors.White, fadeDuration);

		// Display all 9 images and texts in sequence
		for (int i = 0; i < storyImages.Length; i++)
		{
			if (storyImages[i] != null)
			{
				// Set current image and text
				storyImage.Texture = storyImages[i];
				storyTextLabel.Text = storyTexts.Length > i ? storyTexts[i] : "";
				
				// Fade in image
				await Fade(storyImage, Colors.Transparent, Colors.White, fadeDuration);
				
				// Fade in text
				await Fade(storyTextLabel, Colors.Transparent, Colors.White, fadeDuration * 0.5f);
				
				// Hold for the specified duration
				await ToSignal(GetTree().CreateTimer(holdDuration), SceneTreeTimer.SignalName.Timeout);
				
				// Fade out text
				await Fade(storyTextLabel, Colors.White, Colors.Transparent, fadeDuration * 0.5f);
				
				// Fade out image
				await Fade(storyImage, Colors.White, Colors.Transparent, fadeDuration);
			}
		}

		// Fade to black and go to next scene
		await Fade(backdrop, Colors.White, Colors.Black, fadeDuration);
		GetTree().ChangeSceneToFile(nextScene);
	}

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
