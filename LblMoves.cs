using Godot;

public partial class LblMoves : Control
{
	private Label _label;
	private GameManager _gm;

	public override void _Ready()
	{
		// the node that shows the text is the first (only) child Label
		_label = GetNode<Label>("Label");   // or whatever the child is really named
		_gm    = GameManager.Instance;
		_gm.StaminaChanged += OnStaminaChanged;
		OnStaminaChanged(_gm.MovesLeft); // first draw
	}

	private void OnStaminaChanged(int value)
	{
		// scene gone?  do nothing
		if (!IsInstanceValid(_label)) return;

		_label.Text = $"Stamina: {value}";
		_label.Modulate = value == 0 ? Colors.Red : Colors.White;
	}

	public override void _ExitTree()
	{
		// unsubscribe when this UI leaves so the autoload won't call a dead object
		if (IsInstanceValid(_gm))
			_gm.StaminaChanged -= OnStaminaChanged;
	}
}
