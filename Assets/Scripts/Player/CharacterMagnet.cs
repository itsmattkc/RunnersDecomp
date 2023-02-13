using App;
using Message;
using UnityEngine;

namespace Player
{
	public class CharacterMagnet : MonoBehaviour
	{
		private const string EffectNameS = "ef_pl_magnet_s01";

		private const string EffectNameL = "ef_pl_magnet_l01";

		private GameObject m_effect;

		private Vector3 m_defaultOffset;

		private float m_defaultRadius;

		private float m_time;

		private bool m_bigSize;

		[SerializeField]
		private bool m_forChaoAbility;

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

		public bool ForChaoAbility
		{
			get
			{
				return m_forChaoAbility;
			}
		}

		private void Awake()
		{
			SphereCollider component = GetComponent<SphereCollider>();
			if ((bool)component)
			{
				m_defaultOffset = component.center;
				m_defaultRadius = component.radius;
			}
		}

		private void Start()
		{
		}

		public void SetEnable()
		{
			base.gameObject.SetActive(true);
			m_time = -1f;
			if (!m_forChaoAbility)
			{
				m_effect = StateUtil.CreateEffect(this, (!m_bigSize) ? "ef_pl_magnet_s01" : "ef_pl_magnet_l01", false);
				if (m_effect != null)
				{
					StateUtil.SetObjectLocalPositionToCenter(this, m_effect);
				}
				if (StageAbilityManager.Instance != null)
				{
					StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.MAGNET_TIME);
					StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.ITEM_TIME);
					StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.MAGNET_RANGE);
				}
				SoundManager.SePlay("obj_magnet");
			}
		}

		public void SetDisable()
		{
			if (!m_forChaoAbility)
			{
				StateUtil.SendMessageToTerminateItem(ItemType.MAGNET);
			}
			if (m_effect != null)
			{
				StateUtil.DestroyParticle(m_effect, 1f);
				m_effect = null;
			}
			base.gameObject.SetActive(false);
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

		private void SetRadiusAndOffset(float radius, Vector3 offset)
		{
			SphereCollider component = GetComponent<SphereCollider>();
			if ((bool)component)
			{
				if (!Math.NearEqual(radius, component.radius))
				{
					component.radius = radius;
				}
				if (!Math.NearZero((offset - component.center).sqrMagnitude))
				{
					component.center = offset;
				}
				m_defaultOffset = component.center;
				m_defaultRadius = component.radius;
				if (StageAbilityManager.Instance != null)
				{
					float num = StageAbilityManager.Instance.GetChaoAbliltyValue(ChaoAbility.MAGNET_RANGE, 100f) / 100f;
					component.radius = m_defaultRadius * num;
				}
			}
		}

		public void SetDefaultRadiusAndOffset()
		{
			SetRadiusAndOffset(m_defaultRadius, m_defaultOffset);
		}

		public void SetTime(float time)
		{
			m_time = time;
		}

		private void OnTriggerEnter(Collider other)
		{
			MsgOnDrawingRings value = new MsgOnDrawingRings();
			other.gameObject.SendMessage("OnDrawingRings", value, SendMessageOptions.DontRequireReceiver);
		}
	}
}
