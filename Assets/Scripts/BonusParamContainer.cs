using System.Collections.Generic;

public class BonusParamContainer
{
	private List<BonusParam> m_paramList;

	private int m_currentTargetIndex = -1;

	public void Reset()
	{
		if (m_paramList != null)
		{
			foreach (BonusParam param in m_paramList)
			{
				param.Reset();
			}
			m_paramList.Clear();
		}
		m_paramList = null;
	}

	public void addBonus(BonusParam bonusParam)
	{
		if (m_paramList == null)
		{
			m_paramList = new List<BonusParam>();
		}
		m_paramList.Add(bonusParam);
	}

	public BonusParam GetBonusParam(int index)
	{
		BonusParam result = null;
		if (m_paramList != null && index >= 0 && m_paramList.Count > index)
		{
			result = m_paramList[index];
		}
		return result;
	}

	public Dictionary<BonusParam.BonusTarget, List<float>> GetBonusParamOrgData(int index)
	{
		Dictionary<BonusParam.BonusTarget, List<float>> result = null;
		if (m_paramList != null && index >= 0 && m_paramList.Count > index)
		{
			result = m_paramList[index].orgBonusData;
		}
		return result;
	}

	public bool IsDetailInfo(out string detailText)
	{
		detailText = string.Empty;
		int num = m_currentTargetIndex;
		if (num < 0)
		{
			num = 0;
		}
		Dictionary<BonusParam.BonusTarget, List<float>> bonusParamOrgData = GetBonusParamOrgData(num);
		if (bonusParamOrgData != null)
		{
			detailText = BonusParam.GetDetailInfoText(bonusParamOrgData);
		}
		return !string.IsNullOrEmpty(detailText);
	}

	public Dictionary<BonusParam.BonusType, float> GetBonusInfo(int index = -1)
	{
		Dictionary<BonusParam.BonusType, float> dictionary = null;
		List<Dictionary<BonusParam.BonusType, float>> list = new List<Dictionary<BonusParam.BonusType, float>>();
		List<Dictionary<BonusParam.BonusType, float>> list2 = new List<Dictionary<BonusParam.BonusType, float>>();
		m_currentTargetIndex = index;
		if (index < 0)
		{
			if (m_paramList != null)
			{
				foreach (BonusParam param in m_paramList)
				{
					list.Add(param.GetBonusInfo(BonusParam.BonusTarget.CHAO_MAIN, BonusParam.BonusTarget.CHAO_SUB, false));
					list2.Add(param.GetBonusInfo(BonusParam.BonusTarget.CHARA, false));
				}
			}
		}
		else if (m_paramList.Count > index)
		{
			return m_paramList[index].GetBonusInfo();
		}
		if (list2.Count > 0)
		{
			dictionary = new Dictionary<BonusParam.BonusType, float>();
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			int num8 = 0;
			foreach (Dictionary<BonusParam.BonusType, float> item in list)
			{
				if (num8 == 0 || num < item[BonusParam.BonusType.SCORE])
				{
					num = item[BonusParam.BonusType.SCORE];
				}
				if (num8 == 0 || num2 < item[BonusParam.BonusType.RING])
				{
					num2 = item[BonusParam.BonusType.RING];
				}
				if (num8 == 0 || num3 < item[BonusParam.BonusType.ANIMAL])
				{
					num3 = item[BonusParam.BonusType.ANIMAL];
				}
				if (num8 == 0 || num4 < item[BonusParam.BonusType.DISTANCE])
				{
					num4 = item[BonusParam.BonusType.DISTANCE];
				}
				if (num8 == 0 || num5 < item[BonusParam.BonusType.ENEMY_OBJBREAK])
				{
					num5 = item[BonusParam.BonusType.ENEMY_OBJBREAK];
				}
				if (num8 == 0 || num7 < item[BonusParam.BonusType.SPEED])
				{
					num7 = item[BonusParam.BonusType.SPEED];
				}
				if (num8 == 0 || num6 < item[BonusParam.BonusType.TOTAL_SCORE])
				{
					num6 = item[BonusParam.BonusType.TOTAL_SCORE];
				}
				num8++;
			}
			num += list2[0][BonusParam.BonusType.SCORE];
			num2 += list2[0][BonusParam.BonusType.RING];
			num3 += list2[0][BonusParam.BonusType.ANIMAL];
			num4 += list2[0][BonusParam.BonusType.DISTANCE];
			num5 += list2[0][BonusParam.BonusType.ENEMY_OBJBREAK];
			num7 += list2[0][BonusParam.BonusType.SPEED];
			if (num6 == 0f)
			{
				num6 = list2[0][BonusParam.BonusType.TOTAL_SCORE];
			}
			else if (list2[0][BonusParam.BonusType.TOTAL_SCORE] != 0f)
			{
				num6 = BonusUtil.GetTotalScoreBonus(num6, list2[0][BonusParam.BonusType.TOTAL_SCORE]);
			}
			dictionary.Add(BonusParam.BonusType.SCORE, BonusUtil.GetBonusParamValue(BonusParam.BonusType.SCORE, num));
			dictionary.Add(BonusParam.BonusType.RING, BonusUtil.GetBonusParamValue(BonusParam.BonusType.RING, num2));
			dictionary.Add(BonusParam.BonusType.ANIMAL, BonusUtil.GetBonusParamValue(BonusParam.BonusType.ANIMAL, num3));
			dictionary.Add(BonusParam.BonusType.DISTANCE, BonusUtil.GetBonusParamValue(BonusParam.BonusType.DISTANCE, num4));
			dictionary.Add(BonusParam.BonusType.ENEMY_OBJBREAK, BonusUtil.GetBonusParamValue(BonusParam.BonusType.ENEMY_OBJBREAK, num5));
			dictionary.Add(BonusParam.BonusType.SPEED, BonusUtil.GetBonusParamValue(BonusParam.BonusType.SPEED, num7));
			dictionary.Add(BonusParam.BonusType.TOTAL_SCORE, BonusUtil.GetBonusParamValue(BonusParam.BonusType.TOTAL_SCORE, num6));
		}
		return dictionary;
	}
}
