using System;
using System.Collections.Generic;

public class ServerWheelOptions
{
	public enum WheelItemType
	{
		CharacterTokenAmy,
		CharacterTokenTails,
		CharacterTokenKnuckles,
		CharacterTokenShadow,
		CharacterTokenBlaze,
		FeverTime,
		GoldenEnemy,
		RedStarRingsSmall,
		RedStarRingsMedium,
		RedStarRingsLarge,
		RedStarRingsJackpot,
		RingsSmall,
		RingsMedium,
		RingsLarge,
		RingsJackpot,
		SpinAgain,
		Energy,
		Max
	}

	public int m_nextSpinCost;

	public int[] m_items;

	public int[] m_itemQuantities;

	public int[] m_itemWeight;

	public int m_itemWon;

	public int m_spinCost;

	public RouletteUtility.WheelRank m_rouletteRank;

	public int m_numRouletteToken;

	public int m_numJackpotRing;

	public int m_numRemaining;

	public DateTime m_nextFreeSpin;

	public Dictionary<int, ServerItemState> m_itemList;

	public int m_numRemainingFree
	{
		get
		{
			int num = m_numRemaining - m_numRouletteToken;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
	}

	public int NumRequiredSpEggs
	{
		get
		{
			int num = 0;
			if (m_itemList != null)
			{
				Dictionary<int, ServerItemState>.KeyCollection keys = m_itemList.Keys;
				{
					foreach (int item in keys)
					{
						if (item == 220000)
						{
							num += m_itemList[item].m_num;
						}
					}
					return num;
				}
			}
			return num;
		}
	}

	public ServerWheelOptions(ServerWheelOptions options = null)
	{
		m_nextSpinCost = 1;
		m_items = new int[8];
		m_itemQuantities = new int[8];
		m_itemWeight = new int[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == 0)
			{
				m_items[i] = 200000;
			}
			else
			{
				m_items[i] = 120000 + i - 1;
			}
			m_itemQuantities[i] = 1;
			m_itemWeight[i] = 1;
		}
		m_itemWon = 0;
		m_spinCost = 0;
		m_rouletteRank = RouletteUtility.WheelRank.Normal;
		m_numRouletteToken = 0;
		m_numJackpotRing = 0;
		m_numRemaining = 1;
		m_nextFreeSpin = DateTime.Now;
		if (options != null)
		{
			options.CopyTo(this);
		}
	}

	public void Dump()
	{
		string text = string.Join(",", Array.ConvertAll(m_items, (int item) => item.ToString()));
		Debug.Log(string.Format("items={0}, itemWon={1}, spinCost={2}, nextSpinCost={3}, nextFreeSpin={4}", text, m_itemWon, m_spinCost, m_nextSpinCost, m_nextFreeSpin));
	}

	public void RefreshFakeState()
	{
	}

	public ServerItemState GetItem()
	{
		ServerItemState serverItemState = null;
		if (m_itemWon >= 0 && m_itemWon < m_items.Length)
		{
			ServerItem serverItem = new ServerItem((ServerItem.Id)m_items[m_itemWon]);
			if (serverItem.idType != ServerItem.IdType.CHAO && serverItem.idType != ServerItem.IdType.CHARA)
			{
				serverItemState = new ServerItemState();
				serverItemState.m_itemId = (int)serverItem.id;
				serverItemState.m_num = m_itemQuantities[m_itemWon];
			}
		}
		return serverItemState;
	}

	public ServerChaoState GetChao()
	{
		ServerChaoState serverChaoState = null;
		if (m_itemWon >= 0 && m_itemWon < m_items.Length)
		{
			ServerItem serverItem = new ServerItem((ServerItem.Id)m_items[m_itemWon]);
			if (serverItem.idType == ServerItem.IdType.CHAO || serverItem.idType == ServerItem.IdType.CHARA)
			{
				ServerPlayerState playerState = ServerInterface.PlayerState;
				serverChaoState = new ServerChaoState();
				if (serverItem.idType == ServerItem.IdType.CHAO)
				{
					ServerChaoState serverChaoState2 = playerState.ChaoStateByItemID((int)serverItem.id);
					serverChaoState.Id = (int)serverItem.id;
					if (serverChaoState2 != null)
					{
						serverChaoState.Status = serverChaoState2.Status;
						serverChaoState.Level = serverChaoState2.Level;
						serverChaoState.Rarity = serverChaoState2.Rarity;
					}
					else
					{
						serverChaoState.Status = ServerChaoState.ChaoStatus.NotOwned;
						serverChaoState.Level = 0;
						serverChaoState.Rarity = 0;
					}
				}
				else if (serverItem.idType == ServerItem.IdType.CHARA)
				{
					ServerCharacterState serverCharacterState = playerState.CharacterStateByItemID((int)serverItem.id);
					serverChaoState.Id = (int)serverItem.id;
					serverChaoState.Status = ServerChaoState.ChaoStatus.MaxLevel;
					serverChaoState.Level = 0;
					serverChaoState.Rarity = 100;
					if (serverCharacterState == null)
					{
						serverChaoState.Status = ServerChaoState.ChaoStatus.NotOwned;
					}
					else if (serverCharacterState.Id < 0 || !serverCharacterState.IsUnlocked)
					{
						serverChaoState.Status = ServerChaoState.ChaoStatus.NotOwned;
					}
				}
			}
		}
		return serverChaoState;
	}

	public bool IsItemList()
	{
		return m_itemList != null;
	}

	public void ResetItemList()
	{
		if (m_itemList != null)
		{
			m_itemList.Clear();
		}
		m_itemList = null;
	}

	public void AddItemList(ServerItemState item)
	{
		if (m_itemList == null)
		{
			m_itemList = new Dictionary<int, ServerItemState>();
		}
		Debug.Log("ServerWheelOptions AddItemList id:" + item.m_itemId + "  num:" + item.m_num + "  !!!!!!!!!!!!!!!!!!!!!!!!!");
		if (m_itemList.ContainsKey(item.m_itemId))
		{
			m_itemList[item.m_itemId].m_num += item.m_num;
		}
		else
		{
			m_itemList.Add(item.m_itemId, item);
		}
	}

	public void CopyTo(ServerWheelOptions to)
	{
		to.m_nextSpinCost = m_nextSpinCost;
		to.m_itemWon = m_itemWon;
		to.m_items = (m_items.Clone() as int[]);
		to.m_itemQuantities = (m_itemQuantities.Clone() as int[]);
		to.m_itemWeight = (m_itemWeight.Clone() as int[]);
		to.m_spinCost = m_spinCost;
		to.m_rouletteRank = m_rouletteRank;
		to.m_numRouletteToken = m_numRouletteToken;
		to.m_numJackpotRing = m_numJackpotRing;
		to.m_numRemaining = m_numRemaining;
		to.m_nextFreeSpin = m_nextFreeSpin;
		to.ResetItemList();
		if (m_itemList == null || m_itemList.Count <= 0)
		{
			return;
		}
		Dictionary<int, ServerItemState>.KeyCollection keys = m_itemList.Keys;
		foreach (int item in keys)
		{
			to.AddItemList(m_itemList[item]);
		}
	}
}
