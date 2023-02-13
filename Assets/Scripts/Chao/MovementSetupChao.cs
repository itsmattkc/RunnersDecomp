namespace Chao
{
	public class MovementSetupChao
	{
		public static FSMStateFactory<ChaoMovement>[] GetMovementStateTable()
		{
			return new FSMStateFactory<ChaoMovement>[14]
			{
				new FSMStateFactory<ChaoMovement>(2, new ChaoMovePursue()),
				new FSMStateFactory<ChaoMovement>(3, new ChaoMoveComeIn()),
				new FSMStateFactory<ChaoMovement>(4, new ChaoMoveExit()),
				new FSMStateFactory<ChaoMovement>(5, new ChaoMoveStop()),
				new FSMStateFactory<ChaoMovement>(6, new ChaoMoveStopEnd()),
				new FSMStateFactory<ChaoMovement>(7, new ChaoMoveLastChance()),
				new FSMStateFactory<ChaoMovement>(8, new ChaoMoveLastChanceEnd()),
				new FSMStateFactory<ChaoMovement>(9, new ChaoMoveAttackBoss()),
				new FSMStateFactory<ChaoMovement>(11, new ChaoMoveGoRingBanking()),
				new FSMStateFactory<ChaoMovement>(12, new ChaoMoveGoCameraTarget()),
				new FSMStateFactory<ChaoMovement>(13, new ChaoMoveGoCameraTargetUsePlayerSpeed()),
				new FSMStateFactory<ChaoMovement>(15, new ChaoMoveStay()),
				new FSMStateFactory<ChaoMovement>(14, new ChaoMoveGoKillOut()),
				new FSMStateFactory<ChaoMovement>(16, new ChaoMoveOutCameraTarget())
			};
		}
	}
}
