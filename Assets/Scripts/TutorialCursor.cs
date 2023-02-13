using UnityEngine;

public class TutorialCursor : MonoBehaviour
{
	public enum Type
	{
		CHAOSELECT_CHAO,
		CHAOSELECT_MAIN,
		ITEMSELECT_LASER,
		MAINMENU_CHAO,
		MAINMENU_PLAY,
		MAINMENU_PAGE,
		MAINMENU_ROULETTE,
		MAINMENU_BOSS_PLAY,
		ROULETTE_SPIN,
		ROULETTE_OK,
		BACK,
		OPTION,
		CHARASELECT_CHARA,
		CHARASELECT_MAIN,
		CHARASELECT_LEVEL_UP,
		ITEMSELECT_SUBCHARA,
		ROULETTE_TOP_PAGE,
		STAGE_ITEM,
		NUM,
		NONE
	}

	private static readonly TutorialCursorParam[] ParamTable = new TutorialCursorParam[18]
	{
		new TutorialCursorParam("Anchor_5_MC/pattern_ch_chao"),
		new TutorialCursorParam("Anchor_5_MC/pattern_ch_main"),
		new TutorialCursorParam("Anchor_5_MC/pattern_it_laser"),
		new TutorialCursorParam("Anchor_5_MC/pattern_mm_chao"),
		new TutorialCursorParam("Anchor_9_BR/pattern_mm_play"),
		new TutorialCursorParam("Anchor_8_BC/pattern_mm_page3"),
		new TutorialCursorParam("Anchor_8_BC/pattern_mm_roulette"),
		new TutorialCursorParam("Anchor_5_MC/pattern_mm_boss_play"),
		new TutorialCursorParam("Anchor_6_MR/pattern_ro_spin"),
		new TutorialCursorParam("Anchor_5_MC/pattern_ro_ok"),
		new TutorialCursorParam("Anchor_7_BL/pattern_back"),
		new TutorialCursorParam("Anchor_5_MC/pattern_option"),
		new TutorialCursorParam("Anchor_5_MC/pattern_pl_main"),
		new TutorialCursorParam("Anchor_5_MC/pattern_pl_mainsub"),
		new TutorialCursorParam("Anchor_5_MC/pattern_pl_levelup"),
		new TutorialCursorParam("Anchor_5_MC/pattern_pl_change"),
		new TutorialCursorParam("Anchor_5_MC/pattern_ro_premium"),
		new TutorialCursorParam("Anchor_8_BC/pattern_st_item")
	};

	private GameObject[] m_cursorObj = new GameObject[18];

	private UIDraggablePanel m_draggablePanel;

	private Type m_type = Type.NONE;

	private bool m_optionTouch;

	private bool m_created;

	private static TutorialCursor m_instance = null;

	public static TutorialCursor Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		EntryBackKeyCallBack();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance.RemoveBackKeyCallBack();
			m_instance = null;
		}
	}

	public void Setup()
	{
		for (int i = 0; i < 18; i++)
		{
			Transform transform = base.gameObject.transform.FindChild(ParamTable[i].m_name);
			if (transform != null)
			{
				m_cursorObj[i] = transform.gameObject;
				if (m_cursorObj[i] != null)
				{
					m_cursorObj[i].SetActive(false);
				}
			}
		}
		SetUIButtonMessage(Type.OPTION);
		SetUIButtonMessage(Type.ITEMSELECT_SUBCHARA);
		m_draggablePanel = HudMenuUtility.GetMainMenuDraggablePanel();
	}

	private void SetUIButtonMessage(Type type)
	{
		if (!(m_cursorObj[(int)type] != null))
		{
			return;
		}
		Transform transform = m_cursorObj[(int)type].transform.FindChild("blinder/0_all");
		if (transform != null)
		{
			GameObject gameObject = transform.gameObject;
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.enabled = true;
				uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnOptionTouchScreen";
			}
		}
	}

	public void OnStartTutorialCursor(Type type)
	{
		m_optionTouch = false;
		m_created = true;
		BackKeyManager.AddWindowCallBack(base.gameObject);
		SetTutorialCursor(type, true);
	}

	public void OnEndTutorialCursor(Type type)
	{
		m_optionTouch = false;
		m_created = false;
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
		SetTutorialCursor(type, false);
	}

	public bool IsOptionTouchScreen()
	{
		return m_optionTouch;
	}

	public bool IsCreated()
	{
		return m_created;
	}

	private void SetTutorialCursor(Type type, bool active)
	{
		if ((uint)type >= 18u)
		{
			return;
		}
		if (active)
		{
			for (int i = 0; i < 18; i++)
			{
				if (m_cursorObj[i] != null)
				{
					if (type == (Type)i)
					{
						m_cursorObj[i].SetActive(true);
					}
					else
					{
						m_cursorObj[i].SetActive(false);
					}
				}
			}
			m_type = type;
		}
		else
		{
			if (m_cursorObj[(int)type] != null)
			{
				m_cursorObj[(int)type].SetActive(false);
			}
			m_type = Type.NONE;
		}
		SetDraggablePanel(!active);
	}

	private void SetDraggablePanel(bool on)
	{
		if (m_draggablePanel != null)
		{
			m_draggablePanel.enabled = on;
		}
	}

	private void OnOptionTouchScreen()
	{
		m_optionTouch = true;
	}

	private void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (m_created)
		{
			bool flag = false;
			switch (m_type)
			{
			case Type.OPTION:
				flag = true;
				break;
			case Type.CHARASELECT_LEVEL_UP:
				flag = true;
				break;
			case Type.ITEMSELECT_SUBCHARA:
				flag = true;
				break;
			}
			if (flag)
			{
				OnOptionTouchScreen();
				msg.StaySequence();
			}
		}
	}

	public static TutorialCursor GetTutorialCursor()
	{
		TutorialCursor instance = Instance;
		if (instance == null)
		{
			GameObject gameObject = Resources.Load("Prefabs/UI/tutorial_sign") as GameObject;
			GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
			if (gameObject != null && cameraUIObject != null)
			{
				Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
				instance = Instance;
				if (instance != null)
				{
					instance.Setup();
					Vector3 localPosition = instance.gameObject.transform.localPosition;
					Vector3 localScale = instance.gameObject.transform.localScale;
					instance.gameObject.transform.parent = cameraUIObject.transform;
					instance.gameObject.transform.localPosition = localPosition;
					instance.gameObject.transform.localScale = localScale;
				}
			}
		}
		return instance;
	}

	public static void StartTutorialCursor(Type type)
	{
		TutorialCursor tutorialCursor = GetTutorialCursor();
		if (tutorialCursor != null)
		{
			tutorialCursor.OnStartTutorialCursor(type);
		}
	}

	public static void EndTutorialCursor(Type type)
	{
		TutorialCursor tutorialCursor = GetTutorialCursor();
		if (tutorialCursor != null)
		{
			tutorialCursor.OnEndTutorialCursor(type);
		}
	}

	public static bool IsTouchScreen()
	{
		TutorialCursor tutorialCursor = GetTutorialCursor();
		if (tutorialCursor != null)
		{
			return tutorialCursor.IsOptionTouchScreen();
		}
		return false;
	}

	public static void DestroyTutorialCursor()
	{
		TutorialCursor instance = Instance;
		if (instance != null)
		{
			instance.m_created = false;
			instance.SetDraggablePanel(true);
			Object.Destroy(instance.gameObject);
		}
	}
}
