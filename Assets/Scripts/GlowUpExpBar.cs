using System.Collections.Generic;
using Text;
using UnityEngine;

public class GlowUpExpBar : MonoBehaviour
{
	public class ExpInfo
	{
		public int level;

		public int cost;

		public int exp;
	}

	public delegate void LevelUpCallback(int level);

	public delegate void EndCallback();

	private UISlider m_baseSlider;

	private UISlider m_glowUpSlider;

	private ExpInfo m_startInfo = new ExpInfo();

	private ExpInfo m_endInfo = new ExpInfo();

	private LevelUpCallback m_levelUpCallback;

	private EndCallback m_endCallback;

	private HudInterpolateConstant m_interpolate = new HudInterpolateConstant();

	private bool m_isPlaying;

	private UILabel m_expLabel;

	private List<int> m_costList;

	private static readonly int RATIO_TO_VALUE = 10000;

	private static readonly float BAR_SPEED_PER_SEC = 0.333333343f * (float)RATIO_TO_VALUE;

	public void SetBaseSlider(UISlider slider)
	{
		if (!(slider == null))
		{
			m_baseSlider = slider;
			m_baseSlider.value = 0f;
		}
	}

	public void SetGlowUpSlider(UISlider slider)
	{
		if (!(slider == null))
		{
			m_glowUpSlider = slider;
			m_glowUpSlider.value = 0f;
		}
	}

	public void SetStartExp(ExpInfo startInfo)
	{
		if (startInfo != null)
		{
			m_startInfo = startInfo;
			float num = CalcSliderValue(startInfo);
			if (m_baseSlider != null)
			{
				m_baseSlider.value = num;
			}
			if (m_glowUpSlider != null)
			{
				m_glowUpSlider.value = num;
			}
			int cost = m_startInfo.cost;
			int exp = (int)((float)cost * num);
			string text = CalcExpString(exp, cost);
			if (!m_expLabel.gameObject.activeSelf)
			{
				m_expLabel.gameObject.SetActive(true);
			}
			m_expLabel.text = text;
		}
	}

	public void SetExpLabel(UILabel expLabel)
	{
		m_expLabel = expLabel;
	}

	public void SetEndExp(ExpInfo endInfo)
	{
		if (endInfo != null)
		{
			m_endInfo = endInfo;
		}
	}

	public void SetCallback(LevelUpCallback levelUpCallback, EndCallback endCallback)
	{
		m_levelUpCallback = levelUpCallback;
		m_endCallback = endCallback;
	}

	public void SetLevelUpCostList(List<int> expList)
	{
		if (expList == null)
		{
			return;
		}
		m_costList = new List<int>();
		foreach (int exp in expList)
		{
			m_costList.Add(exp);
		}
	}

	public void PlayStart()
	{
		int startValue = (int)(((float)m_startInfo.level + CalcSliderValue(m_startInfo)) * (float)RATIO_TO_VALUE);
		int endValue = (int)(((float)m_endInfo.level + CalcSliderValue(m_endInfo)) * (float)RATIO_TO_VALUE);
		m_interpolate.Setup(startValue, endValue, BAR_SPEED_PER_SEC);
		if (m_baseSlider != null)
		{
			m_baseSlider.value = CalcSliderValue(m_startInfo);
		}
		if (m_glowUpSlider != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_thumb");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
		m_isPlaying = true;
	}

	public void PlaySkip()
	{
		if (m_interpolate != null)
		{
			int currentValue = m_interpolate.CurrentValue;
			int num = currentValue / RATIO_TO_VALUE;
			int num2 = num + 1;
			int forceValue = num2 * RATIO_TO_VALUE - 1;
			m_interpolate.SetForceValue(forceValue);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_isPlaying)
		{
			return;
		}
		int num = m_interpolate.Update(Time.deltaTime);
		int prevValue = m_interpolate.PrevValue;
		float num2 = 0f;
		if (m_glowUpSlider != null)
		{
			float num3 = (float)num / (float)RATIO_TO_VALUE;
			int num4 = num / RATIO_TO_VALUE;
			float num5 = num3 - (float)num4;
			m_glowUpSlider.value = num5;
			num2 = num5;
		}
		int num6 = num / RATIO_TO_VALUE;
		int num7 = prevValue / RATIO_TO_VALUE;
		if (num6 > num7)
		{
			if (m_levelUpCallback != null)
			{
				m_levelUpCallback(num6);
			}
			if (m_baseSlider != null)
			{
				m_baseSlider.value = 0f;
			}
			if (m_costList != null && m_costList.Count > 0 && num7 != m_startInfo.level)
			{
				int item = m_costList[0];
				m_costList.Remove(item);
			}
		}
		if (m_interpolate.IsEnd)
		{
			m_isPlaying = false;
			if (m_endCallback != null)
			{
				m_endCallback();
			}
			if (m_glowUpSlider != null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_thumb");
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
		}
		if (m_expLabel != null)
		{
			string text = string.Empty;
			if (!m_isPlaying)
			{
				int cost = m_endInfo.cost;
				int exp = m_endInfo.exp;
				text = CalcExpString(exp, cost);
			}
			else if (num6 == m_startInfo.level)
			{
				int cost2 = m_startInfo.cost;
				int exp2 = (int)((float)cost2 * num2);
				text = CalcExpString(exp2, cost2);
			}
			else if (num6 == m_endInfo.level)
			{
				int cost3 = m_endInfo.cost;
				int exp3 = (int)((float)cost3 * num2);
				text = CalcExpString(exp3, cost3);
			}
			else if (m_costList != null && m_costList.Count > 0)
			{
				int num8 = m_costList[0];
				int exp4 = (int)((float)num8 * num2);
				text = CalcExpString(exp4, num8);
			}
			if (!m_expLabel.gameObject.activeSelf)
			{
				m_expLabel.gameObject.SetActive(true);
			}
			m_expLabel.text = text;
		}
	}

	private static float CalcSliderValue(ExpInfo info)
	{
		int cost = info.cost;
		int exp = info.exp;
		if (cost == 0)
		{
			return 0f;
		}
		return (float)exp / (float)cost;
	}

	private static string CalcExpString(int exp, int cost)
	{
		string formatNumString = HudUtility.GetFormatNumString(exp);
		string formatNumString2 = HudUtility.GetFormatNumString(cost);
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MileageMap", "point");
		text.ReplaceTag("{VALUE}", formatNumString);
		string text2 = text.text;
		string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Item", "ring").text;
		formatNumString2 += text3;
		return text2 + " / " + formatNumString2;
	}
}
