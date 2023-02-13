using Boss;
using UnityEngine;

public class ObjBossEggmanMap3 : ObjBossEggmanBase
{
	public int m_debugLevel;

	public void SetObjBossEggmanMap3Parameter(ObjBossEggmanMap3Parameter param)
	{
		ObjBossEggmanState component = GetComponent<ObjBossEggmanState>();
		if (component != null)
		{
			component.Init();
			int mapBossLevel = GetMapBossLevel();
			mapBossLevel = Mathf.Max(mapBossLevel, 1);
			mapBossLevel = Mathf.Min(mapBossLevel, 5);
			component.BossParam.TypeBoss = 3;
			component.BossParam.BossLevel = mapBossLevel;
			component.BossParam.DefaultPlayerDistance = param.m_playerDistance;
			component.BossParam.AttackSpeed = param.m_attackSpeed;
			switch (mapBossLevel)
			{
			case 1:
				component.BossParam.BossHPMax = param.m_LV1_hp;
				component.BossParam.BossDistance = param.m_LV1_distance;
				component.BossParam.TableID = param.m_LV1_tblId;
				component.BossParam.AttackTableID = param.m_LV1_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV1_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV1_attackInterspaceMax;
				component.BossParam.RotSpeed = param.m_LV1_rotSpeed;
				component.BossParam.AttackTrapCountMax = param.m_LV1_attackTrapCountMax;
				break;
			case 2:
				component.BossParam.BossHPMax = param.m_LV2_hp;
				component.BossParam.BossDistance = param.m_LV2_distance;
				component.BossParam.TableID = param.m_LV2_tblId;
				component.BossParam.AttackTableID = param.m_LV2_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV2_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV2_attackInterspaceMax;
				component.BossParam.RotSpeed = param.m_LV2_rotSpeed;
				component.BossParam.AttackTrapCountMax = param.m_LV2_attackTrapCountMax;
				break;
			case 3:
				component.BossParam.BossHPMax = param.m_LV3_hp;
				component.BossParam.BossDistance = param.m_LV3_distance;
				component.BossParam.TableID = param.m_LV3_tblId;
				component.BossParam.AttackTableID = param.m_LV3_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV3_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV3_attackInterspaceMax;
				component.BossParam.RotSpeed = param.m_LV3_rotSpeed;
				component.BossParam.AttackTrapCountMax = param.m_LV3_attackTrapCountMax;
				break;
			case 4:
				component.BossParam.BossHPMax = param.m_LV4_hp;
				component.BossParam.BossDistance = param.m_LV4_distance;
				component.BossParam.TableID = param.m_LV4_tblId;
				component.BossParam.AttackTableID = param.m_LV4_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV4_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV4_attackInterspaceMax;
				component.BossParam.RotSpeed = param.m_LV4_rotSpeed;
				component.BossParam.AttackTrapCountMax = param.m_LV4_attackTrapCountMax;
				break;
			case 5:
				component.BossParam.BossHPMax = param.m_LV5_hp;
				component.BossParam.BossDistance = param.m_LV5_distance;
				component.BossParam.TableID = param.m_LV5_tblId;
				component.BossParam.AttackTableID = param.m_LV5_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV5_attackInterspaceMin;
				component.BossParam.AttackInterspaceMax = param.m_LV5_attackInterspaceMax;
				component.BossParam.RotSpeed = param.m_LV5_rotSpeed;
				component.BossParam.AttackTrapCountMax = param.m_LV5_attackTrapCountMax;
				break;
			}
			component.DebugDrawInfo("Map3 level=" + mapBossLevel + "DefaultPlayerDistance=" + component.BossParam.DefaultPlayerDistance + "AttackSpeed=" + component.BossParam.AttackSpeed + "BossHPMax=" + component.BossParam.BossHPMax + "BossDistance=" + component.BossParam.BossDistance + "TableID=" + component.BossParam.TableID + "RotSpeed=" + component.BossParam.RotSpeed);
			component.SetInitState(STATE_ID.AppearMap3);
			component.SetDamageState(STATE_ID.DamageMap3);
			component.Setup();
		}
	}
}
