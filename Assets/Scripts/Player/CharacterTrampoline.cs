using UnityEngine;

namespace Player
{
	public class CharacterTrampoline : MonoBehaviour
	{
		private bool m_requestEnd;

		private GameObject m_effect;

		private float m_time;

		private void Start()
		{
		}

		private void OnEnable()
		{
			m_requestEnd = false;
			m_effect = StateUtil.CreateEffect(this, "ef_pl_trampoline_s01", false);
			StateUtil.SetObjectLocalPositionToCenter(this, m_effect);
		}

		private void OnDisable()
		{
			if (m_effect != null)
			{
				StateUtil.DestroyParticle(m_effect, 1f);
				m_effect = null;
			}
		}

		public void SetEnable()
		{
			base.gameObject.SetActive(true);
		}

		public void SetDisable()
		{
			StateUtil.SendMessageToTerminateItem(ItemType.TRAMPOLINE);
			if (m_effect != null)
			{
				StateUtil.DestroyParticle(m_effect, 1f);
				m_effect = null;
			}
			base.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (m_requestEnd)
			{
				base.gameObject.SetActive(false);
			}
			else if (m_time > 0f)
			{
				m_time -= Time.deltaTime;
				if (m_time <= 0f)
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		public void SetTime(float time)
		{
			m_time = time;
		}

		public void RequestEnd()
		{
			m_requestEnd = true;
		}
	}
}
