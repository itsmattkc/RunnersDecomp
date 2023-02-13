using System;
using UnityEngine;

#if UNITY_ANDROID
public class BindingAndroid : BindingPlugin
{
	public const string PackageNameRelease = "com.sega.sonicrunners";

	private AndroidJavaObject Billing;

	private AndroidJavaObject GCM;

	private AndroidJavaObject Env;

	public BindingAndroid()
	{
		Billing = null;//new AndroidJavaObject(GetPackageName() + ".billing.BillingManager");
		GCM = null;//new AndroidJavaObject(GetPackageName() + ".gcm.GCMManager");
		Env = null;//new AndroidJavaObject(GetPackageName() + ".EnvManager");
	}

	public override void Initialize()
	{
	}

	public override void Review(string defaultComment)
	{
	}

	public override void CreateInAppPurchase(string delegator)
	{
	}

	public override void ClearProductsIdentifier()
	{
	}

	public override void AddProductsIdentifier(string productsId)
	{
	}

	public override void RequestProductsInfo()
	{
	}

	public override string GetProductInfoPrice(string productsId)
	{
		if (Billing == null)
		{
			return null;
		}
		return Billing.Call<string>("getProductPrice", new object[1]
		{
			productsId
		});
	}

	public override string GetProductInfoTitle(string productsId)
	{
		return null;
	}

	public override void BuyProduct(string productsId)
	{
		if (Billing != null)
		{
			Billing.Call("startPurchase", productsId);
		}
	}

	public override bool CanMakePayments()
	{
		if (Billing != null)
		{
			return Billing.Call<bool>("canMakePayments", new object[0]);
		}
		return false;
	}

	public override string GetPurchasedTransaction()
	{
		return null;
	}

	public override string GetProductIdentifier(string transactionId)
	{
		return null;
	}

	public override string GetTransactionReceipt(string transactionId)
	{
		return null;
	}

	public override void FinishTransaction(string transactionId)
	{
		AndroidJavaClass androidJavaClass = null;
		AndroidJavaObject androidJavaObject = null;
		androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		try
		{
			if (androidJavaClass == null)
			{
				return;
			}
			androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (androidJavaObject != null)
			{
				if (Billing != null)
				{
					androidJavaObject.Call("runOnUiThread", (AndroidJavaRunnable)delegate
					{
						Billing.Call("consumePurchase");
					});
				}
				androidJavaObject.Dispose();
			}
			androidJavaClass.Dispose();
		}
		catch (Exception)
		{
			if (androidJavaClass != null)
			{
				androidJavaClass.Dispose();
			}
			if (androidJavaObject != null)
			{
				androidJavaObject.Dispose();
			}
		}
	}

	public override void ResetPaymentQueueDelegate()
	{
	}

	public override void ClearIconBadgeNumber()
	{
	}

	public override string GetNoticeRegistrationId()
	{
		if (GCM != null)
		{
			return GCM.Call<string>("getGcmRegistrationId", new object[0]);
		}
		return null;
	}

	public override void RegistPnote(string guid)
	{
		if (GCM != null)
		{
			GCM.Call("registPnote", guid);
		}
	}

	public override void UnregistPnote()
	{
		if (GCM != null)
		{
			GCM.Call("unregistPnote");
		}
	}

	public override void SendMessagePnote(string message, string sender, string reciever, string launchOption)
	{
		if (GCM != null)
		{
			GCM.Call("sendMessage", message, sender, reciever, launchOption);
		}
	}

	public override void RegistTagsPnote(string tags, string guid)
	{
		if (GCM != null)
		{
			GCM.Call("registTags", tags, guid);
		}
	}

	public override string GetUrlSchemeStr()
	{
		if (Env != null)
		{
			return Env.Call<string>("getAtomScheme", new object[0]);
		}
		return null;
	}

	public override void ClearUrlSchemeStr()
	{
		if (Env != null)
		{
			Env.Call("clearAtomScheme");
		}
	}

	public override string GetPnoteLaunchString()
	{
		if (GCM != null)
		{
			return GCM.Call<string>("getLaunchString", new object[0]);
		}
		return null;
	}

	public static string GetPackageName()
	{
		return "com.sega.sonicrunners";
	}

	public override void GetSystemProxy(out string host, out ushort port)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.System"))
		{
			host = androidJavaClass.CallStatic<string>("getProperty", new object[1]
			{
				"http.proxyHost"
			});
			port = Convert.ToUInt16(androidJavaClass.CallStatic<string>("getProperty", new object[1]
			{
				"http.proxyPort"
			}));
			androidJavaClass.Dispose();
		}
	}

	public override void SetClipBoard(string text)
	{
		if (GCM != null)
		{
			GCM.Call("setTextClipBoard", text);
		}
	}
}
#endif
