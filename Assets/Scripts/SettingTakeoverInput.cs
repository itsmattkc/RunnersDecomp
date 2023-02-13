using AnimationOrTween;
using Text;
using UnityEngine;

public class SettingTakeoverInput : SettingBase
{
	private enum InputState
	{
		INPUTTING,
		DICIDE,
		CANCELED
	}

	private class textLabelData
	{
		public string label;

		public string text_label;

		public textLabelData(string s1, string s2)
		{
			label = s1;
			text_label = s2;
		}
	}

	private enum textKind
	{
		HEADER,
		INPUT_ID,
		INPUT_ID_SPACE,
		INPUT_PASS,
		INPUT_PASS_SPACE,
		END
	}

	private static GameObject m_prefab;

	private GameObject m_object;

	private UIPlayAnimation m_uiAnimation;

	private InputState m_inputState;

	private string m_anchorPath;

	private readonly string ExcludePathName = "UI Root (2D)/Camera/Anchor_5_MC";

	private bool m_isEnd;

	private bool m_isLoaded;

	private bool m_playStartCue;

	private bool m_isWindowOpen;

	private UIInput m_inputId;

	private UIInput m_inputPass;

	private textLabelData[] textParamTable = new textLabelData[5]
	{
		new textLabelData("Lbl_id_info", "takeover_input_header"),
		new textLabelData("Lbl_word_id", "takeover_input_id_head"),
		new textLabelData("Lbl_input_id", "takeover_input_id_space"),
		new textLabelData("Lbl_word_pass", "takeover_input_pass_head"),
		new textLabelData("Lbl_input_pass", "takeover_input_pass_space")
	};

	public bool IsDicide
	{
		get
		{
			return m_inputState == InputState.DICIDE;
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

	public bool IsLoaded
	{
		get
		{
			return m_isLoaded;
		}
	}

	public string InputIdText
	{
		get
		{
			if (m_inputId == null)
			{
				return string.Empty;
			}
			return m_inputId.value;
		}
		private set
		{
		}
	}

	public string InputPassText
	{
		get
		{
			if (m_inputPass == null)
			{
				return string.Empty;
			}
			return m_inputPass.value;
		}
		private set
		{
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
			base.enabled = true;
			if (m_uiAnimation != null)
			{
				EventDelegate.Add(m_uiAnimation.onFinished, OnFinishedOpenAnimationCallback, true);
				m_uiAnimation.Play(true);
			}
			m_inputState = InputState.INPUTTING;
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
	}

	private void SetupWindowData()
	{
		if (m_prefab == null)
		{
			m_prefab = (Resources.Load("Prefabs/UI/window_takeover_id_input") as GameObject);
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
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_object, "Btn_ok");
		if (gameObject2 != null)
		{
			UIButtonMessage uIButtonMessage = gameObject2.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickOkButton";
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_object, "Btn_close");
		if (gameObject3 != null)
		{
			UIButtonMessage uIButtonMessage2 = gameObject3.GetComponent<UIButtonMessage>();
			if (uIButtonMessage2 == null)
			{
				uIButtonMessage2 = gameObject3.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage2.target = base.gameObject;
			uIButtonMessage2.functionName = "OnClickCancelButton";
		}
		TextManager.TextType type = TextManager.TextType.TEXTTYPE_FIXATION_TEXT;
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_object, textParamTable[i].label);
			if (gameObject4 != null)
			{
				UILabel component = gameObject4.GetComponent<UILabel>();
				if (component != null)
				{
					TextUtility.SetText(component, type, "Title", textParamTable[i].text_label);
				}
			}
		}
		m_inputId = GameObjectUtil.FindChildGameObjectComponent<UIInput>(m_object, "Input_id");
		m_inputPass = GameObjectUtil.FindChildGameObjectComponent<UIInput>(m_object, "Input_pass");
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component2 = m_object.GetComponent<Animation>();
			m_uiAnimation.target = component2;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		m_object.SetActive(false);
		Debug.Log("SettingTakeoverInput:SetupWindowData End");
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

	private void OnClickCancelButton()
	{
		PlayCloseAnimation();
		m_inputState = InputState.CANCELED;
	}

	private void OnClickOkButton()
	{
		PlayCloseAnimation();
		SoundManager.SePlay("sys_menu_decide");
		m_inputState = InputState.DICIDE;
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
			OnClickCancelButton();
		}
	}
}
