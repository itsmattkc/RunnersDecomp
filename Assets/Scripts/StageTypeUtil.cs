public class StageTypeUtil
{
	private const string stageBaseName = "w";

	private const string bgmBaseName = "bgm_z_";

	private const string bgmCueSheetName = "BGM_z";

	private const string bgmQuickModeBaseName = "bgm_q_";

	private const string bgmQuickModeCueSheetName = "BGM_q";

	public static string GetName(StageType stageType)
	{
		return "w" + ((int)(stageType + 1)).ToString("D2");
	}

	public static string GetCueSheetName(StageType stageType)
	{
		return "BGM_z" + ((int)(stageType + 1)).ToString("D2");
	}

	public static string GetBgmName(StageType stageType)
	{
		return "bgm_z_" + GetName(stageType);
	}

	public static string GetQuickModeCueSheetName(StageType stageType)
	{
		int num = 1;
		if (stageType != 0 && stageType != StageType.W02 && stageType != StageType.W03)
		{
			num = (int)(stageType + 1);
		}
		return "BGM_q" + num.ToString("D2");
	}

	public static string GetQuickModeBgmName(StageType in_stageType)
	{
		StageType stageType = in_stageType;
		if (stageType == StageType.W02 || stageType == StageType.W03)
		{
			stageType = StageType.W01;
		}
		return "bgm_q_" + GetName(stageType);
	}

	public static StageType GetType(string stageName)
	{
		for (int i = 0; i < 30; i++)
		{
			if (stageName == GetName((StageType)i))
			{
				return (StageType)i;
			}
		}
		return StageType.NONE;
	}

	public static string GetBgmName(string stageName)
	{
		return GetBgmName(GetType(stageName));
	}

	public static string GetCueSheetName(string stageName)
	{
		return GetCueSheetName(GetType(stageName));
	}

	public static string GetQuickModeBgmName(string stageName)
	{
		return GetQuickModeBgmName(GetType(stageName));
	}

	public static string GetQuickModeCueSheetName(string stageName)
	{
		return GetQuickModeCueSheetName(GetType(stageName));
	}

	public static string GetStageName(string nameStr)
	{
		for (int i = 0; i < 30; i++)
		{
			string name = GetName((StageType)i);
			if (nameStr.Contains(name))
			{
				return name;
			}
		}
		return string.Empty;
	}
}
