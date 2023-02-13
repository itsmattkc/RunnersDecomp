using UnityEngine;

public abstract class MenuPlayerSetPartsBase : MonoBehaviour
{
	private UIPanel m_panel;

	private bool m_isReady;

	public MenuPlayerSetPartsBase(string panelName)
	{
		m_isReady = false;
	}

	public void PlayStart()
	{
		OnPlayStart();
	}

	public void PlayEnd()
	{
		OnPlayEnd();
	}

	public void Reset()
	{
		m_isReady = false;
	}

	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (!m_isReady)
		{
			OnSetup();
			PlayStart();
			m_isReady = true;
		}
		OnUpdate(deltaTime);
	}

	protected abstract void OnSetup();

	protected abstract void OnPlayStart();

	protected abstract void OnPlayEnd();

	protected abstract void OnUpdate(float deltaTime);
}
