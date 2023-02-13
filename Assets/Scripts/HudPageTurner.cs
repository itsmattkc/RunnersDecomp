using AnimationOrTween;
using App;
using Message;
using UnityEngine;

public class HudPageTurner : MonoBehaviour
{
	private const string LEFT_BUTTON_PATH = "Anchor_4_ML/mainmenu_grip_L/";

	private const string RIGHT_BUTTON_PATH = "Anchor_6_MR/mainmenu_grip_R/";

	private const string SCROLL_BAR_PATH = "Anchor_5_MC/mainmenu_grip/mainmenu_SB";

	private const string SLIDER_PATH = "Anchor_5_MC/mainmenu_grip/mainmenu_Slider";

	private const string CONTENTS_PATH = "Anchor_5_MC/mainmenu_contents";

	private const string LEFT_BUTTON_NAME = "Btn_mainmenu_pager_L";

	private const string RIGHT_BUTTON_NAME = "Btn_mainmenu_pager_R";

	private const string LEFT_ICON_NAME = "img_pager_icon_l";

	private const string RIGHT_ICON_NAME = "img_pager_icon_r";

	private const string PAGE_BTN_PATH = "Anchor_5_MC/mainmenu_grip/Btn_pager/";

	private const string LEFT_PAGE_BTN_NAME = "Btn_mainmenu_pager_L";

	private const string RIGHT_PAGE_BTN_NAME = "Btn_mainmenu_pager_R";

	private const string SE_NAME = "sys_page_skip";

	private const string BG_PATH = "Anchor_5_MC/mainmenu_grip/custom_bg";

	private const string BG_ANIM = "ui_mm_mileage_bg_Anim";

	private const float PAGE_STEP_VALUE = 0.5f;

	private GameObject m_left_button;

	private GameObject m_right_button;

	private GameObject m_leftPageBtn;

	private GameObject m_rightPageBtn;

	private GameObject m_bg;

	private UITexture m_bg_ui_tex;

	private Animation m_bg_animation;

	private UIScrollBar m_scroll_bar;

	private UISlider m_slider;

	private UIPanel m_contents_panel;

	private UISprite m_left_icon;

	private UISprite m_right_icon;

	private uint m_page_number;

	private bool m_bg_anim;

	private bool m_initEnd;

	public bool TutorialFlag
	{
		get;
		set;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		if (m_bg_ui_tex != null)
		{
			m_bg_ui_tex.material.mainTexture = null;
			m_bg_ui_tex.material = null;
			m_bg_ui_tex.mainTexture = null;
		}
	}

	private void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (mainMenuUIObject != null)
		{
			Transform transform = mainMenuUIObject.transform.FindChild("Anchor_4_ML/mainmenu_grip_L/Btn_mainmenu_pager_L");
			if (transform != null)
			{
				m_left_button = transform.gameObject;
				SetButtonCommponent(m_left_button);
			}
			Transform transform2 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_grip/Btn_pager/Btn_mainmenu_pager_L");
			if (transform2 != null)
			{
				m_leftPageBtn = transform2.gameObject;
				SetButtonCommponent(m_leftPageBtn);
				Transform transform3 = transform2.FindChild("img_pager_icon_l");
				if (transform3 != null)
				{
					m_left_icon = transform3.gameObject.GetComponent<UISprite>();
				}
			}
			Transform transform4 = mainMenuUIObject.transform.FindChild("Anchor_6_MR/mainmenu_grip_R/Btn_mainmenu_pager_R");
			if (transform4 != null)
			{
				m_right_button = transform4.gameObject;
				SetButtonCommponent(m_right_button);
			}
			Transform transform5 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_grip/Btn_pager/Btn_mainmenu_pager_R");
			if (transform5 != null)
			{
				m_rightPageBtn = transform5.gameObject;
				SetButtonCommponent(m_rightPageBtn);
				Transform transform6 = transform5.FindChild("img_pager_icon_r");
				if (transform6 != null)
				{
					m_right_icon = transform6.gameObject.GetComponent<UISprite>();
				}
			}
			GameObject gameObject = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_grip/mainmenu_SB").gameObject;
			if (gameObject != null)
			{
				m_scroll_bar = gameObject.GetComponent<UIScrollBar>();
				if (m_scroll_bar != null)
				{
					EventDelegate.Add(m_scroll_bar.onChange, OnChangeScrollBarValue);
				}
			}
			GameObject gameObject2 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_grip/mainmenu_Slider").gameObject;
			if (gameObject2 != null)
			{
				m_slider = gameObject2.GetComponent<UISlider>();
			}
			Transform transform7 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents");
			if (transform7 != null)
			{
				GameObject gameObject3 = transform7.gameObject;
				if (gameObject3 != null)
				{
					m_contents_panel = gameObject3.GetComponent<UIPanel>();
				}
			}
			m_bg = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_grip/custom_bg").gameObject;
			if (m_bg != null)
			{
				m_bg_animation = m_bg.GetComponent<Animation>();
				GameObject gameObject4 = m_bg.transform.FindChild("img_stage_tex").gameObject;
				if (gameObject4 != null)
				{
					m_bg_ui_tex = gameObject4.GetComponent<UITexture>();
				}
			}
		}
		SetHeader(m_page_number);
		SetIcon(m_page_number);
		m_initEnd = true;
	}

	private void SetButtonCommponent(GameObject obj)
	{
		if (obj != null)
		{
			UIButtonMessage uIButtonMessage = obj.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = obj.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickButtonCallBack";
			}
		}
	}

	private void ChangeNextPage(bool left_page_flag)
	{
		if (!(m_scroll_bar != null))
		{
			return;
		}
		if (left_page_flag)
		{
			if (m_page_number == 2)
			{
				m_scroll_bar.value = 0.5f;
			}
			else if (m_page_number == 1)
			{
				m_scroll_bar.value = 0f;
				SetVisible(m_left_button, false);
				SetVisible(m_leftPageBtn, false);
			}
			SetVisible(m_right_button, true);
			SetVisible(m_rightPageBtn, true);
		}
		else
		{
			if (m_page_number == 0)
			{
				m_scroll_bar.value = 0.5f;
			}
			else if (m_page_number == 1)
			{
				m_scroll_bar.value = 1f;
				SetVisible(m_right_button, false);
				SetVisible(m_rightPageBtn, false);
			}
			SetVisible(m_left_button, true);
			SetVisible(m_leftPageBtn, true);
		}
		if (m_scroll_bar.onDragFinished != null)
		{
			m_scroll_bar.onDragFinished();
		}
	}

	private void OnChangeScrollBarValue()
	{
		if (!(m_slider != null))
		{
			return;
		}
		float value = m_scroll_bar.value;
		if (Math.NearZero(value))
		{
			if (m_page_number != 0)
			{
				m_page_number = 0u;
				SetHeader(m_page_number);
				SetIcon(m_page_number);
				SetBG(m_page_number);
				m_slider.value = 0f;
				SoundManager.SePlay("sys_page_skip");
			}
			SetVisible(m_left_button, false);
			SetVisible(m_leftPageBtn, false);
			SetVisible(m_right_button, true);
			SetVisible(m_rightPageBtn, true);
		}
		else if (Mathf.Abs(value - 0.5f) < 0.02f)
		{
			if (m_page_number != 1)
			{
				m_page_number = 1u;
				SetHeader(m_page_number);
				SetIcon(m_page_number);
				SetBG(m_page_number);
				m_slider.value = 0.5f;
				m_scroll_bar.value = 0.5f;
				SoundManager.SePlay("sys_page_skip");
			}
			SetVisible(m_left_button, true);
			SetVisible(m_leftPageBtn, true);
			SetVisible(m_right_button, true);
			SetVisible(m_rightPageBtn, true);
		}
		else if (Math.NearZero(value - 1f))
		{
			if (m_page_number != 2)
			{
				m_page_number = 2u;
				SetHeader(m_page_number);
				SetIcon(m_page_number);
				SetBG(m_page_number);
				m_slider.value = 1f;
				SoundManager.SePlay("sys_page_skip");
			}
			SetVisible(m_left_button, true);
			SetVisible(m_leftPageBtn, true);
			SetVisible(m_right_button, false);
			SetVisible(m_rightPageBtn, false);
		}
		if (m_contents_panel != null)
		{
			Vector4 clipRange = m_contents_panel.clipRange;
			clipRange.y = 0f;
			m_contents_panel.clipRange = clipRange;
		}
	}

	private void OnClickButtonCallBack(GameObject obj)
	{
		if (obj.name == "Btn_mainmenu_pager_L" || obj.name == "Btn_mainmenu_pager_L")
		{
			ChangeNextPage(true);
		}
		else if (obj.name == "Btn_mainmenu_pager_R" || obj.name == "Btn_mainmenu_pager_R")
		{
			ChangeNextPage(false);
			if (TutorialFlag)
			{
				HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.TUTORIAL_PAGE_MOVE);
			}
		}
	}

	private void SetVisible(GameObject obj, bool flag)
	{
		if (obj != null)
		{
			obj.SetActive(flag);
		}
	}

	private void SetPage(uint page)
	{
		SetHeader(page);
		SetBG(page);
		SetIcon(page);
	}

	private void SetHeader(uint page)
	{
		if (page >= 0 && page <= 2)
		{
			uint num = page + 1;
			HudMenuUtility.SendChangeHeaderText("ui_Header_MainPage" + num);
		}
	}

	private void SetBG(uint page)
	{
		if (!(m_bg != null) || !(m_bg_animation != null) || !(m_bg_ui_tex != null))
		{
			return;
		}
		if (page == 1)
		{
			if (!m_bg_anim)
			{
				m_bg.SetActive(true);
				m_bg_ui_tex.material.mainTexture = MileageMapUtility.GetBGTexture();
				ActiveAnimation.Play(m_bg_animation, "ui_mm_mileage_bg_Anim", Direction.Forward);
				m_bg_anim = true;
			}
		}
		else if (m_bg_anim)
		{
			ActiveAnimation.Play(m_bg_animation, "ui_mm_mileage_bg_Anim", Direction.Reverse);
			m_bg_anim = false;
		}
	}

	private void SetIcon(uint page)
	{
		switch (page)
		{
		case 0u:
			SetIcon(m_right_icon, "ui_mm_pager_icon_1");
			break;
		case 1u:
			SetIcon(m_left_icon, "ui_mm_pager_icon_0");
			SetIcon(m_right_icon, "ui_mm_pager_icon_2");
			break;
		case 2u:
			SetIcon(m_left_icon, "ui_mm_pager_icon_1");
			break;
		}
	}

	private void SetIcon(UISprite uiSprite, string name)
	{
		if (uiSprite != null)
		{
			uiSprite.spriteName = name;
		}
	}

	private void OnSendChangeMainPageHeaderText()
	{
		SetPage(m_page_number);
	}

	private void SetMileageMapProduction()
	{
		m_page_number = 1u;
		SetPage(m_page_number);
		m_scroll_bar.value = 0.5f;
		m_slider.value = 0.5f;
		if (m_scroll_bar.onDragFinished != null)
		{
			m_scroll_bar.onDragFinished();
		}
	}

	public void OnSetBGTexture()
	{
		if (m_bg_ui_tex != null)
		{
			m_bg_ui_tex.material.mainTexture = MileageMapUtility.GetBGTexture();
		}
	}

	public void OnNormalDisplay()
	{
		Initialize();
		SetMileageMapProduction();
	}

	private void OnStartMileageMapProduction()
	{
		Initialize();
		SetMileageMapProduction();
		SetColliderTrigger(m_left_button, true);
		SetColliderTrigger(m_right_button, true);
	}

	private void OnStartRankingProduction()
	{
		Initialize();
		m_page_number = 0u;
		SetPage(m_page_number);
		m_scroll_bar.value = 0f;
		m_slider.value = 0f;
		SetVisible(m_left_button, false);
		SetVisible(m_leftPageBtn, false);
		SetVisible(m_right_button, false);
		SetVisible(m_rightPageBtn, false);
		if (m_scroll_bar.onDragFinished != null)
		{
			m_scroll_bar.onDragFinished();
		}
		SetColliderTrigger(m_left_button, true);
		SetColliderTrigger(m_right_button, true);
	}

	private void OnSetPlayerChaoSetPage()
	{
		Initialize();
		m_scroll_bar.value = 1f;
		if ((bool)m_bg)
		{
			m_bg.SetActive(false);
		}
		if (m_scroll_bar.onDragFinished != null)
		{
			m_scroll_bar.onDragFinished();
		}
	}

	private void OnEndMileageMapProduction()
	{
		SetColliderTrigger(m_left_button, false);
		SetColliderTrigger(m_right_button, false);
	}

	private void SetColliderTrigger(GameObject obj, bool trigger)
	{
		if (obj != null)
		{
			BoxCollider component = obj.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.isTrigger = trigger;
			}
		}
	}
}
