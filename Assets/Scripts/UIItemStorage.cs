using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/UI Item Storage")]
public class UIItemStorage : MonoBehaviour
{
	public int maxItemCount = 8;

	public int maxRows = 4;

	public int maxColumns = 4;

	public GameObject template;

	public UIWidget background;

	public int spacing = 128;

	public int padding = 10;

	private List<InvGameItem> mItems = new List<InvGameItem>();

	public List<InvGameItem> items
	{
		get
		{
			while (mItems.Count < maxItemCount)
			{
				mItems.Add(null);
			}
			return mItems;
		}
	}

	public InvGameItem GetItem(int slot)
	{
		return (slot >= items.Count) ? null : mItems[slot];
	}

	public InvGameItem Replace(int slot, InvGameItem item)
	{
		if (slot < maxItemCount)
		{
			InvGameItem result = items[slot];
			mItems[slot] = item;
			return result;
		}
		return item;
	}

	private void Start()
	{
		if (!(template != null))
		{
			return;
		}
		int num = 0;
		Bounds bounds = default(Bounds);
		for (int i = 0; i < maxRows; i++)
		{
			for (int j = 0; j < maxColumns; j++)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, template);
				Transform transform = gameObject.transform;
				transform.localPosition = new Vector3((float)padding + ((float)j + 0.5f) * (float)spacing, (float)(-padding) - ((float)i + 0.5f) * (float)spacing, 0f);
				UIStorageSlot component = gameObject.GetComponent<UIStorageSlot>();
				if (component != null)
				{
					component.storage = this;
					component.slot = num;
				}
				bounds.Encapsulate(new Vector3((float)padding * 2f + (float)((j + 1) * spacing), (float)(-padding) * 2f - (float)((i + 1) * spacing), 0f));
				if (++num >= maxItemCount)
				{
					if (background != null)
					{
						background.transform.localScale = bounds.size;
					}
					return;
				}
			}
		}
		if (background != null)
		{
			background.transform.localScale = bounds.size;
		}
	}
}
