using DataTable;
using System.Collections.Generic;
using Text;

public class ServerSpinResultGeneral
{
	private Dictionary<int, List<ServerItemState>> m_itemState;

	private Dictionary<int, List<ServerChaoData>> m_acquiredChaoData;

	public int m_requiredSpEggs;

	private int m_getOtomoAndCharaMax = -1;

	public int NumRequiredSpEggs
	{
		get
		{
			return m_requiredSpEggs;
		}
	}

	public bool IsRequiredSpEgg
	{
		get
		{
			return 0 < m_requiredSpEggs;
		}
	}

	public Dictionary<int, ServerItemState> ItemState
	{
		get
		{
			Dictionary<int, ServerItemState> dictionary = new Dictionary<int, ServerItemState>();
			if (m_itemState != null && m_itemState.Count > 0)
			{
				Dictionary<int, List<ServerItemState>>.KeyCollection keys = m_itemState.Keys;
				{
					foreach (int item in keys)
					{
						List<ServerItemState> list = m_itemState[item];
						if (list == null || list.Count <= 0)
						{
							continue;
						}
						ServerItemState serverItemState = new ServerItemState();
						list[0].CopyTo(serverItemState);
						int num = 0;
						foreach (ServerItemState item2 in list)
						{
							if (item2 != null)
							{
								num += item2.m_num;
							}
						}
						serverItemState.m_num = num;
						dictionary.Add(item, serverItemState);
					}
					return dictionary;
				}
			}
			return dictionary;
		}
	}

	public Dictionary<int, ServerChaoData> AcquiredChaoData
	{
		get
		{
			Dictionary<int, ServerChaoData> dictionary = new Dictionary<int, ServerChaoData>();
			if (m_acquiredChaoData != null && m_acquiredChaoData.Count > 0)
			{
				Dictionary<int, List<ServerChaoData>>.KeyCollection keys = m_acquiredChaoData.Keys;
				{
					foreach (int item in keys)
					{
						List<ServerChaoData> list = m_acquiredChaoData[item];
						if (list != null && list.Count > 0)
						{
							dictionary.Add(item, list[list.Count - 1]);
						}
					}
					return dictionary;
				}
			}
			return dictionary;
		}
	}

	public Dictionary<int, bool> IsRequiredChao
	{
		get;
		private set;
	}

	public int ItemWon
	{
		get;
		set;
	}

	private Dictionary<int, int> m_getAlreadyOverlap
	{
		get;
		set;
	}

	public bool IsMulti
	{
		get
		{
			return ItemWon == -1;
		}
	}

	public string AcquiredListText
	{
		get
		{
			return GetAcquiredListText();
		}
	}

	private Dictionary<int, int> m_acquiredChaoCount
	{
		get;
		set;
	}

	public bool IsRankup
	{
		get
		{
			bool result = false;
			Dictionary<int, List<ServerItemState>>.KeyCollection keys = m_itemState.Keys;
			foreach (int item in keys)
			{
				if (new ServerItem((ServerItem.Id)item).idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
				{
					return true;
				}
			}
			return result;
		}
	}

	public ServerSpinResultGeneral()
	{
		m_acquiredChaoData = new Dictionary<int, List<ServerChaoData>>();
		m_acquiredChaoCount = new Dictionary<int, int>();
		IsRequiredChao = new Dictionary<int, bool>();
		m_requiredSpEggs = 0;
		m_itemState = new Dictionary<int, List<ServerItemState>>();
		ItemWon = 0;
		m_getAlreadyOverlap = new Dictionary<int, int>();
		m_getOtomoAndCharaMax = -1;
	}

	public ServerSpinResultGeneral(ServerChaoSpinResult result)
	{
		m_acquiredChaoData = new Dictionary<int, List<ServerChaoData>>();
		m_acquiredChaoCount = new Dictionary<int, int>();
		IsRequiredChao = new Dictionary<int, bool>();
		m_requiredSpEggs = 0;
		m_itemState = new Dictionary<int, List<ServerItemState>>();
		ItemWon = 0;
		m_getAlreadyOverlap = new Dictionary<int, int>();
		m_getOtomoAndCharaMax = 0;
		if (result.AcquiredChaoData != null)
		{
			AddChaoState(result.AcquiredChaoData);
			if (result.ItemState != null && result.ItemState.Count > 0)
			{
				Dictionary<int, ServerItemState>.KeyCollection keys = result.ItemState.Keys;
				foreach (int item in keys)
				{
					AddItemState(new ServerItemState
					{
						m_itemId = result.ItemState[item].m_itemId,
						m_num = result.ItemState[item].m_num
					});
				}
			}
			m_getOtomoAndCharaMax = 1;
		}
		ItemWon = result.ItemWon;
	}

	public ServerSpinResultGeneral(ServerWheelOptions newOptions, ServerWheelOptions oldOptions)
	{
		m_acquiredChaoData = new Dictionary<int, List<ServerChaoData>>();
		m_acquiredChaoCount = new Dictionary<int, int>();
		IsRequiredChao = new Dictionary<int, bool>();
		m_requiredSpEggs = 0;
		m_itemState = new Dictionary<int, List<ServerItemState>>();
		ItemWon = oldOptions.m_itemWon;
		m_getAlreadyOverlap = new Dictionary<int, int>();
		m_getOtomoAndCharaMax = -1;
		ServerChaoData chao = oldOptions.GetChao();
		if (chao != null)
		{
			AddChaoState(chao);
			if (newOptions.IsItemList())
			{
				Dictionary<int, ServerItemState>.KeyCollection keys = newOptions.m_itemList.Keys;
				foreach (int item2 in keys)
				{
					AddItemState(newOptions.m_itemList[item2]);
				}
			}
			m_getOtomoAndCharaMax = 1;
		}
		else
		{
			ServerItemState item = oldOptions.GetItem();
			if (item != null)
			{
				AddItemState(oldOptions.GetItem());
			}
			m_getOtomoAndCharaMax = 0;
		}
	}

	private string GetAcquiredListText()
	{
		string text = null;
		if (IsMulti)
		{
			List<int> list = null;
			if (m_acquiredChaoData != null && m_acquiredChaoData.Count > 0)
			{
				list = new List<int>();
				Dictionary<int, List<ServerChaoData>>.KeyCollection keys = m_acquiredChaoData.Keys;
				foreach (int item in keys)
				{
					foreach (ServerChaoData item2 in m_acquiredChaoData[item])
					{
						if (item2.Rarity >= 100)
						{
							list.Add(item);
						}
					}
				}
				foreach (int item3 in keys)
				{
					foreach (ServerChaoData item4 in m_acquiredChaoData[item3])
					{
						if (item4.Rarity < 100)
						{
							list.Add(item3);
						}
					}
				}
			}
			if (list != null)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				foreach (int item5 in list)
				{
					string serverItemName = new ServerItem((ServerItem.Id)item5).serverItemName;
					text = ((!string.IsNullOrEmpty(text)) ? (text + "\n" + serverItemName) : serverItemName);
					if (m_getAlreadyOverlap != null && m_getAlreadyOverlap.ContainsKey(item5) && m_getAlreadyOverlap[item5] > 0)
					{
						if (dictionary.ContainsKey(item5))
						{
							Dictionary<int, int> dictionary2;
							Dictionary<int, int> dictionary3 = dictionary2 = dictionary;
							int key;
							int key2 = key = item5;
							key = dictionary2[key];
							dictionary3[key2] = key + 1;
						}
						else
						{
							dictionary.Add(item5, 1);
						}
						int num = m_acquiredChaoCount[item5] - m_getAlreadyOverlap[item5];
						if (num < dictionary[item5])
						{
							text = text + " " + TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "GeneralWindow", "get_item_overlap").text;
						}
					}
				}
			}
			if (m_itemState != null && m_itemState.Count > 0)
			{
				Dictionary<int, List<ServerItemState>>.KeyCollection keys2 = m_itemState.Keys;
				{
					foreach (int item6 in keys2)
					{
						string serverItemName2 = new ServerItem((ServerItem.Id)item6).serverItemName;
						foreach (ServerItemState item7 in m_itemState[item6])
						{
							text = ((!string.IsNullOrEmpty(text)) ? (text + "\n" + serverItemName2) : serverItemName2);
							text = text + " × " + item7.m_num;
						}
					}
					return text;
				}
			}
		}
		return text;
	}

	public void AddItemState(ServerItemState itemState)
	{
		if (m_itemState.ContainsKey(itemState.m_itemId))
		{
			m_itemState[itemState.m_itemId].Add(itemState);
		}
		else
		{
			List<ServerItemState> list = new List<ServerItemState>();
			list.Add(itemState);
			m_itemState.Add(itemState.m_itemId, list);
		}
		if (itemState.m_itemId == 220000)
		{
			m_requiredSpEggs += itemState.m_num;
		}
	}

	public void AddChaoState(ServerChaoData chaoState)
	{
		if (m_acquiredChaoData.ContainsKey(chaoState.Id))
		{
			m_acquiredChaoData[chaoState.Id].Add(chaoState);
		}
		else
		{
			List<ServerChaoData> list = new List<ServerChaoData>();
			list.Add(chaoState);
			m_acquiredChaoData.Add(chaoState.Id, list);
		}
		bool flag = false;
		int num = -1;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			if (chaoState.Rarity < 100)
			{
				ServerChaoState serverChaoState = playerState.ChaoStateByItemID(chaoState.Id);
				if (serverChaoState != null)
				{
					flag = (serverChaoState.Status != ServerChaoState.ChaoStatus.MaxLevel);
					num = ((!serverChaoState.IsOwned) ? (-1) : serverChaoState.Level);
				}
			}
			else
			{
				ServerCharacterState serverCharacterState = playerState.CharacterStateByItemID(chaoState.Id);
				if (serverCharacterState == null)
				{
					flag = true;
				}
				else if (serverCharacterState.Id < 0 || !serverCharacterState.IsUnlocked || serverCharacterState.star < serverCharacterState.starMax)
				{
					flag = true;
					num = serverCharacterState.star;
				}
			}
		}
		if (IsRequiredChao.ContainsKey(chaoState.Id))
		{
			IsRequiredChao[chaoState.Id] = flag;
		}
		else
		{
			IsRequiredChao.Add(chaoState.Id, flag);
		}
		if (m_acquiredChaoCount.ContainsKey(chaoState.Id))
		{
			Dictionary<int, int> acquiredChaoCount;
			Dictionary<int, int> dictionary = acquiredChaoCount = m_acquiredChaoCount;
			int id;
			int key = id = chaoState.Id;
			id = acquiredChaoCount[id];
			dictionary[key] = id + 1;
		}
		else
		{
			m_acquiredChaoCount.Add(chaoState.Id, 1);
		}
		if (m_getAlreadyOverlap.ContainsKey(chaoState.Id))
		{
			if (!m_acquiredChaoCount.ContainsKey(chaoState.Id))
			{
				return;
			}
			if (chaoState.Rarity < 100)
			{
				if (num + m_acquiredChaoCount[chaoState.Id] > ChaoTable.ChaoMaxLevel())
				{
					Dictionary<int, int> getAlreadyOverlap;
					Dictionary<int, int> dictionary2 = getAlreadyOverlap = m_getAlreadyOverlap;
					int id;
					int key2 = id = chaoState.Id;
					id = getAlreadyOverlap[id];
					dictionary2[key2] = id + 1;
				}
				return;
			}
			ServerCharacterState serverCharacterState2 = playerState.CharacterStateByItemID(chaoState.Id);
			if (serverCharacterState2 != null)
			{
				if (num + m_acquiredChaoCount[chaoState.Id] > serverCharacterState2.starMax)
				{
					Dictionary<int, int> getAlreadyOverlap2;
					Dictionary<int, int> dictionary3 = getAlreadyOverlap2 = m_getAlreadyOverlap;
					int id;
					int key3 = id = chaoState.Id;
					id = getAlreadyOverlap2[id];
					dictionary3[key3] = id + 1;
				}
			}
			else
			{
				m_getAlreadyOverlap[chaoState.Id] = 1;
			}
		}
		else if (!flag)
		{
			m_getAlreadyOverlap.Add(chaoState.Id, 1);
		}
		else
		{
			m_getAlreadyOverlap.Add(chaoState.Id, 0);
		}
	}

	public void CopyTo(ServerSpinResultGeneral to)
	{
		to.IsRequiredChao = IsRequiredChao;
		to.m_requiredSpEggs = m_requiredSpEggs;
		to.m_itemState.Clear();
		to.m_acquiredChaoData.Clear();
		foreach (List<ServerChaoData> value in m_acquiredChaoData.Values)
		{
			List<ServerChaoData> list = new List<ServerChaoData>();
			int key = 0;
			foreach (ServerChaoData item in value)
			{
				key = item.Id;
				list.Add(item);
			}
			to.m_acquiredChaoData.Add(key, list);
		}
		foreach (List<ServerItemState> value2 in m_itemState.Values)
		{
			List<ServerItemState> list2 = new List<ServerItemState>();
			int key2 = 0;
			foreach (ServerItemState item2 in value2)
			{
				key2 = item2.m_itemId;
				list2.Add(item2);
			}
			to.m_itemState.Add(key2, list2);
		}
		to.ItemWon = ItemWon;
		to.m_getAlreadyOverlap = m_getAlreadyOverlap;
		to.m_acquiredChaoCount = m_acquiredChaoCount;
		to.GetOtomoAndCharaMax();
	}

	public int GetOtomoAndCharaMax()
	{
		int num = 0;
		Dictionary<int, bool>.KeyCollection keys = IsRequiredChao.Keys;
		foreach (int item in keys)
		{
			if (IsRequiredChao[item])
			{
				num++;
			}
		}
		m_getOtomoAndCharaMax = num;
		return num;
	}

	public bool CheckGetChara()
	{
		bool result = false;
		Dictionary<int, bool>.KeyCollection keys = IsRequiredChao.Keys;
		foreach (int item in keys)
		{
			if (item >= 300000 && item < 400000)
			{
				return true;
			}
		}
		return result;
	}

	public ServerChaoData GetShowData(int index)
	{
		ServerChaoData result = null;
		if (m_getOtomoAndCharaMax > 0 && index >= 0 && index < m_getOtomoAndCharaMax)
		{
			Dictionary<int, bool>.KeyCollection keys = IsRequiredChao.Keys;
			List<int> list = new List<int>();
			foreach (int item in keys)
			{
				if (IsRequiredChao[item])
				{
					list.Add(item);
				}
			}
			list.Sort();
			if (list.Count > index && m_acquiredChaoData.ContainsKey(list[index]))
			{
				List<ServerChaoData> list2 = m_acquiredChaoData[list[index]];
				if (list2 != null && list2.Count > 0)
				{
					result = list2[list2.Count - 1];
				}
			}
		}
		return result;
	}

	public void Dump()
	{
	}
}
