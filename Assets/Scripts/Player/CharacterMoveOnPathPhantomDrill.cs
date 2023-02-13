using App;
using UnityEngine;

namespace Player
{
	public class CharacterMoveOnPathPhantomDrill : CharacterMoveBase
	{
		private const float TargetSearchRate = 3f;

		private const float MaxRotate = 180f;

		private PathEvaluator m_path;

		private BlockPathController.PathType m_pathType;

		private bool m_noJump;

		private Vector3 m_offset;

		private float m_speed;

		private bool m_nowJump;

		private Vector3 m_basePosition;

		private Vector3 m_baseVelocity;

		private Vector3 m_jumpVelocity;

		public override void Enter(CharacterMovement context)
		{
			m_path = null;
			m_basePosition = context.transform.position;
			m_baseVelocity = context.Velocity;
			m_nowJump = false;
			m_speed = 0f;
			m_offset = Vector3.zero;
		}

		public override void Leave(CharacterMovement context)
		{
			m_path = null;
			EndJump();
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			StepRunOnPath(context, deltaTime);
			StepJump(context, deltaTime);
			context.Velocity = m_baseVelocity + m_jumpVelocity;
			context.transform.position += context.Velocity * deltaTime;
			Rotate(context, deltaTime);
		}

		public void StepRunOnPath(CharacterMovement context, float deltaTime)
		{
			if (m_path != null && m_path.IsValid())
			{
				Vector3 basePosition = m_basePosition;
				float num = m_speed * deltaTime;
				float num2 = num * 3f;
				Vector3? wpos = Vector3.zero;
				if (!CheckAndChangeToNextPath(context, num))
				{
					m_basePosition += m_baseVelocity * deltaTime;
					return;
				}
				Vector3? normal = null;
				float distance = m_path.Distance;
				m_path.GetPNT(distance + num2, ref wpos, ref normal, ref normal);
				Vector3 currentVelocity = m_baseVelocity;
				Vector3 vector = Vector3.SmoothDamp(basePosition - m_offset, wpos.Value, ref currentVelocity, deltaTime * 3f, float.PositiveInfinity, deltaTime);
				float dist;
				m_path.GetClosestPositionAlongSpline(vector, distance - num, distance + num, out dist);
				m_path.Distance = dist;
				m_basePosition = vector + m_offset;
				m_baseVelocity = (m_basePosition - basePosition) / deltaTime;
			}
		}

		public void StepJump(CharacterMovement context, float deltaTime)
		{
			if (m_nowJump)
			{
				Vector3 rhs = -context.GetGravityDir();
				if (Vector3.Dot(m_jumpVelocity, rhs) < 0f && Vector3.Dot(context.transform.position - m_basePosition, rhs) < 0f)
				{
					EndJump();
				}
				else
				{
					m_jumpVelocity += context.GetGravityDir() * context.Parameter.m_drillJumpGravity * deltaTime;
				}
			}
		}

		private bool CheckAndChangeToNextPath(CharacterMovement context, float runLength)
		{
			if (m_path.Distance > m_path.GetLength() - runLength)
			{
				PathEvaluator stagePathEvaluator = GetStagePathEvaluator(context, m_pathType);
				if (stagePathEvaluator == null)
				{
					m_path = null;
					return false;
				}
				if (stagePathEvaluator.GetPathObject().GetID() == m_path.GetPathObject().GetID())
				{
					return false;
				}
				SetupPath(context, stagePathEvaluator);
			}
			return true;
		}

		public void Jump(CharacterMovement context)
		{
			if (!m_nowJump && !m_noJump)
			{
				m_nowJump = true;
				m_jumpVelocity = -context.GetGravityDir() * context.Parameter.m_drillJumpForce;
			}
		}

		private void EndJump()
		{
			m_nowJump = false;
			m_jumpVelocity = Vector3.zero;
		}

		private void Rotate(CharacterMovement context, float deltaTime)
		{
			Vector3 dst = context.GetForwardDir();
			Vector3 current = dst;
			Vector3 upDir = context.GetUpDir();
			if (Math.Vector3NormalizeIfNotZero(context.Velocity, out dst))
			{
				dst = Vector3.RotateTowards(current, dst, 180f * deltaTime, 0f);
				upDir = Vector3.Cross(dst, CharacterDefs.BaseRightTangent);
				if (Mathf.Abs(Vector3.Dot(dst, CharacterDefs.BaseRightTangent)) > 0.001f)
				{
					Vector3 vector = Vector3.Cross(upDir, CharacterDefs.BaseRightTangent);
					dst = ((!(Vector3.Dot(vector, dst) < 0f)) ? vector : (-vector));
					upDir = Vector3.Cross(dst, CharacterDefs.BaseRightTangent);
				}
				context.SetLookRotation(dst, upDir);
			}
		}

		public void SetupPath(CharacterMovement context, BlockPathController.PathType pathtype, bool noJump, float offset)
		{
			m_pathType = pathtype;
			m_noJump = noJump;
			m_offset = offset * Vector3.up;
			PathEvaluator stagePathEvaluator = GetStagePathEvaluator(context, m_pathType);
			if (stagePathEvaluator != null)
			{
				SetupPath(context, stagePathEvaluator);
			}
			else
			{
				m_path = null;
			}
		}

		private void SetupPath(CharacterMovement context, PathEvaluator pathEvaluator)
		{
			m_path = new PathEvaluator(pathEvaluator);
			float closestPositionAlongSpline = pathEvaluator.GetClosestPositionAlongSpline(context.transform.position, 0f, m_path.GetLength());
			m_path.Distance = closestPositionAlongSpline;
		}

		public void SetSpeed(CharacterMovement context, float speed)
		{
			m_speed = speed;
			m_baseVelocity = context.GetForwardDir() * speed;
		}

		public bool IsPathEnd(float remainDist)
		{
			if (m_path == null)
			{
				return true;
			}
			if (!m_path.IsValid())
			{
				return true;
			}
			if (m_path.GetLength() - m_path.Distance < remainDist)
			{
				return true;
			}
			return false;
		}

		public bool IsPathValid()
		{
			if (m_path.IsValid())
			{
				return true;
			}
			return false;
		}

		public static PathEvaluator GetStagePathEvaluator(CharacterMovement context, BlockPathController.PathType patytype)
		{
			StageBlockPathManager blockPathManager = context.GetBlockPathManager();
			if (blockPathManager != null)
			{
				PathEvaluator curentPathEvaluator = blockPathManager.GetCurentPathEvaluator(patytype);
				if (curentPathEvaluator != null)
				{
					return curentPathEvaluator;
				}
			}
			return null;
		}
	}
}
