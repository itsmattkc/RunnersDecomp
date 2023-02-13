using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Item/UI Rect Item Storage")]
public class UIRectItemStorage : MonoBehaviour
{
	public enum ActiveType
	{
		ACTIVE,
		NOT_ACTTIVE,
		DEFAULT
	}

	public bool isPlaceVertical;

	public int maxItemCount = 8;

	public int maxRows = 4;

	public int maxColumns = 4;

	public GameObject template;

	public UIWidget background;

	public int spacing_x = 128;

	public int spacing_y = 128;

	public int padding = 10;

	private List<UIInvGameItem> mItems = new List<UIInvGameItem>();

	public ActiveType m_activeType = ActiveType.DEFAULT;

	private bool m_initCountainer;

	public List<UIInvGameItem> items
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

	public UIInvGameItem GetItem(int slot)
	{
		return (slot >= items.Count) ? null : mItems[slot];
	}

	public UIInvGameItem Replace(int slot, UIInvGameItem item)
	{
		if (slot < maxItemCount)
		{
			UIInvGameItem result = items[slot];
			mItems[slot] = item;
			return result;
		}
		return item;
	}

	private void Start()
	{
		if (!m_initCountainer)
		{
			InitContainer();
		}
	}

	private void Place(int x, int y, int count, Bounds b)
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, template);
		if (gameObject != null)
		{
			Transform transform = gameObject.transform;
			transform.localPosition = new Vector3((float)padding + ((float)x + 0.5f) * (float)spacing_x, (float)(-padding) - ((float)y + 0.5f) * (float)spacing_y, 0f);
			UIRectItemStorageSlot component = gameObject.GetComponent<UIRectItemStorageSlot>();
			if (component != null)
			{
				component.storage = this;
				component.slot = count;
			}
			switch (m_activeType)
			{
			case ActiveType.ACTIVE:
				gameObject.SetActive(true);
				break;
			case ActiveType.NOT_ACTTIVE:
				gameObject.SetActive(false);
				break;
			}
		}
		b.Encapsulate(new Vector3((float)padding * 2f + (float)((x + 1) * spacing_x), (float)(-padding) * 2f - (float)((y + 1) * spacing_y), 0f));
		if (++count >= maxItemCount && background != null)
		{
			background.transform.localScale = b.size;
		}
	}

	public void Restart()
	{
		GameObject gameObject = base.gameObject;
		GameObject[] array = new GameObject[gameObject.transform.childCount];
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			array[i] = gameObject.transform.GetChild(i).gameObject;
		}
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.transform.parent = null;
			gameObject2.SetActive(false);
			Object.Destroy(gameObject2);
		}
		InitContainer();
	}

	private void InitContainer()
	{
		if (!(template != null))
		{
			return;
		}
		int count = 0;
		Bounds b = default(Bounds);
		if (isPlaceVertical)
		{
			for (int i = 0; i < maxColumns; i++)
			{
				for (int j = 0; j < maxRows; j++)
				{
					Place(i, j, count, b);
				}
			}
		}
		else
		{
			for (int k = 0; k < maxRows; k++)
			{
				for (int l = 0; l < maxColumns; l++)
				{
					Place(l, k, count, b);
				}
			}
		}
		if (background != null)
		{
			background.transform.localScale = b.size;
		}
		m_initCountainer = true;
	}

	public void Strip()
	{
		while (base.transform.childCount > maxItemCount)
		{
			GameObject gameObject = base.transform.GetChild(base.transform.childCount - 1).gameObject;
			gameObject.transform.parent = null;
			Object.Destroy(gameObject);
		}
	}
}
