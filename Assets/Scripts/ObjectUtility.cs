using System.Collections.Generic;
using UnityEngine;

public class ObjectUtility
{
	public static Dictionary<string, UISprite> GetObjectSprite(GameObject target, List<string> objectName)
	{
		Dictionary<string, UISprite> result = null;
		if (objectName != null && objectName.Count > 0)
		{
			result = new Dictionary<string, UISprite>();
			{
				foreach (string item in objectName)
				{
					UISprite uISprite2 = result[item] = GameObjectUtil.FindChildGameObjectComponent<UISprite>(target, item);
				}
				return result;
			}
		}
		return result;
	}

	public static Dictionary<string, UILabel> GetObjectLabel(GameObject target, List<string> objectName)
	{
		Dictionary<string, UILabel> result = null;
		if (objectName != null && objectName.Count > 0)
		{
			result = new Dictionary<string, UILabel>();
			{
				foreach (string item in objectName)
				{
					UILabel uILabel2 = result[item] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(target, item);
				}
				return result;
			}
		}
		return result;
	}

	public static Dictionary<string, UITexture> GetObjectTexture(GameObject target, List<string> objectName)
	{
		Dictionary<string, UITexture> result = null;
		if (objectName != null && objectName.Count > 0)
		{
			result = new Dictionary<string, UITexture>();
			{
				foreach (string item in objectName)
				{
					UITexture uITexture2 = result[item] = GameObjectUtil.FindChildGameObjectComponent<UITexture>(target, item);
				}
				return result;
			}
		}
		return result;
	}

	public static Dictionary<string, GameObject> GetGameObject(GameObject target, List<string> objectName)
	{
		Dictionary<string, GameObject> result = null;
		if (objectName != null && objectName.Count > 0)
		{
			result = new Dictionary<string, GameObject>();
			{
				foreach (string item in objectName)
				{
					GameObject gameObject2 = result[item] = GameObjectUtil.FindChildGameObject(target, item);
				}
				return result;
			}
		}
		return result;
	}

	public static string SetColorString(string org, int r, int g, int b)
	{
		string result = org;
		string text = "[000000]";
		if (r < 0)
		{
			r = 0;
		}
		else if (r > 255)
		{
			r = 255;
		}
		if (g < 0)
		{
			g = 0;
		}
		else if (g > 255)
		{
			g = 255;
		}
		if (b < 0)
		{
			b = 0;
		}
		else if (b > 255)
		{
			b = 255;
		}
		if (r >= 0 && g >= 0 && b >= 0 && r <= 255 && g <= 255 && b <= 255)
		{
			string colorString = GetColorString(r);
			string colorString2 = GetColorString(g);
			string colorString3 = GetColorString(b);
			text = "[" + colorString + colorString2 + colorString3 + "]";
			result = text + org + "[-]";
		}
		return result;
	}

	private static string GetColorString(int param)
	{
		string text = "00";
		if (param >= 0 && param <= 255)
		{
			text = string.Empty;
			int num = param / 16;
			int num2 = param % 16;
			switch (num)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				text = num.ToString();
				break;
			case 10:
				text = "a";
				break;
			case 11:
				text = "b";
				break;
			case 12:
				text = "c";
				break;
			case 13:
				text = "d";
				break;
			case 14:
				text = "e";
				break;
			case 15:
				text = "f";
				break;
			}
			switch (num2)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				text += num2;
				break;
			case 10:
				text += "a";
				break;
			case 11:
				text += "b";
				break;
			case 12:
				text += "c";
				break;
			case 13:
				text += "d";
				break;
			case 14:
				text += "e";
				break;
			case 15:
				text += "f";
				break;
			}
		}
		return text;
	}
}
