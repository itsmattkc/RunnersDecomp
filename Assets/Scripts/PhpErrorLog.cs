using UnityEngine;

internal class PhpErrorLog : MonoBehaviour
{
	private string mErrorText = string.Empty;

	public static PhpErrorLog Create(string errorText)
	{
		GameObject gameObject = new GameObject("PhpErrorLog");
		PhpErrorLog phpErrorLog = gameObject.AddComponent<PhpErrorLog>();
		phpErrorLog.mErrorText = "!!! PHP ERROR : " + errorText;
		return phpErrorLog;
	}

	private void Update()
	{
		Debug.Log(mErrorText);
	}
}
