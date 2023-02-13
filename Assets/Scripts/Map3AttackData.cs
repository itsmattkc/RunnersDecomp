public class Map3AttackData
{
	public BossAttackType m_type;

	public int m_randA;

	public BossAttackPos m_posA;

	public int m_randB;

	public BossAttackPos m_posB;

	public Map3AttackData()
	{
		m_type = BossAttackType.NONE;
		m_randA = 0;
		m_posA = BossAttackPos.NONE;
		m_randB = 0;
		m_posB = BossAttackPos.NONE;
	}

	public Map3AttackData(BossAttackType type, int randA, BossAttackPos posA, int randB, BossAttackPos posB)
	{
		m_type = type;
		m_randA = randA;
		m_posA = posA;
		m_randB = randB;
		m_posB = posB;
	}

	public int GetAttackCount()
	{
		return BossMap3Table.GetBossAttackCount(m_type);
	}
}
