using AnimationOrTween;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RaidBossDamageRewardWindow : WindowBase
{
	private enum BUTTON_ACT
	{
		CLOSE,
		INFO,
		NONE
	}

	private const float UPDATE_TIME = 0.25f;

	[SerializeField]
	private UIPanel mainPanel;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIDraggablePanel m_listPanel;

	[SerializeField]
	private GameObject m_topInfo;

	[SerializeField]
	private GameObject m_topReward;

	[SerializeField]
	private UILabel m_topRewardItem;

	[SerializeField]
	private UILabel m_headerLabel;

	private bool m_close;

	private BUTTON_ACT m_btnAct = BUTTON_ACT.NONE;

	private RaidBossData m_bossData;

	private UIRectItemStorage m_storage;

	private ui_event_raid_scroll m_bossObject;

	private ui_damage_reward_scroll m_playerObject;

	private RaidBossWindow m_parent;

	private bool m_useResult;

	private static RaidBossDamageRewardWindow s_instance;

	public bool useResult
	{
		get
		{
			return m_useResult;
		}
		set
		{
			m_useResult = value;
		}
	}

	private static RaidBossDamageRewardWindow Instance
	{
		get
		{
			return s_instance;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void CallbackRaidBossDataUpdate(RaidBossData data)
	{
		m_bossData = data;
		if (m_bossData == null)
		{
			return;
		}
		m_listPanel.enabled = true;
		m_storage = m_listPanel.GetComponentInChildren<UIRectItemStorage>();
		if (m_headerLabel != null)
		{
			if (m_bossData.end && m_bossData.clear)
			{
				m_headerLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_raid_reward").text;
			}
			else
			{
				m_headerLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_raid_damage").text;
			}
		}
		if (m_topRewardItem != null && m_bossData.end && m_bossData.clear)
		{
			string rewardText = m_bossData.GetRewardText();
			if (!string.IsNullOrEmpty(rewardText))
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "raid_reward_text").text;
				m_topRewardItem.text = TextUtility.Replaces(text, new Dictionary<string, string>
				{
					{
						"{PARAM}",
						rewardText
					}
				});
			}
		}
		if (m_bossData.end && m_bossData.clear)
		{
			m_topInfo.SetActive(false);
			m_topReward.SetActive(true);
			UIRectItemStorage[] componentsInChildren = m_topReward.GetComponentsInChildren<UIRectItemStorage>();
			if (componentsInChildren != null)
			{
				UIRectItemStorage[] array = componentsInChildren;
				foreach (UIRectItemStorage uIRectItemStorage in array)
				{
					uIRectItemStorage.Restart();
				}
			}
			m_bossObject = m_topReward.GetComponentInChildren<ui_event_raid_scroll>();
			m_playerObject = null;
		}
		else
		{
			m_topInfo.SetActive(true);
			m_topReward.SetActive(false);
			UIRectItemStorage[] componentsInChildren2 = m_topInfo.GetComponentsInChildren<UIRectItemStorage>();
			if (componentsInChildren2 != null)
			{
				UIRectItemStorage[] array2 = componentsInChildren2;
				foreach (UIRectItemStorage uIRectItemStorage2 in array2)
				{
					uIRectItemStorage2.Restart();
				}
			}
			m_bossObject = m_topInfo.GetComponentInChildren<ui_event_raid_scroll>();
			m_playerObject = m_topInfo.GetComponentInChildren<ui_damage_reward_scroll>();
			m_playerObject.SetClickCollision(!m_useResult);
		}
		int num = 0;
		long num2 = 0L;
		if (m_bossData.listData != null && m_bossData.listData.Count > 0)
		{
			num = m_bossData.listData.Count;
			foreach (RaidBossUser listDatum in m_bossData.listData)
			{
				if (num2 < listDatum.damage)
				{
					num2 = listDatum.damage;
				}
			}
		}
		m_storage.maxItemCount = (m_storage.maxRows = num);
		m_storage.Restart();
		if (m_storage != null)
		{
			ui_damage_reward_scroll[] componentsInChildren3 = m_storage.GetComponentsInChildren<ui_damage_reward_scroll>();
			if (componentsInChildren3 != null)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					if (num > k)
					{
						if (m_bossData.listData[k].damage == num2)
						{
							m_bossData.listData[k].rankIndex = 0;
						}
						else
						{
							m_bossData.listData[k].rankIndex = 1;
						}
						componentsInChildren3[k].UpdateView(m_bossData.listData[k], m_bossData);
						componentsInChildren3[k].SetClickCollision(!m_useResult);
						if (m_bossData.myData != null && m_bossData.myData.id == m_bossData.listData[k].id)
						{
							componentsInChildren3[k].SetMyRanker(true);
						}
					}
					else
					{
						UnityEngine.Object.Destroy(componentsInChildren3[k].gameObject);
					}
				}
			}
		}
		if (m_bossObject != null)
		{
			m_bossObject.UpdateView(m_bossData, null);
		}
		if (m_playerObject != null)
		{
			if (m_bossData.myData != null)
			{
				if (m_bossData.myData.damage == num2)
				{
					m_bossData.myData.rankIndex = 0;
				}
				else
				{
					m_bossData.myData.rankIndex = 1;
				}
				m_playerObject.UpdateView(m_bossData.myData, m_bossData);
				m_playerObject.SetMyRanker(true);
			}
			else
			{
				UnityEngine.Object.Destroy(m_playerObject.gameObject);
				m_playerObject = null;
			}
		}
		if (m_listPanel != null)
		{
			m_listPanel.ResetPosition();
		}
	}

	public void Setup(RaidBossData data, RaidBossWindow parent)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		m_parent = parent;
		mainPanel.alpha = 1f;
		m_close = false;
		m_btnAct = BUTTON_ACT.NONE;
		m_useResult = false;
		if (m_storage != null)
		{
			m_storage.maxItemCount = (m_storage.maxRows = 0);
			m_storage.Restart();
		}
		if (data != null)
		{
			m_bossData = data;
			m_topInfo.SetActive(false);
			m_topReward.SetActive(false);
			if (m_headerLabel != null)
			{
				if (m_bossData.end && m_bossData.clear)
				{
					m_headerLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_raid_reward").text;
				}
				else
				{
					m_headerLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_raid_damage").text;
				}
			}
			if (m_topRewardItem != null && m_bossData.end && m_bossData.clear)
			{
				string rewardText = m_bossData.GetRewardText();
				if (!string.IsNullOrEmpty(rewardText))
				{
					string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "raid_reward_text").text;
					m_topRewardItem.text = text.Replace("{PARAM}", rewardText);
				}
				else
				{
					m_topRewardItem.text = string.Empty;
				}
			}
		}
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
	}

	public void OnClickClose()
	{
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			BUTTON_ACT btnAct = m_btnAct;
			if (btnAct == BUTTON_ACT.INFO)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (m_parent != null)
			{
				TimeSpan timeLimit = m_bossData.GetTimeLimit();
				if (m_bossData.end || timeLimit.Ticks <= 0)
				{
					RaidBossInfo.currentRaidData = null;
					m_parent.RequestServerGetEventUserRaidBossList();
				}
			}
			base.gameObject.SetActive(false);
		}
		else
		{
			CallbackRaidBossDataUpdate(m_bossData);
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (!ranking_window.isActive)
		{
			if (msg != null)
			{
				msg.StaySequence();
			}
			OnClickClose();
		}
	}

	public static bool IsEnabled()
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.gameObject.activeSelf;
		}
		return result;
	}

	public static RaidBossDamageRewardWindow Create(RaidBossData data, RaidBossWindow parent = null)
	{
		if (s_instance != null)
		{
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(data, parent);
			return s_instance;
		}
		return null;
	}

	private void Awake()
	{
		SetInstance();
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
