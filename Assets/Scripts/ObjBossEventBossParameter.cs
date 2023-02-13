public class ObjBossEventBossParameter : ObjBossParameter
{
	private float m_wispBoostRatio;

	private WispBoostLevel m_wispBoostLevel = WispBoostLevel.NONE;

	private int m_missilePos1;

	private int m_missilePos2;

	private float m_wispInterspace;

	private float m_bumperInterspace;

	private float m_wispSpeedMin;

	private float m_wispSpeedMax;

	private float m_wispSwingMin;

	private float m_wispSwingMax;

	private float m_wispAddXMin;

	private float m_wispAddXMax;

	private float m_missileWaitTime;

	private int m_missileCount;

	private float m_challengeValue;

	public float ChallengeValue
	{
		get
		{
			return m_challengeValue;
		}
	}

	public float BoostRatio
	{
		get
		{
			return m_wispBoostRatio;
		}
		set
		{
			m_wispBoostRatio = value;
		}
	}

	public float BoostRatioDown
	{
		get
		{
			return EventBossParamTable.GetItemData(EventBossParamTableItem.WispRatioDown);
		}
	}

	public float BoostRatioAdd
	{
		get
		{
			return EventBossParamTable.GetItemData(EventBossParamTableItem.WispRatio);
		}
	}

	public WispBoostLevel BoostLevel
	{
		get
		{
			return m_wispBoostLevel;
		}
		set
		{
			m_wispBoostLevel = value;
		}
	}

	public int MissilePos1
	{
		get
		{
			return m_missilePos1;
		}
		set
		{
			m_missilePos1 = value;
		}
	}

	public int MissilePos2
	{
		get
		{
			return m_missilePos2;
		}
		set
		{
			m_missilePos2 = value;
		}
	}

	public float WispInterspace
	{
		get
		{
			return m_wispInterspace;
		}
		set
		{
			m_wispInterspace = value;
		}
	}

	public float BumperInterspace
	{
		get
		{
			return m_bumperInterspace;
		}
		set
		{
			m_bumperInterspace = value;
		}
	}

	public float WispSpeedMin
	{
		get
		{
			return m_wispSpeedMin;
		}
		set
		{
			m_wispSpeedMin = value;
		}
	}

	public float WispSpeedMax
	{
		get
		{
			return m_wispSpeedMax;
		}
		set
		{
			m_wispSpeedMax = value;
		}
	}

	public float WispSwingMin
	{
		get
		{
			return m_wispSwingMin;
		}
		set
		{
			m_wispSwingMin = value;
		}
	}

	public float WispSwingMax
	{
		get
		{
			return m_wispSwingMax;
		}
		set
		{
			m_wispSwingMax = value;
		}
	}

	public float WispAddXMin
	{
		get
		{
			return m_wispAddXMin;
		}
		set
		{
			m_wispAddXMin = value;
		}
	}

	public float WispAddXMax
	{
		get
		{
			return m_wispAddXMax;
		}
		set
		{
			m_wispAddXMax = value;
		}
	}

	public float MissileWaitTime
	{
		get
		{
			return m_missileWaitTime;
		}
		set
		{
			m_missileWaitTime = value;
		}
	}

	public int MissileCount
	{
		get
		{
			return m_missileCount;
		}
		set
		{
			m_missileCount = value;
		}
	}

	protected override void OnSetup()
	{
		int num = 0;
		LevelInformation levelInformation = ObjUtil.GetLevelInformation();
		if (levelInformation != null)
		{
			levelInformation.NumBossHpMax = base.BossHPMax;
			num = base.BossHPMax - levelInformation.NumBossAttack;
		}
		if (num < 1)
		{
			num = 1;
		}
		base.BossHP = num;
		if (EventManager.Instance != null)
		{
			int useRaidbossChallengeCount = EventManager.Instance.UseRaidbossChallengeCount;
			m_challengeValue = EventManager.Instance.GetRaidAttackRate(useRaidbossChallengeCount);
		}
		else
		{
			m_challengeValue = 1f;
		}
	}

	private int GetBoostAttackParam(float attack, float challengeVal)
	{
		float num = attack * challengeVal;
		return (int)num;
	}

	public int GetBoostAttackParam(WispBoostLevel level)
	{
		int result = 0;
		switch (level)
		{
		case WispBoostLevel.LEVEL1:
			result = GetBoostAttackParam(EventBossParamTable.GetItemData(EventBossParamTableItem.BoostAttack1), m_challengeValue);
			break;
		case WispBoostLevel.LEVEL2:
			result = GetBoostAttackParam(EventBossParamTable.GetItemData(EventBossParamTableItem.BoostAttack2), m_challengeValue);
			break;
		case WispBoostLevel.LEVEL3:
			result = GetBoostAttackParam(EventBossParamTable.GetItemData(EventBossParamTableItem.BoostAttack3), m_challengeValue);
			break;
		}
		return result;
	}

	public float GetBoostSpeedParam(WispBoostLevel level)
	{
		float result = 0f;
		switch (level)
		{
		case WispBoostLevel.LEVEL1:
			result = EventBossParamTable.GetItemData(EventBossParamTableItem.BoostSpeed1);
			break;
		case WispBoostLevel.LEVEL2:
			result = EventBossParamTable.GetItemData(EventBossParamTableItem.BoostSpeed2);
			break;
		case WispBoostLevel.LEVEL3:
			result = EventBossParamTable.GetItemData(EventBossParamTableItem.BoostSpeed3);
			break;
		}
		return result;
	}
}
