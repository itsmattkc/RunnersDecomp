using App;
using UnityEngine;

namespace Player
{
	public class CharacterMoveOnPath : CharacterMoveBase
	{
		private PathEvaluator m_path;

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
			if (m_path != null && m_path.IsValid())
			{
				Vector3? normal = null;
				float num = context.Velocity.magnitude * deltaTime;
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
				Debug.DrawLine(wpos2.Value, wpos2.Value + normal2.Value * 1f, Color.red, 1f);
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
		}

		public void SetupPath(Vector3 position, PathComponent component, float? distance)
		{
			m_path = new PathEvaluator(component);
			if (!distance.HasValue)
			{
				float dist = 0f;
				m_path.GetClosestPositionAlongSpline(position, 0f, m_path.GetLength(), out dist);
				m_path.Distance = dist;
			}
			else
			{
				m_path.Distance = distance.Value;
			}
		}

		public bool IsPathEnd(float remainDist)
		{
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
	}
}
