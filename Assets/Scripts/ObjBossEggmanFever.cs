using Boss;

public class ObjBossEggmanFever : ObjBossEggmanBase
{
	private const string FeverModelName = "enm_eggmobile_f";

	public void SetObjBossEggmanFeverParameter(ObjBossEggmanFeverParameter param)
	{
		ObjBossEggmanState component = GetComponent<ObjBossEggmanState>();
		if (component != null)
		{
			component.Init();
			component.BossParam.TypeBoss = 0;
			component.BossParam.BossHPMax = param.m_hp;
			component.BossParam.BossDistance = param.m_distance;
			component.BossParam.TableID = param.m_tblId;
			component.BossParam.DownSpeed = param.m_downSpeed;
			component.BossParam.AttackInterspaceMin = param.m_attackInterspaceMin;
			component.BossParam.AttackInterspaceMax = param.m_attackInterspaceMax;
			component.BossParam.BoundParamMin = param.m_boundParamMin;
			component.BossParam.BoundParamMax = param.m_boundParamMax;
			component.BossParam.BoundMaxRand = param.m_boundMaxRand;
			component.BossParam.ShotSpeed = param.m_shotSpeed;
			component.BossParam.BumperFirstSpeed = param.m_bumperFirstSpeed;
			component.BossParam.BumperOutOfcontrol = param.m_bumperOutOfcontrol;
			component.BossParam.BumperSpeedup = param.m_bumperSpeedup;
			component.BossParam.BallSpeed = param.m_ballSpeed;
			component.SetInitState(STATE_ID.AppearFever);
			component.SetDamageState(STATE_ID.DamageFever);
			component.Setup();
		}
	}

	protected override string GetModelName()
	{
		return "enm_eggmobile_f";
	}
}
