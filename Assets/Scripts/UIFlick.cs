using Message;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/UIFlck")]
public class UIFlick : MonoBehaviour
{
	private const float FLICK_DISTANCE_THRESHOLD_VALUE = 10f;

	private const float FLICK_TIME_THRESHOLD_VALUE = 0.3f;

	private Vector2 m_first_touch_pos = Vector2.zero;

	private Vector2 m_base_point = Vector2.zero;

	private float m_start_time;

	private GameObject m_target_obj;

	private string m_method_name = string.Empty;

	private bool m_distance_flag;

	public float GetDragDistance()
	{
		return m_base_point.x - m_first_touch_pos.x;
	}

	public void SetCallBack(GameObject obj, string method_name)
	{
		m_target_obj = obj;
		m_method_name = method_name;
	}

	private void SendMessage(bool right_flick_flag)
	{
		if (m_target_obj != null)
		{
			FlickType type = (!right_flick_flag) ? FlickType.LEFT : FlickType.RIGHT;
			MsgUIFlick value = new MsgUIFlick(type);
			m_target_obj.SendMessage(m_method_name, value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		int touchCount = Input.touchCount;
		if (touchCount > 0)
		{
			Touch touch = Input.touches[0];
			Vector2 position = touch.position;
			switch (touch.phase)
			{
			case TouchPhase.Canceled:
				break;
			case TouchPhase.Began:
				OnTouchBegan(position);
				break;
			case TouchPhase.Moved:
				OnTouchMove(position);
				break;
			case TouchPhase.Stationary:
				OnTouchStationary(position);
				break;
			case TouchPhase.Ended:
				OnTouchEnd(position);
				break;
			}
		}
	}

	private void UpdateTouchData(Vector2 position, bool first_touch_flag)
	{
		if (first_touch_flag)
		{
			m_first_touch_pos = position;
		}
		m_base_point = position;
		m_start_time = Time.realtimeSinceStartup;
	}

	private void OnTouchBegan(Vector2 position)
	{
		bool first_touch_flag = true;
		UpdateTouchData(position, first_touch_flag);
	}

	private void OnTouchMove(Vector2 position)
	{
		float num = Mathf.Abs(position.x - m_base_point.x);
		m_distance_flag = (num > 10f);
	}

	private void OnTouchStationary(Vector2 position)
	{
		if (m_distance_flag)
		{
			float num = Mathf.Abs(position.x - m_base_point.x);
			if (num < 10f)
			{
				m_distance_flag = false;
			}
		}
	}

	private void OnTouchEnd(Vector2 position)
	{
		if (!m_distance_flag)
		{
			return;
		}
		float num = Time.realtimeSinceStartup - m_start_time;
		if (num < 0.3f)
		{
			float num2 = position.x - m_base_point.x;
			bool right_flick_flag = true;
			if (num2 < 0f)
			{
				right_flick_flag = false;
				SendMessage(right_flick_flag);
				Debug.Log("Left Flick Success");
			}
			else
			{
				SendMessage(right_flick_flag);
				Debug.Log("Right Flick Success");
			}
		}
	}
}
