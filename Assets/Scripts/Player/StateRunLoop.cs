using Message;
using Tutorial;
using UnityEngine;

namespace Player
{
	public class StateRunLoop : FSMState<CharacterState>
	{
		private const float MaxSpeed = 20f;

		private float m_speed;

		private bool m_onBoost;

		private bool m_onBoostEx;

		private CharacterLoopEffect m_loopEffect;

		private CharacterLoopEffect m_exLoopEffect;

		private GameObject m_effectParaloop;

		private float m_effectTime;

		public override void Enter(CharacterState context)
		{
			if (context.TestStatus(Status.NowLanding))
			{
				context.GetAnimator().CrossFade("Landing", 0.05f);
			}
			else
			{
				context.GetAnimator().CrossFade("Run", 0.1f);
			}
			RunLoopPathParameter enteringParameter = context.GetEnteringParameter<RunLoopPathParameter>();
			if (enteringParameter != null && enteringParameter.m_pathComponent != null)
			{
				float? distance = null;
				context.ChangeMovement(MOVESTATE_ID.RunOnPath);
				CharacterMoveOnPath currentState = context.Movement.GetCurrentState<CharacterMoveOnPath>();
				if (currentState != null)
				{
					currentState.SetupPath(context.Position, enteringParameter.m_pathComponent, distance);
				}
			}
			m_speed = context.Movement.HorzVelocity.magnitude;
			m_loopEffect = GameObjectUtil.FindChildGameObjectComponent<CharacterLoopEffect>(context.gameObject, "CharacterBoost");
			m_exLoopEffect = GameObjectUtil.FindChildGameObjectComponent<CharacterLoopEffect>(context.gameObject, "CharacterBoostEx");
			m_effectTime = 0f;
			float animationSpeed = 0f;
			m_onBoost = StateUtil.SetRunningAnimationSpeed(context, ref animationSpeed);
			if (m_onBoost)
			{
				CreateParaLoop(context);
			}
			context.ClearAirAction();
			context.SetNotCharaChange(true);
			context.SetNotUseItem(true);
			if (context.GetCamera() != null)
			{
				MsgPushCamera value = new MsgPushCamera(CameraType.LOOP_TERRAIN, 0.5f);
				context.GetCamera().SendMessage("OnPushCamera", value);
			}
			ObjUtil.PauseCombo(MsgPauseComboTimer.State.PAUSE);
			ObjUtil.SetQuickModeTimePause(true);
			ObjUtil.SetDisableEquipItem(true);
		}

		public override void Leave(CharacterState context)
		{
			m_onBoost = false;
			StateUtil.SetOnBoost(context, m_loopEffect, false);
			DestroyParaloop(context);
			context.SetNotCharaChange(false);
			context.SetNotUseItem(false);
			if (context.GetCamera() != null)
			{
				MsgPopCamera value = new MsgPopCamera(CameraType.LOOP_TERRAIN, 2.5f);
				context.GetCamera().SendMessage("OnPopCamera", value);
			}
			ObjUtil.PauseCombo(MsgPauseComboTimer.State.PLAY);
			m_loopEffect = null;
			if (m_exLoopEffect != null)
			{
				StateUtil.SetOnBoostEx(context, m_loopEffect, false);
				m_exLoopEffect = null;
			}
			ObjUtil.SetQuickModeTimePause(false);
			ObjUtil.SetDisableEquipItem(false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			CharacterMoveOnPath currentState = context.Movement.GetCurrentState<CharacterMoveOnPath>();
			if (currentState != null && currentState.IsPathEnd(0.1f))
			{
				context.ChangeState(STATE_ID.Run);
				return;
			}
			m_speed = GetRunningSpeed(context, deltaTime);
			context.Movement.Velocity = context.Movement.GetForwardDir() * m_speed;
			float animationSpeed = 0f;
			bool onBoost = m_onBoost;
			m_onBoost = StateUtil.SetRunningAnimationSpeed(context, ref animationSpeed);
			CheckOnBoost(context, onBoost);
			CheckOnBoostEx(context, animationSpeed);
			if (!m_onBoost)
			{
				StateUtil.CheckAndCreateRunEffect(context, ref m_effectTime, m_speed, animationSpeed, deltaTime);
			}
		}

		private float GetRunningSpeed(CharacterState context, float deltaTime)
		{
			float speed = m_speed;
			float num = 20f;
			float minLoopRunSpeed = context.Parameter.m_minLoopRunSpeed;
			float num2 = Mathf.Max(minLoopRunSpeed, context.DefaultSpeed);
			Vector3 forwardDir = context.Movement.GetForwardDir();
			bool flag = Vector3.Dot(forwardDir, context.Movement.GetGravityDir()) > 0.1736f;
			float runLoopAccel = context.Parameter.m_runLoopAccel;
			if (flag)
			{
				num2 = num;
			}
			if (speed > num2)
			{
				m_speed = Mathf.Max(speed - context.Parameter.m_runDec * deltaTime, num2);
			}
			else if (speed < minLoopRunSpeed)
			{
				m_speed = minLoopRunSpeed;
			}
			else
			{
				m_speed = Mathf.Min(speed + runLoopAccel * deltaTime, num2);
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

		private void CreateParaLoop(CharacterState context)
		{
			m_effectParaloop = StateUtil.CreateEffect(context, "ef_pl_paraloop01", false);
			if ((bool)m_effectParaloop)
			{
				StateUtil.SetObjectLocalPositionToCenter(context, m_effectParaloop);
			}
			SoundManager.SePlay("act_paraloop");
			context.SetStatus(Status.Paraloop, true);
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.LOOP_COMBO_UP, false);
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.LOOP_MAGNET, false);
			ObjUtil.SendMessageTutorialClear(EventID.PARA_LOOP);
		}

		private void DestroyParaloop(CharacterState context)
		{
			if (m_effectParaloop != null)
			{
				StateUtil.DestroyParticle(m_effectParaloop, 1f);
				m_effectParaloop = null;
			}
			context.SetStatus(Status.Paraloop, false);
		}
	}
}
