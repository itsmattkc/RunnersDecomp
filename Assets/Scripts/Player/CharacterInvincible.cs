using Message;
using UnityEngine;

namespace Player
{
	public class CharacterInvincible : MonoBehaviour
	{
		private const string EffectNameS = "ef_pl_invincible_s01";

		private const string EffectNameL = "ef_pl_invincible_l01";

		private GameObject m_effect;

		private float m_time;

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
			m_effect = StateUtil.CreateEffect(this, (!m_bigSize) ? "ef_pl_invincible_s01" : "ef_pl_invincible_l01", false);
			if ((bool)base.transform.parent)
			{
				CapsuleCollider component = base.transform.parent.GetComponent<CapsuleCollider>();
				if ((bool)component)
				{
					base.transform.localPosition = component.center;
				}
			}
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			if (m_time > 0f)
			{
				m_time -= Time.deltaTime;
				if (m_time <= 0f)
				{
					SetDisable();
				}
			}
		}

		public void SetEnable()
		{
			m_time = -1f;
		}

		public void SetActive(float time)
		{
			base.gameObject.SetActive(true);
			if (m_effect != null && !m_effect.activeInHierarchy)
			{
				m_effect.SetActive(true);
			}
			SetTime(time);
			MsgInvincible value = new MsgInvincible(MsgInvincible.Mode.Start);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgInvincible", value, SendMessageOptions.DontRequireReceiver);
			if (StageAbilityManager.Instance != null)
			{
				StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.ITEM_TIME);
			}
		}

		public void SetDisable()
		{
			StateUtil.SendMessageToTerminateItem(ItemType.INVINCIBLE);
			base.gameObject.SetActive(false);
			GameObject gameObject = base.gameObject.transform.parent.gameObject;
			StateUtil.CreateEffect(this, gameObject, "ef_pl_invincible_cancel_s01", true, ResourceCategory.COMMON_EFFECT);
			MsgInvincible value = new MsgInvincible(MsgInvincible.Mode.End);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgInvincible", value, SendMessageOptions.DontRequireReceiver);
		}

		public void SetNotDraw(bool value)
		{
			if (m_effect != null)
			{
				bool flag = !value;
				if (m_effect.activeInHierarchy != flag)
				{
					m_effect.SetActive(flag);
				}
			}
		}

		public void SetTime(float time)
		{
			m_time = time;
		}
	}
}
