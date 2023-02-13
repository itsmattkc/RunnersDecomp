using Message;
using Tutorial;
using UnityEngine;

namespace Player
{
	public class StateThirdJump : FSMState<CharacterState>
	{
		private float m_jumpForce;

		private float m_speed;

		private float m_addForceTmer;

		private float m_addAcc;

		private CharacterLoopEffect m_effect;

		private bool m_specialJumpMagnet;

		public override void Enter(CharacterState context)
		{
			Vector3 horzVelocity = context.Movement.HorzVelocity;
			bool flag = false;
			JumpParameter enteringParameter = context.GetEnteringParameter<JumpParameter>();
			if (enteringParameter != null)
			{
				flag = enteringParameter.m_onAir;
			}
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, true);
			m_jumpForce = context.Parameter.m_jumpForce;
			m_addForceTmer = context.Parameter.m_jumpAddSec;
			m_addAcc = context.Parameter.m_jumpAddAcc;
			m_speed = horzVelocity.magnitude;
			context.Movement.Velocity = horzVelocity + m_jumpForce * Vector3.up;
			context.GetAnimator().CrossFade("ThirdJump", 0.05f);
			context.AddAirAction();
			context.OnAttack(AttackPower.PlayerSpin, DefensePower.PlayerSpin);
			StateUtil.SetAttackAttributePowerIfPowerType(context, true);
			if (m_effect != null)
			{
				m_effect.SetValid(true);
			}
			if (flag)
			{
				StateUtil.Create2ndJumpEffect(context);
			}
			else
			{
				StateUtil.CreateJumpEffect(context);
				CharaSEUtil.PlayJumpSE(context.charaType);
			}
			m_specialJumpMagnet = false;
			if (context.NumAirAction > 2)
			{
				StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.POWER, ChaoAbility.MAGNET_POWER_TYPE_JUMP, true);
				StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY_AND_TRAP);
				StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY);
				m_specialJumpMagnet = true;
			}
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			if (m_effect != null)
			{
				m_effect.SetValid(false);
			}
			ObjUtil.SendMessageTutorialClear(EventID.JUMP);
			if (m_specialJumpMagnet)
			{
				StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.POWER, ChaoAbility.MAGNET_POWER_TYPE_JUMP, false);
			}
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			Vector3 vertVelocity = context.Movement.VertVelocity;
			Vector3 vector = context.Movement.Velocity - vertVelocity;
			vertVelocity += context.Movement.GetGravity() * deltaTime;
			if (m_addForceTmer >= 0f)
			{
				m_addForceTmer -= deltaTime;
				vertVelocity += context.Movement.GetUpDir() * m_addAcc * deltaTime;
			}
			float magnitude = vector.magnitude;
			if (magnitude < m_speed && magnitude < context.DefaultSpeed)
			{
				m_speed = context.DefaultSpeed;
			}
			float targetSpeed = Mathf.Max(context.DefaultSpeed, m_speed);
			vector = context.Movement.GetForwardDir() * StateUtil.GetForwardSpeedAir(context, targetSpeed, deltaTime);
			context.Movement.Velocity = vector + vertVelocity;
			STATE_ID state = STATE_ID.Non;
			if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
			{
				context.ChangeState(state);
			}
			else if ((!context.m_input.IsTouched() || !StateUtil.CheckAndChangeStateToAirAttack(context, false, false)) && context.Movement.GetVertVelocityScalar() <= 0f && context.Movement.IsOnGround() && !StateUtil.ChangeToJumpStateIfPrecedeInputTouch(context, 0.1f, true))
			{
				StateUtil.NowLanding(context, true);
				context.ChangeState(STATE_ID.Run);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (StateUtil.ChangeAfterSpinattack(context, messageId, msg))
			{
				return true;
			}
			return false;
		}
	}
}
