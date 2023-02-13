using App;
using Player;
using System;
using UnityEngine;

namespace Chao
{
	public class ChaoMovement : MonoBehaviour
	{
		private FSMSystem<ChaoMovement> m_fsm;

		private PlayerInformation m_player_info;

		private ChaoParameter m_chao_param;

		private ChaoHoveringBase m_hoveringMove;

		[SerializeField]
		private Vector3 m_offset_pos = new Vector3(-0.5f, 0.8f, 0f);

		private Vector3 m_target_pos = new Vector3(0f, 0f, 0f);

		private Vector3 m_offsetRadicon = new Vector3(-1f, 0f, 0f);

		private Vector3 m_velocity = Vector3.zero;

		private float m_come_in_speed = 5f;

		private float m_target_access_speed = 5f;

		private bool m_next_state_flag;

		private Vector3 m_moved_velocity = Vector3.zero;

		private Vector3 m_prevPlayerPos = Vector3.zero;

		private bool m_fromComeIn;

		public static readonly Vector3 HorzDir = CharacterDefs.BaseFrontTangent;

		public static readonly Vector3 VertDir = Vector3.up;

		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
			set
			{
				base.transform.position = value;
			}
		}

		public Vector3 Angles
		{
			get
			{
				return base.transform.localEulerAngles;
			}
			set
			{
				base.transform.localEulerAngles = value;
			}
		}

		public float ComeInSpeed
		{
			get
			{
				return m_come_in_speed;
			}
		}

		public float TargetAccessSpeed
		{
			get
			{
				return m_target_access_speed;
			}
		}

		public Vector3 Hovering
		{
			get
			{
				if (m_hoveringMove != null)
				{
					return m_hoveringMove.Position;
				}
				return Vector3.zero;
			}
		}

		public Vector3 OffsetPosition
		{
			get
			{
				return m_offset_pos;
			}
			protected set
			{
				m_offset_pos = value;
			}
		}

		public Vector3 TargetPosition
		{
			get
			{
				return m_target_pos;
			}
		}

		public PlayerInformation PlayerInfo
		{
			get
			{
				return m_player_info;
			}
		}

		public bool IsPlyayerMoved
		{
			get
			{
				if (m_player_info != null)
				{
					return m_player_info.IsMovementUpdated();
				}
				return false;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return m_velocity;
			}
			set
			{
				m_velocity = value;
			}
		}

		public Vector3 MovedVelocity
		{
			get
			{
				return m_moved_velocity;
			}
		}

		public Vector3 VertVelocity
		{
			get
			{
				return Vector3.Dot(m_velocity, Vector3.up) * Vector3.up;
			}
		}

		public Vector3 HorzVelocity
		{
			get
			{
				return m_velocity - VertVelocity;
			}
		}

		public bool NextState
		{
			get
			{
				return m_next_state_flag;
			}
			set
			{
				m_next_state_flag = value;
			}
		}

		public ChaoParameter Parameter
		{
			get
			{
				return m_chao_param;
			}
		}

		public ChaoParameterData ParameterData
		{
			get
			{
				return m_chao_param.Data;
			}
		}

		public Vector3 PlayerPosition
		{
			get
			{
				if (m_player_info != null)
				{
					return m_player_info.Position;
				}
				return Vector3.zero;
			}
		}

		public Vector3 PrevPlayerPosition
		{
			get
			{
				return m_prevPlayerPos;
			}
		}

		public bool FromComeIn
		{
			get
			{
				return m_fromComeIn;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (!App.Math.NearZero(deltaTime))
			{
				Vector3 position = Position;
				if (m_player_info != null)
				{
					m_target_pos = m_player_info.Position;
					m_target_pos.z = 0f;
				}
				if (m_fsm != null)
				{
					m_fsm.CurrentState.Step(this, deltaTime);
				}
				m_moved_velocity = (Position - position) / deltaTime;
				if (m_player_info != null)
				{
					m_prevPlayerPos = PlayerInfo.Position;
				}
			}
		}

		private void OnDestroy()
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.Leave(this);
				m_fsm = null;
			}
		}

		public void ChangeState(MOVESTATE_ID state)
		{
			m_next_state_flag = false;
			if (m_fsm != null && m_fsm.CurrentStateID != (StateID)state)
			{
				m_fromComeIn = (m_fsm.CurrentStateID == (StateID)3);
				m_fsm.ChangeState(this, (int)state);
			}
		}

		public static ChaoMovement Create(GameObject gameObject, ChaoSetupParameterData parameter)
		{
			ChaoMovement chaoMovement = gameObject.AddComponent<ChaoMovement>();
			FSMStateFactory<ChaoMovement>[] movementStateTable = MovementSetupChao.GetMovementStateTable();
			ChaoMovementType moveType = parameter.MoveType;
			if (moveType != 0 && moveType == ChaoMovementType.RADICON)
			{
				movementStateTable = MovementSetupRadicon.GetMovementStateTable();
			}
			chaoMovement.Setup(parameter, movementStateTable);
			return chaoMovement;
		}

		private void Setup(ChaoSetupParameterData parameter, FSMStateFactory<ChaoMovement>[] movementTable)
		{
			SetupBase(parameter);
			SetupFsm(movementTable);
			CreateHovering(parameter);
		}

		private void SetupFsm(FSMStateFactory<ChaoMovement>[] fsmtable)
		{
			m_fsm = new FSMSystem<ChaoMovement>();
			if (m_fsm != null && fsmtable != null)
			{
				foreach (FSMStateFactory<ChaoMovement> fSMStateFactory in fsmtable)
				{
					m_fsm.AddState(fSMStateFactory.stateID, fSMStateFactory.state);
				}
				m_fsm.Init(this, 3);
			}
		}

		private void SetupBase(ChaoSetupParameterData setupParameter)
		{
			m_player_info = ObjUtil.GetPlayerInformation();
			if (m_player_info != null)
			{
				m_prevPlayerPos = m_player_info.Position;
			}
			GameObject gameObject = GameObject.Find("StageChao/ChaoParameter");
			if (gameObject != null)
			{
				m_chao_param = gameObject.GetComponent<ChaoParameter>();
			}
			ChaoType chaoType = ChaoUtility.GetChaoType(base.gameObject);
			if (setupParameter != null)
			{
				switch (chaoType)
				{
				case ChaoType.MAIN:
					OffsetPosition = setupParameter.MainOffset;
					break;
				case ChaoType.SUB:
					OffsetPosition = setupParameter.SubOffset;
					break;
				}
				if (setupParameter.MoveType == ChaoMovementType.RADICON)
				{
					OffsetPosition += m_offsetRadicon;
				}
			}
		}

		private void CreateHovering(ChaoSetupParameterData setupParameter)
		{
			ChaoHoverType hoverType = setupParameter.HoverType;
			if (hoverType == ChaoHoverType.CHAO)
			{
				CreateChaoHover(setupParameter);
			}
		}

		private void CreateChaoHover(ChaoSetupParameterData setupParameter)
		{
			ChaoHovering chaoHovering = base.gameObject.AddComponent<ChaoHovering>();
			ChaoHovering.CInfo cInfo = new ChaoHovering.CInfo(this);
			cInfo.height = setupParameter.HoverHeight;
			cInfo.speed = setupParameter.HoverSpeed;
			switch (ChaoUtility.GetChaoType(base.gameObject))
			{
			case ChaoType.MAIN:
				cInfo.startAngle = setupParameter.HoverStartDegreeMain * ((float)System.Math.PI / 180f);
				break;
			case ChaoType.SUB:
				cInfo.startAngle = setupParameter.HoverStartDegreeSub * ((float)System.Math.PI / 180f);
				break;
			}
			chaoHovering.Setup(cInfo);
			SetHoveringMove(chaoHovering);
		}

		public T GetCurrentState<T>() where T : FSMState<ChaoMovement>
		{
			if (m_fsm == null)
			{
				return (T)null;
			}
			return m_fsm.CurrentState as T;
		}

		public float GetPlayerDisplacement()
		{
			return Vector3.Distance(PlayerPosition, PrevPlayerPosition);
		}

		private void SetHoveringMove(ChaoHoveringBase hovering)
		{
			m_hoveringMove = hovering;
		}

		public void ResetHovering()
		{
			if (m_hoveringMove != null)
			{
				m_hoveringMove.Reset();
			}
		}
	}
}
