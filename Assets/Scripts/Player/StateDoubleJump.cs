using Message;
using Tutorial;
using UnityEngine;

namespace Player
{
	public class StateDoubleJump : FSMState<CharacterState>
	{
		private float m_jumpForce;

		private float m_speed;

		private float m_addForceTmer;

		private float m_addAcc;

		private CharacterLoopEffect m_effect;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, true);
			m_jumpForce = context.Parameter.m_doubleJumpForce;
			m_addForceTmer = context.Parameter.m_doubleJumpAddSec;
			m_addAcc = context.Parameter.m_doubleJumpAddAcc;
			m_speed = Mathf.Max(context.Movement.GetForwardVelocityScalar(), 0f);
			context.GetAnimator().CrossFade("Jump", 0.05f);
			context.OnAttack(AttackPower.PlayerSpin, DefensePower.PlayerSpin);
			StateUtil.SetAttackAttributePowerIfPowerType(context, true);
			m_effect = context.GetSpinAttackEffect();
			if (m_effect != null)
			{
				m_effect.SetValid(true);
			}
			StartJump(context);
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.SPEED, ChaoAbility.MAGNET_SPEED_TYPE_JUMP, true);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY_AND_TRAP);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			if (m_effect != null)
			{
				m_effect.SetValid(false);
			}
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.SPEED, ChaoAbility.MAGNET_SPEED_TYPE_JUMP, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			Vector3 vertVelocity = context.Movement.VertVelocity;
			Vector3 vector = context.Movement.Velocity - vertVelocity;
			vector = context.Movement.GetForwardDir() * StateUtil.GetForwardSpeedAir(context, context.DefaultSpeed, deltaTime);
			vertVelocity += context.Movement.GetGravity() * deltaTime;
			if (m_addForceTmer >= 0f)
			{
				m_addForceTmer -= deltaTime;
				vertVelocity += context.Movement.GetUpDir() * m_addAcc * deltaTime;
			}
			context.Movement.Velocity = vector + vertVelocity;
			STATE_ID state = STATE_ID.Non;
			if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
			{
				context.ChangeState(state);
				return;
			}
			if (context.m_input.IsTouched())
			{
				STATE_ID id = STATE_ID.Non;
				if (StateUtil.GetNextStateToAirAttack(context, ref id, false))
				{
					switch (id)
					{
					case STATE_ID.Non:
						break;
					case STATE_ID.DoubleJump:
						StartJump(context);
						break;
					default:
						ObjUtil.SendMessageTutorialClear(EventID.DOUBLE_JUMP);
						context.ChangeState(id);
						break;
					}
					return;
				}
			}
			if (context.Movement.GetVertVelocityScalar() <= 0f && context.Movement.IsOnGround() && !StateUtil.ChangeToJumpStateIfPrecedeInputTouch(context, 0.1f, true))
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

		private void StartJump(CharacterState context)
		{
			context.Movement.Velocity = context.transform.forward * m_speed + m_jumpForce * Vector3.up;
			StateUtil.Create2ndJumpEffect(context);
			context.AddAirAction();
		}
	}
}
