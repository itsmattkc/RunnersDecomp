using Message;
using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDeckTab : MonoBehaviour
{
	private enum State
	{
		IDLE,
		DECK_CHANGING,
		NUM
	}

	private int m_currentDeckStock;

	private GameObject m_deckColliderObject;

	private State m_state;

	public void UpdateView()
	{
		m_currentDeckStock = DeckUtil.GetDeckCurrentStockIndex();
		SetupTabView();
	}

	private void ChaoSetLoad(int stock)
	{
		if (m_state == State.DECK_CHANGING)
		{
			return;
		}
		m_currentDeckStock = stock;
		DeckUtil.SetDeckCurrentStockIndex(m_currentDeckStock);
		m_state = State.DECK_CHANGING;
		CharaType currentMainCharaType = CharaType.UNKNOWN;
		CharaType currentSubCharaType = CharaType.UNKNOWN;
		int currentMainId = -1;
		int currentSubId = -1;
		DeckUtil.GetDeckData(m_currentDeckStock, ref currentMainCharaType, ref currentSubCharaType, ref currentMainId, ref currentSubId);
		CharaType charaType = CharaType.UNKNOWN;
		CharaType charaType2 = CharaType.UNKNOWN;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			charaType = instance.PlayerData.MainChara;
			charaType2 = instance.PlayerData.SubChara;
		}
		bool flag = false;
		if (currentMainCharaType != CharaType.UNKNOWN && currentMainCharaType != charaType)
		{
			flag = true;
		}
		else if (currentSubCharaType != charaType2)
		{
			flag = true;
		}
		if (flag)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				int mainCharaId = -1;
				int subCharaId = -1;
				ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(currentMainCharaType);
				ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(currentSubCharaType);
				if (serverCharacterState != null)
				{
					mainCharaId = serverCharacterState.Id;
				}
				if (serverCharacterState2 != null)
				{
					subCharaId = serverCharacterState2.Id;
				}
				loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
				DeckUtil.UpdateCharacters(currentMainCharaType, currentSubCharaType);
			}
			else
			{
				DeckUtil.UpdateCharacters(currentMainCharaType, currentSubCharaType);
				ServerChangeCharacter_Succeeded(null);
			}
		}
		else
		{
			DeckUtil.UpdateCharacters(currentMainCharaType, currentSubCharaType);
			ServerChangeCharacter_Succeeded(null);
		}
	}

	private void SetupTabView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_5_MC");
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "2_Character");
		if (!(gameObject2 == null))
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "Deck_tab");
			if (!(gameObject3 == null))
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				list.Add("tab_1");
				list.Add("tab_2");
				list.Add("tab_3");
				list.Add("tab_4");
				list.Add("tab_5");
				list2.Add("OnClickTab1");
				list2.Add("OnClickTab2");
				list2.Add("OnClickTab3");
				list2.Add("OnClickTab4");
				list2.Add("OnClickTab5");
				m_deckColliderObject = GeneralUtil.SetToggleObject(base.gameObject, gameObject3, list2, list, m_currentDeckStock);
			}
		}
	}

	private void OnClickTab1()
	{
		if (m_currentDeckStock != 0)
		{
			ChaoSetLoad(0);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab2()
	{
		if (m_currentDeckStock != 1)
		{
			ChaoSetLoad(1);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab3()
	{
		if (m_currentDeckStock != 2)
		{
			ChaoSetLoad(2);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab4()
	{
		if (m_currentDeckStock != 3)
		{
			ChaoSetLoad(3);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab5()
	{
		if (m_currentDeckStock != 4)
		{
			ChaoSetLoad(4);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void ServerChangeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		int currentMainId = -1;
		int currentSubId = -1;
		CharaType currentMainCharaType = CharaType.SONIC;
		CharaType currentSubCharaType = CharaType.TAILS;
		DeckUtil.GetDeckData(m_currentDeckStock, ref currentMainCharaType, ref currentSubCharaType, ref currentMainId, ref currentSubId);
		int num = -1;
		int num2 = -1;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			num = instance.PlayerData.MainChaoID;
			num2 = instance.PlayerData.SubChaoID;
		}
		bool flag = false;
		if (currentMainId != num)
		{
			flag = true;
		}
		else if (currentSubId != num2)
		{
			flag = true;
		}
		if (flag)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerEquipChao((int)ServerItem.CreateFromChaoId(currentMainId).id, (int)ServerItem.CreateFromChaoId(currentSubId).id, base.gameObject);
				DeckUtil.UpdateChaos(currentMainId, currentSubId);
			}
			else
			{
				ServerEquipChao_Succeeded(null);
				DeckUtil.UpdateChaos(currentMainId, currentSubId);
			}
		}
		else
		{
			DeckUtil.UpdateChaos(currentMainId, currentSubId);
			ServerEquipChao_Succeeded(null);
		}
	}

	private void ServerEquipChao_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		m_state = State.IDLE;
	}

	private bool CheckExsitDeck()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.CheckExsitDeck();
			}
		}
		return false;
	}

	private void Start()
	{
		if (!CheckExsitDeck())
		{
			DeckUtil.SetFirstDeckData();
		}
		m_currentDeckStock = DeckUtil.GetDeckCurrentStockIndex();
		SetupTabView();
	}

	private void Update()
	{
		switch (m_state)
		{
		}
	}
}
