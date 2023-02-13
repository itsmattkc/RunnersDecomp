using App;
using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class CharacterInput : MonoBehaviour
	{
		private enum TouchedStatus
		{
			touched,
			hold,
			released
		}

		private enum Status
		{
			STATUS_STICKENABLED,
			STATUS_DISABLE
		}

		private struct History
		{
			public Bitset32 touchedStatus;

			public float time;
		}

		private Bitset32 m_status;

		private Bitset32 m_touchedStatus;

		private Vector3 m_stick;

		private byte m_historyIndex;

		private History[] m_history;

		private LevelInformation m_levelInformation;

		private void Start()
		{
			m_status.Set(0, true);
			m_stick = Vector3.zero;
			if (m_levelInformation == null)
			{
				m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
			}
		}

		private void Update()
		{
			if (Math.NearZero(Time.deltaTime) || Math.NearZero(Time.timeScale) || m_levelInformation.RequestPause || m_levelInformation.RequestEqitpItem || m_levelInformation.RequestCharaChange)
			{
				return;
			}
			m_touchedStatus.Reset();
			if (!m_status.Test(1))
			{
				if (m_status.Test(0))
				{
					float axis = Input.GetAxis("Vertical");
					float axis2 = Input.GetAxis("Horizontal");
					Vector3 vector = m_stick = Camera.main.transform.right * axis2 + Camera.main.transform.up * axis;
				}
				else
				{
					m_stick = Vector3.zero;
				}
				if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
				{
					m_touchedStatus.Set(0, true);
					m_touchedStatus.Set(1, true);
				}
				else if (Input.GetMouseButton(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)))
				{
					m_touchedStatus.Set(1, true);
				}
				if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
				{
					m_touchedStatus.Set(2, true);
				}
			}
			if (m_history != null)
			{
				m_historyIndex++;
				m_history[m_historyIndex].touchedStatus = m_touchedStatus;
				m_history[m_historyIndex].time = Time.deltaTime;
			}
		}

		public Vector3 GetStick()
		{
			return m_stick;
		}

		public bool IsTouched()
		{
			return m_touchedStatus.Test(0);
		}

		public bool IsHold()
		{
			return m_touchedStatus.Test(1);
		}

		public bool IsReleased()
		{
			return m_touchedStatus.Test(2);
		}

		public void CreateHistory()
		{
			m_history = new History[256];
			m_historyIndex = 0;
		}

		public bool IsTouchedLastSecond(float lastSecond)
		{
			float num = 0f;
			for (byte historyIndex = m_historyIndex; num < lastSecond; num += m_history[historyIndex].time)
			{
				if (m_history[historyIndex].touchedStatus.Test(0))
				{
					return true;
				}
				if (m_history[historyIndex].time <= 0f)
				{
					break;
				}
			}
			return false;
		}

		private void OnInputDisable(MsgDisableInput msg)
		{
			m_status.Set(1, msg.m_disable);
		}
	}
}
