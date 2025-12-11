using Godot;

public partial class PauseMenu : Control
{
	public bool IsPaused => GetTree().Paused;

	public override void _Ready()
	{
		// wire buttons
		GetNode<Button>("VBoxContainer/ResumeBtn").Pressed += OnResume;
		GetNode<Button>("VBoxContainer/OptionsBtn").Pressed += OnOptions;
		GetNode<Button>("VBoxContainer/QuitBtn").Pressed += OnQuit;

		Hide();                       // start invisible
		ProcessMode = ProcessModeEnum.Always; // still receive input when paused
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel") || Input.IsKeyPressed(Key.Escape) ||
	@event.IsActionPressed("pause"))          // you can map your own "pause" action
		{
			Toggle();
			GetViewport().SetInputAsHandled();       // stop propagation
		}
	}

	public void Toggle()
	{
		if (Visible)
			OnResume();
		else
			OnPause();
	}

	private void OnPause()
	{
		Show();
		GetTree().Paused = true;
	}

	private void OnResume()
	{
		Hide();
		GetTree().Paused = false;
	}

	private void OnOptions()
{
	var pop = ResourceLoader.Load<PackedScene>("res://ui/OptionsPopup.tscn").Instantiate<OptionsPopup>();
	AddChild(pop);
}

	private void OnQuit()
	{
		OnResume();                 // unpause before leaving
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
