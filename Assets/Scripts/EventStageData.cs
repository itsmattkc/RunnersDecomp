public class EventStageData
{
	public int bg_id;

	public string stage_key;

	public string stageCueSheetName;

	public string bossStagCueSheetName;

	public string quickStageCueSheetName;

	public string stageBGM;

	public string bossStagBGM;

	public string quickStageBGM;

	public bool IsEndlessModeBGM()
	{
		return !string.IsNullOrEmpty(stageCueSheetName) && !string.IsNullOrEmpty(bossStagCueSheetName) && !string.IsNullOrEmpty(stageBGM) && !string.IsNullOrEmpty(bossStagBGM);
	}

	public bool IsQuickModeBGM()
	{
		return !string.IsNullOrEmpty(quickStageCueSheetName) && !string.IsNullOrEmpty(quickStageBGM);
	}
}
