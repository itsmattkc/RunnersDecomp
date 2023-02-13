using Text;

public class ServerPrizeData
{
	private int m_itemId;

	public int weight;

	public int num;

	public int spinId;

	public int priority;

	public int itemId
	{
		get
		{
			return m_itemId;
		}
		set
		{
			m_itemId = value;
			if (m_itemId >= 300000 && m_itemId < 400000)
			{
				priority = 100;
			}
			else if (m_itemId >= 400000 && m_itemId < 500000)
			{
				if (m_itemId >= 402000)
				{
					priority = 2;
				}
				else if (m_itemId >= 401000)
				{
					priority = 1;
				}
				else
				{
					priority = 0;
				}
			}
			else if (m_itemId == 200002)
			{
				priority = -1;
			}
			else
			{
				priority = -100;
			}
		}
	}

	public ServerPrizeData()
	{
		itemId = 0;
		weight = 0;
		num = 1;
		spinId = 0;
		priority = -100;
	}

	public string GetItemName()
	{
		string text = null;
		ServerItem serverItem = new ServerItem((ServerItem.Id)itemId);
		if (serverItem.idType == ServerItem.IdType.CHARA)
		{
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, RouletteUtility.GetChaoGroupName(itemId), RouletteUtility.GetChaoCellName(itemId)).text;
		}
		if (serverItem.idType == ServerItem.IdType.CHAO)
		{
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, RouletteUtility.GetChaoGroupName(itemId), RouletteUtility.GetChaoCellName(itemId)).text;
		}
		return serverItem.serverItemName;
	}

	public void CopyTo(ServerPrizeData to)
	{
		to.itemId = itemId;
		to.num = num;
		to.weight = weight;
		to.spinId = spinId;
	}
}
