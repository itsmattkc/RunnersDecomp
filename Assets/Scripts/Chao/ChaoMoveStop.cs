using UnityEngine;

namespace Chao
{
	public class ChaoMoveStop : ChaoMoveBase
	{
		private Vector3 m_init_pos = new Vector3(0f, 0f, 0f);

		public override void Enter(ChaoMovement context)
		{
			m_init_pos = context.TargetPosition;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			context.Position = m_init_pos + context.Hovering + context.OffsetPosition;
		}
	}
}
