using SaveData;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MenuPlayerSet : MonoBehaviour
{
	private enum TutorialMode
	{
		Idle,
		WaitWindow,
		WaitClickLevelUpButton,
		EndClickLevelUpButton
	}

	private enum State
	{
		NOT_SETUP,
		SETUPING,
		SETUPED
	}

	private TutorialMode m_tutorialMode;

	private List<MenuPlayerSetPartsBase> m_partsList;

	private SendApollo m_sendApollo;

	private State m_state;

	private int m_currentPage;

	public bool SetUpped
	{
		get
		{
			if (m_state == State.SETUPED)
			{
				return true;
			}
			return false;
		}
	}

	public void StartMainCharacter()
	{
		if (MenuPlayerSetUtil.IsMarkCharaPage())
		{
			int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(MenuPlayerSetUtil.MarkCharaType);
			StartCoroutine(JumpCharacterPage(pageIndexFromCharaType));
			MenuPlayerSetUtil.ResetMarkCharaPage();
		}
		else
		{
			CharaType mainChara = SaveDataManager.Instance.PlayerData.MainChara;
			int pageIndexFromCharaType2 = MenuPlayerSetUtil.GetPageIndexFromCharaType(mainChara);
			StartCoroutine(JumpCharacterPage(pageIndexFromCharaType2));
		}
		if (HudMenuUtility.IsTutorial_CharaLevelUp())
		{
			PrepareTutorialLevelUp();
		}
	}

	public void StartSubCharacter()
	{
		if (MenuPlayerSetUtil.IsMarkCharaPage())
		{
			int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(MenuPlayerSetUtil.MarkCharaType);
			StartCoroutine(JumpCharacterPage(pageIndexFromCharaType));
			MenuPlayerSetUtil.ResetMarkCharaPage();
		}
		else
		{
			CharaType subChara = SaveDataManager.Instance.PlayerData.SubChara;
			int pageIndexFromCharaType2 = MenuPlayerSetUtil.GetPageIndexFromCharaType(subChara);
			StartCoroutine(JumpCharacterPage(pageIndexFromCharaType2));
		}
		if (HudMenuUtility.IsTutorial_CharaLevelUp())
		{
			PrepareTutorialLevelUp();
		}
	}

	public void StartCharacter(CharaType type)
	{
		int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(type);
		StartCoroutine(JumpCharacterPage(pageIndexFromCharaType));
		if (HudMenuUtility.IsTutorial_CharaLevelUp())
		{
			PrepareTutorialLevelUp();
		}
	}

	private IEnumerator JumpCharacterPage(int pageIndex)
	{
		while (m_state != State.SETUPED)
		{
			yield return null;
		}
		MenuPlayerSetContents contetns = base.gameObject.GetComponent<MenuPlayerSetContents>();
		if (contetns != null)
		{
			contetns.JumpCharacterPage(pageIndex);
		}
	}

	private void PrepareTutorialLevelUp()
	{
		HudMenuUtility.SetConnectAlertSimpleUI(true);
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "chara_level_up_explan";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetCommonText("MainMenu", "chara_level_up_explan_caption");
		info.message = TextUtility.GetCommonText("MainMenu", "chara_level_up_explan");
		info.finishedCloseDelegate = GeneralWindowCharaLevelUpCloseCallback;
		GeneralWindow.Create(info);
		string[] value = new string[1];
		SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP7, ref value);
		m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
		m_tutorialMode = TutorialMode.WaitWindow;
	}

	private void GeneralWindowCharaLevelUpCloseCallback()
	{
		HudMenuUtility.SetConnectAlertSimpleUI(false);
		TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHARASELECT_LEVEL_UP);
		m_tutorialMode = TutorialMode.WaitClickLevelUpButton;
	}

	private void OnClickedLevelUpButton()
	{
		TutorialCursor.DestroyTutorialCursor();
		HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.CHARA_LEVEL_UP_EXPLAINED);
		string[] value = new string[1];
		SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP7, ref value);
		m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
		m_tutorialMode = TutorialMode.EndClickLevelUpButton;
	}

	private void Start()
	{
		m_partsList = new List<MenuPlayerSetPartsBase>();
		StartCoroutine(Setup());
	}

	private void OnEnable()
	{
		StartCoroutine(ReSetUp());
	}

	private void Update()
	{
		switch (m_tutorialMode)
		{
		case TutorialMode.Idle:
			break;
		case TutorialMode.WaitClickLevelUpButton:
			break;
		case TutorialMode.WaitWindow:
			if (m_sendApollo != null && m_sendApollo.IsEnd() && GeneralWindow.IsCreated("chara_level_up_explan") && GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			break;
		case TutorialMode.EndClickLevelUpButton:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
				m_tutorialMode = TutorialMode.Idle;
			}
			break;
		}
	}

	private IEnumerator Setup()
	{
		m_state = State.SETUPING;
		UIRectItemStorage itemStoragePage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(base.gameObject, "grid_slot");
		if (itemStoragePage != null)
		{
			int charaCount2 = MenuPlayerSetUtil.GetOpenedCharaCount();
			itemStoragePage.maxRows = 1;
			itemStoragePage.maxColumns = charaCount2;
			itemStoragePage.maxItemCount = charaCount2;
			itemStoragePage.m_activeType = UIRectItemStorage.ActiveType.NOT_ACTTIVE;
		}
		GameObject gripRootObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_pager_alpha_clip");
		if (gripRootObject != null)
		{
			UIRectItemStorage itemStorageShotCut = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(gripRootObject, "slot");
			if (itemStorageShotCut != null)
			{
				int charaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
				itemStorageShotCut.maxRows = 1;
				itemStorageShotCut.maxColumns = charaCount;
				itemStorageShotCut.maxItemCount = charaCount;
			}
		}
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetBG>());
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetGrip>());
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetGripL>());
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetGripR>());
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetReturnButton>());
		m_partsList.Add(base.gameObject.AddComponent<MenuPlayerSetContents>());
		MenuPlayerSetContents contents = base.gameObject.GetComponent<MenuPlayerSetContents>();
		if (contents != null)
		{
			contents.SetCallback(PageChangeCallback);
			while (!contents.IsEndSetup)
			{
				yield return null;
			}
		}
		MenuPlayerSetGripL gripL = base.gameObject.GetComponent<MenuPlayerSetGripL>();
		if (gripL != null)
		{
			gripL.SetCallback(OnClickLeftScrollButton);
		}
		MenuPlayerSetGripR gripR = base.gameObject.GetComponent<MenuPlayerSetGripR>();
		if (gripR != null)
		{
			gripR.SetCallback(OnClickRightScrollButton);
		}
		m_state = State.SETUPED;
	}

	private IEnumerator ReSetUp()
	{
		if (m_state != State.SETUPED)
		{
			yield break;
		}
		UIRectItemStorage itemStoragePage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(base.gameObject, "grid_slot");
		if (itemStoragePage != null)
		{
			int charaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
			if (itemStoragePage.maxColumns != charaCount)
			{
				m_state = State.SETUPING;
				itemStoragePage.maxColumns = charaCount;
				itemStoragePage.maxItemCount = charaCount;
				itemStoragePage.Restart();
				GameObject gripRootObject = GameObjectUtil.FindChildGameObject(base.gameObject, "player_set_pager_alpha_clip");
				if (gripRootObject != null)
				{
					UIRectItemStorage itemStorageShotCut = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(gripRootObject, "slot");
					if (itemStorageShotCut != null)
					{
						itemStorageShotCut.maxColumns = charaCount;
						itemStorageShotCut.maxItemCount = charaCount;
						itemStorageShotCut.Restart();
					}
				}
				MenuPlayerSetContents contents2 = base.gameObject.GetComponent<MenuPlayerSetContents>();
				if (contents2 != null)
				{
					contents2.Reset();
					contents2.SetCallback(PageChangeCallback);
					while (!contents2.IsEndSetup)
					{
						yield return null;
					}
				}
				m_state = State.SETUPED;
			}
		}
		MenuPlayerSetContents contents = base.gameObject.GetComponent<MenuPlayerSetContents>();
		if (contents != null)
		{
			contents.UpdateRibbon();
		}
	}

	private void PageChangeCallback(int pageIndex)
	{
		MenuPlayerSetGripL component = base.gameObject.GetComponent<MenuPlayerSetGripL>();
		if (component != null)
		{
			if (pageIndex <= 0)
			{
				component.SetDisplayFlag(false);
			}
			else
			{
				component.SetDisplayFlag(true);
			}
		}
		MenuPlayerSetGripR component2 = base.gameObject.GetComponent<MenuPlayerSetGripR>();
		if (component2 != null)
		{
			int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
			if (pageIndex >= openedCharaCount - 1)
			{
				component2.SetDisplayFlag(false);
			}
			else
			{
				component2.SetDisplayFlag(true);
			}
		}
		m_currentPage = pageIndex;
	}

	private void OnClickLeftScrollButton()
	{
		m_currentPage--;
		if (m_currentPage < 0)
		{
			m_currentPage = 0;
		}
		StartCoroutine(JumpCharacterPage(m_currentPage));
	}

	private void OnClickRightScrollButton()
	{
		m_currentPage++;
		int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
		if (m_currentPage >= openedCharaCount - 1)
		{
			m_currentPage = openedCharaCount - 1;
		}
		StartCoroutine(JumpCharacterPage(m_currentPage));
	}
}
