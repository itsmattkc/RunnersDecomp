using Text;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
	public enum WindowType
	{
		ChallengeGoShop = 0,
		ChallengeGoShopFromItem = 1,
		EventStart = 2,
		EventOutOfTime = 3,
		EventLastPlay = 4,
		NUM = 5,
		UNKNOWN = -1
	}

	public class WindowInfo
	{
		private string m_captionGroup;

		private string m_captionCell;

		private string m_messageGroup;

		private string m_messageCell;

		private GeneralWindow.ButtonType m_buttonType;

		private bool m_errorSe;

		public string CaptionGroup
		{
			get
			{
				return m_captionGroup;
			}
		}

		public string CaptionCell
		{
			get
			{
				return m_captionCell;
			}
		}

		public string MessageGroup
		{
			get
			{
				return m_messageGroup;
			}
		}

		public string MessageCell
		{
			get
			{
				return m_messageCell;
			}
		}

		public GeneralWindow.ButtonType ButtonType
		{
			get
			{
				return m_buttonType;
			}
		}

		public bool ErrorSe
		{
			get
			{
				return m_errorSe;
			}
		}

		public WindowInfo()
		{
		}

		public WindowInfo(string captionGroup, string captionCell, string messageGroup, string messageCell, GeneralWindow.ButtonType buttonType, bool errorSe)
		{
			m_captionGroup = captionGroup;
			m_captionCell = captionCell;
			m_messageGroup = messageGroup;
			m_messageCell = messageCell;
			m_buttonType = buttonType;
			m_errorSe = errorSe;
		}
	}

	public delegate void ButtonClickedCallback(bool yesButtonClicked);

	private readonly WindowInfo[] m_windowInfo = new WindowInfo[5]
	{
		new WindowInfo("MainMenu", "no_challenge_count", "MainMenu", "no_challenge_count_info", GeneralWindow.ButtonType.ShopCancel, true),
		new WindowInfo("MainMenu", "no_challenge_count", "MainMenu", "no_challenge_count_info", GeneralWindow.ButtonType.ShopCancel, true),
		new WindowInfo("ItemRoulette", "gw_remain_caption", "Event", "ui_Lbl_new_event_start", GeneralWindow.ButtonType.Ok, true),
		new WindowInfo("ItemRoulette", "gw_remain_caption", "Event", "ui_Lbl_event_out_of_time_2", GeneralWindow.ButtonType.Ok, true),
		new WindowInfo("ItemRoulette", "gw_remain_caption", "Event", "ui_Lbl_event_last_time", GeneralWindow.ButtonType.Ok, false)
	};

	private WindowType m_window_type = WindowType.UNKNOWN;

	private ButtonClickedCallback m_buttonClickedCallback;

	private void Start()
	{
		base.enabled = false;
	}

	public void CreateWindow(WindowType window_type, ButtonClickedCallback callback = null)
	{
		if (window_type < WindowType.NUM)
		{
			m_window_type = window_type;
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.buttonType = m_windowInfo[(int)m_window_type].ButtonType;
			info.caption = TextUtility.GetCommonText(m_windowInfo[(int)m_window_type].CaptionGroup, m_windowInfo[(int)m_window_type].CaptionCell);
			info.message = TextUtility.GetCommonText(m_windowInfo[(int)m_window_type].MessageGroup, m_windowInfo[(int)m_window_type].MessageCell);
			info.isPlayErrorSe = m_windowInfo[(int)m_window_type].ErrorSe;
			info.name = "MainMenuWindow";
			GeneralWindow.Create(info);
			base.enabled = true;
			m_buttonClickedCallback = callback;
		}
	}

	public void Update()
	{
		if (GeneralWindow.IsCreated("MainMenuWindow") && GeneralWindow.IsButtonPressed)
		{
			bool flag = false;
			if (WindowType.ChallengeGoShop <= m_window_type && m_window_type < WindowType.NUM && m_windowInfo[(int)m_window_type].ButtonType == GeneralWindow.ButtonType.ShopCancel)
			{
				flag = GeneralWindow.IsYesButtonPressed;
			}
			if (m_buttonClickedCallback != null)
			{
				m_buttonClickedCallback(GeneralWindow.IsYesButtonPressed);
				m_buttonClickedCallback = null;
			}
			GeneralWindow.Close();
			base.enabled = false;
			m_window_type = WindowType.UNKNOWN;
		}
	}
}
