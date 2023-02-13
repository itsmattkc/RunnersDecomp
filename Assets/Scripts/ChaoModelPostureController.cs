using UnityEngine;

public class ChaoModelPostureController : MonoBehaviour
{
	private GameObject m_modelObject;

	private TinyFsmBehavior m_fsmBehavior;

	private Vector3 m_velocity = Vector3.zero;

	private Quaternion m_initRotaion = Quaternion.identity;

	private Quaternion LocalRotaion
	{
		get
		{
			if (m_modelObject != null)
			{
				return m_modelObject.transform.localRotation;
			}
			return Quaternion.identity;
		}
		set
		{
			if (m_modelObject != null)
			{
				m_modelObject.transform.localRotation = value;
			}
		}
	}

	private void Start()
	{
		CreateTinyFsm();
	}

	private void Update()
	{
	}

	private void CreateTinyFsm()
	{
		m_fsmBehavior = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsmBehavior != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateIdle);
			m_fsmBehavior.SetUp(description);
		}
	}

	private void OnDestroy()
	{
		if (m_fsmBehavior != null)
		{
			m_fsmBehavior.ShutDown();
			m_fsmBehavior = null;
		}
	}

	public void SetModelObject(GameObject modelObject)
	{
		m_modelObject = modelObject;
		m_initRotaion = LocalRotaion;
	}

	public void ChangeStateToSpin(Vector3 velocity)
	{
		m_velocity = velocity;
		ChangeState(new TinyFsmState(StateSpin));
	}

	public void ChangeStateToReturnIdle()
	{
		ChangeState(new TinyFsmState(StateReturnToIdle));
	}

	private void ChangeState(TinyFsmState nextState)
	{
		if (m_fsmBehavior != null)
		{
			m_fsmBehavior.ChangeState(nextState);
		}
	}

	private void AddRotation(Quaternion rot)
	{
		LocalRotaion *= rot;
	}

	private TinyFsmState StateIdle(TinyFsmEvent fsmEvent)
	{
		switch (fsmEvent.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSpin(TinyFsmEvent fsmEvent)
	{
		switch (fsmEvent.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			float getDeltaTime = fsmEvent.GetDeltaTime;
			Quaternion rot = Quaternion.AngleAxis(m_velocity.y * getDeltaTime, Vector3.up) * Quaternion.AngleAxis(m_velocity.x * getDeltaTime, Vector3.right) * Quaternion.AngleAxis(m_velocity.z * getDeltaTime, Vector3.forward);
			AddRotation(rot);
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateReturnToIdle(TinyFsmEvent fsmEvent)
	{
		switch (fsmEvent.Signal)
		{
		case -3:
			if (m_velocity.sqrMagnitude < float.Epsilon)
			{
				m_velocity = Vector3.one;
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			float getDeltaTime = fsmEvent.GetDeltaTime;
			Quaternion localRotaion = LocalRotaion;
			localRotaion = (LocalRotaion = Quaternion.RotateTowards(localRotaion, m_initRotaion, m_velocity.magnitude * getDeltaTime));
			if (localRotaion == m_initRotaion)
			{
				ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}
}
