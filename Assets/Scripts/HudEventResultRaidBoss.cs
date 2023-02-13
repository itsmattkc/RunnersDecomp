using AnimationOrTween;
using Message;
using System.Collections.Generic;
using Text;
using UI;
using UnityEngine;

public class HudEventResultRaidBoss : HudEventResultParts
{
	private enum RaidResultState
	{
		INIT,
		IN_BG,
		WAIT_HELP_REQUEST,
		WAIT_HELP,
		WAIT_HELP_FAILURE,
		DAMAGE_WINDOW,
		OUT,
		END
	}

	private GameObject m_resultRootObject;

	private HudEventResult.AnimationEndCallback m_callback;

	private Animation m_animation;

	private Animation m_helpRequestAnimation;

	private HudEventResult.AnimType m_currentAnimType;

	private RaidBossInfo m_info;

	private GameResultScoreInterporate m_score;

	private GameResultScoreInterporate m_redRingScore;

	private UIImageButton m_DamageDetailsButton;

	private UIToggle m_helpRequestToggle;

	private RaidBossDamageRewardWindow m_raidBossDamageWindow;

	private RaidBosshelpRequestWindow m_helpRequestWindow;

	private bool m_isDamageDetailsWindowOpen;

	private bool m_isHelpRequestWindowOpen;

	private bool m_isHelpRequestIn;

	private bool m_isHelpRequestReady;

	private List<ServerEventRaidBossDesiredState> m_desiredList;

	private RaidResultState m_raidResultState;

	private bool m_isBackkeyEnable = true;

	public override bool IsBackkeyEnable()
	{
		return m_isBackkeyEnable;
	}

	public override void Init(GameObject resultRootObject, long beforeTotalPoint, HudEventResult.AnimationEndCallback callback)
	{
		Debug.Log("HudEventResultRaidBoss:Init");
		m_resultRootObject = resultRootObject;
		m_callback = callback;
		m_isDamageDetailsWindowOpen = false;
		m_isHelpRequestWindowOpen = false;
		m_isHelpRequestReady = false;
		m_isBackkeyEnable = true;
		m_info = EventManager.Instance.RaidBossInfo;
		if (m_info == null)
		{
			return;
		}
		m_animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "EventResult_Anim");
		if (m_animation == null)
		{
			return;
		}
		m_helpRequestAnimation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "help_request");
		if (m_helpRequestAnimation != null)
		{
			m_helpRequestAnimation.gameObject.SetActive(false);
		}
		m_DamageDetailsButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_resultRootObject, "Btn_damage_details");
		if (m_DamageDetailsButton != null)
		{
			m_DamageDetailsButton.isEnabled = false;
			UIButtonMessage uIButtonMessage = m_DamageDetailsButton.gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_DamageDetailsButton.gameObject.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.enabled = true;
				uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickDamageDetailsButton";
			}
			if (GameResultUtility.GetBossDestroyFlag())
			{
				string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_reward_get");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_DamageDetailsButton.gameObject, "Lbl_word_damage_details");
				if (uILabel != null)
				{
					uILabel.text = text;
					UILocalizeText component = uILabel.gameObject.GetComponent<UILocalizeText>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_DamageDetailsButton.gameObject, "Lbl_word_damage_details_sh");
				if (uILabel2 != null)
				{
					uILabel2.text = text;
					UILocalizeText component2 = uILabel2.gameObject.GetComponent<UILocalizeText>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
			}
		}
		m_helpRequestToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(m_resultRootObject, "img_check_box_0");
		if (m_helpRequestToggle != null)
		{
			UIButtonMessage uIButtonMessage2 = m_helpRequestToggle.gameObject.AddComponent<UIButtonMessage>();
			if (uIButtonMessage2 != null)
			{
				uIButtonMessage2.enabled = true;
				uIButtonMessage2.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "OnClickHelpRequestButton";
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_resultRootObject, "object_get");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	private bool isHelpRequest()
	{
		bool result = true;
		if (m_info != null && RaidBossInfo.currentRaidData != null && RaidBossInfo.currentRaidData.IsDiscoverer())
		{
			result = GameResultUtility.GetBossDestroyFlag();
			result |= RaidBossInfo.currentRaidData.crowded;
		}
		return result;
	}

	public override void PlayAnimation(HudEventResult.AnimType animType)
	{
		m_currentAnimType = animType;
		Debug.Log("HudEventResultRaidBoss:PlayAnimation >> " + m_currentAnimType);
		switch (animType)
		{
		case HudEventResult.AnimType.IDLE:
			break;
		case HudEventResult.AnimType.OUT_WAIT:
			break;
		case HudEventResult.AnimType.IN:
			m_isHelpRequestIn = false;
			if (!isHelpRequest())
			{
				if (m_helpRequestAnimation != null)
				{
					m_helpRequestAnimation.gameObject.SetActive(true);
					ActiveAnimation activeAnimation2 = ActiveAnimation.Play(m_helpRequestAnimation, "ui_EventResult_raidboss_help_request_intro_Anim", Direction.Forward, true);
					EventDelegate.Add(activeAnimation2.onFinished, AnimationFinishCallback, true);
					m_isHelpRequestIn = true;
				}
				else
				{
					Debug.Log("HudEventResultRaidBoss:PlayAnimation >> help request animation not found!!");
					m_callback(HudEventResult.AnimType.OUT_WAIT);
				}
			}
			else
			{
				m_callback(HudEventResult.AnimType.OUT_WAIT);
			}
			break;
		case HudEventResult.AnimType.IN_BONUS:
			AnimationFinishCallback();
			break;
		case HudEventResult.AnimType.WAIT_ADD_COLLECT_OBJECT:
			SetEnableDamageDetailsButton(true);
			break;
		case HudEventResult.AnimType.ADD_COLLECT_OBJECT:
			AnimationFinishCallback();
			break;
		case HudEventResult.AnimType.SHOW_QUOTA_LIST:
			AnimationFinishCallback();
			break;
		case HudEventResult.AnimType.OUT:
			if (m_isHelpRequestIn)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_helpRequestAnimation, "ui_EventResult_raidboss_help_request_outro_Anim", Direction.Forward, true);
				if (m_helpRequestToggle != null)
				{
					if (m_helpRequestToggle.value)
					{
						if (m_info != null && !m_isHelpRequestReady)
						{
							ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
							if (loggedInServerInterface != null)
							{
								List<string> list = new List<string>();
								SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
								if (socialInterface != null && socialInterface.IsLoggedIn)
								{
									List<SocialUserData> friendList = socialInterface.FriendList;
									foreach (SocialUserData item in friendList)
									{
										string gameId = item.CustomData.GameId;
										list.Add(gameId);
									}
								}
								loggedInServerInterface.RequestServerGetEventRaidBossDesiredList(EventManager.Instance.Id, RaidBossInfo.currentRaidData.id, list, base.gameObject);
								m_raidResultState = RaidResultState.WAIT_HELP_REQUEST;
								m_isHelpRequestReady = true;
							}
						}
					}
					else
					{
						m_raidResultState = RaidResultState.OUT;
						if (activeAnimation != null)
						{
							EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
						}
					}
				}
			}
			else
			{
				AnimationFinishCallback();
				m_raidResultState = RaidResultState.OUT;
			}
			if (m_raidBossDamageWindow != null)
			{
				m_raidBossDamageWindow.OnClickClose();
			}
			SetEnableDamageDetailsButton(false);
			break;
		}
	}

	private void Update()
	{
		switch (m_raidResultState)
		{
		case RaidResultState.WAIT_HELP:
			if (m_helpRequestWindow != null && m_isHelpRequestWindowOpen && m_helpRequestWindow.isFinished())
			{
				m_isHelpRequestWindowOpen = false;
				m_isHelpRequestIn = false;
				m_raidResultState = RaidResultState.OUT;
				AnimationFinishCallback();
			}
			break;
		case RaidResultState.WAIT_HELP_FAILURE:
			if (GeneralWindow.IsCreated("HelpRequestFailure") && GeneralWindow.IsButtonPressed)
			{
				m_isHelpRequestWindowOpen = false;
				m_isHelpRequestIn = false;
				m_raidResultState = RaidResultState.OUT;
				AnimationFinishCallback();
				GeneralWindow.Close();
			}
			break;
		}
		if (!m_isBackkeyEnable && m_raidBossDamageWindow != null && !RaidBossDamageRewardWindow.IsEnabled())
		{
			m_isBackkeyEnable = true;
		}
	}

	private void AnimationFinishCallback()
	{
		if (m_callback != null)
		{
			m_callback(m_currentAnimType);
		}
	}

	private void QuotaPlayEndCallback()
	{
		if (m_callback != null)
		{
			m_callback(m_currentAnimType);
		}
	}

	private void OnClickDamageDetailsButton()
	{
		Debug.Log("HudEventResultRaidBoss:OnClickDamageDetailsButton");
		SoundManager.SePlay("sys_menu_decide");
		if (m_info == null)
		{
			return;
		}
		if (!m_isDamageDetailsWindowOpen)
		{
			m_isDamageDetailsWindowOpen = true;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				int eventId = -1;
				if (EventManager.Instance != null)
				{
					eventId = EventManager.Instance.Id;
				}
				loggedInServerInterface.RequestServerGetEventRaidBossUserList(eventId, RaidBossInfo.currentRaidData.id, base.gameObject);
			}
			else
			{
				ServerGetEventRaidBossUserList_Succeeded(null);
			}
		}
		else
		{
			DamageDetailsWindowOpen();
		}
		m_isBackkeyEnable = false;
	}

	private void DamageDetailsWindowOpen()
	{
		m_raidBossDamageWindow = RaidBossDamageRewardWindow.Create(RaidBossInfo.currentRaidData);
		if (m_raidBossDamageWindow != null)
		{
			GameObject gameObject = m_raidBossDamageWindow.gameObject;
			if (gameObject != null)
			{
				Vector3 localPosition = gameObject.transform.localPosition;
				Vector3 localScale = gameObject.transform.localScale;
				gameObject.transform.parent = m_resultRootObject.transform;
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.localScale = localScale;
			}
			m_raidBossDamageWindow.useResult = true;
		}
	}

	private void OnClickHelpRequestButton()
	{
		Debug.Log("HudEventResultRaidBoss:OnClickHelpRequestButton");
		SoundManager.SePlay("sys_menu_decide");
	}

	private void ServerGetEventRaidBossUserList_Succeeded(MsgGetEventRaidBossUserListSucceed msg)
	{
		DamageDetailsWindowOpen();
	}

	private void ServerGetEventRaidBossDesiredList_Succeeded(MsgEventRaidBossDesiredListSucceed msg)
	{
		m_desiredList = msg.m_desiredList;
		if (m_desiredList == null)
		{
			return;
		}
		if (m_desiredList.Count > 0)
		{
			m_raidResultState = RaidResultState.WAIT_HELP;
			m_helpRequestWindow = RaidBosshelpRequestWindow.Create(m_desiredList);
			if (m_helpRequestWindow != null)
			{
				GameObject gameObject = m_helpRequestWindow.gameObject;
				if (gameObject != null)
				{
					Vector3 localPosition = gameObject.transform.localPosition;
					Vector3 localScale = gameObject.transform.localScale;
					gameObject.transform.parent = m_resultRootObject.transform;
					gameObject.transform.localPosition = localPosition;
					gameObject.transform.localScale = localScale;
				}
				m_isHelpRequestWindowOpen = true;
			}
		}
		else
		{
			m_raidResultState = RaidResultState.WAIT_HELP_FAILURE;
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "HelpRequestFailure";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "Lbl_caption_help_request_failure").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "Lbl_help_request_failure_text").text;
			GeneralWindow.Create(info);
		}
	}

	public void SetEnableDamageDetailsButton(bool flag)
	{
		if (m_DamageDetailsButton != null)
		{
			m_DamageDetailsButton.isEnabled = flag;
		}
	}
}
