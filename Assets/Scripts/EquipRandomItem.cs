using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Equip Random Item")]
public class EquipRandomItem : MonoBehaviour
{
	public InvEquipment equipment;

	private void OnClick()
	{
		if (!(equipment == null))
		{
			List<InvBaseItem> items = InvDatabase.list[0].items;
			if (items.Count != 0)
			{
				int max = 12;
				int num = Random.Range(0, items.Count);
				InvBaseItem invBaseItem = items[num];
				InvGameItem invGameItem = new InvGameItem(num, invBaseItem);
				invGameItem.quality = (InvGameItem.Quality)Random.Range(0, max);
				invGameItem.itemLevel = NGUITools.RandomRange(invBaseItem.minItemLevel, invBaseItem.maxItemLevel);
				equipment.Equip(invGameItem);
			}
		}
	}
}
