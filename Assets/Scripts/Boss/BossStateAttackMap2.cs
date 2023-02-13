using UnityEngine;

namespace Boss
{
	public class BossStateAttackMap2 : BossStateAttackBase
	{
		private enum State
		{
			Idle,
			Start,
			Bumper,
			Missile,
			BossAttackReady,
			BossAttack
		}

		private const float MOVE_SPEED = 14f;

		private const float ATTACK_READY = 1f;

		private const float ATTACK_POSY = 1.5f;

		private const float PASS_DISTANCE = 6f;

		private const float PASS_WARP_DISTANCE = 26f;

		private const float MISSILE_POSX = 10f;

		private static readonly float[] MISSILE_POSY = new float[5]
		{
			1f,
			1f,
			2f,
			2f,
			3f
		};

		private static readonly float[] BOSS_POSY = new float[5]
		{
			2.5f,
			2.5f,
			3.5f,
			3.5f,
			1.5f
		};

		private State m_state;

		private float m_missile_time;

		private float m_boss_time;

		private float m_attackInterspace;

		public override void Enter(ObjBossEggmanState context)
		{
			base.Enter(context);
			context.DebugDrawState("BossStateAttackMap2");
			context.BossMotion.SetMotion(BossMotion.MISSILE_START);
			m_state = State.Start;
			m_missile_time = 0f;
			m_boss_time = 0f;
			m_attackInterspace = context.GetAttackInterspace();
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
			{
				if (context.IsPlayerDead())
				{
					break;
				}
				if (context.IsBossDistanceEnd())
				{
					context.ChangeState(STATE_ID.PassMapDistanceEnd);
					break;
				}
				int randomRange = ObjUtil.GetRandomRange100();
				if (randomRange < context.BossParam.BumperRand)
				{
					context.CreateBumper(true);
				}
				m_missile_time = 0f;
				m_state = State.Bumper;
				break;
			}
			case State.Bumper:
				m_missile_time += delta;
				if (!(m_missile_time > context.BossParam.MissileInterspace * 0.5f))
				{
					break;
				}
				if (!context.IsPlayerDead() && !context.IsBossDistanceEnd())
				{
					int num = Random.Range(0, MISSILE_POSY.Length);
					float num2 = MISSILE_POSY[num];
					Vector3 position4 = context.transform.position;
					float x2 = position4.x + 10f;
					Vector3 position5 = context.transform.position;
					Vector3 pos = new Vector3(x2, num2, position5.z);
					context.CreateMissile(pos);
					if (num < BOSS_POSY.Length)
					{
						Vector3 position6 = context.transform.position;
						if (Mathf.Abs(num2 - position6.y) < 2f)
						{
							SetMove(context, 1f, 14f, BOSS_POSY[num]);
						}
						else
						{
							Vector3 position7 = context.transform.position;
							SetMove(context, 0f, 0f, position7.y);
						}
					}
				}
				m_state = State.Missile;
				break;
			case State.Missile:
				UpdateMove(context, delta);
				m_missile_time += delta;
				if (m_missile_time > context.BossParam.MissileInterspace)
				{
					m_state = State.Start;
				}
				break;
			case State.BossAttackReady:
				UpdateMove(context, delta);
				m_boss_time += delta;
				if (m_boss_time > 1f)
				{
					context.BossEffect.PlayBoostEffect(ObjBossEggmanEffect.BoostType.Attack);
					ObjUtil.PlaySE("boss_eggmobile_dash");
					m_boss_time = 0f;
					m_state = State.BossAttack;
				}
				break;
			case State.BossAttack:
			{
				if (Random.Range(0, 2) == 0)
				{
					context.SetSpeed(0f - context.BossParam.AttackSpeedMin);
				}
				else
				{
					context.SetSpeed(0f - context.BossParam.AttackSpeedMax);
				}
				context.BossParam.MinSpeed = context.BossParam.Speed;
				float playerBossPositionX = context.GetPlayerBossPositionX();
				if (playerBossPositionX < 0f && Mathf.Abs(playerBossPositionX) > 6f)
				{
					context.SetSpeed(0f);
					context.BossParam.MinSpeed = context.BossParam.Speed;
					Vector3 position = context.transform.position;
					float x = position.x + 26f;
					Vector3 startPos = context.BossParam.StartPos;
					float y = startPos.y;
					Vector3 position2 = context.transform.position;
					Vector3 position3 = new Vector3(x, y, position2.z);
					context.transform.position = position3;
					context.ChangeState(STATE_ID.AppearMap2_2);
				}
				break;
			}
			}
			if (context.IsPlayerDead() || m_state == State.BossAttack || m_state == State.BossAttackReady)
			{
				return;
			}
			m_boss_time += delta;
			if (m_boss_time > m_attackInterspace)
			{
				if (context.IsBossDistanceEnd())
				{
					context.ChangeState(STATE_ID.PassMapDistanceEnd);
					return;
				}
				m_boss_time = 0f;
				SetMove(context, 1f, 14f, 1.5f);
				context.BossMotion.SetMotion(BossMotion.ATTACK);
				m_state = State.BossAttackReady;
			}
		}
	}
}
