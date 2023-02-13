using System.Collections;
using Text;
using UnityEngine;

public class ItemWindow : MonoBehaviour
{
	public delegate void ItemBuyCallback(ItemType itemType);

	private GameObject m_instantItemObject;

	private GameObject m_itemObject;

	private ItemType m_itemType;

	private ItemBuyCallback m_callback;

	private int m_FreeCount;

	public void SetItemBuyCallback(ItemBuyCallback callback)
	{
		m_callback = callback;
	}

	public void SetWindowActive()
	{
		if (m_instantItemObject != null && m_instantItemObject.activeSelf)
		{
			m_instantItemObject.SetActive(false);
		}
		if (m_itemObject != null && !m_itemObject.activeSelf)
		{
			m_itemObject.SetActive(true);
		}
	}

	public void SetItemType(ItemType itemType)
	{
		m_itemType = itemType;
		m_FreeCount = ItemSetUtility.GetFreeItemNum(m_itemType);
		UpdateView();
	}

	public void UpdateView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_itemObject, "row_0");
		if (gameObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_item_icon");
			if (uISprite != null)
			{
				int itemType = (int)m_itemType;
				uISprite.spriteName = "ui_cmn_icon_item_" + itemType;
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject.gameObject, "item_stock");
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject.gameObject, "img_use_ring_bg");
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject.gameObject, "img_free_bg");
			int itemNum = ItemSetUtility.GetItemNum(m_itemType);
			if (gameObject2 != null && gameObject3 != null && gameObject4 != null)
			{
				if (m_FreeCount > 0)
				{
					gameObject4.SetActive(true);
					gameObject2.SetActive(false);
					gameObject3.SetActive(false);
					UpdateFreeCount(m_FreeCount);
				}
				else if (itemNum > 0)
				{
					gameObject4.SetActive(false);
					gameObject2.SetActive(true);
					gameObject3.SetActive(false);
					UpdateItemCount();
				}
				else
				{
					gameObject4.SetActive(false);
					gameObject2.SetActive(false);
					gameObject3.SetActive(true);
					UpdateCampaignView();
				}
			}
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_item_name");
			if (uILabel != null)
			{
				string cellName = "name" + (int)(m_itemType + 1);
				string text2 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", cellName).text;
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uILabel.gameObject, "Lbl_item_name_sdw");
				if (uILabel2 != null)
				{
					uILabel2.text = text2;
				}
			}
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_itemObject, "Lbl_item_info");
		if (uILabel3 != null)
		{
			int num = 0;
			AbilityType abilityType = StageItemManager.s_dicItemTypeToCharAbilityType[m_itemType];
			SaveDataManager instance = SaveDataManager.Instance;
			CharaType mainChara = instance.PlayerData.MainChara;
			CharaAbility charaAbility = instance.CharaData.AbilityArray[(int)mainChara];
			num = (int)(((uint)abilityType < charaAbility.Ability.Length) ? charaAbility.Ability[(int)abilityType] : 0);
			float itemTimeFromChara = StageItemManager.GetItemTimeFromChara(m_itemType);
			string cellName2 = "details" + (int)(m_itemType + 1);
			TextObject text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", cellName2);
			text3.ReplaceTag("{LEVEL}", num.ToString());
			text3.ReplaceTag("{TIME}", itemTimeFromChara.ToString("0.0"));
			uILabel3.text = text3.text;
		}
	}

	private void UpdateCampaignView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_itemObject, "row_0");
		if (gameObject == null)
		{
			return;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_ring_number");
		if (uILabel != null)
		{
			string text = uILabel.text = ItemSetUtility.GetOneItemCostString(m_itemType);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "img_sale_icon");
		if (gameObject2 != null)
		{
			ServerCampaignData campaignDataInSession = ItemSetUtility.GetCampaignDataInSession((int)new ServerItem(m_itemType).id);
			bool active = false;
			if (m_FreeCount == 0 && campaignDataInSession != null)
			{
				active = true;
			}
			gameObject2.SetActive(active);
		}
	}

	private void UpdateItemCount()
	{
		int itemNum = ItemSetUtility.GetItemNum(m_itemType);
		GameObject parent = GameObjectUtil.FindChildGameObject(m_itemObject, "row_0");
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_number");
		if (uILabel != null)
		{
			uILabel.text = itemNum.ToString();
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_number_sdw");
			if (uILabel2 != null)
			{
				uILabel2.text = itemNum.ToString();
			}
		}
	}

	private void UpdateFreeCount(int value)
	{
		GameObject parent = GameObjectUtil.FindChildGameObject(m_itemObject, "row_0");
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_free_number");
		if (uILabel != null)
		{
			uILabel.text = value.ToString();
		}
	}

	public void SetEquipMark(bool isEquip)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_itemObject, "img_cursor");
		if (gameObject != null)
		{
			gameObject.SetActive(isEquip);
		}
	}

	public void SetEquipMarkColor(ItemButton.CursorColor cursorColor)
	{
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_itemObject, "img_cursor");
		int num = (int)cursorColor;
		uISprite.spriteName = "ui_itemset_2_cursor_" + num;
	}

	private void Start()
	{
		m_instantItemObject = GameObjectUtil.FindChildGameObject(base.gameObject, "boost_info_pla");
		m_itemObject = GameObjectUtil.FindChildGameObject(base.gameObject, "item_info_pla");
		StartCoroutine(SetupUIRectItemStorage());
	}

	private void Update()
	{
		UpdateCampaignView();
	}

	private IEnumerator SetupUIRectItemStorage()
	{
		if (m_itemObject != null)
		{
			m_itemObject.SetActive(true);
			yield return null;
			m_itemObject.SetActive(false);
		}
		yield return null;
	}

	private void BuyCompleteCallback(ItemType itemType)
	{
		UpdateItemCount();
		m_callback(itemType);
	}

	private void BuyCancelledCallback(ItemType itemType)
	{
	}
}
