using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class DeckUtil
{
	public class DeckSet
	{
		public CharaType charaMain;

		public CharaType charaSub = CharaType.UNKNOWN;

		public int chaoMain = -1;

		public int chaoSub = -1;

		public bool isCurrentSelect;
	}

	private static int s_chaoSetCurrentStockIndex = -1;

	public static void SetFirstDeckData()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		SystemData systemdata = instance.GetSystemdata();
		if (systemdata != null)
		{
			SaveDataManager instance2 = SaveDataManager.Instance;
			if (instance2 != null)
			{
				systemdata.SaveDeckData(0, instance2.PlayerData.MainChara, instance2.PlayerData.SubChara, instance2.PlayerData.MainChaoID, instance2.PlayerData.SubChaoID);
			}
		}
	}

	public static int GetDeckCurrentStockIndex()
	{
		int num = s_chaoSetCurrentStockIndex;
		if (num < 0)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					SaveDataManager instance2 = SaveDataManager.Instance;
					if (instance2 != null)
					{
						num = (s_chaoSetCurrentStockIndex = systemdata.GetDeckCurrentStockIndex());
					}
				}
			}
		}
		return num;
	}

	public static void SetDeckCurrentStockIndex(int index)
	{
		if (index >= 0 && index < 6)
		{
			s_chaoSetCurrentStockIndex = index;
		}
	}

	public static void CharaSetSaveAuto(int currentMainId, int currentSubId)
	{
		int deckCurrentStockIndex = GetDeckCurrentStockIndex();
		CharaSetSave(deckCurrentStockIndex, currentMainId, currentSubId);
	}

	private static bool CharaSetSave(int stock, int currentMainId, int currentSubId)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SaveDataManager instance2 = SaveDataManager.Instance;
				instance2.PlayerData.MainChara = new ServerItem((ServerItem.Id)currentMainId).charaType;
				instance2.PlayerData.SubChara = new ServerItem((ServerItem.Id)currentSubId).charaType;
				instance2.SavePlayerData();
				systemdata.SaveDeckDataChara(stock);
				return true;
			}
		}
		return false;
	}

	public static void ChaoSetSaveAuto(int currentMainId, int currentSubId)
	{
		int deckCurrentStockIndex = GetDeckCurrentStockIndex();
		ChaoSetSave(deckCurrentStockIndex, currentMainId - 400000, currentSubId - 400000);
	}

	private static bool ChaoSetSave(int stock, int currentMainId, int currentSubId)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SaveDataManager instance2 = SaveDataManager.Instance;
				instance2.PlayerData.MainChaoID = currentMainId;
				instance2.PlayerData.SubChaoID = currentSubId;
				instance2.SavePlayerData();
				systemdata.SaveDeckDataChao(stock);
				return true;
			}
		}
		return false;
	}

	public static bool DeckReset(int stock)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.RestDeckData(stock);
			}
		}
		return true;
	}

	public static bool DeckSetSave(int stock, CharaType currentMainCharaType, CharaType currentSubCharaType, int currentMainId, int currentSubId)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.SaveDeckData(stock, currentMainCharaType, currentSubCharaType, currentMainId, currentSubId);
			}
		}
		return true;
	}

	public static bool DeckSetLoad(int stock, GameObject callbackObject)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		bool flag = false;
		SaveDataManager instance = SaveDataManager.Instance;
		CharaType currentMainCharaType = instance.PlayerData.MainChara;
		CharaType currentSubCharaType = instance.PlayerData.SubChara;
		int currentMainId = instance.PlayerData.MainChaoID;
		int currentSubId = instance.PlayerData.SubChaoID;
		flag = DeckSetLoad(stock, ref currentMainCharaType, ref currentSubCharaType, ref currentMainId, ref currentSubId, callbackObject);
		if (flag)
		{
			instance.PlayerData.MainChara = currentMainCharaType;
			instance.PlayerData.SubChara = currentSubCharaType;
			instance.PlayerData.MainChaoID = currentMainId;
			instance.PlayerData.SubChaoID = currentSubId;
		}
		return flag;
	}

	public static bool DeckSetLoad(int stock, ref CharaType currentMainCharaType, ref CharaType currentSubCharaType, ref int currentMainId, ref int currentSubId, GameObject callbackObject = null)
	{
		if (stock < 0 || stock >= 6)
		{
			return false;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SaveDataManager instance2 = SaveDataManager.Instance;
				CharaType mainCharaType = CharaType.SONIC;
				CharaType subCharaType = CharaType.UNKNOWN;
				int mainChaoId = -1;
				int subChaoId = -1;
				systemdata.GetDeckData(stock, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
				CharaType mainChara = instance2.PlayerData.MainChara;
				CharaType subChara = instance2.PlayerData.SubChara;
				int mainChaoID = instance2.PlayerData.MainChaoID;
				int subChaoID = instance2.PlayerData.SubChaoID;
				currentMainCharaType = mainCharaType;
				currentSubCharaType = subCharaType;
				currentMainId = mainChaoId;
				currentSubId = subChaoId;
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (mainChaoId != mainChaoID || subChaoId != subChaoID)
				{
					if (loggedInServerInterface != null && callbackObject != null)
					{
						loggedInServerInterface.RequestServerEquipChao((int)ServerItem.CreateFromChaoId(mainChaoId).id, (int)ServerItem.CreateFromChaoId(subChaoId).id, callbackObject);
					}
				}
				else if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerEquipChao_Dummy", SendMessageOptions.DontRequireReceiver);
				}
				if (mainCharaType != mainChara || subCharaType != subChara)
				{
					if (loggedInServerInterface != null && callbackObject != null)
					{
						int mainCharaId = -1;
						int subCharaId = -1;
						ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(mainCharaType);
						ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(subCharaType);
						if (serverCharacterState != null)
						{
							mainCharaId = serverCharacterState.Id;
						}
						if (serverCharacterState2 != null)
						{
							subCharaId = serverCharacterState2.Id;
						}
						loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, callbackObject);
					}
				}
				else if (callbackObject != null)
				{
					callbackObject.SendMessage("RequestServerChangeCharacter_Dummy", SendMessageOptions.DontRequireReceiver);
				}
				return true;
			}
		}
		return false;
	}

	public static void GetDeckData(int stock, ref CharaType currentMainCharaType, ref CharaType currentSubCharaType, ref int currentMainId, ref int currentSubId)
	{
		if (stock < 0 || stock >= 6)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance == null))
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				CharaType mainCharaType = CharaType.SONIC;
				CharaType subCharaType = CharaType.UNKNOWN;
				int mainChaoId = -1;
				int subChaoId = -1;
				systemdata.GetDeckData(stock, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
				currentMainCharaType = mainCharaType;
				currentSubCharaType = subCharaType;
				currentMainId = mainChaoId;
				currentSubId = subChaoId;
			}
		}
	}

	public static void GetDeckData(int stock, ref CharaType currentMainCharaType, ref CharaType currentSubCharaType)
	{
		if (stock < 0 || stock >= 6)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance == null))
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				CharaType mainCharaType = CharaType.SONIC;
				CharaType subCharaType = CharaType.UNKNOWN;
				int mainChaoId = -1;
				int subChaoId = -1;
				systemdata.GetDeckData(stock, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
				currentMainCharaType = mainCharaType;
				currentSubCharaType = subCharaType;
			}
		}
	}

	public static void UpdateCharacters(CharaType mainChara, CharaType subChara)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance == null))
		{
			instance.PlayerData.MainChara = mainChara;
			instance.PlayerData.SubChara = subChara;
		}
	}

	public static void UpdateChaos(int mainChaoId, int subChaoId)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance == null))
		{
			instance.PlayerData.MainChaoID = mainChaoId;
			instance.PlayerData.SubChaoID = subChaoId;
		}
	}

	public static bool IsChaoSetSave(int stock)
	{
		bool result = true;
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				result = systemdata.IsSaveDeckData(stock);
			}
		}
		return result;
	}

	public static List<DeckSet> GetDeckList()
	{
		List<DeckSet> list = new List<DeckSet>();
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				for (int i = 0; i < 6; i++)
				{
					DeckSet deckSet = new DeckSet();
					systemdata.GetDeckData(i, out deckSet.charaMain, out deckSet.charaSub, out deckSet.chaoMain, out deckSet.chaoSub);
					list.Add(deckSet);
				}
			}
		}
		int deckCurrentStockIndex = GetDeckCurrentStockIndex();
		if (list.Count > deckCurrentStockIndex)
		{
			list[deckCurrentStockIndex].isCurrentSelect = true;
		}
		return list;
	}
}
