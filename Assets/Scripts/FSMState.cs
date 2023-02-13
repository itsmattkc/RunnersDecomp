using Message;

public abstract class FSMState<Context>
{
	public virtual void Enter(Context context)
	{
	}

	public virtual void Leave(Context context)
	{
	}

	public virtual void Step(Context context, float deltatTime)
	{
	}

	public virtual void OnGUI(Context context)
	{
	}

	public virtual bool DispatchMessage(Context context, int messageId, MessageBase msg)
	{
		return false;
	}
}
