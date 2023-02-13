using SaveData;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
	public class OptionInfo
	{
		public string label;

		public string icon;

		public OptionType type;
	}

	[SerializeField]
	private UIRectItemStorage m_itemStorage;

	[SerializeField]
	private UIScrollBar m_scrollBar;

	private GameObject m_backButtonObj;

	private bool m_inited;

	private bool m_systemSaveFlag;

	public readonly OptionInfo[] m_optionInfos = new OptionInfo[18]
	{
		new OptionInfo
		{
			label = "users_results",
			icon = "ui_option_icon_note"
		},
		new OptionInfo
		{
			label = "buying_info",
			icon = "ui_option_icon_note",
			type = OptionType.BUYING_HISTORY
		},
		new OptionInfo
		{
			label = "push_notification",
			icon = "ui_option_icon_gear",
			type = OptionType.PUSH_NOTIFICATION
		},
		new OptionInfo
		{
			label = "weight_saving",
			icon = "ui_option_icon_gear",
			type = OptionType.WEIGHT_SAVING
		},
		new OptionInfo
		{
			label = "id_check",
			icon = "ui_option_icon_note",
			type = OptionType.ID_CHECK
		},
		new OptionInfo
		{
			label = "tutorial",
			icon = "ui_option_icon_note",
			type = OptionType.TUTORIAL
		},
		new OptionInfo
		{
			label = "past_results",
			icon = "ui_option_icon_arrow",
			type = OptionType.PAST_RESULTS
		},
		new OptionInfo
		{
			label = "staff_credit",
			icon = "ui_option_icon_note",
			type = OptionType.STAFF_CREDIT
		},
		new OptionInfo
		{
			label = "terms_of_service",
			icon = "ui_option_icon_arrow",
			type = OptionType.TERMS_OF_SERVICE
		},
		new OptionInfo
		{
			label = "user_name",
			icon = "ui_option_icon_gear",
			type = OptionType.USER_NAME
		},
		new OptionInfo
		{
			label = "sound_config",
			icon = "ui_option_icon_gear",
			type = OptionType.SOUND_CONFIG
		},
		new OptionInfo
		{
			label = "invite_friend",
			icon = "ui_option_icon_gear",
			type = OptionType.INVITE_FRIEND
		},
		new OptionInfo
		{
			label = "acceptance_of_invite",
			icon = "ui_option_icon_gear",
			type = OptionType.ACCEPT_INVITE
		},
		new OptionInfo
		{
			label = "facebook_access",
			icon = "ui_option_icon_gear",
			type = OptionType.FACEBOOK_ACCESS
		},
		new OptionInfo
		{
			label = "help",
			icon = "ui_option_icon_arrow",
			type = OptionType.HELP
		},
		new OptionInfo
		{
			label = "cash_clear",
			icon = "ui_option_icon_gear",
			type = OptionType.CACHE_CLEAR
		},
		new OptionInfo
		{
			label = "copyright",
			icon = "ui_option_icon_note",
			type = OptionType.COPYRIGHT
		},
		new OptionInfo
		{
			label = "back_title",
			icon = "ui_option_icon_arrow",
			type = OptionType.BACK_TITLE
		}
	};

	public bool SystemSaveFlag
	{
		set
		{
			m_systemSaveFlag = value;
		}
	}

	public bool IsEndSetup
	{
		get
		{
			return m_inited;
		}
	}

	private void Start()
	{
		base.enabled = false;
		m_backButtonObj = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_cmn_back");
	}

	private void Initialize()
	{
		if (!m_inited)
		{
			UpdateRectItemStorage();
			m_inited = true;
		}
		m_scrollBar.value = 0f;
		m_systemSaveFlag = false;
		CheckESRBButtonView();
	}

	private void UpdateRectItemStorage()
	{
		if (!(m_itemStorage != null))
		{
			return;
		}
		int num = m_optionInfos.Length;
		m_itemStorage.maxItemCount = num;
		int num2 = num % m_itemStorage.maxColumns;
		m_itemStorage.maxRows = num / m_itemStorage.maxColumns + num2;
		m_itemStorage.Restart();
		ui_option_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_scroll>(true);
		int num3 = componentsInChildren.Length;
		int num4 = m_itemStorage.maxRows * m_itemStorage.maxColumns;
		for (int i = 0; i < num4; i++)
		{
			if (i < num3 && i < m_itemStorage.maxItemCount)
			{
				componentsInChildren[i].gameObject.SetActive(true);
				componentsInChildren[i].UpdateView(m_optionInfos[i], this);
			}
			else
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}
		}
	}

	public void SetButtonTrigger(bool flag)
	{
		ui_option_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_scroll>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(componentsInChildren[i].gameObject, "Btn_option_top");
			if (gameObject != null)
			{
				BoxCollider component = gameObject.GetComponent<BoxCollider>();
				if (component != null)
				{
					component.isTrigger = flag;
				}
			}
		}
		if (m_backButtonObj != null)
		{
			BoxCollider component2 = m_backButtonObj.GetComponent<BoxCollider>();
			if (component2 != null)
			{
				component2.isTrigger = flag;
			}
		}
	}

	private void OnGUI()
	{
	}

	private void CheckESRBButtonView()
	{
		RegionManager instance = RegionManager.Instance;
		ui_option_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_scroll>(true);
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (componentsInChildren[i].OptionInfo != null && (componentsInChildren[i].OptionInfo.type == OptionType.FACEBOOK_ACCESS || componentsInChildren[i].OptionInfo.type == OptionType.PUSH_NOTIFICATION))
			{
				bool enableImageButton = false;
				if (instance != null && instance.IsUseSNS())
				{
					enableImageButton = true;
				}
				componentsInChildren[i].SetEnableImageButton(enableImageButton);
			}
		}
	}

	private void OnStartOptionUI()
	{
		Initialize();
	}

	private void OnEndOptionUI()
	{
		if (m_systemSaveFlag)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				instance.SaveSystemData();
			}
			m_systemSaveFlag = false;
		}
	}
}
