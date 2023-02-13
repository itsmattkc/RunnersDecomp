using UnityEngine;

public class LaserCamera : CameraState
{
	private enum Mode
	{
		Stay,
		MoveFast,
		MoveSlow,
		MoveConst
	}

	private Mode m_mode;

	private Vector3 m_playerPosition;

	public LaserCamera()
		: base(CameraType.LASER)
	{
	}

	public override void OnEnter(CameraManager manager)
	{
		SetCameraParameter(manager.GetParameter());
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		m_playerPosition = playerInformation.Position;
		m_mode = Mode.Stay;
	}

	public override void Update(CameraManager manager, float deltaTime)
	{
		Camera component = manager.GetComponent<Camera>();
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		Vector3 playerPosition = m_playerPosition;
		Vector3 position = component.WorldToViewportPoint(m_playerPosition = playerInformation.Position);
		CameraManager.LaserEditParameter laserParameter = manager.LaserParameter;
		float upScrollViewPort = laserParameter.m_upScrollViewPort;
		float downScrollViewPort = laserParameter.m_downScrollViewPort;
		if (position.y > upScrollViewPort)
		{
			position.y = upScrollViewPort;
		}
		if (position.y < downScrollViewPort)
		{
			position.y = downScrollViewPort;
		}
		switch (m_mode)
		{
		case Mode.Stay:
			if (position.x > laserParameter.m_rightScrollViewPort)
			{
				m_mode = Mode.MoveFast;
			}
			return;
		case Mode.MoveFast:
		{
			float num = (position.x - laserParameter.m_leftScrollViewPort) / laserParameter.m_fastScrollTime;
			position.x -= num * deltaTime;
			if (position.x < laserParameter.m_leftScrollViewPort)
			{
				position.x = laserParameter.m_leftScrollViewPort;
				m_mode = Mode.MoveConst;
			}
			break;
		}
		case Mode.MoveSlow:
		{
			float fastScrollTime = laserParameter.m_fastScrollTime;
			position.x += fastScrollTime * deltaTime;
			if (position.x > laserParameter.m_rightScrollViewPort)
			{
				m_mode = Mode.MoveFast;
			}
			break;
		}
		case Mode.MoveConst:
			position.x = laserParameter.m_leftScrollViewPort;
			break;
		}
		Vector3 b = component.ViewportToWorldPoint(position);
		Vector3 b2 = (m_mode != Mode.MoveConst) ? (m_playerPosition - b) : Vector3.zero;
		Vector3 target = m_param.m_target + (m_playerPosition - playerPosition) + b2;
		m_param.m_target = target;
		m_param.m_position = m_param.m_target + manager.GetTargetToCamera();
	}

	public override void OnDrawGizmos(CameraManager manager)
	{
		Gizmos.DrawRay(m_playerPosition, m_param.m_upDirection * 0.5f);
	}
}
