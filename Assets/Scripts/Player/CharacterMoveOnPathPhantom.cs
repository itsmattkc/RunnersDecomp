using App;
using UnityEngine;

namespace Player
{
	public class CharacterMoveOnPathPhantom : CharacterMoveBase
	{
		private PathEvaluator m_path;

		private BlockPathController.PathType m_pathType;

		public override void Enter(CharacterMovement context)
		{
			m_path = null;
		}

		public override void Leave(CharacterMovement context)
		{
			m_path = null;
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			float num = context.Velocity.magnitude * deltaTime;
			if (m_path == null || !m_path.IsValid())
			{
				return;
			}
			if (m_path.Distance > m_path.GetLength() - num)
			{
				PathEvaluator stagePathEvaluator = GetStagePathEvaluator(context, m_pathType);
				if (stagePathEvaluator == null)
				{
					m_path = null;
					return;
				}
				if (stagePathEvaluator.GetPathObject().GetID() == m_path.GetPathObject().GetID())
				{
					context.transform.position += context.transform.forward * num;
					return;
				}
				SetupPath(context, stagePathEvaluator, false);
			}
			Vector3? normal = null;
			m_path.Advance(num);
			float distance = m_path.Distance;
			Vector3? wpos = Vector3.zero;
			m_path.GetPNT(ref wpos, ref normal, ref normal);
			Vector3 position = context.transform.position;
			Vector3 dst;
			if (Math.Vector3NormalizeIfNotZero(wpos.Value - position, out dst))
			{
				wpos = position + dst * num;
			}
			float dist;
			m_path.GetClosestPositionAlongSpline(wpos.Value, distance - num, distance + num, out dist);
			Vector3? wpos2 = Vector3.zero;
			Vector3? normal2 = Vector3.zero;
			Vector3? tangent = Vector3.zero;
			m_path.GetPNT(dist, ref wpos2, ref normal2, ref tangent);
			m_path.Distance = dist;
			context.transform.position = wpos.Value;
			Quaternion identity = Quaternion.identity;
			if (Mathf.Abs(Vector3.Dot(tangent.Value, CharacterDefs.BaseRightTangent)) > 0.001f)
			{
				Vector3 vector = Vector3.Cross(normal2.Value, CharacterDefs.BaseRightTangent);
				tangent = ((!(Vector3.Dot(vector, tangent.Value) < 0f)) ? new Vector3?(vector) : new Vector3?(-vector));
			}
			identity.SetLookRotation(tangent.Value, normal2.Value);
			context.transform.rotation = identity;
		}

		public void SetupPath(CharacterMovement context, BlockPathController.PathType pathtype)
		{
			m_pathType = pathtype;
			PathEvaluator stagePathEvaluator = GetStagePathEvaluator(context, m_pathType);
			if (stagePathEvaluator != null)
			{
				SetupPath(context, stagePathEvaluator, true);
			}
			else
			{
				m_path = null;
			}
		}

		private void SetupPath(CharacterMovement context, PathEvaluator pathEvaluator, bool setup)
		{
			m_path = new PathEvaluator(pathEvaluator);
			float closestPositionAlongSpline = pathEvaluator.GetClosestPositionAlongSpline(context.transform.position, 0f, m_path.GetLength());
			m_path.Distance = closestPositionAlongSpline;
			if (!setup)
			{
				return;
			}
			Vector3 vector = context.transform.position;
			Vector3? normal = null;
			Vector3? wpos = Vector3.zero;
			m_path.GetPNT(ref wpos, ref normal, ref normal);
			Vector3 value = wpos.Value;
			if (vector.x - value.x > 0.5f)
			{
				float x = vector.x;
				Vector3 sideViewPathPos = context.SideViewPathPos;
				vector = new Vector3(x, sideViewPathPos.y, vector.z);
				closestPositionAlongSpline = pathEvaluator.GetClosestPositionAlongSpline(vector, 0f, m_path.GetLength());
				if (m_path.Distance < closestPositionAlongSpline)
				{
					m_path.Distance = closestPositionAlongSpline;
				}
				SetFrontAddPath(context, pathEvaluator, vector, 5f, 3);
			}
		}

		private void SetFrontAddPath(CharacterMovement context, PathEvaluator pathEvaluator, Vector3 playerPos, float pathY, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3? normal = null;
				Vector3? wpos = Vector3.zero;
				m_path.GetPNT(ref wpos, ref normal, ref normal);
				Vector3 value = wpos.Value;
				if (playerPos.x - value.x > 0.5f)
				{
					playerPos += new Vector3(0f, pathY, 0f);
					float closestPositionAlongSpline = pathEvaluator.GetClosestPositionAlongSpline(playerPos, 0f, m_path.GetLength());
					if (m_path.Distance < closestPositionAlongSpline)
					{
						m_path.Distance = closestPositionAlongSpline;
					}
					continue;
				}
				break;
			}
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
