public class ServerRingExchangeList
{
	public int m_ringItemId;

	public int m_itemId;

	public int m_itemNum;

	public int m_price;

	public ServerRingExchangeList()
	{
		m_itemId = 0;
		m_itemNum = 0;
		m_price = 0;
	}

	public void Dump()
	{
		Debug.Log(string.Format("itemId={0}, itemNum={1}, price={2}", m_itemId, m_itemNum, m_price));
	}

	public void CopyTo(ServerRingExchangeList dest)
	{
		dest.m_ringItemId = m_ringItemId;
		dest.m_itemId = m_itemId;
		dest.m_itemNum = m_itemNum;
		dest.m_price = m_price;
	}
}
