using Text;

public class ShopItemTable
{
	private static ShopItemData[] m_shopItemDataTable;

	public static ShopItemData[] GetDataTable()
	{
		if (m_shopItemDataTable == null)
		{
			m_shopItemDataTable = new ShopItemData[8];
			for (int i = 0; i < m_shopItemDataTable.Length; i++)
			{
				int num = i + 1;
				m_shopItemDataTable[i] = new ShopItemData();
				m_shopItemDataTable[i].number = num;
				m_shopItemDataTable[i].rings = 100;
				m_shopItemDataTable[i].SetName(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", "name" + num).text);
				m_shopItemDataTable[i].SetDetails(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", "details" + num).text);
			}
		}
		return m_shopItemDataTable;
	}

	public static ShopItemData GetShopItemData(int id)
	{
		ShopItemData[] dataTable = GetDataTable();
		foreach (ShopItemData shopItemData in dataTable)
		{
			if (shopItemData.id == id)
			{
				return shopItemData;
			}
		}
		return null;
	}

	public static ShopItemData GetShopItemDataOfIndex(int index)
	{
		return GetDataTable()[index];
	}
}
