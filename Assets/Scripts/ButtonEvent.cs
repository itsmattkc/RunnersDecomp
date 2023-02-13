using Message;
using System.Collections;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
	private static bool m_debugInfo = true;

	private GameObject m_menu_anim_obj;

	private ButtonEventTimer m_timer;

	private ButtonEventBackButton m_backButton;

	private ButtonEventPageControl m_pageControl;

	private ButtonInfoTable m_info_table = new ButtonInfoTable();

	private bool m_updateRanking;

	public bool IsTransform
	{
		get
		{
			if (m_pageControl != null && m_pageControl.IsTransform)
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		BackKeyManager.AddEventCallBack(base.gameObject);
	}

	public void OnStartMainMenu()
	{
		DebugInfoDraw("OnStartMainMenu");
		Initialize();
	}

	private void Initialize()
	{
		m_menu_anim_obj = HudMenuUtility.GetMenuAnimUIObject();
		if (FontManager.Instance != null)
		{
			FontManager.Instance.ReplaceFont();
		}
		if (AtlasManager.Instance != null)
		{
			AtlasManager.Instance.ReplaceAtlasForMenu(true);
		}
		if (m_menu_anim_obj != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_menu_anim_obj, "MainMenuCmnUI");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_menu_anim_obj, "MainMenuUI4");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
		}
		if (m_timer == null)
		{
			m_timer = base.gameObject.AddComponent<ButtonEventTimer>();
		}
		if (m_backButton == null)
		{
			m_backButton = base.gameObject.AddComponent<ButtonEventBackButton>();
			m_backButton.Initialize(ButtonClickedCallback);
		}
		if (m_pageControl == null)
		{
			m_pageControl = base.gameObject.AddComponent<ButtonEventPageControl>();
			m_pageControl.Initialize(OnPageResourceLoadedCallback);
		}
	}

	public void PageChange(ButtonInfoTable.ButtonType buttonType, bool isClearHistory, bool isButtonPressed)
	{
		if (!(m_pageControl == null) && buttonType != ButtonInfoTable.ButtonType.UNKNOWN && !(m_timer == null) && !m_timer.IsWaiting())
		{
			m_pageControl.PageChange(buttonType, isClearHistory, isButtonPressed);
			m_timer.SetWaitTimeDefault();
		}
	}

	public void PageChangeMessage(MsgMenuButtonEvent msg)
	{
		ButtonInfoTable.ButtonType buttonType = msg.ButtonType;
		bool clearHistories = msg.m_clearHistories;
		bool isButtonPressed = false;
		PageChange(buttonType, clearHistories, isButtonPressed);
	}

	private void ButtonClickedCallback(ButtonInfoTable.ButtonType buttonType)
	{
		bool isClearHistory = false;
		bool isButtonPressed = true;
		PageChange(buttonType, isClearHistory, isButtonPressed);
	}

	private void OnClickPlatformBackButtonEvent()
	{
		if (!(m_timer == null) && !m_timer.IsWaiting() && !(m_pageControl == null))
		{
			m_pageControl.PageBack();
			m_timer.SetWaitTimeDefault();
		}
	}

	private void OnPageResourceLoadedCallback()
	{
		if (FontManager.Instance != null)
		{
			FontManager.Instance.ReplaceFont();
		}
		if (AtlasManager.Instance != null)
		{
			AtlasManager.Instance.ReplaceAtlasForMenu(true);
		}
		if (m_backButton != null)
		{
			for (uint num = 0u; num < 49; num++)
			{
				m_backButton.SetupBackButton((ButtonInfoTable.ButtonType)num);
			}
		}
	}

	private void OnShopBackButtonClicked()
	{
		GameObjectUtil.SendMessageFindGameObject("ShopUI2", "OnShopBackButtonClicked", null, SendMessageOptions.DontRequireReceiver);
		HudMenuUtility.SendEnableShopButton(true);
	}

	private void OnOptionBackButtonClicked()
	{
		GameObjectUtil.SendMessageFindGameObject("OptionUI", "OnEndOptionUI", null, SendMessageOptions.DontRequireReceiver);
		if (m_updateRanking)
		{
			HudMenuUtility.SendMsgUpdateRanking();
			m_updateRanking = false;
		}
	}

	private void OnUpdateRankingFlag()
	{
		m_updateRanking = true;
	}

	private void OnMenuEventClicked(GameObject menuObj)
	{
		StartCoroutine(LoadEventMenuResourceIfNotLoaded(menuObj));
	}

	private IEnumerator LoadEventMenuResourceIfNotLoaded(GameObject menuObj)
	{
		yield return null;
		if (menuObj != null)
		{
			menuObj.SendMessage("OnButtonEventCallBack", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void DebugInfoDraw(string msg)
	{
	}
}
