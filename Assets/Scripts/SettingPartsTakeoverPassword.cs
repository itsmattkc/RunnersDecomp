using Text;
using UnityEngine;

public class SettingPartsTakeoverPassword : SettingBase
{
	private enum State
	{
		STATE_NONE = -1,
		STATE_IDLE,
		STATE_LOAD,
		STATE_SETTING,
		STATE_WAIT_END,
		STATE_END
	}

	private enum InputState
	{
		INPUTTING,
		DECIDED,
		CANCELED
	}

	private State m_state;

	private GameObject m_object;

	private UIPlayAnimation m_uiAnimation;

	private UIInput m_input;

	private UILabel m_label;

	private string m_anchorPath;

	private readonly string ExcludePathName = "UI Root (2D)/Camera/Anchor_5_MC";

	private bool m_calcelButtonUseFlag = true;

	private bool m_isLoaded;

	private bool m_playStartCue;

	private InputState m_inputState;

	public string InputText
	{
		get
		{
			if (m_input == null)
			{
				return string.Empty;
			}
			return m_input.value;
		}
		private set
		{
		}
	}

	public UILabel TextLabel
	{
		get
		{
			return m_label;
		}
	}

	public bool IsDecided
	{
		get
		{
			if (m_inputState == InputState.DECIDED)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public bool IsCanceled
	{
		get
		{
			if (m_inputState == InputState.CANCELED)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public void SetCancelButtonUseFlag(bool useFlag)
	{
		m_calcelButtonUseFlag = useFlag;
	}

	public void SetOkButtonEnabled(bool enabled)
	{
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_object, "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = enabled;
		}
	}

	private void OnDestroy()
	{
		if (m_object != null)
		{
			Object.Destroy(m_object);
		}
	}

	protected override void OnSetup(string anchorPath)
	{
		if (!m_isLoaded)
		{
			m_anchorPath = ExcludePathName;
			m_state = State.STATE_LOAD;
		}
	}

	private void SetupWindowData()
	{
		m_object = HudMenuUtility.GetLoadMenuChildObject("window_password_setting", true);
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
		m_input = GameObjectUtil.FindChildGameObjectComponent<UIInput>(m_object, "Input_password");
		EventDelegate item = new EventDelegate(OnFinishedInput);
		m_input.onSubmit.Add(item);
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_object, "Btn_ok");
		if (gameObject2 != null)
		{
			UIButtonMessage uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickOkButton";
			UIPlayAnimation component = gameObject2.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedAnimation, false);
			}
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_object, "Btn_close");
		if (gameObject3 != null)
		{
			if (!m_calcelButtonUseFlag)
			{
				gameObject3.SetActive(false);
			}
			else
			{
				gameObject3.SetActive(true);
				UIButtonMessage uIButtonMessage2 = gameObject3.AddComponent<UIButtonMessage>();
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "OnClickCancelButton";
				window_name_setting component2 = m_object.GetComponent<window_name_setting>();
				if (component2 == null)
				{
					m_object.AddComponent<window_name_setting>();
				}
			}
			UIPlayAnimation component3 = gameObject3.GetComponent<UIPlayAnimation>();
			if (component3 != null)
			{
				EventDelegate.Add(component3.onFinished, OnFinishedAnimation, false);
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_name_setting");
		if (gameObject4 != null)
		{
			UILabel component4 = gameObject4.GetComponent<UILabel>();
			if (component4 != null)
			{
				TextUtility.SetCommonText(component4, "Option", "take_over_password_setting");
			}
		}
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_name_setting_sub");
		if (gameObject5 != null)
		{
			UILabel component5 = gameObject5.GetComponent<UILabel>();
			if (component5 != null)
			{
				TextUtility.SetCommonText(component5, "Option", "take_over_password_setting_info");
			}
		}
		GameObject gameObject6 = GameObjectUtil.FindChildGameObject(m_object, "Lbl_input_password");
		if (gameObject6 != null)
		{
			m_label = gameObject6.GetComponent<UILabel>();
			if (m_label != null)
			{
				TextUtility.SetCommonText(m_label, "Option", "take_over_password_input");
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component6 = m_object.GetComponent<Animation>();
			m_uiAnimation.target = component6;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
	}

	protected override void OnPlayStart()
	{
		if (m_isLoaded)
		{
			m_playStartCue = false;
			if (m_object != null)
			{
				m_object.SetActive(true);
			}
			if (m_uiAnimation != null)
			{
				m_uiAnimation.Play(true);
			}
			m_inputState = InputState.INPUTTING;
			m_state = State.STATE_SETTING;
			UIInput uIInput = GameObjectUtil.FindChildGameObjectComponent<UIInput>(m_object, "Input_password");
			if (uIInput != null)
			{
				uIInput.value = string.Empty;
			}
			if (m_label != null)
			{
				TextUtility.SetCommonText(m_label, "Option", "take_over_password_input");
			}
			SoundManager.SePlay("sys_window_open");
		}
		else
		{
			m_playStartCue = true;
		}
	}

	protected override bool OnIsEndPlay()
	{
		if (m_state == State.STATE_END)
		{
			return true;
		}
		return false;
	}

	protected override void OnUpdate()
	{
		switch (m_state)
		{
		case State.STATE_IDLE:
			break;
		case State.STATE_WAIT_END:
			break;
		case State.STATE_END:
			break;
		case State.STATE_LOAD:
			m_isLoaded = true;
			SetupWindowData();
			if (m_playStartCue)
			{
				OnPlayStart();
			}
			break;
		case State.STATE_SETTING:
			if (m_input.selected)
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Option", "take_over_password_input").text;
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_object, "Lbl_input_password");
				UIInput uIInput = GameObjectUtil.FindChildGameObjectComponent<UIInput>(m_object, "Input_password");
				if (text != null && uIInput != null && uILabel != null)
				{
					string value = uIInput.value;
					if (value.IndexOf(text) >= 0)
					{
						uIInput.value = string.Empty;
						uILabel.text = string.Empty;
					}
				}
			}
			if (m_inputState == InputState.DECIDED || m_inputState == InputState.CANCELED)
			{
				m_state = State.STATE_WAIT_END;
			}
			break;
		}
	}

	private void OnFinishedAnimation()
	{
		if (m_object != null)
		{
			m_object.SetActive(false);
		}
		m_state = State.STATE_END;
	}

	private void OnClickOkButton()
	{
		m_inputState = InputState.DECIDED;
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickCancelButton()
	{
		m_inputState = InputState.CANCELED;
		SoundManager.SePlay("sys_window_close");
	}

	private void OnFinishedInput()
	{
		Debug.Log("Input Finished! Input Text is" + m_input.value);
	}
}
