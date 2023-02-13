using UnityEngine;

namespace Boss
{
	public class BossStateAttackEventBase : FSMState<ObjBossEventBossState>
	{
		public class TimeParam
		{
			public float m_time;

			public float m_timeMax;

			public TimeParam()
			{
				ResetParam(0f, 0f);
			}

			public void ResetParam(float time, float timeMax)
			{
				m_time = time;
				m_timeMax = timeMax;
			}
		}

		private const float PASS_DISTANCE = 6f;

		private const float PASS_DISTANCE2 = 2f;

		private const float PASS_WARP_DISTANCE = 26f;

		private const float MISSILE_POSX = 17f;

		private const float BUMPER_POSX = 15f;

		private const float SPEEDUP_DISTANCE = 10f;

		private static readonly float[] MISSILE_POSY = new float[3]
		{
			1f,
			2f,
			3f
		};

		private static readonly float[] TRAPBLL_POSY = new float[4]
		{
			1f,
			1f,
			2f,
			3f
		};

		private float m_time;

		private TimeParam m_bumperTime = new TimeParam();

		private float m_speed_up;

		private float m_speed_down;

		private TimeParam m_missileTime = new TimeParam();

		private TimeParam m_missileWaitTime = new TimeParam();

		private int m_missileCount;

		private TimeParam m_trapBallTime = new TimeParam();

		private TimeParam m_trapBallWaitTime = new TimeParam();

		private int m_trapBallCount;

		private Map3AttackData m_trapBallData;

		public override void Enter(ObjBossEventBossState context)
		{
			context.SetHitCheck(true);
			context.BossMotion.SetMotion(EventBossMotion.ATTACK);
			m_time = 0f;
			m_bumperTime.ResetParam(0f, context.BossParam.BumperInterspace * 0.1f);
			m_speed_up = 0f;
			m_speed_down = 0f;
			m_missileTime.ResetParam(0f, context.BossParam.MissileInterspace * 0.1f);
			m_missileWaitTime.ResetParam(0f, context.BossParam.MissileWaitTime * 0.1f);
			m_missileCount = 0;
			m_trapBallTime.ResetParam(0f, context.BossParam.AttackInterspaceMin * 0.1f);
			m_trapBallWaitTime.ResetParam(0f, context.BossParam.AttackInterspaceMax * 0.1f);
			m_trapBallCount = 0;
			m_trapBallData = null;
		}

		public override void Leave(ObjBossEventBossState context)
		{
			context.SetHitCheck(false);
		}

		protected bool UpdateBoost(ObjBossEventBossState context, float delta)
		{
			float playerBossPositionX = context.GetPlayerBossPositionX();
			if (playerBossPositionX < 2f)
			{
				context.SetSpeed(GetBoostSpeed(context, WispBoostLevel.LEVEL3));
			}
			else
			{
				context.SetSpeed(GetBoostSpeed(context, context.BossParam.BoostLevel));
			}
			if (playerBossPositionX < 0f && Mathf.Abs(playerBossPositionX) > 6f)
			{
				Vector3 position = context.transform.position;
				float x = position.x + 26f;
				Vector3 startPos = context.BossParam.StartPos;
				float y = startPos.y;
				Vector3 position2 = context.transform.position;
				Vector3 position3 = new Vector3(x, y, position2.z);
				context.transform.position = position3;
				return true;
			}
			return false;
		}

		protected bool UpdateBumper(ObjBossEventBossState context, float delta)
		{
			m_bumperTime.m_time += delta;
			if (m_bumperTime.m_time > m_bumperTime.m_timeMax)
			{
				m_bumperTime.m_time = 0f;
				m_bumperTime.m_timeMax = context.BossParam.BumperInterspace;
				int randomRange = ObjUtil.GetRandomRange100();
				if (randomRange < context.BossParam.BumperRand)
				{
					context.CreateBumper(false, 15f);
				}
			}
			if (context.IsHitBumper())
			{
				float playerBossPositionX = context.GetPlayerBossPositionX();
				if (playerBossPositionX < 10f)
				{
					m_speed_up = context.BossParam.BumperSpeedup;
					context.SetSpeed(m_speed_up * 0.1f);
					m_speed_down = 0f;
					return true;
				}
			}
			return false;
		}

		protected bool UpdateBumperSpeedup(ObjBossEventBossState context, float delta)
		{
			context.UpdateSpeedDown(delta, m_speed_down);
			float num = GetBoostSpeed(context, context.BossParam.BoostLevel) * 0.7f;
			if (context.BossParam.Speed < num)
			{
				context.SetSpeed(num);
				return true;
			}
			m_speed_down += delta * m_speed_up * 1.2f;
			return false;
		}

		protected void UpdateMissile(ObjBossEventBossState context, float delta)
		{
			if (context.BossParam.MissileCount <= 0)
			{
				return;
			}
			m_missileWaitTime.m_time += delta;
			if (!(m_missileWaitTime.m_time > m_missileWaitTime.m_timeMax))
			{
				return;
			}
			m_missileTime.m_time += delta;
			if (m_missileTime.m_time > m_missileTime.m_timeMax)
			{
				m_missileTime.m_time = 0f;
				m_missileTime.m_timeMax = context.BossParam.MissileInterspace;
				int num = 2;
				int randomRange = ObjUtil.GetRandomRange100();
				int missilePos = context.BossParam.MissilePos1;
				int missilePos2 = context.BossParam.MissilePos2;
				if (randomRange < missilePos)
				{
					num = 0;
				}
				else if (randomRange < missilePos + missilePos2)
				{
					num = 1;
				}
				float y = MISSILE_POSY[num];
				Vector3 playerPosition = context.GetPlayerPosition();
				float x = playerPosition.x + 17f;
				Vector3 position = context.transform.position;
				Vector3 pos = new Vector3(x, y, position.z);
				context.CreateMissile(pos);
				m_missileCount++;
				if (m_missileCount >= context.BossParam.MissileCount)
				{
					m_missileCount = 0;
					m_missileTime.m_time = 0f;
					m_missileTime.m_timeMax = context.BossParam.MissileWaitTime;
				}
			}
		}

		protected void UpdateTrapBall(ObjBossEventBossState context, float delta)
		{
			m_trapBallWaitTime.m_time += delta;
			if (!(m_trapBallWaitTime.m_time > m_trapBallWaitTime.m_timeMax))
			{
				return;
			}
			m_trapBallTime.m_time += delta;
			if (!(m_trapBallTime.m_time > m_trapBallTime.m_timeMax))
			{
				return;
			}
			m_trapBallTime.m_time = 0f;
			m_trapBallTime.m_timeMax = context.BossParam.AttackInterspaceMin;
			if (m_trapBallData == null)
			{
				m_trapBallData = context.BossParam.GetMap3AttackData();
			}
			Map3AttackData trapBallData = m_trapBallData;
			if (trapBallData != null)
			{
				context.BossParam.AttackBallFlag = true;
				if (m_trapBallCount == 0)
				{
					context.CreateTrapBall(context.BossParam.GetMap3BomTblA(trapBallData.m_type), TRAPBLL_POSY[(int)trapBallData.m_posA], 0, false);
				}
				else
				{
					context.CreateTrapBall(context.BossParam.GetMap3BomTblB(trapBallData.m_type), TRAPBLL_POSY[(int)trapBallData.m_posB], 0, false);
				}
				m_trapBallCount++;
				if (m_trapBallCount >= trapBallData.GetAttackCount())
				{
					m_trapBallData = null;
					m_trapBallCount = 0;
					m_trapBallWaitTime.m_time = 0f;
					m_trapBallWaitTime.m_timeMax = context.BossParam.AttackInterspaceMax;
				}
			}
		}

		private float GetBoostSpeed(ObjBossEventBossState context, WispBoostLevel level)
		{
			if (level == WispBoostLevel.NONE)
			{
				return context.BossParam.PlayerSpeed;
			}
			float boostSpeedParam = context.BossParam.GetBoostSpeedParam(level);
			return context.BossParam.PlayerSpeed - boostSpeedParam;
		}

		protected bool UpdateTime(float delta, float time_max)
		{
			m_time += delta;
			if (m_time > time_max)
			{
				return true;
			}
			return false;
		}

		protected void ResetTime()
		{
			m_time = 0f;
		}
	}
}
