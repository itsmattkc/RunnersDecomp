using Message;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ui_event_raid_scroll : MonoBehaviour
{
	private const float UPDATE_TIME = 0.25f;

	private const float RELOAD_DELAY_TIME = 1f;

	[SerializeField]
	private UISprite m_bossIcon;

	[SerializeField]
	private UISprite m_bossRarity;

	[SerializeField]
	private UILabel m_bossName;

	[SerializeField]
	private UILabel m_bossNameSh;

	[SerializeField]
	private UILabel m_bossLv;

	[SerializeField]
	private UILabel m_bossLife;

	[SerializeField]
	private UISlider m_bossLifeBar;

	[SerializeField]
	private UILabel m_discoverer;

	[SerializeField]
	private UILabel m_limit;

	[SerializeField]
	private GameObject m_rightsideBtns;

	[SerializeField]
	private GameObject m_rightsideBtn0;

	[SerializeField]
	private GameObject m_rightsideBtn1;

	[SerializeField]
	private GameObject m_rightsideBtn2;

	private RaidBossWindow m_parent;

	private RaidBossData m_bossData;

	private bool m_infoWindow;

	private float m_targetFrameTime = 0.0166666675f;

	private float m_reloadDelay;

	private float m_timeCounter = 0.25f;

	private void Start()
	{
		m_timeCounter = 0.25f;
	}

	private void Update()
	{
		m_timeCounter -= m_targetFrameTime;
		if (m_timeCounter <= 0f)
		{
			PeriodicUpdate();
			m_timeCounter = 0.25f;
		}
		if (!(m_reloadDelay > 0f) || !m_infoWindow || !(m_parent != null))
		{
			return;
		}
		m_reloadDelay -= Time.deltaTime;
		if (!(m_reloadDelay <= 0f))
		{
			return;
		}
		if (m_rightsideBtn0 != null)
		{
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_rightsideBtn0, "Btn_log");
			UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_rightsideBtn0, "Btn_play");
			if (uIImageButton != null)
			{
				uIImageButton.isEnabled = false;
			}
			if (uIImageButton2 != null)
			{
				uIImageButton2.isEnabled = false;
			}
		}
		if (m_parent.isLoading || !GeneralUtil.IsNetwork())
		{
			m_reloadDelay = 1f;
			Debug.Log("ui_event_raid_scroll reload retry!  IsNetwork:" + GeneralUtil.IsNetwork());
		}
		else
		{
			RaidBossInfo.currentRaidData = null;
			m_parent.RequestServerGetEventUserRaidBossList();
			m_reloadDelay = 0f;
		}
	}

	private void PeriodicUpdate()
	{
		if (m_bossData != null)
		{
			m_limit.text = m_bossData.GetTimeLimitString(true);
			if (m_reloadDelay <= 0f && m_infoWindow && m_parent != null && !m_bossData.end && m_bossData.GetTimeLimit().Ticks <= 0)
			{
				m_reloadDelay = 1f;
			}
		}
	}

	public void UpdateView(RaidBossData bossData, RaidBossWindow parent, bool infoWindow = false)
	{
		m_parent = parent;
		m_bossData = bossData;
		m_timeCounter = 0.25f;
		m_targetFrameTime = 1f / (float)Application.targetFrameRate;
		m_infoWindow = infoWindow;
		m_reloadDelay = 0f;
		if (m_bossData != null)
		{
			if (m_bossName != null && m_bossNameSh != null)
			{
				UILabel bossName = m_bossName;
				string name = m_bossData.name;
				m_bossNameSh.text = name;
				bossName.text = name;
			}
			if (m_discoverer != null)
			{
				if (m_bossData.IsDiscoverer())
				{
					ServerSettingState settingState = ServerInterface.SettingState;
					if (settingState != null)
					{
						m_discoverer.text = settingState.m_userName;
					}
					else
					{
						m_discoverer.text = m_bossData.discoverer;
					}
				}
				else
				{
					m_discoverer.text = m_bossData.discoverer;
				}
			}
			if (m_bossLife != null)
			{
				m_bossLife.text = string.Format("{0}/{1}", m_bossData.hp, m_bossData.hpMax);
			}
			if (m_bossLifeBar != null)
			{
				m_bossLifeBar.value = m_bossData.GetHpRate();
				m_bossLifeBar.numberOfSteps = 1;
				m_bossLifeBar.ForceUpdate();
			}
			if (m_bossLv != null)
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
				m_bossLv.text = TextUtility.Replace(text, "{PARAM}", m_bossData.lv.ToString());
			}
			if (m_bossIcon != null && m_bossRarity != null)
			{
				m_bossIcon.spriteName = "ui_gp_gauge_boss_icon_raid_" + m_bossData.rarity;
				if (m_bossData.rarity >= 2)
				{
					m_bossRarity.spriteName = "ui_event_raidboss_window_bosslevel_2";
					m_bossRarity.enabled = true;
				}
				else if (m_bossData.rarity >= 1)
				{
					m_bossRarity.spriteName = "ui_event_raidboss_window_bosslevel_1";
					m_bossRarity.enabled = true;
				}
				else
				{
					m_bossRarity.spriteName = "ui_event_raidboss_window_bosslevel_0";
					m_bossRarity.enabled = true;
				}
			}
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "bar_base");
			if (uISprite != null)
			{
				if (m_bossData.IsDiscoverer())
				{
					uISprite.spriteName = "ui_event_raidboss_window_boss_bar_1";
				}
				else
				{
					uISprite.spriteName = "ui_event_raidboss_window_boss_bar_0";
				}
			}
		}
		PeriodicUpdate();
		UpdateBtn(infoWindow);
	}

	private void UpdateBtn(bool infoWindow = false)
	{
		if (m_bossData == null)
		{
			return;
		}
		if (infoWindow)
		{
			bool flag = false;
			if (!(m_rightsideBtns != null))
			{
				return;
			}
			m_rightsideBtns.SetActive(true);
			if (m_rightsideBtn0 != null)
			{
				if (!m_bossData.end)
				{
					m_rightsideBtn0.SetActive(true);
					UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_rightsideBtn0, "Btn_log");
					UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_rightsideBtn0, "Btn_play");
					if (uIImageButton != null && m_bossData != null)
					{
						if (m_bossData.IsDiscoverer() && !m_bossData.participation)
						{
							uIImageButton.isEnabled = false;
						}
						else
						{
							uIImageButton.isEnabled = true;
						}
					}
					if (uIImageButton2 != null && m_bossData != null)
					{
						uIImageButton2.isEnabled = true;
					}
					flag = true;
				}
				else
				{
					m_rightsideBtn0.SetActive(false);
				}
			}
			if (m_rightsideBtn1 != null)
			{
				if (!flag)
				{
					if (m_bossData.end && m_bossData.clear)
					{
						m_rightsideBtn1.SetActive(true);
						flag = true;
					}
					else
					{
						m_rightsideBtn1.SetActive(false);
					}
				}
				else
				{
					m_rightsideBtn1.SetActive(false);
				}
			}
			if (!(m_rightsideBtn2 != null))
			{
				return;
			}
			if (!flag)
			{
				if ((m_bossData.participation || m_bossData.IsDiscoverer()) && m_bossData.end && !m_bossData.clear)
				{
					m_rightsideBtn2.SetActive(true);
					flag = true;
				}
				else
				{
					m_rightsideBtn2.SetActive(false);
				}
			}
			else
			{
				m_rightsideBtn2.SetActive(false);
			}
		}
		else if (m_rightsideBtns != null)
		{
			m_rightsideBtns.SetActive(false);
			if (m_rightsideBtn0 != null)
			{
				m_rightsideBtn0.SetActive(false);
			}
			if (m_rightsideBtn1 != null)
			{
				m_rightsideBtn1.SetActive(false);
			}
			if (m_rightsideBtn2 != null)
			{
				m_rightsideBtn2.SetActive(false);
			}
		}
	}

	private void OnClickPlay()
	{
		if (m_bossData == null || !(m_parent != null))
		{
			return;
		}
		if (GeneralUtil.IsNetwork())
		{
			TimeSpan timeLimit = m_bossData.GetTimeLimit();
			if (timeLimit.TotalSeconds > 0.5 || timeLimit.TotalSeconds < -0.25)
			{
				m_parent.OnClickBossPlayButton(m_bossData);
			}
		}
		else
		{
			m_parent.ShowNoCommunication();
		}
	}

	private void OnClickInfo()
	{
		if (m_bossData == null || !(m_parent != null))
		{
			return;
		}
		if (GeneralUtil.IsNetwork())
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				int eventId = -1;
				if (EventManager.Instance != null)
				{
					eventId = EventManager.Instance.Id;
				}
				loggedInServerInterface.RequestServerGetEventRaidBossUserList(eventId, m_bossData.id, base.gameObject);
			}
			else
			{
				ServerGetEventRaidBossUserList_Succeeded(null);
			}
		}
		else
		{
			m_parent.ShowNoCommunication();
		}
	}

	private void ServerGetEventRaidBossUserList_Succeeded(MsgGetEventRaidBossUserListSucceed msg)
	{
		if (m_bossData == null || !(EventManager.Instance != null))
		{
			return;
		}
		RaidBossInfo raidBossInfo = EventManager.Instance.RaidBossInfo;
		if (raidBossInfo != null)
		{
			List<RaidBossData> raidData = raidBossInfo.raidData;
			foreach (RaidBossData item in raidData)
			{
				if (item != null && item.id == m_bossData.id)
				{
					m_bossData = item;
					break;
				}
			}
		}
		if (m_bossData.end && m_bossData.clear)
		{
			m_parent.OnClickBossRewardButton(m_bossData);
			SetMessageBoxConnect();
		}
		else
		{
			m_parent.OnClickBossInfoButton(m_bossData);
		}
	}

	private void ServerGetEventRaidBossUserList_Failed(MsgGetEventRaidBossUserListSucceed msg)
	{
		m_parent.OnClickBossRewardButton(m_bossData);
	}

	private void SetMessageBoxConnect()
	{
		if (SaveDataManager.Instance != null && SaveDataManager.Instance.ConnectData != null)
		{
			SaveDataManager.Instance.ConnectData.ReplaceMessageBox = true;
		}
	}
}
