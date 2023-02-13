namespace Boss
{
	public class BossStatePassEventDistanceEnd : FSMState<ObjBossEventBossState>
	{
		private const float WAIT_TIME = 1f;

		private float m_time;

		public override void Enter(ObjBossEventBossState context)
		{
			context.DebugDrawState("BossStatePassEventDistanceEnd");
			context.SetHitCheck(false);
			m_time = 0f;
		}

		public override void Leave(ObjBossEventBossState context)
		{
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			m_time += delta;
			if (m_time > 1f)
			{
				context.ChangeState(EVENTBOSS_STATE_ID.Pass);
			}
		}
	}
}
