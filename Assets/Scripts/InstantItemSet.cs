using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class InstantItemSet : MonoBehaviour
{
	private InstantItemButton[] m_instantButtons = new InstantItemButton[3];

	private InstantItemWindow m_window;

	private BoostItemType m_itemType;

	public List<BoostItemType> GetCheckedItemType()
	{
		List<BoostItemType> list = new List<BoostItemType>();
		for (int i = 0; i < 3; i++)
		{
			InstantItemButton instantItemButton = m_instantButtons[i];
			if (!(instantItemButton == null) && instantItemButton.IsChecked())
			{
				list.Add((BoostItemType)i);
			}
		}
		return list;
	}

	public void Setup()
	{
		GameObject itemSetRootObject = ItemSetUtility.GetItemSetRootObject();
		m_window = GameObjectUtil.FindChildGameObjectComponent<InstantItemWindow>(itemSetRootObject, "info_pla");
		for (int i = 0; i < 3; i++)
		{
			InstantItemButton instantItemButton = GameObjectUtil.FindChildGameObjectComponent<InstantItemButton>(base.gameObject, ItemSetUtility.ButtonObjectName[i]);
			if (!(instantItemButton == null))
			{
				instantItemButton.Setup((BoostItemType)i, OnClickInstantButton);
				m_instantButtons[i] = instantItemButton;
			}
		}
		m_itemType = BoostItemType.SCORE_BONUS;
		m_window.SetWindowActive();
		m_window.SetInstantItemType(m_itemType);
		m_window.SetCheckMark(false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if ((bool)m_window)
		{
			m_window.SetInstantItemType(m_itemType);
		}
	}

	public void ResetCheckMark()
	{
		StageInfo stageInfo = GameObjectUtil.FindGameObjectComponent<StageInfo>("StageInfo");
		if (stageInfo != null)
		{
			int num = stageInfo.BoostItemValid.Length;
			for (int i = 0; i < num; i++)
			{
				stageInfo.BoostItemValid[i] = false;
			}
		}
		if (m_window != null)
		{
			m_window.SetCheckMark(false);
		}
		for (int j = 0; j < 3; j++)
		{
			if (m_instantButtons[j] != null)
			{
				m_instantButtons[j].ResetCheckMark();
			}
		}
	}

	public void SetupBoostedItem()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		PlayerData playerData = null;
		if (!(instance != null))
		{
			return;
		}
		playerData = instance.PlayerData;
		if (playerData == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			bool isChecked = playerData.BoostedItem[i];
			if (!(m_instantButtons[i] == null))
			{
				m_instantButtons[i].SetupBoostedItemButton(isChecked);
			}
		}
	}

	public void UpdateFreeItemList(ServerFreeItemState freeItemState)
	{
		List<ServerItemState> itemList = freeItemState.itemList;
		InstantItemButton[] instantButtons = m_instantButtons;
		foreach (InstantItemButton instantItemButton in instantButtons)
		{
			if (instantItemButton == null)
			{
				continue;
			}
			for (int j = 0; j < itemList.Count; j++)
			{
				if (instantItemButton.boostItemType == itemList[j].GetItem().boostItemType)
				{
					instantItemButton.UpdateFreeItemCount(itemList[j].m_num);
				}
			}
		}
	}

	private void OnClickInstantButton(BoostItemType itemType, bool isChecked)
	{
		if (m_window == null)
		{
			return;
		}
		m_window.SetWindowActive();
		m_window.SetInstantItemType(itemType);
		m_window.SetCheckMark(isChecked);
		m_itemType = itemType;
		StageInfo stageInfo = GameObjectUtil.FindGameObjectComponent<StageInfo>("StageInfo");
		if (stageInfo != null)
		{
			stageInfo.BoostItemValid[(int)itemType] = isChecked;
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			PlayerData playerData = instance.PlayerData;
			if (playerData != null)
			{
				playerData.BoostedItem[(int)itemType] = isChecked;
			}
		}
	}
}
