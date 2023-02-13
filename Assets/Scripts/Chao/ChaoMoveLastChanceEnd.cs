using UnityEngine;

namespace Chao
{
	public class ChaoMoveLastChanceEnd : ChaoMoveBase
	{
		public override void Enter(ChaoMovement context)
		{
			if (context.gameObject != null && context.gameObject.transform != null)
			{
				Vector3 localEulerAngles = context.gameObject.transform.localEulerAngles;
				localEulerAngles.y = 90f;
				context.gameObject.transform.localEulerAngles = localEulerAngles;
			}
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			context.Position = context.TargetPosition;
		}
	}
}
