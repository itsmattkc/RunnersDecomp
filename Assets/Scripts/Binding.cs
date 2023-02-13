using UnityEngine;

public class Binding
{
	private static BindingPlugin bindingInstance = null;

	public static readonly Binding Instance = new Binding();

	private Binding()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
#if UNITY_ANDROID
			bindingInstance = new BindingAndroid();
#endif
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		}
		if (bindingInstance != null)
		{
			bindingInstance.Initialize();
		}
	}

	public void Review(string defaultComment)
	{
		if (bindingInstance != null)
		{
			bindingInstance.Review(defaultComment);
		}
	}

	public void CreateInAppPurchase(string delegator)
	{
		if (bindingInstance != null)
		{
			bindingInstance.CreateInAppPurchase(delegator);
		}
	}

	public void ClearProductsIdentifier()
	{
		if (bindingInstance != null)
		{
			bindingInstance.ClearProductsIdentifier();
		}
	}

	public void AddProductsIdentifier(string productsId)
	{
		if (bindingInstance != null)
		{
			bindingInstance.AddProductsIdentifier(productsId);
		}
	}

	public void RequestProductsInfo()
	{
		if (bindingInstance != null)
		{
			bindingInstance.RequestProductsInfo();
		}
	}

	public string GetProductInfoPrice(string productsId)
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetProductInfoPrice(productsId);
	}

	public string GetProductInfoTitle(string productsId)
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetProductInfoTitle(productsId);
	}

	public void BuyProduct(string productsId)
	{
		if (bindingInstance != null)
		{
			bindingInstance.BuyProduct(productsId);
		}
	}

	public bool CanMakePayments()
	{
		if (bindingInstance == null)
		{
			return false;
		}
		return bindingInstance.CanMakePayments();
	}

	public string GetPurchasedTransaction()
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetPurchasedTransaction();
	}

	public string GetProductIdentifier(string transactionId)
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetProductIdentifier(transactionId);
	}

	public string GetTransactionReceipt(string transactionId)
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetTransactionReceipt(transactionId);
	}

	public void FinishTransaction(string transactionId)
	{
		if (bindingInstance != null)
		{
			bindingInstance.FinishTransaction(transactionId);
		}
	}

	public void ResetPaymentQueueDelegate()
	{
		if (bindingInstance != null)
		{
			bindingInstance.ResetPaymentQueueDelegate();
		}
	}

	public string GetNoticeRegistrationId()
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetNoticeRegistrationId();
	}

	public string GetUrlSchemeStr()
	{
		if (bindingInstance == null)
		{
			return null;
		}
		return bindingInstance.GetUrlSchemeStr();
	}

	public void ClearUrlSchemeStr()
	{
		if (bindingInstance != null)
		{
			bindingInstance.ClearUrlSchemeStr();
		}
	}

	public void RegistPnote(string guid)
	{
		if (bindingInstance != null)
		{
			bindingInstance.RegistPnote(guid);
		}
	}

	public void UnregistPnote()
	{
		if (bindingInstance != null)
		{
			bindingInstance.UnregistPnote();
		}
	}

	public void SendMessagePnote(string message, string sender, string reciever, string launchOption)
	{
		if (bindingInstance != null)
		{
			bindingInstance.SendMessagePnote(message, sender, reciever, launchOption);
		}
	}

	public void RegistTagsPnote(string tags, string guid)
	{
		if (bindingInstance != null)
		{
			bindingInstance.RegistTagsPnote(tags, guid);
		}
	}

	public string GetPnoteLaunchString()
	{
		if (bindingInstance != null)
		{
			return bindingInstance.GetPnoteLaunchString();
		}
		return null;
	}

	public void ClearIconBadgeNumber()
	{
		if (bindingInstance != null)
		{
			bindingInstance.ClearIconBadgeNumber();
		}
	}

	public void GetSystemProxy(out string host, out ushort port)
	{
		if (bindingInstance != null)
		{
			bindingInstance.GetSystemProxy(out host, out port);
			return;
		}
		host = null;
		port = 0;
	}

	public void SetClipBoard(string text)
	{
		if (bindingInstance != null)
		{
			bindingInstance.SetClipBoard(text);
		}
	}
}
