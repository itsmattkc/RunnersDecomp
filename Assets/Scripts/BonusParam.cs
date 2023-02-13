using DataTable;
using System.Collections.Generic;

public class BonusParam
{
	public enum BonusTarget
	{
		CHARA,
		CHAO_MAIN,
		CHAO_SUB,
		ALL
	}

	public enum BonusType
	{
		SCORE,
		RING,
		ANIMAL,
		DISTANCE,
		ENEMY_OBJBREAK,
		TOTAL_SCORE,
		SPEED,
		NONE
	}

	public enum BonusAffinity
	{
		NONE,
		GOOD
	}

	private Dictionary<BonusTarget, List<float>> m_bonusData;

	private Dictionary<BonusTarget, CharacterAttribute> m_attribute;

	public Dictionary<BonusTarget, List<float>> orgBonusData
	{
		get
		{
			return m_bonusData;
		}
	}

	public BonusParam()
	{
		Reset();
	}

	public static Dictionary<BonusTarget, List<float>> GetBonusDataTotal(Dictionary<BonusTarget, List<float>> orgDataA, Dictionary<BonusTarget, List<float>> orgDataB)
	{
		Dictionary<BonusTarget, List<float>> result = null;
		Dictionary<BonusTarget, List<float>> dictionary = null;
		if (orgDataA != null && orgDataA.Count > 0 && orgDataB != null && orgDataB.Count > 0)
		{
			result = new Dictionary<BonusTarget, List<float>>();
			Dictionary<BonusTarget, List<float>>.KeyCollection keys = orgDataA.Keys;
			{
				foreach (BonusTarget item in keys)
				{
					List<float> list = new List<float>();
					if (orgDataA[item] != null && orgDataB[item] != null)
					{
						for (int i = 0; i < orgDataA[item].Count; i++)
						{
							if (i == 5)
							{
								float totalScoreBonus = BonusUtil.GetTotalScoreBonus(orgDataA[item][i], orgDataB[item][i]);
								list.Add(totalScoreBonus);
							}
							else
							{
								list.Add(orgDataA[item][i] + orgDataB[item][i]);
							}
						}
					}
					result.Add(item, list);
				}
				return result;
			}
		}
		if (orgDataA != null && orgDataA.Count > 0)
		{
			result = new Dictionary<BonusTarget, List<float>>();
			dictionary = orgDataA;
			Dictionary<BonusTarget, List<float>>.KeyCollection keys2 = dictionary.Keys;
			{
				foreach (BonusTarget item2 in keys2)
				{
					List<float> list2 = new List<float>();
					if (dictionary[item2] != null)
					{
						for (int j = 0; j < dictionary[item2].Count; j++)
						{
							list2.Add(dictionary[item2][j]);
						}
					}
					result.Add(item2, list2);
				}
				return result;
			}
		}
		if (orgDataB != null && orgDataB.Count > 0)
		{
			result = new Dictionary<BonusTarget, List<float>>();
			dictionary = orgDataB;
			Dictionary<BonusTarget, List<float>>.KeyCollection keys3 = dictionary.Keys;
			{
				foreach (BonusTarget item3 in keys3)
				{
					List<float> list3 = new List<float>();
					if (dictionary[item3] != null)
					{
						for (int k = 0; k < dictionary[item3].Count; k++)
						{
							list3.Add(dictionary[item3][k]);
						}
					}
					result.Add(item3, list3);
				}
				return result;
			}
		}
		return result;
	}

	public void Reset()
	{
		if (m_bonusData != null)
		{
			Dictionary<BonusTarget, List<float>>.KeyCollection keys = m_bonusData.Keys;
			foreach (BonusTarget item in keys)
			{
				if (m_bonusData[item] != null)
				{
					m_bonusData[item].Clear();
				}
			}
			m_bonusData.Clear();
		}
		if (m_attribute != null)
		{
			m_attribute.Clear();
		}
		m_bonusData = new Dictionary<BonusTarget, List<float>>();
		m_attribute = new Dictionary<BonusTarget, CharacterAttribute>();
	}

	private void SetBonusChao(ChaoData chaoData, BonusTarget target, CharacterAttribute charaAtribute)
	{
		if (chaoData == null || chaoData.chaoAbilitys == null || chaoData.chaoAbilitys.Length <= 0)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float item = 0f;
		float item2 = 0f;
		for (int i = 0; i < chaoData.chaoAbilitys.Length; i++)
		{
			ChaoAbility chaoAbility = chaoData.chaoAbilitys[i];
			float num6 = 0f;
			num6 = ((charaAtribute != chaoData.charaAtribute) ? chaoData.abilityValue[chaoData.level] : chaoData.bonusAbilityValue[chaoData.level]);
			switch (chaoAbility)
			{
			case ChaoAbility.ALL_BONUS_COUNT:
				num += num6;
				num2 += num6;
				num3 += num6;
				num4 += num6;
				break;
			case ChaoAbility.SCORE_COUNT:
				num += num6;
				break;
			case ChaoAbility.RING_COUNT:
				num2 += num6;
				break;
			case ChaoAbility.ANIMAL_COUNT:
				num3 += num6;
				break;
			case ChaoAbility.RUNNIGN_DISTANCE:
				num4 += num6;
				break;
			case ChaoAbility.ENEMY_SCORE:
				num5 += num6;
				break;
			}
		}
		List<float> list = new List<float>();
		list.Add(num);
		list.Add(num2);
		list.Add(num3);
		list.Add(num4);
		list.Add(num5);
		list.Add(item);
		list.Add(item2);
		if (!m_bonusData.ContainsKey(target))
		{
			m_bonusData.Add(target, list);
			m_attribute.Add(target, chaoData.charaAtribute);
			return;
		}
		m_bonusData[target] = list;
		if (m_attribute.ContainsKey(target))
		{
			m_attribute[target] = chaoData.charaAtribute;
		}
	}

	public void AddBonusChao(ChaoData chaoDataMain, ChaoData chaoDataSub = null)
	{
		CharacterAttribute characterAttribute = CharacterAttribute.UNKNOWN;
		if (m_attribute != null && m_attribute.ContainsKey(BonusTarget.CHARA))
		{
			characterAttribute = m_attribute[BonusTarget.CHARA];
			SetBonusChao(chaoDataMain, BonusTarget.CHAO_MAIN, characterAttribute);
			SetBonusChao(chaoDataSub, BonusTarget.CHAO_SUB, characterAttribute);
		}
	}

	public void AddBonusChara(ServerCharacterState charaMainState, CharaType mainType, ServerCharacterState charaSubState, CharaType subType)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		Dictionary<BonusType, float> bonusParam;
		if (BonusUtil.GetTeamBonus(mainType, out bonusParam) && bonusParam != null)
		{
			Dictionary<BonusType, float>.KeyCollection keys = bonusParam.Keys;
			foreach (BonusType item in keys)
			{
				switch (item)
				{
				case BonusType.SCORE:
					num += bonusParam[item];
					break;
				case BonusType.RING:
					num2 += bonusParam[item];
					break;
				case BonusType.ANIMAL:
					num3 += bonusParam[item];
					break;
				case BonusType.DISTANCE:
					num4 += bonusParam[item];
					break;
				case BonusType.ENEMY_OBJBREAK:
					num5 += bonusParam[item];
					break;
				case BonusType.TOTAL_SCORE:
					num6 += bonusParam[item];
					break;
				case BonusType.SPEED:
					num7 += bonusParam[item];
					break;
				default:
					Debug.Log(" not bonus team !");
					break;
				}
			}
		}
		if (subType != CharaType.UNKNOWN && subType != CharaType.NUM && BonusUtil.GetTeamBonus(subType, out bonusParam) && bonusParam != null)
		{
			Dictionary<BonusType, float>.KeyCollection keys2 = bonusParam.Keys;
			foreach (BonusType item2 in keys2)
			{
				switch (item2)
				{
				case BonusType.SCORE:
					num += bonusParam[item2];
					break;
				case BonusType.RING:
					num2 += bonusParam[item2];
					break;
				case BonusType.ANIMAL:
					num3 += bonusParam[item2];
					break;
				case BonusType.DISTANCE:
					num4 += bonusParam[item2];
					break;
				case BonusType.ENEMY_OBJBREAK:
					num5 += bonusParam[item2];
					break;
				case BonusType.TOTAL_SCORE:
					num6 = BonusUtil.GetTotalScoreBonus(num6, bonusParam[item2]);
					break;
				case BonusType.SPEED:
					num7 += bonusParam[item2];
					break;
				default:
					Debug.Log(" not bonus team !");
					break;
				}
			}
		}
		ImportAbilityTable instance2 = ImportAbilityTable.GetInstance();
		if (instance2 != null)
		{
			num2 += instance2.GetAbilityPotential(AbilityType.RING_BONUS, charaMainState.AbilityLevel[8]);
			num3 += instance2.GetAbilityPotential(AbilityType.ANIMAL, charaMainState.AbilityLevel[10]);
			num4 += instance2.GetAbilityPotential(AbilityType.DISTANCE_BONUS, charaMainState.AbilityLevel[9]);
			Dictionary<BonusType, float> starBonusList = charaMainState.GetStarBonusList();
			if (starBonusList != null)
			{
				Dictionary<BonusType, float>.KeyCollection keys3 = starBonusList.Keys;
				foreach (BonusType item3 in keys3)
				{
					switch (item3)
					{
					case BonusType.SCORE:
						num += starBonusList[item3];
						break;
					case BonusType.RING:
						num2 += starBonusList[item3];
						break;
					case BonusType.ANIMAL:
						num3 += starBonusList[item3];
						break;
					case BonusType.DISTANCE:
						num4 += starBonusList[item3];
						break;
					case BonusType.ENEMY_OBJBREAK:
						num5 += starBonusList[item3];
						break;
					case BonusType.SPEED:
						num7 += starBonusList[item3];
						break;
					case BonusType.TOTAL_SCORE:
						num6 += starBonusList[item3];
						break;
					}
				}
			}
			if (charaSubState != null)
			{
				num2 += instance2.GetAbilityPotential(AbilityType.RING_BONUS, charaSubState.AbilityLevel[8]);
				num3 += instance2.GetAbilityPotential(AbilityType.ANIMAL, charaSubState.AbilityLevel[10]);
				num4 += instance2.GetAbilityPotential(AbilityType.DISTANCE_BONUS, charaSubState.AbilityLevel[9]);
				Dictionary<BonusType, float> starBonusList2 = charaSubState.GetStarBonusList();
				if (starBonusList2 != null)
				{
					Dictionary<BonusType, float>.KeyCollection keys4 = starBonusList2.Keys;
					foreach (BonusType item4 in keys4)
					{
						switch (item4)
						{
						case BonusType.SCORE:
							num += starBonusList2[item4];
							break;
						case BonusType.RING:
							num2 += starBonusList2[item4];
							break;
						case BonusType.ANIMAL:
							num3 += starBonusList2[item4];
							break;
						case BonusType.DISTANCE:
							num4 += starBonusList2[item4];
							break;
						case BonusType.ENEMY_OBJBREAK:
							num5 += starBonusList2[item4];
							break;
						case BonusType.SPEED:
							num7 += starBonusList2[item4];
							break;
						case BonusType.TOTAL_SCORE:
							num6 += starBonusList2[item4];
							break;
						}
					}
				}
			}
		}
		List<float> list = new List<float>();
		list.Add(num);
		list.Add(num2);
		list.Add(num3);
		list.Add(num4);
		list.Add(num5);
		list.Add(num6);
		list.Add(num7);
		if (!m_bonusData.ContainsKey(BonusTarget.CHARA))
		{
			m_bonusData.Add(BonusTarget.CHARA, list);
			m_attribute.Add(BonusTarget.CHARA, CharaTypeUtil.GetCharacterAttribute(mainType));
			return;
		}
		m_bonusData[BonusTarget.CHARA] = list;
		if (m_attribute.ContainsKey(BonusTarget.CHARA))
		{
			m_attribute[BonusTarget.CHARA] = CharaTypeUtil.GetCharacterAttribute(mainType);
		}
	}

	public BonusAffinity GetBonusAffinity(BonusTarget target)
	{
		BonusAffinity result = BonusAffinity.NONE;
		if (m_attribute.ContainsKey(BonusTarget.CHARA) && target != BonusTarget.ALL && target != 0)
		{
			CharacterAttribute characterAttribute = m_attribute[BonusTarget.CHARA];
			if (m_attribute.ContainsKey(target) && m_attribute[target] == characterAttribute)
			{
				result = BonusAffinity.GOOD;
			}
		}
		return result;
	}

	public bool IsDetailInfo(out string detailText)
	{
		detailText = GetDetailInfoText(m_bonusData);
		return !string.IsNullOrEmpty(detailText);
	}

	public static string GetDetailInfoText(Dictionary<BonusTarget, List<float>> orgBonusData)
	{
		if (orgBonusData == null || orgBonusData.Count == 0)
		{
			return string.Empty;
		}
		string text = string.Empty;
		Dictionary<BonusTarget, List<float>>.KeyCollection keys = orgBonusData.Keys;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (BonusTarget item in keys)
		{
			if (item != 0)
			{
				continue;
			}
			List<float> list3 = orgBonusData[item];
			for (int i = 0; i < list3.Count; i++)
			{
				BonusType bonusType = (BonusType)i;
				float orgValue = list3[i];
				switch (bonusType)
				{
				case BonusType.TOTAL_SCORE:
					if (!BonusUtil.IsBonusMeritByOrgValue(bonusType, orgValue))
					{
						string bonusParamText = BonusUtil.GetBonusParamText(bonusType, orgValue);
						if (!string.IsNullOrEmpty(bonusParamText))
						{
							list2.Add(BonusUtil.GetBonusParamText(bonusType, orgValue));
						}
					}
					break;
				case BonusType.SPEED:
				{
					string bonusParamText = BonusUtil.GetBonusParamText(bonusType, orgValue);
					if (!string.IsNullOrEmpty(bonusParamText))
					{
						if (BonusUtil.IsBonusMeritByOrgValue(bonusType, orgValue))
						{
							list.Add(BonusUtil.GetBonusParamText(bonusType, orgValue));
						}
						else
						{
							list2.Add(BonusUtil.GetBonusParamText(bonusType, orgValue));
						}
					}
					break;
				}
				}
			}
		}
		if (list.Count > 0)
		{
			foreach (string item2 in list)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += item2;
			}
		}
		if (list2.Count > 0)
		{
			foreach (string item3 in list2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += item3;
			}
			return text;
		}
		return text;
	}

	public Dictionary<BonusType, float> GetBonusInfo(BonusTarget target = BonusTarget.ALL, bool offsetUse = true)
	{
		Dictionary<BonusType, float> dictionary = new Dictionary<BonusType, float>();
		if (target == BonusTarget.ALL)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			Dictionary<BonusTarget, List<float>>.KeyCollection keys = m_bonusData.Keys;
			foreach (BonusTarget item in keys)
			{
				if (m_bonusData[item] != null && m_bonusData[item].Count > 0)
				{
					num += m_bonusData[item][0];
					num2 += m_bonusData[item][1];
					num3 += m_bonusData[item][2];
					num4 += m_bonusData[item][3];
					num5 += m_bonusData[item][4];
					num7 += m_bonusData[item][6];
					if (num6 == 0f)
					{
						num6 = m_bonusData[item][5];
					}
					else if (m_bonusData[item][5] != 0f)
					{
						num6 = BonusUtil.GetTotalScoreBonus(num6, m_bonusData[item][5]);
					}
				}
			}
			if (offsetUse)
			{
				dictionary.Add(BonusType.SCORE, BonusUtil.GetBonusParamValue(BonusType.SCORE, num));
				dictionary.Add(BonusType.RING, BonusUtil.GetBonusParamValue(BonusType.RING, num2));
				dictionary.Add(BonusType.ANIMAL, BonusUtil.GetBonusParamValue(BonusType.ANIMAL, num3));
				dictionary.Add(BonusType.DISTANCE, BonusUtil.GetBonusParamValue(BonusType.DISTANCE, num4));
				dictionary.Add(BonusType.ENEMY_OBJBREAK, BonusUtil.GetBonusParamValue(BonusType.ENEMY_OBJBREAK, num5));
				dictionary.Add(BonusType.SPEED, BonusUtil.GetBonusParamValue(BonusType.SPEED, num7));
				dictionary.Add(BonusType.TOTAL_SCORE, BonusUtil.GetBonusParamValue(BonusType.TOTAL_SCORE, num6));
			}
			else
			{
				dictionary.Add(BonusType.SCORE, num);
				dictionary.Add(BonusType.RING, num2);
				dictionary.Add(BonusType.ANIMAL, num3);
				dictionary.Add(BonusType.DISTANCE, num4);
				dictionary.Add(BonusType.ENEMY_OBJBREAK, num5);
				dictionary.Add(BonusType.SPEED, num7);
				dictionary.Add(BonusType.TOTAL_SCORE, num6);
			}
		}
		else if (m_bonusData != null && m_bonusData.ContainsKey(target))
		{
			if (offsetUse)
			{
				dictionary.Add(BonusType.SCORE, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][0]));
				dictionary.Add(BonusType.RING, BonusUtil.GetBonusParamValue(BonusType.RING, m_bonusData[target][1]));
				dictionary.Add(BonusType.ANIMAL, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][2]));
				dictionary.Add(BonusType.DISTANCE, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][3]));
				dictionary.Add(BonusType.ENEMY_OBJBREAK, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][4]));
				dictionary.Add(BonusType.SPEED, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][6]));
				dictionary.Add(BonusType.TOTAL_SCORE, BonusUtil.GetBonusParamValue(BonusType.SCORE, m_bonusData[target][5]));
			}
			else
			{
				dictionary.Add(BonusType.SCORE, m_bonusData[target][0]);
				dictionary.Add(BonusType.RING, m_bonusData[target][1]);
				dictionary.Add(BonusType.ANIMAL, m_bonusData[target][2]);
				dictionary.Add(BonusType.DISTANCE, m_bonusData[target][3]);
				dictionary.Add(BonusType.ENEMY_OBJBREAK, m_bonusData[target][4]);
				dictionary.Add(BonusType.SPEED, m_bonusData[target][6]);
				dictionary.Add(BonusType.TOTAL_SCORE, m_bonusData[target][5]);
			}
		}
		else if (offsetUse)
		{
			dictionary.Add(BonusType.SCORE, BonusUtil.GetBonusParamValue(BonusType.SCORE, 0f));
			dictionary.Add(BonusType.RING, BonusUtil.GetBonusParamValue(BonusType.RING, 0f));
			dictionary.Add(BonusType.ANIMAL, BonusUtil.GetBonusParamValue(BonusType.ANIMAL, 0f));
			dictionary.Add(BonusType.DISTANCE, BonusUtil.GetBonusParamValue(BonusType.DISTANCE, 0f));
			dictionary.Add(BonusType.ENEMY_OBJBREAK, BonusUtil.GetBonusParamValue(BonusType.ENEMY_OBJBREAK, 0f));
			dictionary.Add(BonusType.SPEED, BonusUtil.GetBonusParamValue(BonusType.SPEED, 0f));
			dictionary.Add(BonusType.TOTAL_SCORE, BonusUtil.GetBonusParamValue(BonusType.TOTAL_SCORE, 0f));
		}
		else
		{
			dictionary.Add(BonusType.SCORE, 0f);
			dictionary.Add(BonusType.RING, 0f);
			dictionary.Add(BonusType.ANIMAL, 0f);
			dictionary.Add(BonusType.DISTANCE, 0f);
			dictionary.Add(BonusType.ENEMY_OBJBREAK, 0f);
			dictionary.Add(BonusType.SPEED, 0f);
			dictionary.Add(BonusType.TOTAL_SCORE, 0f);
		}
		return dictionary;
	}

	public Dictionary<BonusType, float> GetBonusInfo(BonusTarget targetA, BonusTarget targetB, bool offsetUse = true)
	{
		Dictionary<BonusType, float> dictionary = new Dictionary<BonusType, float>();
		if (targetA != BonusTarget.ALL && targetB != BonusTarget.ALL && targetA != targetB)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			Dictionary<BonusTarget, List<float>>.KeyCollection keys = m_bonusData.Keys;
			foreach (BonusTarget item in keys)
			{
				if (m_bonusData[item] != null && m_bonusData[item].Count > 0 && (item == targetA || item == targetB))
				{
					num += m_bonusData[item][0];
					num2 += m_bonusData[item][1];
					num3 += m_bonusData[item][2];
					num4 += m_bonusData[item][3];
					num5 += m_bonusData[item][4];
					num7 += m_bonusData[item][6];
					if (num6 == 0f)
					{
						num6 = m_bonusData[item][5];
					}
					else if (m_bonusData[item][5] != 0f)
					{
						num6 = BonusUtil.GetTotalScoreBonus(num6, m_bonusData[item][5]);
					}
				}
			}
			if (offsetUse)
			{
				dictionary.Add(BonusType.SCORE, BonusUtil.GetBonusParamValue(BonusType.SCORE, num));
				dictionary.Add(BonusType.RING, BonusUtil.GetBonusParamValue(BonusType.RING, num2));
				dictionary.Add(BonusType.ANIMAL, BonusUtil.GetBonusParamValue(BonusType.ANIMAL, num3));
				dictionary.Add(BonusType.DISTANCE, BonusUtil.GetBonusParamValue(BonusType.DISTANCE, num4));
				dictionary.Add(BonusType.ENEMY_OBJBREAK, BonusUtil.GetBonusParamValue(BonusType.ENEMY_OBJBREAK, num5));
				dictionary.Add(BonusType.SPEED, BonusUtil.GetBonusParamValue(BonusType.SPEED, num7));
				dictionary.Add(BonusType.TOTAL_SCORE, BonusUtil.GetBonusParamValue(BonusType.TOTAL_SCORE, num6));
			}
			else
			{
				dictionary.Add(BonusType.SCORE, num);
				dictionary.Add(BonusType.RING, num2);
				dictionary.Add(BonusType.ANIMAL, num3);
				dictionary.Add(BonusType.DISTANCE, num4);
				dictionary.Add(BonusType.ENEMY_OBJBREAK, num5);
				dictionary.Add(BonusType.SPEED, num7);
				dictionary.Add(BonusType.TOTAL_SCORE, num6);
			}
		}
		return dictionary;
	}
}
