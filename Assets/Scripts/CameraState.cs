using UnityEngine;

public class CameraState
{
	protected CameraParameter m_param;

	private CameraType m_type;

	public CameraState(CameraType type)
	{
		m_param = default(CameraParameter);
		m_param.m_upDirection = Vector3.up;
		m_type = type;
	}

	public virtual void OnEnter(CameraManager manager)
	{
	}

	public virtual void OnLeave(CameraManager manager)
	{
	}

	public virtual void Update(CameraManager manager, float deltaTime)
	{
	}

	public void GetCameraParameter(ref CameraParameter parameter)
	{
		parameter.m_target = m_param.m_target;
		parameter.m_position = m_param.m_position;
		parameter.m_upDirection = m_param.m_upDirection;
	}

	public void SetCameraParameter(CameraParameter param)
	{
		m_param.m_target = param.m_target;
		m_param.m_position = param.m_position;
		m_param.m_upDirection = param.m_upDirection;
	}

	public CameraType GetCameraType()
	{
		return m_type;
	}

	public virtual void OnDrawGizmos(CameraManager manager)
	{
	}
}
