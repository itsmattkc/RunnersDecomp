using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Storage Slot")]
public class UIStorageSlot : UIItemSlot
{
	public UIItemStorage storage;

	public int slot;

	protected override InvGameItem observedItem
	{
		get
		{
			return (!(storage != null)) ? null : storage.GetItem(slot);
		}
	}

	protected override InvGameItem Replace(InvGameItem item)
	{
		return (!(storage != null)) ? item : storage.Replace(slot, item);
	}
}
