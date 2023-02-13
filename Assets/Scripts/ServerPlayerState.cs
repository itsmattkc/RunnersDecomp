using System;
using System.Collections.Generic;

public class ServerPlayerState
{
	public enum CHARA_SORT
	{
		NONE,
		CHARA_ATTR,
		TEAM_ATTR
	}

	public long m_highScore;

	public int m_multiplier;

	public int m_numContinuesUsed;

	public int m_currentMissionSet;

	public bool[] m_missionsComplete;

	public long m_totalHighScore;

	public long m_totalHighScoreQuick;

	public long m_totalDistance;

	public long m_maxDistance;

	public int m_leagueIndex;

	public int m_leagueIndexQuick;

	public int m_numRings;

	public int m_numFreeRings;

	public int m_numBuyRings;

	public int m_numRedRings;

	public int m_numFreeRedRings;

	public int m_numBuyRedRings;

	public int m_numEnergy;

	public int m_numFreeEnergy;

	public int m_numBuyEnergy;

	public DateTime m_energyRenewsAt;

	public DateTime m_nextWeeklyLeaderboard;

	public DateTime m_endDailyMissionDate;

	public int m_numUnreadMessages;

	public int m_dailyMissionId;

	public int m_dailyMissionValue;

	public bool m_dailyChallengeComplete;

	public int m_numDailyChalCont;

	public int m_mainCharaId;

	public int m_subCharaId;

	public bool m_useSubCharacter;

	public int m_mainChaoId;

	public int m_subChaoId;

	public int[] m_equipItemList = new int[3];

	public int m_numPlaying;

	public int m_numAnimals;

	public int m_numRank;

	private ServerCharacterState[] m_characterState;

	private List<ServerChaoState> m_chaoState;

	private Dictionary<int, ServerItemState> m_itemStates;

	private ServerPlayCharacterState[] m_playCharacterState;

	public int unlockedCharacterNum
	{
		get
		{
			int num = 0;
			if (m_characterState != null && m_characterState.Length > 0)
			{
				ServerCharacterState[] characterState = m_characterState;
				foreach (ServerCharacterState serverCharacterState in characterState)
				{
					if (serverCharacterState != null && serverCharacterState.IsUnlocked)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	public List<ServerChaoState> ChaoStates
	{
		get
		{
			return m_chaoState;
		}
	}

	public ServerPlayerState()
	{
		m_highScore = 1234L;
		m_multiplier = 1;
		m_numContinuesUsed = 0;
		m_currentMissionSet = 0;
		m_missionsComplete = new bool[3];
		for (int i = 0; i < 3; i++)
		{
			m_missionsComplete[i] = false;
		}
		m_totalHighScoreQuick = 0L;
		m_totalHighScore = 0L;
		m_totalDistance = 0L;
		m_maxDistance = 0L;
		m_leagueIndex = 0;
		m_leagueIndexQuick = 0;
		m_characterState = new ServerCharacterState[29];
		for (int j = 0; j < 29; j++)
		{
			m_characterState[j] = new ServerCharacterState();
		}
		m_characterState[0].m_tokenCost = 0;
		m_characterState[0].m_numTokens = 0;
		m_characterState[0].Status = ServerCharacterState.CharacterStatus.Unlocked;
		m_characterState[1].m_tokenCost = 20;
		m_characterState[2].m_tokenCost = 30;
		m_chaoState = new List<ServerChaoState>();
		m_playCharacterState = new ServerPlayCharacterState[29];
		for (int k = 0; k < m_equipItemList.Length; k++)
		{
			m_equipItemList[k] = -1;
		}
		m_itemStates = new Dictionary<int, ServerItemState>();
		m_numRings = 0;
		m_numFreeRings = 0;
		m_numBuyRings = 0;
		m_numRedRings = 0;
		m_numFreeRedRings = 0;
		m_numBuyRedRings = 0;
		m_numEnergy = 0;
		m_numFreeEnergy = 0;
		m_numBuyEnergy = 0;
		m_numUnreadMessages = 0;
		m_dailyMissionId = 0;
		m_dailyMissionValue = 0;
		m_dailyChallengeComplete = false;
		m_numDailyChalCont = 0;
		m_mainChaoId = -1;
		m_subChaoId = -1;
		m_energyRenewsAt = DateTime.Now;
		m_nextWeeklyLeaderboard = DateTime.Now + new TimeSpan(7, 0, 0, 0);
		m_endDailyMissionDate = DateTime.Now;
	}

	public ServerCharacterState CharacterState(CharaType type)
	{
		if (type == CharaType.UNKNOWN)
		{
			return null;
		}
		if (type >= CharaType.NUM)
		{
			return null;
		}
		return m_characterState[(int)type];
	}

	public void ClearCharacterState()
	{
		for (int i = 0; i < 29; i++)
		{
			m_characterState[i] = new ServerCharacterState();
		}
	}

	public ServerCharacterState CharacterStateByItemID(int itemID)
	{
		return CharacterState(new ServerItem((ServerItem.Id)itemID).charaType);
	}

	public ServerPlayCharacterState PlayCharacterState(CharaType type)
	{
		if (type == CharaType.UNKNOWN)
		{
			return null;
		}
		if (type >= CharaType.NUM)
		{
			return null;
		}
		return m_playCharacterState[(int)type];
	}

	public void SetCharacterState(ServerCharacterState characterState)
	{
		if (characterState == null)
		{
			return;
		}
		CharaType charaType = new ServerItem((ServerItem.Id)characterState.Id).charaType;
		if (charaType == CharaType.UNKNOWN)
		{
			return;
		}
		int num = (int)charaType;
		if (num >= 29)
		{
			return;
		}
		ServerCharacterState serverCharacterState = m_characterState[num];
		CharaType charaType2 = new ServerItem((ServerItem.Id)serverCharacterState.Id).charaType;
		if (charaType2 != CharaType.UNKNOWN)
		{
			characterState.OldStatus = serverCharacterState.Status;
			characterState.OldCost = serverCharacterState.Cost;
			characterState.OldExp = serverCharacterState.Exp;
			characterState.OldAbiltyLevel.Clear();
			foreach (int item in serverCharacterState.AbilityLevel)
			{
				characterState.OldAbiltyLevel.Add(item);
			}
		}
		m_characterState[num] = characterState;
		NetUtil.SyncSaveDataAndDataBase(m_characterState);
	}

	public void SetCharacterState(ServerCharacterState[] characterStates)
	{
		if (characterStates == null)
		{
			return;
		}
		foreach (ServerCharacterState serverCharacterState in characterStates)
		{
			if (serverCharacterState != null)
			{
				SetCharacterState(serverCharacterState);
			}
		}
	}

	public void SetPlayCharacterState(ServerPlayCharacterState playCharacterState)
	{
		if (playCharacterState == null)
		{
			return;
		}
		CharaType charaType = new ServerItem((ServerItem.Id)playCharacterState.Id).charaType;
		if (charaType == CharaType.UNKNOWN)
		{
			return;
		}
		int num = (int)charaType;
		if (num >= 29)
		{
			return;
		}
		ServerCharacterState serverCharacterState = m_characterState[num];
		if (serverCharacterState != null)
		{
			serverCharacterState.OldAbiltyLevel.Clear();
			foreach (int item in serverCharacterState.AbilityLevel)
			{
				serverCharacterState.OldAbiltyLevel.Add(item);
			}
			serverCharacterState.OldStatus = serverCharacterState.Status;
			serverCharacterState.OldCost = serverCharacterState.Cost;
			serverCharacterState.OldExp = serverCharacterState.Exp;
			serverCharacterState.AbilityLevel.Clear();
			foreach (int item2 in playCharacterState.AbilityLevel)
			{
				serverCharacterState.AbilityLevel.Add(item2);
			}
			serverCharacterState.Level = playCharacterState.Level;
			serverCharacterState.Cost = playCharacterState.Cost;
			serverCharacterState.Exp = playCharacterState.Exp;
			serverCharacterState.NumRedRings = playCharacterState.NumRedRings;
			m_characterState[num] = serverCharacterState;
			NetUtil.SyncSaveDataAndDataBase(m_characterState);
		}
		m_playCharacterState[num] = playCharacterState;
	}

	public void ClearPlayCharacterState()
	{
		for (int i = 0; i < 29; i++)
		{
			m_playCharacterState[i] = null;
		}
	}

	public ServerChaoState ChaoStateByItemID(int itemID)
	{
		if (m_chaoState != null)
		{
			int count = m_chaoState.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_chaoState[i].Id == itemID)
				{
					return m_chaoState[i];
				}
			}
		}
		return null;
	}

	public ServerChaoState ChaoStateByArrayIndex(int index)
	{
		if (m_chaoState != null && index < m_chaoState.Count)
		{
			return m_chaoState[index];
		}
		return null;
	}

	public void SetChaoState(List<ServerChaoState> newChaoStateList)
	{
		if (m_chaoState == null)
		{
			return;
		}
		foreach (ServerChaoState newChaoState in newChaoStateList)
		{
			if (newChaoState == null)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < m_chaoState.Count; i++)
			{
				if (m_chaoState[i].Id == newChaoState.Id)
				{
					m_chaoState[i] = newChaoState;
					flag = true;
				}
			}
			if (!flag)
			{
				m_chaoState.Add(newChaoState);
			}
		}
		NetUtil.SyncSaveDataAndDataBase(m_chaoState);
	}

	public ServerItemState GetItemStateByType(ServerConstants.RunModifierType type)
	{
		int id = 0;
		switch (type)
		{
		case ServerConstants.RunModifierType.SpringBonus:
			id = 3;
			break;
		case ServerConstants.RunModifierType.RingStreak:
			id = 2;
			break;
		case ServerConstants.RunModifierType.EnemyCombo:
			id = 4;
			break;
		}
		return GetItemStateById(id);
	}

	public ServerItemState GetItemStateById(int id)
	{
		if (m_itemStates.ContainsKey(id))
		{
			return m_itemStates[id];
		}
		return null;
	}

	public int GetNumItemByType(ServerConstants.RunModifierType type)
	{
		ServerItemState itemStateByType = GetItemStateByType(type);
		if (itemStateByType != null)
		{
			return itemStateByType.m_num;
		}
		return 0;
	}

	public int GetNumItemById(int id)
	{
		ServerItemState itemStateById = GetItemStateById(id);
		if (itemStateById != null)
		{
			return itemStateById.m_num;
		}
		return 0;
	}

	public void AddItemState(ServerItemState itemState)
	{
		if (m_itemStates.ContainsKey(itemState.m_itemId))
		{
			m_itemStates[itemState.m_itemId].m_num += itemState.m_num;
		}
		else
		{
			m_itemStates.Add(itemState.m_itemId, itemState);
		}
	}

	public void Dump()
	{
		string arg = string.Join(":", Array.ConvertAll(m_missionsComplete, (bool item) => item.ToString()));
		Debug.Log(string.Format("highScore={0}, multiplier={1}, numRings={2}, numRedRings={3}, energy={4}, energyRenewsAt={5}", m_highScore, m_multiplier, m_numRings, m_numRedRings, m_numEnergy, m_energyRenewsAt));
		Debug.Log(string.Format("currentMissionSet={0}, missions={1}, numContinuesUsed={2}", m_currentMissionSet, arg, m_numContinuesUsed));
		for (int i = 0; i < 29; i++)
		{
			m_characterState[i].Dump();
		}
	}

	public void RefreshFakeState()
	{
	}

	public void CopyTo(ServerPlayerState to)
	{
		to.m_highScore = m_highScore;
		to.m_numContinuesUsed = m_numContinuesUsed;
		to.m_multiplier = m_multiplier;
		to.m_currentMissionSet = m_currentMissionSet;
		to.m_missionsComplete = (m_missionsComplete.Clone() as bool[]);
		to.m_totalHighScoreQuick = m_totalHighScoreQuick;
		to.m_totalHighScore = m_totalHighScore;
		to.m_totalDistance = m_totalDistance;
		to.m_maxDistance = m_maxDistance;
		to.m_leagueIndex = m_leagueIndex;
		to.m_leagueIndexQuick = m_leagueIndexQuick;
		to.m_numRings = m_numRings;
		to.m_numFreeRings = m_numFreeRings;
		to.m_numBuyRings = m_numBuyRings;
		to.m_numRedRings = m_numRedRings;
		to.m_numFreeRedRings = m_numFreeRedRings;
		to.m_numBuyRedRings = m_numBuyRedRings;
		to.m_numEnergy = m_numEnergy;
		to.m_numFreeEnergy = m_numFreeEnergy;
		to.m_numBuyEnergy = m_numBuyEnergy;
		to.m_energyRenewsAt = m_energyRenewsAt;
		to.m_numUnreadMessages = m_numUnreadMessages;
		to.m_dailyMissionId = m_dailyMissionId;
		to.m_dailyMissionValue = m_dailyMissionValue;
		to.m_dailyChallengeComplete = m_dailyChallengeComplete;
		to.m_numDailyChalCont = m_numDailyChalCont;
		to.m_nextWeeklyLeaderboard = m_nextWeeklyLeaderboard;
		to.m_endDailyMissionDate = m_endDailyMissionDate;
		to.m_mainChaoId = m_mainChaoId;
		to.m_mainCharaId = m_mainCharaId;
		to.m_subCharaId = m_subCharaId;
		to.m_useSubCharacter = m_useSubCharacter;
		to.m_subChaoId = m_subChaoId;
		to.m_numPlaying = m_numPlaying;
		to.m_numAnimals = m_numAnimals;
		to.m_numRank = m_numRank;
		to.m_itemStates.Clear();
		foreach (ServerItemState value in m_itemStates.Values)
		{
			to.m_itemStates.Add(value.m_itemId, value);
		}
		for (int i = 0; i < m_equipItemList.Length; i++)
		{
			to.m_equipItemList[i] = m_equipItemList[i];
		}
		ServerCharacterState[] characterState = m_characterState;
		foreach (ServerCharacterState serverCharacterState in characterState)
		{
			if (serverCharacterState != null && serverCharacterState.Id >= 0)
			{
				to.SetCharacterState(serverCharacterState);
			}
		}
		to.SetChaoState(m_chaoState);
		NetUtil.SyncSaveDataAndDataBase(this);
	}

	private void SetChaoState(ServerChaoState srcState, ref ServerChaoState dstState)
	{
		dstState.IsLvUp = ((0 != dstState.Id) & (dstState.IsLvUp | (dstState.Level < srcState.Level)));
		dstState.IsNew = ((0 != dstState.Id) & (dstState.IsNew | (dstState.Status == ServerChaoState.ChaoStatus.NotOwned && srcState.Status != ServerChaoState.ChaoStatus.NotOwned)));
		dstState.Id = srcState.Id;
		dstState.Level = srcState.Level;
		dstState.Rarity = srcState.Rarity;
		dstState.Status = srcState.Status;
		dstState.Dealing = srcState.Dealing;
		dstState.NumInvite = srcState.NumInvite;
		dstState.NumAcquired = srcState.NumAcquired;
		dstState.Hidden = srcState.Hidden;
		if (dstState.IsNew)
		{
			Debug.Log("requrired new chao. id : " + dstState.Id);
		}
		if (dstState.IsLvUp)
		{
			Debug.Log("chao level up. id : " + dstState.Id);
		}
	}

	public List<CharaType> GetCharacterTypeList(CHARA_SORT sort = CHARA_SORT.NONE, bool descending = false, int offset = 0)
	{
		List<CharaType> result = null;
		Dictionary<CharaType, ServerCharacterState> characterStateList = GetCharacterStateList(sort, descending, offset);
		if (characterStateList != null && characterStateList.Count > 0)
		{
			result = new List<CharaType>();
			Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = characterStateList.Keys;
			{
				foreach (CharaType item in keys)
				{
					result.Add(item);
				}
				return result;
			}
		}
		return result;
	}

	public Dictionary<CharaType, ServerCharacterState> GetCharacterStateList(CHARA_SORT sort = CHARA_SORT.NONE, bool descending = false, int offset = 0)
	{
		Dictionary<CharaType, ServerCharacterState> outList = new Dictionary<CharaType, ServerCharacterState>();
		Dictionary<CharaType, ServerCharacterState> dictionary = null;
		if (m_characterState != null && m_characterState.Length > 0)
		{
			ServerCharacterState[] characterState = m_characterState;
			foreach (ServerCharacterState serverCharacterState in characterState)
			{
				CharaType charaType = serverCharacterState.charaType;
				if (charaType == CharaType.UNKNOWN)
				{
					continue;
				}
				if (sort == CHARA_SORT.NONE)
				{
					outList.Add(charaType, serverCharacterState);
					continue;
				}
				if (dictionary == null)
				{
					dictionary = new Dictionary<CharaType, ServerCharacterState>();
				}
				dictionary.Add(charaType, serverCharacterState);
			}
		}
		if (sort != 0 && dictionary != null)
		{
			switch (sort)
			{
			case CHARA_SORT.CHARA_ATTR:
				GetCharacterStateListCharaAttr(ref outList, dictionary, descending, offset);
				break;
			case CHARA_SORT.TEAM_ATTR:
				GetCharacterStateListTeamAttr(ref outList, dictionary, descending, offset);
				break;
			default:
				outList = dictionary;
				break;
			}
		}
		return outList;
	}

	private void GetCharacterStateListCharaAttr(ref Dictionary<CharaType, ServerCharacterState> outList, Dictionary<CharaType, ServerCharacterState> orgList, bool descending, int offset)
	{
		Dictionary<CharacterAttribute, List<ServerCharacterState>> dictionary = new Dictionary<CharacterAttribute, List<ServerCharacterState>>();
		CharacterDataNameInfo instance = CharacterDataNameInfo.Instance;
		if (instance != null)
		{
			Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = orgList.Keys;
			foreach (CharaType item in keys)
			{
				CharacterDataNameInfo.Info dataByID = instance.GetDataByID(item);
				if (dictionary.ContainsKey(dataByID.m_attribute))
				{
					dictionary[dataByID.m_attribute].Add(orgList[item]);
					continue;
				}
				List<ServerCharacterState> list = new List<ServerCharacterState>();
				list.Add(orgList[item]);
				dictionary.Add(dataByID.m_attribute, list);
			}
		}
		if (dictionary.Count <= 0 || outList == null)
		{
			return;
		}
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			CharacterAttribute characterAttribute = CharacterAttribute.SPEED;
			characterAttribute = (CharacterAttribute)((!descending) ? ((offset + i) % num) : ((offset - i + num) % num));
			if (!dictionary.ContainsKey(characterAttribute))
			{
				continue;
			}
			foreach (ServerCharacterState item2 in dictionary[characterAttribute])
			{
				outList.Add(item2.charaType, item2);
			}
		}
	}

	private void GetCharacterStateListTeamAttr(ref Dictionary<CharaType, ServerCharacterState> outList, Dictionary<CharaType, ServerCharacterState> orgList, bool descending, int offset)
	{
		Dictionary<TeamAttribute, List<ServerCharacterState>> dictionary = new Dictionary<TeamAttribute, List<ServerCharacterState>>();
		CharacterDataNameInfo instance = CharacterDataNameInfo.Instance;
		if (instance != null)
		{
			Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = orgList.Keys;
			foreach (CharaType item in keys)
			{
				CharacterDataNameInfo.Info dataByID = instance.GetDataByID(item);
				if (dictionary.ContainsKey(dataByID.m_teamAttribute))
				{
					dictionary[dataByID.m_teamAttribute].Add(orgList[item]);
					continue;
				}
				List<ServerCharacterState> list = new List<ServerCharacterState>();
				list.Add(orgList[item]);
				dictionary.Add(dataByID.m_teamAttribute, list);
			}
		}
		if (dictionary.Count <= 0 || outList == null)
		{
			return;
		}
		int num = 8;
		for (int i = 0; i < num; i++)
		{
			TeamAttribute teamAttribute = TeamAttribute.HERO;
			teamAttribute = (TeamAttribute)((!descending) ? ((offset + i) % num) : ((offset - i + num) % num));
			if (!dictionary.ContainsKey(teamAttribute))
			{
				continue;
			}
			foreach (ServerCharacterState item2 in dictionary[teamAttribute])
			{
				outList.Add(item2.charaType, item2);
			}
		}
	}
}
