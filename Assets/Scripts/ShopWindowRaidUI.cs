using AnimationOrTween;
using Message;
using UnityEngine;

public class ShopWindowRaidUI : MonoBehaviour
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
		if (GeneralWindow.IsCreated("EventEndError") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.SHOP_BACK);
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
		Animation animation = GameObjectUtil.FindGameObjectComponent<Animation>("shop_raid_window");
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
		m_productIndex = -1;
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
			if (m_presentLabel != null)
			{
				m_presentLabel.text = m_exchangeItem.m_bonusLabels[0].text;
			}
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

	private void OnClickBuyRaidEnergy()
	{
		if (!ShopUI.isRaidbossEvent())
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "EventEndError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = ShopUI.GetText("gw_raid_count_error_caption");
			info.message = ShopUI.GetText("gw_raid_count_error_text");
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
			return;
		}
		SoundManager.SePlay("sys_window_close");
		if (SaveDataManager.Instance.ItemData.RedRingCount < m_exchangeItem.m_storeItem.m_price)
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "RingShortError";
			info2.buttonType = GeneralWindow.ButtonType.YesNo;
			info2.caption = ShopUI.GetText("gw_rsring_short_error_caption");
			info2.message = ShopUI.GetText("gw_rsring_short_error_text");
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
		}
		else if ((long)EventManager.Instance.RaidbossChallengeCount >= 99999L || 99999L - (long)EventManager.Instance.RaidbossChallengeCount < m_exchangeItem.quantity)
		{
			GeneralWindow.CInfo info3 = default(GeneralWindow.CInfo);
			info3.name = "ChallengeCountError";
			info3.buttonType = GeneralWindow.ButtonType.Ok;
			info3.caption = ShopUI.GetText("gw_challenge_count_error_caption");
			info3.message = ShopUI.GetText("gw_challenge_count_error_text");
			info3.isPlayErrorSe = true;
			GeneralWindow.Create(info3);
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
			if (msg != null)
			{
				msg.StaySequence();
			}
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_close");
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
	}
}
