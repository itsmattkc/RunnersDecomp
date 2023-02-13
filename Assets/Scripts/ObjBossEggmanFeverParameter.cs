using System;

[Serializable]
public class ObjBossEggmanFeverParameter : SpawnableParameter
{
	public int m_hp;

	public int m_distance;

	public int m_tblId;

	public float m_downSpeed;

	public float m_attackInterspaceMin;

	public float m_attackInterspaceMax;

	public float m_boundParamMin;

	public float m_boundParamMax;

	public int m_boundMaxRand;

	public float m_shotSpeed;

	public float m_bumperFirstSpeed;

	public float m_bumperOutOfcontrol;

	public float m_bumperSpeedup;

	public float m_ballSpeed;

	public ObjBossEggmanFeverParameter()
		: base("ObjBossEggmanFever")
	{
		m_hp = 3;
		m_distance = 500;
		m_tblId = 0;
		m_downSpeed = 1f;
		m_attackInterspaceMin = 1f;
		m_attackInterspaceMax = 2f;
		m_boundParamMin = 0f;
		m_boundParamMax = 1.5f;
		m_boundMaxRand = 50;
		m_shotSpeed = 15f;
		m_bumperFirstSpeed = 10f;
		m_bumperOutOfcontrol = 0.3f;
		m_bumperSpeedup = 100f;
		m_ballSpeed = 8f;
	}
}
