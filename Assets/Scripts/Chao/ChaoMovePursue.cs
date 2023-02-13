using UnityEngine;

namespace Chao
{
	public class ChaoMovePursue : ChaoMoveBase
	{
		private enum PhaseXAxis
		{
			DEFAULT,
			CHARA_SPEED_UP,
			CHARA_SPEED_DOWN
		}

		private enum PhaseYAxis
		{
			DEFAULT,
			CHARA_UP,
			CHARA_DWON
		}

		private const float SPEED_DOWN_THRESHOLD_DISTANCE = 2.9f;

		private const float SPEED_DOWN_THRESHOLD_VEC = 4f;

		private const float SPEED_UP_THRESHOLD_VEC = 3.5f;

		private const float MAX_OFFESET_X = 3f;

		private const float MAX_OFFESET_Y = 5f;

		private const float PURSUE_MAX_SPEED_Y = 3f;

		private const float PURSUE_MIN_SPEED_Y = 2f;

		private const float Y_PURSUE_DISTANCE_THRESHOLD = 3f;

		private float m_deltaVecY;

		private float m_chao_v_acc = 1f;

		private float m_chao_v_dec = 1f;

		private float m_pursue_v_acc = 1f;

		private float m_speedOffsetX;

		private float m_speedOffsetY;

		private float m_speedUpThreshold = 4f;

		private Vector3 m_targetPos = new Vector3(0f, 0f, 0f);

		private Vector3 m_preTargetPos = new Vector3(0f, 0f, 0f);

		private Vector3 m_prePosition = new Vector3(0f, 0f, 0f);

		private Vector3 m_basePosition = new Vector3(0f, 0f, 0f);

		private Vector3 m_offsetChao = new Vector3(0f, 0f, 0f);

		private PhaseXAxis m_phaseXAxis;

		private PhaseYAxis m_phaseYAxis;

		public override void Enter(ChaoMovement context)
		{
			m_offsetChao = context.OffsetPosition;
			m_preTargetPos = context.TargetPosition + m_offsetChao;
			if (context.FromComeIn)
			{
				m_basePosition = m_preTargetPos;
				m_speedOffsetX = 0f;
				m_phaseXAxis = PhaseXAxis.DEFAULT;
			}
			else
			{
				m_basePosition = context.Position;
				m_speedOffsetX = m_basePosition.x - m_preTargetPos.x;
				m_phaseXAxis = PhaseXAxis.DEFAULT;
				if (m_speedOffsetX < 0f)
				{
					m_speedOffsetX = 0f;
				}
			}
			m_prePosition = m_basePosition;
			m_chao_v_acc = context.ParameterData.m_chao_v_acc_speed;
			m_chao_v_dec = context.ParameterData.m_chao_v_dec_speed;
			m_pursue_v_acc = context.Parameter.Data.m_chao_v_acc;
			m_deltaVecY = 0f;
			m_speedUpThreshold = 3.5f;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			m_targetPos = context.TargetPosition + m_offsetChao;
			if (!context.IsPlyayerMoved)
			{
				context.Position = m_basePosition + context.Hovering;
				m_preTargetPos = m_targetPos;
			}
			else
			{
				if (deltaTime <= 0f)
				{
					return;
				}
				Vector3 vector = m_targetPos - m_preTargetPos;
				if (vector.x < -100f)
				{
					m_preTargetPos.x = 0f;
					vector = context.TargetPosition - m_preTargetPos;
					m_speedOffsetX = 0f;
					m_phaseXAxis = PhaseXAxis.DEFAULT;
				}
				CalcXPosition(context, deltaTime);
				CalcYPosition(context, deltaTime);
				Vector3 preTargetPos = m_preTargetPos;
				preTargetPos.x = m_targetPos.x - m_speedOffsetX;
				preTargetPos.y = m_basePosition.y + m_speedOffsetY;
				m_basePosition = preTargetPos;
				if (m_basePosition.x < m_prePosition.x)
				{
					m_basePosition.x = m_prePosition.x;
					if (m_phaseXAxis == PhaseXAxis.CHARA_SPEED_UP)
					{
						m_phaseXAxis = PhaseXAxis.CHARA_SPEED_DOWN;
					}
				}
				context.Position = m_basePosition + context.Hovering;
				m_preTargetPos = m_targetPos;
				m_prePosition = m_basePosition;
			}
		}

		private void SetSpeedUpThreshold()
		{
			StageAbilityManager instance = StageAbilityManager.Instance;
			if (instance != null)
			{
				float num = 1f;
				if (instance.IsEasySpeed(PlayingCharacterType.MAIN))
				{
					m_speedUpThreshold += num;
				}
				if (instance.IsEasySpeed(PlayingCharacterType.SUB))
				{
					m_speedUpThreshold += num;
				}
			}
		}

		private bool IsXAxisSpeedUp(ChaoMovement context)
		{
			if (context.PlayerInfo != null)
			{
				float defaultSpeed = context.PlayerInfo.DefaultSpeed;
				Vector3 horizonVelocity = context.PlayerInfo.HorizonVelocity;
				return horizonVelocity.x > defaultSpeed + m_speedUpThreshold;
			}
			return false;
		}

		private bool IsXAxisSpeedDown(ChaoMovement context)
		{
			if (context.PlayerInfo != null)
			{
				Vector3 horizonVelocity = context.PlayerInfo.HorizonVelocity;
				return horizonVelocity.x < context.PlayerInfo.DefaultSpeed - 4f;
			}
			return false;
		}

		private void CalcXPosition(ChaoMovement context, float deltaTime)
		{
			bool flag = IsXAxisSpeedUp(context);
			if (flag)
			{
				m_phaseXAxis = PhaseXAxis.CHARA_SPEED_UP;
			}
			if (m_phaseXAxis == PhaseXAxis.CHARA_SPEED_UP)
			{
				m_speedOffsetX += m_chao_v_dec * deltaTime;
				if (m_speedOffsetX > 3f)
				{
					m_speedOffsetX = 3f;
				}
				if (!flag && m_speedOffsetX > 2.9f)
				{
					m_phaseXAxis = PhaseXAxis.CHARA_SPEED_DOWN;
				}
				else if (IsXAxisSpeedDown(context))
				{
					m_phaseXAxis = PhaseXAxis.CHARA_SPEED_DOWN;
				}
			}
			else if (m_phaseXAxis == PhaseXAxis.CHARA_SPEED_DOWN)
			{
				m_speedOffsetX -= m_chao_v_acc * deltaTime;
				if (m_speedOffsetX < 0f)
				{
					m_speedOffsetX = 0f;
					m_phaseXAxis = PhaseXAxis.DEFAULT;
				}
			}
			else
			{
				m_speedOffsetX = 0f;
			}
		}

		private bool ChangePhaseY(PhaseYAxis phase)
		{
			if (m_phaseYAxis != phase)
			{
				m_phaseYAxis = phase;
				return true;
			}
			return false;
		}

		private void CalcYPosition(ChaoMovement context, float deltaTime)
		{
			float num = m_targetPos.y - m_basePosition.y;
			if (m_phaseYAxis == PhaseYAxis.DEFAULT)
			{
				if (num > m_offsetChao.y)
				{
					if (ChangePhaseY(PhaseYAxis.CHARA_UP))
					{
						m_deltaVecY = 0f;
					}
				}
				else
				{
					if (!(num < -0.01f))
					{
						m_speedOffsetY = 0f;
						ChangePhaseY(PhaseYAxis.DEFAULT);
						return;
					}
					if (ChangePhaseY(PhaseYAxis.CHARA_DWON))
					{
						m_deltaVecY = 0f;
					}
				}
			}
			if (num < 3f)
			{
				m_deltaVecY -= m_pursue_v_acc * deltaTime;
				if (m_deltaVecY < 2f)
				{
					m_deltaVecY = 2f;
				}
			}
			else
			{
				m_deltaVecY += m_pursue_v_acc * deltaTime;
				if (m_deltaVecY > 3f)
				{
					m_deltaVecY = 3f;
				}
			}
			switch (m_phaseYAxis)
			{
			case PhaseYAxis.CHARA_UP:
			{
				float num4 = m_deltaVecY * deltaTime;
				float num5 = num - num4;
				if (num5 < 0f)
				{
					m_speedOffsetY = num;
					m_phaseYAxis = PhaseYAxis.DEFAULT;
				}
				else if (num5 > 5f)
				{
					m_speedOffsetY = num - 5f;
				}
				else
				{
					m_speedOffsetY = num4;
				}
				break;
			}
			case PhaseYAxis.CHARA_DWON:
			{
				float num2 = m_deltaVecY * deltaTime;
				float num3 = num + num2;
				if (num3 > 0f)
				{
					if (num3 > m_offsetChao.y)
					{
						m_speedOffsetY = 0f - num2;
						m_phaseYAxis = PhaseYAxis.CHARA_UP;
					}
					else
					{
						m_speedOffsetY = num;
						m_phaseYAxis = PhaseYAxis.DEFAULT;
					}
				}
				else if (num3 < -5f)
				{
					m_speedOffsetY = num + 5f;
				}
				else
				{
					m_speedOffsetY = 0f - num2;
				}
				break;
			}
			}
			if (-0.0001 < (double)m_speedOffsetY && m_speedOffsetY < 0.0001f)
			{
				m_speedOffsetY = 0f;
			}
		}
	}
}
