using System;
using UnityEngine;

namespace Chao
{
	public class ChaoHovering : ChaoHoveringBase
	{
		public class CInfo : CInfoBase
		{
			public float height;

			public float speed;

			public float startAngle;

			public CInfo(ChaoMovement movement)
				: base(movement)
			{
			}
		}

		public const float PI_2 = (float)Math.PI * 2f;

		private float m_angle;

		[SerializeField]
		private float m_hovering_speed = (float)Math.PI;

		[SerializeField]
		private float m_hovering_height = 0.3f;

		private Vector3 m_hovering_pos = Vector3.zero;

		protected override void SetupImpl(CInfoBase info)
		{
			CInfo cInfo = info as CInfo;
			m_hovering_height = cInfo.height;
			m_hovering_speed = cInfo.speed * ((float)Math.PI / 180f);
			m_angle = cInfo.startAngle;
		}

		public override void Reset()
		{
			m_angle = 0f;
		}

		private void Start()
		{
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			m_angle += m_hovering_speed * deltaTime;
			if (m_angle > (float)Math.PI * 2f)
			{
				m_angle -= (float)Math.PI * 2f;
			}
			m_hovering_pos.y = m_hovering_height * Mathf.Sin(m_angle);
			base.Position = m_hovering_pos;
		}
	}
}
