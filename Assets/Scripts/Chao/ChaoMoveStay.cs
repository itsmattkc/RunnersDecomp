using UnityEngine;

namespace Chao
{
	public class ChaoMoveStay : ChaoMoveBase
	{
		private Vector3 m_stayPosition;

		public override void Enter(ChaoMovement context)
		{
			m_stayPosition = context.Position - context.Hovering;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			context.Position = m_stayPosition + context.Hovering;
		}
	}
}
