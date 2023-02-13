using Text;
using UnityEngine;

public class window_tutorial_other_character : WindowBase
{
	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private GameObject m_nextBtn;

	[SerializeField]
	private UIImageButton m_rightImageBtn;

	[SerializeField]
	private UIImageButton m_leftImageBtn;

	[SerializeField]
	private UITexture m_texture;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_mainTextLabel;

	[SerializeField]
	private UILabel m_nextTextLabel;

	private ResourceSceneLoader m_loaderComponent;

	private bool m_isEnd;

	private bool m_isLoading;

	private bool m_playOpen;

	private int m_pageCount;

	private int m_pageIndex;

	private UIPlayAnimation m_uiAnimation;

	private window_tutorial.ScrollInfo m_scrollInfo;

	private string m_sceneName;

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
		if (m_closeBtn != null)
		{
			UIPlayAnimation component = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedAnimationCallback, false);
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component2 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component2;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		if (m_texture != null)
		{
			m_texture.enabled = false;
		}
	}

	public void SetScrollInfo(window_tutorial.ScrollInfo info)
	{
		m_scrollInfo = info;
		m_isLoading = false;
		m_isEnd = false;
		if (m_texture != null)
		{
			m_texture.enabled = false;
		}
		m_pageIndex = 0;
		m_pageCount = 0;
		if (m_scrollInfo != null)
		{
			m_pageCount = HudTutorial.GetTexuterPageCount(m_scrollInfo.HudId);
		}
		CheckLoadTexture();
	}

	public void PlayOpenWindow()
	{
		m_playOpen = true;
		base.enabled = true;
		base.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (m_playOpen)
		{
			SetupText();
			SetNextBtn();
			if (m_uiAnimation != null)
			{
				m_uiAnimation.Play(true);
			}
			SoundManager.SePlay("sys_window_open");
			m_playOpen = false;
		}
		if (m_isLoading && m_loaderComponent != null && m_loaderComponent.Loaded)
		{
			SetupTexture();
			m_loaderComponent = null;
			m_isLoading = false;
		}
		if (!m_playOpen && !m_isLoading)
		{
			base.enabled = false;
		}
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private void OnClickLeftButton()
	{
		m_pageIndex--;
		if (m_pageIndex < 0)
		{
			m_pageIndex = 0;
		}
		SetupTexture();
		SetupText();
		SetPage();
		SoundManager.SePlay("sys_page_skip");
	}

	private void OnClickRightButton()
	{
		m_pageIndex++;
		if (m_pageIndex == m_pageCount)
		{
			m_pageIndex = m_pageCount - 1;
		}
		SetupTexture();
		SetupText();
		SetPage();
		SoundManager.SePlay("sys_page_skip");
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	private void CheckLoadTexture()
	{
		if (m_scrollInfo == null)
		{
			return;
		}
		switch (m_scrollInfo.DispType)
		{
		case window_tutorial.DisplayType.QUICK:
			m_sceneName = HudTutorial.GetLoadQuickModeSceneName(m_scrollInfo.HudId);
			break;
		case window_tutorial.DisplayType.CHARA:
			m_sceneName = HudTutorial.GetLoadSceneName(m_scrollInfo.Chara);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_1:
			m_sceneName = HudTutorial.GetLoadSceneName(BossType.MAP1);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_2:
			m_sceneName = HudTutorial.GetLoadSceneName(BossType.MAP2);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_3:
			m_sceneName = HudTutorial.GetLoadSceneName(BossType.MAP3);
			break;
		}
		if (!string.IsNullOrEmpty(m_sceneName))
		{
			GameObject x = GameObject.Find(m_sceneName);
			if (x == null)
			{
				LoadTexture(m_sceneName);
			}
			else
			{
				SetupTexture();
			}
		}
	}

	private void SetupTexture()
	{
		if (m_scrollInfo == null || string.IsNullOrEmpty(m_sceneName))
		{
			return;
		}
		GameObject gameObject = GameObject.Find(m_sceneName);
		if (!(gameObject != null))
		{
			return;
		}
		HudTutorialTexture component = gameObject.GetComponent<HudTutorialTexture>();
		if (component != null && m_texture != null)
		{
			uint num = (uint)component.m_texList.Length;
			if ((uint)m_pageIndex < num)
			{
				m_texture.mainTexture = component.m_texList[m_pageIndex];
				m_texture.enabled = true;
			}
		}
	}

	private void LoadTexture(string sceneName)
	{
		if (!m_isLoading)
		{
			if (m_loaderComponent == null)
			{
				m_loaderComponent = base.gameObject.AddComponent<ResourceSceneLoader>();
			}
			if (m_loaderComponent != null)
			{
				m_loaderComponent.AddLoad(sceneName, true, false);
				m_isLoading = true;
			}
		}
	}

	private void SetupText()
	{
		if (m_scrollInfo != null)
		{
			string text = string.Empty;
			switch (m_scrollInfo.DispType)
			{
			case window_tutorial.DisplayType.QUICK:
			{
				string commonText2 = TextUtility.GetCommonText("Tutorial", "caption_quickmode_tutorial");
				text = TextUtility.GetCommonText("Tutorial", "caption_explan2", "{PARAM_NAME}", commonText2);
				break;
			}
			case window_tutorial.DisplayType.CHARA:
			{
				string cellID = CharaName.Name[(int)m_scrollInfo.Chara];
				string commonText = TextUtility.GetCommonText("CharaName", cellID);
				text = TextUtility.GetCommonText("Option", "chara_operation_method", "{CHARA_NAME}", commonText);
				break;
			}
			case window_tutorial.DisplayType.BOSS_MAP_1:
			{
				string textCommonBossName3 = BossTypeUtil.GetTextCommonBossName(BossType.MAP1);
				text = TextUtility.GetCommonText("Option", "boss_attack_method", "{BOSS_NAME}", textCommonBossName3);
				break;
			}
			case window_tutorial.DisplayType.BOSS_MAP_2:
			{
				string textCommonBossName2 = BossTypeUtil.GetTextCommonBossName(BossType.MAP2);
				text = TextUtility.GetCommonText("Option", "boss_attack_method", "{BOSS_NAME}", textCommonBossName2);
				break;
			}
			case window_tutorial.DisplayType.BOSS_MAP_3:
			{
				string textCommonBossName = BossTypeUtil.GetTextCommonBossName(BossType.MAP3);
				text = TextUtility.GetCommonText("Option", "boss_attack_method", "{BOSS_NAME}", textCommonBossName);
				break;
			}
			}
			if (m_headerTextLabel != null)
			{
				m_headerTextLabel.text = text;
			}
			if (m_mainTextLabel != null)
			{
				m_mainTextLabel.text = HudTutorial.GetExplainText(m_scrollInfo.HudId, m_pageIndex);
			}
		}
	}

	private void SetNextBtn()
	{
		if (m_scrollInfo != null && m_nextBtn != null)
		{
			bool flag = m_pageCount > 1;
			m_nextBtn.SetActive(flag);
			if (flag)
			{
				SetPage();
			}
		}
	}

	private void SetPage()
	{
		if (m_nextTextLabel != null)
		{
			int num = m_pageIndex + 1;
			m_nextTextLabel.text = num + "/" + m_pageCount;
		}
		if (m_rightImageBtn != null)
		{
			m_rightImageBtn.isEnabled = (m_pageCount != m_pageIndex + 1);
		}
		if (m_leftImageBtn != null)
		{
			m_leftImageBtn.isEnabled = (m_pageIndex != 0);
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.SendMessage("OnClick");
		}
	}
}
