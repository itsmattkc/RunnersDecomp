using Message;
using UnityEngine;

namespace Player
{
	public class StateRun : FSMState<CharacterState>
	{
		private float m_speed;

		private float m_effectTime;

		private bool m_onBoost;

		private bool m_onBoostEx;

		private CharacterLoopEffect m_loopEffect;

		private CharacterLoopEffect m_exLoopEffect;

		private WispBoostLevel m_bossBoostLevel;

		public override void Enter(CharacterState context)
		{
			if (context.TestStatus(Status.NowLanding))
			{
				context.GetAnimator().CrossFade("Landing", 0.1f);
			}
			else
			{
				context.GetAnimator().CrossFade("Run", 0.1f);
			}
			context.ChangeMovement(MOVESTATE_ID.Run);
			m_speed = context.Movement.GetForwardVelocityScalar();
			m_loopEffect = GameObjectUtil.FindChildGameObjectComponent<CharacterLoopEffect>(context.gameObject, "CharacterBoost");
			m_exLoopEffect = GameObjectUtil.FindChildGameObjectComponent<CharacterLoopEffect>(context.gameObject, "CharacterBoostEx");
			context.ClearAirAction();
			m_effectTime = 0f;
			m_bossBoostLevel = context.BossBoostLevel;
		}

		public override void Leave(CharacterState context)
		{
			m_onBoost = false;
			m_onBoostEx = false;
			StateUtil.SetOnBoost(context, m_loopEffect, false);
			m_loopEffect = null;
			if (m_exLoopEffect != null)
			{
				StateUtil.SetOnBoostEx(context, m_exLoopEffect, false);
				m_exLoopEffect = null;
			}
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			bool flag = context.Movement.IsOnGround();
			Vector3 vertVelocity = context.Movement.VertVelocity;
			m_speed = GetRunningSpeed(context, deltaTime);
			context.Movement.HorzVelocity = context.Movement.GetForwardDir() * GetRunningSpeed(context, deltaTime);
			HitInfo info;
			if (flag && context.Movement.GetGroundInfo(out info))
			{
				StateUtil.SetRotateOnGround(context);
				vertVelocity += context.Movement.GetGravity() * deltaTime;
				context.Movement.VertVelocity = vertVelocity;
			}
			if (context.m_input.IsTouched())
			{
				context.ChangeState(STATE_ID.Jump);
				return;
			}
			if (!flag)
			{
				context.ChangeState(STATE_ID.Fall);
				return;
			}
			STATE_ID state = STATE_ID.Non;
			if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
			{
				context.ChangeState(state);
				return;
			}
			bool onBoost = m_onBoost;
			float animationSpeed = 0f;
			m_onBoost = StateUtil.SetRunningAnimationSpeed(context, ref animationSpeed);
			CheckOnBoost(context, onBoost);
			CheckOnBoostEx(context, animationSpeed);
			if (context.BossBoostLevel != m_bossBoostLevel)
			{
				m_effectTime = 0f;
				m_bossBoostLevel = context.BossBoostLevel;
			}
			if (!m_onBoost || context.BossBoostLevel != WispBoostLevel.NONE)
			{
				StateUtil.CheckAndCreateRunEffect(context, ref m_effectTime, m_speed, animationSpeed, deltaTime);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			MsgRunLoopPath msgRunLoopPath = msg as MsgRunLoopPath;
			if (msgRunLoopPath != null)
			{
				RunLoopPathParameter runLoopPathParameter = context.CreateEnteringParameter<RunLoopPathParameter>();
				runLoopPathParameter.Set(msgRunLoopPath.m_component);
				context.ChangeState(STATE_ID.RunLoop);
			}
			return true;
		}

		private float GetRunningSpeed(CharacterState context, float deltaTime)
		{
			float speed = m_speed;
			float maxSpeed = context.Parameter.m_maxSpeed;
			float num = context.DefaultSpeed;
			float defaultSpeed = context.DefaultSpeed;
			Vector3 forwardDir = context.Movement.GetForwardDir();
			bool flag = Vector3.Dot(forwardDir, context.Movement.GetGravityDir()) > 0.1736f;
			float runAccel = context.Parameter.m_runAccel;
			if (flag)
			{
				num = maxSpeed;
			}
			if (speed > num)
			{
				m_speed = Mathf.Max(speed - context.Parameter.m_runDec * deltaTime, num);
			}
			else if (speed < defaultSpeed)
			{
				m_speed = defaultSpeed;
			}
			else
			{
				m_speed = Mathf.Min(speed + runAccel * deltaTime, num);
			}
			return m_speed;
		}

		private void CheckOnBoost(CharacterState context, bool oldBoost)
		{
			if (!oldBoost && m_onBoost)
			{
				StateUtil.SetOnBoost(context, m_loopEffect, true);
			}
			else if (oldBoost && !m_onBoost)
			{
				StateUtil.SetOnBoost(context, m_loopEffect, false);
			}
		}

		private void CheckOnBoostEx(CharacterState context, float speed)
		{
			if (m_exLoopEffect != null)
			{
				bool flag = 0.6f < speed && speed < 0.9f;
				if (flag && !m_onBoostEx)
				{
					StateUtil.SetOnBoostEx(context, m_exLoopEffect, true);
					m_onBoostEx = true;
				}
				else if (!flag && m_onBoostEx)
				{
					StateUtil.SetOnBoostEx(context, m_exLoopEffect, false);
					m_onBoostEx = false;
				}
			}
		}
	}
}
