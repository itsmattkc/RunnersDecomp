using DataTable;
using System.Collections.Generic;
using Text;

public class BonusUtil
{
	private const int BOUNES_OFFSET_POINT = 0;

	public const float TEAM_ATTRIBUTE_DEMERIT_VALUE_EASY = -0.2f;

	public const float TEAM_ATTRIBUTE_DEMERIT_DOUBLE_VALUE_EASY = -0.36f;

	public const float TEAM_ATTRIBUTE_DEMERIT_VALUE_EASY_RATIO = 0.8f;

	public const float TEAM_ATTRIBUTE_DEMERIT_VALUE_EASY_DOUBLE_RATIO = 0.64f;

	private const string ABILITY_ICON_SPRITE_NAME_BASE = "ui_chao_set_ability_icon_{PARAM}";

	private static List<CharaType> s_charaList;

	private static List<ChaoData> s_chaoList;

	public static float GetTotalScoreBonus(float currentBonusRate, float addBonusRate)
	{
		float num = 0f;
		if (currentBonusRate == 0f)
		{
			return addBonusRate;
		}
		if (currentBonusRate < 0f)
		{
			if (addBonusRate > 0f)
			{
				return currentBonusRate + addBonusRate;
			}
			float num2 = 1f + addBonusRate;
			float num3 = 1f + currentBonusRate;
			return -1f + num3 * num2;
		}
		if (addBonusRate > 0f)
		{
			float num4 = 1f - addBonusRate;
			float num5 = 1f - currentBonusRate;
			return 1f - num5 * num4;
		}
		return currentBonusRate + addBonusRate;
	}

	public static string GetAbilityIconSpriteName(BonusParam.BonusType type, float value)
	{
		string result = string.Empty;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Dscore") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Uscore"));
			break;
		case BonusParam.BonusType.RING:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Dring") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Uring"));
			break;
		case BonusParam.BonusType.ANIMAL:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Danimal") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Uanimal"));
			break;
		case BonusParam.BonusType.DISTANCE:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Drange") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Urange"));
			break;
		case BonusParam.BonusType.ENEMY_OBJBREAK:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Denemy") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Uenemy"));
			break;
		case BonusParam.BonusType.TOTAL_SCORE:
			result = ((!IsBonusMerit(type, value)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Dfscore") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Ufscore"));
			break;
		case BonusParam.BonusType.SPEED:
			result = ((!(value > 100f)) ? "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Dspeed") : "ui_chao_set_ability_icon_{PARAM}".Replace("{PARAM}", "Uspeed"));
			break;
		}
		return result;
	}

	private static float GetTeamDemritBonus(TeamAttribute type)
	{
		float result = 0f;
		if (type == TeamAttribute.EASY)
		{
			result = -0.2f;
		}
		return result;
	}

	public static Dictionary<BonusParam.BonusType, bool> IsTeamBonus(CharaType charaType, List<BonusParam.BonusType> types)
	{
		Dictionary<BonusParam.BonusType, bool> dictionary = null;
		if (charaType != CharaType.UNKNOWN && charaType != CharaType.NUM && types != null)
		{
			dictionary = new Dictionary<BonusParam.BonusType, bool>();
			Dictionary<BonusParam.BonusType, float> bonusParam;
			if (GetTeamBonus(charaType, out bonusParam))
			{
				for (int i = 0; i < types.Count; i++)
				{
					dictionary.Add(types[i], bonusParam.ContainsKey(types[i]));
				}
			}
		}
		return dictionary;
	}

	public static bool IsTeamBonus(CharaType charaType, BonusParam.BonusType type)
	{
		bool result = false;
		Dictionary<BonusParam.BonusType, float> bonusParam;
		if (charaType != CharaType.UNKNOWN && charaType != CharaType.NUM && type != BonusParam.BonusType.NONE && GetTeamBonus(charaType, out bonusParam) && bonusParam.ContainsKey(type))
		{
			result = true;
		}
		return result;
	}

	public static bool IsBonusMerit(BonusParam.BonusType type, float value)
	{
		bool result = false;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
		case BonusParam.BonusType.RING:
		case BonusParam.BonusType.ANIMAL:
		case BonusParam.BonusType.DISTANCE:
		case BonusParam.BonusType.ENEMY_OBJBREAK:
		case BonusParam.BonusType.TOTAL_SCORE:
			if (value >= 0f)
			{
				result = true;
			}
			break;
		case BonusParam.BonusType.SPEED:
			if (value < 100f)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static bool IsBonusMeritByOrgValue(BonusParam.BonusType type, float orgValue)
	{
		bool result = false;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
		case BonusParam.BonusType.RING:
		case BonusParam.BonusType.ANIMAL:
		case BonusParam.BonusType.DISTANCE:
		case BonusParam.BonusType.ENEMY_OBJBREAK:
		case BonusParam.BonusType.TOTAL_SCORE:
			if (orgValue >= 0f)
			{
				result = true;
			}
			break;
		case BonusParam.BonusType.SPEED:
			if (orgValue > 0f)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static float GetBonusParamValue(BonusParam.BonusType type, float orgValue, ref bool merit)
	{
		float result = 0f;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
		case BonusParam.BonusType.RING:
		case BonusParam.BonusType.ANIMAL:
		case BonusParam.BonusType.DISTANCE:
		case BonusParam.BonusType.ENEMY_OBJBREAK:
			result = orgValue;
			merit = (orgValue >= 0f);
			break;
		case BonusParam.BonusType.TOTAL_SCORE:
			result = orgValue * 100f;
			merit = (orgValue >= 0f);
			break;
		case BonusParam.BonusType.SPEED:
			result = 100f - orgValue;
			merit = (orgValue > 0f);
			break;
		}
		return result;
	}

	public static float GetBonusParamValue(BonusParam.BonusType type, float orgValue)
	{
		float result = 0f;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
		case BonusParam.BonusType.RING:
		case BonusParam.BonusType.ANIMAL:
		case BonusParam.BonusType.DISTANCE:
		case BonusParam.BonusType.ENEMY_OBJBREAK:
			result = orgValue;
			break;
		case BonusParam.BonusType.TOTAL_SCORE:
			result = orgValue * 100f;
			break;
		case BonusParam.BonusType.SPEED:
			result = 100f - orgValue;
			break;
		}
		return result;
	}

	public static string GetBonusParamText(BonusParam.BonusType type, float orgValue)
	{
		string result = string.Empty;
		bool merit = false;
		float bonusParamValue = GetBonusParamValue(type, orgValue, ref merit);
		string text = string.Empty;
		switch (type)
		{
		case BonusParam.BonusType.SCORE:
		case BonusParam.BonusType.RING:
		case BonusParam.BonusType.ANIMAL:
		case BonusParam.BonusType.DISTANCE:
		case BonusParam.BonusType.ENEMY_OBJBREAK:
		case BonusParam.BonusType.TOTAL_SCORE:
			if (bonusParamValue != 0f)
			{
				text = ((!merit) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "bonus", "info_type_down_" + type).text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "bonus", "info_type_up_" + type).text);
			}
			break;
		case BonusParam.BonusType.SPEED:
			if (bonusParamValue != 100f)
			{
				text = ((!merit) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "bonus", "info_type_up_" + type).text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "bonus", "info_type_down_" + type).text);
			}
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			result = text.Replace("{PARAM}", bonusParamValue.ToString());
		}
		return result;
	}

	public static bool GetTeamBonus(CharaType type, out Dictionary<BonusParam.BonusType, float> bonusParam)
	{
		bonusParam = new Dictionary<BonusParam.BonusType, float>();
		if (type != CharaType.UNKNOWN && type != CharaType.NUM && CharacterDataNameInfo.Instance != null)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(type);
			if (dataByID != null)
			{
				TeamAttribute teamAttribute = CharaTypeUtil.GetTeamAttribute(type);
				float num = 0f;
				TeamAttributeCategory teamAttributeCategory = TeamAttributeCategory.NONE;
				switch (dataByID.m_teamAttributeCategory)
				{
				case TeamAttributeCategory.SCORE:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.SCORE);
					bonusParam.Add(BonusParam.BonusType.SCORE, num);
					break;
				case TeamAttributeCategory.RING:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.RING);
					bonusParam.Add(BonusParam.BonusType.RING, num);
					break;
				case TeamAttributeCategory.ANIMAL:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.ANIMAL);
					bonusParam.Add(BonusParam.BonusType.ANIMAL, num);
					break;
				case TeamAttributeCategory.DISTANCE:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.DISTANCE);
					bonusParam.Add(BonusParam.BonusType.DISTANCE, num);
					break;
				case TeamAttributeCategory.ENEMY_OBJBREAK:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.ENEMY);
					bonusParam.Add(BonusParam.BonusType.ENEMY_OBJBREAK, num);
					break;
				case TeamAttributeCategory.EASY_SPEED:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.SPEED);
					bonusParam.Add(BonusParam.BonusType.SPEED, num);
					if (GetTeamDemritBonus(teamAttribute) != 0f)
					{
						num = GetTeamDemritBonus(teamAttribute);
						bonusParam.Add(BonusParam.BonusType.TOTAL_SCORE, num);
					}
					break;
				case TeamAttributeCategory.DISTANCE_ANIMAL:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.DISTANCE);
					bonusParam.Add(BonusParam.BonusType.DISTANCE, num);
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.ANIMAL);
					bonusParam.Add(BonusParam.BonusType.ANIMAL, num);
					break;
				case TeamAttributeCategory.LOW_SPEED_SCORE:
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.SCORE);
					bonusParam.Add(BonusParam.BonusType.SCORE, num);
					num = dataByID.GetTeamAttributeValue(TeamAttributeBonusType.SPEED);
					bonusParam.Add(BonusParam.BonusType.SPEED, num);
					break;
				}
			}
		}
		return bonusParam.Count > 0;
	}

	private static void ResetUserBelongings()
	{
		if (s_charaList != null)
		{
			s_charaList.Clear();
			s_charaList = null;
		}
		if (s_chaoList != null)
		{
			s_chaoList.Clear();
			s_chaoList = null;
		}
	}

	private static void SetupUserBelongings()
	{
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			int num = 29;
			if (s_charaList != null)
			{
				s_charaList.Clear();
				s_charaList = null;
			}
			for (int i = 0; i < num; i++)
			{
				ServerCharacterState serverCharacterState = playerState.CharacterState((CharaType)i);
				if (serverCharacterState != null && serverCharacterState.IsUnlocked)
				{
					if (s_charaList == null)
					{
						s_charaList = new List<CharaType>();
						s_charaList.Add((CharaType)i);
					}
					else
					{
						s_charaList.Add((CharaType)i);
					}
				}
			}
		}
		if (s_chaoList != null)
		{
			s_chaoList.Clear();
			s_chaoList = null;
		}
		s_chaoList = ChaoTable.GetPossessionChaoData();
	}

	public static BonusParamContainer GetCurrentBonusData(CharaType charaMainType, CharaType charaSubType, int chaoMainId, int chaoSubId)
	{
		BonusParamContainer bonusParamContainer = null;
		if (charaMainType != CharaType.UNKNOWN && charaMainType != CharaType.NUM)
		{
			SetupUserBelongings();
			bonusParamContainer = new BonusParamContainer();
			BonusParam bonusParam = null;
			bonusParam = GetCurrentBonusParam(charaMainType, charaSubType, chaoMainId, chaoSubId);
			if (bonusParam != null)
			{
				bonusParamContainer.addBonus(bonusParam);
			}
			if (charaSubType != CharaType.UNKNOWN && charaSubType != CharaType.NUM)
			{
				bonusParam = GetCurrentBonusParam(charaSubType, charaMainType, chaoMainId, chaoSubId);
				if (bonusParam != null)
				{
					bonusParamContainer.addBonus(bonusParam);
				}
			}
		}
		return bonusParamContainer;
	}

	private static BonusParam GetCurrentBonusParam(CharaType charaMainType, CharaType charaSubType, int chaoMainId, int chaoSubId)
	{
		BonusParam bonusParam = null;
		if (charaMainType != CharaType.UNKNOWN && charaMainType != CharaType.NUM)
		{
			if (s_chaoList == null || s_charaList == null)
			{
				SetupUserBelongings();
			}
			if (s_charaList != null && s_charaList.Count > 0)
			{
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < s_charaList.Count; i++)
				{
					if (s_charaList[i] == charaMainType)
					{
						num = i;
						if (charaSubType == CharaType.UNKNOWN || charaSubType == CharaType.NUM || num2 != -1)
						{
							break;
						}
					}
					if (s_charaList[i] == charaSubType)
					{
						num2 = i;
						if (num != -1)
						{
							break;
						}
					}
				}
				if (num >= 0)
				{
					ServerPlayerState playerState = ServerInterface.PlayerState;
					if (playerState != null)
					{
						ServerCharacterState serverCharacterState = null;
						ServerCharacterState charaSubState = null;
						serverCharacterState = playerState.CharacterState(s_charaList[num]);
						if (num2 >= 0)
						{
							charaSubState = playerState.CharacterState(s_charaList[num2]);
						}
						if (serverCharacterState != null)
						{
							bonusParam = new BonusParam();
							bonusParam.AddBonusChara(serverCharacterState, charaMainType, charaSubState, charaSubType);
						}
					}
				}
				if (bonusParam != null)
				{
					ChaoData chaoData = null;
					ChaoData chaoData2 = null;
					if (s_chaoList != null && s_chaoList.Count > 0 && (chaoMainId >= 0 || chaoSubId >= 0))
					{
						foreach (ChaoData s_chao in s_chaoList)
						{
							if (s_chao.id == chaoMainId)
							{
								chaoData = s_chao;
								if (chaoData2 != null || chaoSubId < 0)
								{
									break;
								}
							}
							else if (s_chao.id == chaoSubId)
							{
								chaoData2 = s_chao;
								if (chaoData != null || chaoMainId < 0)
								{
									break;
								}
							}
						}
					}
					if (chaoData != null || chaoData2 != null)
					{
						if (chaoData2 == null)
						{
							bonusParam.AddBonusChao(chaoData);
						}
						else if (chaoData == null)
						{
							bonusParam.AddBonusChao(chaoData2);
						}
						else
						{
							bonusParam.AddBonusChao(chaoData, chaoData2);
						}
					}
				}
			}
		}
		return bonusParam;
	}
}
