public class ServerRedStarItemState
{
	public int m_storeItemId;

	public int m_itemId;

	public int m_numItem;

	public int m_price;

	public string m_priceDisp;

	public string m_productId;

	public ServerRedStarItemState()
	{
		m_storeItemId = 0;
		m_itemId = 0;
		m_numItem = 0;
		m_price = 0;
		m_priceDisp = string.Empty;
		m_productId = string.Empty;
	}

	public void CopyTo(ServerRedStarItemState dest)
	{
		dest.m_storeItemId = m_storeItemId;
		dest.m_itemId = m_itemId;
		dest.m_numItem = m_numItem;
		dest.m_price = m_price;
		dest.m_priceDisp = m_priceDisp;
		dest.m_productId = m_productId;
	}

	public void Dump()
	{
		Debug.Log(string.Format("storeItemId={0}, itemId={1}, numItem={2}, price={3}", m_storeItemId, m_itemId, m_numItem, m_price));
	}
}
