using UnityEngine;

public class EffectPlayTime : MonoBehaviour
{
	private float m_passedTime;

	public float m_endTime = 1f;

	private void Update()
	{
		m_passedTime += Time.deltaTime;
		if (m_passedTime > m_endTime)
		{
			base.gameObject.SetActive(false);
			if (StageEffectManager.Instance != null)
			{
				StageEffectManager.Instance.SleepEffect(base.gameObject);
			}
		}
	}

	public void PlayEffect()
	{
		m_passedTime = 0f;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.time = 0f;
			particleSystem.Play();
		}
	}
}
