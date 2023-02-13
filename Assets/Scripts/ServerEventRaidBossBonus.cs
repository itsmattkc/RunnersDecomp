public class ServerEventRaidBossBonus
{
	private int m_encounterBonus;

	private int m_wrestleBonus;

	private int m_damageRateBonus;

	private int m_damageTopBonus;

	private int m_beatBonus;

	public int EncounterBonus
	{
		get
		{
			return m_encounterBonus;
		}
		set
		{
			m_encounterBonus = value;
		}
	}

	public int WrestleBonus
	{
		get
		{
			return m_wrestleBonus;
		}
		set
		{
			m_wrestleBonus = value;
		}
	}

	public int DamageRateBonus
	{
		get
		{
			return m_damageRateBonus;
		}
		set
		{
			m_damageRateBonus = value;
		}
	}

	public int DamageTopBonus
	{
		get
		{
			return m_damageTopBonus;
		}
		set
		{
			m_damageTopBonus = value;
		}
	}

	public int BeatBonus
	{
		get
		{
			return m_beatBonus;
		}
		set
		{
			m_beatBonus = value;
		}
	}

	public ServerEventRaidBossBonus()
	{
		m_encounterBonus = 0;
		m_wrestleBonus = 0;
		m_damageRateBonus = 0;
		m_damageTopBonus = 0;
		m_beatBonus = 0;
	}

	public void CopyTo(ServerEventRaidBossBonus to)
	{
		to.m_encounterBonus = m_encounterBonus;
		to.m_wrestleBonus = m_wrestleBonus;
		to.m_damageRateBonus = m_damageRateBonus;
		to.m_damageTopBonus = m_damageTopBonus;
		to.m_beatBonus = m_beatBonus;
	}
}
