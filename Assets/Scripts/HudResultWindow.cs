using System;
using UnityEngine;

public class HudResultWindow : MonoBehaviour
{
	private enum Type
	{
		CHAO_1,
		CHAO_2,
		CHAO_COUNT,
		CAMPAIGN,
		CHARA_1,
		CHARA_2,
		MILEAGE,
		NUM
	}

	[Serializable]
	private class ScoreInfo
	{
		[SerializeField]
		public UILabel m_score;

		[SerializeField]
		public UILabel m_ring;

		[SerializeField]
		public UILabel m_animal;

		[SerializeField]
		public UILabel m_distance;
	}

	[Serializable]
	private class TexInfo
	{
		[SerializeField]
		public UITexture m_scoreTex;

		[SerializeField]
		public UITexture m_ringTex;

		[SerializeField]
		public UITexture m_animalTex;

		[SerializeField]
		public UITexture m_distanceTex;
	}

	[Serializable]
	private class SprInfo
	{
		[SerializeField]
		public UISprite m_scoreTex;

		[SerializeField]
		public UISprite m_ringTex;

		[SerializeField]
		public UISprite m_animalTex;

		[SerializeField]
		public UISprite m_distanceTex;
	}

	[SerializeField]
	private ScoreInfo[] m_scoreInfos = new ScoreInfo[7];

	[SerializeField]
	private TexInfo m_chao1TexInfos = new TexInfo();

	[SerializeField]
	private TexInfo m_chao2TexInfos = new TexInfo();

	[SerializeField]
	private SprInfo m_chara1TexInfos = new SprInfo();

	[SerializeField]
	private SprInfo m_chara2TexInfos = new SprInfo();

	[SerializeField]
	private UILabel m_totalScore;

	private GameObject m_result;

	public void Setup(GameObject result, bool bossResult)
	{
		m_result = result;
		StageScoreManager instance = StageScoreManager.Instance;
		if (instance == null)
		{
			return;
		}
		SaveDataManager instance2 = SaveDataManager.Instance;
		if (instance2 == null)
		{
			return;
		}
		StageAbilityManager instance3 = StageAbilityManager.Instance;
		if (!(instance3 == null))
		{
			SetScore(instance.BonusCountMainChaoData, instance3.MainChaoBonusValueRate, m_scoreInfos[0], bossResult);
			SetChaoTexture(m_chao1TexInfos, instance2.PlayerData.MainChaoID, instance3.MainChaoBonusValueRate, bossResult);
			SetScore(instance.BonusCountSubChaoData, instance3.SubChaoBonusValueRate, m_scoreInfos[1], bossResult);
			SetChaoTexture(m_chao2TexInfos, instance2.PlayerData.SubChaoID, instance3.SubChaoBonusValueRate, bossResult);
			SetScore(instance.BonusCountChaoCountData, instance3.CountChaoBonusValueRate, m_scoreInfos[2], bossResult);
			SetText(m_scoreInfos[2].m_ring, -1L);
			SetText(m_scoreInfos[2].m_animal, -1L);
			SetText(m_scoreInfos[2].m_distance, -1L);
			SetScore(instance.BonusCountCampaignData, GameResultUtility.GetCampaignBonusRate(instance3), m_scoreInfos[3], bossResult);
			SetText(m_scoreInfos[3].m_score, -1L);
			SetText(m_scoreInfos[3].m_animal, -1L);
			SetText(m_scoreInfos[3].m_distance, -1L);
			SetScore(instance.BonusCountMainCharaData, instance3.MainCharaBonusValueRate, m_scoreInfos[4], bossResult);
			SetCharaTexture(m_chara1TexInfos, instance2.PlayerData.MainChara, instance3.MainCharaBonusValueRate, bossResult);
			SetScore(instance.BonusCountSubCharaData, instance3.SubCharaBonusValueRate, m_scoreInfos[5], bossResult);
			SetCharaTexture(m_chara2TexInfos, instance2.PlayerData.SubChara, instance3.SubCharaBonusValueRate, bossResult);
			SetScore(instance.MileageBonusScoreData, instance3.MileageBonusScoreRate, m_scoreInfos[6], bossResult);
			if (bossResult)
			{
				SetText(m_scoreInfos[6].m_ring, -1L);
				SetText(m_totalScore, -1L);
			}
			else
			{
				UILabel totalScore = m_totalScore;
				StageAbilityManager.BonusRate mileageBonusScoreRate = instance3.MileageBonusScoreRate;
				SetText(totalScore, (!(mileageBonusScoreRate.final_score > 0f)) ? (-1) : instance.MileageBonusScoreData.final_score);
			}
		}
	}

	private void OnClickNoButton()
	{
		SoundManager.SePlay("sys_window_close");
		if (m_result != null)
		{
			m_result.SendMessage("OnClickDetailsEndButton");
		}
	}

	private void SetScore(StageScoreManager.ResultData resultData, StageAbilityManager.BonusRate bonusRate, ScoreInfo scoreInfo, bool bossResult)
	{
		if (resultData != null && scoreInfo != null)
		{
			SetText(scoreInfo.m_ring, (!(bonusRate.ring > 0f)) ? (-1) : resultData.ring);
			if (bossResult)
			{
				SetText(scoreInfo.m_score, -1L);
				SetText(scoreInfo.m_animal, -1L);
				SetText(scoreInfo.m_distance, -1L);
			}
			else
			{
				SetText(scoreInfo.m_score, (!(bonusRate.score > 0f)) ? (-1) : resultData.score);
				SetText(scoreInfo.m_animal, (!(bonusRate.animal > 0f)) ? (-1) : resultData.animal);
				SetText(scoreInfo.m_distance, (!(bonusRate.distance > 0f)) ? (-1) : resultData.distance);
			}
		}
	}

	private void SetText(UILabel label, long score)
	{
		if (!(label == null))
		{
			if (score >= 0)
			{
				label.text = GameResultUtility.GetScoreFormat(score);
				label.gameObject.SetActive(true);
			}
			else
			{
				label.gameObject.SetActive(false);
			}
		}
	}

	private void SetChaoTexture(TexInfo texInfo, int chaoId, StageAbilityManager.BonusRate bonusRate, bool bossResult)
	{
		HudUtility.SetChaoTexture(texInfo.m_ringTex, (!(bonusRate.ring > 0f)) ? (-1) : chaoId, true);
		if (bossResult)
		{
			HudUtility.SetChaoTexture(texInfo.m_scoreTex, -1, true);
			HudUtility.SetChaoTexture(texInfo.m_animalTex, -1, true);
			HudUtility.SetChaoTexture(texInfo.m_distanceTex, -1, true);
		}
		else
		{
			HudUtility.SetChaoTexture(texInfo.m_scoreTex, (!(bonusRate.score > 0f)) ? (-1) : chaoId, true);
			HudUtility.SetChaoTexture(texInfo.m_animalTex, (!(bonusRate.animal > 0f)) ? (-1) : chaoId, true);
			HudUtility.SetChaoTexture(texInfo.m_distanceTex, (!(bonusRate.distance > 0f)) ? (-1) : chaoId, true);
		}
	}

	private void SetCharaTexture(SprInfo texInfo, CharaType charaType, StageAbilityManager.BonusRate bonusRate, bool bossResult)
	{
		GameResultUtility.SetCharaTexture(texInfo.m_ringTex, (!(bonusRate.ring > 0f)) ? CharaType.UNKNOWN : charaType);
		if (bossResult)
		{
			GameResultUtility.SetCharaTexture(texInfo.m_scoreTex, CharaType.UNKNOWN);
			GameResultUtility.SetCharaTexture(texInfo.m_animalTex, CharaType.UNKNOWN);
			GameResultUtility.SetCharaTexture(texInfo.m_distanceTex, CharaType.UNKNOWN);
		}
		else
		{
			GameResultUtility.SetCharaTexture(texInfo.m_scoreTex, (!(bonusRate.score > 0f)) ? CharaType.UNKNOWN : charaType);
			GameResultUtility.SetCharaTexture(texInfo.m_animalTex, (!(bonusRate.animal > 0f)) ? CharaType.UNKNOWN : charaType);
			GameResultUtility.SetCharaTexture(texInfo.m_distanceTex, (!(bonusRate.distance > 0f)) ? CharaType.UNKNOWN : charaType);
		}
	}
}
