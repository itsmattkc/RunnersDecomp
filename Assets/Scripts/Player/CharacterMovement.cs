using App.Utility;
using System;
using UnityEngine;

namespace Player
{
	public class CharacterMovement : MonoBehaviour
	{
		private enum Status
		{
			OnGround,
			OnRunPath,
			IgnoreCollision,
			ThroughBroken,
			RayPos_Dirty
		}

		public enum HitType
		{
			Down,
			Up,
			Front,
			NUM
		}

		private const float PlayerGravitySize = 45f;

		private const float maxLengthOnSearchGround = 30f;

		private Vector3[] m_dir = new Vector3[3];

		private Vector3[] m_rayOffset = new Vector3[3];

		private FSMSystem<CharacterMovement> m_fsm;

		private Vector3 m_velocity;

		private Vector3 m_displacement;

		private Vector3 m_prevRayPosition;

		private float m_distanceToGround;

		private StageBlockPathManager m_blockPathManager;

		private bool m_alreadySetup;

		private readonly float m_enableLandCos = Mathf.Cos((float)Math.PI / 4f);

		public bool m_doneFixedUpdate;

		private PlayerInformation m_information;

		private bool m_dispInfo;

		private HitInfo[] m_hitInfo = new HitInfo[3];

		private HitInfo m_sweepHitInfo = default(HitInfo);

		private float m_gravitySize = 45f;

		private Vector3 m_gravityDir = -Vector3.up;

		private Vector3 m_groundUpDir = Vector3.up;

		private Bitset32 m_status;

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

		public Vector3 HorzVelocity
		{
			get
			{
				return m_velocity - Vector3.Project(m_velocity, base.transform.up);
			}
			set
			{
				m_velocity = value + VertVelocity;
			}
		}

		public Vector3 VertVelocity
		{
			get
			{
				return Vector3.Project(m_velocity, base.transform.up);
			}
			set
			{
				m_velocity = value + HorzVelocity;
			}
		}

		public float EnableLandCos
		{
			get
			{
				return m_enableLandCos;
			}
		}

		public Vector3 RaycastCheckPosition
		{
			get
			{
				return m_prevRayPosition;
			}
		}

		public Vector3 SideViewPathPos
		{
			get
			{
				return m_information.SideViewPathPos;
			}
		}

		public bool ThroughBreakable
		{
			get
			{
				return m_status.Test(3);
			}
			set
			{
				m_status.Set(3, value);
			}
		}

		public float DistanceToGround
		{
			get
			{
				return m_distanceToGround;
			}
		}

		public Vector3 GroundUpDirection
		{
			get
			{
				return m_groundUpDir;
			}
		}

		public CharacterParameterData Parameter
		{
			get
			{
				return GetComponent<CharacterParameter>().GetData();
			}
		}

		private void Start()
		{
			SetupOnStart();
		}

		public void SetupOnStart()
		{
			if (m_alreadySetup)
			{
				return;
			}
			m_prevRayPosition = base.transform.position;
			m_gravityDir = -Vector3.up;
			m_hitInfo = new HitInfo[3];
			m_sweepHitInfo = default(HitInfo);
			if (m_fsm == null)
			{
				m_fsm = new FSMSystem<CharacterMovement>();
				FSMStateFactory<CharacterMovement>[] stateTable = GetStateTable();
				FSMStateFactory<CharacterMovement>[] array = stateTable;
				foreach (FSMStateFactory<CharacterMovement> stateFactory in array)
				{
					m_fsm.AddState(stateFactory);
				}
				m_fsm.Init(this, 2);
			}
			if (m_blockPathManager == null)
			{
				m_blockPathManager = GameObjectUtil.FindGameObjectComponent<StageBlockPathManager>("StageBlockManager");
			}
			if (m_information == null)
			{
				GameObject gameObject = GameObject.Find("PlayerInformation");
				m_information = gameObject.GetComponent<PlayerInformation>();
			}
			m_alreadySetup = true;
		}

		private void LateUpdate()
		{
			m_information.SetMovementUpdated(false);
		}

		private void FixedUpdate()
		{
			m_sweepHitInfo.Reset();
			m_status.Set(4, false);
			Vector3 position = base.transform.position;
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.Step(this, Time.deltaTime);
			}
			m_displacement = base.transform.position - position;
			if (!m_status.Test(4))
			{
				m_prevRayPosition = base.transform.position + base.transform.up * 0.1f;
			}
			UpdateHitInfo();
			m_information.SetMovementUpdated(true);
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
			if (m_fsm != null && m_fsm.CurrentStateID != (StateID)state)
			{
				m_fsm.ChangeState(this, (int)state);
			}
		}

		public T GetCurrentState<T>() where T : FSMState<CharacterMovement>
		{
			if (m_fsm == null)
			{
				return (T)null;
			}
			return m_fsm.CurrentState as T;
		}

		private void UpdateHitInfo()
		{
			Vector3 position = base.transform.position;
			Vector3 up = base.transform.up;
			float num = 0.1f;
			float distance = 0.1f + num;
			CapsuleCollider component = GetComponent<CapsuleCollider>();
			float d = 0f;
			float d2 = 0f;
			Vector3 direction = Vector3.zero;
			if ((bool)component)
			{
				d = component.height;
				d2 = component.radius;
				direction = component.center;
			}
			m_dir[0] = -base.transform.up;
			m_dir[1] = up;
			m_dir[2] = base.transform.forward;
			m_rayOffset[0] = Vector3.zero;
			m_rayOffset[1] = m_dir[1] * d;
			m_rayOffset[2] = component.transform.TransformDirection(direction) + m_dir[2] * d2;
			for (int i = 0; i < 3; i++)
			{
				Vector3 vector = m_dir[i];
				Vector3 origin = position + m_rayOffset[i] - vector * num;
				RaycastHit hitInfo;
				if (Physics.Raycast(origin, vector, out hitInfo, distance))
				{
					m_hitInfo[i].Set(hitInfo);
				}
				else
				{
					m_hitInfo[i].Reset();
				}
			}
			if (!m_hitInfo[0].IsValid() && m_sweepHitInfo.IsValid())
			{
				Vector3 rhs = up;
				float num2 = Vector3.Dot(m_sweepHitInfo.info.normal, rhs);
				if (num2 > m_enableLandCos)
				{
					m_hitInfo[0].Set(m_sweepHitInfo.info);
				}
			}
			m_status.Set(0, m_hitInfo[0].valid);
			if (IsOnGround())
			{
				m_groundUpDir = m_hitInfo[0].info.normal;
				m_distanceToGround = 0f;
				return;
			}
			m_groundUpDir = -GetGravityDir();
			RaycastHit hitInfo2;
			if (Physics.Raycast(base.transform.position, GetGravityDir(), out hitInfo2, 30f))
			{
				m_distanceToGround = hitInfo2.distance;
			}
			else
			{
				m_distanceToGround = 30f;
			}
		}

		public void SetRaycastCheckPosition(Vector3 pos)
		{
			m_prevRayPosition = pos;
			m_status.Set(4, true);
		}

		public bool GetHitInfo(HitType type, out HitInfo info)
		{
			info = m_hitInfo[(int)type];
			return info.valid;
		}

		public bool IsHit(HitType type)
		{
			return m_hitInfo[(int)type].IsValid();
		}

		public void SetSweepHitInfo(HitInfo info)
		{
			m_sweepHitInfo = info;
		}

		public bool GetGroundInfo(out HitInfo info)
		{
			info = m_hitInfo[0];
			return m_hitInfo[0].valid;
		}

		public bool IsOnGround()
		{
			return m_status.Test(0);
		}

		public void OffGround()
		{
			m_status.Set(0, false);
			m_hitInfo[0].Reset();
			m_groundUpDir = -GetGravityDir();
		}

		public void ResetPosition(Vector3 pos)
		{
			base.transform.position = pos;
			m_prevRayPosition = pos;
		}

		public void ResetRotation(Quaternion rot)
		{
			base.transform.rotation = rot;
		}

		public void SetLookRotation(Vector3 front, Vector3 up)
		{
			Quaternion identity = Quaternion.identity;
			identity.SetLookRotation(front, up);
			base.transform.rotation = identity;
		}

		public float GetVertVelocityScalar()
		{
			return Vector3.Dot(m_velocity, base.transform.up);
		}

		public float GetForwardVelocityScalar()
		{
			return Vector3.Dot(m_velocity, GetForwardDir());
		}

		public Vector3 GetForwardDir()
		{
			return base.transform.forward;
		}

		public Vector3 GetUpDir()
		{
			return base.transform.up;
		}

		public Vector3 GetGravity()
		{
			return m_gravityDir * m_gravitySize;
		}

		public Vector3 GetGravityDir()
		{
			return m_gravityDir;
		}

		public Vector3 GetDisplacement()
		{
			return m_displacement;
		}

		public StageBlockPathManager GetBlockPathManager()
		{
			return m_blockPathManager;
		}

		private static FSMStateFactory<CharacterMovement>[] GetStateTable()
		{
			return new FSMStateFactory<CharacterMovement>[9]
			{
				new FSMStateFactory<CharacterMovement>(2, new CharacterMoveRun()),
				new FSMStateFactory<CharacterMovement>(3, new CharacterMoveAir()),
				new FSMStateFactory<CharacterMovement>(4, new CharacterMoveIgnoreCollision()),
				new FSMStateFactory<CharacterMovement>(5, new CharacterMoveOnPath()),
				new FSMStateFactory<CharacterMovement>(6, new CharacterMoveTarget()),
				new FSMStateFactory<CharacterMovement>(7, new CharacterMoveOnPathPhantom()),
				new FSMStateFactory<CharacterMovement>(8, new CharacterMoveTargetBoss()),
				new FSMStateFactory<CharacterMovement>(9, new CharacterMoveOnPathPhantomDrill()),
				new FSMStateFactory<CharacterMovement>(10, new CharacterMoveAsteroid())
			};
		}
	}
}
