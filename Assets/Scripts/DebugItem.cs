using UnityEngine;

public class DebugItem : MonoBehaviour
{
	private GameObject[] m_item_object = new GameObject[8];

	private GameObject m_ring_object;

	private GameObject m_red_ring_object;

	private void Start()
	{
		m_item_object[0] = GameObject.Find("InvincibleCountLabel");
		m_item_object[1] = GameObject.Find("BarrierCountLabel");
		m_item_object[2] = GameObject.Find("MagnetCountLabel");
		m_item_object[3] = GameObject.Find("TrampolineCountLabel");
		m_item_object[4] = GameObject.Find("ComboCountLabel");
		m_item_object[5] = GameObject.Find("LaserCountLabel");
		m_item_object[6] = GameObject.Find("DrillCountLabel");
		m_item_object[7] = GameObject.Find("AsteroidCountLabel");
		m_ring_object = GameObject.Find("RingCountLabel");
		m_red_ring_object = GameObject.Find("RedRingCountLabel");
		ItemPool.Initialize();
	}

	private void Update()
	{
		for (uint num = 0u; num < 8; num++)
		{
			if ((bool)m_item_object[num])
			{
				uint itemCount = ItemPool.GetItemCount((ItemType)num);
				UILabel component = m_item_object[num].GetComponent<UILabel>();
				if ((bool)component)
				{
					component.text = itemCount.ToString();
				}
			}
		}
		if ((bool)m_ring_object)
		{
			uint ringCount = ItemPool.RingCount;
			UILabel component2 = m_ring_object.GetComponent<UILabel>();
			if ((bool)component2)
			{
				component2.text = ringCount.ToString();
			}
		}
		if ((bool)m_red_ring_object)
		{
			uint redRingCount = ItemPool.RedRingCount;
			UILabel component3 = m_red_ring_object.GetComponent<UILabel>();
			if ((bool)component3)
			{
				component3.text = redRingCount.ToString();
			}
		}
	}

	private void OnAddInvincibleCount(GameObject obj)
	{
		if (obj.name == "InvincibleAddButton")
		{
			uint itemCount = ItemPool.GetItemCount(ItemType.INVINCIBLE);
			ItemPool.SetItemCount(ItemType.INVINCIBLE, itemCount + 1);
		}
	}

	private void OnSubInvincibleCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.INVINCIBLE);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.INVINCIBLE, itemCount - 1);
		}
	}

	private void OnAddBarrierCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.BARRIER);
		ItemPool.SetItemCount(ItemType.BARRIER, itemCount + 1);
	}

	private void OnSubBarrierCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.BARRIER);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.BARRIER, itemCount - 1);
		}
	}

	private void OnAddMagnetCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.MAGNET);
		ItemPool.SetItemCount(ItemType.MAGNET, itemCount + 1);
	}

	private void OnSubMagnetCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.MAGNET);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.MAGNET, itemCount - 1);
		}
	}

	private void OnAddTrampolineCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.TRAMPOLINE);
		ItemPool.SetItemCount(ItemType.TRAMPOLINE, itemCount + 1);
	}

	private void OnSubTrampolineCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.TRAMPOLINE);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.TRAMPOLINE, itemCount - 1);
		}
	}

	private void OnAddComboCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.COMBO);
		ItemPool.SetItemCount(ItemType.COMBO, itemCount + 1);
	}

	private void OnSubComboCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.COMBO);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.COMBO, itemCount - 1);
		}
	}

	private void OnAddLaserCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.LASER);
		ItemPool.SetItemCount(ItemType.LASER, itemCount + 1);
	}

	private void OnSubLaserCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.LASER);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.LASER, itemCount - 1);
		}
	}

	private void OnAddDrillCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.DRILL);
		ItemPool.SetItemCount(ItemType.DRILL, itemCount + 1);
	}

	private void OnSubDrillCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.DRILL);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.DRILL, itemCount - 1);
		}
	}

	private void OnAddAsteroidCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.ASTEROID);
		ItemPool.SetItemCount(ItemType.ASTEROID, itemCount + 1);
	}

	private void OnSubAsteroidCount()
	{
		uint itemCount = ItemPool.GetItemCount(ItemType.ASTEROID);
		if (itemCount != 0)
		{
			ItemPool.SetItemCount(ItemType.ASTEROID, itemCount - 1);
		}
	}

	private void OnAddRingCount()
	{
		ItemPool.RingCount++;
	}

	private void OnSubRingCount()
	{
		uint ringCount = ItemPool.RingCount;
		if (ringCount != 0)
		{
			ItemPool.RingCount = ringCount - 1;
		}
	}

	private void OnAddRedRingCount()
	{
		ItemPool.RedRingCount++;
	}

	private void OnSubRedRingCount()
	{
		uint redRingCount = ItemPool.RedRingCount;
		if (redRingCount != 0)
		{
			ItemPool.RedRingCount = redRingCount - 1;
		}
	}
}
