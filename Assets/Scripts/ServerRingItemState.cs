public class ServerRingItemState
{
	public int m_itemId;

	public int m_cost;

	public ServerRingItemState()
	{
		m_itemId = 0;
		m_cost = 0;
	}

	public void Dump()
	{
		Debug.Log(string.Format("itemId={0}, cost={1}", m_itemId, m_cost));
	}
}
