using SaveData;
using System;
using UnityEngine;

public class LocalNotification
{
	private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static void Initialize()
	{
		CancelAllNotifications();
	}

	public static void OnActive()
	{
		ClearRecieveNotifications();
	}

	private static void ClearRecieveNotifications()
	{
	}

	public static void EnableNotification(bool value)
	{
		if (value)
		{
			PnoteNotification.RequestRegister();
		}
		else
		{
			PnoteNotification.RequestUnregister();
		}
	}

	public static void RegisterNotification(float second, string message)
	{
		if (!(SystemSaveManager.Instance != null) || SystemSaveManager.Instance.GetSystemdata().pushNotice)
		{
			DateTime dateTime = DateTime.Now.AddSeconds(second);
			long num = ToUnixTime(dateTime);
#if UNITY_ANDROID
			//AndroidJavaObject androidJavaObject = new AndroidJavaObject(BindingAndroid.GetPackageName() + ".gcm.GCMManager");
			//androidJavaObject.Call("registLocalNotification", num, message);
#endif
		}
	}

	public static void CancelAllNotifications()
	{
#if UNITY_ANDROID
		//AndroidJavaObject androidJavaObject = new AndroidJavaObject(BindingAndroid.GetPackageName() + ".gcm.GCMManager");
		//androidJavaObject.Call("clearAllNotification");
#endif
	}

	private static long ToUnixTime(DateTime dateTime)
	{
		dateTime = dateTime.ToUniversalTime();
		return (long)dateTime.Subtract(UnixEpoch).TotalMilliseconds;
	}
}
