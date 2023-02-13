using App;
using App.Utility;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/Components/TinyFSM")]
public class TinyFsmBehavior : MonoBehaviour
{
	public class Description
	{
		public MonoBehaviour controlBehavior;

		public TinyFsmState initState
		{
			get;
			set;
		}

		public bool hierarchical
		{
			get;
			set;
		}

		public bool onFixedUpdate
		{
			get;
			set;
		}

		public bool ignoreDeltaTime
		{
			get;
			set;
		}

		public Description(MonoBehaviour control)
		{
			controlBehavior = control;
			initState = TinyFsmState.Top();
			hierarchical = false;
			onFixedUpdate = false;
			ignoreDeltaTime = false;
		}
	}

	private enum StatusFlag
	{
		STATUS_END_SETUP,
		STATUS_ENABLE_UPDATE,
		STATUS_ON_CHANGE_STATE,
		STATUS_HIERARCHICAL,
		STATUS_FIXEDUPDATE,
		STATUS_IGNORE_DELTATIME,
		STATUS_SHUTDOWN
	}

	private TinyFsm m_fsm;

	private MonoBehaviour m_controlBehavior;

	private Bitset32 m_statusFlags;

	public bool NowShutDown
	{
		get
		{
			return m_statusFlags.Test(6);
		}
	}

	public void SetUp(Description desc)
	{
		if (m_fsm == null)
		{
			m_fsm = new TinyFsm();
			m_controlBehavior = desc.controlBehavior;
			m_fsm.Initialize(m_controlBehavior, desc.initState, desc.hierarchical);
			m_statusFlags.Set(4, desc.onFixedUpdate);
			m_statusFlags.Set(5, desc.ignoreDeltaTime);
			m_statusFlags.Set(0, true);
			m_statusFlags.Set(1, true);
		}
	}

	public void ShutDown()
	{
		m_statusFlags.Set(6, true);
		if (m_controlBehavior != null && m_fsm != null)
		{
			m_fsm.Shutdown(m_controlBehavior);
		}
		m_fsm = null;
		m_controlBehavior = null;
	}

	private void Update()
	{
		if ((!Math.NearZero(Time.deltaTime) || m_statusFlags.Test(5)) && !m_statusFlags.Test(4))
		{
			UpdateImpl(Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (m_statusFlags.Test(4))
		{
			UpdateImpl(Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		ShutDown();
	}

	public bool ChangeState(TinyFsmState state)
	{
		if (m_fsm == null || m_controlBehavior == null)
		{
			return false;
		}
		bool result = false;
		if (!m_statusFlags.Test(2))
		{
			m_statusFlags.Set(2);
			result = m_fsm.ChangeState(m_controlBehavior, state);
			m_statusFlags.Reset(2);
		}
		return result;
	}

	public TinyFsmState GetCurrentState()
	{
		if (m_fsm != null)
		{
			return m_fsm.GetCurrentState();
		}
		return TinyFsmState.Top();
	}

	private void SetEnableUpdate(bool isEnableUpdate)
	{
		m_statusFlags.Set(1, isEnableUpdate);
	}

	private void UpdateImpl(float deltaTime)
	{
		if (m_fsm != null && !(m_controlBehavior == null) && m_statusFlags.Test(1) && m_statusFlags.Test(0))
		{
			m_fsm.Dispatch(m_controlBehavior, TinyFsmEvent.CreateUpdate(deltaTime));
		}
	}

	public void Dispatch(TinyFsmEvent signal)
	{
		if (m_fsm != null && !(m_controlBehavior == null))
		{
			if (signal.Signal < 1)
			{
				Debug.Log("Cannot Dispatch Signal ID is not for User.\n");
			}
			m_fsm.Dispatch(m_controlBehavior, signal);
		}
	}
}
