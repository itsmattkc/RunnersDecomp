using System.Collections.Generic;

public class ServerPlayCharacterState
{
	public enum CharacterStatus
	{
		Locked,
		Unlocked,
		MaxLevel
	}

	public enum LockCondition
	{
		OPEN,
		MILEAGE_EPISODE,
		RING_OR_RED_STAR_RING
	}

	public List<int> AbilityLevel = new List<int>();

	public List<int> AbilityNumRings = new List<int>();

	public List<int> abilityLevelUp = new List<int>();

	public List<int> abilityLevelUpExp = new List<int>();

	public int Id
	{
		get;
		set;
	}

	public CharacterStatus Status
	{
		get;
		set;
	}

	public int Level
	{
		get;
		set;
	}

	public int Cost
	{
		get;
		set;
	}

	public int NumRedRings
	{
		get;
		set;
	}

	public LockCondition Condition
	{
		get;
		set;
	}

	public int Exp
	{
		get;
		set;
	}

	public int star
	{
		get;
		set;
	}

	public int starMax
	{
		get;
		set;
	}

	public int priceNumRings
	{
		get;
		set;
	}

	public int priceNumRedRings
	{
		get;
		set;
	}

	public int UnlockCost
	{
		get
		{
			if (Status == CharacterStatus.Locked)
			{
				return Cost;
			}
			return -1;
		}
	}

	public int LevelUpCost
	{
		get
		{
			if (Status == CharacterStatus.Unlocked)
			{
				return Cost;
			}
			return -1;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			return CharacterStatus.Locked != Status;
		}
	}

	public bool IsMaxLevel
	{
		get
		{
			return CharacterStatus.MaxLevel == Status;
		}
	}

	public float QuickModeTimeExtension
	{
		get
		{
			float result = 0f;
			if (starMax > 0)
			{
				StageTimeTable stageTimeTable = GameObjectUtil.FindGameObjectComponent<StageTimeTable>("StageTimeTable");
				if (stageTimeTable != null)
				{
					float num = stageTimeTable.GetTableItemData(StageTimeTableItem.OverlapBonus);
					result = (float)star * num;
				}
			}
			return result;
		}
	}

	public ServerPlayCharacterState()
	{
		Id = -1;
		Status = CharacterStatus.Locked;
		Level = 10;
		Cost = 0;
		star = 0;
		starMax = 0;
		priceNumRings = 0;
		priceNumRedRings = 0;
	}

	public static ServerCharacterState CreateCharacterState(ServerPlayCharacterState playCharaState)
	{
		if (playCharaState == null)
		{
			return null;
		}
		ServerCharacterState serverCharacterState = new ServerCharacterState();
		serverCharacterState.Id = playCharaState.Id;
		serverCharacterState.Status = (ServerCharacterState.CharacterStatus)playCharaState.Status;
		serverCharacterState.Level = playCharaState.Level;
		serverCharacterState.Cost = playCharaState.Cost;
		serverCharacterState.NumRedRings = playCharaState.NumRedRings;
		serverCharacterState.star = playCharaState.star;
		serverCharacterState.starMax = playCharaState.starMax;
		serverCharacterState.priceNumRings = playCharaState.priceNumRings;
		serverCharacterState.priceNumRedRings = playCharaState.priceNumRedRings;
		foreach (int item in playCharaState.AbilityLevel)
		{
			serverCharacterState.AbilityLevel.Add(item);
		}
		foreach (int abilityNumRing in playCharaState.AbilityNumRings)
		{
			serverCharacterState.AbilityNumRings.Add(abilityNumRing);
		}
		serverCharacterState.Condition = (ServerCharacterState.LockCondition)playCharaState.Condition;
		serverCharacterState.Exp = playCharaState.Exp;
		return serverCharacterState;
	}

	public static ServerPlayCharacterState CreatePlayCharacterState(ServerCharacterState charaState)
	{
		if (charaState == null)
		{
			return null;
		}
		ServerPlayCharacterState serverPlayCharacterState = new ServerPlayCharacterState();
		serverPlayCharacterState.Id = charaState.Id;
		serverPlayCharacterState.Status = serverPlayCharacterState.Status;
		serverPlayCharacterState.Level = charaState.Level;
		serverPlayCharacterState.Cost = charaState.Cost;
		serverPlayCharacterState.NumRedRings = charaState.NumRedRings;
		serverPlayCharacterState.star = charaState.star;
		serverPlayCharacterState.starMax = charaState.starMax;
		serverPlayCharacterState.priceNumRings = charaState.priceNumRings;
		serverPlayCharacterState.priceNumRedRings = charaState.priceNumRedRings;
		foreach (int item in charaState.AbilityLevel)
		{
			serverPlayCharacterState.AbilityLevel.Add(item);
		}
		foreach (int abilityNumRing in charaState.AbilityNumRings)
		{
			serverPlayCharacterState.AbilityNumRings.Add(abilityNumRing);
		}
		serverPlayCharacterState.Condition = (LockCondition)charaState.Condition;
		serverPlayCharacterState.Exp = charaState.Exp;
		return serverPlayCharacterState;
	}

	public void Dump()
	{
		Debug.Log(string.Concat("Id=", Id, ", Status=", Status, ", Level=", Level, ", Cost=", Cost, ", UnlockCost=", UnlockCost, ", LevelUpCost=", LevelUpCost));
	}
}
