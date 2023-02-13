using SaveData;

public static class ItemPool
{
	private class ItemData
	{
		public uint[] item_count = new uint[8];

		public ItemData()
		{
			uint num = 8u;
			for (uint num2 = 0u; num2 < num; num2++)
			{
				item_count[num2] = 0u;
			}
		}
	}

	public const uint MAX_ITEM_COUNT = 99u;

	public const uint MAX_RING_COUNT = 9999999u;

	public const uint MAX_REDRING_COUNT = 9999999u;

	private static ItemData m_item_data = new ItemData();

	private static uint m_ring_count = 0u;

	private static uint m_red_ring_count = 0u;

	public static uint RingCount
	{
		get
		{
			return m_ring_count;
		}
		set
		{
			m_ring_count = value;
			if (m_ring_count > 9999999)
			{
				m_ring_count = 9999999u;
			}
		}
	}

	public static uint RedRingCount
	{
		get
		{
			return m_red_ring_count;
		}
		set
		{
			m_red_ring_count = value;
			if (m_red_ring_count > 9999999)
			{
				m_red_ring_count = 9999999u;
			}
		}
	}

	public static void SetItemCount(ItemType i_item_type, uint i_count)
	{
		if (i_item_type < ItemType.NUM)
		{
			m_item_data.item_count[(int)i_item_type] = i_count;
			if (m_item_data.item_count[(int)i_item_type] > 99)
			{
				m_item_data.item_count[(int)i_item_type] = 99u;
			}
		}
	}

	public static uint GetItemCount(ItemType i_item_type)
	{
		if (i_item_type < ItemType.NUM)
		{
			return m_item_data.item_count[(int)i_item_type];
		}
		return 0u;
	}

	public static void Initialize()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			SaveData.ItemData itemData = instance.ItemData;
			RingCount = itemData.RingCount;
			RedRingCount = itemData.RedRingCount;
			for (uint num = 0u; num < 8; num++)
			{
				ItemType itemType = (ItemType)num;
				SetItemCount(itemType, itemData.GetItemCount(itemType));
			}
		}
	}

	public static void ReflctSaveData()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			SaveData.ItemData itemData = instance.ItemData;
			itemData.RingCount = m_ring_count;
			itemData.RedRingCount = m_red_ring_count;
			for (uint num = 0u; num < 8; num++)
			{
				itemData.SetItemCount((ItemType)num, m_item_data.item_count[num]);
			}
			instance.ItemData = itemData;
			instance.SaveItemData();
		}
	}
}
