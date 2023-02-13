using SaveData;

public class CharaTypeUtil
{
	private static readonly CharaParam[] CHARA_PARAMS = new CharaParam[29]
	{
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_0, HudTutorial.Id.CHARA_0),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_1, HudTutorial.Id.CHARA_1),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_2, HudTutorial.Id.CHARA_2),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_3, HudTutorial.Id.CHARA_3),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_4, HudTutorial.Id.CHARA_4),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_5, HudTutorial.Id.CHARA_5),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_6, HudTutorial.Id.CHARA_6),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_7, HudTutorial.Id.CHARA_7),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_8, HudTutorial.Id.CHARA_8),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_9, HudTutorial.Id.CHARA_9),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_10, HudTutorial.Id.CHARA_10),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_11, HudTutorial.Id.CHARA_11),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_12, HudTutorial.Id.CHARA_12),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_13, HudTutorial.Id.CHARA_13),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_14, HudTutorial.Id.CHARA_14),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_15, HudTutorial.Id.CHARA_15),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_16, HudTutorial.Id.CHARA_16),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_17, HudTutorial.Id.CHARA_17),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_18, HudTutorial.Id.CHARA_18),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_19, HudTutorial.Id.CHARA_19),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_20, HudTutorial.Id.CHARA_20),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_21, HudTutorial.Id.CHARA_21),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_22, HudTutorial.Id.CHARA_22),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_23, HudTutorial.Id.CHARA_23),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_24, HudTutorial.Id.CHARA_24),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_25, HudTutorial.Id.CHARA_25),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_26, HudTutorial.Id.CHARA_26),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_27, HudTutorial.Id.CHARA_27),
		new CharaParam(SystemData.CharaTutorialFlagStatus.CHARA_28, HudTutorial.Id.CHARA_28)
	};

	public static SystemData.CharaTutorialFlagStatus GetCharacterSaveDataFlagStatus(CharaType type)
	{
		if ((uint)type < 29u)
		{
			return CHARA_PARAMS[(int)type].m_flagStatus;
		}
		return SystemData.CharaTutorialFlagStatus.NONE;
	}

	public static HudTutorial.Id GetCharacterTutorialID(CharaType type)
	{
		if ((uint)type < 29u)
		{
			return CHARA_PARAMS[(int)type].m_tutorialID;
		}
		return HudTutorial.Id.NONE;
	}

	public static CharacterAttribute GetCharacterAttribute(CharaType type)
	{
		CharacterAttribute result = CharacterAttribute.UNKNOWN;
		if (CharacterDataNameInfo.Instance != null)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(type);
			if (dataByID != null)
			{
				result = dataByID.m_attribute;
			}
		}
		return result;
	}

	public static TeamAttribute GetTeamAttribute(CharaType type)
	{
		TeamAttribute result = TeamAttribute.UNKNOWN;
		if (CharacterDataNameInfo.Instance != null)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(type);
			if (dataByID != null)
			{
				result = dataByID.m_teamAttribute;
			}
		}
		return result;
	}

	public static string GetCharaSpriteNameSuffix(CharaType charaType)
	{
		if (CharacterDataNameInfo.Instance != null)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(charaType);
			if (dataByID != null)
			{
				return string.Format("{0:D2}_{1}", (int)charaType, dataByID.m_hud_suffix);
			}
		}
		return string.Empty;
	}
}
