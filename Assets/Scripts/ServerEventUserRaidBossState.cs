using System;

public class ServerEventUserRaidBossState
{
	private int m_numRaidBossRings;

	private int m_raidBossEnergy;

	private int m_raidBossEnergyBuy;

	private int m_numBeatedEncounter;

	private int m_numBeatedEnterprise;

	private int m_numRaidBossEncountered;

	private DateTime m_energyRenewsAt;

	public int NumRaidbossRings
	{
		get
		{
			return m_numRaidBossRings;
		}
		set
		{
			m_numRaidBossRings = value;
		}
	}

	public int RaidBossEnergy
	{
		get
		{
			return m_raidBossEnergy;
		}
		set
		{
			m_raidBossEnergy = value;
		}
	}

	public int RaidbossEnergyBuy
	{
		get
		{
			return m_raidBossEnergyBuy;
		}
		set
		{
			m_raidBossEnergyBuy = value;
		}
	}

	public int RaidBossEnergyCount
	{
		get
		{
			return m_raidBossEnergy + m_raidBossEnergyBuy;
		}
	}

	public int NumBeatedEncounter
	{
		get
		{
			return m_numBeatedEncounter;
		}
		set
		{
			m_numBeatedEncounter = value;
		}
	}

	public int NumBeatedEnterprise
	{
		get
		{
			return m_numBeatedEnterprise;
		}
		set
		{
			m_numBeatedEnterprise = value;
		}
	}

	public int NumRaidBossEncountered
	{
		get
		{
			return m_numRaidBossEncountered;
		}
		set
		{
			m_numRaidBossEncountered = value;
		}
	}

	public DateTime EnergyRenewsAt
	{
		get
		{
			return m_energyRenewsAt;
		}
		set
		{
			m_energyRenewsAt = value;
		}
	}

	public ServerEventUserRaidBossState()
	{
		m_numRaidBossRings = 0;
		m_raidBossEnergy = 0;
		m_raidBossEnergyBuy = 0;
		m_numBeatedEncounter = 0;
		m_numBeatedEnterprise = 0;
		m_numRaidBossEncountered = 0;
		m_energyRenewsAt = DateTime.MinValue;
	}

	public void CopyTo(ServerEventUserRaidBossState to)
	{
		to.m_numRaidBossRings = m_numRaidBossRings;
		to.m_raidBossEnergy = m_raidBossEnergy;
		to.m_raidBossEnergyBuy = m_raidBossEnergyBuy;
		to.m_numBeatedEncounter = m_numBeatedEncounter;
		to.m_numBeatedEnterprise = m_numBeatedEnterprise;
		to.m_numRaidBossEncountered = m_numRaidBossEncountered;
		to.m_energyRenewsAt = m_energyRenewsAt;
	}
}
