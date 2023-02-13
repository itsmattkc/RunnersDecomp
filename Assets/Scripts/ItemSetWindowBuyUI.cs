using Message;
using Text;
using UnityEngine;

public class ItemSetWindowBuyUI : MonoBehaviour
{
	private int m_id = -1;

	private int m_count;

	private ShopItemData m_shopItemData;

	private bool m_isUnsetTriggerOfPopupList;

	private int buyItemCount
	{
		get
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_buy_volume");
			if (uILabel != null)
			{
				return int.Parse(uILabel.text);
			}
			return 0;
		}
	}

	private int buyNeedRingCount
	{
		get
		{
			if (m_shopItemData != null)
			{
				return m_shopItemData.rings * buyItemCount;
			}
			return 0;
		}
	}

	private void Start()
	{
		m_id = -1;
	}

	private void Update()
	{
		if (m_isUnsetTriggerOfPopupList)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Drop-down List");
			if (gameObject != null)
			{
				foreach (BoxCollider item in GameObjectUtil.FindChildGameObjectsComponents<BoxCollider>(gameObject, "Label"))
				{
					item.isTrigger = false;
				}
			}
			m_isUnsetTriggerOfPopupList = false;
		}
		if (GeneralWindow.IsCreated("ItemBuyOverError") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			OpenWindow(m_id, m_count);
		}
		if (GeneralWindow.IsCreated("ItemBuyRingError") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			if (GeneralWindow.IsYesButtonPressed)
			{
				GameObjectUtil.SendMessageFindGameObject("ItemSetUI", "OnClose", "ShopUI.OnOpenRing", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void OpenWindow(int id, int count)
	{
		m_id = id;
		m_count = count;
		m_shopItemData = ShopItemTable.GetShopItemData(id);
		SoundManager.SePlay("sys_window_open");
		UIPopupList uIPopupList = GameObjectUtil.FindChildGameObjectComponent<UIPopupList>(base.gameObject, "Ppl_buy_volume");
		if (uIPopupList != null)
		{
			uIPopupList.value = uIPopupList.items[0];
		}
		UpdateView();
	}

	private void UpdateView()
	{
		if (m_id == -1)
		{
			return;
		}
		if (m_shopItemData != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_name");
			if (uILabel != null)
			{
				uILabel.text = m_shopItemData.name;
			}
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbt_item_effect");
			if (uILabel2 != null)
			{
				uILabel2.text = ItemSetWindowEquipUI.GetItemDetailsText(m_shopItemData);
			}
			UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_price");
			if (uILabel3 != null)
			{
				uILabel3.text = HudUtility.GetFormatNumString(m_shopItemData.rings);
			}
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_volume");
		if (uILabel4 != null)
		{
			uILabel4.text = HudUtility.GetFormatNumString(m_count);
		}
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_total_price");
		if (uILabel5 != null)
		{
			uILabel5.text = HudUtility.GetFormatNumString(buyNeedRingCount);
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_item");
		if (uISprite != null)
		{
			uISprite.spriteName = "ui_cmn_icon_item_" + m_id;
		}
	}

	private void OnClickBuy()
	{
		int itemCount = (int)SaveDataManager.Instance.ItemData.GetItemCount((ItemType)m_id);
		if ((long)itemCount >= 99L || 99L - (long)itemCount < buyItemCount)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "ItemBuyOverError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_over_error_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_over_error_text").text;
			info.parentGameObject = base.gameObject;
			GeneralWindow.Create(info);
			return;
		}
		if (SaveDataManager.Instance.ItemData.RingCount < buyNeedRingCount)
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "ItemBuyRingError";
			info2.buttonType = GeneralWindow.ButtonType.ShopCancel;
			info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_ring__error_caption").text;
			info2.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_ring__error_text").text;
			info2.parentGameObject = base.gameObject;
			GeneralWindow.Create(info2);
			return;
		}
		SoundManager.SePlay("sys_menu_decide");
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerRingExchange((int)new ServerItem((ItemType)m_id).id, buyItemCount, base.gameObject);
			return;
		}
		SoundManager.SePlay("sys_buy");
		SaveDataManager.Instance.ItemData.RingCount -= (uint)buyNeedRingCount;
		SaveDataManager.Instance.ItemData.SetItemCount((ItemType)m_id, SaveDataManager.Instance.ItemData.GetItemCount((ItemType)m_id) + (uint)buyItemCount);
		UpdateItemSetUIView();
	}

	private void ServerRingExchange_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		SoundManager.SePlay("sys_buy");
		UpdateItemSetUIView();
	}

	public static void UpdateItemSetUIView()
	{
		ItemSetUI itemSetUI = GameObjectUtil.FindGameObjectComponent<ItemSetUI>("ItemSetUI");
		if (itemSetUI != null)
		{
			itemSetUI.UpdateView();
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void OnClickClose()
	{
		SoundManager.SePlay("sys_window_close");
	}

	public void OnValueChangePopupList()
	{
		UpdateView();
	}

	public void OnClickPopupList()
	{
		m_isUnsetTriggerOfPopupList = true;
	}
}
