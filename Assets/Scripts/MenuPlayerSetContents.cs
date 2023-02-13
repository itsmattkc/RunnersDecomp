using SaveData;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MenuPlayerSetContents : MenuPlayerSetPartsBase
{
	public delegate void PageChangeCallback(int pageIndex);

	private MenuPlayerSetCharaPage[] m_charaPage = new MenuPlayerSetCharaPage[29];

	private MenuPlayerSetShortCutMenu m_shortCutMenu;

	private HudScrollBar m_scrollBar;

	private PageChangeCallback m_callback;

	private bool m_isEndSetup;

	public bool IsEndSetup
	{
		get
		{
			return m_isEndSetup;
		}
		private set
		{
		}
	}

	public MenuPlayerSetContents()
		: base("player_set_contents")
	{
	}

	public void SetCallback(PageChangeCallback callback)
	{
		m_callback = callback;
	}

	protected override void OnSetup()
	{
		StartCoroutine(SetupCoroutine());
	}

	private void OnDestroy()
	{
		MenuPlayerSetCharaPage[] charaPage = m_charaPage;
		foreach (MenuPlayerSetCharaPage menuPlayerSetCharaPage in charaPage)
		{
			if (!(menuPlayerSetCharaPage == null))
			{
				Object.Destroy(menuPlayerSetCharaPage);
			}
		}
		if (m_scrollBar != null)
		{
			m_scrollBar.SetPageChangeCallback(null);
			Object.Destroy(m_scrollBar);
		}
	}

	private IEnumerator SetupCoroutine()
	{
		m_isEndSetup = false;
		GameObject playerSetRoot = MenuPlayerSetUtil.GetPlayerSetRoot();
		GameObject pageRootParent = GameObjectUtil.FindChildGameObject(playerSetRoot, "grid_slot");
		List<GameObject> pageList2 = null;
		int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
		while (true)
		{
			pageList2 = GameObjectUtil.FindChildGameObjects(pageRootParent, "ui_player_set_2_cell(Clone)");
			if (pageList2 == null || pageList2.Count >= openedCharaCount)
			{
				break;
			}
			yield return null;
		}
		for (int index2 = 0; index2 < pageList2.Count; index2++)
		{
			GameObject pageObject = pageList2[index2];
			if (pageObject == null)
			{
				continue;
			}
			MenuPlayerSetCharaPage page = pageObject.AddComponent<MenuPlayerSetCharaPage>();
			CharaType charaType = MenuPlayerSetUtil.GetCharaTypeFromPageIndex(index2);
			if (charaType != CharaType.UNKNOWN)
			{
				page.Setup(pageObject, charaType);
				while (!page.IsEndSetUp)
				{
					yield return null;
				}
				MenuPlayerSetUtil.ActivateCharaPageObjects(page.gameObject, false);
				m_charaPage[index2] = page;
			}
		}
		yield return null;
		for (int index = 0; index < pageList2.Count; index++)
		{
			GameObject pageObject2 = pageList2[index];
			if (!(pageObject2 == null))
			{
				MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[index].gameObject, false);
			}
		}
		GameObject gripRootObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_pager_alpha_clip");
		if (gripRootObject != null)
		{
			m_shortCutMenu = gripRootObject.AddComponent<MenuPlayerSetShortCutMenu>();
			if (m_shortCutMenu != null)
			{
				m_shortCutMenu.Setup(ShortCutButtonClickedCallback);
			}
			while (!(m_shortCutMenu != null) || !m_shortCutMenu.IsEndSetup)
			{
				yield return null;
			}
		}
		if (m_scrollBar == null)
		{
			m_scrollBar = base.gameObject.AddComponent<HudScrollBar>();
		}
		UIScrollBar scrollBar = GameObjectUtil.FindChildGameObjectComponent<UIScrollBar>(base.gameObject, "player_set_SB");
		int pageCount = pageList2.Count;
		m_scrollBar.Setup(scrollBar, pageCount);
		m_scrollBar.SetPageChangeCallback(ChangePageCallback);
		m_isEndSetup = true;
	}

	public void JumpCharacterPage(int pageIndex)
	{
		StartCoroutine(JumpCharaPageCoroutine(pageIndex));
	}

	public void ChangeMainChara(CharaType newCharaType)
	{
		PlayerData playerData = SaveDataManager.Instance.PlayerData;
		CharaType mainChara = playerData.MainChara;
		if (mainChara == newCharaType)
		{
			return;
		}
		CharaType subChara = playerData.SubChara;
		if (subChara == newCharaType)
		{
			CharaType mainChara2 = playerData.MainChara;
			playerData.MainChara = playerData.SubChara;
			playerData.SubChara = mainChara2;
		}
		else
		{
			if (playerData.SubChara == CharaType.UNKNOWN)
			{
				playerData.SubChara = playerData.MainChara;
			}
			playerData.MainChara = newCharaType;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			int mainCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(playerData.MainChara);
			if (serverCharacterState != null)
			{
				mainCharaId = serverCharacterState.Id;
			}
			int subCharaId = -1;
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(playerData.SubChara);
			if (serverCharacterState2 != null)
			{
				subCharaId = serverCharacterState2.Id;
			}
			loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
		}
		UpdateRibbon();
	}

	public void ChangeSubChara(CharaType newCharaType)
	{
		PlayerData playerData = SaveDataManager.Instance.PlayerData;
		CharaType subChara = playerData.SubChara;
		if (subChara == newCharaType)
		{
			return;
		}
		CharaType mainChara = playerData.MainChara;
		if (mainChara == newCharaType)
		{
			CharaType subChara2 = playerData.SubChara;
			if (subChara2 == CharaType.UNKNOWN)
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PlayerSet", "ui_Lbl_player_config").text;
				string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "change_chara_error").text;
				info.caption = text;
				info.message = text2;
				info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.isPlayErrorSe = true;
				GeneralWindow.Create(info);
				return;
			}
			playerData.SubChara = playerData.MainChara;
			playerData.MainChara = subChara2;
		}
		playerData.SubChara = newCharaType;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			int mainCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(playerData.MainChara);
			if (serverCharacterState != null)
			{
				mainCharaId = serverCharacterState.Id;
			}
			int subCharaId = -1;
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(playerData.SubChara);
			if (serverCharacterState2 != null)
			{
				subCharaId = serverCharacterState2.Id;
			}
			loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
		}
		UpdateRibbon();
	}

	public void UnlockedChara(CharaType unlockedChara)
	{
		if (MenuPlayerSetUtil.GetPlayableCharaCount() == 2)
		{
			ChangeSubChara(unlockedChara);
		}
		else
		{
			int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(unlockedChara);
			MenuPlayerSetCharaPage menuPlayerSetCharaPage = m_charaPage[pageIndexFromCharaType];
			if (menuPlayerSetCharaPage != null)
			{
				menuPlayerSetCharaPage.OnSelected();
			}
		}
		if (m_shortCutMenu != null)
		{
			m_shortCutMenu.UnclockCharacter(unlockedChara);
		}
		AchievementManager.RequestUpdate();
	}

	protected override void OnPlayStart()
	{
	}

	protected override void OnPlayEnd()
	{
	}

	protected override void OnUpdate(float deltaTime)
	{
		if (GeneralWindow.IsOkButtonPressed)
		{
			GeneralWindow.Close();
		}
	}

	private IEnumerator JumpCharaPageCoroutine(int pageIndex)
	{
		while (!m_isEndSetup)
		{
			yield return null;
		}
		if (m_scrollBar != null)
		{
			m_scrollBar.PageJump(pageIndex, true);
		}
	}

	public void UpdateRibbon()
	{
		for (int i = 0; i < 29; i++)
		{
			MenuPlayerSetCharaPage menuPlayerSetCharaPage = m_charaPage[i];
			if (!(menuPlayerSetCharaPage == null))
			{
				menuPlayerSetCharaPage.UpdateRibbon();
			}
		}
	}

	private void ShortCutButtonClickedCallback(CharaType charaType)
	{
		if (m_scrollBar != null)
		{
			int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(charaType);
			m_scrollBar.PageJump(pageIndexFromCharaType, false);
		}
	}

	private void ChangePageCallback(int prevPage, int currentPage)
	{
		int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
		int num = Mathf.Clamp(prevPage - 1, 0, openedCharaCount - 1);
		int num2 = Mathf.Clamp(prevPage + 1, 0, openedCharaCount - 1);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[num].gameObject, false);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[prevPage].gameObject, false);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[num2].gameObject, false);
		int num3 = Mathf.Clamp(currentPage - 1, 0, openedCharaCount - 1);
		int num4 = Mathf.Clamp(currentPage + 1, 0, openedCharaCount - 1);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[num3].gameObject, true);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[currentPage].gameObject, true);
		MenuPlayerSetUtil.ActivateCharaPageObjects(m_charaPage[num4].gameObject, true);
		if (m_shortCutMenu != null)
		{
			CharaType charaTypeFromPageIndex = MenuPlayerSetUtil.GetCharaTypeFromPageIndex(currentPage);
			m_shortCutMenu.SetActiveCharacter(charaTypeFromPageIndex, false);
		}
		if (m_callback != null)
		{
			m_callback(currentPage);
		}
		SoundManager.SePlay("sys_page_skip");
	}
}
