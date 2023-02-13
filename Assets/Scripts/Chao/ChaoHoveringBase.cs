using UnityEngine;

namespace Chao
{
	public class ChaoHoveringBase : MonoBehaviour
	{
		public class CInfoBase
		{
			public ChaoMovement movement;

			protected CInfoBase(ChaoMovement movement_)
			{
				movement = movement_;
			}
		}

		private ChaoMovement m_movement;

		private Vector3 m_position;

		public Vector3 Position
		{
			get
			{
				return m_position;
			}
			protected set
			{
				m_position = value;
			}
		}

		public ChaoMovement Movement
		{
			get
			{
				return m_movement;
			}
		}

		public void Setup(CInfoBase cinfo)
		{
			m_movement = cinfo.movement;
			SetupImpl(cinfo);
		}

		protected virtual void SetupImpl(CInfoBase info)
		{
		}

		public virtual void Reset()
		{
		}
	}
}
