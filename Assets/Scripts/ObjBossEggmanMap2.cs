using Boss;
using UnityEngine;

public class ObjBossEggmanMap2 : ObjBossEggmanBase
{
	public int m_debugLevel;

	public void SetObjBossEggmanMap2Parameter(ObjBossEggmanMap2Parameter param)
	{
		ObjBossEggmanState component = GetComponent<ObjBossEggmanState>();
		if (component != null)
		{
			component.Init();
			int mapBossLevel = GetMapBossLevel();
			mapBossLevel = Mathf.Max(mapBossLevel, 1);
			mapBossLevel = Mathf.Min(mapBossLevel, 5);
			component.BossParam.TypeBoss = 2;
			component.BossParam.BossLevel = mapBossLevel;
			component.BossParam.DefaultPlayerDistance = param.m_playerDistance;
			component.BossParam.BumperFirstSpeed = param.m_bumperFirstSpeed;
			component.BossParam.BumperOutOfcontrol = param.m_bumperOutOfcontrol;
			component.BossParam.ShotSpeed = param.m_shotSpeed;
			switch (mapBossLevel)
			{
			case 1:
				component.BossParam.BossHPMax = param.m_LV1_hp;
				component.BossParam.BossDistance = param.m_LV1_distance;
				component.BossParam.TableID = param.m_LV1_tblId;
				component.BossParam.AttackInterspaceMin = param.m_LV1_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV1_attackInterspaceMax;
				component.BossParam.AttackSpeedMin = param.m_LV1_attackSpeedMin;
				component.BossParam.AttackSpeedMax = param.m_LV1_attackSpeedMax;
				component.BossParam.MissileSpeed = param.m_LV1_missileSpeed;
				component.BossParam.MissileInterspace = param.m_LV1_missileInterspace;
				component.BossParam.BoundParamMin = param.m_LV1_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV1_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV1_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV1_bumperRand;
				component.BossParam.BallSpeed = param.m_LV1_ballSpeed;
				break;
			case 2:
				component.BossParam.BossHPMax = param.m_LV2_hp;
				component.BossParam.BossDistance = param.m_LV2_distance;
				component.BossParam.TableID = param.m_LV2_tblId;
				component.BossParam.AttackInterspaceMin = param.m_LV2_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV2_attackInterspaceMax;
				component.BossParam.AttackSpeedMin = param.m_LV2_attackSpeedMin;
				component.BossParam.AttackSpeedMax = param.m_LV2_attackSpeedMax;
				component.BossParam.MissileSpeed = param.m_LV2_missileSpeed;
				component.BossParam.MissileInterspace = param.m_LV2_missileInterspace;
				component.BossParam.BoundParamMin = param.m_LV2_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV2_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV2_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV2_bumperRand;
				component.BossParam.BallSpeed = param.m_LV2_ballSpeed;
				break;
			case 3:
				component.BossParam.BossHPMax = param.m_LV3_hp;
				component.BossParam.BossDistance = param.m_LV3_distance;
				component.BossParam.TableID = param.m_LV3_tblId;
				component.BossParam.AttackInterspaceMin = param.m_LV3_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV3_attackInterspaceMax;
				component.BossParam.AttackSpeedMin = param.m_LV3_attackSpeedMin;
				component.BossParam.AttackSpeedMax = param.m_LV3_attackSpeedMax;
				component.BossParam.MissileSpeed = param.m_LV3_missileSpeed;
				component.BossParam.MissileInterspace = param.m_LV3_missileInterspace;
				component.BossParam.BoundParamMin = param.m_LV3_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV3_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV3_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV3_bumperRand;
				component.BossParam.BallSpeed = param.m_LV3_ballSpeed;
				break;
			case 4:
				component.BossParam.BossHPMax = param.m_LV4_hp;
				component.BossParam.BossDistance = param.m_LV4_distance;
				component.BossParam.TableID = param.m_LV4_tblId;
				component.BossParam.AttackInterspaceMin = param.m_LV4_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV4_attackInterspaceMax;
				component.BossParam.AttackSpeedMin = param.m_LV4_attackSpeedMin;
				component.BossParam.AttackSpeedMax = param.m_LV4_attackSpeedMax;
				component.BossParam.MissileSpeed = param.m_LV4_missileSpeed;
				component.BossParam.MissileInterspace = param.m_LV4_missileInterspace;
				component.BossParam.BoundParamMin = param.m_LV4_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV4_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV4_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV4_bumperRand;
				component.BossParam.BallSpeed = param.m_LV4_ballSpeed;
				break;
			case 5:
				component.BossParam.BossHPMax = param.m_LV5_hp;
				component.BossParam.BossDistance = param.m_LV5_distance;
				component.BossParam.TableID = param.m_LV5_tblId;
				component.BossParam.AttackInterspaceMin = param.m_LV5_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV5_attackInterspaceMax;
				component.BossParam.AttackSpeedMin = param.m_LV5_attackSpeedMin;
				component.BossParam.AttackSpeedMax = param.m_LV5_attackSpeedMax;
				component.BossParam.MissileSpeed = param.m_LV5_missileSpeed;
				component.BossParam.MissileInterspace = param.m_LV5_missileInterspace;
				component.BossParam.BoundParamMin = param.m_LV5_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV5_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV5_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV5_bumperRand;
				component.BossParam.BallSpeed = param.m_LV5_ballSpeed;
				break;
			}
			component.DebugDrawInfo("Map2 level=" + mapBossLevel + "BossHPMax=" + component.BossParam.BossHPMax + "BossDistance=" + component.BossParam.BossDistance + "TableID=" + component.BossParam.TableID);
			component.SetInitState(STATE_ID.AppearMap2);
			component.SetDamageState(STATE_ID.DamageMap2);
			component.Setup();
		}
	}
}
