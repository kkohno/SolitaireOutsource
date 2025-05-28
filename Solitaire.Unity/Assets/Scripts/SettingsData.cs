public class SettingsData
{
	public bool SoundEnabled{ get; set; }

	public bool TimerEnabled{ get; set; }

	public bool TapMoveEnabled{ get; set; }

	public bool HintsEnabled{ get; set; }

	public bool FinalAnimEnabled{ get; set; }

	public bool BgAnimEnabled{ get; set; }

	public bool ThreeFlips{ get; set; }

	public bool ChangeOrient{ get; set; }

	public int UndoForAds{ get; set; }

	public int Background{ get; set; }

	public int CardStyle{ get; set; }

	public int CardBack{ get; set; }

	public EffectStyle Effect{ get; set; }

	public double Volume{ get; set; }

	public string Language { get; set; }

	public GameType DeckType { get; set; }

	public ScoreType ScoreType { get; set; }

	public SettingsData ()
	{
		//default values
		CardStyle = 0;
		CardBack = 0;
		SoundEnabled = true;
		TimerEnabled = true;
		TapMoveEnabled = true;
		UndoForAds = 3;
		HintsEnabled = true;
		FinalAnimEnabled = true;
		BgAnimEnabled = true;
		ThreeFlips = false;
		Background = 0;
		ChangeOrient = true;
		Volume = 1.0;
		Language = "English";
		Effect = EffectStyle.Dust;
		ScoreType = ScoreType.Standart;
		DeckType = GameType.OneOnOne;
	}
}


