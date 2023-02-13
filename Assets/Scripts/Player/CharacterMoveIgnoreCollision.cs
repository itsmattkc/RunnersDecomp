using UnityEngine;

namespace Player
{
	public class CharacterMoveIgnoreCollision : CharacterMoveBase
	{
		public override void Enter(CharacterMovement context)
		{
		}

		public override void Leave(CharacterMovement context)
		{
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			Vector3 position = context.transform.position + context.Velocity * deltaTime;
			context.transform.position = position;
		}
	}
}
