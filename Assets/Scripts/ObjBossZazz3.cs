using Boss;
using UnityEngine;

public class ObjBossZazz3 : ObjBossEventBossBase
{
	private const string ModelName = "enm_zazz_p";

	public int m_debugLevel;

	protected override string GetModelName()
	{
		return "enm_zazz_p";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.EVENT_RESOURCE;
	}

	protected override BossType GetBossType()
	{
		return BossType.EVENT3;
	}

	public void SetObjBossZazz3Parameter(ObjBossZazz3Parameter param)
	{
		ObjBossEventBossState component = GetComponent<ObjBossEventBossState>();
		if (component != null)
		{
			component.Init();
			int eventBossLevel = GetEventBossLevel();
			eventBossLevel = Mathf.Max(eventBossLevel, 1);
			eventBossLevel = Mathf.Min(eventBossLevel, 5);
			int eventBossHpMax = GetEventBossHpMax();
			eventBossHpMax = Mathf.Max(eventBossHpMax, 1);
			component.BossParam.TypeBoss = (int)GetBossType();
			component.BossParam.BossLevel = eventBossLevel;
			component.BossParam.BossHPMax = eventBossHpMax;
			component.BossParam.DefaultPlayerDistance = param.m_playerDistance;
			component.BossParam.BumperFirstSpeed = param.m_bumperFirstSpeed;
			component.BossParam.BumperOutOfcontrol = param.m_bumperOutOfcontrol;
			component.BossParam.BumperSpeedup = param.m_bumperSpeedup;
			component.BossParam.ShotSpeed = param.m_shotSpeed;
			switch (eventBossLevel)
			{
			case 1:
				component.BossParam.BossDistance = param.m_LV1_distance;
				component.BossParam.TableID = param.m_LV1_tblId;
				component.BossParam.AttackTableID = param.m_LV1_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV1_attackInterspace;
				component.BossParam.AttackInterspaceMax = param.m_LV1_attackWaitTime;
				component.BossParam.RotSpeed = param.m_LV1_rotSpeed;
				component.BossParam.BoundParamMin = param.m_LV1_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV1_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV1_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV1_bumperRand;
				component.BossParam.BallSpeed = param.m_LV1_ballSpeed;
				component.BossParam.WispInterspace = param.m_LV1_wispInterspace;
				component.BossParam.BumperInterspace = param.m_LV1_bumperInterspace;
				component.BossParam.WispSpeedMin = param.m_LV1_wispSpeedMin;
				component.BossParam.WispSpeedMax = param.m_LV1_wispSpeedMax;
				component.BossParam.WispSwingMin = param.m_LV1_wispSwingMin;
				component.BossParam.WispSwingMax = param.m_LV1_wispSwingMax;
				component.BossParam.WispAddXMin = param.m_LV1_wispAddXMin;
				component.BossParam.WispAddXMax = param.m_LV1_wispAddXMax;
				break;
			case 2:
				component.BossParam.BossDistance = param.m_LV2_distance;
				component.BossParam.TableID = param.m_LV2_tblId;
				component.BossParam.AttackTableID = param.m_LV2_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV2_attackInterspace;
				component.BossParam.AttackInterspaceMax = param.m_LV2_attackWaitTime;
				component.BossParam.RotSpeed = param.m_LV2_rotSpeed;
				component.BossParam.BoundParamMin = param.m_LV2_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV2_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV2_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV2_bumperRand;
				component.BossParam.BallSpeed = param.m_LV2_ballSpeed;
				component.BossParam.WispInterspace = param.m_LV2_wispInterspace;
				component.BossParam.BumperInterspace = param.m_LV2_bumperInterspace;
				component.BossParam.WispSpeedMin = param.m_LV2_wispSpeedMin;
				component.BossParam.WispSpeedMax = param.m_LV2_wispSpeedMax;
				component.BossParam.WispSwingMin = param.m_LV2_wispSwingMin;
				component.BossParam.WispSwingMax = param.m_LV2_wispSwingMax;
				component.BossParam.WispAddXMin = param.m_LV2_wispAddXMin;
				component.BossParam.WispAddXMax = param.m_LV2_wispAddXMax;
				break;
			case 3:
				component.BossParam.BossDistance = param.m_LV3_distance;
				component.BossParam.TableID = param.m_LV3_tblId;
				component.BossParam.AttackTableID = param.m_LV3_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV3_attackInterspace;
				component.BossParam.AttackInterspaceMax = param.m_LV3_attackWaitTime;
				component.BossParam.RotSpeed = param.m_LV3_rotSpeed;
				component.BossParam.BoundParamMin = param.m_LV3_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV3_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV3_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV3_bumperRand;
				component.BossParam.BallSpeed = param.m_LV3_ballSpeed;
				component.BossParam.WispInterspace = param.m_LV3_wispInterspace;
				component.BossParam.BumperInterspace = param.m_LV3_bumperInterspace;
				component.BossParam.WispSpeedMin = param.m_LV3_wispSpeedMin;
				component.BossParam.WispSpeedMax = param.m_LV3_wispSpeedMax;
				component.BossParam.WispSwingMin = param.m_LV3_wispSwingMin;
				component.BossParam.WispSwingMax = param.m_LV3_wispSwingMax;
				component.BossParam.WispAddXMin = param.m_LV3_wispAddXMin;
				component.BossParam.WispAddXMax = param.m_LV3_wispAddXMax;
				break;
			case 4:
				component.BossParam.BossDistance = param.m_LV4_distance;
				component.BossParam.TableID = param.m_LV4_tblId;
				component.BossParam.AttackTableID = param.m_LV4_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV4_attackInterspace;
				component.BossParam.AttackInterspaceMax = param.m_LV4_attackWaitTime;
				component.BossParam.RotSpeed = param.m_LV4_rotSpeed;
				component.BossParam.BoundParamMin = param.m_LV4_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV4_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV4_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV4_bumperRand;
				component.BossParam.BallSpeed = param.m_LV4_ballSpeed;
				component.BossParam.WispInterspace = param.m_LV4_wispInterspace;
				component.BossParam.BumperInterspace = param.m_LV4_bumperInterspace;
				component.BossParam.WispSpeedMin = param.m_LV4_wispSpeedMin;
				component.BossParam.WispSpeedMax = param.m_LV4_wispSpeedMax;
				component.BossParam.WispSwingMin = param.m_LV4_wispSwingMin;
				component.BossParam.WispSwingMax = param.m_LV4_wispSwingMax;
				component.BossParam.WispAddXMin = param.m_LV4_wispAddXMin;
				component.BossParam.WispAddXMax = param.m_LV4_wispAddXMax;
				break;
			case 5:
				component.BossParam.BossDistance = param.m_LV5_distance;
				component.BossParam.TableID = param.m_LV5_tblId;
				component.BossParam.AttackTableID = param.m_LV5_attackTblId;
				component.BossParam.AttackInterspaceMin = param.m_LV5_attackInterspace;
				component.BossParam.AttackInterspaceMax = param.m_LV5_attackWaitTime;
				component.BossParam.RotSpeed = param.m_LV5_rotSpeed;
				component.BossParam.BoundParamMin = param.m_LV5_boundParamMin;
				component.BossParam.BoundParamMax = param.m_LV5_boundParamMax;
				component.BossParam.BoundMaxRand = param.m_LV5_boundMaxRand;
				component.BossParam.BumperRand = param.m_LV5_bumperRand;
				component.BossParam.BallSpeed = param.m_LV5_ballSpeed;
				component.BossParam.WispInterspace = param.m_LV5_wispInterspace;
				component.BossParam.BumperInterspace = param.m_LV5_bumperInterspace;
				component.BossParam.WispSpeedMin = param.m_LV5_wispSpeedMin;
				component.BossParam.WispSpeedMax = param.m_LV5_wispSpeedMax;
				component.BossParam.WispSwingMin = param.m_LV5_wispSwingMin;
				component.BossParam.WispSwingMax = param.m_LV5_wispSwingMax;
				component.BossParam.WispAddXMin = param.m_LV5_wispAddXMin;
				component.BossParam.WispAddXMax = param.m_LV5_wispAddXMax;
				break;
			}
			component.DebugDrawInfo("Event2 level=" + eventBossLevel + "BossHPMax=" + component.BossParam.BossHPMax + "BossDistance=" + component.BossParam.BossDistance + "TableID=" + component.BossParam.TableID);
			component.SetInitState(EVENTBOSS_STATE_ID.AppearEvent2);
			component.SetDamageState(EVENTBOSS_STATE_ID.DamageEvent2);
			component.Setup();
		}
	}
}
