namespace Boss
{
	public class BossStatePassMap : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Wait,
			End
		}

		private const float PASS_SPEED = 2f;

		private const float END_DISTANCE = 8f;

		private const float END_TIME = 1f;

		private State m_state;

		private float m_time;

		private float m_pass_speed;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStatePassMap");
			context.SetHitCheck(false);
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
			m_state = State.Wait;
			m_time = 0f;
			m_pass_speed = context.BossParam.PlayerSpeed * 2f;
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			context.UpdateSpeedUp(delta, m_pass_speed);
			switch (m_state)
			{
			case State.Wait:
			{
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
