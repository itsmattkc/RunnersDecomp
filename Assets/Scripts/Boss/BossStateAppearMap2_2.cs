using UnityEngine;

namespace Boss
{
	public class BossStateAppearMap2_2 : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Move,
			Wait
		}

		private static Vector3 APPEAR_ROT = new Vector3(0f, 90f, 0f);

		private static float APPEAR_OFFSET_POS_Y = 7f;

		private static float MOVE_SPEED = 2.5f;

		private static float WAIT_TIME = 0.5f;

		private State m_state;

		private float m_time;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStateAppearMap2_2");
			context.SetHitCheck(false);
			context.transform.rotation = Quaternion.Euler(APPEAR_ROT);
			Transform transform = context.transform;
			Vector3 playerPosition = context.GetPlayerPosition();
			float x = playerPosition.x + context.BossParam.DefaultPlayerDistance;
			Vector3 startPos = context.BossParam.StartPos;
			transform.position = new Vector3(x, startPos.y + APPEAR_OFFSET_POS_Y, 0f);
			context.SetupMoveY(1f);
			context.KeepSpeed();
			context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
			m_time = 0f;
			m_state = State.Move;
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			switch (m_state)
			{
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
					context.BossMotion.SetMotion(BossMotion.MISSILE_START);
					m_state = State.Wait;
				}
				break;
			}
			case State.Wait:
				m_time += delta;
				if (m_time > WAIT_TIME)
				{
					context.ChangeState(STATE_ID.AttackMap2);
					m_state = State.Idle;
				}
				break;
			}
		}
	}
}
