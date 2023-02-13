using UnityEngine;

namespace Player
{
	public class CharacterBarrier : MonoBehaviour
	{
		private GameObject m_effect;

		private bool m_bigSize;

		public bool IsBigSize
		{
			get
			{
				return m_bigSize;
			}
			set
			{
				m_bigSize = value;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void SetEnable()
		{
			base.gameObject.SetActive(true);
			m_effect = StateUtil.CreateEffect(this, (!m_bigSize) ? "ef_pl_barrier_lv1_s01" : "ef_pl_barrier_lv1_l01", false);
			if (m_effect != null)
			{
				StateUtil.SetObjectLocalPositionToCenter(this, m_effect);
			}
			SoundManager.SePlay("obj_item_barrier");
		}

		public void SetDisable()
		{
			if (m_effect != null)
			{
				Object.Destroy(m_effect);
				m_effect = null;
			}
			base.gameObject.SetActive(false);
		}

		private void Stop()
		{
			SetDisable();
			SoundManager.SePlay("obj_item_barrier_brk");
			GameObject gameObject = base.gameObject.transform.parent.gameObject;
			StateUtil.CreateEffect(this, gameObject, "ef_pl_barrier_cancel_s01", true, ResourceCategory.COMMON_EFFECT);
		}

		public void SetNotDraw(bool value)
		{
			if (m_effect != null)
			{
				m_effect.SetActive(!value);
			}
		}

		public void Damaged()
		{
			Stop();
		}
	}
}
