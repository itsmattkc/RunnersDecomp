namespace Boss
{
	public class BossStateAttackEvent2 : BossStateAttackEventBase
	{
		private enum State
		{
			Idle,
			Speedup
		}

		private State m_state;

		public override void Enter(ObjBossEventBossState context)
		{
			base.Enter(context);
			context.DebugDrawState("BossStateAttackEvent2");
			m_state = State.Idle;
		}

		public override void Leave(ObjBossEventBossState context)
		{
			base.Leave(context);
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			if (context.IsPlayerDead())
			{
				return;
			}
			if (context.IsBossDistanceEnd())
			{
				context.ChangeState(EVENTBOSS_STATE_ID.PassDistanceEnd);
				return;
			}
			UpdateTrapBall(context, delta);
			if (UpdateBumper(context, delta))
			{
				m_state = State.Speedup;
			}
			switch (m_state)
			{
			case State.Idle:
				if (UpdateBoost(context, delta))
				{
					context.ChangeState(EVENTBOSS_STATE_ID.AppearEvent2_2);
				}
				break;
			case State.Speedup:
				if (UpdateBumperSpeedup(context, delta))
				{
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
