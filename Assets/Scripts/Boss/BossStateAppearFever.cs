using Message;
using UnityEngine;

namespace Boss
{
	public class BossStateAppearFever : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Start,
			Appear,
			Open,
			Wait
		}

		private const float PLAYER_DISTANCE = 9f;

		private const float WAIT_TIME = 2f;

		private static Vector3 APPEAR_ROT = new Vector3(0f, 90f, 0f);

		private State m_state;

		private float m_time;

		private float m_distance;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStateAppearFever");
			context.SetHitCheck(false);
			context.transform.rotation = Quaternion.Euler(APPEAR_ROT);
			context.OpenHpGauge();
			m_state = State.Start;
			m_time = 0f;
			m_distance = 9f + context.BossParam.AddSpeedDistance;
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			switch (m_state)
			{
			case State.Start:
				context.BossMotion.SetMotion(BossMotion.MOVE_R);
				context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
				m_state = State.Appear;
				break;
			case State.Appear:
			{
				float playerDistance = context.GetPlayerDistance();
				if (playerDistance < m_distance)
				{
					context.RequestStartChaoAbility();
					context.KeepSpeed();
					context.BossEffect.PlayFoundEffect();
					context.BossMotion.SetMotion(BossMotion.NOTICE);
					ObjUtil.PlaySE("boss_find");
					GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialFeverBoss", new MsgTutorialFeverBoss(), SendMessageOptions.DontRequireReceiver);
					m_time = 0f;
					m_state = State.Open;
				}
				break;
			}
			case State.Open:
				m_time += delta;
				if (m_time > 0.6f)
				{
					ObjUtil.PlaySE("boss_bomb_drop");
					m_time = 0f;
					m_state = State.Wait;
				}
				break;
			case State.Wait:
				m_time += delta;
				if (m_time > 2f)
				{
					context.BossParam.SetupBossTable();
					context.StartGauge();
					context.ChangeState(STATE_ID.AttackFever);
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
