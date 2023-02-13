using UnityEngine;

namespace Player
{
	public class CharacterLoopEffect : MonoBehaviour
	{
		private string m_effectname;

		private string m_sename;

		private SoundManager.PlayId m_seID;

		private float m_stopTimer;

		private GameObject m_effect;

		private bool m_valid;

		private ResourceCategory m_category = ResourceCategory.COMMON_EFFECT;

		private void Start()
		{
			m_effect = StateUtil.CreateEffect(this, m_effectname, false, m_category);
		}

		private void OnEnable()
		{
			if (m_sename != null)
			{
				m_seID = SoundManager.SePlay(m_sename);
			}
		}

		private void OnDisable()
		{
			if (m_sename != null)
			{
				SoundManager.SeStop(m_seID);
				m_seID = SoundManager.PlayId.NONE;
			}
		}

		private void Update()
		{
			if (!m_valid)
			{
				if (m_stopTimer > 0f)
				{
					m_stopTimer -= Time.deltaTime;
					return;
				}
				base.gameObject.SetActive(false);
				m_stopTimer = 0f;
			}
		}

		public void SetValid(bool valid)
		{
			if (valid && !m_valid)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(true);
				}
				StateUtil.PlayParticle(m_effect);
			}
			else if (!valid && m_valid)
			{
				base.gameObject.SetActive(false);
			}
			m_valid = valid;
			m_stopTimer = 0f;
		}

		public void Stop(float stopTime)
		{
			if (m_effect != null && stopTime > 0f)
			{
				StateUtil.StopParticle(m_effect);
				m_stopTimer = stopTime;
			}
			m_valid = false;
		}

		public void Setup(string effectname, ResourceCategory category)
		{
			m_effectname = effectname;
			m_category = category;
		}

		public void SetSE(string sename)
		{
			m_sename = sename;
		}
	}
}
