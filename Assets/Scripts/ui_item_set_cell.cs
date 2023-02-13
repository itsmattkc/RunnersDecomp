using AnimationOrTween;
using UnityEngine;

public class ui_item_set_cell : MonoBehaviour
{
	private int m_id;

	private int m_count;

	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	public void UpdateView(ItemType itemType, int count)
	{
		UpdateView((int)itemType, count);
	}

	public void UpdateView(int id, int count)
	{
		m_id = id;
		m_count = count;
		if (id == -1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_volume");
		if (uILabel != null)
		{
			uILabel.text = count.ToString();
		}
		bool flag = base.gameObject.transform.parent.name == "slot" || base.gameObject.transform.parent.name == "slot_equip";
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_item");
		if (uISprite != null)
		{
			uISprite.spriteName = "ui_cmn_icon_item_" + ((count != 0 || !flag) ? string.Empty : "g_") + id;
		}
	}

	private void OnClick()
	{
		switch (base.gameObject.transform.parent.name)
		{
		case "slot":
		{
			SoundManager.SePlay("sys_menu_decide");
			Animation animation = GameObjectUtil.FindGameObjectComponent<Animation>("menu_Anim");
			if (animation != null)
			{
				ActiveAnimation.Play(animation, "ui_menu_item_Anim", Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
			}
			break;
		}
		case "slot_equip":
		case "slot_item":
		{
			ItemSetWindowEquipUI itemSetWindowEquipUI = GameObjectUtil.FindChildGameObjectComponent<ItemSetWindowEquipUI>(base.gameObject.transform.root.gameObject, "ItemSetWindowEquipUI");
			if (itemSetWindowEquipUI != null)
			{
				itemSetWindowEquipUI.gameObject.SetActive(true);
				itemSetWindowEquipUI.OpenWindow(m_id, m_count);
			}
			break;
		}
		}
	}
}
