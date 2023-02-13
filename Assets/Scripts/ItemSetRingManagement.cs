using UnityEngine;

public class ItemSetRingManagement : MonoBehaviour
{
	private int m_offset;

	public void AddOffset(int offset)
	{
		m_offset += offset;
		if (m_offset > 0)
		{
			m_offset = 0;
		}
		else
		{
			UpdateRingCount();
		}
	}

	public void UpdateRingCount()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			instance.ItemData.RingCountOffset = m_offset;
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	public bool IsEnablePurchase(int itemCost)
	{
		int displayRingCount = GetDisplayRingCount();
		if (itemCost <= displayRingCount)
		{
			return true;
		}
		return false;
	}

	public int GetDisplayRingCount()
	{
		int result = 0;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			result = instance.ItemData.DisplayRingCount;
		}
		return result;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
