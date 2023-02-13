using Message;
using UnityEngine;

namespace Boss
{
	public class BossStateAppearEventBase : FSMState<ObjBossEventBossState>
	{
		private enum State
		{
			Idle,
			Start,
			Move,
			Wait1,
			Wait2,
			Wait3
		}

		private static Vector3 APPEAR_ROT = new Vector3(0f, 90f, 0f);

		private static float APPEAR_OFFSET_POS_Y = 7f;

		private static float MOVE_SPEED = 2.5f;

		private static float START_TIME = 3.5f;

		private static float WAIT_TIME1 = 2f;

		private static float WAIT_TIME2 = 1f;

		private static float WAIT_TIME3 = 1f;

		private State m_state;

		private float m_time;

		public override void Enter(ObjBossEventBossState context)
		{
			context.DebugDrawState("BossStateAppearEvent");
			context.SetHitCheck(false);
			context.transform.rotation = Quaternion.Euler(APPEAR_ROT);
			Transform transform = context.transform;
			Vector3 playerPosition = context.GetPlayerPosition();
			float x = playerPosition.x + context.BossParam.DefaultPlayerDistance;
			Vector3 startPos = context.BossParam.StartPos;
			transform.position = new Vector3(x, startPos.y + APPEAR_OFFSET_POS_Y, 0f);
			context.SetupMoveY(1f);
			context.KeepSpeed();
			if (IsFirst())
			{
				context.OpenHpGauge();
			}
			else
			{
				context.BossMotion.SetMotion(EventBossMotion.ATTACK, false);
			}
			m_time = 0f;
			m_state = State.Start;
		}

		public override void Leave(ObjBossEventBossState context)
		{
		}

		public override void Step(ObjBossEventBossState context, float delta)
		{
			switch (m_state)
			{
			case State.Start:
				if (IsFirst())
				{
					m_time += delta;
					if (m_time > START_TIME)
					{
						m_time = 0f;
						m_state = State.Move;
					}
				}
				else
				{
					m_time = 0f;
					m_state = State.Move;
				}
				break;
			case State.Move:
			{
				Vector3 startPos = context.BossParam.StartPos;
				context.UpdateMoveY(delta, startPos.y, MOVE_SPEED);
				Vector3 startPos2 = context.BossParam.StartPos;
				float y = startPos2.y;
				Vector3 position = context.transform.position;
				if (Mathf.Abs(y - position.y) < 0.1f)
				{
					Transform transform = context.transform;
					Vector3 position2 = context.transform.position;
					float x = position2.x;
					Vector3 startPos3 = context.BossParam.StartPos;
					float y2 = startPos3.y;
					Vector3 position3 = context.transform.position;
					transform.position = new Vector3(x, y2, position3.z);
					if (IsFirst())
					{
						context.BossMotion.SetMotion(EventBossMotion.APPEAR);
					}
					m_time = 0f;
					m_state = State.Wait1;
				}
				break;
			}
			case State.Wait1:
				if (IsFirst())
				{
					m_time += delta;
					if (m_time > WAIT_TIME1)
					{
						MsgTutorialMapBoss value = new MsgTutorialMapBoss();
						GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialMapBoss", value, SendMessageOptions.DontRequireReceiver);
						context.BossParam.SetupBossTable();
						context.StartGauge();
						m_time = 0f;
						m_state = State.Wait2;
					}
				}
				else
				{
					m_time = 0f;
					m_state = State.Wait2;
				}
				break;
			case State.Wait2:
				if (IsFirst())
				{
					m_time += delta;
					if (m_time > WAIT_TIME2)
					{
						context.RequestStartChaoAbility();
						m_time = 0f;
						m_state = State.Wait3;
					}
				}
				else
				{
					m_time = 0f;
					m_state = State.Wait3;
				}
				break;
			case State.Wait3:
				m_time += delta;
				if (m_time > WAIT_TIME3)
				{
					context.ChangeState(GetNextChangeState());
					m_state = State.Idle;
				}
				break;
			}
		}

		protected virtual EVENTBOSS_STATE_ID GetNextChangeState()
		{
			return EVENTBOSS_STATE_ID.AttackEvent1;
		}

		protected virtual bool IsFirst()
		{
			return true;
		}
	}
}
