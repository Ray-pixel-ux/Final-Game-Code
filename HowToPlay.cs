using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class HowToPlay : Control
{
	[Export] private string nextScene = "res://scenes/TutorialStage.tscn";
	[Export] private Texture2D[] pages = new Texture2D[3];

	private TextureRect imageRect;
	private Label pageLabel;
	private Button prevBtn, nextBtn;
	private int current = 0;

	public override void _Ready()
	{
		/*  B.  use the TextureRect you placed manually  */
		imageRect = GetNode<TextureRect>("ImageRect");   // <-- changed
		imageRect.Modulate = Colors.Transparent;         // still start invisible

		/*  page counter  */
		pageLabel = new Label
		{
			AnchorsPreset = (int)LayoutPreset.TopWide,
			AnchorTop = 0.02f, AnchorBottom = 0.08f,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		pageLabel.AddThemeFontSizeOverride("font_size", 24);
		AddChild(pageLabel);

		/*  buttons  */
		prevBtn = new Button { Text = "Previous" };
		nextBtn = new Button { Text = "Next" };
		prevBtn.SetPosition(new Vector2(30, GetViewportRect().Size.Y - 70));
		nextBtn.SetPosition(new Vector2(GetViewportRect().Size.X - 120, GetViewportRect().Size.Y - 70));
		AddChild(prevBtn);
		AddChild(nextBtn);
		prevBtn.Pressed += () => { current--; ShowPage(); };
		nextBtn.Pressed += () => { current++; ShowPage(); };

		ShowPage();
		_ = FadeIn();
	}

	private void ShowPage()
	{
		imageRect.Texture = pages[current];
		pageLabel.Text = $"Page {current + 1} / {pages.Length}";
		prevBtn.Disabled = current == 0;
		nextBtn.Disabled = current == pages.Length - 1;
	}

	private async Task FadeIn()
	{
		var tween = CreateTween().BindNode(this);
		tween.TweenProperty(imageRect, "modulate:a", 1f, 0.4f);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	public override void _Input(InputEvent evt)
	{
		if (evt is InputEventKey k && k.Keycode == Key.Space && k.Pressed)
		{
			GetViewport().SetInputAsHandled();
			GetTree().ChangeSceneToFile(nextScene);
		}
	}
}
