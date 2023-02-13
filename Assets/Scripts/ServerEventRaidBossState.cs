using System;

public class ServerEventRaidBossState
{
	public enum StatusType
	{
		INIT,
		BOSS_ALIVE,
		BOSS_ESCAPE,
		REWARD,
		PROCESS_END
	}

	private long m_raidBossId;

	private int m_level;

	private int m_rarity;

	private int m_hitPoint;

	private int m_maxHitPoint;

	private int m_status;

	private DateTime m_escapeAt;

	private string m_encounterName;

	private bool m_encounterFlag;

	private bool m_crowdedFlag;

	private bool m_participationFlag;

	public long Id
	{
		get
		{
			return m_raidBossId;
		}
		set
		{
			m_raidBossId = value;
		}
	}

	public int Level
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int Rarity
	{
		get
		{
			return m_rarity;
		}
		set
		{
			m_rarity = value;
		}
	}

	public int HitPoint
	{
		get
		{
			return m_hitPoint;
		}
		set
		{
			m_hitPoint = value;
		}
	}

	public int MaxHitPoint
	{
		get
		{
			return m_maxHitPoint;
		}
		set
		{
			m_maxHitPoint = value;
		}
	}

	public int Status
	{
		get
		{
			return m_status;
		}
		set
		{
			m_status = value;
		}
	}

	public DateTime EscapeAt
	{
		get
		{
			return m_escapeAt;
		}
		set
		{
			m_escapeAt = value;
		}
	}

	public string EncounterName
	{
		get
		{
			return m_encounterName;
		}
		set
		{
			m_encounterName = value;
		}
	}

	public bool Encounter
	{
		get
		{
			return m_encounterFlag;
		}
		set
		{
			m_encounterFlag = value;
		}
	}

	public bool Crowded
	{
		get
		{
			return m_crowdedFlag;
		}
		set
		{
			m_crowdedFlag = value;
		}
	}

	public bool Participation
	{
		get
		{
			return m_participationFlag;
		}
		set
		{
			m_participationFlag = value;
		}
	}

	public ServerEventRaidBossState()
	{
		m_raidBossId = 0L;
		m_level = 0;
		m_rarity = 0;
		m_hitPoint = 0;
		m_maxHitPoint = 0;
		m_status = 0;
		m_escapeAt = DateTime.MinValue;
		m_encounterName = string.Empty;
		m_encounterFlag = false;
		m_crowdedFlag = false;
		m_participationFlag = false;
	}

	public StatusType GetStatusType()
	{
		StatusType result = StatusType.INIT;
		switch (m_status)
		{
		case 1:
			result = StatusType.BOSS_ALIVE;
			break;
		case 2:
			result = StatusType.BOSS_ESCAPE;
			break;
		case 3:
			result = StatusType.REWARD;
			break;
		case 4:
			result = StatusType.PROCESS_END;
			break;
		}
		return result;
	}

	public void CopyTo(ServerEventRaidBossState to)
	{
		to.m_raidBossId = m_raidBossId;
		to.m_level = m_level;
		to.m_rarity = m_rarity;
		to.m_hitPoint = m_hitPoint;
		to.m_maxHitPoint = m_maxHitPoint;
		to.m_status = m_status;
		to.m_escapeAt = m_escapeAt;
		to.m_encounterName = m_encounterName;
		to.m_encounterFlag = m_encounterFlag;
		to.m_crowdedFlag = m_crowdedFlag;
		to.m_participationFlag = m_participationFlag;
	}
}
