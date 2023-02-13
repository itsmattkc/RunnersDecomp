using AnimationOrTween;
using Message;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShopWindowRsringUI : MonoBehaviour
{
	public delegate void PurchaseEndCallback(bool isSuccess);

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

	[SerializeField]
	private GameObject m_blinderGameObject;

	private int m_productIndex;

	private ShopUI.EexchangeItem m_exchangeItem;

	private AgeVerification m_ageVerification;

	private GameObject m_windowObject;

	private PurchaseEndCallback m_callback;

	[SerializeField]
	public UILabel m_presentLabel;

	private void Start()
	{
		if (m_blinderGameObject == null)
		{
			m_blinderGameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "blinder");
		}
		m_windowObject = GameObjectUtil.FindChildGameObject(base.gameObject, "window");
	}

	private void Update()
	{
		UpdateView();
		if (m_ageVerification != null)
		{
			if (m_ageVerification.IsEnd)
			{
				m_ageVerification = null;
				OpenWindow(m_productIndex, m_exchangeItem, m_callback);
			}
		}
		else if (GeneralWindow.IsCreated("RsringCountError") || GeneralWindow.IsCreated("PurchaseFailed") || GeneralWindow.IsCreated("PurchaseCanceled") || GeneralWindow.IsCreated("AgeVerificationError"))
		{
			if (GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
			}
		}
		else
		{
			ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow != null && itemGetWindow.IsCreated("PurchaseSuccess") && itemGetWindow.IsEnd)
			{
				itemGetWindow.Reset();
			}
		}
	}

	public void OpenWindow(int productIndex, ShopUI.EexchangeItem exchangeItem, PurchaseEndCallback callback)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		m_productIndex = productIndex;
		m_exchangeItem = exchangeItem;
		m_callback = callback;
		if (m_windowObject != null)
		{
			m_windowObject.SetActive(true);
		}
		SoundManager.SePlay("sys_window_open");
		UpdateView();
		Animation animation = GameObjectUtil.FindGameObjectComponent<Animation>("shop_rsring_window");
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

	private void OnClickOk()
	{
		if (9999999 - SaveDataManager.Instance.ItemData.RedRingCount < m_exchangeItem.quantity)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RsringCountError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = ShopUI.GetText("gw_rsring_count_error_caption");
			info.message = ShopUI.GetText("gw_rsring_count_error_text");
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
		}
		else if (!CheckPurchaseOver(HudUtility.GetMixedStringToInt(m_exchangeItem.m_costLabel.text)))
		{
			SoundManager.SePlay("sys_menu_decide");
			m_blinderGameObject.SetActive(true);
			NativeObserver instance = NativeObserver.Instance;
			if (((ServerInterface.LoggedInServerInterface != null) ? true : false) && instance != null)
			{
				instance.BuyProduct(instance.GetProductName(m_productIndex), PurchaseSuccessCallback, PurchaseFailedCallback, PurchaseCanceledCallback);
			}
			else
			{
				StartCoroutine(PurchaseCallbackEmulateCoroutine(Random.Range(0, 3)));
			}
		}
	}

	private IEnumerator PurchaseCallbackEmulateCoroutine(int recultCode)
	{
		yield return new WaitForSeconds(1f);
		switch (recultCode)
		{
		case 0:
			if (ServerInterface.LoggedInServerInterface == null)
			{
				SaveDataManager.Instance.ItemData.RedRingCount += (uint)m_exchangeItem.quantity;
			}
			PurchaseSuccessCallback(0);
			break;
		case 1:
			PurchaseFailedCallback(NativeObserver.FailStatus.Deferred);
			break;
		case 2:
			PurchaseCanceledCallback();
			break;
		}
	}

	private bool CheckPurchaseOver(int addPurchase)
	{
		ServerSettingState serverSettingState = null;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			serverSettingState = ServerInterface.SettingState;
		}
		else
		{
			serverSettingState = new ServerSettingState();
			serverSettingState.m_monthPurchase = 1000;
			serverSettingState.m_birthday = string.Empty;
			serverSettingState.m_birthday = "1990-9-30";
		}
		if (string.IsNullOrEmpty(serverSettingState.m_birthday))
		{
			m_ageVerification = base.gameObject.GetComponent<AgeVerification>();
			if (m_ageVerification == null)
			{
				m_ageVerification = base.gameObject.AddComponent<AgeVerification>();
			}
			m_ageVerification.Setup("Camera/menu_Anim/ShopPage/" + base.gameObject.name + "/Anchor_5_MC");
			m_ageVerification.PlayStart();
			return true;
		}
		if (HudUtility.CheckPurchaseOver(serverSettingState.m_birthday, serverSettingState.m_monthPurchase, addPurchase))
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "AgeVerificationError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = ShopUI.GetText("gw_age_verification_error_caption");
			info.message = ShopUI.GetText("gw_age_verification_error_text", new Dictionary<string, string>
			{
				{
					"{PURCHASE}",
					HudUtility.GetFormatNumString(serverSettingState.m_monthPurchase)
				}
			});
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
			return true;
		}
		return false;
	}

	private void PurchaseSuccessCallback(int scValue)
	{
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
		if (itemGetWindow != null)
		{
			itemGetWindow.Create(new ItemGetWindow.CInfo
			{
				name = "PurchaseSuccess",
				caption = ShopUI.GetText("gw_purchase_success_caption"),
				serverItemId = m_exchangeItem.m_storeItem.m_itemId,
				imageCount = ShopUI.GetText("gw_purchase_success_text", new Dictionary<string, string>
				{
					{
						"{COUNT}",
						HudUtility.GetFormatNumString(m_exchangeItem.quantity)
					}
				})
			});
		}
		SoundManager.SePlay("sys_buy_real_money");
		int num = 0;
		if (m_exchangeItem != null && m_exchangeItem.m_storeItem != null)
		{
			num = m_exchangeItem.m_storeItem.m_price;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetRedStarExchangeList(0, base.gameObject);
		}
		else
		{
			ServerGetRedStarExchangeList_Succeeded(null);
		}
	}

	private void PurchaseFailedCallback(NativeObserver.FailStatus status)
	{
		string cellName = "gw_purchase_failed_caption";
		string cellName2 = "gw_purchase_failed_text";
		if (status == NativeObserver.FailStatus.Deferred)
		{
			cellName = "gw_purchase_deferred_caption";
			cellName2 = "gw_purchase_deferred_text";
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "PurchaseFailed";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = ShopUI.GetText(cellName);
		info.message = ShopUI.GetText(cellName2);
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
		m_blinderGameObject.SetActive(false);
		if (m_callback != null)
		{
			m_callback(false);
			m_callback = null;
		}
	}

	private void PurchaseCanceledCallback()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "PurchaseCanceled";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = ShopUI.GetText("gw_purchase_canceled_caption");
		info.message = ShopUI.GetText("gw_purchase_canceled_text");
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
		m_blinderGameObject.SetActive(false);
		if (m_callback != null)
		{
			m_callback(false);
			m_callback = null;
		}
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
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

	private void ServerGetRedStarExchangeList_Succeeded(MsgGetRedStarExchangeListSucceed msg)
	{
		m_blinderGameObject.SetActive(false);
		if (m_callback != null)
		{
			m_callback(true);
			m_callback = null;
		}
	}
}
