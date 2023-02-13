using App;
using Message;
using UnityEngine;

namespace Player
{
	public class CharacterMagnetPhantom : MonoBehaviour
	{
		private Vector3 m_defaultOffset;

		private float m_defaultRadius;

		private bool m_offDrawing;

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
					StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.MAGNET_RANGE);
				}
			}
		}

		public void SetDefaultRadiusAndOffset()
		{
			SetRadiusAndOffset(m_defaultRadius, m_defaultOffset);
		}

		public void SetOffDrawing(bool value)
		{
			m_offDrawing = value;
			SphereCollider component = GetComponent<SphereCollider>();
			if ((bool)component)
			{
				component.enabled = !m_offDrawing;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!m_offDrawing)
			{
				MsgOnDrawingRings value = new MsgOnDrawingRings();
				GameObjectUtil.SendDelayedMessageToGameObject(other.gameObject, "OnDrawingRings", value);
			}
		}
	}
}
