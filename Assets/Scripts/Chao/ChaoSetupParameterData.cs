using System;
using UnityEngine;

namespace Chao
{
	[Serializable]
	public class ChaoSetupParameterData
	{
		private const float DefaultHoverSpeedDegree = 180f;

		private const float DefaultHoverHeight = 0.2f;

		private const float DefaultHoverStartDegreeMain = 0f;

		private const float DefaultHoverStartDegreeSub = 160f;

		[SerializeField]
		private Vector3 m_mainOffset = new Vector3(-1.1f, 1.4f, 0f);

		[SerializeField]
		private Vector3 m_subOffset = new Vector3(-1.9f, 1f, 0f);

		[SerializeField]
		private float m_colliRadius = 0.3f;

		[SerializeField]
		private Vector3 m_colliCenter = new Vector3(0f, 0.13f, -0.05f);

		[SerializeField]
		private ChaoMovementType m_movementType;

		[SerializeField]
		private ChaoHoverType m_hoverType = ChaoHoverType.CHAO;

		[SerializeField]
		private bool m_useCustomHoverParam;

		[SerializeField]
		private float m_hoverSpeedDegree = 180f;

		[SerializeField]
		private float m_hoverHeight = 0.2f;

		[SerializeField]
		private float m_hoverStartDegreeMain;

		[SerializeField]
		private float m_hoverStartDegreeSub = 160f;

		[SerializeField]
		private ShaderType m_shaderType;

		public Vector3 MainOffset
		{
			get
			{
				return m_mainOffset;
			}
		}

		public Vector3 SubOffset
		{
			get
			{
				return m_subOffset;
			}
		}

		public float ColliRadius
		{
			get
			{
				return m_colliRadius;
			}
			private set
			{
				m_colliRadius = value;
			}
		}

		public Vector3 ColliCenter
		{
			get
			{
				return m_colliCenter;
			}
			private set
			{
				m_colliCenter = value;
			}
		}

		public ChaoMovementType MoveType
		{
			get
			{
				return m_movementType;
			}
			private set
			{
				m_movementType = value;
			}
		}

		public ChaoHoverType HoverType
		{
			get
			{
				return m_hoverType;
			}
		}

		public bool UseCustomHoverParam
		{
			get
			{
				return m_useCustomHoverParam;
			}
		}

		public float HoverSpeed
		{
			get
			{
				if (m_useCustomHoverParam)
				{
					return m_hoverSpeedDegree;
				}
				return 180f;
			}
		}

		public float HoverHeight
		{
			get
			{
				if (m_useCustomHoverParam)
				{
					return m_hoverHeight;
				}
				return 0.2f;
			}
		}

		public float HoverStartDegreeMain
		{
			get
			{
				if (m_useCustomHoverParam)
				{
					return m_hoverStartDegreeMain;
				}
				return 0f;
			}
		}

		public float HoverStartDegreeSub
		{
			get
			{
				if (m_useCustomHoverParam)
				{
					return m_hoverStartDegreeSub;
				}
				return 160f;
			}
		}

		public ShaderType ShaderOffset
		{
			get
			{
				return m_shaderType;
			}
		}

		public void CopyTo(ChaoSetupParameterData dst)
		{
			dst.ColliRadius = ColliRadius;
			dst.ColliCenter = ColliCenter;
			dst.MoveType = MoveType;
			dst.m_mainOffset = m_mainOffset;
			dst.m_subOffset = m_subOffset;
			dst.m_hoverType = m_hoverType;
			dst.m_useCustomHoverParam = m_useCustomHoverParam;
			dst.m_hoverSpeedDegree = m_hoverSpeedDegree;
			dst.m_hoverHeight = m_hoverHeight;
			dst.m_hoverStartDegreeMain = m_hoverStartDegreeMain;
			dst.m_hoverStartDegreeSub = m_hoverStartDegreeSub;
			dst.m_shaderType = m_shaderType;
		}
	}
}
