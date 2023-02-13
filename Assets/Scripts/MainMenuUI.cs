using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	private MainMenuDeckTab m_deckTab;

	private HudEpisodeButton m_episodeButton;

	private HudEpisodeBanner m_episodeBanner;

	private HudCampaignBanner m_quickCampainBaner;

	private HudCampaignBanner m_endlessCampainBaner;

	private HudQuickModeStagePicture m_quickModeStagePicture;

	private HudMainMenuRankingButton m_endlessModeRankingButton;

	private HudMainMenuRankingButton m_quickModeRankingButton;

	private HudDailyBattleButton m_dailyBattleButton;

	private HudQuestionButton m_quickModeQuestionButton;

	private HudQuestionButton m_endlessModeQuestionButton;

	public void UpdateView()
	{
		if (m_episodeBanner != null)
		{
			m_episodeBanner.UpdateView();
		}
		if (m_deckTab != null)
		{
			m_deckTab.UpdateView();
		}
		if (m_dailyBattleButton != null)
		{
			m_dailyBattleButton.UpdateView();
		}
		if (m_quickCampainBaner != null)
		{
			m_quickCampainBaner.UpdateView();
		}
		if (m_endlessCampainBaner != null)
		{
			m_endlessCampainBaner.UpdateView();
		}
	}

	private void Start()
	{
		if (m_deckTab == null)
		{
			m_deckTab = base.gameObject.AddComponent<MainMenuDeckTab>();
		}
		if (m_episodeButton == null)
		{
			m_episodeButton = new HudEpisodeButton();
			bool isBossStage = MileageMapUtility.IsBossStage();
			CharacterAttribute characterAttribute = CharacterAttribute.SPEED;
			int mileageStageIndex = MileageMapDataManager.Instance.MileageStageIndex;
			CharacterAttribute[] characterAttribute2 = MileageMapUtility.GetCharacterAttribute(mileageStageIndex);
			characterAttribute = characterAttribute2[0];
			m_episodeButton.Initialize(base.gameObject, isBossStage, characterAttribute);
		}
		if (m_episodeBanner == null)
		{
			m_episodeBanner = new HudEpisodeBanner();
			m_episodeBanner.Initialize(base.gameObject);
		}
		if (m_quickCampainBaner == null)
		{
			m_quickCampainBaner = base.gameObject.AddComponent<HudCampaignBanner>();
			m_quickCampainBaner.Initialize(base.gameObject, true);
		}
		if (m_endlessCampainBaner == null)
		{
			m_endlessCampainBaner = base.gameObject.AddComponent<HudCampaignBanner>();
			m_endlessCampainBaner.Initialize(base.gameObject, false);
		}
		if (m_quickModeStagePicture == null)
		{
			m_quickModeStagePicture = new HudQuickModeStagePicture();
			m_quickModeStagePicture.Initialize(base.gameObject);
		}
		if (m_endlessModeRankingButton == null)
		{
			m_endlessModeRankingButton = new HudMainMenuRankingButton();
			m_endlessModeRankingButton.Intialize(base.gameObject, false);
		}
		if (m_quickModeRankingButton == null)
		{
			m_quickModeRankingButton = new HudMainMenuRankingButton();
			m_quickModeRankingButton.Intialize(base.gameObject, true);
		}
		if (m_dailyBattleButton == null)
		{
			m_dailyBattleButton = new HudDailyBattleButton();
			m_dailyBattleButton.Initialize(base.gameObject);
		}
		if (m_quickModeQuestionButton == null)
		{
			m_quickModeQuestionButton = base.gameObject.AddComponent<HudQuestionButton>();
			m_quickModeQuestionButton.Initialize(true);
		}
		if (m_endlessModeQuestionButton == null)
		{
			m_endlessModeQuestionButton = base.gameObject.AddComponent<HudQuestionButton>();
			m_endlessModeQuestionButton.Initialize(false);
		}
		HudMenuUtility.SendChangeHeaderText("ui_Header_MainPage2");
		BackKeyManager.AddMainMenuUI(base.gameObject);
	}

	private void Update()
	{
		if (m_endlessModeRankingButton != null)
		{
			m_endlessModeRankingButton.Update();
		}
		if (m_quickModeRankingButton != null)
		{
			m_quickModeRankingButton.Update();
		}
		if (m_dailyBattleButton != null)
		{
			m_dailyBattleButton.Update();
		}
	}

	private void OnUpdateQuickModeData()
	{
		if (m_quickModeStagePicture != null)
		{
			m_quickModeStagePicture.UpdateDisplay();
		}
		if (m_quickCampainBaner != null)
		{
			m_quickCampainBaner.UpdateView();
		}
		if (m_endlessCampainBaner != null)
		{
			m_endlessCampainBaner.UpdateView();
		}
	}
}
