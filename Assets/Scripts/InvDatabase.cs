using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Examples/Item Database")]
public class InvDatabase : MonoBehaviour
{
	private static InvDatabase[] mList;

	private static bool mIsDirty = true;

	public int databaseID;

	public List<InvBaseItem> items = new List<InvBaseItem>();

	public UIAtlas iconAtlas;

	public static InvDatabase[] list
	{
		get
		{
			if (mIsDirty)
			{
				mIsDirty = false;
				mList = NGUITools.FindActive<InvDatabase>();
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

	private InvBaseItem GetItem(int id16)
	{
		int i = 0;
		for (int count = items.Count; i < count; i++)
		{
			InvBaseItem invBaseItem = items[i];
			if (invBaseItem.id16 == id16)
			{
				return invBaseItem;
			}
		}
		return null;
	}

	private static InvDatabase GetDatabase(int dbID)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			InvDatabase invDatabase = list[i];
			if (invDatabase.databaseID == dbID)
			{
				return invDatabase;
			}
		}
		return null;
	}

	public static InvBaseItem FindByID(int id32)
	{
		InvDatabase database = GetDatabase(id32 >> 16);
		return (!(database != null)) ? null : database.GetItem(id32 & 0xFFFF);
	}

	public static InvBaseItem FindByName(string exact)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			InvDatabase invDatabase = list[i];
			int j = 0;
			for (int count = invDatabase.items.Count; j < count; j++)
			{
				InvBaseItem invBaseItem = invDatabase.items[j];
				if (invBaseItem.name == exact)
				{
					return invBaseItem;
				}
			}
		}
		return null;
	}

	public static int FindItemID(InvBaseItem item)
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			InvDatabase invDatabase = list[i];
			if (invDatabase.items.Contains(item))
			{
				return (invDatabase.databaseID << 16) | item.id16;
			}
		}
		return -1;
	}
}
