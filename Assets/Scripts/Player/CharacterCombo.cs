using UnityEngine;

namespace Player
{
	public class CharacterCombo : MonoBehaviour
	{
		private CharacterMovement m_movement;

		private GameObject m_effect;

		private PlayerInformation m_information;

		private float m_time;

		private bool m_requestEnd;

		private void Start()
		{
			m_movement = base.transform.parent.gameObject.GetComponent<CharacterMovement>();
			m_information = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		}

		private void OnEnable()
		{
			SetCombo(true);
		}

		private void OnDisable()
		{
			SetCombo(false);
		}

		public void SetEnable()
		{
			base.gameObject.SetActive(true);
			m_time = -1f;
			m_effect = StateUtil.CreateEffect(this, "ef_pl_combobonus01", false);
			if (m_effect != null)
			{
				StateUtil.SetObjectLocalPositionToCenter(this, m_effect);
			}
			if (StageAbilityManager.Instance != null)
			{
				StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.COMBO_TIME);
				StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.ITEM_TIME);
			}
			SoundManager.SePlay("obj_combo_loop");
		}

		public void SetDisable()
		{
			StateUtil.SendMessageToTerminateItem(ItemType.COMBO);
			if (m_effect != null)
			{
				StateUtil.DestroyParticle(m_effect, 1f);
				m_effect = null;
			}
			base.gameObject.SetActive(false);
			m_requestEnd = false;
		}

		private void Update()
		{
			if (m_movement != null && m_movement.IsOnGround() && m_requestEnd)
			{
				SetDisable();
			}
			if (m_time > 0f)
			{
				m_time -= Time.deltaTime;
				if (m_time <= 0f)
				{
					RequestEnd();
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

		private void SetCombo(bool flag)
		{
			if (m_information == null)
			{
				m_information = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
			}
			if (m_information != null)
			{
				m_information.SetCombo(flag);
			}
		}
	}
}
