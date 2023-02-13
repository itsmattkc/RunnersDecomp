using Message;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeObserver : MonoBehaviour
{
	public enum IAPResult
	{
		ProductsRequestCompleted,
		ProductsRequestInvalid,
		PaymentTransactionPurchasing,
		PaymentTransactionPurchased,
		PaymentTransactionFailed,
		PaymentTransactionRestored
	}

	public enum FailStatus
	{
		Disable,
		AppStoreFailed,
		ServerFailed,
		Deferred
	}

	public delegate void IAPDelegate(IAPResult result);

	public delegate void PurchaseSuccessCallback(int scValue);

	public delegate void PurchaseFailedCallback(FailStatus status);

	private IAPDelegate iapDelegate;

	private bool isIAPurchasing;

	private PurchaseSuccessCallback purchaseSuccessCallback;

	private PurchaseFailedCallback purchaseFailedCallback;

	private Action purchaseCancelCallback;

	private bool initialized;

	private bool productInfoEnable;

	private string consumingProductId = string.Empty;

	private string m_processingReceipt = string.Empty;

	private static NativeObserver instance;

	public static int ProductCount
	{
		get
		{
			return ServerInterface.RedStarItemList.Count;
		}
	}

	public static NativeObserver Instance
	{
		get
		{
			if (instance != null && !instance.initialized)
			{
				instance.StartInAppPurchase();
			}
			return instance;
		}
		private set
		{
		}
	}

	public static bool IsBusy
	{
		get;
		set;
	}

	public string GetProductName(int productIndex)
	{
		if (productIndex >= ProductCount)
		{
			return null;
		}
		return ServerInterface.RedStarItemList[productIndex].m_productId;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public string GetProductPrice(string productId)
	{
		return Binding.Instance.GetProductInfoPrice(productId);
	}

	public void StartInAppPurchase()
	{
		Binding.Instance.CreateInAppPurchase("Observer");
		initialized = true;
	}

	public void CheckCurrentTransaction()
	{
		Log("NativeObserver : CheckCurrentTransaction");
		Binding.Instance.FinishTransaction(string.Empty);
		string[] array = ReciptGet();
		if (array != null)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text != null)
				{
					Debug.Log("receipts=" + text);
				}
			}
		}
		if (array != null && array.Length > 0)
		{
			_StartBuyFlow(array[0]);
		}
	}

	public void RequestProductsInfo(List<string> productList, IAPDelegate del)
	{
		if (productInfoEnable)
		{
			if (del != null)
			{
				del(IAPResult.ProductsRequestCompleted);
			}
			return;
		}
		iapDelegate = del;
		if (isIAPurchasing)
		{
			if (iapDelegate != null)
			{
				iapDelegate(IAPResult.PaymentTransactionFailed);
			}
		}
		else
		{
			StartCoroutine(GetPriceAsync(productList));
		}
	}

	public void ResetIapDelegate()
	{
		iapDelegate = null;
	}

	public void BuyProduct(string productId, PurchaseSuccessCallback successCallback, PurchaseFailedCallback failCallback, Action cancelCallback)
	{
		if (isIAPurchasing)
		{
			if (failCallback != null)
			{
				failCallback(FailStatus.Disable);
			}
			return;
		}
		NetMonitor netMonitor = NetMonitor.Instance;
		if (netMonitor != null)
		{
			netMonitor.StartMonitor(null);
		}
		Binding.Instance.BuyProduct(productId);
		purchaseSuccessCallback = successCallback;
		purchaseFailedCallback = failCallback;
		purchaseCancelCallback = cancelCallback;
	}

	public void OnBeforePurchaseFinishedSuccess(string message)
	{
		Log("NativeObserver : OnBeforePurchaseFinishedSuccess :" + message);
		ReciptPush(message);
	}

	public void OnPurchaseFinishedSuccess(string message)
	{
		Log("NativeObserver : OnPurchaseFinishedSuccess :" + message);
		NetMonitor netMonitor = NetMonitor.Instance;
		if (netMonitor != null)
		{
			netMonitor.EndMonitorForward(null, null, null);
			netMonitor.EndMonitorBackward();
		}
		_StartBuyFlow(message);
	}

	private void _StartBuyFlow(string message)
	{
		string[] array = message.Split(',');
		if (array == null || array.Length < 3)
		{
			Debug.Log("NativeObserver._StartBuyFlow:no Reciept or invalid Reciept");
			if (purchaseCancelCallback != null)
			{
				purchaseCancelCallback();
			}
			ClearCallback();
			OnStopBusy();
		}
		else
		{
			Debug.Log("NativeObserver._StartBuyFlow:Retry send reciept");
			ExecCpChargeBuyCommit(array[0], WWW.UnEscapeURL(array[1]), WWW.UnEscapeURL(array[2]), message);
			OnStopBusy();
		}
	}

	public void OnPurchaseFinishedCancel(string message)
	{
		Log("NativeObserver : OnPurchaseFinishedCancel :" + message);
		if (purchaseCancelCallback != null)
		{
			purchaseCancelCallback();
		}
		ClearCallback();
		OnStopBusy();
		NetMonitor netMonitor = NetMonitor.Instance;
		if (netMonitor != null)
		{
			netMonitor.EndMonitorForward(null, null, null);
			netMonitor.EndMonitorBackward();
		}
	}

	public void OnPurchaseFinishedFailed(string message)
	{
		Log("NativeObserver : OnPurchaseFinishedFailed :" + message);
		if (purchaseFailedCallback != null)
		{
			purchaseFailedCallback(FailStatus.AppStoreFailed);
		}
		ClearCallback();
		OnStopBusy();
		NetMonitor netMonitor = NetMonitor.Instance;
		if (netMonitor != null)
		{
			netMonitor.EndMonitorForward(null, null, null);
			netMonitor.EndMonitorBackward();
		}
	}

	private void ExecCpChargeBuyCommit(string productId, string json, string sign, string message)
	{
		Log(string.Format("NativeObserver : ExecCpChargeBuyCommit :{0} , {1}, {2}", productId, json, sign));
		m_processingReceipt = message;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			consumingProductId = productId;
			loggedInServerInterface.RequestServerBuyAndroid(json, sign, base.gameObject);
		}
	}

	private void OnStartBusy()
	{
		Debug.Log("NativeObserver : StartBusy");
		isIAPurchasing = true;
	}

	private void OnStopBusy()
	{
		Debug.Log("NativeObserver : StopBusy");
		isIAPurchasing = false;
	}

	private void Log(string log)
	{
		Debug.Log(log);
	}

	private void ClearCallback()
	{
		purchaseSuccessCallback = null;
		purchaseFailedCallback = null;
		purchaseCancelCallback = null;
	}

	private void ReciptPush(string recipt)
	{
		SystemSaveManager systemSaveManager = SystemSaveManager.Instance;
		if (!(systemSaveManager != null))
		{
			return;
		}
		SystemData systemdata = systemSaveManager.GetSystemdata();
		if (systemdata != null)
		{
			string purchasedRecipt = systemdata.purchasedRecipt;
			if (string.IsNullOrEmpty(purchasedRecipt))
			{
				systemdata.purchasedRecipt = recipt;
			}
			else
			{
				systemdata.purchasedRecipt = purchasedRecipt + "@" + recipt;
			}
			systemSaveManager.SaveSystemData();
			Debug.Log("NativeObserver.ReciptPush:" + systemdata.purchasedRecipt);
		}
	}

	private string[] ReciptGet()
	{
		SystemSaveManager systemSaveManager = SystemSaveManager.Instance;
		if (systemSaveManager != null)
		{
			SystemData systemdata = systemSaveManager.GetSystemdata();
			if (systemdata != null)
			{
				string purchasedRecipt = systemdata.purchasedRecipt;
				Debug.Log("NativeObserver ReciptGet: " + purchasedRecipt);
				if (!string.IsNullOrEmpty(purchasedRecipt))
				{
					return purchasedRecipt.Split('@');
				}
			}
		}
		return null;
	}

	private void ReciptDelete(string recipt)
	{
		SystemSaveManager systemSaveManager = SystemSaveManager.Instance;
		if (!(systemSaveManager != null))
		{
			return;
		}
		SystemData systemdata = systemSaveManager.GetSystemdata();
		if (systemdata == null)
		{
			return;
		}
		string purchasedRecipt = systemdata.purchasedRecipt;
		if (string.IsNullOrEmpty(purchasedRecipt))
		{
			return;
		}
		string[] array = purchasedRecipt.Split('@');
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == recipt))
			{
				if (text.Length > 0)
				{
					text += "@";
				}
				text += array[i];
			}
		}
		if (text.Length > 0)
		{
			systemdata.purchasedRecipt = text;
			systemSaveManager.SaveSystemData();
		}
		else
		{
			systemdata.purchasedRecipt = text;
			systemSaveManager.SaveSystemData();
		}
	}

	private IEnumerator GetPriceAsync(List<string> productList)
	{
		for (int index = 0; index < productList.Count; index++)
		{
			string productName = productList[index];
			if (productName == null)
			{
				continue;
			}
			DateTime startTime = DateTime.Now;
			while (true)
			{
				string price = GetProductPrice(productName);
				if (!string.IsNullOrEmpty(price))
				{
					break;
				}
				DateTime currentTime = DateTime.Now;
				if ((float)(currentTime - startTime).Seconds >= 10f)
				{
					break;
				}
				yield return null;
			}
		}
		productInfoEnable = true;
		if (iapDelegate != null)
		{
			iapDelegate(IAPResult.ProductsRequestCompleted);
		}
	}

	private void ServerBuyAndroid_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		ReciptDelete(m_processingReceipt);
		int rsrType = 0;
		for (int i = 0; i < ProductCount; i++)
		{
			string productName = GetProductName(i);
			if (productName == consumingProductId)
			{
				rsrType = i;
				break;
			}
		}
		consumingProductId = string.Empty;
		if (purchaseSuccessCallback != null)
		{
			purchaseSuccessCallback(0);
		}
	}

	private void ServerBuyAndroid_Failed(MsgServerConnctFailed msg)
	{
		if (msg.m_status == ServerInterface.StatusCode.AlreadyProcessedReceipt)
		{
			int rsrType = 0;
			for (int i = 0; i < ProductCount; i++)
			{
				string productName = GetProductName(i);
				if (productName == consumingProductId)
				{
					rsrType = i;
					break;
				}
			}
			ReciptDelete(m_processingReceipt);
		}
		if (purchaseFailedCallback != null)
		{
			purchaseFailedCallback(FailStatus.ServerFailed);
		}
	}
}
