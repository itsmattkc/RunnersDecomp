using System.Collections.Generic;
using UnityEngine;

public class RouletteBoard : RoulettePartsBase
{
	public const int ROULETTE_SPIN_MIN_ROT = 2;

	public const float ROULETTE_SPIN_SPEED = 10f;

	public const float ROULETTE_SPIN_SLOW_SPEED_RATE = 0.4f;

	public const float ROULETTE_SPIN_SLOW_SPEED_LAST = 0.025f;

	public const float ROULETTE_SPIN_SLOW_DEG = 180f;

	public const float ROULETTE_SPIN_SKIP_POINT_RATE = 0.5f;

	public const float ROULETTE_SPIN_MAX = 9999999f;

	private const float ARROW_ROTATION_X = 42f;

	private const float ARROW_ROTATION_Y = 9.139092E-05f;

	[SerializeField]
	private GameObject m_arrow;

	[SerializeField]
	private List<RouletteBoardPattern> m_pattern;

	[SerializeField]
	private RouletteItem m_orgRouletteItem;

	private float m_currentDegree;

	private float m_degreeSlow;

	private float m_degreeSlowLast;

	private float m_currentDegreeMax;

	private float m_timeRate = 1f;

	private float m_arrowSpeed;

	private int m_currentArrowPos = -1;

	private RouletteBoardPattern m_currentBoardPattern;

	private float ONE_FPS_TIME = 0.0166666675f;

	public ServerWheelOptionsData wheelData
	{
		get
		{
			if (m_parent == null)
			{
				return null;
			}
			return m_parent.wheelData;
		}
	}

	public override void Setup(RouletteTop parent)
	{
		base.Setup(parent);
		m_isEffectLock = false;
		if (m_parent != null && m_parent.wheelData != null && m_parent.wheelData.category == RouletteCategory.ITEM)
		{
			m_isEffectLock = true;
		}
		ONE_FPS_TIME = 1f / (float)Application.targetFrameRate;
		SetupBoard(m_parent.wheelData);
		SetupArrow(m_parent.wheelData);
		UpdateEffectSetting();
	}

	public override void OnUpdateWheelData(ServerWheelOptionsData data)
	{
		m_isEffectLock = false;
		if (data != null && data.category == RouletteCategory.ITEM)
		{
			m_isEffectLock = true;
		}
		SetupBoard(data);
		SetupArrow(data);
		UpdateEffectSetting();
	}

	private void SetupBoard(ServerWheelOptionsData data)
	{
		if (data == null)
		{
			return;
		}
		int rouletteBoardPattern = data.GetRouletteBoardPattern();
		if (m_pattern == null || m_pattern.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_pattern.Count; i++)
		{
			RouletteBoardPattern rouletteBoardPattern2 = m_pattern[i];
			if (rouletteBoardPattern2 != null)
			{
				if (rouletteBoardPattern == i)
				{
					rouletteBoardPattern2.Setup(this, m_orgRouletteItem);
					m_currentBoardPattern = rouletteBoardPattern2;
				}
				else
				{
					rouletteBoardPattern2.Reset();
				}
			}
		}
	}

	private void SetupArrow(ServerWheelOptionsData data)
	{
		if (!(m_arrow != null))
		{
			return;
		}
		m_arrow.SetActive(true);
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_arrow, "img_roulette_arrow_0");
		if (uISprite != null && data != null)
		{
			uISprite.spriteName = data.GetRouletteArrowSprite();
		}
		m_currentDegree = 0f;
		m_currentArrowPos = -1;
		m_arrow.transform.rotation = Quaternion.Euler(42f, 9.139092E-05f, 0f);
		if (m_currentBoardPattern != null)
		{
			int cellIndex = m_currentBoardPattern.GetCellIndex(m_currentDegree);
			if (cellIndex >= 0)
			{
				m_currentArrowPos = cellIndex;
				m_currentBoardPattern.SetCurrentCell(m_currentArrowPos);
			}
		}
	}

	protected override void UpdateParts()
	{
		if (!base.isSpin || !(m_currentDegreeMax > 0f) || !(m_arrow != null))
		{
			return;
		}
		bool flag = false;
		if (m_currentDegree < m_currentDegreeMax)
		{
			float arrowMove = GetArrowMove();
			if (m_arrowSpeed > arrowMove)
			{
				flag = true;
			}
			m_currentDegree += arrowMove;
			if (m_currentDegreeMax <= m_currentDegree)
			{
				if (m_parent != null)
				{
					m_parent.OnRouletteSpinEnd();
				}
				m_currentDegree = m_currentDegreeMax;
				int currentDegreeRot = GetCurrentDegreeRot();
				if (currentDegreeRot > 0)
				{
					m_currentDegree -= (float)currentDegreeRot * 360f;
				}
				m_currentDegreeMax = 0f;
			}
		}
		if (m_partsUpdateCount % 2 == 0L || flag)
		{
			int cellIndex = m_currentBoardPattern.GetCellIndex(m_currentDegree);
			if (cellIndex >= 0)
			{
				if (m_currentArrowPos != cellIndex)
				{
					base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Arrow, 0f);
				}
				m_currentArrowPos = cellIndex;
				m_currentBoardPattern.SetCurrentCell(m_currentArrowPos);
			}
		}
		m_arrow.transform.rotation = Quaternion.Euler(42f, 9.139092E-05f, 0f - m_currentDegree);
	}

	public override void UpdateEffectSetting()
	{
		if (m_pattern != null && m_pattern.Count > 0)
		{
			for (int i = 0; i < m_pattern.Count; i++)
			{
				RouletteBoardPattern rouletteBoardPattern = m_pattern[i];
				if (rouletteBoardPattern != null)
				{
					rouletteBoardPattern.UpdateEffectSetting();
				}
			}
		}
		if (!base.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BOARD))
		{
			m_isEffectLock = true;
		}
	}

	public override void DestroyParts()
	{
		if (m_currentBoardPattern != null)
		{
			m_currentBoardPattern.Reset();
		}
		m_currentBoardPattern = null;
		base.DestroyParts();
	}

	public override void OnSpinStart()
	{
		ONE_FPS_TIME = 1f / (float)Application.targetFrameRate;
		m_degreeSlow = 180f;
		m_degreeSlowLast = 1f;
		m_currentDegreeMax = 9999999f;
	}

	public override void OnSpinSkip()
	{
		if (base.spinDecisionIndex == -1)
		{
			return;
		}
		float num = m_currentDegreeMax - m_currentDegree;
		if (num >= m_degreeSlow * 0.5f)
		{
			float num2 = (int)((num - m_degreeSlow * 0.5f) / 360f);
			if (num2 > 0f)
			{
				m_currentDegree += 360f * num2;
			}
		}
	}

	public override void OnSpinDecision()
	{
		if (base.spinDecisionIndex == -1)
		{
			return;
		}
		int currentDegreeRot = GetCurrentDegreeRot();
		int num = 3;
		if (currentDegreeRot >= 2)
		{
			num = currentDegreeRot + 1;
		}
		if (!(m_currentBoardPattern != null))
		{
			return;
		}
		float startRot;
		float endRot;
		float lastSpeedRate;
		m_currentBoardPattern.GetCellData(base.spinDecisionIndex, out startRot, out endRot, out lastSpeedRate);
		float num2 = endRot - startRot;
		float num3 = 0f;
		float num4 = Random.Range(0f, 1f);
		if (lastSpeedRate > 0.8f)
		{
			num3 = startRot + num2 * num4;
		}
		else
		{
			num4 = 1f - num4 * num4;
			num3 = (((int)(m_currentDegree * 100f) % 2 != 0) ? (endRot - num2 * num4) : (startRot + num2 * num4));
			if (num4 < 0.8f && lastSpeedRate < 0.5f)
			{
				lastSpeedRate = 0.5f;
			}
		}
		float currentDegreeMax = (float)num * 360f + num3;
		m_degreeSlow = 180f + Random.Range(0f, 30f);
		if (lastSpeedRate < 1f)
		{
			m_degreeSlow += Random.Range(10f, 30f);
			if (lastSpeedRate < 0.5f)
			{
				m_degreeSlow += Random.Range(30f, 50f);
			}
		}
		m_currentDegreeMax = currentDegreeMax;
		m_degreeSlowLast = lastSpeedRate;
	}

	public override void OnSpinEnd()
	{
	}

	public override void OnSpinError()
	{
	}

	private float GetLastSlowPoint()
	{
		float result = m_degreeSlow * 0.4f;
		if (m_degreeSlowLast < 1f)
		{
			result = m_degreeSlow * (0.4f + (1f - m_degreeSlowLast) * 0.02f);
		}
		return result;
	}

	private int GetCurrentDegreeRot()
	{
		return (int)(m_currentDegree / 360f);
	}

	private int GetEndDegreeRot()
	{
		float num = m_currentDegreeMax - m_currentDegree;
		int num2 = (int)(num / 360f);
		if (num2 < 0)
		{
			num2 = 0;
		}
		return num2;
	}

	private float GetEndDegreeRotFloat()
	{
		float num = m_currentDegreeMax - m_currentDegree;
		float num2 = num / 360f;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return num2;
	}

	private float GetArrowMove()
	{
		if (m_partsUpdateCount % 7 == 0L || m_arrowSpeed <= 0.5f)
		{
			m_timeRate = Time.deltaTime / ONE_FPS_TIME;
			if (m_timeRate > 1.2f)
			{
				m_timeRate = 1.2f;
			}
			else if (m_timeRate < 0.9f)
			{
				m_timeRate = 0.9f;
			}
			m_arrowSpeed = 10f * m_timeRate;
		}
		float num = m_arrowSpeed;
		float num2 = GetEndDegreeRotFloat();
		if (num2 > 3f)
		{
			num2 = 3f;
		}
		if (num2 > 0f)
		{
			num *= 1f + num2 * 0.25f;
		}
		if (num2 < 1.5f && m_currentDegreeMax - m_currentDegree <= m_degreeSlow)
		{
			float num3 = m_currentDegreeMax - m_currentDegree;
			float num4 = num3 / m_degreeSlow;
			float num5 = 0f;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			num5 = num4;
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			if (num5 < 0.025f * m_degreeSlowLast)
			{
				num5 = 0.025f * m_degreeSlowLast;
			}
			num *= num5;
		}
		return num;
	}
}
