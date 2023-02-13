public class PanelFade
{
	private const int InterpolateEndValue = 10000;

	private UIPanel m_panel;

	private HudValueInterpolate m_hudInterpolate;

	public void Setup(UIPanel panel)
	{
		m_panel = panel;
		m_hudInterpolate = new HudValueInterpolate();
	}

	public void PlayStart(float fadeTime, bool isFadeIn)
	{
		if (isFadeIn)
		{
			m_hudInterpolate.Setup(0L, 10000L, fadeTime);
		}
		else
		{
			m_hudInterpolate.Setup(10000L, 0L, fadeTime);
		}
	}

	public void Update(float deltaTime)
	{
		if (m_hudInterpolate != null && !m_hudInterpolate.IsEnd)
		{
			long num = m_hudInterpolate.Update(deltaTime);
			m_panel.alpha = num / 10000;
		}
	}

	public bool IsEndFade()
	{
		if (m_hudInterpolate != null && !m_hudInterpolate.IsEnd)
		{
			return false;
		}
		return true;
	}
}
