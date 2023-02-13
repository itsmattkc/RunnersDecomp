using UnityEngine;

internal class InformationDataLoadRetryProcess : ServerRetryProcess
{
	private TitleDataLoader m_loader;

	public InformationDataLoadRetryProcess(GameObject returnObject, TitleDataLoader loader)
		: base(returnObject)
	{
		m_loader = loader;
	}

	public override void Retry()
	{
		if (m_loader != null)
		{
			m_loader.RetryInformationDataLoad();
		}
	}
}
