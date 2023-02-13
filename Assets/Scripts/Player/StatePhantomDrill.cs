using Message;
using UnityEngine;

namespace Player
{
	public class StatePhantomDrill : FSMState<CharacterState>
	{
		private enum SubState
		{
			GODOWN,
			RUN,
			RETURN
		}

		private const float jump_force = 14f;

		private const float lerpDelta = 3f;

		private const float godown_offset = 1.5f;

		private const float ray_offset = 3f;

		private const float ray_offset2 = 5f;

		private const float offset_noground = 2f;

		private float m_time;

		private SubState m_substate;

		private GameObject m_truck;

		private Vector3 m_targetPos = Vector3.zero;

		private PathEvaluator m_targetPath;

		private GameObject m_effect;

		private Vector3 m_prevPosition;

		private bool m_nowInDirt;

		private bool m_changePhantomCancel;

		public override void Enter(CharacterState context)
		{
			StateUtil.DeactiveInvincible(context);
			StateUtil.SetNotDrawItemEffect(context, true);
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			m_effect = PhantomDrillUtil.ChangeVisualOnEnter(context);
			m_truck = PhantomDrillUtil.CreateTruck(context);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			context.OnAttackAttribute(AttackAttribute.PhantomDrill);
			m_time = -1f;
			ChangePhantomParameter enteringParameter = context.GetEnteringParameter<ChangePhantomParameter>();
			if (enteringParameter != null)
			{
				m_time = enteringParameter.Timer;
			}
			m_targetPath = StateUtil.GetStagePathEvaluator(context, BlockPathController.PathType.DRILL);
			if (m_targetPath != null)
			{
				m_targetPos = m_targetPath.GetWorldPosition();
			}
			GotoDown(context);
			m_prevPosition = context.Position;
			m_nowInDirt = false;
			StateUtil.SendMessageTransformPhantom(context, PhantomType.DRILL);
			if (context.GetChangePhantomCancel() == ItemType.DRILL)
			{
				m_changePhantomCancel = true;
			}
			else
			{
				m_changePhantomCancel = false;
			}
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			StateUtil.SetNotDrawItemEffect(context, false);
			PhantomDrillUtil.ChangeVisualOnLeave(context, m_effect);
			PhantomDrillUtil.DestroyTruck(m_truck);
			m_effect = null;
			m_truck = null;
			m_targetPath = null;
			StateUtil.SendMessageReturnFromPhantom(context, PhantomType.DRILL);
			context.SetChangePhantomCancel(ItemType.UNKNOWN);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			if (m_changePhantomCancel)
			{
				m_changePhantomCancel = false;
				DispatchMessage(context, 12289, new MsgInvalidateItem(ItemType.DRILL));
				return;
			}
			switch (m_substate)
			{
			case SubState.GODOWN:
				if (StepGoDown(context, deltaTime))
				{
					return;
				}
				break;
			case SubState.RUN:
				if (StepRunning(context, deltaTime))
				{
					return;
				}
				break;
			case SubState.RETURN:
				if (StepReturn(context, deltaTime))
				{
					return;
				}
				break;
			}
			bool nowInDirt = m_nowInDirt;
			m_nowInDirt = PhantomDrillUtil.CheckTruckDraw(context, m_truck);
			if ((nowInDirt && !m_nowInDirt) || (!nowInDirt && m_nowInDirt))
			{
				PhantomDrillUtil.CheckAndCreateFogEffect(context, !nowInDirt && m_nowInDirt, m_prevPosition);
			}
			m_prevPosition = context.Position;
		}

		private bool StepGoDown(CharacterState context, float deltaTime)
		{
			float magnitude = context.Movement.Velocity.magnitude;
			float num = Vector3.Distance(m_targetPos, context.Position);
			Vector3 vector = Vector3.Normalize(m_targetPos - context.Position);
			if (num > magnitude * deltaTime)
			{
				Vector3 up = Vector3.Cross(vector, context.transform.right);
				StateUtil.SetRotation(context, up, vector);
				context.Movement.Velocity = vector * context.Parameter.m_drillSpeed;
			}
			else
			{
				context.Movement.ResetPosition(m_targetPos);
				GotoRun(context);
			}
			return false;
		}

		private bool StepRunning(CharacterState context, float deltaTime)
		{
			Vector3 velocity = context.Parameter.m_drillSpeed * CharacterDefs.BaseFrontTangent;
			context.Movement.Velocity = velocity;
			if (m_time > 0f)
			{
				m_time -= deltaTime;
				if (m_time < 0f)
				{
					GotoReturn(context);
					return false;
				}
			}
			CharacterMoveOnPathPhantomDrill currentState = context.Movement.GetCurrentState<CharacterMoveOnPathPhantomDrill>();
			if (currentState != null && currentState.IsPathEnd(0f))
			{
				GotoReturn(context);
				return false;
			}
			if (currentState != null && context.m_input.IsTouched())
			{
				currentState.Jump(context.Movement);
			}
			return false;
		}

		private bool StepReturn(CharacterState context, float deltaTime)
		{
			float magnitude = context.Movement.Velocity.magnitude;
			float num = Vector3.Distance(m_targetPos, context.Position);
			Vector3 vector = Vector3.Normalize(m_targetPos - context.Position);
			if (num > magnitude * deltaTime)
			{
				Vector3 up = Vector3.Cross(vector, context.transform.right);
				StateUtil.SetRotation(context, up, vector);
				context.Movement.Velocity = vector * context.Parameter.m_drillSpeed;
				return false;
			}
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			context.Movement.ResetPosition(m_targetPos);
			context.Movement.Velocity = context.Movement.GetForwardDir() * context.DefaultSpeed + context.Movement.GetUpDir() * 14f;
			StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.DRILL, false);
			return true;
		}

		private void GotoDown(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.IgnoreCollision);
			m_substate = SubState.GODOWN;
		}

		private void GotoRun(CharacterState context)
		{
			StartPathMove(context);
			m_substate = SubState.RUN;
		}

		private void GotoReturn(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.IgnoreCollision);
			PathEvaluator stagePathEvaluator = StateUtil.GetStagePathEvaluator(context, BlockPathController.PathType.SV);
			if (stagePathEvaluator != null)
			{
				m_targetPos = stagePathEvaluator.GetWorldPosition();
				Vector3 a = -context.Movement.GetGravityDir();
				Vector3 origin = m_targetPos + a * 1.5f;
				Ray ray = new Ray(origin, -a);
				int num = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Terrain"));
				num = -1 - (1 << LayerMask.NameToLayer("Player"));
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 3f, num))
				{
					m_targetPos = hitInfo.point + hitInfo.normal * 0.1f;
				}
				else
				{
					origin = m_targetPos + a * 5f;
					if (Physics.Raycast(ray, out hitInfo, 5f, num))
					{
						m_targetPos = hitInfo.point + hitInfo.normal * 0.1f;
					}
					else
					{
						m_targetPos = stagePathEvaluator.GetWorldPosition() + a * 2f;
					}
				}
			}
			else
			{
				m_targetPos = context.Position;
			}
			CapsuleCollider component = context.GetComponent<CapsuleCollider>();
			if (component != null)
			{
				int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Terrain"));
				Vector3 vector = -context.Movement.GetGravityDir();
				if (StateUtil.CheckOverlapSphere(m_targetPos, vector, component.height * 0.5f + 0.2f, layerMask))
				{
					float num2 = 2f;
					StateUtil.CapsuleCast(component, m_targetPos + vector * num2, vector, layerMask, -vector, num2 - 0.1f, ref m_targetPos, true);
				}
			}
			m_substate = SubState.RETURN;
		}

		private void StartPathMove(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.RunOnPathPhantomDrill);
			CharacterMoveOnPathPhantomDrill currentState = context.Movement.GetCurrentState<CharacterMoveOnPathPhantomDrill>();
			if (currentState != null)
			{
				currentState.SetupPath(context.Movement, BlockPathController.PathType.DRILL, false, 0f);
				currentState.SetSpeed(context.Movement, context.Parameter.m_drillSpeed);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (messageId == 12289)
			{
				MsgInvalidateItem msgInvalidateItem = msg as MsgInvalidateItem;
				if (msgInvalidateItem != null && msgInvalidateItem.m_itemType == ItemType.DRILL)
				{
					if (m_substate == SubState.RUN)
					{
						GotoReturn(context);
					}
					else if (m_substate == SubState.GODOWN)
					{
						StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.DRILL, false);
					}
					return true;
				}
				return true;
			}
			return false;
		}
	}
}
