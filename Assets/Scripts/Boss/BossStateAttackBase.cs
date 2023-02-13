namespace Boss
{
	public class BossStateAttackBase : FSMState<ObjBossEggmanState>
	{
		private float m_speed;

		private float m_time;

		private float m_boss_y;

		public override void Enter(ObjBossEggmanState context)
		{
			context.SetHitCheck(true);
			context.SetupMoveY(0f);
			m_speed = 0f;
			m_time = 0f;
			m_boss_y = 0f;
		}

		public override void Leave(ObjBossEggmanState context)
		{
			context.SetHitCheck(false);
		}

		protected bool UpdateTime(float delta, float time_max)
		{
			m_time += delta;
			if (m_time > time_max)
			{
				return true;
			}
			return false;
		}

		protected void ResetTime()
		{
			m_time = 0f;
		}

		protected void UpdateMove(ObjBossEggmanState context, float delta)
		{
			context.UpdateMoveY(delta, m_boss_y, m_speed);
		}

		protected void SetMove(ObjBossEggmanState context, float step, float speed, float boss_y)
		{
			context.SetupMoveY(step);
			m_speed = speed;
			m_boss_y = boss_y;
		}

		protected bool IsMoveStepEquals(ObjBossEggmanState context, float val)
		{
			if (context.BossParam.StepMoveY.Equals(val))
			{
				return true;
			}
			return false;
		}
	}
}
