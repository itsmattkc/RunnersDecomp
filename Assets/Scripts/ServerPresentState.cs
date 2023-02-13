public class ServerPresentState
{
	public int m_itemId;

	public int m_numItem;

	public int m_additionalInfo1;

	public int m_additionalInfo2;

	public ServerPresentState()
	{
		m_itemId = 0;
		m_numItem = 0;
		m_additionalInfo1 = 0;
		m_additionalInfo2 = 0;
	}

	public void CopyTo(ServerPresentState to)
	{
		to.m_itemId = m_itemId;
		to.m_numItem = m_numItem;
		to.m_additionalInfo1 = m_additionalInfo1;
		to.m_additionalInfo2 = m_additionalInfo2;
	}
}
