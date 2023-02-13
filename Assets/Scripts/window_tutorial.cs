using SaveData;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class window_tutorial : WindowBase
{
	public enum DisplayType
	{
		TUTORIAL,
		QUICK,
		CHARA,
		BOSS_MAP_1,
		BOSS_MAP_2,
		BOSS_MAP_3,
		UNKNOWN
	}

	public class ScrollInfo
	{
		private DisplayType m_dispType = DisplayType.UNKNOWN;

		private CharaType m_charaType = CharaType.UNKNOWN;

		private HudTutorial.Id m_hudId = HudTutorial.Id.NONE;

		private window_tutorial m_parentWindow;

		public DisplayType DispType
		{
			get
			{
				return m_dispType;
			}
		}

		public CharaType Chara
		{
			get
			{
				return m_charaType;
			}
		}

		public HudTutorial.Id HudId
		{
			get
			{
				return m_hudId;
			}
		}

		public window_tutorial Parent
		{
			get
			{
				return m_parentWindow;
			}
		}

		public ScrollInfo()
		{
		}

		public ScrollInfo(window_tutorial window, DisplayType dispType, CharaType charaType = CharaType.UNKNOWN)
		{
			m_parentWindow = window;
			m_dispType = dispType;
			m_charaType = charaType;
			switch (m_dispType)
			{
			case DisplayType.QUICK:
				m_hudId = HudTutorial.Id.QUICK_1;
				break;
			case DisplayType.CHARA:
				m_hudId = CharaTypeUtil.GetCharacterTutorialID(m_charaType);
				break;
			case DisplayType.BOSS_MAP_1:
				m_hudId = HudTutorial.Id.MAPBOSS_1;
				break;
			case DisplayType.BOSS_MAP_2:
				m_hudId = HudTutorial.Id.MAPBOSS_2;
				break;
			case DisplayType.BOSS_MAP_3:
				m_hudId = HudTutorial.Id.MAPBOSS_3;
				break;
			}
		}
	}

	private readonly BossType[] TUTORIAL_BOSS_TYPE_TABLE = new BossType[2]
	{
		BossType.MAP2,
		BossType.MAP3
	};

	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UIRectItemStorage m_itemStorage;

	[SerializeField]
	private UILabel m_headerTextLabel;

	private bool m_isEnd;

	private bool m_inited;

	private UIPlayAnimation m_uiAnimation;

	private List<ScrollInfo> m_scrollInfos = new List<ScrollInfo>();

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
		OptionMenuUtility.TranObj(base.gameObject);
		base.enabled = false;
		if (m_closeBtn != null)
		{
			UIPlayAnimation component = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedCloseAnimationCallback, false);
			}
			UIButtonMessage uIButtonMessage = m_closeBtn.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_closeBtn.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.enabled = true;
				uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickCloseButton";
			}
		}
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "tutorial");
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component2 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component2;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		if (ServerInterface.MileageMapState != null)
		{
			if (!HudMenuUtility.IsSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_2) && ServerInterface.MileageMapState.m_episode > 2)
			{
				HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_2);
			}
			if (!HudMenuUtility.IsSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_3) && ServerInterface.MileageMapState.m_episode > 3)
			{
				HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_3);
			}
		}
	}

	private void OnClickCloseButton()
	{
		SetCloseBtnColliderTrigger(true);
		SoundManager.SePlay("sys_window_close");
	}

	private void OnFinishedInAnimationCallback()
	{
		SetCloseBtnColliderTrigger(false);
	}

	private void OnFinishedCloseAnimationCallback()
	{
		m_isEnd = true;
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		if (m_inited)
		{
			UpdateRectItemStorage();
		}
		else
		{
			StartCoroutine(DelayUpdateRectItemStorage());
		}
		if (m_uiAnimation != null)
		{
			EventDelegate.Add(m_uiAnimation.onFinished, OnFinishedInAnimationCallback, false);
			m_uiAnimation.Play(true);
		}
		SoundManager.SePlay("sys_window_open");
	}

	public IEnumerator DelayUpdateRectItemStorage()
	{
		int waite_frame = 1;
		while (waite_frame > 0)
		{
			waite_frame--;
			yield return null;
		}
		UpdateRectItemStorage();
		m_inited = true;
	}

	private void UpdateRectItemStorage()
	{
		if (!(m_itemStorage != null))
		{
			return;
		}
		m_scrollInfos.Clear();
		m_scrollInfos.Add(new ScrollInfo(this, DisplayType.TUTORIAL));
		m_scrollInfos.Add(new ScrollInfo(this, DisplayType.QUICK));
		if (SystemSaveManager.Instance != null)
		{
			SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
			if (systemdata != null)
			{
				m_scrollInfos.Add(new ScrollInfo(this, DisplayType.CHARA, CharaType.SONIC));
				for (int i = 1; i < 29; i++)
				{
					SystemData.CharaTutorialFlagStatus characterSaveDataFlagStatus = CharaTypeUtil.GetCharacterSaveDataFlagStatus((CharaType)i);
					if (systemdata.IsFlagStatus(characterSaveDataFlagStatus))
					{
						m_scrollInfos.Add(new ScrollInfo(this, DisplayType.CHARA, (CharaType)i));
					}
				}
				m_scrollInfos.Add(new ScrollInfo(this, DisplayType.BOSS_MAP_1));
				SystemData.FlagStatus bossSaveDataFlagStatus = BossTypeUtil.GetBossSaveDataFlagStatus(BossType.MAP2);
				if (systemdata.IsFlagStatus(bossSaveDataFlagStatus))
				{
					m_scrollInfos.Add(new ScrollInfo(this, DisplayType.BOSS_MAP_2));
				}
				SystemData.FlagStatus bossSaveDataFlagStatus2 = BossTypeUtil.GetBossSaveDataFlagStatus(BossType.MAP3);
				if (systemdata.IsFlagStatus(bossSaveDataFlagStatus2))
				{
					m_scrollInfos.Add(new ScrollInfo(this, DisplayType.BOSS_MAP_3));
				}
			}
		}
		m_itemStorage.maxItemCount = m_scrollInfos.Count;
		m_itemStorage.maxRows = m_scrollInfos.Count;
		m_itemStorage.Restart();
		ui_option_tutorial_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_tutorial_scroll>(true);
		int num = componentsInChildren.Length;
		for (int j = 0; j < num; j++)
		{
			componentsInChildren[j].gameObject.SetActive(true);
			componentsInChildren[j].UpdateView(m_scrollInfos[j]);
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		bool flag = false;
		ui_option_tutorial_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_tutorial_scroll>(true);
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (componentsInChildren[i].OpenWindow)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}

	public void SetCloseBtnColliderTrigger(bool triggerFlag)
	{
		if (m_closeBtn != null)
		{
			BoxCollider component = m_closeBtn.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.isTrigger = triggerFlag;
			}
		}
		ui_option_tutorial_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_tutorial_scroll>(true);
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(componentsInChildren[i].gameObject, "Btn_character");
			if (boxCollider != null)
			{
				boxCollider.isTrigger = triggerFlag;
			}
		}
	}
}
