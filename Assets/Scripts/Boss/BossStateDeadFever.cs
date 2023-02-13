using Tutorial;
using UnityEngine;

namespace Boss
{
	public class BossStateDeadFever : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Wait1,
			Wait2,
			Bom,
			End
		}

		private const float PASS_SPEED = 2f;

		private const float BOM_DISTANCE = 7f;

		private const float BOM_TIME = 0.7f;

		private const float WAIT_TIME1 = 1f;

		private const float WAIT_TIME2 = 0.5f;

		private const float BOM_SHOT_SPEED = 25f;

		private State m_state;

		private float m_pass_speed;

		private float m_time;

		private float m_distance;

		private float m_bom_time;

		private GameObject m_bom_obj;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStateDeadFever");
			context.SetHitCheck(false);
			m_state = State.Wait1;
			m_pass_speed = 0f;
			m_time = 0f;
			m_bom_time = 0.7f;
			m_distance = 7f + context.BossParam.AddSpeedDistance;
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
			ObjUtil.SendMessageTutorialClear(EventID.FEVER_BOSS);
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			context.UpdateSpeedUp(delta, m_pass_speed);
			switch (m_state)
			{
			case State.Wait1:
				m_time += delta;
				if (m_time > 1f)
				{
					context.BossMotion.SetMotion(BossMotion.ESCAPE);
					m_time = 0f;
					m_state = State.Wait2;
				}
				break;
			case State.Wait2:
				m_time += delta;
				if (m_time > 0.5f)
				{
					m_pass_speed = context.BossParam.PlayerSpeed * 2f;
					m_time = 0f;
					m_state = State.Bom;
				}
				break;
			case State.Bom:
			{
				float playerBossPositionX = context.GetPlayerBossPositionX();
				if (playerBossPositionX > m_distance)
				{
					m_bom_obj = context.CreateBom(false, 25f, true);
					m_time = 0f;
					m_state = State.End;
				}
				break;
			}
			case State.End:
				m_time += delta;
				if (m_time > m_bom_time)
				{
					context.BlastBom(m_bom_obj);
					context.BossEnd(true);
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
