using AnimationOrTween;
using Message;
using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class SettingPartsAcceptInvite : SettingBase
{
	private enum State
	{
		IDLE,
		WAIT_SNS_RESPONSE,
		SETUP_BEFORE,
		WAIT_SETUP,
		SETUP,
		UPDATE,
		DECIDE_FRIEND,
		WAIT_SERVER_RESPONSE,
		WAIT_END_WINDOW_CLOSE
	}

	private bool m_isValid = true;

	private bool m_isEnd;

	private bool m_isSetup;

	private GameObject m_object;

	private UIPlayAnimation m_uiAnimation;

	private List<SettingPartsInviteButton> m_buttons;

	private UIRectItemStorage m_itemStorage;

	private string m_anchorPath;

	private State m_state;

	private SocialUserData m_decidedFriendData;

	private MsgSocialFriendListResponse m_msg;

	private readonly string ExcludePathName = "UI Root (2D)/Camera/Anchor_5_MC";

	private void Start()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void OnDestroy()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	protected override void OnSetup(string anthorPath)
	{
		m_anchorPath = ExcludePathName;
	}

	protected override void OnPlayStart()
	{
		m_isEnd = false;
		bool flag = true;
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface == null)
		{
			flag = false;
		}
		else if (!socialInterface.IsLoggedIn)
		{
			flag = false;
		}
		if (!flag)
		{
			m_isValid = false;
			m_isEnd = true;
		}
		else
		{
			socialInterface.RequestInvitedFriend(base.gameObject);
		}
	}

	protected override bool OnIsEndPlay()
	{
		return m_isEnd;
	}

	protected override void OnUpdate()
	{
		if (!m_isValid || m_isEnd)
		{
			return;
		}
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.WAIT_SNS_RESPONSE:
			break;
		case State.UPDATE:
			break;
		case State.WAIT_SERVER_RESPONSE:
			break;
		case State.SETUP_BEFORE:
			if (m_isSetup)
			{
				m_state = State.WAIT_SETUP;
				break;
			}
			SetupWindowData();
			m_isSetup = true;
			m_state = State.WAIT_SETUP;
			break;
		case State.WAIT_SETUP:
			m_state = State.SETUP;
			break;
		case State.SETUP:
		{
			if (m_itemStorage != null && m_msg != null)
			{
				m_itemStorage.maxRows = m_msg.m_friends.Count;
				m_itemStorage.Restart();
			}
			List<GameObject> list = GameObjectUtil.FindChildGameObjects(m_object, "ui_option_window_invite_scroll(Clone)");
			if (list == null)
			{
				break;
			}
			m_buttons.Clear();
			if (m_msg != null)
			{
				List<SocialUserData> friends = m_msg.m_friends;
				for (int i = 0; i < friends.Count; i++)
				{
					SocialUserData socialUserData = friends[i];
					if (socialUserData != null)
					{
						GameObject gameObject = list[i];
						if (!(gameObject == null))
						{
							SettingPartsInviteButton settingPartsInviteButton = gameObject.AddComponent<SettingPartsInviteButton>();
							settingPartsInviteButton.Setup(socialUserData, InviteButtonPressedCallback);
							m_buttons.Add(settingPartsInviteButton);
						}
					}
				}
			}
			if (m_object != null)
			{
				m_object.SetActive(true);
			}
			if (m_uiAnimation != null)
			{
				m_uiAnimation.Play(true);
			}
			SoundManager.SePlay("sys_window_open");
			m_state = State.UPDATE;
			break;
		}
		case State.DECIDE_FRIEND:
			if (GeneralWindow.IsYesButtonPressed)
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					string gameId = m_decidedFriendData.CustomData.GameId;
					loggedInServerInterface.RequestServerSetInviteCode(gameId, base.gameObject);
				}
				m_decidedFriendData = null;
				m_state = State.WAIT_SERVER_RESPONSE;
				GeneralWindow.Close();
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				m_decidedFriendData = null;
				m_state = State.UPDATE;
				GeneralWindow.Close();
			}
			break;
		case State.WAIT_END_WINDOW_CLOSE:
			if (GeneralWindow.IsCreated("CreateEndAcceptWindow") && GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				OnClickCancelButton();
				m_state = State.UPDATE;
			}
			break;
		}
	}

	protected void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (!(m_object != null) || !m_object.activeSelf || m_state != State.UPDATE)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_object, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}

	private void OnClickCancelButton()
	{
		if (m_object != null)
		{
			Animation component = m_object.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Reverse);
				if (activeAnimation != null)
				{
					EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallback, true);
				}
			}
		}
		SoundManager.SePlay("sys_window_close");
	}

	private void InviteButtonPressedCallback(SocialUserData friendData)
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		TextManager.TextType type = TextManager.TextType.TEXTTYPE_FIXATION_TEXT;
		info.caption = TextUtility.GetText(type, "FaceBook", "ui_Lbl_verification");
		info.message = TextUtility.GetText(type, "FaceBook", "ui_Lbl_accept_invite_text", "{FRIEND_NAME}", friendData.Name);
		GeneralWindow.Create(info);
		m_decidedFriendData = friendData;
		m_state = State.DECIDE_FRIEND;
	}

	private void RequestInviteListEndCallback(MsgSocialFriendListResponse msg)
	{
		if (msg != null)
		{
			if (msg.m_result.IsError)
			{
				m_isEnd = true;
				return;
			}
			m_msg = msg;
			m_state = State.SETUP_BEFORE;
		}
	}

	private void ServerSetInviteCode_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.FRIEDN_ACCEPT_INVITE);
		CreateEndAcceptWindow();
		m_state = State.WAIT_END_WINDOW_CLOSE;
	}

	private void ServerSetInviteCode_Failed(MsgServerConnctFailed msg)
	{
		m_state = State.UPDATE;
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
		m_object.SetActive(false);
	}

	private void SetupWindowData()
	{
		m_object = HudMenuUtility.GetLoadMenuChildObject("window_invite", true);
		if (!(m_object != null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find(m_anchorPath);
		if (gameObject != null)
		{
			Vector3 localPosition = m_object.transform.localPosition;
			Vector3 localScale = m_object.transform.localScale;
			m_object.transform.parent = gameObject.transform;
			m_object.transform.localPosition = localPosition;
			m_object.transform.localScale = localScale;
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component = m_object.GetComponent<Animation>();
			m_uiAnimation.target = component;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_object, "Btn_close");
		if (gameObject2 != null)
		{
			UIButtonMessage uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickCancelButton";
		}
		m_itemStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_object, "slot");
		if (m_itemStorage != null && m_msg != null)
		{
			m_itemStorage.maxRows = m_msg.m_friends.Count;
		}
		m_buttons = new List<SettingPartsInviteButton>();
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_invite");
		if (gameObject3 != null)
		{
			UILabel component2 = gameObject3.GetComponent<UILabel>();
			if (component2 != null)
			{
				TextUtility.SetCommonText(component2, "Option", "acceptance_of_invite");
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_invite_sub");
		if (gameObject4 != null)
		{
			UILabel component3 = gameObject4.GetComponent<UILabel>();
			if (component3 != null)
			{
				TextUtility.SetCommonText(component3, "Option", "acceptance_of_invite_info");
			}
		}
		UIPanel component4 = m_object.GetComponent<UIPanel>();
		if (component4 != null)
		{
			component4.alpha = 0f;
		}
		m_object.SetActive(true);
	}

	private void CreateEndAcceptWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.buttonType = GeneralWindow.ButtonType.Ok;
		TextManager.TextType type = TextManager.TextType.TEXTTYPE_FIXATION_TEXT;
		info.caption = TextUtility.GetText(type, "FaceBook", "ui_Lbl_ask_accept_invite_caption");
		info.message = TextUtility.GetText(type, "FaceBook", "ui_Lbl_accept_invite_end_text");
		info.name = "CreateEndAcceptWindow";
		GeneralWindow.Create(info);
	}
}
