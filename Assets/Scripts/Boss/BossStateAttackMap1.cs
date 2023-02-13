using UnityEngine;

namespace Boss
{
	public class BossStateAttackMap1 : BossStateAttackBase
	{
		private enum State
		{
			Idle,
			Start,
			Bom
		}

		private State m_state;

		private float m_attackInterspace;

		public override void Enter(ObjBossEggmanState context)
		{
			base.Enter(context);
			context.DebugDrawState("BossStateAttackMap1");
			m_state = State.Start;
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
				bool flag = true;
				if (context.BossParam.AttackBallFlag)
				{
					flag = false;
					context.BossParam.AttackBallFlag = false;
				}
				else
				{
					int randomRange = ObjUtil.GetRandomRange100();
					if (randomRange < context.BossParam.TrapRand && context.BossParam.AttackTrapCount < context.BossParam.AttackTrapCountMax)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					CreateBall(context, BossBallType.TRAP);
					context.BossParam.AttackTrapCount++;
				}
				else
				{
					CreateBall(context, BossBallType.ATTACK);
					context.BossParam.AttackBallFlag = true;
					context.BossParam.AttackTrapCount = 0;
				}
				ResetTime();
				m_attackInterspace = context.GetAttackInterspace();
				m_state = State.Bom;
				break;
			}
			case State.Bom:
				if (UpdateTime(delta, m_attackInterspace))
				{
					m_state = State.Start;
				}
				break;
			}
		}

		private void CreateBall(ObjBossEggmanState context, BossBallType type)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossBall");
			if (!(gameObject != null))
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, ObjBossUtil.GetBossHatchPos(context.gameObject), Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				ObjBossBall component = gameObject2.GetComponent<ObjBossBall>();
				if ((bool)component)
				{
					ObjBossBall.SetData setData = default(ObjBossBall.SetData);
					setData.obj = context.gameObject;
					setData.bound_param = context.GetBoundParam();
					setData.type = type;
					setData.shot_rot = context.GetShotRotation(context.BossParam.ShotRotBase);
					setData.shot_speed = context.BossParam.ShotSpeed;
					setData.attack_speed = context.BossParam.AttackSpeed;
					setData.firstSpeed = 0f;
					setData.outOfcontrol = 0f;
					setData.ballSpeed = context.BossParam.BallSpeed;
					setData.bossAppear = true;
					component.Setup(setData);
				}
			}
		}
	}
}
