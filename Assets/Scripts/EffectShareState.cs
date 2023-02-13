using UnityEngine;

public class EffectShareState : MonoBehaviour
{
	public enum State
	{
		Sleep,
		Active
	}

	private State m_state;

	public EffectPlayType m_effectType;

	public bool IsSleep()
	{
		return m_state == State.Sleep;
	}

	public void SetState(State state)
	{
		m_state = state;
	}

	private void Start()
	{
		base.enabled = false;
	}
}
