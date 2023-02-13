namespace Boss
{
	public class BossStatePassMapDistanceEnd : FSMState<ObjBossEggmanState>
	{
		private const float WAIT_TIME = 1f;

		private float m_time;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStatePassMapDistanceEnd");
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
			context.SetHitCheck(false);
			m_time = 0f;
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			m_time += delta;
			if (m_time > 1f)
			{
				context.ChangeState(STATE_ID.PassMap);
			}
		}
	}
}
