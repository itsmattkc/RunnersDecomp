using UnityEngine;

namespace Boss
{
	public class BossStateDamageBase : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			SpeedDown,
			Wait
		}

		private const float START_SPEED = 60f;

		private const float START_DOWNSPEED = 120f;

		private const float ROT_SPEED = 30f;

		private const float ROT_DOWNSPEED = 0.3f;

		private const float ROT_MIN = 10f;

		private State m_state;

		private Quaternion m_start_rot = Quaternion.identity;

		private float m_speed_down;

		private float m_rot_speed;

		private float m_rot_speed_down;

		private float m_rot_time;

		private float m_rot_min;

		private Vector3 m_rot_agl = Vector3.zero;

		private float m_distance;

		private bool m_rot_end;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState(GetStateName());
			context.SetHitCheck(false);
			context.AddDamage();
			context.BossEffect.PlayHitEffect();
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
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
			m_rot_speed = 0f;
			m_rot_speed_down = 0f;
			m_rot_time = 0f;
			m_rot_min = 0f;
			m_rot_agl = context.transform.right;
			m_distance = 0f;
			m_rot_end = false;
			Setup(context);
			if (context.ColorPowerHit)
			{
				context.DebugDrawState(GetStateName() + "ColorPowerHit");
				SetupColorPowerDamage(context);
			}
			else
			{
				context.CreateFeverRing();
			}
		}

		public override void Leave(ObjBossEggmanState context)
		{
			context.ColorPowerHit = false;
			context.ChaoHit = false;
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			context.UpdateSpeedDown(delta, m_speed_down);
			if (!m_rot_end)
			{
				float d = delta * 60f * m_rot_speed;
				m_rot_speed -= delta * 60f * m_rot_speed_down;
				context.transform.rotation = Quaternion.Euler(m_rot_agl * d) * context.transform.rotation;
				Vector3 eulerAngles = context.transform.rotation.eulerAngles;
				float x = eulerAngles.x;
				if (m_rot_speed < m_rot_min && x > 270f && x < 359f)
				{
					SetMotion(context, false);
					m_rot_time = 0f;
					m_rot_end = true;
				}
			}
			if (m_rot_end)
			{
				m_rot_time += delta * 5f;
				context.transform.rotation = Quaternion.Slerp(context.transform.rotation, m_start_rot, m_rot_time);
			}
			switch (m_state)
			{
			case State.SpeedDown:
				if (context.BossParam.Speed < context.BossParam.PlayerSpeed)
				{
					SetStateSpeedDown(context);
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
					ChangeStateWait(context);
				}
				break;
			}
			}
		}

		protected virtual string GetStateName()
		{
			return string.Empty;
		}

		protected virtual void Setup(ObjBossEggmanState context)
		{
		}

		private void SetupColorPowerDamage(ObjBossEggmanState context)
		{
			float damageSpeedParam = context.GetDamageSpeedParam();
			context.SetSpeed(60f * damageSpeedParam);
			SetSpeedDown(120f);
			SetRotSpeed(30f);
			SetRotSpeedDown(0.3f);
			SetRotMin(10f);
			SetRotAngle(-context.transform.right);
		}

		private void SetMotion(ObjBossEggmanState context, bool flag)
		{
			if (flag)
			{
				context.BossMotion.SetMotion(BossMotion.DAMAGE_R_HC, flag);
				return;
			}
			if (context.BossParam.BossHP > 0)
			{
				context.BossMotion.SetMotion(BossMotion.DAMAGE_R_HC, false);
				return;
			}
			context.BossEffect.PlayEscapeEffect(context);
			context.BossMotion.SetMotion(BossMotion.ESCAPE);
		}

		protected virtual void SetStateSpeedDown(ObjBossEggmanState context)
		{
		}

		protected virtual void ChangeStateWait(ObjBossEggmanState context)
		{
		}

		protected void SetRotAngle(Vector3 angle)
		{
			m_rot_agl = angle;
		}

		protected void SetSpeedDown(float val)
		{
			m_speed_down = val;
		}

		protected void SetRotSpeed(float val)
		{
			m_rot_speed = val;
		}

		protected void SetRotSpeedDown(float val)
		{
			m_rot_speed_down = val;
		}

		protected void SetRotMin(float val)
		{
			m_rot_min = val;
		}

		protected void SetDistance(float val)
		{
			m_distance = val;
		}
	}
}
