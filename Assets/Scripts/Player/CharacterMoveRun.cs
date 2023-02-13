using UnityEngine;

namespace Player
{
	public class CharacterMoveRun : CharacterMoveBase
	{
		private float m_errorTime;

		private float m_errorTimeMax;

		private Vector3 m_prevPos = Vector3.zero;

		private HitInfo m_sweepHitInfo;

		public override void Enter(CharacterMovement context)
		{
			m_sweepHitInfo = default(HitInfo);
			m_errorTime = 0f;
			m_prevPos = Vector3.zero;
			m_errorTimeMax = context.Parameter.m_hitWallDeadTime * 2f;
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
			Vector3 newRayPosition = context.RaycastCheckPosition;
			MovementUtil.CheckAndPushOutByRaycast(context.transform, context.RaycastCheckPosition, ref newRayPosition);
			context.SetRaycastCheckPosition(newRayPosition);
			if (m_prevPos == newRayPosition)
			{
				m_errorTime += deltaTime;
				if (m_errorTime > m_errorTimeMax)
				{
					CapsuleCollider component = context.GetComponent<CapsuleCollider>();
					if (component != null)
					{
						int layer = -1 - (1 << LayerMask.NameToLayer("Player"));
						MovementUtil.SweepMoveInnerParam innerParam = new MovementUtil.SweepMoveInnerParam(component, new Vector3(-0.2f, 0.1f, 0f), layer);
						MovementUtil.SweepMoveOuterParam outerParam = new MovementUtil.SweepMoveOuterParam();
						MovementUtil.SweepMove(context.transform, innerParam, outerParam);
					}
				}
			}
			else
			{
				m_errorTime = 0f;
			}
			m_prevPos = newRayPosition;
			context.SetSweepHitInfo(m_sweepHitInfo);
			if (Vector3.Dot(context.transform.forward, CharacterDefs.BaseFrontTangent) < -0.866f)
			{
				Debug.Log("Warning:CharacterRotate is Reversed.");
				context.SetLookRotation(context.transform.forward, context.transform.up);
			}
		}
	}
}
