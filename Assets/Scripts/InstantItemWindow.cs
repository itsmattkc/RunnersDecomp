using Text;
using UnityEngine;

public class InstantItemWindow : MonoBehaviour
{
	private BoostItemType m_itemType;

	private GameObject m_instantItemObject;

	private GameObject m_itemObject;

	private int m_FreeCount;

	public void SetWindowActive()
	{
		if (m_instantItemObject != null && !m_instantItemObject.activeSelf)
		{
			m_instantItemObject.SetActive(true);
		}
		if (m_itemObject != null && m_itemObject.activeSelf)
		{
			m_itemObject.SetActive(false);
		}
	}

	public void SetInstantItemType(BoostItemType itemType)
	{
		m_itemType = itemType;
		m_FreeCount = ItemSetUtility.GetFreeBoostItemNum(itemType);
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_instantItemObject, "img_boost_icon");
		if (uISprite != null)
		{
			int itemType2 = (int)m_itemType;
			uISprite.spriteName = "ui_itemset_2_boost_icon_" + itemType2;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_instantItemObject, "Lbl_boost_name");
		if (uILabel != null)
		{
			string cellName = "instant_name" + (int)(m_itemType + 1);
			string text2 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", cellName).text;
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uILabel.gameObject, "Lbl_boost_name_sdw");
			if (uILabel2 != null)
			{
				uILabel2.text = text2;
			}
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_instantItemObject, "Lbl_boost_percent");
		if (uILabel3 != null)
		{
			if (itemType == BoostItemType.SCORE_BONUS)
			{
				uILabel3.gameObject.SetActive(true);
				string text4 = uILabel3.text = "100.0%";
				UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_instantItemObject, "Lbl_boost_percent_sdw");
				if (uILabel4 != null)
				{
					uILabel4.text = text4;
				}
			}
			else
			{
				uILabel3.gameObject.SetActive(false);
			}
		}
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_instantItemObject, "Lbl_item_info");
		if (uILabel5 != null)
		{
			string cellName2 = "instant_details" + (int)(m_itemType + 1);
			uILabel5.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", cellName2).text;
		}
		GameObject parent = GameObjectUtil.FindChildGameObject(m_instantItemObject, "row_0");
		UILabel uILabel6 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_free_number");
		if (uILabel6 != null)
		{
			uILabel6.text = m_FreeCount.ToString();
		}
		UpdateCampaignView();
	}

	public void SetCheckMark(bool isCheck)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_instantItemObject, "img_checkmark");
		if (gameObject != null)
		{
			gameObject.SetActive(isCheck);
		}
	}

	private void UpdateCampaignView()
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_instantItemObject, "Lbl_ring_number");
		if (uILabel != null)
		{
			uILabel.text = ItemSetUtility.GetInstantItemCostString(m_itemType);
		}
		bool flag = false;
		if (m_FreeCount > 0)
		{
			flag = true;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon");
		if (gameObject != null)
		{
			ServerCampaignData campaignDataInSession = ItemSetUtility.GetCampaignDataInSession((int)new ServerItem(m_itemType).id);
			bool active = false;
			if (!flag && campaignDataInSession != null)
			{
				active = true;
			}
			gameObject.SetActive(active);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_instantItemObject, "row_0");
		if (gameObject2 != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2.gameObject, "img_use_ring_bg");
			if (gameObject3 != null)
			{
				gameObject3.SetActive(!flag);
			}
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject2.gameObject, "img_free_bg");
			if (gameObject4 != null)
			{
				gameObject4.SetActive(flag);
			}
		}
	}

	private void Awake()
	{
		m_instantItemObject = GameObjectUtil.FindChildGameObject(base.gameObject, "boost_info_pla");
		m_itemObject = GameObjectUtil.FindChildGameObject(base.gameObject, "item_info_pla");
	}

	private void Update()
	{
		UpdateCampaignView();
	}
}
