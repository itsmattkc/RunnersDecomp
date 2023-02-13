using UnityEngine;

public class GameResultUtility
{
	private static long m_oldBestScore;

	private static bool m_isBossDestroy;

	private static int m_RaidbossBeatBonus;

	public static float ScoreInterpolateTime = 1.5f;

	public static Animation SearchAnimation(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<Animation>();
	}

	public static void SaveOldBestScore()
	{
		RankingUtil.RankingMode rankingMode = RankingUtil.RankingMode.ENDLESS;
		if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
		{
			rankingMode = RankingUtil.RankingMode.QUICK;
		}
		m_oldBestScore = RankingManager.GetMyHiScore(rankingMode, false);
		RankingManager.SavePlayerRankingData(rankingMode);
	}

	public static long GetOldBestScore()
	{
		return m_oldBestScore;
	}

	public static long GetNewBestScore()
	{
		StageScoreManager instance = StageScoreManager.Instance;
		long result = 0L;
		if (instance != null)
		{
			result = instance.FinalScore;
		}
		return result;
	}

	public static void SetBossDestroyFlag(bool flag)
	{
		m_isBossDestroy = flag;
	}

	public static bool GetBossDestroyFlag()
	{
		return m_isBossDestroy;
	}

	public static void SetRaidbossBeatBonus(int value)
	{
		m_RaidbossBeatBonus = value;
	}

	public static int GetRaidbossBeatBonus()
	{
		return m_RaidbossBeatBonus;
	}

	public static void SetActiveBonus(GameObject parentAnimObject, string labelName, float score)
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parentAnimObject, labelName);
		if (!(uILabel == null) && score > 0f)
		{
			uILabel.text = string.Format("{0:0.0}", score) + "%";
			parentAnimObject.gameObject.SetActive(true);
		}
	}

	public static void SetCharaTexture(UISprite uiTex, CharaType charaType)
	{
		if (!(uiTex == null))
		{
			if (charaType != CharaType.UNKNOWN)
			{
				uiTex.spriteName = HudUtility.MakeCharaTextureName(charaType, HudUtility.TextureType.TYPE_S);
				uiTex.gameObject.SetActive(true);
			}
			else
			{
				uiTex.gameObject.SetActive(false);
			}
		}
	}

	public static StageAbilityManager.BonusRate GetCampaignBonusRate(StageAbilityManager stageAbilityManager)
	{
		StageAbilityManager.BonusRate result = default(StageAbilityManager.BonusRate);
		if (stageAbilityManager != null)
		{
			result.score = 0f;
			result.ring = stageAbilityManager.CampaignValueRate;
			result.red_ring = 0f;
			result.animal = 0f;
			result.distance = 0f;
		}
		return result;
	}

	public static string GetScoreFormat(long val)
	{
		return HudUtility.GetFormatNumString(val);
	}
}
