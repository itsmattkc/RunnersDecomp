using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class StateReachCannon : FSMState<CharacterState>
	{
		private enum Flags
		{
			LAUNCH_CANNON,
			MODEL_OFF
		}

		private enum SubState
		{
			JUMP,
			GOTARGET,
			HOLD
		}

		private const float ReachedTime = 0.5f;

		private const float HeightOffset = 1.5f;

		private const float GoTargetTime = 0.2f;

		private Vector3 m_reachPosition;

		private float m_height;

		private GameObject m_catchedObject;

		private Vector3 m_horzVelocity;

		private Vector3 m_vertVelocity;

		private float m_timer;

		private Bitset32 m_flag;

		private SubState m_substate;

		public override void Enter(CharacterState context)
		{
			CannonReachParameter enteringParameter = context.GetEnteringParameter<CannonReachParameter>();
			if (enteringParameter != null)
			{
				m_reachPosition = enteringParameter.m_position;
				m_height = enteringParameter.m_height;
				m_catchedObject = enteringParameter.m_catchedObject;
			}
			else
			{
				m_reachPosition = context.Position;
				m_height = 0f;
				m_catchedObject = null;
			}
			m_flag.Reset();
			context.ChangeMovement(MOVESTATE_ID.Air);
			m_substate = SubState.JUMP;
			StateUtil.SetAirMovementToRotateGround(context, false);
			context.GetAnimator().CrossFade("SpinBall", 0.1f);
			CalcReachedVelocity(context);
			m_timer = 0.25f;
			context.SetNotCharaChange(true);
			context.SetNotUseItem(true);
			context.ClearAirAction();
			ObjUtil.PauseCombo(MsgPauseComboTimer.State.PAUSE);
			ObjUtil.SetDisableEquipItem(true);
		}

		public override void Leave(CharacterState context)
		{
			if (!m_flag.Test(0))
			{
				if (m_flag.Test(1))
				{
					context.SetModelNotDraw(false);
					StateUtil.SetNotDrawItemEffect(context, false);
				}
				if (m_catchedObject != null)
				{
					MsgOnExitAbideObject value = new MsgOnExitAbideObject();
					m_catchedObject.SendMessage("OnExitAbideObject", value);
				}
			}
			context.SetNotCharaChange(false);
			context.SetNotUseItem(false);
			ObjUtil.PauseCombo(MsgPauseComboTimer.State.PLAY);
			ObjUtil.SetDisableEquipItem(false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			switch (m_substate)
			{
			case SubState.JUMP:
				context.Movement.Velocity += context.Movement.GetGravity() * deltaTime;
				m_timer -= deltaTime;
				if (m_timer <= 0f)
				{
					context.ChangeMovement(MOVESTATE_ID.GoTarget);
					StateUtil.SetTargetMovement(context, m_reachPosition, context.transform.rotation, 0.2f);
					m_timer = 0.2f;
					m_substate = SubState.GOTARGET;
				}
				break;
			case SubState.GOTARGET:
				if (!m_flag.Test(1) && (m_reachPosition - context.Position).sqrMagnitude < m_height * m_height)
				{
					context.SetModelNotDraw(true);
					StateUtil.SetNotDrawItemEffect(context, true);
					m_flag.Set(1, true);
				}
				m_timer -= deltaTime;
				if (m_timer <= 0f)
				{
					if (!m_flag.Test(1))
					{
						context.SetModelNotDraw(true);
						StateUtil.SetNotDrawItemEffect(context, true);
						m_flag.Set(1, true);
					}
					if (m_catchedObject != null)
					{
						MsgOnAbidePlayerLocked value = new MsgOnAbidePlayerLocked();
						m_catchedObject.SendMessage("OnAbidePlayerLocked", value);
					}
					StateUtil.ResetVelocity(context);
					context.ChangeMovement(MOVESTATE_ID.Air);
					m_substate = SubState.HOLD;
				}
				break;
			case SubState.HOLD:
				StateUtil.ResetVelocity(context);
				break;
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (messageId == 24578)
			{
				if (m_substate != SubState.HOLD)
				{
					return true;
				}
				MsgOnCannonImpulse msgOnCannonImpulse = msg as MsgOnCannonImpulse;
				if (msgOnCannonImpulse != null)
				{
					CannonLaunchParameter cannonLaunchParameter = context.CreateEnteringParameter<CannonLaunchParameter>();
					if (cannonLaunchParameter != null)
					{
						cannonLaunchParameter.Set(msgOnCannonImpulse.m_position, msgOnCannonImpulse.m_rotation, msgOnCannonImpulse.m_firstSpeed, m_height, msgOnCannonImpulse.m_outOfControl);
						m_flag.Set(0, true);
						context.ChangeState(STATE_ID.LaunchCannon);
						msgOnCannonImpulse.m_succeed = true;
					}
					return true;
				}
				return true;
			}
			return false;
		}

		private void CalcReachedVelocity(CharacterState context)
		{
			Vector3 vector = m_reachPosition - context.Position;
			Vector3 b = Vector3.Project(vector, -context.Movement.GetGravityDir());
			Vector3 a = vector - b;
			Vector3 gravity = context.Movement.GetGravity();
			float num = 0.5f;
			float num2 = b.magnitude + 1.5f;
			float d = (num2 + 0.5f * gravity.magnitude * num * num) / num;
			m_vertVelocity = Vector3.up * d;
			m_horzVelocity = a / 0.5f;
			context.Movement.Velocity = m_vertVelocity + m_horzVelocity;
		}
	}
}
