using Message;
using UnityEngine;

namespace Player
{
	public class StateLastChance : FSMState<CharacterState>
	{
		private enum SubState
		{
			GoTarget,
			Run,
			EndAction,
			EndWait
		}

		private const float offset = 2f;

		private SubState m_substate;

		private float m_time;

		private Vector3 m_targetPos;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, false);
			StateUtil.ActiveChaoAbilityMagnetObject(context);
			m_time = 3f;
			if (StageAbilityManager.Instance != null)
			{
				m_time = StageAbilityManager.Instance.GetChaoAbliltyValue(ChaoAbility.LAST_CHANCE, m_time);
			}
			context.SetNotCharaChange(true);
			context.SetNotUseItem(true);
			context.SetLastChance(true);
			context.ClearAirAction();
			context.SetModelNotDraw(true);
			GotoTarget(context);
			StateUtil.DeactiveInvincible(context);
			StateUtil.DeactiveBarrier(context);
			StateUtil.DeactiveMagetObject(context);
			StateUtil.DeactiveTrampoline(context);
			MsgStartLastChance value = new MsgStartLastChance(context.gameObject);
			GameObjectUtil.SendMessageToTagObjects("Chao", "OnStartLastChance", value, SendMessageOptions.DontRequireReceiver);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			context.SetNotCharaChange(false);
			context.SetNotUseItem(false);
			context.SetLastChance(false);
			context.SetModelNotDraw(false);
			StateUtil.DeactiveChaoAbilityMagetObject(context);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			switch (m_substate)
			{
			case SubState.EndWait:
				break;
			case SubState.GoTarget:
				StepTarget(context, deltaTime);
				break;
			case SubState.Run:
				StepRun(context, deltaTime);
				break;
			case SubState.EndAction:
				m_time -= deltaTime;
				if (m_time < 0f)
				{
					GotoEndWait(context);
				}
				break;
			}
		}

		private void StepTarget(CharacterState context, float deltaTime)
		{
			float magnitude = context.Movement.Velocity.magnitude;
			float num = Vector3.Distance(m_targetPos, context.Position);
			Vector3 a = Vector3.Normalize(m_targetPos - context.Position);
			if (num > magnitude * deltaTime)
			{
				context.Movement.Velocity = a * context.Parameter.m_lastChanceSpeed;
				return;
			}
			context.Movement.ResetPosition(m_targetPos);
			GotoRun(context);
		}

		private void StepRun(CharacterState context, float deltaTime)
		{
			m_time -= deltaTime;
			if (m_time < 0f)
			{
				GotoEndAction(context);
			}
			else if (context.GetLevelInformation().NowFeverBoss)
			{
				GotoEndAction(context);
			}
		}

		private void GotoTarget(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.IgnoreCollision);
			PathEvaluator stagePathEvaluator = StateUtil.GetStagePathEvaluator(context, BlockPathController.PathType.SV);
			if (stagePathEvaluator != null)
			{
				Vector3 worldPosition = stagePathEvaluator.GetWorldPosition();
				m_targetPos = stagePathEvaluator.GetWorldPosition() + Vector3.up * 2f;
				float num = Vector3.Dot(context.Position - worldPosition, -context.Movement.GetGravityDir());
				if (num < 0f)
				{
					context.Movement.ResetPosition(context.Position + num * context.Movement.GetGravityDir());
					context.GetPlayerInformation().SetTransform(context.transform);
				}
			}
			else
			{
				m_targetPos = context.Position;
			}
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			m_substate = SubState.GoTarget;
		}

		private void GotoRun(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.RunOnPathPhantomDrill);
			CharacterMoveOnPathPhantomDrill currentState = context.Movement.GetCurrentState<CharacterMoveOnPathPhantomDrill>();
			if (currentState != null)
			{
				currentState.SetupPath(context.Movement, BlockPathController.PathType.SV, true, 2f);
				currentState.SetSpeed(context.Movement, context.Parameter.m_lastChanceSpeed);
			}
			m_substate = SubState.Run;
		}

		private void GotoEndAction(CharacterState context)
		{
			context.Movement.ChangeState(MOVESTATE_ID.IgnoreCollision);
			StateUtil.ResetVelocity(context);
			MsgEndLastChance value = new MsgEndLastChance();
			GameObjectUtil.SendMessageToTagObjects("Chao", "OnEndLastChance", value, SendMessageOptions.DontRequireReceiver);
			m_time = 1f;
			m_substate = SubState.EndAction;
		}

		private void GotoEndWait(CharacterState context)
		{
			MsgNotifyDead value = new MsgNotifyDead();
			GameObject gameObject = GameObject.Find("GameModeStage");
			if ((bool)gameObject)
			{
				gameObject.SendMessage("OnMsgNotifyDead", value, SendMessageOptions.DontRequireReceiver);
			}
			GameObjectUtil.SendDelayedMessageToTagObjects("Boss", "OnMsgNotifyDead", value);
			m_substate = SubState.EndWait;
		}
	}
}
