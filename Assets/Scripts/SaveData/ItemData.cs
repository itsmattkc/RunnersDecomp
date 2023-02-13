using System.Collections.Generic;

namespace SaveData
{
	public class ItemData
	{
		private uint m_ring_count;

		private uint m_red_ring_count;

		private int m_ring_count_offset;

		private int m_red_ring_count_offset;

		private uint[] m_item_count = new uint[8];

		private Dictionary<ServerItem.Id, long> m_etc_item_count;

		private Dictionary<ServerItem.Id, long> m_etc_item_count_offset;

		public uint RingCount
		{
			get
			{
				return m_ring_count;
			}
			set
			{
				m_ring_count = value;
			}
		}

		public int RingCountOffset
		{
			get
			{
				return m_ring_count_offset;
			}
			set
			{
				m_ring_count_offset = value;
			}
		}

		public int DisplayRingCount
		{
			get
			{
				return (int)RingCount + RingCountOffset;
			}
		}

		public uint RedRingCount
		{
			get
			{
				return m_red_ring_count;
			}
			set
			{
				m_red_ring_count = value;
			}
		}

		public int RedRingCountOffset
		{
			get
			{
				return m_red_ring_count_offset;
			}
			set
			{
				m_red_ring_count_offset = value;
			}
		}

		public int DisplayRedRingCount
		{
			get
			{
				return (int)RedRingCount + RedRingCountOffset;
			}
		}

		public uint[] ItemCount
		{
			get
			{
				return m_item_count;
			}
			set
			{
				m_item_count = value;
			}
		}

		public ItemData()
		{
			m_ring_count = 0u;
			m_red_ring_count = 0u;
			for (uint num = 0u; num < 8; num++)
			{
				m_item_count[num] = 0u;
			}
		}

		public uint GetItemCount(ItemType type)
		{
			if (IsValidType(type))
			{
				return m_item_count[(int)type];
			}
			return 0u;
		}

		public void SetItemCount(ItemType type, uint count)
		{
			if (IsValidType(type))
			{
				m_item_count[(int)type] = count;
			}
		}

		public uint GetAllItemCount()
		{
			uint num = 0u;
			for (ItemType itemType = ItemType.INVINCIBLE; itemType < ItemType.NUM; itemType++)
			{
				num += GetItemCount(itemType);
			}
			return num;
		}

		private bool IsValidType(ItemType type)
		{
			return ItemType.INVINCIBLE <= type && type < ItemType.NUM;
		}

		public void SetEtcItemCount(ServerItem.Id itemId, long count)
		{
			if (m_etc_item_count == null && m_etc_item_count_offset == null)
			{
				m_etc_item_count = new Dictionary<ServerItem.Id, long>();
				m_etc_item_count_offset = new Dictionary<ServerItem.Id, long>();
			}
			if (m_etc_item_count.ContainsKey(itemId))
			{
				m_etc_item_count[itemId] = count;
				m_etc_item_count_offset[itemId] = 0L;
			}
			else
			{
				m_etc_item_count.Add(itemId, count);
				m_etc_item_count_offset.Add(itemId, 0L);
			}
		}

		public void AddEtcItemCount(ServerItem.Id itemId, long count)
		{
			if (m_etc_item_count == null && m_etc_item_count_offset == null)
			{
				m_etc_item_count = new Dictionary<ServerItem.Id, long>();
				m_etc_item_count_offset = new Dictionary<ServerItem.Id, long>();
			}
			if (m_etc_item_count.ContainsKey(itemId))
			{
				Dictionary<ServerItem.Id, long> etc_item_count;
				Dictionary<ServerItem.Id, long> dictionary = etc_item_count = m_etc_item_count;
				ServerItem.Id key;
				ServerItem.Id key2 = key = itemId;
				long num = etc_item_count[key];
				dictionary[key2] = num + count;
				m_etc_item_count_offset[itemId] = 0L;
			}
			else
			{
				m_etc_item_count.Add(itemId, count);
				m_etc_item_count_offset.Add(itemId, 0L);
			}
		}

		public bool SetEtcItemCountOffset(ServerItem.Id itemId, long offset)
		{
			bool result = false;
			if (m_etc_item_count != null && m_etc_item_count_offset != null && m_etc_item_count_offset.ContainsKey(itemId))
			{
				m_etc_item_count_offset[itemId] = offset;
				result = true;
			}
			return result;
		}

		public long GetEtcItemCount(ServerItem.Id itemId)
		{
			long result = 0L;
			if (m_etc_item_count != null && m_etc_item_count.ContainsKey(itemId) && m_etc_item_count_offset.ContainsKey(itemId))
			{
				result = m_etc_item_count[itemId] + m_etc_item_count_offset[itemId];
			}
			return result;
		}

		public long GetEtcItemCountOffset(ServerItem.Id itemId)
		{
			long result = 0L;
			if (m_etc_item_count != null && m_etc_item_count.ContainsKey(itemId) && m_etc_item_count_offset.ContainsKey(itemId))
			{
				result = m_etc_item_count_offset[itemId];
			}
			return result;
		}

		public bool IsEtcItemCount(ServerItem.Id itemId)
		{
			bool result = false;
			if (m_etc_item_count != null && m_etc_item_count.ContainsKey(itemId) && m_etc_item_count_offset.ContainsKey(itemId))
			{
				result = true;
			}
			return result;
		}
	}
}
