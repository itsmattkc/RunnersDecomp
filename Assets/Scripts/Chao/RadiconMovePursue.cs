using UnityEngine;

namespace Chao
{
	public class RadiconMovePursue : ChaoMoveBase
	{
		private enum PhaseXAxis
		{
			DEFAULT,
			CHARA_SPEED_UP,
			CHARA_SPEED_DOWN
		}

		private const float SPEED_UP_THRESHOLD_VEC = 3f;

		private const float MAX_OFFESET_X = 3f;

		private const float MAX_OFFESET_Y = 5f;

		private const float SPEED_DOWN_THRESHOLD_DISTANCE = 2.9f;

		private const float SPEED_DOWN_THRESHOLD_VEC = 4f;

		private const float MaxDist_V = 7f;

		private const float FirstV_Speed = 0.05f;

		private const float VertPursue_Height = 2.5f;

		private float m_speedOffsetX;

		private float m_vertSpeed;

		private float m_radicon_v_acc = 1f;

		private float m_radicon_v_dec = 1f;

		private float m_radicon_v_vel = 1f;

		private float m_speedUpThreshold = 4f;

		private Vector3 m_targetPos = new Vector3(0f, 0f, 0f);

		private Vector3 m_preTargetPos = new Vector3(0f, 0f, 0f);

		private Vector3 m_prePosition = new Vector3(0f, 0f, 0f);

		private Vector3 m_basePosition = new Vector3(0f, 0f, 0f);

		private Vector3 m_offsetRadicon = new Vector3(0f, 0f, 0f);

		private PhaseXAxis m_phaseXAxis;

		public override void Enter(ChaoMovement context)
		{
			m_offsetRadicon = context.OffsetPosition;
			m_preTargetPos = context.TargetPosition + m_offsetRadicon;
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
			m_vertSpeed = (Vector3.Dot(context.MovedVelocity, ChaoMovement.VertDir) * ChaoMovement.VertDir).magnitude;
			m_radicon_v_acc = context.ParameterData.m_radicon_v_acc_speed;
			m_radicon_v_dec = context.ParameterData.m_radicon_v_dec_speed;
			m_radicon_v_vel = context.ParameterData.m_radicon_v_vel;
			m_speedUpThreshold = 3f;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			m_targetPos = context.TargetPosition + m_offsetRadicon;
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
				Vector3 vector2 = m_targetPos - m_basePosition;
				Vector3 b = Vector3.Dot(vector2, ChaoMovement.HorzDir) * ChaoMovement.HorzDir;
				Vector3 subVert = vector2 - b;
				CalcXPosition(context, deltaTime);
				CalcVertVelocity(context, deltaTime, subVert);
				Vector3 prePosition = m_prePosition;
				prePosition.x = m_targetPos.x - m_speedOffsetX;
				prePosition.y = m_prePosition.y + deltaTime * m_vertSpeed * subVert.y;
				m_basePosition = prePosition;
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

		private void CalcXPosition(ChaoMovement context, float deltaTime)
		{
			bool flag = IsXAxisSpeedUp(context);
			if (flag)
			{
				m_phaseXAxis = PhaseXAxis.CHARA_SPEED_UP;
			}
			if (m_phaseXAxis == PhaseXAxis.CHARA_SPEED_UP)
			{
				m_speedOffsetX += m_radicon_v_dec * deltaTime;
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
				m_speedOffsetX -= m_radicon_v_acc * deltaTime;
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

		private void CalcVertVelocity(ChaoMovement context, float deltaTime, Vector3 subVert)
		{
			float magnitude = subVert.magnitude;
			if (m_vertSpeed < 0.05f)
			{
				if (magnitude > 2.5f)
				{
					m_vertSpeed = 0.05f;
				}
				return;
			}
			float num = 7f;
			if (magnitude < m_vertSpeed * deltaTime)
			{
				m_vertSpeed = 0f;
				return;
			}
			if (magnitude > 7f)
			{
				m_vertSpeed = m_radicon_v_vel;
				return;
			}
			m_vertSpeed = Mathf.Min(m_vertSpeed, m_radicon_v_vel);
			float num2 = Mathf.Lerp(0f, m_radicon_v_vel, magnitude / num);
			if (m_vertSpeed < num2)
			{
				m_vertSpeed = Mathf.MoveTowards(m_vertSpeed, num2, m_radicon_v_acc * deltaTime);
			}
			else
			{
				m_vertSpeed = Mathf.MoveTowards(m_vertSpeed, num2, m_radicon_v_dec * deltaTime);
			}
		}
	}
}
