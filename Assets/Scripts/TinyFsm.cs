using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/TinyFsm")]
public class TinyFsm
{
	private TinyFsmState m_cur;

	private TinyFsmState m_src;

	private bool m_hierarchical;

	public TinyFsm()
	{
		m_src = TinyFsmState.Top();
		m_hierarchical = false;
	}

	public void Initialize(MonoBehaviour context, TinyFsmState state, bool hierarchical)
	{
		m_hierarchical = hierarchical;
		if (m_hierarchical)
		{
			List<TinyFsmState> list = new List<TinyFsmState>();
			TinyFsmState tinyFsmState = state;
			while (!tinyFsmState.IsTop())
			{
				list.Add(tinyFsmState);
				tinyFsmState = Super(context, tinyFsmState);
			}
			list.Reverse();
			foreach (TinyFsmState item in list)
			{
				Enter(item);
			}
			m_cur = state;
		}
		else
		{
			Enter(state);
			m_cur = state;
		}
	}

	public void Shutdown(MonoBehaviour context)
	{
		if (m_hierarchical)
		{
			TinyFsmState tinyFsmState = m_cur;
			while (!tinyFsmState.IsTop())
			{
				Leave(tinyFsmState);
				tinyFsmState = Super(context, tinyFsmState);
			}
			m_cur.Clear();
			m_src.Clear();
		}
		else
		{
			Leave(m_cur);
			m_cur.Clear();
		}
	}

	public void Dispatch(MonoBehaviour context, TinyFsmEvent e)
	{
		if (m_hierarchical)
		{
			m_src = m_cur;
			while (!m_src.IsTop() && !m_src.IsEnd())
			{
				TinyFsmState tinyFsmState = Trigger(context, m_src, e);
				if (tinyFsmState.IsEnd() || !tinyFsmState.IsValid())
				{
					break;
				}
				m_src = Super(context, m_src);
			}
		}
		else
		{
			m_cur.Call(e);
		}
	}

	public bool ChangeState(MonoBehaviour context, TinyFsmState state)
	{
		if (m_cur == state)
		{
			return false;
		}
		if (m_hierarchical)
		{
			for (TinyFsmState tinyFsmState = m_cur; tinyFsmState != m_src; tinyFsmState = Super(context, tinyFsmState))
			{
				Leave(tinyFsmState);
			}
			if (m_src == state)
			{
				Leave(m_src);
				Enter(state);
			}
			else
			{
				List<TinyFsmState> list = new List<TinyFsmState>();
				TinyFsmState tinyFsmState2 = m_src;
				while (!tinyFsmState2.IsTop())
				{
					list.Add(tinyFsmState2);
					tinyFsmState2 = Super(context, tinyFsmState2);
				}
				List<TinyFsmState> list2 = new List<TinyFsmState>();
				TinyFsmState tinyFsmState3 = state;
				while (!tinyFsmState3.IsTop())
				{
					list2.Add(tinyFsmState3);
					tinyFsmState3 = Super(context, tinyFsmState3);
				}
				list.Reverse();
				list2.Reverse();
				IEnumerator<TinyFsmState> enumerator = list.GetEnumerator();
				IEnumerator<TinyFsmState> enumerator2 = list2.GetEnumerator();
				while (enumerator.Current != list[0] && enumerator2.Current != list2[0] && enumerator.Current == enumerator2.Current)
				{
					enumerator.MoveNext();
					enumerator2.MoveNext();
				}
				list.Reverse();
				foreach (TinyFsmState item in list)
				{
					Leave(item);
					if (item == enumerator.Current)
					{
						break;
					}
				}
				while (enumerator2.Current != list2[0])
				{
					Enter(enumerator2.Current);
					enumerator2.MoveNext();
				}
				m_cur = state;
			}
		}
		else
		{
			Leave(m_cur);
			Enter(state);
			m_cur = state;
		}
		return true;
	}

	public TinyFsmState GetCurrentState()
	{
		return m_cur;
	}

	private static TinyFsmState Trigger(MonoBehaviour context, TinyFsmState state, TinyFsmEvent e)
	{
		return state.Call(e);
	}

	private static TinyFsmState Super(MonoBehaviour context, TinyFsmState state)
	{
		return Trigger(context, state, TinyFsmEvent.CreateSuper());
	}

	private static void Init(TinyFsmState state)
	{
		state.Call(TinyFsmEvent.CreateInit());
	}

	private static void Enter(TinyFsmState state)
	{
		state.Call(TinyFsmEvent.CreateEnter());
	}

	private static void Leave(TinyFsmState state)
	{
		state.Call(TinyFsmEvent.CreateLeave());
	}
}
