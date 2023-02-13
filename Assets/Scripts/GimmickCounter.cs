using System.Collections.Generic;
using UnityEngine;

public class GimmickCounter : MonoBehaviour
{
	private int m_waitTimer;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_waitTimer == 5)
		{
			InitCoroutine();
		}
		m_waitTimer++;
	}

	private void InitCoroutine()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		GameObject gameObject = base.gameObject;
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			if (child == null)
			{
				continue;
			}
			GameObject gameObject2 = child.gameObject;
			if (gameObject2 == null)
			{
				continue;
			}
			string name = gameObject2.name;
			switch (name)
			{
			case "MultiSetLine":
			case "MultiSetSector":
			case "MultiSetCircle":
			case "MultiSetParaloopCircle":
			{
				int childCount2 = child.childCount;
				for (int j = 0; j < childCount2; j++)
				{
					Transform child2 = child.GetChild(j);
					if (child2 == null)
					{
						continue;
					}
					GameObject gameObject3 = child2.gameObject;
					if (!(gameObject3 == null))
					{
						string name2 = gameObject3.name;
						int value2;
						if (!dictionary.TryGetValue(name2, out value2))
						{
							dictionary.Add(name2, 1);
							continue;
						}
						Dictionary<string, int> dictionary4;
						Dictionary<string, int> dictionary5 = dictionary4 = dictionary;
						string key;
						string key3 = key = name2;
						int num = dictionary4[key];
						dictionary5[key3] = num + 1;
					}
				}
				break;
			}
			default:
			{
				int value;
				if (!dictionary.TryGetValue(name, out value))
				{
					dictionary.Add(name, 1);
					break;
				}
				Dictionary<string, int> dictionary2;
				Dictionary<string, int> dictionary3 = dictionary2 = dictionary;
				string key;
				string key2 = key = name;
				int num = dictionary2[key];
				dictionary3[key2] = num + 1;
				break;
			}
			}
		}
		Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
		string text = "BlockID = " + gameObject.name + "\n";
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			string key = text;
			text = key + item.Key + ":" + item.Value + "\n";
		}
		Debug.Log(text);
		Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
	}
}
