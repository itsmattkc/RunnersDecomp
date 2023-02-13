using UnityEngine;

namespace Boss
{
	public class BossStateAppearMap2 : BossStateAppearMapBase
	{
		private static float WAIT_TIME1 = 3f;

		private static float WAIT_TIME2 = 2f;

		private static float WAIT_TIME3 = 2f;

		public override void Enter(ObjBossEggmanState context)
		{
			base.Enter(context);
			Transform transform = context.transform;
			Vector3 position = context.transform.position;
			float x = position.x;
			Vector3 startPos = context.BossParam.StartPos;
			float y = startPos.y;
			Vector3 position2 = context.transform.position;
			transform.position = new Vector3(x, y, position2.z);
		}

		protected override float GetTime1()
		{
			return WAIT_TIME1;
		}

		protected override float GetTime2()
		{
			return WAIT_TIME2;
		}

		protected override float GetTime3()
		{
			return WAIT_TIME3;
		}

		protected override void SetMotion3(ObjBossEggmanState context)
		{
			context.BossMotion.SetMotion(BossMotion.MISSILE_START);
		}

		protected override STATE_ID GetNextChangeState()
		{
			return STATE_ID.AttackMap2;
		}
	}
}
