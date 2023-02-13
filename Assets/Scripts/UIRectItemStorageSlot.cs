using UnityEngine;

[AddComponentMenu("NGUI/Item/UI Rect Item Storage Slot")]
public class UIRectItemStorageSlot : UIRectItemSlot
{
	public UIRectItemStorage storage;

	public UIRectItemStorageRanking storageRanking;

	public int slot;

	protected override UIInvGameItem observedItem
	{
		get
		{
			UIInvGameItem uIInvGameItem = (!(storage != null)) ? null : storage.GetItem(slot);
			if (uIInvGameItem == null)
			{
				uIInvGameItem = ((!(storageRanking != null)) ? null : storageRanking.GetItem(slot));
			}
			return uIInvGameItem;
		}
	}

	protected override UIInvGameItem Replace(UIInvGameItem item)
	{
		UIInvGameItem uIInvGameItem = null;
		if (storage != null)
		{
			return storage.Replace(slot, item);
		}
		if (storageRanking != null)
		{
			return storageRanking.Replace(slot, item);
		}
		return item;
	}
}
