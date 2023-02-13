using AnimationOrTween;
using App.Utility;
using SaveData;
using Text;
using UnityEngine;

public class SettingPartsPushNotice : SettingBase
{
	private enum InputState
	{
		INPUTTING,
		PRESSED_ON,
		PRESSED_OFF,
		CANCELED
	}

	private class InfoButton
	{
		public string Name;

		public string FunctionName;

		public InfoButton(string s1, string s2)
		{
			Name = s1;
			FunctionName = s2;
		}
	}

	private static GameObject m_prefab;

	private GameObject m_object;

	private UIPlayAnimation m_uiAnimation;

	private InputState m_inputState;

	private string m_anchorPath;

	private readonly string ExcludePathName = "UI Root (2D)/Camera/Anchor_5_MC";

	private bool m_isEnd;

	private bool m_isOverwrite;

	private bool m_isLoaded;

	private bool m_playStartCue;

	private GameObject m_closeBtn;

	private int m_closeBtnEnabled = -1;

	private bool m_LocalPushNoticeFlag;

	private Bitset32 m_infoStateBackup = new Bitset32(0u);

	private UIToggle[] m_InfoStatusToggle = new UIToggle[3];

	private bool m_isWindowOpen;

	private InfoButton[] InfoCheckToggleButton = new InfoButton[3]
	{
		new InfoButton("img_check_box_0", "OnClickEventInfoButton"),
		new InfoButton("img_check_box_1", "OnClickChallengeInfoButton"),
		new InfoButton("img_check_box_2", "OnClickFriendInfoButton")
	};

	public bool IsPressedOn
	{
		get
		{
			return m_inputState == InputState.PRESSED_ON;
		}
		private set
		{
		}
	}

	public bool IsPressedOff
	{
		get
		{
			return m_inputState == InputState.PRESSED_OFF;
		}
		private set
		{
		}
	}

	public bool IsCanceled
	{
		get
		{
			return m_inputState == InputState.CANCELED;
		}
		private set
		{
		}
	}

	public bool IsOverwrite
	{
		get
		{
			return m_isOverwrite;
		}
		private set
		{
		}
	}

	public bool IsLoaded
	{
		get
		{
			return m_isLoaded;
		}
	}

	public void SetWindowActive(bool flag)
	{
		if (m_object != null)
		{
			m_object.SetActive(flag);
		}
	}

	protected override void OnSetup(string anthorPath)
	{
		if (!m_isLoaded)
		{
			m_anchorPath = ExcludePathName;
		}
	}

	protected override void OnPlayStart()
	{
		m_isEnd = false;
		m_playStartCue = false;
		m_isWindowOpen = false;
		if (m_isLoaded)
		{
			SetWindowActive(true);
			if (m_uiAnimation != null)
			{
				EventDelegate.Add(m_uiAnimation.onFinished, OnFinishedOpenAnimationCallback, true);
				m_uiAnimation.Play(true);
			}
			m_inputState = InputState.INPUTTING;
			m_isOverwrite = false;
			SoundManager.SePlay("sys_window_open");
			BackKeyManager.AddWindowCallBack(base.gameObject);
		}
		else
		{
			m_playStartCue = true;
		}
	}

	protected override bool OnIsEndPlay()
	{
		return m_isEnd;
	}

	protected override void OnUpdate()
	{
		if (!m_isLoaded)
		{
			m_isLoaded = true;
			base.enabled = false;
			SetupWindowData();
			if (m_playStartCue)
			{
				OnPlayStart();
			}
		}
		if (m_closeBtnEnabled != -1 && m_closeBtn != null)
		{
			if (m_closeBtnEnabled == 1)
			{
				m_closeBtn.SetActive(true);
			}
			else
			{
				m_closeBtn.SetActive(false);
			}
			m_closeBtnEnabled = -1;
		}
	}

	private void SetupWindowData()
	{
		if (m_prefab == null)
		{
			m_prefab = (Resources.Load("Prefabs/UI/window_pushinfo_setting2") as GameObject);
		}
		m_object = (Object.Instantiate(m_prefab, Vector3.zero, Quaternion.identity) as GameObject);
		if (!(m_object != null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find(m_anchorPath);
		if (gameObject != null)
		{
			Vector3 localPosition = new Vector3(0f, 0f, 0f);
			Vector3 localScale = m_object.transform.localScale;
			m_object.transform.parent = gameObject.transform;
			m_object.transform.localPosition = localPosition;
			m_object.transform.localScale = localScale;
		}
		window_pushinfo_setting component = m_object.GetComponent<window_pushinfo_setting>();
		if (component == null)
		{
			m_object.AddComponent<window_pushinfo_setting>();
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				m_infoStateBackup = new Bitset32(systemdata.pushNoticeFlags);
			}
			m_LocalPushNoticeFlag = IsPushNotice();
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_object, "Btn_ok");
		if (gameObject2 != null)
		{
			m_closeBtn = gameObject2;
			UIButtonMessage uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickOkButton";
		}
		for (int i = 0; i < 3; i++)
		{
			if (i == 2)
			{
				continue;
			}
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_object, InfoCheckToggleButton[i].Name);
			if (gameObject3 != null)
			{
				UIButtonMessage uIButtonMessage2 = gameObject3.AddComponent<UIButtonMessage>();
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = InfoCheckToggleButton[i].FunctionName;
				UIToggle component2 = gameObject3.GetComponent<UIToggle>();
				if (component2 != null)
				{
					component2.value = IsPushNoticeFlagStatus((SystemData.PushNoticeFlagStatus)i);
					m_InfoStatusToggle[i] = component2;
				}
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_object, "Btn_on");
		if (gameObject4 != null)
		{
			UIButtonMessage uIButtonMessage3 = gameObject4.AddComponent<UIButtonMessage>();
			uIButtonMessage3.target = base.gameObject;
			uIButtonMessage3.functionName = "OnClickOnButton";
			UIToggle component3 = gameObject4.GetComponent<UIToggle>();
			if (component3 != null)
			{
				component3.value = m_LocalPushNoticeFlag;
			}
		}
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(m_object, "Btn_off");
		if (gameObject5 != null)
		{
			UIButtonMessage uIButtonMessage4 = gameObject5.AddComponent<UIButtonMessage>();
			uIButtonMessage4.target = base.gameObject;
			uIButtonMessage4.functionName = "OnClickOffButton";
			UIToggle component4 = gameObject5.GetComponent<UIToggle>();
			if (component4 != null)
			{
				component4.value = !m_LocalPushNoticeFlag;
			}
		}
		TextManager.TextType type = TextManager.TextType.TEXTTYPE_FIXATION_TEXT;
		GameObject gameObject6 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_pushinfo_setting");
		if (gameObject6 != null)
		{
			UILabel component5 = gameObject6.GetComponent<UILabel>();
			if (component5 != null)
			{
				TextUtility.SetText(component5, type, "Option", "push_notification");
			}
		}
		GameObject gameObject7 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_pushinfo_setting_sub");
		if (gameObject7 != null)
		{
			UILabel component6 = gameObject7.GetComponent<UILabel>();
			if (component6 != null)
			{
				TextUtility.SetText(component6, type, "Option", "push_notification_info");
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component7 = m_object.GetComponent<Animation>();
			m_uiAnimation.target = component7;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		m_object.SetActive(false);
		Debug.Log("SettingPartsPushNotice:SetupWindowData End");
	}

	private void PlayCloseAnimation()
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
					m_isWindowOpen = false;
				}
			}
		}
		SoundManager.SePlay("sys_window_close");
	}

	private void OverwriteSystemData()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		SystemData systemdata = instance.GetSystemdata();
		if (systemdata != null)
		{
			if (systemdata.pushNotice != m_LocalPushNoticeFlag)
			{
				systemdata.pushNotice = m_LocalPushNoticeFlag;
				LocalNotification.EnableNotification(m_LocalPushNoticeFlag);
				m_isOverwrite = true;
			}
			PnoteNotification.RegistTagsPnote(systemdata.pushNoticeFlags);
			if (systemdata.pushNoticeFlags != m_infoStateBackup)
			{
				m_isOverwrite = true;
			}
		}
	}

	private bool IsPushNotice()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.pushNotice;
			}
		}
		return false;
	}

	private bool IsPushNoticeFlagStatus(SystemData.PushNoticeFlagStatus state)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.IsFlagStatus(state);
			}
		}
		return false;
	}

	private void SetPushNoticeFlagStatus(SystemData.PushNoticeFlagStatus state, bool flag)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.SetFlagStatus(state, flag);
			}
		}
	}

	public void SetCloseButtonEnabled(bool enabled)
	{
		if (enabled)
		{
			m_closeBtnEnabled = 1;
		}
		else
		{
			m_closeBtnEnabled = 1;
			enabled = true;
		}
		if (m_closeBtn != null)
		{
			m_closeBtn.SetActive(enabled);
			m_closeBtnEnabled = -1;
		}
	}

	private void OnClickCancelButton()
	{
		PlayCloseAnimation();
		m_inputState = InputState.CANCELED;
	}

	private void OnClickOnButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		m_inputState = InputState.PRESSED_ON;
		m_LocalPushNoticeFlag = true;
	}

	private void OnClickOffButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		m_inputState = InputState.PRESSED_OFF;
		m_LocalPushNoticeFlag = false;
	}

	private void OnClickOkButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		PlayCloseAnimation();
		OverwriteSystemData();
	}

	private void OnClickEventInfoButton()
	{
		InfoCheckBoxCommon(SystemData.PushNoticeFlagStatus.EVENT_INFO);
	}

	private void OnClickChallengeInfoButton()
	{
		InfoCheckBoxCommon(SystemData.PushNoticeFlagStatus.CHALLENGE_INFO);
	}

	private void OnClickFriendInfoButton()
	{
		InfoCheckBoxCommon(SystemData.PushNoticeFlagStatus.FRIEND_INFO);
	}

	private void InfoCheckBoxCommon(SystemData.PushNoticeFlagStatus status)
	{
		UIToggle uIToggle = m_InfoStatusToggle[(int)status];
		bool flag = false;
		if (uIToggle != null)
		{
			flag = uIToggle.value;
			SetPushNoticeFlagStatus(status, uIToggle.value);
		}
		if (flag)
		{
			SoundManager.SePlay("sys_menu_decide");
		}
		else
		{
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void OnFinishedOpenAnimationCallback()
	{
		if (!m_isWindowOpen)
		{
			m_isWindowOpen = true;
		}
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
		if (m_object != null)
		{
			m_object.SetActive(false);
			BackKeyManager.RemoveWindowCallBack(base.gameObject);
		}
	}

	public void OnClickPlatformBackButton()
	{
		if (base.gameObject.activeSelf && m_isWindowOpen)
		{
			OnClickOkButton();
		}
	}
}
