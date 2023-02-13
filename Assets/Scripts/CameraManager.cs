using Message;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[Serializable]
	public class CameraEditParameter
	{
		public float m_upScrollLine = 3f;

		public float m_downScrollLine = -1f;

		public float m_upScrollLimit = 10f;

		public float m_downScrollLimit = -5f;

		public float m_downScrollLineOnDown = 2f;
	}

	[Serializable]
	public class LaserEditParameter
	{
		public float m_upScrollViewPort = 0.8f;

		public float m_downScrollViewPort = 0.2f;

		public float m_rightScrollViewPort = 0.758f;

		public float m_leftScrollViewPort = 0.1f;

		public float m_fastScrollTime = 0.2f;

		public float m_slowScrollSpeed = 0.5f;
	}

	[Serializable]
	public class JumpBoardEditParameter
	{
		public float m_waitTime;

		public float m_upScrollViewPort = 0.6f;

		public float m_leftScrollViewPort = 0.2f;

		public float m_depthScrollViewPort = 26f;

		public float m_scrollTime = 0.5f;
	}

	[Serializable]
	public class CannonEditParameter
	{
		public float m_waitTime;

		public float m_upScrollViewPort = 0.3f;

		public float m_leftScrollViewPort = 0.2f;

		public float m_depthScrollViewPort = 26f;

		public float m_scrollTime = 0.5f;
	}

	[Serializable]
	public class LoopTerrainEditParameter
	{
		public float m_waitTime;

		public float m_upScrollViewPort = 0.3f;

		public float m_leftScrollViewPort = 0.1f;

		public float m_depthScrollViewPort = 18f;

		public float m_scrollTime;
	}

	[Serializable]
	public class StartActEditParameter
	{
		public Vector3 m_cameraOffset = new Vector3(0f, 0f, -10f);

		public Vector3 m_targetOffset = new Vector3(0f, 0.5f, 0f);

		public float m_nearStayTime = 2.5f;

		public float m_nearToFarTime = 1f;
	}

	private PlayerInformation m_playerInfo;

	private Vector3 m_defaultTargetOffset;

	private Vector3 m_playerPosition;

	private float m_prevDistanceGround;

	private Vector3 m_targetToCamera;

	private Vector3 m_differencePos;

	private Vector3 m_differenceTarget;

	private bool m_differenceApproachFlag;

	private List<CameraState> m_cameraList = new List<CameraState>();

	private float m_interpolateTime;

	private float m_interpolateRate;

	private float m_ratePerSec;

	private CameraParameter m_param = default(CameraParameter);

	private CameraParameter m_topParam = default(CameraParameter);

	private CameraParameter m_lowerParam = default(CameraParameter);

	public Vector3 m_startCameraPos = new Vector3(4.5f, 2f, -18f);

	public CameraEditParameter m_editParameter = new CameraEditParameter();

	public LaserEditParameter m_laserParameter = new LaserEditParameter();

	public JumpBoardEditParameter m_jumpBoardParameter = new JumpBoardEditParameter();

	public CannonEditParameter m_cannonParameter = new CannonEditParameter();

	public LoopTerrainEditParameter m_loopTerrainParameter = new LoopTerrainEditParameter();

	public StartActEditParameter m_startActParameter = new StartActEditParameter();

	[SerializeField]
	private bool m_drawInfo;

	[SerializeField]
	private bool m_debugInterpolate;

	[SerializeField]
	private float m_debugPushInterpolateTime = 0.5f;

	[SerializeField]
	private float m_debugPopInterpolateTime = 0.5f;

	private Rect m_window;

	public CameraEditParameter EditParameter
	{
		get
		{
			return m_editParameter;
		}
	}

	public LaserEditParameter LaserParameter
	{
		get
		{
			return m_laserParameter;
		}
	}

	public JumpBoardEditParameter JumpBoardParameter
	{
		get
		{
			return m_jumpBoardParameter;
		}
	}

	public CannonEditParameter CannonParameter
	{
		get
		{
			return m_cannonParameter;
		}
	}

	public LoopTerrainEditParameter LoopTerrainParameter
	{
		get
		{
			return m_loopTerrainParameter;
		}
	}

	public StartActEditParameter StartActParameter
	{
		get
		{
			return m_startActParameter;
		}
	}

	private void Start()
	{
		Camera camera = base.camera;
		float fieldOfView = camera.fieldOfView;
		ScreenType screenType = ScreenUtil.GetScreenType();
		float num = 1.5f;
		switch (screenType)
		{
		case ScreenType.iPhone5:
		{
			float num4 = 1.77777779f;
			fieldOfView = (camera.fieldOfView = fieldOfView / (num4 / num));
			break;
		}
		case ScreenType.iPad:
		{
			float num2 = 1.33333337f;
			fieldOfView = (camera.fieldOfView = fieldOfView / (num2 / num));
			break;
		}
		}
		m_playerInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
		m_playerPosition = m_playerInfo.Position;
		base.transform.position = m_startCameraPos;
		m_param.m_target = base.transform.position;
		m_param.m_target.z = m_playerPosition.z;
		m_param.m_position = base.transform.position;
		m_param.m_upDirection = Vector3.up;
		m_defaultTargetOffset = base.transform.position - m_playerPosition;
		m_defaultTargetOffset.z = 0f;
		base.transform.position = m_param.m_position;
		base.transform.LookAt(m_param.m_target, m_param.m_upDirection);
		m_targetToCamera = m_param.m_position - m_param.m_target;
		GameFollowCamera state = new GameFollowCamera();
		PushCamera(state);
	}

	private void LateUpdate()
	{
		if (m_playerInfo == null || m_playerInfo.IsDead())
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (deltaTime == 0f)
		{
			return;
		}
		foreach (CameraState camera in m_cameraList)
		{
			camera.Update(this, deltaTime);
		}
		int index = m_cameraList.Count - 1;
		if (IsNowInterpolate())
		{
			m_interpolateRate += m_ratePerSec * deltaTime;
			if (m_interpolateTime > 0f)
			{
				m_interpolateTime -= deltaTime;
				if (m_interpolateTime <= 0f)
				{
					if (m_interpolateRate < 0.5f)
					{
						CameraState state = m_cameraList[index];
						PopCamera(state);
					}
					m_interpolateTime = 0f;
				}
			}
			if (m_interpolateRate > 1f)
			{
				m_interpolateRate = 1f;
			}
			else if (m_interpolateRate < 0f)
			{
				m_interpolateRate = 0f;
			}
		}
		index = m_cameraList.Count - 1;
		if (IsNowInterpolate())
		{
			if (m_ratePerSec < 0f)
			{
				CameraState cameraState = m_cameraList[index - 1];
				cameraState.GetCameraParameter(ref m_lowerParam);
				if (m_differenceApproachFlag)
				{
					CameraState cameraState2 = m_cameraList[index];
					cameraState2.GetCameraParameter(ref m_topParam);
					m_differencePos = m_lowerParam.m_position - m_topParam.m_position;
					m_differenceTarget = m_lowerParam.m_target - m_topParam.m_target;
					m_differenceApproachFlag = false;
				}
				float t = Mathf.Max(1f - m_interpolateRate, 0f);
				m_param.m_position = m_lowerParam.m_position - Vector3.Lerp(m_differencePos, Vector3.zero, t);
				m_param.m_target = m_lowerParam.m_target - Vector3.Lerp(m_differenceTarget, Vector3.zero, t);
				m_param.m_upDirection = Vector3.Lerp(m_param.m_upDirection, m_lowerParam.m_upDirection, t);
			}
			else
			{
				CameraState cameraState3 = m_cameraList[index];
				cameraState3.GetCameraParameter(ref m_topParam);
				m_param.m_position = Vector3.Lerp(m_param.m_position, m_topParam.m_position, m_interpolateRate);
				m_param.m_target = Vector3.Lerp(m_param.m_target, m_topParam.m_target, m_interpolateRate);
				m_param.m_upDirection = Vector3.Lerp(m_param.m_upDirection, m_topParam.m_upDirection, m_interpolateRate);
			}
		}
		else
		{
			CameraState cameraState4 = m_cameraList[index];
			cameraState4.GetCameraParameter(ref m_param);
		}
		base.transform.position = m_param.m_position;
		base.transform.LookAt(m_param.m_target, m_param.m_upDirection);
	}

	private void PushNewCamera(CameraType camType, UnityEngine.Object parameter, float interpolateTime)
	{
		CameraState cameraState = CreateNewCamera(camType, parameter);
		if (cameraState != null)
		{
			PushCamera(cameraState);
			StartInterpolate(true, interpolateTime);
			CameraState prevGimmickCamera = GetPrevGimmickCamera();
			if (prevGimmickCamera != null)
			{
				PopCamera(prevGimmickCamera);
			}
		}
	}

	private void PopCamera(CameraType camType, float interpolateTime)
	{
		CameraState cameraByType = GetCameraByType(camType);
		if (cameraByType == null)
		{
			return;
		}
		if (cameraByType == GetTopCamera())
		{
			CameraState prevGimmickCamera = GetPrevGimmickCamera();
			if (prevGimmickCamera != null)
			{
				PopCamera(prevGimmickCamera);
			}
			if (!IsNowInterpolate() || !(m_ratePerSec < 0f))
			{
				StartInterpolate(false, interpolateTime);
			}
		}
		else
		{
			PopCamera(cameraByType);
		}
	}

	private CameraState CreateNewCamera(CameraType camType, UnityEngine.Object parameter)
	{
		CameraState result = null;
		switch (camType)
		{
		case CameraType.DEFAULT:
			result = new GameFollowCamera();
			break;
		case CameraType.LASER:
			result = new LaserCamera();
			break;
		case CameraType.JUMPBOARD:
			result = new JumpBoardCamera();
			break;
		case CameraType.CANNON:
			result = new CannonCamera();
			break;
		case CameraType.LOOP_TERRAIN:
			result = new LoopTerrainCamera();
			break;
		case CameraType.START_ACT:
			result = new StartCamera();
			break;
		}
		return result;
	}

	public CameraState GetCameraByType(CameraType type)
	{
		foreach (CameraState camera in m_cameraList)
		{
			if (camera.GetCameraType() == type)
			{
				return camera;
			}
		}
		return null;
	}

	private CameraState GetTopCamera()
	{
		return m_cameraList[m_cameraList.Count - 1];
	}

	private void PushCamera(CameraState state)
	{
		state.OnEnter(this);
		m_cameraList.Add(state);
	}

	private void PopCamera(CameraState state)
	{
		state.OnLeave(this);
		m_cameraList.Remove(state);
	}

	private void StartInterpolate(bool push, float time)
	{
		m_interpolateTime = time;
		if (push)
		{
			m_interpolateRate = 0f;
			m_ratePerSec = 1f / time;
		}
		else
		{
			m_interpolateRate = 1f;
			m_ratePerSec = 0f - 1f / time;
			m_differenceApproachFlag = true;
		}
	}

	private void OnDrawGizmos()
	{
		foreach (CameraState camera in m_cameraList)
		{
			camera.OnDrawGizmos(this);
		}
	}

	private void OnPushCamera(MsgPushCamera msg)
	{
		PushNewCamera(msg.m_type, msg.m_parameter, msg.m_interpolateTime);
	}

	private void OnPopCamera(MsgPopCamera msg)
	{
		PopCamera(msg.m_type, msg.m_interpolateTime);
	}

	private CameraState GetPrevGimmickCamera()
	{
		CameraState cameraState = null;
		foreach (CameraState camera in m_cameraList)
		{
			CameraType cameraType = camera.GetCameraType();
			if (CameraTypeData.IsGimmickCamera(cameraType))
			{
				if (cameraState != null)
				{
					return cameraState;
				}
				cameraState = camera;
			}
		}
		return null;
	}

	public PlayerInformation GetPlayerInformation()
	{
		return m_playerInfo;
	}

	public CameraParameter GetParameter()
	{
		return m_param;
	}

	public Vector3 GetTargetOffset()
	{
		return m_defaultTargetOffset;
	}

	public Vector3 GetTargetToCamera()
	{
		return m_targetToCamera;
	}

	private bool IsNowInterpolate()
	{
		return m_interpolateTime > 0f && m_cameraList.Count > 1;
	}

	public void OnMsgTutorialResetForRetry(MsgTutorialResetForRetry msg)
	{
		while (GetTopCamera().GetCameraType() != 0)
		{
			PopCamera(GetTopCamera());
		}
		GameFollowCamera gameFollowCamera = GetCameraByType(CameraType.DEFAULT) as GameFollowCamera;
		if (gameFollowCamera != null)
		{
			gameFollowCamera.OnMsgTutorialResetForRetry(this, msg);
		}
	}

	private void WindowFunction(int windowID)
	{
		string empty = string.Empty;
		CameraParameter param = m_param;
		string text = empty;
		empty = text + "POS  :" + param.m_position.x.ToString("F2") + " " + param.m_position.y.ToString("F2") + " " + param.m_position.z.ToString("F2") + " \n";
		text = empty;
		empty = text + "TARGET:" + param.m_target.x.ToString("F2") + " " + param.m_target.y.ToString("F2") + " " + param.m_target.z.ToString("F2") + " \n";
		empty += "\n";
		for (int num = m_cameraList.Count - 1; num >= 0; num--)
		{
			CameraState cameraState = m_cameraList[num];
			empty = empty + cameraState.ToString() + ":\n";
			CameraParameter parameter = default(CameraParameter);
			cameraState.GetCameraParameter(ref parameter);
			text = empty;
			empty = text + "POS  :" + parameter.m_position.x.ToString("F2") + " " + parameter.m_position.y.ToString("F2") + " " + parameter.m_position.z.ToString("F2") + " \n";
			text = empty;
			empty = text + "TARGET:" + parameter.m_target.x.ToString("F2") + " " + parameter.m_target.y.ToString("F2") + " " + parameter.m_target.z.ToString("F2") + " \n";
		}
		if (IsNowInterpolate())
		{
			text = empty;
			empty = text + "Interpolate time:" + m_interpolateTime.ToString("F2") + " rate:" + m_interpolateRate.ToString("F2");
		}
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = empty;
		Rect position = new Rect(10f, 20f, 270f, 280f);
		GUI.Label(position, gUIContent);
	}
}
