using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public enum CollectibleType
{
	Gem, Treat, Watch, Ticket, Coin, TrashBin, TeddyBear,
	TrashBinGold, CrescentRelic, CrescentBlue, Coffee, BaseBat
}

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Signal] public delegate void StaminaChangedEventHandler(int newValue);
	[Signal] public delegate void StaminaDepletedEventHandler();
	[Signal] public delegate void CoinCollectedEventHandler();
	[Signal] public delegate void PowerUpChangedEventHandler();

	/* --------------- new counter system --------------- */
	private readonly Dictionary<CollectibleType, int> _counters = new();
	public int TotalCollected(CollectibleType t) => _counters[t];
	public  int GrandTotal() => _counters.Values.Sum();
	/* -------------------------------------------------- */

	/* --------------- legacy coin façade --------------- */
	public int CoinsCollected => _counters[CollectibleType.Gem] +
								 _counters[CollectibleType.Treat] +
								 _counters[CollectibleType.Watch]; // whatever you still expose
	/* -------------------------------------------------- */

	public int Stamina        { get; private set; } = 10;
	public int MovesLeft      => Stamina;
	public int ItemPoints     => GrandTotal();

	private bool levelEnded = false;
	
	public bool ForceCookedImage { get; set; } = false;

	/*  property with trace  */
	private string _lastLevel = "TutorialStage";
	public string LastLevel
	{
		get => _lastLevel;
		set
		{
			_lastLevel = value;
			GD.Print("TRACE: LastLevel set to = ", value);
		}
	}

	public override void _Ready()
	{
		Instance = this;
		// zero every type
		foreach (CollectibleType t in Enum.GetValues<CollectibleType>())
			_counters[t] = 0;
	}

	/* ========== legacy helpers (still used by old gems/treats) ========== */
	public void AddCoin() => AddCollectible(CollectibleType.Gem);
	public void AddItemPoint() => AddCoin();
	public void AddWatch() => AddCollectible(CollectibleType.Watch);

	/* ========== universal collectible ========== */
	public void AddCollectible(CollectibleType type)
	{
		if (levelEnded) return;
		_counters[type]++;
		GD.Print($"{type} collected, total = {_counters[type]}  (grand={GrandTotal()})");
		EmitSignal(SignalName.CoinCollected); // refresh UI
		if (GrandTotal() >= 4) EndLevel();
	}

	/* ========== stamina ========== */
	public bool CanSpendStamina() => Stamina > 0;

	public bool SpendStamina()
	{
		if (levelEnded || Stamina <= 0) return false;
		Stamina--;
		EmitSignal(SignalName.StaminaChanged, Stamina);
		if (Stamina == 0) EmitSignal(SignalName.StaminaDepleted);
		return true;
	}

	public void SpendMove() => SpendStamina();

	/* ========== level end ========== */
	private void EndLevel()
	{
		if (levelEnded) return;
		levelEnded = true;
		bool win = GrandTotal() >= 4 || (Stamina == 0 && GrandTotal() > 0);
		OnLevelEnded(win);
	}

	private void OnLevelEnded(bool win)
	{
		GD.Print(win ? "WIN" : "LOSE");
		CallDeferred(nameof(GotoGameOver));
	}

	/* ========== scene change ========== */
	private void GotoGameOver()
	{
		GD.Print("DEBUG: About to save LastLevel = ", LastLevel);
		SaveHelper.SaveLastLevel(LastLevel);

		bool perfect = GrandTotal() >= 4;
		if (perfect)
			GetTree().ChangeSceneToFile("res://scenes/AbsoluteCinema.tscn");
		else
			GetTree().ChangeSceneToFile("res://scenes/GameOverScreen.tscn");
	}

	/* ========== reset ========== */
	public void Reset()
	{
		foreach (CollectibleType t in Enum.GetValues<CollectibleType>())
			_counters[t] = 0;
		Stamina    = 10;
		levelEnded = false;
		EmitSignal(SignalName.StaminaChanged, Stamina);
	}

	public void CheckGameOverNow()
	{
		if (Stamina == 0) CallDeferred(nameof(GotoGameOver));
	}

	/* ========== persistence ========== */
	public void LoadPersistedLastLevel()
	{
		string saved = SaveHelper.LoadLastLevel();
		LastLevel = saved;
		NextSceneData.LastLevel = saved;
	}

	public void SetCurrentLevel(string levelName)
	{
		LastLevel = levelName;
		NextSceneData.LastLevel = levelName;
		SaveHelper.SaveLastLevel(levelName);
	}
}
