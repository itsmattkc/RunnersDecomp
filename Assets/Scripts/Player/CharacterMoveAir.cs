using UnityEngine;

namespace Player
{
	public class CharacterMoveAir : CharacterMoveBase
	{
		private const float cos135 = -0.707f;

		private const float RotateDegree = 720f;

		private HitInfo m_sweepHitInfo;

		private bool m_rotateToGround;

		private float m_errorTime;

		private Vector3 m_prevPos = Vector3.zero;

		public override void Enter(CharacterMovement context)
		{
			m_sweepHitInfo = default(HitInfo);
			context.OffGround();
			SetRotateToGround(false);
			m_errorTime = 0f;
			m_prevPos = Vector3.zero;
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
			CapsuleCollider component = context.GetComponent<CapsuleCollider>();
			if (m_sweepHitInfo.IsValid() && !context.IsOnGround())
			{
				RaycastHit info = m_sweepHitInfo.info;
				Vector3 vector = info.point - component.bounds.center;
				vector -= Vector3.Project(vector, Vector3.up);
				if (Vector3.Dot(info.normal, CharacterDefs.BaseFrontTangent) < -0.707f && vector.sqrMagnitude < component.radius * component.radius)
				{
					int layer = -1 - (1 << LayerMask.NameToLayer("Player"));
					MovementUtil.SweepMoveInnerParam innerParam = new MovementUtil.SweepMoveInnerParam(component, info.normal * 0.1f, layer);
					MovementUtil.SweepMoveOuterParam outerParam = new MovementUtil.SweepMoveOuterParam();
					MovementUtil.SweepMove(context.transform, innerParam, outerParam);
				}
			}
			if (m_rotateToGround && !m_sweepHitInfo.IsValid())
			{
				MovementUtil.UpdateRotateOnGravityUp(context, 720f, deltaTime);
			}
			Vector3 newRayPosition = context.RaycastCheckPosition;
			MovementUtil.CheckAndPushOutByRaycast(context.transform, context.RaycastCheckPosition, ref newRayPosition);
			context.SetRaycastCheckPosition(newRayPosition);
			if (m_prevPos == newRayPosition)
			{
				m_errorTime += deltaTime;
				if (m_errorTime > 0.5f)
				{
					RaycastHit info2 = m_sweepHitInfo.info;
					int layer2 = -1 - (1 << LayerMask.NameToLayer("Player"));
					MovementUtil.SweepMoveInnerParam innerParam2 = new MovementUtil.SweepMoveInnerParam(component, info2.normal * 0.1f, layer2);
					MovementUtil.SweepMoveOuterParam outerParam2 = new MovementUtil.SweepMoveOuterParam();
					MovementUtil.SweepMove(context.transform, innerParam2, outerParam2);
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

		public void SetRotateToGround(bool value)
		{
			m_rotateToGround = value;
		}
	}
}
