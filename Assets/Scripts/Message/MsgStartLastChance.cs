using UnityEngine;

namespace Message
{
	public class MsgStartLastChance : MessageBase
	{
		private GameObject m_parentObject;

		public GameObject ParentObject
		{
			get
			{
				return m_parentObject;
			}
		}

		public MsgStartLastChance(GameObject parentObject)
			: base(24587)
		{
			m_parentObject = parentObject;
		}
	}
}
