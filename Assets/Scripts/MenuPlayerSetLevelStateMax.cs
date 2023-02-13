using Text;
using UnityEngine;

public class MenuPlayerSetLevelStateMax : MenuPlayerSetLevelState
{
	private void Start()
	{
	}

	public override void ChangeLabels()
	{
		int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_lv");
		if (uILabel != null)
		{
			uILabel.text = TextUtility.GetTextLevel(level.ToString());
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_item_price");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_word_max");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(true);
		}
	}

	private void LateUpdate()
	{
	}
}
