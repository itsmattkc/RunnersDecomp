using SaveData;
using Text;

public class BossTypeUtil
{
	private const string HUD_SPRITENAME1 = "ui_gp_word_boss_";

	private const string HUD_SPRITENAME2 = "ui_gp_gauge_boss_icon_";

	private static readonly BossParam[] BOSS_PARAMS = new BossParam[7]
	{
		new BossParam("eggman", SystemData.FlagStatus.TUTORIAL_FEVER_BOSS, HudTutorial.Id.FEVERBOSS, BossCharaType.EGGMAN, BossCategory.FEVER, 0, 0),
		new BossParam("eggman1", SystemData.FlagStatus.TUTORIAL_BOSS_MAP_1, HudTutorial.Id.MAPBOSS_1, BossCharaType.EGGMAN, BossCategory.MAP, 0, 0),
		new BossParam("eggman2", SystemData.FlagStatus.TUTORIAL_BOSS_MAP_2, HudTutorial.Id.MAPBOSS_2, BossCharaType.EGGMAN, BossCategory.MAP, 1, 1),
		new BossParam("eggman3", SystemData.FlagStatus.TUTORIAL_BOSS_MAP_3, HudTutorial.Id.MAPBOSS_3, BossCharaType.EGGMAN, BossCategory.MAP, 2, 2),
		new BossParam("n", SystemData.FlagStatus.NONE, HudTutorial.Id.EVENTBOSS_1, BossCharaType.EVENT, BossCategory.EVENT, 0, 0),
		new BossParam("r", SystemData.FlagStatus.NONE, HudTutorial.Id.EVENTBOSS_1, BossCharaType.EVENT, BossCategory.EVENT, 1, 0),
		new BossParam("p", SystemData.FlagStatus.NONE, HudTutorial.Id.EVENTBOSS_2, BossCharaType.EVENT, BossCategory.EVENT, 2, 1)
	};

	public static SystemData.FlagStatus GetBossSaveDataFlagStatus(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_flagStatus;
		}
		return SystemData.FlagStatus.NONE;
	}

	public static HudTutorial.Id GetBossTutorialID(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_tutorialID;
		}
		return HudTutorial.Id.NONE;
	}

	public static BossCharaType GetBossCharaType(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_bossCharaType;
		}
		return BossCharaType.NONE;
	}

	public static BossCategory GetBossCategory(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_bossCategory;
		}
		return BossCategory.FEVER;
	}

	public static int GetLayerNumber(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_layerNumber;
		}
		return 0;
	}

	public static int GetIndexNumber(BossType type)
	{
		if ((uint)type < 7u)
		{
			return BOSS_PARAMS[(int)type].m_indexNumber;
		}
		return 0;
	}

	public static string GetBossBgmName(BossType type)
	{
		if (GetBossCharaType(type) == BossCharaType.EGGMAN)
		{
			return "BGM_boss01";
		}
		return EventBossObjectTable.GetItemData(EventBossObjectTableItem.BgmFile);
	}

	public static string GetBossBgmCueSheetName(BossType type)
	{
		if (GetBossCharaType(type) == BossCharaType.EGGMAN)
		{
			return "bgm_z_boss01";
		}
		return string.Concat(str2: (GetLayerNumber(type) + 1).ToString("D2"), str0: EventBossObjectTable.GetItemData(EventBossObjectTableItem.BgmCueName), str1: "_");
	}

	public static string GetBossHudSpriteName(BossType type)
	{
		return "ui_gp_word_boss_" + ((int)GetBossCharaType(type)).ToString(string.Empty);
	}

	public static string GetBossHudSpriteIconName(BossType type)
	{
		return "ui_gp_gauge_boss_icon_" + ((int)GetBossCharaType(type)).ToString(string.Empty);
	}

	public static string GetTextCommonBossName(BossType type)
	{
		if ((uint)type < 7u)
		{
			if (BOSS_PARAMS[(int)type].m_bossCharaType == BossCharaType.EGGMAN)
			{
				return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "BossName", BOSS_PARAMS[(int)type].m_name).text;
			}
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "EventBossName", string.Concat(str3: EventManager.GetSpecificId().ToString(), str0: "bossname_", str1: BOSS_PARAMS[(int)type].m_name, str2: "_")).text;
		}
		return string.Empty;
	}

	public static string GetTextCommonBossCharaName(BossType type)
	{
		if ((uint)type < 7u)
		{
			if (BOSS_PARAMS[(int)type].m_bossCharaType == BossCharaType.EGGMAN)
			{
				return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "BossName", "eggman").text;
			}
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "EventBossName", "bossname_" + EventManager.GetSpecificId()).text;
		}
		return string.Empty;
	}

	public static BossType GetBossTypeRarity(int rarity)
	{
		switch (rarity)
		{
		case 0:
			return BossType.EVENT1;
		case 1:
			return BossType.EVENT2;
		case 2:
			return BossType.EVENT3;
		default:
			return BossType.EVENT1;
		}
	}
}
