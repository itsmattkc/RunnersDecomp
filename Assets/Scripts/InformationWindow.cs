using UnityEngine;

public class InformationWindow : WindowBase
{
	public enum ButtonPattern
	{
		NONE = -1,
		TEXT,
		OK,
		FEED,
		SHOP_CANCEL,
		FEED_BROWSER,
		FEED_ROULETTE,
		FEED_SHOP,
		FEED_EVENT,
		FEED_EVENT_LIST,
		BROWSER,
		ROULETTE,
		SHOP,
		EVENT,
		EVENT_LIST,
		ROULETTE_IFNO,
		QUICK_EVENT_INFO,
		DESIGNATED_AREA_TEXT,
		DESIGNATED_AREA_IMAGE,
		NUM
	}

	public struct Information
	{
		public string imageId;

		public string caption;

		public string bodyText;

		public string url;

		public Texture2D texture;

		public ButtonPattern pattern;

		public RankingType rankingType;
	}

	public enum RankingType
	{
		NON,
		WORLD,
		LEAGUE,
		EVENT,
		QUICK_LEAGUE
	}

	public enum ButtonType
	{
		NONE = -1,
		LEFT,
		RIGHT,
		CLOSE,
		NUM
	}

	public enum ObjNameType
	{
		ROOT,
		LEFT,
		RIGHT,
		NUM
	}

	private GameObject m_prefab;

	private readonly string[,] ButtonName = new string[18, 3]
	{
		{
			"pattern_0",
			"Btn_1_ok",
			null
		},
		{
			"pattern_0",
			"Btn_1_ok",
			null
		},
		{
			"pattern_0",
			"Btn_2_post",
			null
		},
		{
			"pattern_2",
			"Btn_cancel",
			"Btn_shop"
		},
		{
			"pattern_3",
			"Btn_1_browser",
			"Btn_post"
		},
		{
			"pattern_3",
			"Btn_3_roulette",
			"Btn_post"
		},
		{
			"pattern_3",
			"Btn_4_shop",
			"Btn_post"
		},
		{
			"pattern_3",
			"Btn_5_event",
			"Btn_post"
		},
		{
			"pattern_3",
			"Btn_6_event_list",
			"Btn_post"
		},
		{
			"pattern_0",
			"Btn_3_browser",
			null
		},
		{
			"pattern_0",
			"Btn_5_roulette",
			null
		},
		{
			"pattern_0",
			"Btn_6_shop",
			null
		},
		{
			"pattern_0",
			"Btn_7_event",
			null
		},
		{
			"pattern_0",
			"Btn_8_event_list",
			null
		},
		{
			"pattern_0",
			"Btn_1_ok",
			null
		},
		{
			"pattern_0",
			"Btn_1_ok",
			null
		},
		{
			"pattern_0",
			"Btn_1_ok",
			null
		},
		{
			"pattern_0",
			"Btn_1_ok",
			null
		}
	};

	private readonly string[] CallbackFuncName = new string[3]
	{
		"OnClickLeftButton",
		"OnClickRightButton",
		"OnClickCloseButton"
	};

	private bool[] m_pressedFlag = new bool[3];

	private Information m_info;

	private RankingResultWorldRanking m_rankingResultWorld;

	private RankingResultWorldRanking m_eventRankingResult;

	private RankingResultLeague m_rankingResultLeague;

	private bool m_created;

	private bool m_endFlag;

	public UITexture bannerTexture
	{
		get
		{
			return base.gameObject.GetComponentInChildren<UITexture>();
		}
	}

	public bool IsButtonPress(ButtonType type)
	{
		if (m_pressedFlag[(int)type])
		{
			return true;
		}
		return false;
	}

	public bool IsEnd()
	{
		return m_endFlag;
	}

	private void ResetScrollBar()
	{
		if (!(m_prefab != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, "textView");
		if (gameObject != null)
		{
			UIScrollBar uIScrollBar = GameObjectUtil.FindChildGameObjectComponent<UIScrollBar>(gameObject, "Scroll_Bar");
			if (uIScrollBar != null)
			{
				uIScrollBar.value = 0f;
			}
		}
	}

	private void SetRootObjActive(string rootName, bool activeFlag)
	{
		if (m_prefab != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, rootName);
			if (gameObject != null)
			{
				gameObject.SetActive(activeFlag);
			}
		}
	}

	private void SetObjActive(GameObject obj, bool activeFlag)
	{
		if (obj != null)
		{
			obj.SetActive(activeFlag);
		}
	}

	private void SetClickBtnCallBack(GameObject buttonRoot, string objectName, string callbackFuncName)
	{
		if (!(buttonRoot != null) || string.IsNullOrEmpty(objectName))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(buttonRoot, objectName);
		if (gameObject != null)
		{
			UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				component.onFinished.Clear();
				EventDelegate.Add(component.onFinished, OnFinishedAnimationCallback, false);
			}
			UIButtonMessage component2 = gameObject.GetComponent<UIButtonMessage>();
			if (component2 != null)
			{
				component2.target = base.gameObject;
				component2.functionName = callbackFuncName;
			}
		}
	}

	private void SetActiveBtn(GameObject buttonRoot, string objectName)
	{
		if (buttonRoot != null && !string.IsNullOrEmpty(objectName))
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(buttonRoot, objectName);
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	public void SetTexture(Texture2D texture)
	{
		if (!(texture != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, "img_ad_tex");
		if (gameObject != null)
		{
			UITexture component = gameObject.GetComponent<UITexture>();
			if (component != null)
			{
				gameObject.SetActive(true);
				component.enabled = true;
				component.height = texture.height;
				component.mainTexture = texture;
			}
		}
	}

	public void Create(Information info, GameObject newsWindowObj)
	{
		m_info = info;
		m_endFlag = false;
		for (int i = 0; i < m_pressedFlag.Length; i++)
		{
			m_pressedFlag[i] = false;
		}
		switch (info.rankingType)
		{
		case RankingType.WORLD:
			CreateWorldRanking(info);
			break;
		case RankingType.LEAGUE:
			CreateLeagueRanking(info, false);
			break;
		case RankingType.QUICK_LEAGUE:
			CreateLeagueRanking(info, true);
			break;
		case RankingType.EVENT:
			CreateEvent(info);
			break;
		case RankingType.NON:
			m_created = true;
			CreateNormal(info, newsWindowObj);
			break;
		}
	}

	private void CreateLeagueRanking(Information info, bool quick)
	{
		m_rankingResultLeague = RankingResultLeague.Create(info.bodyText, quick);
	}

	private void CreateWorldRanking(Information info)
	{
		m_rankingResultWorld = RankingResultWorldRanking.GetResultWorldRanking();
		if (m_rankingResultWorld != null)
		{
			m_rankingResultWorld.Setup(RankingResultWorldRanking.ResultType.WORLD_RANKING, info.bodyText);
			m_rankingResultWorld.PlayStart();
		}
	}

	private void CreateEvent(Information info)
	{
		m_eventRankingResult = RankingResultWorldRanking.GetResultWorldRanking();
		if (m_eventRankingResult != null)
		{
			m_eventRankingResult.Setup(RankingResultWorldRanking.ResultType.EVENT_RANKING, info.bodyText);
			m_eventRankingResult.PlayStart();
		}
	}

	private void CreateNormal(Information info, GameObject newsWindowObj)
	{
		if (m_prefab == null)
		{
			m_prefab = newsWindowObj;
		}
		if (!(m_prefab != null))
		{
			return;
		}
		SetCallBack();
		m_prefab.SetActive(false);
		SetRootObjActive("pattern_0", false);
		SetRootObjActive("pattern_1", false);
		SetRootObjActive("pattern_2", false);
		SetRootObjActive("pattern_3", false);
		SetRootObjActive("pattern_close", true);
		SetRootObjActive("textView", true);
		ResetScrollBar();
		int pattern = (int)info.pattern;
		string name = ButtonName[pattern, 0];
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, name);
		SetObjActive(gameObject, true);
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			if (!(gameObject2 == null))
			{
				gameObject2.SetActive(false);
			}
		}
		SetActiveBtn(gameObject, ButtonName[pattern, 1]);
		SetActiveBtn(gameObject, ButtonName[pattern, 2]);
		GameObject buttonRoot = GameObjectUtil.FindChildGameObject(m_prefab, "pattern_close");
		SetActiveBtn(buttonRoot, "Btn_close");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_prefab, "Lbl_body");
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_prefab, "img_ad_tex");
		if (gameObject4 != null)
		{
			UITexture component = gameObject4.GetComponent<UITexture>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		SetObjActive(gameObject3, true);
		SetObjActive(gameObject4, false);
		UILabel component2 = gameObject3.GetComponent<UILabel>();
		if (component2 != null)
		{
			switch (info.rankingType)
			{
			case RankingType.NON:
				component2.text = info.bodyText;
				break;
			}
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_prefab, "Lbl_caption");
		if (uILabel != null)
		{
			uILabel.text = info.caption;
		}
		if (info.pattern != 0 && info.pattern != ButtonPattern.DESIGNATED_AREA_TEXT)
		{
			if (info.rankingType == RankingType.NON)
			{
				SetObjActive(gameObject3, false);
			}
			InformationImageManager instance = InformationImageManager.Instance;
			if (instance != null && !string.IsNullOrEmpty(info.imageId))
			{
				instance.Load(info.imageId, false, OnLoadedTextureCallback);
			}
		}
		SetESRB(info.pattern, gameObject);
		PlayAnimation();
		base.enabled = true;
	}

	private void PlayAnimation()
	{
		if (m_prefab != null)
		{
			m_prefab.SetActive(true);
			UIPlayAnimation uIPlayAnimation = base.gameObject.GetComponent<UIPlayAnimation>();
			if (uIPlayAnimation == null)
			{
				uIPlayAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
			}
			if (uIPlayAnimation != null)
			{
				Animation animation = uIPlayAnimation.target = m_prefab.GetComponent<Animation>();
				uIPlayAnimation.clipName = "ui_cmn_window_Anim";
				uIPlayAnimation.Play(true);
			}
		}
	}

	private void OnClickLeftButton()
	{
		m_pressedFlag[0] = true;
		m_created = false;
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickRightButton()
	{
		m_pressedFlag[1] = true;
		m_created = false;
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickCloseButton()
	{
		m_pressedFlag[2] = true;
		m_created = false;
		SoundManager.SePlay("sys_window_close");
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void Update()
	{
		switch (m_info.rankingType)
		{
		case RankingType.WORLD:
			if (m_rankingResultWorld != null && m_rankingResultWorld.IsEnd)
			{
				m_endFlag = true;
				base.enabled = false;
			}
			break;
		case RankingType.LEAGUE:
		case RankingType.QUICK_LEAGUE:
			if (m_rankingResultLeague != null && m_rankingResultLeague.IsEnd())
			{
				m_endFlag = true;
				base.enabled = false;
			}
			break;
		case RankingType.EVENT:
			if (m_eventRankingResult != null && m_eventRankingResult.IsEnd)
			{
				m_endFlag = true;
				base.enabled = false;
			}
			break;
		case RankingType.NON:
			base.enabled = false;
			break;
		}
	}

	private void SetESRB(ButtonPattern pattern, GameObject parentObj)
	{
		if (!(parentObj != null))
		{
			return;
		}
		GameObject gameObject = null;
		switch (pattern)
		{
		default:
			return;
		case ButtonPattern.SHOP_CANCEL:
			return;
		case ButtonPattern.FEED:
			gameObject = GameObjectUtil.FindChildGameObject(parentObj, ButtonName[(int)pattern, 1]);
			break;
		case ButtonPattern.FEED_BROWSER:
		case ButtonPattern.FEED_ROULETTE:
		case ButtonPattern.FEED_SHOP:
		case ButtonPattern.FEED_EVENT:
		case ButtonPattern.FEED_EVENT_LIST:
			gameObject = GameObjectUtil.FindChildGameObject(parentObj, ButtonName[(int)pattern, 2]);
			break;
		}
		if (gameObject != null && RegionManager.Instance != null && !RegionManager.Instance.IsUseSNS())
		{
			UIImageButton component = gameObject.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = false;
			}
		}
	}

	private void SetCallBack()
	{
		SetClickBtnCallBack(m_prefab, "Btn_close", CallbackFuncName[2]);
		GameObject buttonRoot = GameObjectUtil.FindChildGameObject(m_prefab, "pattern_0");
		SetClickBtnCallBack(buttonRoot, "Btn_1_ok", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_2_post", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_3_browser", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_5_roulette", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_6_shop", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_7_event", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot, "Btn_8_event_list", CallbackFuncName[1]);
		GameObject buttonRoot2 = GameObjectUtil.FindChildGameObject(m_prefab, "pattern_2");
		SetClickBtnCallBack(buttonRoot2, "Btn_shop", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot2, "Btn_cancel", CallbackFuncName[0]);
		GameObject buttonRoot3 = GameObjectUtil.FindChildGameObject(m_prefab, "pattern_3");
		SetClickBtnCallBack(buttonRoot3, "Btn_post", CallbackFuncName[1]);
		SetClickBtnCallBack(buttonRoot3, "Btn_0_unpost", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_1_browser", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_2_item", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_3_roulette", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_4_shop", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_5_event", CallbackFuncName[0]);
		SetClickBtnCallBack(buttonRoot3, "Btn_6_event_list", CallbackFuncName[0]);
	}

	private void OnFinishedAnimationCallback()
	{
		if (m_prefab != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, "img_ad_tex");
			if (gameObject != null)
			{
				UITexture component = gameObject.GetComponent<UITexture>();
				if (component != null && component.mainTexture != null)
				{
					component.mainTexture = null;
				}
			}
			m_prefab.SetActive(false);
		}
		m_endFlag = true;
	}

	private void OnLoadedTextureCallback(Texture2D texture)
	{
		SetTexture(texture);
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (!m_created)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (!(m_prefab != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_prefab, "Btn_close");
		if (gameObject != null && gameObject.activeSelf)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
