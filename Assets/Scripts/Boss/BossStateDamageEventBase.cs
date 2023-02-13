using UnityEngine;

namespace Boss
{
	public class BossStateDamageEventBase : FSMState<ObjBossEventBossState>
	{
		private enum State
		{
			Idle,
			SpeedDown,
			Wait
		}

		private const float START_SPEED = 60f;

		private const float START_DOWNSPEED = 120f;

		private const float WAIT_DOWNSPEED = 1f;

		private const float ROT_SPEED = 30f;

		private const float ROT_DOWNSPEED = 0.3f;

		private const float ROT_MIN = 10f;

		private State m_state;

		private Quaternion m_start_rot = Quaternion.identity;

		private float m_speed_down;

		private float m_distance;

		private bool m_rot_end;

		public override void Enter(ObjBossEventBossState context)
		{
			context.DebugDrawState("BossStateDamageEvent");
			context.SetHitCheck(false);
			context.AddDamage();
			context.BossEffect.PlayHitEffect(context.BossParam.BoostLevel);
			if (context.BossParam.BossHP > 0)
			{
				ObjUtil.PlaySE("boss_damage");
			}
			else
			{
				ObjUtil.PlaySE("boss_explosion");
				context.BossClear();
			}
			SetMotion(context, true);
			m_state = State.SpeedDown;
			m_start_rot = context.transform.rotation;
			m_speed_down = 0f;
			m_distance = 0f;
			m_rot_end = false;
			float damageSpeedParam = context.GetDamageSpeedParam();
			context.SetSpeed(60f * damageSpeedParam);
			SetSpeedDown(120f);
			SetDistance(context.BossParam.DefaultPlayerDistance);
			if (context.ColorPowerHit)
			{
				context.DebugDrawState("BossStateDamageEvent ColorPowerHit");
				SetupColorPowerDamage(context);
			}
			else
			{
				context.CreateEventFeverRing(context.GetDropRingAggressivity());
			}
		}

		public override void Leave(ObjBossEventBossState context)
		{
			context.ColorPowerHit = false;
			context.ChaoHit = false;
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			context.UpdateSpeedDown(delta, m_speed_down);
			switch (m_state)
			{
			case State.SpeedDown:
				if (context.BossParam.Speed < context.BossParam.PlayerSpeed)
				{
					SetSpeedDown(1f);
					m_state = State.Wait;
				}
				break;
			case State.Wait:
			{
				if (context.IsPlayerDead() && context.IsClear())
				{
					ChangeStateWait(context);
					break;
				}
				float playerDistance = context.GetPlayerDistance();
				if (playerDistance < m_distance)
				{
					context.SetSpeed(context.BossParam.PlayerSpeed * 0.7f);
					context.transform.rotation = m_start_rot;
					if (!m_rot_end)
					{
						SetMotion(context, false);
					}
					context.KeepSpeed();
					ChangeStateWait(context);
				}
				break;
			}
			}
		}

		private void SetupColorPowerDamage(ObjBossEventBossState context)
		{
			context.SetSpeed(60f);
			SetSpeedDown(120f);
		}

		private void SetMotion(ObjBossEventBossState context, bool flag)
		{
			if (flag)
			{
				context.BossMotion.SetMotion(EventBossMotion.DAMAGE);
				return;
			}
			if (context.BossParam.BossHP > 0)
			{
				context.BossMotion.SetMotion(EventBossMotion.ATTACK);
				return;
			}
			context.BossEffect.PlayEscapeEffect(context);
			context.BossMotion.SetMotion(EventBossMotion.ESCAPE);
		}

		protected virtual void ChangeStateWait(ObjBossEventBossState context)
		{
		}

		protected void SetSpeedDown(float val)
		{
			m_speed_down = val;
		}

		protected void SetDistance(float val)
		{
			m_distance = val;
		}
	}
}
