using AnimationOrTween;
using System.Collections;
using UnityEngine;

public class NetworkErrorWindow : MonoBehaviour
{
	public enum ButtonType
	{
		Ok,
		YesNo,
		HomePage,
		Repayment,
		TextOnly
	}

	public struct CInfo
	{
		public delegate void FinishedCloseDelegate();

		public string name;

		public ButtonType buttonType;

		public string anchor_path;

		public GameObject parentGameObject;

		public string caption;

		public string message;

		public FinishedCloseDelegate finishedCloseDelegate;

		public bool disableButton;

		public bool isPlayErrorSe;
	}

	private struct Pressed
	{
		public bool m_isButtonPressed;

		public bool m_isOkButtonPressed;

		public bool m_isYesButtonPressed;

		public bool m_isNoButtonPressed;
	}

	private const char FORM_FEED_CODE = '\f';

	private static GameObject m_windowPrefab;

	private static GameObject m_windowObject;

	private static CInfo m_info;

	private static bool m_disableButton;

	private static bool m_created;

	private static UILabel m_captionLabel;

	private static Pressed m_pressed;

	private static Pressed m_resultPressed;

	private static string[] m_messages;

	private static int m_messageCount;

	public static CInfo Info
	{
		get
		{
			return m_info;
		}
	}

	public static bool Created
	{
		get
		{
			return m_created;
		}
	}

	public static bool IsButtonPressed
	{
		get
		{
			return m_resultPressed.m_isButtonPressed;
		}
	}

	public static bool IsOkButtonPressed
	{
		get
		{
			return m_resultPressed.m_isOkButtonPressed;
		}
	}

	public static bool IsYesButtonPressed
	{
		get
		{
			return m_resultPressed.m_isYesButtonPressed;
		}
	}

	public static bool IsNoButtonPressed
	{
		get
		{
			return m_resultPressed.m_isNoButtonPressed;
		}
	}

	private void Start()
	{
		m_windowObject = base.gameObject;
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		Transform transform = null;
		transform = ((m_info.parentGameObject != null) ? GameObjectUtil.FindChildGameObject(m_info.parentGameObject, "Anchor_5_MC").transform : ((m_info.anchor_path == null) ? gameObject.transform.Find("Camera/Anchor_5_MC") : gameObject.transform.Find(m_info.anchor_path)));
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localScale = base.transform.localScale;
		base.transform.parent = transform;
		base.transform.localPosition = localPosition;
		base.transform.localScale = localScale;
		SetDisableButton(m_disableButton);
		GameObject gameObject2 = m_windowObject.transform.Find("window/Lbl_caption").gameObject;
		m_captionLabel = gameObject2.GetComponent<UILabel>();
		m_captionLabel.text = m_info.caption;
		GameObject gameObject3 = m_windowObject.transform.Find("window/pattern_btn_use/textView/ScrollView/Lbl_body").gameObject;
		string str = "window/pattern_btn_use/textView/";
		Transform transform2 = m_windowObject.transform.Find("window/pattern_btn_use");
		Transform transform3 = m_windowObject.transform.Find("window/pattern_btn_use/textView");
		Transform transform4 = m_windowObject.transform.Find("window/pattern_btn_use/pattern_0");
		Transform transform5 = m_windowObject.transform.Find("window/pattern_btn_use/pattern_1");
		Transform transform6 = m_windowObject.transform.Find("window/pattern_btn_use/pattern_2");
		Transform transform7 = m_windowObject.transform.Find("window/pattern_btn_use/pattern_3");
		transform2.gameObject.SetActive(true);
		transform3.gameObject.SetActive(true);
		switch (m_info.buttonType)
		{
		case ButtonType.Ok:
			transform4.gameObject.SetActive(true);
			transform5.gameObject.SetActive(false);
			transform6.gameObject.SetActive(false);
			transform7.gameObject.SetActive(false);
			break;
		case ButtonType.YesNo:
			transform4.gameObject.SetActive(false);
			transform5.gameObject.SetActive(true);
			transform6.gameObject.SetActive(false);
			transform7.gameObject.SetActive(false);
			break;
		case ButtonType.HomePage:
			transform4.gameObject.SetActive(false);
			transform5.gameObject.SetActive(false);
			transform6.gameObject.SetActive(true);
			transform7.gameObject.SetActive(false);
			break;
		case ButtonType.Repayment:
			transform4.gameObject.SetActive(false);
			transform5.gameObject.SetActive(false);
			transform6.gameObject.SetActive(false);
			transform7.gameObject.SetActive(true);
			break;
		case ButtonType.TextOnly:
			transform4.gameObject.SetActive(false);
			transform5.gameObject.SetActive(false);
			transform6.gameObject.SetActive(false);
			transform7.gameObject.SetActive(false);
			break;
		}
		m_messages = null;
		m_messageCount = 0;
		if (m_info.message != null)
		{
			m_messages = m_info.message.Split('\f');
			UILabel component = gameObject3.GetComponent<UILabel>();
			component.text = ((m_messages.Length < 1) ? null : m_messages[m_messageCount++]);
			GameObject gameObject4 = m_windowObject.transform.Find(str + "ScrollView").gameObject;
			UIPanel component2 = gameObject4.GetComponent<UIPanel>();
			Vector4 clipRange = component2.clipRange;
			float w = clipRange.w;
			Vector2 printedSize = component.printedSize;
			float y = printedSize.y;
			Vector3 localScale2 = component.transform.localScale;
			float num = y * localScale2.y;
			if (!(num > w))
			{
				BoxCollider component3 = m_windowObject.transform.Find(str + "ScrollOutline").GetComponent<BoxCollider>();
				component3.enabled = false;
			}
		}
		if (m_info.isPlayErrorSe)
		{
			SoundManager.SePlay("sys_error");
		}
	}

	public static GameObject Create(CInfo info)
	{
		SetUIEffect(false);
		m_info = info;
		m_disableButton = info.disableButton;
		m_pressed.m_isButtonPressed = false;
		m_pressed.m_isOkButtonPressed = false;
		m_pressed.m_isYesButtonPressed = false;
		m_pressed.m_isNoButtonPressed = false;
		m_resultPressed = m_pressed;
		m_created = true;
		if (m_windowPrefab == null)
		{
			m_windowPrefab = (Resources.Load("Prefabs/UI/NetworkErrorWindow") as GameObject);
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_windowPrefab, "pattern_0");
			if (gameObject != null)
			{
				UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(gameObject, "Btn_ok");
				if (uIPlayAnimation != null)
				{
					uIPlayAnimation.enabled = false;
				}
			}
		}
		if (m_windowObject != null)
		{
			return null;
		}
		m_windowObject = (Object.Instantiate(m_windowPrefab, Vector3.zero, Quaternion.identity) as GameObject);
		m_windowObject.SetActive(true);
		SoundManager.SePlay("sys_window_open");
		Animation component = m_windowObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation.Play(component, Direction.Forward);
		}
		return m_windowObject;
	}

	public static bool Close()
	{
		m_info = default(CInfo);
		m_pressed = default(Pressed);
		m_resultPressed = default(Pressed);
		m_created = false;
		if (m_windowObject != null)
		{
			SoundManager.SePlay("sys_window_close");
			DestroyWindow();
			return true;
		}
		return false;
	}

	public static void ResetButton()
	{
		m_pressed = default(Pressed);
		m_resultPressed = default(Pressed);
	}

	public static bool IsCreated(string name)
	{
		CInfo info = Info;
		return info.name == name;
	}

	private void OnClickOkButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isOkButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
			Animation component = m_windowObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Reverse);
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedCloseAnim, true);
			}
		}
	}

	private void OnClickRepaymetButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isButtonPressed = true;
			m_resultPressed.m_isButtonPressed = true;
		}
	}

	private void OnClickYesButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isYesButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
	}

	private void OnClickNoButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_window_close");
			m_pressed.m_isNoButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
	}

	public void OnFinishedCloseAnim()
	{
		StartCoroutine(OnFinishedCloseAnimCoroutine());
	}

	private IEnumerator OnFinishedCloseAnimCoroutine()
	{
		float waitStartTime = Time.unscaledTime;
		while (true)
		{
			float currentTime = Time.unscaledTime;
			float elapseTime = currentTime - waitStartTime;
			if (elapseTime >= 0.5f)
			{
				break;
			}
			yield return null;
		}
		m_resultPressed = m_pressed;
		if (m_info.finishedCloseDelegate != null)
		{
			m_info.finishedCloseDelegate();
		}
		DestroyWindow();
	}

	private static void DestroyWindow()
	{
		if (m_windowObject != null)
		{
			Object.Destroy(m_windowObject);
			m_windowObject = null;
		}
		SetUIEffect(true);
	}

	public static void SetDisableButton(bool disableButton)
	{
		m_disableButton = disableButton;
		if (m_windowObject != null)
		{
			UIButton[] componentsInChildren = m_windowObject.GetComponentsInChildren<UIButton>(true);
			foreach (UIButton uIButton in componentsInChildren)
			{
				uIButton.isEnabled = !m_disableButton;
			}
			UIImageButton[] componentsInChildren2 = m_windowObject.GetComponentsInChildren<UIImageButton>(true);
			foreach (UIImageButton uIImageButton in componentsInChildren2)
			{
				uIImageButton.isEnabled = !m_disableButton;
			}
		}
	}

	private static void SetUIEffect(bool flag)
	{
		if (UIEffectManager.Instance != null)
		{
			UIEffectManager.Instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, flag);
		}
	}

	private static void SendButtonMessage(string patternName, string btnName)
	{
		if (!(m_windowObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_windowObject, patternName);
		if (gameObject != null)
		{
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject, btnName);
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
	}

	public static void OnClickPlatformBackButton()
	{
		if (m_created)
		{
			switch (m_info.buttonType)
			{
			case ButtonType.Repayment:
			case ButtonType.TextOnly:
				break;
			case ButtonType.Ok:
				SendButtonMessage("pattern_0", "Btn_ok");
				break;
			case ButtonType.YesNo:
				SendButtonMessage("pattern_1", "Btn_no");
				break;
			case ButtonType.HomePage:
				SendButtonMessage("pattern_2", "Btn_no");
				break;
			}
		}
	}
}
