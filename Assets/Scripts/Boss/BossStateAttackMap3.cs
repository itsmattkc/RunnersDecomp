using UnityEngine;

namespace Boss
{
	public class BossStateAttackMap3 : BossStateAttackBase
	{
		private enum State
		{
			Idle,
			Start,
			Move,
			CreateBom,
			CreateBomNext,
			Bom
		}

		private const float MOVE_SPEED1 = 7f;

		private const float MOVE_SPEED2 = 30f;

		private const float MOVE_SPEED3 = 15f;

		private const float MOVE_TIME = 0.2f;

		private static Map3AttackData ATTACK_DATA_NONE = new Map3AttackData();

		private static readonly float[] BOSS_POSY = new float[4]
		{
			2f,
			2f,
			3f,
			4f
		};

		private static readonly float[] BOM_SPEED = new float[11]
		{
			7f,
			7f,
			7f,
			30f,
			30f,
			30f,
			30f,
			15f,
			15f,
			15f,
			15f
		};

		private State m_state;

		private int m_attackCount;

		private Map3AttackData m_attackData = ATTACK_DATA_NONE;

		private float m_attackInterspace;

		public override void Enter(ObjBossEggmanState context)
		{
			base.Enter(context);
			context.DebugDrawState("BossStateAttackMap3");
			m_state = State.Start;
			m_attackCount = 0;
			m_attackData = ATTACK_DATA_NONE;
			m_attackInterspace = 0f;
		}

		public override void Leave(ObjBossEggmanState context)
		{
			base.Leave(context);
		}

		public override void Step(ObjBossEggmanState context, float delta)
		{
			switch (m_state)
			{
			case State.Start:
				if (context.IsPlayerDead())
				{
					break;
				}
				if (context.IsBossDistanceEnd())
				{
					context.ChangeState(STATE_ID.PassMapDistanceEnd);
				}
				else if (m_attackData.m_type == BossAttackType.NONE)
				{
					Map3AttackData map3AttackData = context.BossParam.GetMap3AttackData();
					if (map3AttackData != null)
					{
						m_attackData = map3AttackData;
						m_attackCount = 0;
						int num = Mathf.Min((int)m_attackData.m_posA, BOSS_POSY.Length);
						int num2 = Mathf.Min((int)m_attackData.m_type, BOM_SPEED.Length);
						float speed = BOM_SPEED[num2];
						SetMove(context, 1f, speed, BOSS_POSY[num]);
						ResetTime();
						m_state = State.Move;
					}
				}
				else
				{
					int num3 = Mathf.Min((int)m_attackData.m_posB, BOSS_POSY.Length);
					int num4 = Mathf.Min((int)m_attackData.m_type, BOM_SPEED.Length);
					float speed2 = BOM_SPEED[num4];
					SetMove(context, 1f, speed2, BOSS_POSY[num3]);
					ResetTime();
					m_state = State.Move;
				}
				break;
			case State.Move:
			{
				UpdateMove(context, delta);
				bool flag = UpdateTime(delta, 0.2f);
				if (IsMoveStepEquals(context, 0f) && flag)
				{
					m_state = State.CreateBom;
				}
				break;
			}
			case State.CreateBom:
				if (context.IsPlayerDead())
				{
					break;
				}
				if (context.IsBossDistanceEnd())
				{
					context.ChangeState(STATE_ID.PassMapDistanceEnd);
					break;
				}
				if (m_attackCount == 0)
				{
					context.CreateTrapBall(context.BossParam.GetMap3BomTblA(m_attackData.m_type), 0f, m_attackData.m_randA, true);
				}
				else
				{
					context.CreateTrapBall(context.BossParam.GetMap3BomTblB(m_attackData.m_type), 0f, m_attackData.m_randB, true);
				}
				m_attackCount++;
				if (m_attackCount >= m_attackData.GetAttackCount())
				{
					m_attackInterspace = context.GetAttackInterspace();
					m_attackData = ATTACK_DATA_NONE;
					m_state = State.Bom;
				}
				else
				{
					m_state = State.CreateBomNext;
				}
				ResetTime();
				break;
			case State.CreateBomNext:
				if (UpdateTime(delta, 0.2f))
				{
					ResetTime();
					m_state = State.Start;
				}
				break;
			case State.Bom:
				if (UpdateTime(delta, m_attackInterspace))
				{
					ResetTime();
					m_state = State.Start;
				}
				break;
			}
		}
	}
}
