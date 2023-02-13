using Message;
using UnityEngine;

namespace Player
{
	public class StateDamage : FSMState<CharacterState>
	{
		private float m_timer;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Run);
			context.Movement.OffGround();
			m_timer = context.Parameter.m_damageStumbleTime;
			context.GetAnimator().CrossFade("Damaged", 0.05f);
			context.Movement.HorzVelocity = context.Movement.GetForwardDir() * context.DefaultSpeed * context.Parameter.m_damageSpeedRate;
			context.StartDamageBlink();
			if (!context.m_notDropRing)
			{
				SoundManager.SePlay("act_ringspread");
			}
			context.ClearAirAction();
			if ((bool)StageTutorialManager.Instance)
			{
				MsgTutorialDamage value = new MsgTutorialDamage();
				StageTutorialManager.Instance.SendMessage("OnMsgTutorialDamage", value, SendMessageOptions.DontRequireReceiver);
			}
			GameObjectUtil.SendDelayedMessageToTagObjects("Boss", "OnPlayerDamage", new MsgBossPlayerDamage(false));
			ObjUtil.StopCombo();
		}

		public override void Leave(CharacterState context)
		{
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			bool flag = context.Movement.IsOnGround();
			Vector3 vertVelocity = context.Movement.VertVelocity;
			if (flag)
			{
				HitInfo info;
				if (context.Movement.GetGroundInfo(out info))
				{
					Vector3 normal = info.info.normal;
					vertVelocity -= Vector3.Project(vertVelocity, normal);
					context.Movement.VertVelocity = vertVelocity;
				}
			}
			else
			{
				vertVelocity += context.Movement.GetGravity() * deltaTime;
				context.Movement.VertVelocity = vertVelocity;
			}
			context.Movement.HorzVelocity = context.Movement.GetForwardDir() * context.DefaultSpeed * context.Parameter.m_damageSpeedRate;
			m_timer -= deltaTime;
			if (m_timer <= context.Parameter.m_damageEnableJumpTime && context.m_input.IsTouched())
			{
				context.ChangeState(STATE_ID.Jump);
				return;
			}
			if (m_timer <= 0f)
			{
				if (flag)
				{
					context.ChangeState(STATE_ID.Run);
				}
				else
				{
					context.ChangeState(STATE_ID.Fall);
				}
				return;
			}
			STATE_ID state = STATE_ID.Non;
			if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
			{
				context.ChangeState(state);
			}
		}
	}
}
