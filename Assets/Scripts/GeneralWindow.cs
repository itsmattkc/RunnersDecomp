using AnimationOrTween;
using System;
using System.Diagnostics;
using UnityEngine;

public class GeneralWindow : WindowBase
{
	public enum ButtonType
	{
		None,
		Ok,
		YesNo,
		ShopCancel,
		TweetCancel,
		Close,
		TweetOk,
		OkNextSkip,
		OkNextSkipAllSkip
	}

	public struct CInfo
	{
		public struct Event
		{
			public class FaceWindow
			{
				public Texture texture;

				public string name;

				public EffectType effectType;

				public AnimType animType;

				public ReverseType reverseType;

				public ShowingType showingType;
			}

			public FaceWindow[] faceWindows;

			public ArrowType arrowType;

			public string bgmCueName;

			public string seCueName;

			public string message;
		}

		public delegate void FinishedCloseDelegate();

		public bool isImgView;

		public string imgTextureName;

		public string name;

		public ButtonType buttonType;

		public string caption;

		public string anchor_path;

		public GameObject parentGameObject;

		public bool disableButton;

		public string message;

		public string imageName;

		public string imageCount;

		public Event[] events;

		public FinishedCloseDelegate finishedCloseDelegate;

		public bool isPlayErrorSe;

		public bool isNotPlaybackDefaultBgm;

		public bool isActiveImageView;

		public bool isSpecialEvent;
	}

	private struct Pressed
	{
		public bool m_isButtonPressed;

		public bool m_isOkButtonPressed;

		public bool m_isYesButtonPressed;

		public bool m_isNoButtonPressed;

		public bool m_isAllSkipButtonPressed;
	}

	[Serializable]
	private class FaceWindowUI
	{
		[SerializeField]
		public string m_windowKey;

		[SerializeField]
		public GameObject m_faceWindowGameObject;

		[SerializeField]
		public UITexture m_faceTexture;

		[SerializeField]
		public GameObject m_namePlateGameObject;

		[SerializeField]
		public UILabel m_nameLabel;

		[SerializeField]
		public GameObject m_balloonArrow;

		[SerializeField]
		public GameObject m_disableFilter;

		[SerializeField]
		public Animation m_fadeAnimation;

		[SerializeField]
		public Animation m_vibrateAnimation;

		[SerializeField]
		public Animation[] m_faceAnimations = new Animation[2];
	}

	private const char FORM_FEED_CODE = '\f';

	private static GameObject m_generalWindowPrefab;

	private static GameObject m_generalWindowObject;

	private static CInfo m_info;

	private static bool m_disableButton;

	private static UILabel m_captionLabel;

	private static UILabel m_imageCountLabel;

	private static Pressed m_pressed;

	private static Pressed m_resultPressed;

	private static bool m_isChangedBgm;

	private static bool m_created;

	private static bool m_isOpend;

	private static bool m_playedCloseSE;

	private static int m_eventCount;

	private static string[] m_messages;

	private static int m_messageCount;

	private static UILabel m_messageLabel;

	[SerializeField]
	private GameObject[] m_textViews = new GameObject[2];

	[SerializeField]
	private GameObject m_imgView;

	[SerializeField]
	private GameObject m_imgItem;

	[SerializeField]
	private GameObject m_imgChao;

	[SerializeField]
	private UILabel m_imgName;

	[SerializeField]
	private UILabel m_imgCount;

	[SerializeField]
	private GameObject m_imgDecoEff;

	[SerializeField]
	private GameObject[] m_eventViews = new GameObject[2];

	[SerializeField]
	private UILabel[] m_eventTexts = new UILabel[2];

	[SerializeField]
	private GameObject m_eventOkButton;

	[SerializeField]
	private GameObject m_eventNextButton;

	[SerializeField]
	private GameObject m_eventAllSkipButton;

	[SerializeField]
	private GameObject m_spEventOkButton;

	[SerializeField]
	private GameObject m_spEventNextButton;

	[SerializeField]
	private UIScrollBar m_eventScrollBar;

	private static float m_createdTime;

	[SerializeField]
	private FaceWindowUI[] m_singleFaceWindowUis = new FaceWindowUI[1];

	[SerializeField]
	private FaceWindowUI[] m_twinFaceWindowUis = new FaceWindowUI[2];

	[SerializeField]
	public UITexture m_eventWindowBgTexture;

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

	public static bool IsAllSkipButtonPressed
	{
		get
		{
			return m_resultPressed.m_isAllSkipButtonPressed;
		}
	}

	private void Start()
	{
		m_generalWindowObject = base.gameObject;
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		Transform transform = null;
		transform = ((m_info.parentGameObject != null) ? GameObjectUtil.FindChildGameObject(m_info.parentGameObject, "Anchor_5_MC").transform : ((m_info.anchor_path == null) ? gameObject.transform.Find("Camera/Anchor_5_MC") : gameObject.transform.Find(m_info.anchor_path)));
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localScale = base.transform.localScale;
		base.transform.parent = transform;
		base.transform.localPosition = localPosition;
		base.transform.localScale = localScale;
		SetDisableButton(m_disableButton);
		GameObject gameObject2 = m_generalWindowObject.transform.Find("window/Lbl_caption").gameObject;
		m_captionLabel = gameObject2.GetComponent<UILabel>();
		m_captionLabel.text = m_info.caption;
		m_imageCountLabel = m_imgCount;
		GameObject gameObject3 = m_generalWindowObject.transform.Find("window/pattern_btn_use/textView/ScrollView/Lbl_body").gameObject;
		string str = "window/pattern_btn_use/textView/";
		Transform transform2 = m_generalWindowObject.transform.Find("window/pattern_btn_less");
		Transform transform3 = m_generalWindowObject.transform.Find("window/pattern_btn_use");
		Transform transform4 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_0");
		Transform transform5 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_1");
		Transform transform6 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_2");
		Transform transform7 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_3");
		Transform transform8 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_4");
		Transform transform9 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_5");
		Transform transform10 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_6");
		Transform transform11 = m_generalWindowObject.transform.Find("window/pattern_btn_use/pattern_7");
		transform2.gameObject.SetActive(false);
		transform3.gameObject.SetActive(false);
		transform4.gameObject.SetActive(false);
		transform5.gameObject.SetActive(false);
		transform6.gameObject.SetActive(false);
		transform7.gameObject.SetActive(false);
		transform8.gameObject.SetActive(false);
		transform9.gameObject.SetActive(false);
		transform10.gameObject.SetActive(false);
		transform11.gameObject.SetActive(false);
		bool flag = false;
		if (RegionManager.Instance != null)
		{
			flag = RegionManager.Instance.IsUseSNS();
		}
		switch (m_info.buttonType)
		{
		case ButtonType.None:
			transform2.gameObject.SetActive(true);
			gameObject3 = m_generalWindowObject.transform.Find("window/pattern_btn_less/bl_textView/bl_ScrollView/bl_Lbl_body").gameObject;
			str = "window/pattern_btn_less/bl_textView/bl_";
			break;
		case ButtonType.Ok:
			transform3.gameObject.SetActive(true);
			transform4.gameObject.SetActive(true);
			break;
		case ButtonType.YesNo:
			transform3.gameObject.SetActive(true);
			transform5.gameObject.SetActive(true);
			if (m_info.name == "FacebookLogin" && !flag)
			{
				UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(transform5.gameObject, "Btn_yes");
				if (uIImageButton2 != null)
				{
					uIImageButton2.isEnabled = false;
				}
			}
			break;
		case ButtonType.ShopCancel:
			transform3.gameObject.SetActive(true);
			transform6.gameObject.SetActive(true);
			break;
		case ButtonType.TweetCancel:
			transform3.gameObject.SetActive(true);
			transform7.gameObject.SetActive(true);
			if (!flag)
			{
				UIImageButton uIImageButton3 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(transform7.gameObject, "Btn_post");
				if (uIImageButton3 != null)
				{
					uIImageButton3.isEnabled = false;
				}
			}
			break;
		case ButtonType.Close:
			transform3.gameObject.SetActive(true);
			transform8.gameObject.SetActive(true);
			break;
		case ButtonType.TweetOk:
			transform3.gameObject.SetActive(true);
			transform9.gameObject.SetActive(true);
			if (!flag)
			{
				UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(transform9.gameObject, "Btn_post");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = false;
				}
			}
			break;
		case ButtonType.OkNextSkip:
			transform3.gameObject.SetActive(true);
			transform10.gameObject.SetActive(true);
			break;
		case ButtonType.OkNextSkipAllSkip:
			transform3.gameObject.SetActive(true);
			transform11.gameObject.SetActive(true);
			break;
		}
		int num = (m_info.buttonType != 0) ? 1 : 0;
		m_isChangedBgm = false;
		m_playedCloseSE = false;
		m_eventCount = 0;
		m_messages = null;
		m_messageCount = 0;
		m_messageLabel = null;
		m_textViews[num].SetActive(m_info.message != null);
		if (m_info.message != null)
		{
			m_messages = m_info.message.Split('\f');
			UILabel component = gameObject3.GetComponent<UILabel>();
			component.text = ((m_messages.Length < 1) ? null : m_messages[m_messageCount++]);
			m_messageLabel = component;
			GameObject gameObject4 = m_generalWindowObject.transform.Find(str + "ScrollView").gameObject;
			UIPanel component2 = gameObject4.GetComponent<UIPanel>();
			Vector4 clipRange = component2.clipRange;
			float w = clipRange.w;
			Vector2 printedSize = component.printedSize;
			float y = printedSize.y;
			Vector3 localScale2 = component.transform.localScale;
			float num2 = y * localScale2.y;
			if (!(num2 > w))
			{
				BoxCollider component3 = m_generalWindowObject.transform.Find(str + "ScrollOutline").GetComponent<BoxCollider>();
				component3.enabled = false;
			}
		}
		bool isActiveImageView = m_info.isActiveImageView;
		m_imgView.SetActive(isActiveImageView);
		if (isActiveImageView)
		{
			m_imgItem.SetActive(false);
			m_imgChao.SetActive(false);
			m_imgDecoEff.SetActive(false);
			m_imgName.text = m_info.imageName;
			m_imgCount.text = m_info.imageCount;
		}
		bool flag2 = m_info.events != null;
		m_eventViews[num].SetActive(flag2);
		if (flag2)
		{
			m_messageLabel = m_eventTexts[num];
			NextEvent();
			SetOkNextButton();
		}
		bool isImgView = m_info.isImgView;
		m_imgView.SetActive(isImgView);
		if (isImgView)
		{
			m_imgItem.SetActive(false);
			m_imgChao.SetActive(false);
			m_imgDecoEff.SetActive(false);
			m_textViews[num].SetActive(false);
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_imgView, "Lbl_explan");
			if (uILabel != null)
			{
				uILabel.text = m_info.message;
			}
			UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_imgView, "img_tutorial_other_character");
			if (uITexture != null)
			{
				GameObject gameObject5 = GameObject.Find(m_info.imgTextureName);
				if (gameObject5 != null)
				{
					AssetBundleTexture component4 = gameObject5.GetComponent<AssetBundleTexture>();
					if (component4 != null)
					{
						uITexture.mainTexture = component4.m_tex;
					}
				}
			}
		}
		if (m_info.isPlayErrorSe)
		{
			SoundManager.SePlay("sys_error");
		}
	}

	private void Update()
	{
		if (!(m_createdTime < 2f))
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (m_createdTime < 1f && m_createdTime + deltaTime >= 1f)
		{
			UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				UILabel[] array = componentsInChildren;
				foreach (UILabel uILabel in array)
				{
					uILabel.gameObject.SendMessage("Start");
				}
				Debug.Log("GeneralWindow UILabel restart " + componentsInChildren.Length + " !");
			}
		}
		else if (m_createdTime < 2f && m_createdTime + deltaTime >= 2f)
		{
			UILabel[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<UILabel>();
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				UILabel[] array2 = componentsInChildren2;
				foreach (UILabel uILabel2 in array2)
				{
					uILabel2.gameObject.SendMessage("Start");
				}
				Debug.Log("GeneralWindow UILabel restart " + componentsInChildren2.Length + " !!");
			}
		}
		m_createdTime += deltaTime;
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void SendButtonMessage(string patternName, string btnName)
	{
		Transform transform = m_generalWindowObject.transform.Find(patternName);
		if (transform != null && transform.gameObject.activeSelf)
		{
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(transform.gameObject, btnName);
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isOpend)
		{
			switch (m_info.buttonType)
			{
			case ButtonType.Ok:
				SendButtonMessage("window/pattern_btn_use/pattern_0", "Btn_ok");
				break;
			case ButtonType.YesNo:
				SendButtonMessage("window/pattern_btn_use/pattern_1", "Btn_no");
				break;
			case ButtonType.ShopCancel:
				SendButtonMessage("window/pattern_btn_use/pattern_2", "Btn_cancel");
				break;
			case ButtonType.TweetCancel:
				SendButtonMessage("window/pattern_btn_use/pattern_3", "Btn_ok");
				break;
			case ButtonType.Close:
				SendButtonMessage("window/pattern_btn_use/pattern_4", "Btn_close");
				break;
			case ButtonType.TweetOk:
				SendButtonMessage("window/pattern_btn_use/pattern_5", "Btn_ok");
				break;
			case ButtonType.OkNextSkip:
				SendButtonMessage("window/pattern_btn_use/pattern_6", "Btn_skip");
				break;
			case ButtonType.OkNextSkipAllSkip:
				SendButtonMessage("window/pattern_btn_use/pattern_7", "Btn_skip");
				break;
			}
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
	}

	private bool NextEvent()
	{
		if (m_info.events != null && m_eventCount < m_info.events.Length)
		{
			CInfo.Event @event = m_info.events[m_eventCount++];
			if (!string.IsNullOrEmpty(@event.bgmCueName))
			{
				if (WindowBodyData.IsBgmStop(@event.bgmCueName))
				{
					SoundManager.BgmStop();
				}
				else if (@event.bgmCueName.Contains(","))
				{
					string[] array = @event.bgmCueName.Split(',');
					if (array != null && array.Length > 1)
					{
						SoundManager.BgmPlay(array[0], array[1]);
					}
				}
				else
				{
					SoundManager.BgmPlay(@event.bgmCueName);
				}
				m_isChangedBgm = true;
			}
			if (!string.IsNullOrEmpty(@event.seCueName))
			{
				SoundManager.SePlay(@event.seCueName);
			}
			FaceWindowUI[][] array2 = new FaceWindowUI[2][]
			{
				m_singleFaceWindowUis,
				m_twinFaceWindowUis
			};
			foreach (FaceWindowUI[] array3 in array2)
			{
				FaceWindowUI[] array4 = array3;
				foreach (FaceWindowUI faceWindowUI in array4)
				{
					faceWindowUI.m_faceWindowGameObject.SetActive(false);
					faceWindowUI.m_namePlateGameObject.SetActive(false);
					faceWindowUI.m_balloonArrow.SetActive(false);
				}
			}
			Texture texture = (!m_info.isSpecialEvent) ? MileageMapUtility.GetWindowBGTexture() : EventUtility.GetBGTexture();
			if (texture != null)
			{
				m_eventWindowBgTexture.mainTexture = texture;
			}
			switch (@event.arrowType)
			{
			case ArrowType.MIDDLE:
				m_singleFaceWindowUis[0].m_balloonArrow.SetActive(true);
				break;
			case ArrowType.RIGHT:
				m_twinFaceWindowUis[1].m_balloonArrow.SetActive(true);
				break;
			case ArrowType.LEFT:
				m_twinFaceWindowUis[0].m_balloonArrow.SetActive(true);
				break;
			case ArrowType.TWO_SIDES:
				m_twinFaceWindowUis[0].m_balloonArrow.SetActive(true);
				m_twinFaceWindowUis[1].m_balloonArrow.SetActive(true);
				break;
			}
			if (@event.faceWindows != null)
			{
				FaceWindowUI[] array5 = (@event.faceWindows.Length != 1) ? m_twinFaceWindowUis : m_singleFaceWindowUis;
				for (int k = 0; k < @event.faceWindows.Length; k++)
				{
					CInfo.Event.FaceWindow faceWindow = @event.faceWindows[k];
					FaceWindowUI faceWindowUI2 = array5[k];
					Animation[] faceAnimations = faceWindowUI2.m_faceAnimations;
					foreach (Animation animation in faceAnimations)
					{
						animation.gameObject.SetActive(false);
					}
					switch (faceWindow.showingType)
					{
					case ShowingType.NORMAL:
						faceWindowUI2.m_faceWindowGameObject.SetActive(true);
						faceWindowUI2.m_disableFilter.SetActive(false);
						break;
					case ShowingType.DARK:
						faceWindowUI2.m_faceWindowGameObject.SetActive(true);
						faceWindowUI2.m_disableFilter.SetActive(true);
						break;
					case ShowingType.HIDE:
						faceWindowUI2.m_faceWindowGameObject.SetActive(false);
						faceWindowUI2.m_disableFilter.SetActive(false);
						break;
					}
					faceWindowUI2.m_namePlateGameObject.SetActive(!string.IsNullOrEmpty(faceWindow.name));
					faceWindowUI2.m_nameLabel.text = faceWindow.name;
					faceWindowUI2.m_faceTexture.mainTexture = faceWindow.texture;
					Rect uvRect = faceWindowUI2.m_faceTexture.uvRect;
					ReverseType reverseType = faceWindow.reverseType;
					if (reverseType == ReverseType.MIRROR)
					{
						uvRect.width = 0f - Mathf.Abs(uvRect.width);
					}
					else
					{
						uvRect.width = Mathf.Abs(uvRect.width);
					}
					faceWindowUI2.m_faceTexture.uvRect = uvRect;
					EffectType effectType = faceWindow.effectType;
					if (effectType == EffectType.BANG || effectType == EffectType.BLAST)
					{
						GameObject gameObject = faceWindowUI2.m_faceAnimations[(int)(faceWindow.effectType - 1)].gameObject;
						gameObject.SetActive(false);
						gameObject.SetActive(true);
					}
					string str = "_vibe_skip_Anim";
					string str2 = "_intro_skip_Anim";
					switch (faceWindow.animType)
					{
					case AnimType.VIBRATION:
						str = "_vibe_Anim";
						break;
					case AnimType.FADE_IN:
						str2 = "_intro_Anim";
						break;
					case AnimType.FADE_OUT:
						str2 = "_outro_Anim";
						break;
					}
					ActiveAnimation.Play(faceWindowUI2.m_vibrateAnimation, "ui_gn_window_event_tex_" + faceWindowUI2.m_windowKey + str, Direction.Forward);
					ActiveAnimation.Play(faceWindowUI2.m_fadeAnimation, "ui_gn_window_event_tex_" + faceWindowUI2.m_windowKey + str2, Direction.Forward);
				}
			}
			m_messageCount = 0;
			m_messages = @event.message.Split('\f');
			EventNextMessage();
			return true;
		}
		return false;
	}

	private bool EventNextMessage()
	{
		if (m_messages != null && m_messageCount < m_messages.Length)
		{
			m_messageLabel.text = m_messages[m_messageCount++];
			m_eventScrollBar.value = 0f;
			return true;
		}
		return false;
	}

	private void SetOkNextButton()
	{
		bool flag = (m_info.events != null && m_eventCount < m_info.events.Length) || (m_messages != null && m_messageCount < m_messages.Length);
		if (m_info.buttonType == ButtonType.OkNextSkip)
		{
			m_spEventOkButton.SetActive(!flag);
			m_spEventNextButton.SetActive(flag);
		}
		else if (m_info.buttonType == ButtonType.OkNextSkipAllSkip)
		{
			m_eventOkButton.SetActive(!flag);
			m_eventNextButton.SetActive(flag);
			m_eventAllSkipButton.SetActive(flag);
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
		m_pressed.m_isAllSkipButtonPressed = false;
		m_resultPressed = m_pressed;
		m_created = true;
		m_isOpend = true;
		m_isChangedBgm = false;
		m_playedCloseSE = false;
		m_createdTime = 0f;
		if (m_generalWindowPrefab == null)
		{
			m_generalWindowPrefab = (Resources.Load("Prefabs/UI/GeneralWindow") as GameObject);
		}
		if (m_generalWindowObject != null)
		{
			return null;
		}
		m_generalWindowObject = (UnityEngine.Object.Instantiate(m_generalWindowPrefab, Vector3.zero, Quaternion.identity) as GameObject);
		m_generalWindowObject.SetActive(true);
		ResetScrollBar();
		SoundManager.SePlay("sys_window_open");
		Animation component = m_generalWindowObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation.Play(component, Direction.Forward);
		}
		return m_generalWindowObject;
	}

	public static bool Close()
	{
		bool playedCloseSE = m_playedCloseSE;
		m_info = default(CInfo);
		m_pressed = default(Pressed);
		m_resultPressed = default(Pressed);
		m_created = false;
		m_isOpend = false;
		m_playedCloseSE = false;
		if (m_generalWindowObject != null)
		{
			if (!playedCloseSE)
			{
				SoundManager.SePlay("sys_window_close");
			}
			DestroyWindow();
			return true;
		}
		return false;
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
		}
		m_isOpend = false;
	}

	private void OnClickYesButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isYesButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
		m_isOpend = false;
	}

	private void OnClickNoButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_window_close");
			m_pressed.m_isNoButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
			m_playedCloseSE = true;
		}
		m_isOpend = false;
	}

	private void OnClickNextButton()
	{
		if (EventNextMessage() || NextEvent())
		{
			SoundManager.SePlay("sys_page_skip");
		}
		SetOkNextButton();
	}

	private void OnClickSkipButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_window_close");
			m_pressed.m_isNoButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
			m_playedCloseSE = true;
		}
		m_isOpend = false;
	}

	private void OnClickAllSkipButton()
	{
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_window_close");
			m_pressed.m_isButtonPressed = true;
			m_pressed.m_isAllSkipButtonPressed = true;
			m_playedCloseSE = true;
		}
		m_isOpend = false;
	}

	public void OnFinishedCloseAnim()
	{
		m_resultPressed = m_pressed;
		m_created = false;
		if (m_info.finishedCloseDelegate != null)
		{
			m_info.finishedCloseDelegate();
		}
		DestroyWindow();
		if (!m_isChangedBgm)
		{
			return;
		}
		if (m_info.isNotPlaybackDefaultBgm)
		{
			SoundManager.BgmStop();
		}
		else if (m_info.isSpecialEvent)
		{
			string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.EventTop_BgmName);
			if (string.IsNullOrEmpty(data))
			{
				HudMenuUtility.ChangeMainMenuBGM();
				return;
			}
			string cueSheetName = "BGM_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
			SoundManager.BgmChange(data, cueSheetName);
		}
		else
		{
			HudMenuUtility.ChangeMainMenuBGM();
		}
	}

	private static void DestroyWindow()
	{
		if (m_generalWindowObject != null)
		{
			UnityEngine.Object.Destroy(m_generalWindowObject);
			m_generalWindowObject = null;
		}
		SetUIEffect(true);
	}

	public static void SetCaption(string caption)
	{
		m_captionLabel.text = caption;
	}

	public static void SetImageCount(string text)
	{
		m_imageCountLabel.text = text;
	}

	public static void SetDisableButton(bool disableButton)
	{
		m_disableButton = disableButton;
		if (m_generalWindowObject != null)
		{
			UIButton[] componentsInChildren = m_generalWindowObject.GetComponentsInChildren<UIButton>(true);
			foreach (UIButton uIButton in componentsInChildren)
			{
				uIButton.isEnabled = !m_disableButton;
			}
			UIImageButton[] componentsInChildren2 = m_generalWindowObject.GetComponentsInChildren<UIImageButton>(true);
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

	private static void ResetScrollBar()
	{
		if (!(m_generalWindowObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_generalWindowObject, "textView");
		if (gameObject != null)
		{
			UIScrollBar uIScrollBar = GameObjectUtil.FindChildGameObjectComponent<UIScrollBar>(gameObject, "Scroll_Bar");
			if (uIScrollBar != null)
			{
				uIScrollBar.value = 0f;
			}
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_generalWindowObject, "bl_textView");
		if (gameObject2 != null)
		{
			UIScrollBar uIScrollBar2 = GameObjectUtil.FindChildGameObjectComponent<UIScrollBar>(gameObject2, "bl_Scroll_Bar");
			if (uIScrollBar2 != null)
			{
				uIScrollBar2.value = 0f;
			}
		}
	}

	public static Texture2D GetDummyTexture(int index)
	{
		Color[] array = new Color[8]
		{
			Color.red,
			Color.blue,
			Color.green,
			Color.yellow,
			Color.white,
			Color.cyan,
			Color.gray,
			Color.magenta
		};
		Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
		texture2D.SetPixel(0, 0, Color.black);
		texture2D.SetPixel(1, 0, Color.black);
		texture2D.SetPixel(0, 1, array[(long)(uint)index % (long)array.Length]);
		texture2D.SetPixel(1, 1, array[(long)(uint)index % (long)array.Length]);
		texture2D.Apply();
		return texture2D;
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
