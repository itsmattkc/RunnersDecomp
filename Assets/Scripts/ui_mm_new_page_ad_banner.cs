using Message;
using Text;
using UnityEngine;

public class ui_mm_new_page_ad_banner : MonoBehaviour
{
	public class BannerInfo
	{
		public int index;

		public int type;

		public NetNoticeItem item;

		public InformationWindow.Information info;
	}

	[SerializeField]
	private UITexture m_uiTexture;

	[SerializeField]
	private GameObject m_badgeAlert;

	private EasySnsFeed m_easySnsFeed;

	private BannerInfo m_bannerInfo;

	private InformationWindow m_infoWindow;

	private bool m_buttonPressFlag;

	private bool m_init;

	private void Start()
	{
		base.enabled = false;
		UIButtonMessage component = base.gameObject.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.functionName = "OnClickScroll";
		}
	}

	public void Update()
	{
		if (m_easySnsFeed != null)
		{
			EasySnsFeed.Result result = m_easySnsFeed.Update();
			if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
			{
				m_easySnsFeed = null;
				base.enabled = false;
			}
		}
		if (!(m_infoWindow != null))
		{
			return;
		}
		if (m_buttonPressFlag)
		{
			if (m_infoWindow.IsEnd() && m_easySnsFeed == null)
			{
				base.enabled = false;
			}
		}
		else if (m_infoWindow.IsButtonPress(InformationWindow.ButtonType.LEFT))
		{
			ClickLeftButton();
			m_buttonPressFlag = true;
		}
		else if (m_infoWindow.IsButtonPress(InformationWindow.ButtonType.RIGHT))
		{
			ClickRightButton();
			m_buttonPressFlag = true;
		}
		else if (m_infoWindow.IsButtonPress(InformationWindow.ButtonType.CLOSE))
		{
			ClickCloseButton();
			m_buttonPressFlag = true;
		}
	}

	private void CreateEasySnsFeed()
	{
		if (m_bannerInfo != null)
		{
			m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/Anchor_5_MC", TextUtility.GetCommonText("ItemRoulette", "feed_jackpot_caption"), m_bannerInfo.info.bodyText);
		}
	}

	private void RequestGetEventList()
	{
		if (!(EventManager.Instance != null))
		{
			return;
		}
		if (EventManager.Instance.IsSetEventStateInfo)
		{
			RequestLoadEventResource();
			return;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventReward(EventManager.Instance.Id, base.gameObject);
		}
	}

	private void ClickLeftButton()
	{
		if (m_bannerInfo != null)
		{
			switch (m_bannerInfo.info.pattern)
			{
			case InformationWindow.ButtonPattern.FEED_BROWSER:
				Application.OpenURL(m_bannerInfo.info.url);
				break;
			case InformationWindow.ButtonPattern.FEED_ROULETTE:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO_ROULETTE);
				break;
			case InformationWindow.ButtonPattern.FEED_SHOP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				break;
			case InformationWindow.ButtonPattern.FEED_EVENT:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.PLAY_EVENT);
				break;
			case InformationWindow.ButtonPattern.FEED_EVENT_LIST:
				RequestGetEventList();
				break;
			}
		}
	}

	private void ClickRightButton()
	{
		if (m_bannerInfo != null)
		{
			switch (m_bannerInfo.info.pattern)
			{
			case InformationWindow.ButtonPattern.FEED_EVENT_LIST:
				break;
			case InformationWindow.ButtonPattern.SHOP_CANCEL:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				break;
			case InformationWindow.ButtonPattern.FEED:
				CreateEasySnsFeed();
				break;
			case InformationWindow.ButtonPattern.FEED_BROWSER:
				CreateEasySnsFeed();
				break;
			case InformationWindow.ButtonPattern.FEED_ROULETTE:
				CreateEasySnsFeed();
				break;
			case InformationWindow.ButtonPattern.FEED_SHOP:
				CreateEasySnsFeed();
				break;
			case InformationWindow.ButtonPattern.FEED_EVENT:
				CreateEasySnsFeed();
				break;
			case InformationWindow.ButtonPattern.BROWSER:
				Application.OpenURL(m_bannerInfo.info.url);
				break;
			case InformationWindow.ButtonPattern.ROULETTE:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO_ROULETTE);
				break;
			case InformationWindow.ButtonPattern.SHOP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				break;
			case InformationWindow.ButtonPattern.EVENT:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.PLAY_EVENT);
				break;
			case InformationWindow.ButtonPattern.EVENT_LIST:
				RequestGetEventList();
				break;
			}
		}
	}

	private void ClickCloseButton()
	{
	}

	private void SaveInformation()
	{
		if (m_bannerInfo != null && m_bannerInfo.item != null && !ServerInterface.NoticeInfo.IsCheckedForMenuIcon(m_bannerInfo.item))
		{
			ServerInterface.NoticeInfo.m_isShowedNoticeInfo = true;
			ServerInterface.NoticeInfo.UpdateChecked(m_bannerInfo.item);
			if (m_badgeAlert != null)
			{
				m_badgeAlert.SetActive(false);
			}
		}
	}

	public void UpdateView(BannerInfo bannerinfo)
	{
		m_bannerInfo = bannerinfo;
		if (m_bannerInfo != null)
		{
			switch (m_bannerInfo.info.rankingType)
			{
			case InformationWindow.RankingType.NON:
				SetInfoBanner();
				break;
			case InformationWindow.RankingType.LEAGUE:
				SetEndlessRankingBannerLeague();
				break;
			case InformationWindow.RankingType.WORLD:
				SetRankingBannerAll();
				break;
			case InformationWindow.RankingType.EVENT:
				SetInfoBanner();
				break;
			case InformationWindow.RankingType.QUICK_LEAGUE:
				SetQuickRankingBannerLeague();
				break;
			}
			if (m_bannerInfo != null && m_bannerInfo.item != null && !ServerInterface.NoticeInfo.IsCheckedForMenuIcon(m_bannerInfo.item) && m_badgeAlert != null)
			{
				m_badgeAlert.SetActive(true);
			}
			m_init = true;
		}
	}

	private void SetInfoBanner()
	{
		if (m_bannerInfo != null && !string.IsNullOrEmpty(m_bannerInfo.info.imageId))
		{
			InformationImageManager.Instance.Load(m_bannerInfo.info.imageId, true, OnLoadCallback);
		}
	}

	private bool SetRankingBannerAll()
	{
		bool result = false;
		if (m_uiTexture != null)
		{
			string name = "ui_tex_ranking_all";
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
				m_uiTexture.mainTexture = component.m_tex;
			}
		}
		return result;
	}

	private bool SetEndlessRankingBannerLeague()
	{
		bool result = false;
		if (m_uiTexture != null)
		{
			string name = "ui_tex_ranking_rival_endless_" + TextUtility.GetSuffixe();
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
				m_uiTexture.mainTexture = component.m_tex;
			}
		}
		return result;
	}

	private void SetQuickRankingBannerLeague()
	{
		if (m_uiTexture != null)
		{
			string name = "ui_tex_ranking_rival_quick_" + TextUtility.GetSuffixe();
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
				m_uiTexture.mainTexture = component.m_tex;
			}
		}
	}

	public void OnLoadCallback(Texture2D texture)
	{
		if (m_uiTexture != null && texture != null)
		{
			m_uiTexture.mainTexture = texture;
		}
	}

	public void UpdateTexture(Texture texture)
	{
		if (texture != null)
		{
			m_uiTexture.mainTexture = texture;
		}
	}

	private void OnClickScroll()
	{
		if (!m_init)
		{
			return;
		}
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "NewsWindow");
			if (gameObject != null)
			{
				SoundManager.SePlay("sys_menu_decide");
				if (m_bannerInfo != null)
				{
					base.enabled = true;
					m_buttonPressFlag = false;
					m_infoWindow = base.gameObject.GetComponent<InformationWindow>();
					if (m_infoWindow == null)
					{
						m_infoWindow = base.gameObject.AddComponent<InformationWindow>();
					}
					if (m_infoWindow != null)
					{
						m_infoWindow.Create(m_bannerInfo.info, gameObject);
					}
				}
			}
		}
		SaveInformation();
	}

	public void OnEndChildPage()
	{
	}

	public void OnButtonEventCallBack()
	{
		CreateEeventList();
	}

	private void CreateEeventList()
	{
		if (EventManager.Instance != null)
		{
			switch (EventManager.Instance.Type)
			{
			case EventManager.EventType.SPECIAL_STAGE:
				EventRewardWindow.Create(EventManager.Instance.SpecialStageInfo);
				break;
			case EventManager.EventType.RAID_BOSS:
				EventRewardWindow.Create(EventManager.Instance.RaidBossInfo);
				break;
			case EventManager.EventType.COLLECT_OBJECT:
				EventRewardWindow.Create(EventManager.Instance.EtcEventInfo);
				break;
			}
		}
	}

	private void RequestLoadEventResource()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnMenuEventClicked", base.gameObject, SendMessageOptions.DontRequireReceiver);
	}

	private void ServerGetEventReward_Succeeded(MsgGetEventRewardSucceed msg)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventState(EventManager.Instance.Id, base.gameObject);
		}
	}

	private void ServerGetEventState_Succeeded(MsgGetEventStateSucceed msg)
	{
		if (EventManager.Instance != null)
		{
			EventManager.Instance.SetEventInfo();
		}
		RequestLoadEventResource();
	}
}
