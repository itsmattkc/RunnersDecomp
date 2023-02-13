public class ServerOptionUserResult
{
	public long m_totalSumHightScore;

	public long m_quickTotalSumHightScore;

	public long m_numTakeAllRings;

	public long m_numTakeAllRedRings;

	public int m_numChaoRoulette;

	public int m_numItemRoulette;

	public int m_numJackPot;

	public int m_numMaximumJackPotRings;

	public int m_numSupport;

	public void CopyTo(ServerOptionUserResult to)
	{
		if (to != null)
		{
			to.m_totalSumHightScore = m_totalSumHightScore;
			to.m_quickTotalSumHightScore = m_quickTotalSumHightScore;
			to.m_numTakeAllRings = m_numTakeAllRings;
			to.m_numTakeAllRedRings = m_numTakeAllRedRings;
			to.m_numChaoRoulette = m_numChaoRoulette;
			to.m_numItemRoulette = m_numItemRoulette;
			to.m_numJackPot = m_numJackPot;
			to.m_numMaximumJackPotRings = m_numMaximumJackPotRings;
			to.m_numSupport = m_numSupport;
		}
	}
}
