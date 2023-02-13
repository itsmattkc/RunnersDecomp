using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIInvDatabase : MonoBehaviour
{
	private static UIInvDatabase[] mList;

	private static bool mIsDirty = true;

	public int databaseID;

	public List<UIInvBaseItem> items = new List<UIInvBaseItem>();

	public UIAtlas iconAtlas;

	public static UIInvDatabase[] list
	{
		get
		{
			if (mIsDirty)
			{
				mIsDirty = false;
				mList = NGUITools.FindActive<UIInvDatabase>();
			}
			return mList;
		}
	}

	private void OnEnable()
	{
		mIsDirty = true;
	}

	private void OnDisable()
	{
		mIsDirty = true;
	}

	private UIInvBaseItem GetItem(int id16)
	{
		int i = 0;
		for (int count = items.Count; i < count; i++)
		{
			UIInvBaseItem uIInvBaseItem = items[i];
			if (uIInvBaseItem.id16 == id16)
			{
				return uIInvBaseItem;
			}
		}
		return null;
	}

	private static UIInvDatabase GetDatabase(int dbID)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			UIInvDatabase uIInvDatabase = list[i];
			if (uIInvDatabase.databaseID == dbID)
			{
				return uIInvDatabase;
			}
		}
		return null;
	}

	public static UIInvBaseItem FindByID(int id32)
	{
		UIInvDatabase database = GetDatabase(id32 >> 16);
		return (!(database != null)) ? null : database.GetItem(id32 & 0xFFFF);
	}

	public static UIInvBaseItem FindByName(string exact)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			UIInvDatabase uIInvDatabase = list[i];
			int j = 0;
			for (int count = uIInvDatabase.items.Count; j < count; j++)
			{
				UIInvBaseItem uIInvBaseItem = uIInvDatabase.items[j];
				if (uIInvBaseItem.name == exact)
				{
					return uIInvBaseItem;
				}
			}
		}
		return null;
	}

	public static int FindItemID(UIInvBaseItem item)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			UIInvDatabase uIInvDatabase = list[i];
			if (uIInvDatabase.items.Contains(item))
			{
				return (uIInvDatabase.databaseID << 16) | item.id16;
			}
		}
		return -1;
	}
}
