using UnityEngine;

[AddComponentMenu("NGUI/Examples/Equip Items")]
public class EquipItems : MonoBehaviour
{
	public int[] itemIDs;

	private void Start()
	{
		if (itemIDs != null && itemIDs.Length > 0)
		{
			InvEquipment invEquipment = GetComponent<InvEquipment>();
			if (invEquipment == null)
			{
				invEquipment = base.gameObject.AddComponent<InvEquipment>();
			}
			int max = 12;
			int i = 0;
			for (int num = itemIDs.Length; i < num; i++)
			{
				int num2 = itemIDs[i];
				InvBaseItem invBaseItem = InvDatabase.FindByID(num2);
				if (invBaseItem != null)
				{
					InvGameItem invGameItem = new InvGameItem(num2, invBaseItem);
					invGameItem.quality = (InvGameItem.Quality)Random.Range(0, max);
					invGameItem.itemLevel = NGUITools.RandomRange(invBaseItem.minItemLevel, invBaseItem.maxItemLevel);
					invEquipment.Equip(invGameItem);
				}
				else
				{
					Debug.LogWarning("Can't resolve the item ID of " + num2);
				}
			}
		}
		Object.Destroy(this);
	}
}
