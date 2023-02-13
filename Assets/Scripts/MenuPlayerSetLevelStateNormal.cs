using Text;
using UnityEngine;

public class MenuPlayerSetLevelStateNormal : MenuPlayerSetLevelState
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
			gameObject.SetActive(true);
			UILabel component = gameObject.GetComponent<UILabel>();
			component.text = ((float)MenuPlayerSetUtil.GetAbilityCost(m_params.Character)).ToString();
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_word_max");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(false);
		}
	}

	private void LateUpdate()
	{
	}
}
