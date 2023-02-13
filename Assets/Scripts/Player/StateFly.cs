using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class StateFly : FSMState<CharacterState>
	{
		private enum Flag
		{
			CANNOTFLY,
			NOWUP,
			HOLD
		}

		private const float ChaoAbilityExtendTimeRate = 2f;

		private float m_canFlyTime;

		private Bitset32 m_flag;

		private GameObject m_effect;

		private string m_effectName;

		private bool CannotFly
		{
			get
			{
				return m_flag.Test(0);
			}
			set
			{
				m_flag.Set(0, value);
			}
		}

		private bool NowUp
		{
			get
			{
				return m_flag.Test(1);
			}
			set
			{
				m_flag.Set(1, value);
			}
		}

		private bool Hold
		{
			get
			{
				return m_flag.Test(2);
			}
			set
			{
				m_flag.Set(2, value);
			}
		}

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			context.GetAnimator().CrossFade("SecondJump", 0.1f);
			CharaSEUtil.PlayFlySE(context.charaType);
			m_effectName = "ef_pl_" + context.CharacterName.ToLower() + "_fly01";
			m_flag.Reset();
			NowUp = true;
			Hold = true;
			m_canFlyTime = context.Parameter.m_canFlyTime;
			context.Movement.VertVelocity = -context.Movement.GetGravityDir() * context.Parameter.m_flyUpFirstSpeed;
			StateUtil.SetAirMovementToRotateGround(context, true);
			context.OnAttack(AttackPower.PlayerSpin, DefensePower.PlayerSpin);
			CreateEffect(context);
			context.AddAirAction();
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.FLY, ChaoAbility.MAGNET_FLY_TYPE_JUMP, true);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY_AND_TRAP);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			DeleteEffect();
			context.SetStatus(Status.InvincibleByChao, false);
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.FLY, ChaoAbility.MAGNET_FLY_TYPE_JUMP, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			Vector3 a = context.Movement.GetForwardDir() * StateUtil.GetForwardSpeedAir(context, context.DefaultSpeed * context.Parameter.m_flySpeedRate, deltaTime);
			Vector3 b = Vector3.zero;
			float limitHeitht = context.Parameter.m_limitHeitht;
			Vector3 pos = context.Position;
			StateUtil.GetBaseGroundPosition(context, ref pos);
			if (context.m_input.IsHold() && !CannotFly)
			{
				float num = Vector3.Magnitude(context.Position - pos);
				if (num < limitHeitht)
				{
					float num2 = 0f;
					if (!NowUp)
					{
						num2 = context.Parameter.m_flyUpFirstSpeed;
						m_canFlyTime -= context.Parameter.m_flyDecSec2ndPress;
						if (!Hold)
						{
							CharaSEUtil.PlayFlySE(context.charaType);
						}
					}
					else
					{
						float vertVelocityScalar = context.Movement.GetVertVelocityScalar();
						num2 = Mathf.Min(vertVelocityScalar + context.Parameter.m_flyUpForce * deltaTime, context.Parameter.m_flyUpSpeedMax);
					}
					b = -context.Movement.GetGravityDir() * num2;
				}
				NowUp = true;
				Hold = true;
				if (m_effect == null)
				{
					CreateEffect(context);
				}
				m_canFlyTime -= deltaTime;
				if (m_canFlyTime < 0f)
				{
					CannotFly = true;
				}
			}
			else
			{
				NowUp = false;
				Hold = false;
				float vertVelocityScalar2 = context.Movement.GetVertVelocityScalar();
				b = ((!(vertVelocityScalar2 < 0f - context.Parameter.m_flydownSpeedMax)) ? (context.Movement.VertVelocity + context.Movement.GetGravity() * context.Parameter.m_flyGravityRate * deltaTime) : (context.Parameter.m_flydownSpeedMax * context.Movement.GetGravityDir()));
				DeleteEffect();
			}
			context.Movement.Velocity = a + b;
			if (context.Movement.IsOnGround() && !StateUtil.ChangeToJumpStateIfPrecedeInputTouch(context, 0.1f, false))
			{
				StateUtil.NowLanding(context, false);
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

		private void CreateEffect(CharacterState context)
		{
			m_effect = StateUtil.CreateEffect(context, m_effectName, true, ResourceCategory.CHARA_EFFECT);
		}

		private void DeleteEffect()
		{
			StateUtil.DestroyParticle(m_effect, 1f);
			m_effect = null;
		}
	}
}
