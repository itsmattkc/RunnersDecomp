public abstract class SocialTaskBase
{
	public enum ProcessState
	{
		NONE = -1,
		IDLE,
		PROCESSING,
		END
	}

	private ProcessState m_state;

	public SocialTaskBase()
	{
		m_state = ProcessState.IDLE;
	}

	public bool IsDone()
	{
		if (m_state == ProcessState.END)
		{
			return true;
		}
		return false;
	}

	public void Update()
	{
		switch (m_state)
		{
		case ProcessState.END:
			break;
		case ProcessState.IDLE:
			OnStartProcess();
			m_state = ProcessState.PROCESSING;
			break;
		case ProcessState.PROCESSING:
			OnUpdate();
			if (OnIsEndProcess())
			{
				m_state = ProcessState.END;
			}
			break;
		}
	}

	public string GetTaskName()
	{
		return OnGetTaskName();
	}

	protected abstract void OnStartProcess();

	protected abstract void OnUpdate();

	protected abstract bool OnIsEndProcess();

	protected abstract string OnGetTaskName();
}
