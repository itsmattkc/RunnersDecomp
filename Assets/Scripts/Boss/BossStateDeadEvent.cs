namespace Boss
{
	public class BossStateDeadEvent : FSMState<ObjBossEventBossState>
	{
		private enum State
		{
			Idle,
			Wait1,
			Wait2
		}

		private const float PASS_SPEED = 2f;

		private const float WAIT_TIME1 = 1.5f;

		private const float WAIT_TIME2 = 3f;

		private State m_state;

		private float m_pass_speed;

		private float m_time;

		public override void Enter(ObjBossEventBossState context)
		{
			context.DebugDrawState("BossStateDeadMap");
			context.SetHitCheck(false);
			m_state = State.Wait1;
			m_pass_speed = 0f;
			m_time = 0f;
		}

		public override void Leave(ObjBossEventBossState context)
		{
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			context.UpdateSpeedUp(delta, m_pass_speed);
			switch (m_state)
			{
			case State.Wait1:
				m_time += delta;
				if (m_time > 1.5f)
				{
					context.SetClear();
					m_pass_speed = context.BossParam.PlayerSpeed * 2f;
					m_time = 0f;
					m_state = State.Wait2;
				}
				break;
			case State.Wait2:
				m_time += delta;
				if (m_time > 3f)
				{
					context.BossEnd(true);
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
