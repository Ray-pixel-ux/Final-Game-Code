using Godot;

public partial class Level9 : Node2D
{
	[Export] private AudioStream titleMusic; // drag your music file here
	
	private AudioStreamPlayer music;
	
	public override void _Ready()
	{
		// 2. music (optional – no error if you leave export empty)
	if (titleMusic != null)
	{
		music = new AudioStreamPlayer { Stream = titleMusic, Autoplay = true };
		AddChild(music);
	}
		
		GameManager.Instance.LoadPersistedLastLevel(); // <-- ADD
		GameManager.Instance.SetCurrentLevel("Level9"); // save immediately
		//  REMOVE any SaveHelper call here
	}
}
