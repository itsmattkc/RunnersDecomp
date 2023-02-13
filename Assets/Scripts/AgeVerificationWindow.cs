using AnimationOrTween;
using Message;
using UnityEngine;

public class AgeVerificationWindow : MonoBehaviour
{
	private GameObject m_prefabObject;

	private GameObject m_sceneLoaderObj;

	private AgeVerificationYear m_yearButton;

	private AgeVerificationButton m_monthButtons;

	private AgeVerificationButton m_dayButtons;

	private bool m_isEnd;

	private bool m_isLoad;

	public bool IsReady
	{
		get
		{
			return m_isLoad;
		}
		private set
		{
		}
	}

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
		private set
		{
		}
	}

	public bool NoInput
	{
		get
		{
			if (m_yearButton != null && m_yearButton.NoInput)
			{
				return true;
			}
			if (m_monthButtons != null && m_monthButtons.NoInput)
			{
				return true;
			}
			if (m_dayButtons != null && m_dayButtons.NoInput)
			{
				return true;
			}
			return false;
		}
	}

	public void Setup(string anchorPath)
	{
		if (!m_isLoad)
		{
			SetWindowData();
			m_isLoad = true;
		}
	}

	private void SetWindowData()
	{
		m_prefabObject = base.gameObject;
		if (!(m_prefabObject != null))
		{
			return;
		}
		m_yearButton = m_prefabObject.AddComponent<AgeVerificationYear>();
		if (m_yearButton != null)
		{
			m_yearButton.SetCallback(ButtonClickedCallback);
			m_yearButton.Setup();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefabObject, "month_set");
		if (gameObject != null)
		{
			m_monthButtons = gameObject.AddComponent<AgeVerificationButton>();
			UILabel label = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_month_x0");
			m_monthButtons.SetLabel(AgeVerificationButton.LabelType.TYPE_TEN, label);
			GameObject upObject = GameObjectUtil.FindChildGameObject(gameObject, "Btn_month_x0_up");
			GameObject downObject = GameObjectUtil.FindChildGameObject(gameObject, "Btn_month_x0_down");
			m_monthButtons.SetButton(upObject, downObject);
			m_monthButtons.Setup(ButtonClickedCallback);
			for (int i = 1; i <= 12; i++)
			{
				m_monthButtons.AddValuePreset(i);
			}
			m_monthButtons.SetDefaultValue(1);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_prefabObject, "day_set");
		if (gameObject2 != null)
		{
			m_dayButtons = gameObject2.AddComponent<AgeVerificationButton>();
			UILabel label2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_day_x0");
			m_dayButtons.SetLabel(AgeVerificationButton.LabelType.TYPE_TEN, label2);
			GameObject upObject2 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_day_up");
			GameObject downObject2 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_day_down");
			m_dayButtons.SetButton(upObject2, downObject2);
			m_dayButtons.Setup(ButtonClickedCallback);
			for (int j = 1; j <= 31; j++)
			{
				m_dayButtons.AddValuePreset(j);
			}
			m_dayButtons.SetDefaultValue(1);
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_prefabObject, "Btn_ok");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OkButtonClickedCallback";
		}
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_prefabObject, "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = false;
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_prefabObject, "window");
		if (gameObject3 != null)
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_prefabObject, "blinder");
		if (gameObject4 != null)
		{
			gameObject4.SetActive(false);
		}
	}

	public void PlayStart()
	{
		m_isEnd = false;
		if (m_prefabObject == null)
		{
			return;
		}
		m_prefabObject.SetActive(true);
		Animation component = m_prefabObject.GetComponent<Animation>();
		if (!(component == null))
		{
			ActiveAnimation.Play(component, Direction.Forward);
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefabObject, "window");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_prefabObject, "blinder");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
			BackKeyManager.AddWindowCallBack(base.gameObject);
		}
	}

	public int GetYearValue()
	{
		if (m_yearButton != null)
		{
			return m_yearButton.CurrentValue;
		}
		return 1970;
	}

	public int GetMonthValue()
	{
		if (m_monthButtons != null)
		{
			return m_monthButtons.CurrentValue;
		}
		return 1;
	}

	public int GetDayValue()
	{
		if (m_dayButtons != null)
		{
			return m_dayButtons.CurrentValue;
		}
		return 1;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ButtonClickedCallback()
	{
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_prefabObject, "Btn_ok");
		if (uIImageButton != null)
		{
			bool isEnabled = true;
			int yearValue = GetYearValue();
			int monthValue = GetMonthValue();
			int dayValue = GetDayValue();
			if (!AgeVerificationUtility.IsValidDate(yearValue, monthValue, dayValue))
			{
				isEnabled = false;
			}
			if (NoInput)
			{
				isEnabled = false;
			}
			uIImageButton.isEnabled = isEnabled;
		}
	}

	private void OkButtonClickedCallback()
	{
		Debug.Log("AgeVerificationWindow.OKButtonClickedCallback");
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			string text = GetYearValue().ToString();
			string text2 = GetMonthValue().ToString();
			string text3 = GetDayValue().ToString();
			string birthday = text + "-" + text2 + "-" + text3;
			loggedInServerInterface.RequestServerSetBirthday(birthday, base.gameObject);
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void ServerSetBirthday_Succeeded(MsgSetBirthday msg)
	{
		m_isEnd = true;
		RankingUI.CheckSnsUse();
		if (m_prefabObject == null)
		{
			return;
		}
		Animation component = m_prefabObject.GetComponent<Animation>();
		if (component == null)
		{
			return;
		}
		ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Reverse);
		if (activeAnimation == null)
		{
			return;
		}
		EventDelegate.Add(activeAnimation.onFinished, new EventDelegate(delegate
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefabObject, "blinder");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			BackKeyManager.RemoveWindowCallBack(base.gameObject);
		}), true);
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
	}
}
