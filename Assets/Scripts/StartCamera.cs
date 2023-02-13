using UnityEngine;

public class StartCamera : CameraState
{
	private enum Type
	{
		NEAR,
		BACK,
		FAR
	}

	private Type m_type;

	private Vector3 m_nearTargetOffset;

	private Vector3 m_nearCameraOffset;

	private Vector3 m_playerPosition;

	private float m_timer;

	private float m_rate;

	private float m_perRate;

	public StartCamera()
		: base(CameraType.START_ACT)
	{
	}

	public override void OnEnter(CameraManager manager)
	{
		SetCameraParameter(manager.GetParameter());
		m_nearTargetOffset = manager.StartActParameter.m_targetOffset;
		m_nearCameraOffset = manager.StartActParameter.m_cameraOffset;
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		m_playerPosition = playerInformation.Position;
		m_type = Type.NEAR;
		m_rate = 0f;
		m_perRate = 0f;
		m_timer = manager.StartActParameter.m_nearStayTime;
	}

	public override void Update(CameraManager manager, float deltaTime)
	{
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		Vector3 position = playerInformation.Position;
		m_playerPosition.x = position.x;
		m_playerPosition.y = position.y;
		m_playerPosition.z = 0f;
		Vector3 vector = m_playerPosition + m_nearTargetOffset;
		Vector3 vector2 = vector + m_nearCameraOffset;
		vector2.z -= vector.z;
		Vector3 vector3 = vector;
		Vector3 vector4 = vector2;
		CameraState cameraByType = manager.GetCameraByType(CameraType.DEFAULT);
		if (cameraByType != null)
		{
			CameraParameter parameter = m_param;
			cameraByType.GetCameraParameter(ref parameter);
			vector3 = parameter.m_target;
			vector4 = parameter.m_position;
		}
		switch (m_type)
		{
		case Type.NEAR:
			m_param.m_target = vector;
			m_param.m_position = vector2;
			m_timer -= deltaTime;
			if (m_timer < 0f)
			{
				if (manager.StartActParameter.m_nearToFarTime > 0f)
				{
					m_rate = 0f;
					m_perRate = 1f / manager.StartActParameter.m_nearToFarTime;
					m_type = Type.BACK;
				}
				else
				{
					m_type = Type.FAR;
				}
			}
			break;
		case Type.BACK:
			m_rate += m_perRate * deltaTime;
			if (m_rate > 1f)
			{
				m_param.m_target = vector3;
				m_param.m_position = vector4;
				m_type = Type.FAR;
			}
			else
			{
				m_param.m_target = Vector3.Lerp(vector, vector3, m_rate);
				m_param.m_position = Vector3.Lerp(vector2, vector4, m_rate);
			}
			break;
		case Type.FAR:
			m_param.m_target = vector3;
			m_param.m_position = vector4;
			break;
		}
	}
}
