public class ServerDailyBattleStatus
{
	public int numWin;

	public int numLose;

	public int numDraw;

	public int numLoseByDefault;

	public int goOnWin;

	public int goOnLose;

	public ServerDailyBattleStatus()
	{
		numWin = 0;
		numLose = 0;
		numDraw = 0;
		numLoseByDefault = 0;
		goOnWin = 0;
		goOnLose = 0;
	}

	public void Dump()
	{
		Debug.Log(string.Format("ServerDailyBattleStatus  numWin:{0} numLose:{1} numDraw:{2} numLoseByDefault:{3} goOnWin:{4} goOnLose:{5}", numWin, numLose, numDraw, numLoseByDefault, goOnWin, goOnLose));
	}

	public void CopyTo(ServerDailyBattleStatus dest)
	{
		dest.numWin = numWin;
		dest.numLose = numLose;
		dest.numDraw = numDraw;
		dest.numLoseByDefault = numLoseByDefault;
		dest.goOnWin = goOnWin;
		dest.goOnLose = goOnLose;
	}
}
