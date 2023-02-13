using UnityEngine;

public class ItemGetWindowUtil
{
	public static ItemGetWindow GetItemGetWindow()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "item_get_Window");
			if (gameObject2 != null)
			{
				return gameObject2.GetComponent<ItemGetWindow>();
			}
		}
		return null;
	}
}
