using Message;
using Tutorial;
using UnityEngine;

namespace Boss
{
	public class BossStatePassFever : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Bom,
			Shot,
			End
		}

		private const float PASS_SPEED = 2f;

		private const float BOM_DISTANCE1 = 0f;

		private const float BOM_DISTANCE2 = 9f;

		private const float BOM_DISTANCE3 = 7f;

		private const float BOM_SHOT_SPEED = 20f;

		private State m_state;

		private float m_pass_speed;

		private float m_distance_1;

		private float m_distance_2;

		private float m_distance_3;

		private GameObject m_bom_obj;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStatePassFever");
			context.SetHitCheck(false);
			m_state = State.Bom;
			m_pass_speed = context.BossParam.PlayerSpeed * 2f;
			m_distance_1 = 0f + context.BossParam.AddSpeedDistance;
			m_distance_2 = 9f + context.BossParam.AddSpeedDistance;
			m_distance_3 = 7f + context.BossParam.AddSpeedDistance;
			context.BossMotion.SetMotion(BossMotion.PASS);
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Attack);
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
			case State.Bom:
			{
				float playerBossPositionX = context.GetPlayerBossPositionX();
				if (playerBossPositionX > m_distance_1)
				{
					ObjUtil.PauseCombo(MsgPauseComboTimer.State.PAUSE_TIMER);
					m_bom_obj = context.CreateBom(false, 20f, false);
					m_state = State.Shot;
				}
				break;
			}
			case State.Shot:
			{
				float playerBossPositionX2 = context.GetPlayerBossPositionX();
				if (playerBossPositionX2 > m_distance_2)
				{
					context.ShotBom(m_bom_obj);
					m_state = State.End;
				}
				break;
			}
			case State.End:
			{
				Vector3 position = m_bom_obj.transform.position;
				float x = position.x;
				Vector3 playerPosition = context.GetPlayerPosition();
				float num = x - playerPosition.x;
				if (num < m_distance_3)
				{
					context.BlastBom(m_bom_obj);
					context.BossEnd(false);
					m_state = State.Idle;
				}
				break;
			}
			}
		}
	}
}
