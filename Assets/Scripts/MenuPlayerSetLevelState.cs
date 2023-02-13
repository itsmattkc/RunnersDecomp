using UnityEngine;

public abstract class MenuPlayerSetLevelState : MonoBehaviour
{
	protected enum EventSignal
	{
		BUTTON_PRESSED = 100,
		SERVER_RESPONSE_END
	}

	protected AbilityButtonParams m_params;

	public MenuPlayerSetLevelState()
	{
	}

	public void Setup(AbilityButtonParams param)
	{
		m_params = param;
	}

	public abstract void ChangeLabels();
}
