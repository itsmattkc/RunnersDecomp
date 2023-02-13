using UnityEngine;

namespace Boss
{
	public class BossStateAttackFever : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Start,
			Bom,
			Speedup,
			SpeedupEnd
		}

		private const float PASS_DISTANCE = 8f;

		private const float SWEAT_DISTANCE = 5f;

		private const float SPEEDUP_DISTANCE = 10f;

		private State m_state;

		private float m_speed_down;

		private float m_speed_down2;

		private float m_speed_up;

		private float m_distance_pass;

		private float m_distance_sweat;

		private bool m_sweat_effect;

		private float m_sweat_effect_time;

		private float m_time;

		private float m_attackInterspace;

		private bool m_bumper = true;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStateSpeedDown");
			context.SetHitCheck(true);
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
			m_state = State.Start;
			m_time = 0f;
			m_speed_down = context.BossParam.DownSpeed * context.BossParam.AddSpeedRatio;
			m_speed_down2 = m_speed_down;
			m_speed_up = 0f;
			m_distance_pass = 8f + context.BossParam.AddSpeedDistance;
			m_distance_sweat = 5f + context.BossParam.AddSpeedDistance;
			m_sweat_effect = false;
			m_sweat_effect_time = 0f;
			m_bumper = true;
		}

		public override void Leave(ObjBossEggmanState context)
		{
			context.SetHitCheck(false);
			context.BossEffect.PlaySweatEffectEnd();
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			context.UpdateSpeedDown(delta, m_speed_down);
			float playerBossPositionX = context.GetPlayerBossPositionX();
			if (playerBossPositionX < 0f)
			{
				SetSweatEffect(context);
				m_bumper = false;
				if (Mathf.Abs(playerBossPositionX) > m_distance_pass)
				{
					context.ChangeState(STATE_ID.PassFever);
				}
				return;
			}
			if (playerBossPositionX < m_distance_sweat)
			{
				SetSweatEffect(context);
				m_bumper = false;
			}
			else
			{
				ResetSweatEffect(context, delta);
				m_bumper = true;
			}
			switch (m_state)
			{
			case State.Start:
				if (context.IsHitBumper())
				{
					if (playerBossPositionX < 10f)
					{
						m_speed_up = context.BossParam.BumperSpeedup;
						m_state = State.Speedup;
					}
				}
				else
				{
					context.CreateBumper(true);
					m_attackInterspace = context.GetAttackInterspace();
					ResetTime();
					m_state = State.Bom;
				}
				break;
			case State.Bom:
				if (context.IsBossDistanceEnd())
				{
					context.ChangeState(STATE_ID.PassFeverDistanceEnd);
				}
				else if (context.IsHitBumper())
				{
					if (playerBossPositionX < 10f)
					{
						m_speed_up = context.BossParam.BumperSpeedup;
						m_state = State.Speedup;
					}
				}
				else if (UpdateTime(delta, m_attackInterspace) && m_bumper)
				{
					m_state = State.Start;
				}
				break;
			case State.Speedup:
				context.SetSpeed(m_speed_up * 0.1f);
				m_state = State.SpeedupEnd;
				break;
			case State.SpeedupEnd:
			{
				float num = context.BossParam.PlayerSpeed * 0.7f;
				if (context.BossParam.Speed < num)
				{
					m_speed_down = m_speed_down2;
					context.SetSpeed(num);
					m_state = State.Bom;
				}
				else
				{
					m_speed_down += delta * m_speed_up * 1.2f;
				}
				break;
			}
			}
		}

		private bool UpdateTime(float delta, float time_max)
		{
			m_time += delta;
			if (m_time > time_max)
			{
				return true;
			}
			return false;
		}

		private void ResetTime()
		{
			m_time = 0f;
		}

		private void SetSweatEffect(ObjBossEggmanState context)
		{
			if (!m_sweat_effect)
			{
				context.BossEffect.PlaySweatEffectStart();
				m_sweat_effect_time = 1f;
				m_sweat_effect = true;
			}
		}

		private void ResetSweatEffect(ObjBossEggmanState context, float delta)
		{
			m_sweat_effect_time -= delta;
			if (m_sweat_effect_time < 0f && m_sweat_effect)
			{
				context.BossEffect.PlaySweatEffectEnd();
				m_sweat_effect = false;
			}
		}
	}
}
