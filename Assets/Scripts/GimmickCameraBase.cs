using UnityEngine;

public class GimmickCameraBase : CameraState
{
	protected enum Mode
	{
		Idle,
		Wait,
		Move
	}

	protected struct GimmickCameraParameter
	{
		public float m_waitTime;

		public float m_upScrollViewPort;

		public float m_leftScrollViewPort;

		public float m_depthScrollViewPort;

		public float m_scrollTime;
	}

	protected Mode m_mode;

	private GimmickCameraParameter m_gimmickCameraParam = default(GimmickCameraParameter);

	private float m_time;

	private Vector3 m_speed;

	private Vector3 m_playerPosition;

	private Camera m_camera;

	public GimmickCameraBase(CameraType type)
		: base(type)
	{
	}

	public override void OnEnter(CameraManager manager)
	{
		SetCameraParameter(manager.GetParameter());
		m_gimmickCameraParam = GetGimmickCameraParameter(manager);
		m_mode = Mode.Wait;
		m_time = 0f;
		m_speed = Vector3.zero;
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		m_playerPosition = playerInformation.Position;
		m_camera = manager.GetComponent<Camera>();
	}

	public override void Update(CameraManager manager, float deltaTime)
	{
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		Vector3 playerPosition = m_playerPosition;
		Vector3 position = m_playerPosition = playerInformation.Position;
		Vector3 nowTarget = m_param.m_target + (m_playerPosition - playerPosition);
		if ((bool)m_camera)
		{
			Vector3 viewPort = m_camera.WorldToViewportPoint(position);
			UpdateGimmickCamera(manager, deltaTime, viewPort, ref nowTarget);
			m_param.m_target = nowTarget;
			m_param.m_position = m_param.m_target + manager.GetTargetToCamera();
		}
	}

	public override void OnDrawGizmos(CameraManager manager)
	{
		Gizmos.DrawRay(m_playerPosition, m_param.m_upDirection * 0.5f);
	}

	protected virtual GimmickCameraParameter GetGimmickCameraParameter(CameraManager manager)
	{
		return m_gimmickCameraParam;
	}

	private void UpdateGimmickCamera(CameraManager manager, float deltaTime, Vector3 viewPort, ref Vector3 nowTarget)
	{
		switch (m_mode)
		{
		case Mode.Wait:
			m_time += deltaTime;
			if (m_time > m_gimmickCameraParam.m_waitTime)
			{
				m_speed.x = GetSpeed(viewPort.x, m_gimmickCameraParam.m_leftScrollViewPort, m_gimmickCameraParam.m_scrollTime, 0.01f);
				m_speed.y = GetSpeed(viewPort.y, m_gimmickCameraParam.m_upScrollViewPort, m_gimmickCameraParam.m_scrollTime, 0.01f);
				m_speed.z = GetSpeed(viewPort.z, m_gimmickCameraParam.m_depthScrollViewPort, m_gimmickCameraParam.m_scrollTime, 0.01f);
				m_mode = Mode.Move;
			}
			break;
		case Mode.Move:
			viewPort.x = UpdateScroll(viewPort.x, m_gimmickCameraParam.m_leftScrollViewPort, m_speed.x * deltaTime);
			viewPort.y = UpdateScroll(viewPort.y, m_gimmickCameraParam.m_upScrollViewPort, m_speed.y * deltaTime);
			viewPort.z = UpdateScroll(viewPort.z, m_gimmickCameraParam.m_depthScrollViewPort, m_speed.z * deltaTime);
			nowTarget = GetNowTarget(viewPort, nowTarget);
			m_time += deltaTime;
			if (m_time > m_gimmickCameraParam.m_scrollTime)
			{
				m_mode = Mode.Idle;
			}
			break;
		}
	}

	private Vector3 GetNowTarget(Vector3 viewPort, Vector3 nowTarget)
	{
		if ((bool)m_camera)
		{
			Vector3 b = m_camera.ViewportToWorldPoint(viewPort);
			return nowTarget + (m_playerPosition - b);
		}
		return nowTarget;
	}

	private static float GetSpeed(float value, float tgt_value, float time, float minmax)
	{
		float value2 = (value - tgt_value) / time;
		return GetMinMax(value2, minmax);
	}

	private static float GetMinMax(float value, float minmax)
	{
		float a = Mathf.Abs(value);
		float b = Mathf.Abs(minmax);
		a = Mathf.Max(a, b);
		return (!(value < 0f)) ? a : (0f - a);
	}

	private static float UpdateScroll(float value, float tgt_value, float speed)
	{
		float num = value - speed;
		if (speed < 0f)
		{
			if (num > tgt_value)
			{
				num = tgt_value;
			}
		}
		else if (num < tgt_value)
		{
			num = tgt_value;
		}
		return num;
	}
}
