using System.Collections.Generic;

public class FSMSystem<Context>
{
	private List<FSMStateFactory<Context>> m_states;

	private int m_currentStateID;

	private FSMState<Context> m_currentState;

	public StateID CurrentStateID
	{
		get
		{
			return (StateID)m_currentStateID;
		}
	}

	public FSMState<Context> CurrentState
	{
		get
		{
			return m_currentState;
		}
	}

	public FSMSystem()
	{
		m_states = new List<FSMStateFactory<Context>>();
	}

	public void AddState(int stateID, FSMState<Context> s)
	{
		FSMStateFactory<Context> stateFactory = new FSMStateFactory<Context>(stateID, s);
		AddState(stateFactory);
	}

	public void AddState(FSMStateFactory<Context> stateFactory)
	{
		if (CurrentState != null)
		{
			Debug.LogError("FSM ERROR: Impossible to add state " + stateFactory.stateID + ". State is already Initialized");
		}
		if (stateFactory.state == null)
		{
			Debug.LogError("FSM ERROR: Null reference is not allowed");
		}
		if (GetStateFactory(stateFactory.stateID) != null)
		{
			Debug.LogError("FSM ERROR: Impossible to add state " + stateFactory.stateID + " because state has already been added");
		}
		else
		{
			m_states.Add(stateFactory);
		}
	}

	public void Init(Context context, int id)
	{
		FSMStateFactory<Context> stateFactory = GetStateFactory(id);
		if (stateFactory != null)
		{
			m_currentState = stateFactory.state;
			m_currentStateID = stateFactory.stateID;
			m_currentState.Enter(context);
		}
		else
		{
			Debug.LogError("FSM ERROR: Impossible to Init " + id + ". State is not Found");
		}
	}

	private FSMStateFactory<Context> GetStateFactory(int id)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].stateID == id)
			{
				return m_states[i];
			}
		}
		return null;
	}

	private FSMState<Context> GetState(int id)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].stateID == id)
			{
				return m_states[i].state;
			}
		}
		return null;
	}

	public void ReplaceState(int stateID, FSMState<Context> s)
	{
		if (CurrentState != null)
		{
			Debug.LogError("FSM ERROR: Impossible to replace state " + stateID + ". State is already Initialized");
		}
		if (s == null)
		{
			Debug.LogError("FSM ERROR: Null reference is not allowed");
		}
		FSMStateFactory<Context> stateFactory = GetStateFactory(stateID);
		if (stateFactory != null)
		{
			stateFactory.state = s;
			return;
		}
		FSMStateFactory<Context> item = new FSMStateFactory<Context>(stateID, s);
		m_states.Add(item);
	}

	public void ChangeState(Context context, int stateID)
	{
		FSMStateFactory<Context> stateFactory = GetStateFactory(stateID);
		if (stateFactory != null)
		{
			m_currentState.Leave(context);
			m_currentState = stateFactory.state;
			m_currentStateID = stateFactory.stateID;
			m_currentState.Enter(context);
		}
	}
}
