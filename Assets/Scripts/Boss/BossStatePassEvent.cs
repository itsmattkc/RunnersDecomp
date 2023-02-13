namespace Boss
{
	public class BossStatePassEvent : FSMState<ObjBossEventBossState>
	{
		private enum State
		{
			Idle,
			Wait1,
			Wait2,
			End
		}

		private const float PASS_SPEED = 2f;

		private const float END_DISTANCE = 8f;

		private const float END_TIME = 1f;

		private const float WAIT_TIME = 1f;

		private State m_state;

		private float m_time;

		private float m_pass_speed;

		public override void Enter(ObjBossEventBossState context)
		{
			context.DebugDrawState("BossStatePassMap");
			context.SetHitCheck(false);
			m_state = State.Wait1;
			m_time = 0f;
			context.BossMotion.SetMotion(EventBossMotion.PASS);
		}

		public override void Leave(ObjBossEventBossState context)
		{
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			switch (m_state)
			{
			case State.Wait1:
				m_time += delta;
				if (m_time > 1f)
				{
					m_pass_speed = context.BossParam.PlayerSpeed * 2f;
					m_state = State.Wait2;
				}
				break;
			case State.Wait2:
			{
				context.UpdateSpeedUp(delta, m_pass_speed);
				float playerBossPositionX = context.GetPlayerBossPositionX();
				if (playerBossPositionX > 8f)
				{
					context.SetFailed();
					m_time = 0f;
					m_state = State.End;
				}
				break;
			}
			case State.End:
				context.UpdateSpeedUp(delta, m_pass_speed);
				m_time += delta;
				if (m_time > 1f)
				{
					context.BossEnd(false);
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
