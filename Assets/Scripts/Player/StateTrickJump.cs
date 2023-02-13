using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class StateTrickJump : FSMState<CharacterState>
	{
		private enum Flags
		{
			SUCCEED,
			ENABLE_TRICK,
			TRICK_END,
			ISFALL,
			AUTO_SUCCEED
		}

		private const float LerpDelta = 0.5f;

		private const float cos5 = 0.9962f;

		private bool m_jumpCamera;

		private float m_animTime = 0.2f;

		private Bitset32 m_flag;

		private float m_outOfControlTime;

		private int m_numTrick;

		private bool Succeed
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

		private bool EnableTrick
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

		private bool TrickEnd
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

		private bool Falling
		{
			get
			{
				return m_flag.Test(3);
			}
			set
			{
				m_flag.Set(3, value);
			}
		}

		private bool AutoSucceed
		{
			get
			{
				return m_flag.Test(4);
			}
			set
			{
				m_flag.Set(4, value);
			}
		}

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, false);
			m_flag.Reset();
			m_outOfControlTime = 0f;
			Vector3 velocity = Vector3.zero;
			float num = 0f;
			TrickJumpParameter enteringParameter = context.GetEnteringParameter<TrickJumpParameter>();
			if (enteringParameter != null)
			{
				context.Movement.ResetPosition(enteringParameter.m_position);
				StateUtil.SetRotation(context, -context.Movement.GetGravityDir());
				m_outOfControlTime = enteringParameter.m_outOfControlTime;
				num = enteringParameter.m_firstSpeed;
				velocity = enteringParameter.m_rotation * Vector3.up * num;
				Succeed = enteringParameter.m_succeed;
			}
			StageAbilityManager instance = StageAbilityManager.Instance;
			if (instance != null)
			{
				ChaoAbility ability = ChaoAbility.JUMP_RAMP;
				if (instance.HasChaoAbility(ability))
				{
					float chaoAbilityValue = instance.GetChaoAbilityValue(ability);
					float num2 = Random.Range(0f, 99.9f);
					if (chaoAbilityValue >= num2)
					{
						if (!Succeed)
						{
							m_outOfControlTime = enteringParameter.m_succeedOutOfcontrol;
							num = enteringParameter.m_succeedFirstSpeed;
							velocity = enteringParameter.m_succeedRotation * Vector3.up * num;
						}
						Succeed = true;
						instance.RequestPlayChaoEffect(ability);
					}
				}
			}
			context.Movement.Velocity = velocity;
			if (Succeed)
			{
				context.GetAnimator().CrossFade("TrickJumpIdle", 0.1f);
				EnableTrick = true;
				m_jumpCamera = true;
				SoundManager.SePlay("obj_jumpboard_ok");
				StateUtil.SetJumpRampMagnet(context, true);
				ChaoAbility ability2 = ChaoAbility.JUMP_RAMP_TRICK_SUCCESS;
				if (instance.HasChaoAbility(ability2))
				{
					float chaoAbilityValue2 = instance.GetChaoAbilityValue(ability2);
					float num3 = Random.Range(0f, 99.9f);
					if (chaoAbilityValue2 >= num3)
					{
						AutoSucceed = true;
						ObjUtil.RequestStartAbilityToChao(ability2, false);
					}
				}
			}
			else
			{
				context.GetAnimator().CrossFade("Damaged", 0.1f);
				m_jumpCamera = false;
				SoundManager.SePlay("obj_jumpboard_ng");
			}
			context.OnAttack(AttackPower.PlayerSpin, DefensePower.PlayerSpin);
			StateUtil.SetAttackAttributePowerIfPowerType(context, true);
			context.SetNotCharaChange(true);
			m_numTrick = 0;
			context.ClearAirAction();
			if (m_jumpCamera && context.GetCamera() != null)
			{
				MsgPushCamera value = new MsgPushCamera(CameraType.JUMPBOARD, 0.5f);
				context.GetCamera().SendMessage("OnPushCamera", value);
			}
			StateUtil.ThroughBreakable(context, true);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			context.SetNotCharaChange(false);
			if (m_jumpCamera && context.GetCamera() != null)
			{
				MsgPopCamera value = new MsgPopCamera(CameraType.JUMPBOARD, 0.5f);
				context.GetCamera().SendMessage("OnPopCamera", value);
			}
			StateUtil.ThroughBreakable(context, false);
			StateUtil.SetJumpRampMagnet(context, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			m_outOfControlTime -= deltaTime;
			CheckTrick(context, deltaTime);
			if (Succeed && !TrickEnd && !EnableTrick)
			{
				CheckAnimation(context, deltaTime);
			}
			if (m_outOfControlTime < 0f)
			{
				StateUtil.ThroughBreakable(context, false);
				if (!Falling && !Succeed)
				{
					context.GetAnimator().CrossFade("Fall", 0.3f);
				}
				Falling = true;
				Vector3 velocity = context.Movement.Velocity;
				velocity += context.Movement.GetGravity() * deltaTime;
				context.Movement.Velocity = velocity;
			}
			if (context.Movement.GetVertVelocityScalar() <= 0f && context.Movement.IsOnGround())
			{
				StateUtil.NowLanding(context, true);
				context.ChangeState(STATE_ID.Run);
			}
		}

		private void CheckTrick(CharacterState context, float deltaTime)
		{
			CharacterInput component = context.GetComponent<CharacterInput>();
			if (component != null && EnableTrick && !StateUtil.IsAnimationInTransition(context) && (AutoSucceed || component.IsTouched()))
			{
				int num = CharacterDefs.TrickScore[m_numTrick];
				MsgCaution caution = new MsgCaution((HudCaution.Type)(4 + m_numTrick));
				HudCaution.Instance.SetCaution(caution);
				MsgCaution caution2 = new MsgCaution(HudCaution.Type.TRICK_BONUS_N, num);
				HudCaution.Instance.SetCaution(caution2);
				ObjUtil.SendMessageAddScore(num);
				ObjUtil.SendMessageScoreCheck(new StageScoreData(1, num));
				string str = "TrickJump";
				str += (m_numTrick % 3 + 1).ToString("D1");
				context.GetAnimator().CrossFade(str, 0.05f);
				GameObject gameobj = StateUtil.CreateEffect(context, "ef_pl_trick01", true);
				StateUtil.SetObjectLocalPositionToCenter(context, gameobj);
				EnableTrick = false;
				m_numTrick++;
				m_animTime = 0.25f;
				if (m_numTrick < 5)
				{
					SoundManager.SePlay("obj_jumpboard_trick");
				}
				else
				{
					SoundManager.SePlay("obj_jumpboard_trick_last");
				}
			}
		}

		private void CheckAnimation(CharacterState context, float deltaTime)
		{
			if (EnableTrick || m_numTrick <= 0)
			{
				return;
			}
			if (m_animTime > 0f)
			{
				m_animTime -= deltaTime;
				if (m_animTime <= 0f)
				{
					context.GetAnimator().CrossFade("TrickJumpIdle", 0.05f);
					m_animTime = -1f;
				}
			}
			else if (m_numTrick >= 5)
			{
				context.GetAnimator().CrossFade("Fall", 0.5f);
				TrickEnd = true;
				EnableTrick = false;
			}
			else
			{
				EnableTrick = true;
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (Falling && StateUtil.ChangeAfterSpinattack(context, messageId, msg))
			{
				return true;
			}
			return false;
		}
	}
}
