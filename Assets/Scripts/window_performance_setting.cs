using AnimationOrTween;
using Message;
using SaveData;
using Text;
using UnityEngine;

public class window_performance_setting : WindowBase
{
	public enum State
	{
		EXEC,
		CLOSE
	}

	[SerializeField]
	private UIToggle m_checkBox0;

	[SerializeField]
	private UIToggle m_checkBox1;

	private State m_State;

	private bool m_selected;

	private UIPlayAnimation m_uiAnimation;

	private bool m_isToggleLock;

	private bool m_isEnd;

	private bool m_isChangeCheckBox0;

	private bool m_isChangeCheckBox1;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
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
		m_selected = false;
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component;
			m_uiAnimation.clipName = "ui_cmn_window_Anim2";
		}
		SoundManager.SePlay("sys_window_open");
	}

	public void Setup()
	{
		m_selected = false;
		m_State = State.EXEC;
		UpdateButtonImage();
	}

	private void Update()
	{
		if (GeneralWindow.IsCreated("BackTitleSelect") && m_selected && GeneralWindow.IsButtonPressed)
		{
			bool flag = IsLightMode();
			bool flag2 = IsHighTexture();
			if (m_isChangeCheckBox0)
			{
				SaveSystemData(!flag);
			}
			if (m_isChangeCheckBox1)
			{
				SaveSystemDataTex(!flag2);
			}
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				instance.SaveSystemData();
			}
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

	private void UpdateButtonImage()
	{
		bool startsActive = IsLightMode();
		bool flag = IsHighTexture();
		m_isChangeCheckBox0 = false;
		m_isChangeCheckBox1 = false;
		m_isToggleLock = true;
		if (m_checkBox0 != null)
		{
			m_checkBox0.startsActive = startsActive;
			m_checkBox0.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
		}
		if (m_checkBox1 != null)
		{
			m_checkBox1.startsActive = !flag;
			m_checkBox1.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
		}
		m_isToggleLock = false;
	}

	private void SaveSystemData(bool lightModeFlag)
	{
		bool flag = IsLightMode();
		if (flag == lightModeFlag)
		{
			return;
		}
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

	private void ShowBackTileMessage()
	{
		if (m_isChangeCheckBox0 || m_isChangeCheckBox1)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BackTitleSelect";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
			info.message = TextUtility.GetCommonText("Option", "weight_saving_back_title_text");
			GeneralWindow.Create(info);
			m_selected = true;
			base.enabled = true;
		}
		else
		{
			PlayCloseAnimation();
		}
	}

	public void OnChangeCheckBox0()
	{
		if (!m_isToggleLock)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_isChangeCheckBox0 = !m_isChangeCheckBox0;
		}
	}

	public void OnChangeCheckBox1()
	{
		if (!m_isToggleLock)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_isChangeCheckBox1 = !m_isChangeCheckBox1;
		}
	}

	private void OnClickOkButton()
	{
		m_State = State.CLOSE;
		m_isToggleLock = false;
		ShowBackTileMessage();
	}

	private void OnClickCloseButton()
	{
		m_State = State.CLOSE;
		m_isToggleLock = false;
		PlayCloseAnimation();
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
		m_selected = false;
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
			base.enabled = true;
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_close");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
