using UnityEngine;

public class AndroidObserver : MonoBehaviour
{
	private static AndroidObserver instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnBeforePurchaseFinishedSuccess(string message)
	{
		Debug.Log("AndroidObserver : OnBeforePurchaseFinishedSuccess :" + message);
		NativeObserver.Instance.OnBeforePurchaseFinishedSuccess(message);
	}

	public void OnPurchaseFinishedSuccess(string message)
	{
		Debug.Log("AndroidObserver : OnPurchaseFinishedSuccess :" + message);
		NativeObserver.Instance.OnPurchaseFinishedSuccess(message);
	}

	public void OnPurchaseFinishedFailed(string message)
	{
		Debug.Log("AndroidObserver : OnPurchaseFinishedFailed :" + message);
		NativeObserver.Instance.OnPurchaseFinishedFailed(message);
	}

	public void OnPurchaseFinishedCancel(string message)
	{
		Debug.Log("AndroidObserver : OnPurchaseFinishedCancel :" + message);
		NativeObserver.Instance.OnPurchaseFinishedCancel(message);
	}
}
