using UnityEngine;

public class AssetBundleRetryProcess : ServerRetryProcess
{
	public AssetBundleRetryProcess(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		if (m_callbackObject != null)
		{
			AssetBundleLoader component = m_callbackObject.GetComponent<AssetBundleLoader>();
			if (component != null)
			{
				component.RetryLoadScene(this);
			}
		}
	}
}
