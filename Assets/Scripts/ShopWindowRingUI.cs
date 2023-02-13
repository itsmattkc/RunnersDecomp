using AnimationOrTween;
using Message;
using Text;
using UnityEngine;

public class ShopWindowRingUI : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_itemsGameObject = new GameObject[5];

	[SerializeField]
	private GameObject m_panelGameObject;

	[SerializeField]
	public UILabel m_quantityLabel;

	[SerializeField]
	public UILabel m_costLabel;

	[SerializeField]
	public GameObject m_bonusGameObject;

	[SerializeField]
	public UILabel[] m_bonusLabels = new UILabel[2];

	[SerializeField]
	public UISprite m_saleSprite;

	private int m_productIndex;

	private ShopUI.EexchangeItem m_exchangeItem;

	private GameObject m_windowObject;

	[SerializeField]
	public UILabel m_presentLabel;

	private void Start()
	{
		m_windowObject = GameObjectUtil.FindChildGameObject(base.gameObject, "window");
	}

	private void Update()
	{
		UpdateView();
		if (GeneralWindow.IsCreated("RingShortError") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, "Btn_tab_rsring");
				if (uIToggle != null)
				{
					uIToggle.value = true;
				}
			}
			GeneralWindow.Close();
		}
		if (GeneralWindow.IsCreated("RingCountError") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
		}
	}

	public void OpenWindow(int productIndex, ShopUI.EexchangeItem exchangeItem)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		m_productIndex = productIndex;
		m_exchangeItem = exchangeItem;
		if (m_windowObject != null)
		{
			m_windowObject.SetActive(true);
		}
		SoundManager.SePlay("sys_window_open");
		UpdateView();
		Animation animation = GameObjectUtil.FindGameObjectComponent<Animation>("shop_ring_window");
		if (animation != null)
		{
			ActiveAnimation.Play(animation, "ui_shop_window_Anim", Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
		}
	}

	private void OnClickClose()
	{
		SoundManager.SePlay("sys_window_close");
	}

	public void OnFinishedCloseAnim()
	{
		for (int i = 0; i < m_itemsGameObject.Length; i++)
		{
			m_itemsGameObject[i].SetActive(false);
		}
		m_panelGameObject.SetActive(false);
		if (m_windowObject != null)
		{
			m_windowObject.SetActive(false);
		}
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	private void UpdateView()
	{
		if (m_exchangeItem == null)
		{
			return;
		}
		for (int i = 0; i < m_itemsGameObject.Length; i++)
		{
			m_itemsGameObject[i].SetActive(i == m_productIndex);
		}
		m_panelGameObject.SetActive(true);
		m_quantityLabel.text = m_exchangeItem.m_quantityLabel.text;
		m_costLabel.text = m_exchangeItem.m_costLabel.text;
		m_bonusGameObject.SetActive(m_exchangeItem.m_bonusGameObject != null && m_exchangeItem.m_bonusGameObject.activeSelf);
		if (m_exchangeItem.m_bonusGameObject != null)
		{
			for (int j = 0; j < m_bonusLabels.Length; j++)
			{
				m_bonusLabels[j].text = m_exchangeItem.m_bonusLabels[j].text;
			}
			m_presentLabel.text = m_exchangeItem.m_bonusLabels[0].text;
		}
		m_saleSprite.gameObject.SetActive(m_exchangeItem.m_saleSprite.gameObject.activeSelf);
		GameObject gameObject = ShopUI.CalcSaleIconObject(m_exchangeItem.m_saleSprite.gameObject, m_productIndex);
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_saleSprite.gameObject, "img_sale_icon");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(gameObject.activeSelf);
			}
		}
	}

	private void OnClickBuyRing()
	{
		SoundManager.SePlay("sys_window_close");
		if (SaveDataManager.Instance.ItemData.RedRingCount < m_exchangeItem.m_storeItem.m_price)
		{
			bool flag = ServerInterface.IsRSREnable();
			GeneralWindow.ButtonType buttonType = (!flag) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.YesNo;
			string message = (!flag) ? TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text_2") : ShopUI.GetText("gw_rsring_short_error_text");
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RingShortError";
			info.buttonType = buttonType;
			info.caption = ShopUI.GetText("gw_rsring_short_error_caption");
			info.message = message;
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
		}
		else if (SaveDataManager.Instance.ItemData.RingCount >= 9999999 || 9999999 - SaveDataManager.Instance.ItemData.RingCount < m_exchangeItem.quantity)
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "RingCountError";
			info2.buttonType = GeneralWindow.ButtonType.Ok;
			info2.caption = ShopUI.GetText("gw_ring_count_error_caption");
			info2.message = ShopUI.GetText("gw_ring_count_error_text");
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
		}
		else
		{
			SoundManager.SePlay("sys_menu_decide");
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerRedStarExchange(m_exchangeItem.m_storeItem.m_storeItemId, base.gameObject);
				return;
			}
			SoundManager.SePlay("sys_buy");
			SaveDataManager.Instance.ItemData.RedRingCount -= (uint)m_exchangeItem.m_storeItem.m_price;
			SaveDataManager.Instance.ItemData.RingCount += (uint)m_exchangeItem.quantity;
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		}
	}

	private void ServerRedStarExchange_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		SoundManager.SePlay("sys_buy");
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (!(m_windowObject == null) && m_windowObject.activeSelf)
		{
			msg.StaySequence();
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_close");
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
	}
}
