using AnimationOrTween;
using Message;
using SaveData;
using Text;
using UnityEngine;

public class window_event_setting : WindowBase
{
	public enum TextType
	{
		WEIGHT_SAVING,
		FACEBOOK_ACCESS
	}

	public enum State
	{
		EXEC,
		PRESS_LOGIN,
		PRESS_LOGOUT,
		CLOSE
	}

	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private GameObject m_liteButton;

	[SerializeField]
	private GameObject m_textureButton;

	[SerializeField]
	private UIImageButton m_onButtonLite;

	[SerializeField]
	private UIImageButton m_offButtonLite;

	[SerializeField]
	private UIImageButton m_onButtonTex;

	[SerializeField]
	private UIImageButton m_offButtonTex;

	private TextType m_textType;

	private State m_State;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_headerSubTextLabel;

	[SerializeField]
	private UILabel m_ButtonOnTextLabel;

	[SerializeField]
	private UILabel m_ButtonOnSubTextLabel;

	[SerializeField]
	private UILabel m_ButtonOffTextLabel;

	[SerializeField]
	private UILabel m_ButtonOffSubTextLabel;

	private UIPlayAnimation m_uiAnimation;

	private bool m_isEnd;

	private bool m_isOverwrite;

	private float m_initY = -3000f;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public bool IsOverwrite
	{
		get
		{
			return m_isOverwrite;
		}
	}

	public State EndState
	{
		get
		{
			return m_State;
		}
	}

	private void Start()
	{
		OptionMenuUtility.TranObj(base.gameObject);
		base.enabled = false;
		if (m_closeBtn != null)
		{
			UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
			if (component == null)
			{
				m_closeBtn.AddComponent<UIButtonMessage>();
				component = m_closeBtn.GetComponent<UIButtonMessage>();
			}
			if (component != null)
			{
				component.enabled = true;
				component.trigger = UIButtonMessage.Trigger.OnClick;
				component.target = base.gameObject;
				component.functionName = "OnClickCloseButton";
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component2 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component2;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		SoundManager.SePlay("sys_window_open");
	}

	public void Setup(TextType textType)
	{
		m_State = State.EXEC;
		m_textType = textType;
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
		{
			bool lightValue2 = IsLightMode();
			bool texValue = IsHighTexture();
			UpdateButtonImage(lightValue2, texValue);
			TextUtility.SetCommonText(m_headerTextLabel, "Option", "weight_saving");
			TextUtility.SetCommonText(m_headerSubTextLabel, "Option", "weight_saving_info");
			TextUtility.SetCommonText(m_ButtonOnTextLabel, "Option", "button_on");
			TextUtility.SetCommonText(m_ButtonOnSubTextLabel, "Option", "button_on");
			TextUtility.SetCommonText(m_ButtonOffTextLabel, "Option", "button_off");
			TextUtility.SetCommonText(m_ButtonOffSubTextLabel, "Option", "button_off");
			break;
		}
		case TextType.FACEBOOK_ACCESS:
		{
			bool lightValue = IsLogin();
			UpdateButtonImage(lightValue, false);
			TextUtility.SetCommonText(m_headerTextLabel, "Option", "facebook_access");
			TextUtility.SetCommonText(m_headerSubTextLabel, "Option", "facebook_access_info");
			TextUtility.SetCommonText(m_ButtonOnTextLabel, "Option", "login");
			TextUtility.SetCommonText(m_ButtonOnSubTextLabel, "Option", "login");
			TextUtility.SetCommonText(m_ButtonOffTextLabel, "Option", "logout");
			TextUtility.SetCommonText(m_ButtonOffSubTextLabel, "Option", "logout");
			break;
		}
		}
	}

	private void Update()
	{
		if (GeneralWindow.IsCreated("BackTitleSelect") && GeneralWindow.IsButtonPressed)
		{
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.TITLE);
			GeneralWindow.Close();
			base.enabled = false;
			PlayCloseAnimation();
		}
	}

	private void PlayCloseAnimation()
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallback, true);
			}
		}
		SoundManager.SePlay("sys_window_close");
	}

	private bool IsLightMode()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.lightMode;
			}
		}
		return false;
	}

	private bool IsHighTexture()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.highTexture;
			}
		}
		return false;
	}

	private bool IsLogin()
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			return socialInterface.IsLoggedIn;
		}
		return false;
	}

	private void UpdatePosition()
	{
		if (m_liteButton == null || m_textureButton == null)
		{
			return;
		}
		if (m_textType == TextType.WEIGHT_SAVING)
		{
			if (m_initY == -3000f)
			{
				Vector3 localPosition = m_liteButton.transform.localPosition;
				m_initY = localPosition.y;
			}
			Transform transform = m_liteButton.transform;
			Vector3 localPosition2 = m_liteButton.transform.localPosition;
			float x = localPosition2.x;
			float initY = m_initY;
			Vector3 localPosition3 = m_liteButton.transform.localPosition;
			transform.localPosition = new Vector3(x, initY, localPosition3.x);
			m_textureButton.SetActive(true);
			return;
		}
		if (m_initY == -3000f)
		{
			Vector3 localPosition4 = m_liteButton.transform.localPosition;
			m_initY = localPosition4.y;
		}
		float initY2 = m_initY;
		Vector3 localPosition5 = m_textureButton.transform.localPosition;
		float y = initY2 + (localPosition5.y - m_initY) * 0.5f;
		Transform transform2 = m_liteButton.transform;
		Vector3 localPosition6 = m_liteButton.transform.localPosition;
		float x2 = localPosition6.x;
		Vector3 localPosition7 = m_liteButton.transform.localPosition;
		transform2.localPosition = new Vector3(x2, y, localPosition7.x);
		m_textureButton.SetActive(false);
	}

	private void UpdateButtonImage(bool lightValue, bool texValue)
	{
		UpdatePosition();
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
			if (m_onButtonTex != null)
			{
				if (texValue)
				{
					m_onButtonTex.gameObject.SetActive(false);
				}
				else
				{
					m_onButtonTex.gameObject.SetActive(true);
				}
			}
			if (m_offButtonTex != null)
			{
				if (!texValue)
				{
					m_offButtonTex.gameObject.SetActive(false);
				}
				else
				{
					m_offButtonTex.gameObject.SetActive(true);
				}
			}
			if (m_onButtonLite != null)
			{
				if (lightValue)
				{
					m_onButtonLite.gameObject.SetActive(false);
				}
				else
				{
					m_onButtonLite.gameObject.SetActive(true);
				}
			}
			if (m_offButtonLite != null)
			{
				if (!lightValue)
				{
					m_offButtonLite.gameObject.SetActive(false);
				}
				else
				{
					m_offButtonLite.gameObject.SetActive(true);
				}
			}
			break;
		case TextType.FACEBOOK_ACCESS:
			if (m_onButtonLite != null)
			{
				if (lightValue)
				{
					m_onButtonLite.gameObject.SetActive(false);
				}
				else
				{
					m_onButtonLite.gameObject.SetActive(true);
				}
			}
			if (m_offButtonLite != null)
			{
				if (!lightValue)
				{
					m_offButtonLite.gameObject.SetActive(false);
				}
				else
				{
					m_offButtonLite.gameObject.SetActive(true);
				}
			}
			break;
		}
	}

	private void SaveSystemData(bool lightModeFlag)
	{
		bool flag = IsLightMode();
		if (flag == lightModeFlag)
		{
			return;
		}
		m_isOverwrite = true;
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.lightMode = lightModeFlag;
			}
		}
	}

	private void SaveSystemDataTex(bool texModeFlag)
	{
		bool flag = IsHighTexture();
		if (flag == texModeFlag)
		{
			return;
		}
		m_isOverwrite = true;
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.highTexture = texModeFlag;
			}
		}
	}

	private void OnClickOnButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
		{
			SaveSystemData(true);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BackTitleSelect";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
			info.message = TextUtility.GetCommonText("Option", "weight_saving_back_title_text");
			GeneralWindow.Create(info);
			base.enabled = true;
			break;
		}
		case TextType.FACEBOOK_ACCESS:
			m_State = State.PRESS_LOGIN;
			PlayCloseAnimation();
			break;
		}
	}

	private void OnClickOffButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
		{
			SaveSystemData(false);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BackTitleSelect";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
			info.message = TextUtility.GetCommonText("Option", "weight_saving_back_title_text");
			GeneralWindow.Create(info);
			base.enabled = true;
			break;
		}
		case TextType.FACEBOOK_ACCESS:
			m_State = State.PRESS_LOGOUT;
			PlayCloseAnimation();
			break;
		}
	}

	private void OnClickTexOnButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
		{
			SaveSystemDataTex(true);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BackTitleSelect";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
			info.message = TextUtility.GetCommonText("Option", "weight_saving_back_title_text");
			GeneralWindow.Create(info);
			base.enabled = true;
			break;
		}
		case TextType.FACEBOOK_ACCESS:
			PlayCloseAnimation();
			break;
		}
	}

	private void OnClickTexOffButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		switch (m_textType)
		{
		case TextType.WEIGHT_SAVING:
		{
			SaveSystemDataTex(false);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BackTitleSelect";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
			info.message = TextUtility.GetCommonText("Option", "weight_saving_back_title_text");
			GeneralWindow.Create(info);
			base.enabled = true;
			break;
		}
		case TextType.FACEBOOK_ACCESS:
			PlayCloseAnimation();
			break;
		}
	}

	private void OnClickCloseButton()
	{
		m_State = State.CLOSE;
		PlayCloseAnimation();
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
			base.enabled = false;
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.SendMessage("OnClick");
		}
	}
}
