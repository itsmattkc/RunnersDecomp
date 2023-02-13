using Boss;
using UnityEngine;

public class ObjBossEggmanMap1 : ObjBossEggmanBase
{
	public int m_debugLevel;

	public void SetObjBossEggmanMap1Parameter(ObjBossEggmanMap1Parameter param)
	{
		ObjBossEggmanState component = GetComponent<ObjBossEggmanState>();
		if (component != null)
		{
			component.Init();
			int mapBossLevel = GetMapBossLevel();
			mapBossLevel = Mathf.Max(mapBossLevel, 1);
			mapBossLevel = Mathf.Min(mapBossLevel, 5);
			component.BossParam.TypeBoss = 1;
			component.BossParam.BossLevel = mapBossLevel;
			component.BossParam.DefaultPlayerDistance = param.m_playerDistance;
			component.BossParam.ShotSpeed = param.m_shotSpeed;
			component.BossParam.AttackSpeed = param.m_attackSpeed;
			switch (mapBossLevel)
			{
			case 1:
				component.BossParam.BossHPMax = param.m_LV1_hp;
				component.BossParam.BossDistance = param.m_LV1_distance;
				component.BossParam.TableID = param.m_LV1_tblId;
				component.BossParam.BoundParamMin = param.m_LV1_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV1_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV1_boundMaxRand;
				component.BossParam.TrapRand = param.m_LV1_trapRand;
				component.BossParam.AttackInterspaceMin = param.m_LV1_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV1_attackInterspaceMax;
				component.BossParam.AttackTrapCountMax = param.m_LV1_attackTrapCountMax;
				component.BossParam.BallSpeed = param.m_LV1_ballSpeed;
				break;
			case 2:
				component.BossParam.BossHPMax = param.m_LV2_hp;
				component.BossParam.BossDistance = param.m_LV2_distance;
				component.BossParam.TableID = param.m_LV2_tblId;
				component.BossParam.BoundParamMin = param.m_LV2_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV2_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV2_boundMaxRand;
				component.BossParam.TrapRand = param.m_LV2_trapRand;
				component.BossParam.AttackInterspaceMin = param.m_LV2_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV2_attackInterspaceMax;
				component.BossParam.AttackTrapCountMax = param.m_LV2_attackTrapCountMax;
				component.BossParam.BallSpeed = param.m_LV2_ballSpeed;
				break;
			case 3:
				component.BossParam.BossHPMax = param.m_LV3_hp;
				component.BossParam.BossDistance = param.m_LV3_distance;
				component.BossParam.TableID = param.m_LV3_tblId;
				component.BossParam.BoundParamMin = param.m_LV3_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV3_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV3_boundMaxRand;
				component.BossParam.TrapRand = param.m_LV3_trapRand;
				component.BossParam.AttackInterspaceMin = param.m_LV3_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV3_attackInterspaceMax;
				component.BossParam.AttackTrapCountMax = param.m_LV3_attackTrapCountMax;
				component.BossParam.BallSpeed = param.m_LV3_ballSpeed;
				break;
			case 4:
				component.BossParam.BossHPMax = param.m_LV4_hp;
				component.BossParam.BossDistance = param.m_LV4_distance;
				component.BossParam.TableID = param.m_LV4_tblId;
				component.BossParam.BoundParamMin = param.m_LV4_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV4_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV4_boundMaxRand;
				component.BossParam.TrapRand = param.m_LV4_trapRand;
				component.BossParam.AttackInterspaceMin = param.m_LV4_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV4_attackInterspaceMax;
				component.BossParam.AttackTrapCountMax = param.m_LV4_attackTrapCountMax;
				component.BossParam.BallSpeed = param.m_LV4_ballSpeed;
				break;
			case 5:
				component.BossParam.BossHPMax = param.m_LV5_hp;
				component.BossParam.BossDistance = param.m_LV5_distance;
				component.BossParam.TableID = param.m_LV5_tblId;
				component.BossParam.BoundParamMin = param.m_LV5_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV5_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV5_boundMaxRand;
				component.BossParam.TrapRand = param.m_LV5_trapRand;
				component.BossParam.AttackInterspaceMin = param.m_LV5_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV5_attackInterspaceMax;
				component.BossParam.AttackTrapCountMax = param.m_LV5_attackTrapCountMax;
				component.BossParam.BallSpeed = param.m_LV5_ballSpeed;
				break;
			}
			component.DebugDrawInfo("Map1 level=" + mapBossLevel + "BossHPMax=" + component.BossParam.BossHPMax + "BossDistance=" + component.BossParam.BossDistance + "TableID=" + component.BossParam.TableID);
			component.SetInitState(STATE_ID.AppearMap1);
			component.SetDamageState(STATE_ID.DamageMap1);
			component.Setup();
		}
	}
}
