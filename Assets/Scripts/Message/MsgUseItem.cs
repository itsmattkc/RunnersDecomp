using UnityEngine;

namespace Message
{
	public class MsgUseItem : MessageBase
	{
		public readonly ItemType m_itemType;

		public float m_time;

		public GameObject m_gameObject;

		public MsgUseItem(ItemType itemType)
			: base(12288)
		{
			m_itemType = itemType;
		}

		public MsgUseItem(ItemType itemType, float time)
			: base(12288)
		{
			m_itemType = itemType;
			m_time = time;
		}

		public MsgUseItem(ItemType itemType, GameObject obj)
			: base(12288)
		{
			m_itemType = itemType;
			m_gameObject = obj;
		}
	}
}
