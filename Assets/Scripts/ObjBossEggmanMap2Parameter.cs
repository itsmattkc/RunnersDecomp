using System;

[Serializable]
public class ObjBossEggmanMap2Parameter : SpawnableParameter
{
	public float m_playerDistance;

	public float m_bumperFirstSpeed;

	public float m_bumperOutOfcontrol;

	public float m_shotSpeed;

	public int m_LV1_hp;

	public int m_LV1_distance;

	public int m_LV1_tblId;

	public float m_LV1_attackInterspaceMin;

	public float m_LV1_attackInterspaceMax;

	public float m_LV1_attackSpeedMin;

	public float m_LV1_attackSpeedMax;

	public float m_LV1_missileSpeed;

	public float m_LV1_missileInterspace;

	public float m_LV1_boundParamMin;

	public float m_LV1_boundParamMax;

	public int m_LV1_boundMaxRand;

	public int m_LV1_bumperRand;

	public float m_LV1_ballSpeed;

	public int m_LV2_hp;

	public int m_LV2_distance;

	public int m_LV2_tblId;

	public float m_LV2_attackInterspaceMin;

	public float m_LV2_attackInterspaceMax;

	public float m_LV2_attackSpeedMin;

	public float m_LV2_attackSpeedMax;

	public float m_LV2_missileSpeed;

	public float m_LV2_missileInterspace;

	public float m_LV2_boundParamMin;

	public float m_LV2_boundParamMax;

	public int m_LV2_boundMaxRand;

	public int m_LV2_bumperRand;

	public float m_LV2_ballSpeed;

	public int m_LV3_hp;

	public int m_LV3_distance;

	public int m_LV3_tblId;

	public float m_LV3_attackInterspaceMin;

	public float m_LV3_attackInterspaceMax;

	public float m_LV3_attackSpeedMin;

	public float m_LV3_attackSpeedMax;

	public float m_LV3_missileSpeed;

	public float m_LV3_missileInterspace;

	public float m_LV3_boundParamMin;

	public float m_LV3_boundParamMax;

	public int m_LV3_boundMaxRand;

	public int m_LV3_bumperRand;

	public float m_LV3_ballSpeed;

	public int m_LV4_hp;

	public int m_LV4_distance;

	public int m_LV4_tblId;

	public float m_LV4_attackInterspaceMin;

	public float m_LV4_attackInterspaceMax;

	public float m_LV4_attackSpeedMin;

	public float m_LV4_attackSpeedMax;

	public float m_LV4_missileSpeed;

	public float m_LV4_missileInterspace;

	public float m_LV4_boundParamMin;

	public float m_LV4_boundParamMax;

	public int m_LV4_boundMaxRand;

	public int m_LV4_bumperRand;

	public float m_LV4_ballSpeed;

	public int m_LV5_hp;

	public int m_LV5_distance;

	public int m_LV5_tblId;

	public float m_LV5_attackInterspaceMin;

	public float m_LV5_attackInterspaceMax;

	public float m_LV5_attackSpeedMin;

	public float m_LV5_attackSpeedMax;

	public float m_LV5_missileSpeed;

	public float m_LV5_missileInterspace;

	public float m_LV5_boundParamMin;

	public float m_LV5_boundParamMax;

	public int m_LV5_boundMaxRand;

	public int m_LV5_bumperRand;

	public float m_LV5_ballSpeed;

	public ObjBossEggmanMap2Parameter()
		: base("ObjBossEggmanMap2")
	{
		m_playerDistance = 8.5f;
		m_bumperFirstSpeed = 10f;
		m_bumperOutOfcontrol = 0.3f;
		m_shotSpeed = 15f;
		m_LV1_hp = 3;
		m_LV1_distance = 500;
		m_LV1_tblId = 0;
		m_LV1_attackInterspaceMin = 5f;
		m_LV1_attackInterspaceMax = 5f;
		m_LV1_attackSpeedMin = 2f;
		m_LV1_attackSpeedMax = 4f;
		m_LV1_missileSpeed = 6f;
		m_LV1_missileInterspace = 1f;
		m_LV1_boundParamMin = 0f;
		m_LV1_boundParamMax = 1.5f;
		m_LV1_boundMaxRand = 50;
		m_LV1_bumperRand = 30;
		m_LV1_ballSpeed = 8f;
		m_LV2_hp = 5;
		m_LV2_distance = 700;
		m_LV2_tblId = 0;
		m_LV2_attackInterspaceMin = 5f;
		m_LV2_attackInterspaceMax = 5f;
		m_LV2_attackSpeedMin = 2f;
		m_LV2_attackSpeedMax = 4f;
		m_LV2_missileSpeed = 6f;
		m_LV2_missileInterspace = 1f;
		m_LV2_boundParamMin = 0f;
		m_LV2_boundParamMax = 1.5f;
		m_LV2_boundMaxRand = 50;
		m_LV2_bumperRand = 30;
		m_LV2_ballSpeed = 8f;
		m_LV3_hp = 7;
		m_LV3_distance = 1000;
		m_LV3_tblId = 0;
		m_LV3_attackInterspaceMin = 5f;
		m_LV3_attackInterspaceMax = 5f;
		m_LV3_attackSpeedMin = 2f;
		m_LV3_attackSpeedMax = 4f;
		m_LV3_missileSpeed = 6f;
		m_LV3_missileInterspace = 1f;
		m_LV3_boundParamMin = 0f;
		m_LV3_boundParamMax = 1.5f;
		m_LV3_boundMaxRand = 50;
		m_LV3_bumperRand = 30;
		m_LV3_ballSpeed = 8f;
		m_LV4_hp = 9;
		m_LV4_distance = 1300;
		m_LV4_tblId = 0;
		m_LV4_attackInterspaceMin = 5f;
		m_LV4_attackInterspaceMax = 5f;
		m_LV4_attackSpeedMin = 2f;
		m_LV4_attackSpeedMax = 4f;
		m_LV4_missileSpeed = 6f;
		m_LV4_missileInterspace = 1f;
		m_LV4_boundParamMin = 0f;
		m_LV4_boundParamMax = 1.5f;
		m_LV4_boundMaxRand = 50;
		m_LV4_bumperRand = 30;
		m_LV4_ballSpeed = 8f;
		m_LV5_hp = 12;
		m_LV5_distance = 1500;
		m_LV5_tblId = 0;
		m_LV5_attackInterspaceMin = 5f;
		m_LV5_attackInterspaceMax = 5f;
		m_LV5_attackSpeedMin = 2f;
		m_LV5_attackSpeedMax = 4f;
		m_LV5_missileSpeed = 6f;
		m_LV5_missileInterspace = 1f;
		m_LV5_boundParamMin = 0f;
		m_LV5_boundParamMax = 1.5f;
		m_LV5_boundMaxRand = 50;
		m_LV5_bumperRand = 30;
		m_LV5_ballSpeed = 8f;
	}
}
