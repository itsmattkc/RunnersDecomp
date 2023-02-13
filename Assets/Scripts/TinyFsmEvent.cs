using Message;
using UnityEngine;

public class TinyFsmEvent : TinyFsmSystemEvent
{
	private enum EventDataType
	{
		NON,
		DELTATIME,
		MESSAGE,
		OBJECT,
		INTEGER
	}

	private EventDataType m_type;

	private int m_integer;

	private float m_float;

	private MessageBase m_message;

	private Object m_object;

	public float GetDeltaTime
	{
		get
		{
			return m_float;
		}
	}

	public MessageBase GetMessage
	{
		get
		{
			if (m_type == EventDataType.MESSAGE)
			{
				return m_message;
			}
			return null;
		}
	}

	public Object GetObject
	{
		get
		{
			if (m_type == EventDataType.OBJECT)
			{
				return m_object;
			}
			return null;
		}
	}

	public int GetInt
	{
		get
		{
			if (m_type == EventDataType.INTEGER)
			{
				return m_integer;
			}
			return 0;
		}
	}

	public TinyFsmEvent(int sig)
		: base(sig)
	{
	}

	public TinyFsmEvent(int sig, float deltaTime)
		: base(sig)
	{
		m_type = EventDataType.DELTATIME;
		m_float = deltaTime;
	}

	public TinyFsmEvent(int sig, MessageBase msg)
		: base(sig)
	{
		m_type = EventDataType.MESSAGE;
		m_message = msg;
	}

	public TinyFsmEvent(int sig, Object obj)
		: base(sig)
	{
		m_type = EventDataType.OBJECT;
		m_object = obj;
	}

	public TinyFsmEvent(int sig, int integer)
		: base(sig)
	{
		m_type = EventDataType.OBJECT;
		m_integer = integer;
	}

	public static TinyFsmEvent CreateSignal(int sig)
	{
		return new TinyFsmEvent(sig);
	}

	public static TinyFsmEvent CreateSuper()
	{
		return CreateSignal(-1);
	}

	public static TinyFsmEvent CreateInit()
	{
		return CreateSignal(-2);
	}

	public static TinyFsmEvent CreateEnter()
	{
		return CreateSignal(-3);
	}

	public static TinyFsmEvent CreateLeave()
	{
		return CreateSignal(-4);
	}

	public static TinyFsmEvent CreateUserEvent(int signal)
	{
		return CreateSignal(signal);
	}

	public static TinyFsmEvent CreateUpdate(float deltaTime)
	{
		return new TinyFsmEvent(0, deltaTime);
	}

	public static TinyFsmEvent CreateMessage(MessageBase msg)
	{
		return new TinyFsmEvent(1, msg);
	}

	public static TinyFsmEvent CreateUserEventObject(int signal, Object obj)
	{
		return new TinyFsmEvent(signal, obj);
	}

	public static TinyFsmEvent CreateUserEventInt(int signal, int integer)
	{
		return new TinyFsmEvent(signal, integer);
	}
}
