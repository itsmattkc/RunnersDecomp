using System;

public class ServerLoginBonusState
{
	public int m_numLogin;

	public int m_numBonus;

	public DateTime m_lastBonusTime;

	public ServerLoginBonusState()
	{
		m_numLogin = 0;
		m_numBonus = 0;
		m_lastBonusTime = DateTime.Now;
	}

	public void CopyTo(ServerLoginBonusState dest)
	{
		dest.m_numLogin = m_numLogin;
		dest.m_numBonus = m_numBonus;
		dest.m_lastBonusTime = m_lastBonusTime;
	}
}
