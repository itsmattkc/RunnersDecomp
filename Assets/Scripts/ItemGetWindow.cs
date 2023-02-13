using AnimationOrTween;
using DataTable;
using Text;
using UnityEngine;

public class ItemGetWindow : WindowBase
{
	public enum ButtonType
	{
		Ok,
		TweetCancel
	}

	public class CInfo
	{
		public delegate void FinishedCloseDelegate();

		public bool disableButton;

		public ButtonType buttonType;

		public string name;

		public string caption;

		public int serverItemId = -1;

		public string imageCount;

		public FinishedCloseDelegate finishedCloseDelegate;
	}

	private struct Pressed
	{
		public bool m_isButtonPressed;

		public bool m_isOkButtonPressed;

		public bool m_isYesButtonPressed;

		public bool m_isNoButtonPressed;
	}

	private CInfo m_info = new CInfo();

	private Pressed m_pressed;

	[SerializeField]
	private GameObject m_imgView;

	[SerializeField]
	private GameObject m_imgEventTex;

	[SerializeField]
	private UITexture m_uiTexture;

	[SerializeField]
	private GameObject m_imgItem;

	[SerializeField]
	private UISprite m_imgItemSprite;

	[SerializeField]
	private UILabel m_imgName;

	[SerializeField]
	private UILabel m_imgCount;

	[SerializeField]
	private GameObject m_imgDecoEff;

	private string m_itemImageSpriteName;

	private bool m_disableButton;

	private bool m_isEnd = true;

	private bool m_isOpened;

	public CInfo Info
	{
		get
		{
			return m_info;
		}
	}

	public bool IsOkButtonPressed
	{
		get
		{
			return m_pressed.m_isOkButtonPressed;
		}
	}

	public bool IsYesButtonPressed
	{
		get
		{
			return m_pressed.m_isYesButtonPressed;
		}
	}

	public bool IsNoButtonPressed
	{
		get
		{
			return m_pressed.m_isNoButtonPressed;
		}
	}

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void SetWindowData()
	{
		base.gameObject.SetActive(true);
		SetDisableButton(m_disableButton);
		bool active = false;
		bool active2 = false;
		switch (m_info.buttonType)
		{
		case ButtonType.Ok:
			active = true;
			break;
		case ButtonType.TweetCancel:
			active2 = true;
			break;
		}
		Transform transform = base.gameObject.transform.Find("window/pattern_btn_use");
		Transform transform2 = base.gameObject.transform.Find("window/pattern_btn_use/pattern_0");
		Transform transform3 = base.gameObject.transform.Find("window/pattern_btn_use/pattern_1");
		if (transform != null)
		{
			transform.gameObject.SetActive(true);
		}
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(active);
		}
		if (transform3 != null)
		{
			transform3.gameObject.SetActive(active2);
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(transform3.gameObject, "Btn_post");
			if (uIImageButton != null)
			{
				if (RegionManager.Instance != null)
				{
					uIImageButton.isEnabled = RegionManager.Instance.IsUseSNS();
				}
				else
				{
					uIImageButton.isEnabled = false;
				}
			}
		}
		Transform transform4 = base.gameObject.transform.Find("window/Lbl_caption");
		if (transform4 != null)
		{
			UILabel component = transform4.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = m_info.caption;
			}
		}
		if (m_imgView != null)
		{
			m_imgView.SetActive(true);
		}
		bool active3 = true;
		bool active4 = true;
		bool active5 = true;
		string text = string.Empty;
		ServerItem serverItem = new ServerItem((ServerItem.Id)m_info.serverItemId);
		if (serverItem.idType == ServerItem.IdType.CHAO)
		{
			int chaoId = serverItem.chaoId;
			if (ChaoTextureManager.Instance != null)
			{
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(m_uiTexture, null, true);
				ChaoTextureManager.Instance.GetTexture(chaoId, info);
			}
			ChaoData chaoData = ChaoTable.GetChaoData(chaoId);
			if (chaoData != null)
			{
				text = chaoData.name;
				m_info.imageCount = TextUtility.GetTextLevel(chaoData.level.ToString());
			}
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_imgEventTex, "img_tex_flame");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			active3 = false;
		}
		else if (serverItem.rewardType != RewardType.NONE)
		{
			if (serverItem.id == ServerItem.Id.JACKPOT)
			{
				m_itemImageSpriteName = "ui_cmn_icon_item_" + 8;
				active4 = false;
				text = TextUtility.GetCommonText("Item", "ring");
			}
			else
			{
				m_itemImageSpriteName = "ui_cmn_icon_item_" + (int)serverItem.rewardType;
				active4 = false;
				text = serverItem.serverItemName;
			}
		}
		else
		{
			m_itemImageSpriteName = null;
			active3 = false;
			active4 = false;
			active5 = false;
		}
		if (m_imgEventTex != null)
		{
			m_imgEventTex.SetActive(active4);
		}
		if (m_imgItem != null)
		{
			m_imgItem.SetActive(active3);
		}
		if (m_imgItemSprite != null)
		{
			m_imgItemSprite.spriteName = m_itemImageSpriteName;
		}
		if (m_imgName != null)
		{
			m_imgName.text = text;
		}
		if (m_imgCount != null)
		{
			m_imgCount.text = m_info.imageCount;
		}
		if (m_imgDecoEff != null)
		{
			m_imgDecoEff.SetActive(active5);
		}
	}

	public GameObject Create(CInfo info)
	{
		RouletteManager.OpenRouletteWindow();
		m_info = info;
		m_disableButton = info.disableButton;
		m_pressed.m_isButtonPressed = false;
		m_pressed.m_isOkButtonPressed = false;
		m_pressed.m_isYesButtonPressed = false;
		m_pressed.m_isNoButtonPressed = false;
		m_isEnd = false;
		m_isOpened = false;
		SetWindowData();
		SoundManager.SePlay("sys_window_open");
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Forward);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallback, true);
			}
		}
		return base.gameObject;
	}

	public void Reset()
	{
		m_info = new CInfo();
		m_disableButton = false;
		m_pressed.m_isButtonPressed = false;
		m_pressed.m_isOkButtonPressed = false;
		m_pressed.m_isYesButtonPressed = false;
		m_pressed.m_isNoButtonPressed = false;
		m_isEnd = true;
		m_isOpened = false;
	}

	public bool IsCreated(string name)
	{
		return m_info.name == name;
	}

	private void OnClickOkButton()
	{
		RouletteManager.CloseRouletteWindow();
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isOkButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
		m_isOpened = false;
	}

	private void OnClickYesButton()
	{
		RouletteManager.CloseRouletteWindow();
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isYesButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
		m_isOpened = false;
	}

	private void OnClickNoButton()
	{
		RouletteManager.CloseRouletteWindow();
		if (!m_pressed.m_isButtonPressed)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_pressed.m_isNoButtonPressed = true;
			m_pressed.m_isButtonPressed = true;
		}
		m_isOpened = false;
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (data.tex != null && m_uiTexture != null)
		{
			m_uiTexture.enabled = true;
			m_uiTexture.mainTexture = data.tex;
		}
	}

	public void OnFinishedAnimationCallback()
	{
		m_isOpened = true;
	}

	public void OnFinishedCloseAnim()
	{
		RouletteManager.CloseRouletteWindow();
		m_isEnd = true;
		if (m_info.finishedCloseDelegate != null)
		{
			m_info.finishedCloseDelegate();
		}
		base.gameObject.SetActive(false);
	}

	public void SetDisableButton(bool disableButton)
	{
		m_disableButton = disableButton;
		UIButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButton>(true);
		foreach (UIButton uIButton in componentsInChildren)
		{
			uIButton.isEnabled = !m_disableButton;
		}
		UIImageButton[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<UIImageButton>(true);
		foreach (UIImageButton uIImageButton in componentsInChildren2)
		{
			uIImageButton.isEnabled = !m_disableButton;
		}
	}

	private void SendButtonMessage(string patternName, string btnName)
	{
		Transform transform = base.gameObject.transform.Find(patternName);
		if (transform != null)
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
		if (m_isOpened)
		{
			switch (m_info.buttonType)
			{
			case ButtonType.Ok:
				SendButtonMessage("window/pattern_btn_use/pattern_0", "Btn_ok");
				break;
			case ButtonType.TweetCancel:
				SendButtonMessage("window/pattern_btn_use/pattern_1", "Btn_ok");
				break;
			}
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
	}
}
