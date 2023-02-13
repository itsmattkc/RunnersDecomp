using Message;
using UnityEngine;

namespace Boss
{
	public class BossStateAppearMapBase : FSMState<ObjBossEggmanState>
	{
		private enum State
		{
			Idle,
			Start,
			Wait1,
			Wait2,
			Event1,
			Event2,
			Event3
		}

		private static Vector3 APPEAR_ROT = new Vector3(0f, 90f, 0f);

		private State m_state;

		private float m_time;

		public override void Enter(ObjBossEggmanState context)
		{
			context.DebugDrawState("BossStateAppearMap");
			context.SetHitCheck(false);
			context.transform.rotation = Quaternion.Euler(APPEAR_ROT);
			context.OpenHpGauge();
			m_time = 0f;
			m_state = State.Start;
		}

		public override void Leave(ObjBossEggmanState context)
		{
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			switch (m_state)
			{
			case State.Start:
				context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Normal);
				m_state = State.Wait1;
				break;
			case State.Wait1:
			{
				float playerDistance = context.GetPlayerDistance();
				if (playerDistance < context.BossParam.DefaultPlayerDistance)
				{
					context.KeepSpeed();
					MsgTutorialMapBoss value = new MsgTutorialMapBoss();
					GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialMapBoss", value, SendMessageOptions.DontRequireReceiver);
					m_state = State.Wait2;
				}
				break;
			}
			case State.Wait2:
				SetMotion1(context);
				m_state = State.Event1;
				break;
			case State.Event1:
				m_time += delta;
				if (m_time > GetTime1())
				{
					context.RequestStartChaoAbility();
					SetMotion2(context);
					m_time = 0f;
					m_state = State.Event2;
				}
				break;
			case State.Event2:
				m_time += delta;
				if (m_time > GetTime2())
				{
					SetMotion3(context);
					m_state = State.Event3;
				}
				break;
			case State.Event3:
				m_time += delta;
				if (m_time > GetTime3())
				{
					context.BossParam.SetupBossTable();
					context.StartGauge();
					context.ChangeState(GetNextChangeState());
					m_state = State.Idle;
				}
				break;
			}
		}

		protected virtual float GetTime1()
		{
			return 0f;
		}

		protected virtual float GetTime2()
		{
			return 0f;
		}

		protected virtual float GetTime3()
		{
			return 0f;
		}

		protected virtual void SetMotion1(ObjBossEggmanState context)
		{
			context.BossMotion.SetMotion(BossMotion.APPEAR);
		}

		protected virtual void SetMotion2(ObjBossEggmanState context)
		{
			context.BossMotion.SetMotion(BossMotion.BOM_START);
			ObjUtil.PlaySE("boss_bomb_drop");
		}

		protected virtual void SetMotion3(ObjBossEggmanState context)
		{
		}

		protected virtual STATE_ID GetNextChangeState()
		{
			return STATE_ID.AttackMap1;
		}
	}
}
