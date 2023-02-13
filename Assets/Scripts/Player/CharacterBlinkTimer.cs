using UnityEngine;

namespace Player
{
	public class CharacterBlinkTimer : MonoBehaviour
	{
		private float m_timer;

		private CharacterState m_context;

		private void Start()
		{
		}

		private void OnDestroy()
		{
			End();
		}

		public void Setup(CharacterState ctx, float damageTime)
		{
			m_context = ctx;
			m_timer = damageTime;
			m_context.SetStatus(Status.Damaged, true);
			base.enabled = true;
		}

		public void End()
		{
			if ((bool)m_context)
			{
				m_context.SetVisibleBlink(false);
				m_context.SetStatus(Status.Damaged, false);
			}
			base.enabled = false;
		}

		private void FixedUpdate()
		{
			if (!(m_context == null))
			{
				m_timer -= Time.deltaTime;
				if (m_timer <= 0f)
				{
					End();
				}
				else if (Mathf.FloorToInt(m_timer * 100f) % 20 > 10)
				{
					m_context.SetVisibleBlink(true);
				}
				else
				{
					m_context.SetVisibleBlink(false);
				}
			}
		}
	}
}
