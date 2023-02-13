using Message;
using UnityEngine;

public class GameFollowCamera : CameraState
{
	private const float ZOffset = 0f;

	private const float PathPosSensitive = 6f;

	private const float downScrollLineSensitive = 2f;

	private const float pathDistanceSensitive = 0.5f;

	private Vector3 m_defaultTargetOffset;

	private Vector3 m_playerPosition;

	private float m_distPathToTarget;

	private Vector3 m_sideViewPos;

	private float m_downScrollLine;

	public GameFollowCamera()
		: base(CameraType.DEFAULT)
	{
	}

	public override void OnEnter(CameraManager manager)
	{
		SetCameraParameter(manager.GetParameter());
		m_defaultTargetOffset = manager.GetTargetOffset();
		m_downScrollLine = manager.EditParameter.m_downScrollLine;
		ResetParameters(manager);
	}

	public override void Update(CameraManager manager, float deltaTime)
	{
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		Vector3 sideViewPathPos = playerInformation.SideViewPathPos;
		Vector3 position = playerInformation.Position;
		float num = manager.m_startCameraPos.y + 1f;
		float y = sideViewPathPos.y;
		float y2 = position.y;
		sideViewPathPos = Vector3.Lerp(m_sideViewPos, sideViewPathPos, 6f * deltaTime);
		float num2 = y2 - y;
		float num3 = Vector3.Dot(playerInformation.VerticalVelocity, -playerInformation.GravityDir);
		float to = manager.EditParameter.m_downScrollLine;
		if (!playerInformation.IsOnGround() && num3 < -2f && num2 > num)
		{
			to = manager.EditParameter.m_downScrollLineOnDown;
		}
		m_downScrollLine = Mathf.Lerp(m_downScrollLine, to, 2f * deltaTime);
		float num4 = manager.EditParameter.m_upScrollLine + m_distPathToTarget;
		float num5 = m_downScrollLine + m_distPathToTarget;
		float upScrollLimit = manager.EditParameter.m_upScrollLimit;
		float downScrollLimit = manager.EditParameter.m_downScrollLimit;
		if (num2 > num4)
		{
			m_distPathToTarget += num2 - num4;
		}
		if (num2 < num5)
		{
			m_distPathToTarget += num2 - num5;
		}
		if (num2 < num && num2 > 0f)
		{
			m_distPathToTarget = Mathf.Lerp(m_distPathToTarget, manager.m_startCameraPos.y, 0.5f * deltaTime);
		}
		m_distPathToTarget = Mathf.Clamp(m_distPathToTarget, downScrollLimit, upScrollLimit);
		m_playerPosition.x = position.x;
		m_playerPosition.y = sideViewPathPos.y + m_distPathToTarget;
		m_playerPosition.z = 0f;
		m_param.m_target.x = m_playerPosition.x + m_defaultTargetOffset.x;
		m_param.m_target.y = m_playerPosition.y + m_defaultTargetOffset.y;
		m_param.m_target.z = 0f;
		m_param.m_position = m_param.m_target + manager.GetTargetToCamera();
		m_param.m_target.z = 0f;
		m_sideViewPos = sideViewPathPos;
	}

	public override void OnDrawGizmos(CameraManager manager)
	{
		Gizmos.DrawRay(m_playerPosition, m_param.m_upDirection * 0.5f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(m_playerPosition, 0.5f);
		Vector3 center = new Vector3(m_playerPosition.x, m_sideViewPos.y + m_distPathToTarget, m_playerPosition.z);
		center.y = m_sideViewPos.y + m_distPathToTarget + manager.EditParameter.m_upScrollLine;
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(center, 0.5f);
		center.y = m_sideViewPos.y + m_distPathToTarget + m_downScrollLine;
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(center, 0.5f);
	}

	private void ResetParameters(CameraManager manager)
	{
		PlayerInformation playerInformation = manager.GetPlayerInformation();
		m_playerPosition = playerInformation.Position;
		Vector3 position = playerInformation.Position;
		float y = position.y;
		Vector3 sideViewPathPos = playerInformation.SideViewPathPos;
		float y2 = sideViewPathPos.y;
		m_distPathToTarget = y - y2;
		m_sideViewPos = playerInformation.SideViewPathPos;
	}

	public void OnMsgTutorialResetForRetry(CameraManager manager, MsgTutorialResetForRetry msg)
	{
		ResetParameters(manager);
	}
}
