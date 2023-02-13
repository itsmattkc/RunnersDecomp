using UnityEngine;

namespace Player
{
	public class CharacterMoveAsteroid : CharacterMoveBase
	{
		private const float RotateDegree = 720f;

		private HitInfo m_sweepHitInfo;

		public override void Enter(CharacterMovement context)
		{
			m_sweepHitInfo = default(HitInfo);
		}

		public override void Leave(CharacterMovement context)
		{
			m_sweepHitInfo.Reset();
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			float num = context.Velocity.magnitude * deltaTime;
			if (num <= 0.0001f)
			{
				context.SetRaycastCheckPosition(context.RaycastCheckPosition);
				return;
			}
			MovementUtil.SweepMoveForRunAndAir(context, deltaTime, ref m_sweepHitInfo);
			if (!context.IsOnGround() && !m_sweepHitInfo.IsValid())
			{
				MovementUtil.UpdateRotateOnGravityUp(context, 720f, deltaTime);
			}
			Vector3 newRayPosition = context.RaycastCheckPosition;
			MovementUtil.CheckAndPushOutByRaycast(context.transform, context.RaycastCheckPosition, ref newRayPosition);
			context.SetRaycastCheckPosition(newRayPosition);
			context.SetSweepHitInfo(m_sweepHitInfo);
			if (Vector3.Dot(context.transform.forward, CharacterDefs.BaseFrontTangent) < -0.866f)
			{
				Debug.Log("Warning:CharacterRotate is Reversed.");
				context.SetLookRotation(context.transform.forward, context.transform.up);
			}
		}
	}
}
