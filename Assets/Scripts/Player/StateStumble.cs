using UnityEngine;

namespace Player
{
	public class StateStumble : FSMState<CharacterState>
	{
		private const float DamageTime = 2f;

		private const float EnableJump = 1.7f;

		private const float horzVelocityNowall = 5f;

		private float m_timer;

		private bool m_noWall;

		private bool m_onAir;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			context.Movement.OffGround();
			m_timer = 2f;
			context.GetAnimator().CrossFade("Damaged", 0.1f);
			context.Movement.Velocity = context.Movement.GetUpDir() * context.Parameter.m_stumbleJumpForce + context.Movement.GetForwardDir() * context.DefaultSpeed;
			m_noWall = false;
			m_onAir = true;
			context.ClearAirAction();
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
				if (m_onAir)
				{
					context.ChangeMovement(MOVESTATE_ID.Run);
					m_onAir = false;
				}
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
				if (!m_onAir)
				{
					context.ChangeMovement(MOVESTATE_ID.Air);
					m_onAir = true;
				}
				if (!m_noWall)
				{
					Vector3 position = context.Position;
					Vector3 baseFrontTangent = CharacterDefs.BaseFrontTangent;
					int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Terrain"));
					Ray ray = new Ray(position, baseFrontTangent);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, 1.5f, layerMask))
					{
						m_noWall = true;
						context.Movement.VertVelocity = context.Movement.GetUpDir() * 5f;
					}
				}
				else
				{
					vertVelocity += context.Movement.GetGravity() * deltaTime;
					context.Movement.VertVelocity = vertVelocity;
				}
			}
			context.Movement.HorzVelocity = context.Movement.GetForwardDir() * context.DefaultSpeed;
			m_timer -= deltaTime;
			if (m_timer <= 1.7f && context.m_input.IsTouched())
			{
				context.ChangeState(STATE_ID.Jump);
			}
			else if (m_timer <= 0f)
			{
				if (flag)
				{
					context.ChangeState(STATE_ID.Run);
				}
				else
				{
					context.ChangeState(STATE_ID.Fall);
				}
			}
			else if (StateUtil.CheckDeadByHitWall(context, deltaTime))
			{
				context.ChangeState(STATE_ID.Dead);
			}
		}
	}
}
