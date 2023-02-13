using System.Collections.Generic;
using UnityEngine;

public class TickerWindow : MonoBehaviour
{
	public struct CInfo
	{
		public List<ServerTickerData> tickerList;

		public string labelName;

		public float moveSpeed;

		public float moveSpeedUp;
	}

	private enum Mode
	{
		Idle,
		Start,
		Wait1,
		SpeedUpMove,
		Wait2,
		Move
	}

	private Mode m_mode;

	private CInfo m_info = default(CInfo);

	private UILabel m_uiLabel;

	private float m_textSize;

	private float m_startPos;

	private int m_count;

	private float m_timer;

	private void Update()
	{
		if (!(m_uiLabel != null))
		{
			return;
		}
		switch (m_mode)
		{
		case Mode.Start:
			m_timer = 1f;
			m_mode = Mode.Wait1;
			break;
		case Mode.Wait1:
			m_timer -= Time.deltaTime;
			if (m_timer <= 0f)
			{
				m_mode = Mode.SpeedUpMove;
			}
			break;
		case Mode.SpeedUpMove:
		{
			UpdateMovePos(m_info.moveSpeedUp * Time.deltaTime * 60f);
			Vector3 localPosition2 = m_uiLabel.transform.localPosition;
			float x2 = localPosition2.x;
			float num2 = 24f;
			if (x2 < num2)
			{
				SetResetPos(num2);
				m_timer = 1f;
				m_mode = Mode.Wait2;
			}
			break;
		}
		case Mode.Wait2:
			m_timer -= Time.deltaTime;
			if (m_timer <= 0f)
			{
				m_textSize = m_uiLabel.width;
				m_mode = Mode.Move;
			}
			break;
		case Mode.Move:
		{
			UpdateMovePos(m_info.moveSpeed * Time.deltaTime * 60f);
			Vector3 localPosition = m_uiLabel.transform.localPosition;
			float x = localPosition.x;
			float num = 0f - m_textSize;
			if (x < num)
			{
				SetupNext();
				m_mode = Mode.Start;
			}
			break;
		}
		}
	}

	public void Setup(CInfo info)
	{
		m_info = info;
		m_count = 0;
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			m_uiLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, m_info.labelName);
			if (m_uiLabel != null && m_info.tickerList != null && m_info.tickerList.Count > 0)
			{
				m_uiLabel.text = m_info.tickerList[m_count].Param;
				Vector4 clipRange = component.clipRange;
				m_startPos = clipRange.z;
				SetResetPos(m_startPos);
				m_mode = Mode.Start;
			}
		}
	}

	private void SetupNext()
	{
		if (m_info.tickerList != null && m_uiLabel != null)
		{
			m_count++;
			if (m_count >= m_info.tickerList.Count)
			{
				m_count = 0;
			}
			m_uiLabel.text = m_info.tickerList[m_count].Param;
		}
		SetResetPos(m_startPos);
	}

	public void ResetWindow()
	{
		SetResetPos(m_startPos);
		m_mode = Mode.Idle;
	}

	private void SetResetPos(float startPos)
	{
		if (m_uiLabel != null)
		{
			Vector3 localPosition = m_uiLabel.transform.localPosition;
			m_uiLabel.transform.localPosition = new Vector3(startPos, localPosition.y, localPosition.z);
		}
	}

	private void UpdateMovePos(float move)
	{
		if (m_uiLabel != null)
		{
			m_uiLabel.transform.localPosition -= Vector3.right * move;
		}
	}
}
