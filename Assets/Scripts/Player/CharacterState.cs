using App;
using App.Utility;
using Message;
using System;
using UnityEngine;

namespace Player
{
	public class CharacterState : MonoBehaviour
	{
		private PlayerInformation m_information;

		private CameraManager m_camera;

		private CharaType m_charaType = CharaType.UNKNOWN;

		private bool m_subPlayer;

		public CharacterInput m_input;

		private CharacterMovement m_movement;

		private FSMSystem<CharacterState> m_fsm;

		private Animator m_bodyAnimator;

		private string m_bodyName;

		private string m_charaName;

		private string m_suffixName;

		private Bitset32 m_status;

		private Bitset32 m_visibleStatus;

		private Bitset32 m_options;

		private StateEnteringParameter m_enteringParam;

		private CharacterBlinkTimer m_blinkTimer;

		private AttackPower m_attack;

		private DefensePower m_defense;

		private uint m_attackAttribute;

		private PlayerSpeed m_nowSpeedLevel;

		private PhantomType m_nowPhantomType = PhantomType.NONE;

		private CharacterAttribute m_attribute;

		private TeamAttribute m_teamAttribute;

		[SerializeField]
		private float m_defaultSpeed = 8f;

		[SerializeField]
		private bool m_notLoadCharaParameter = true;

		private PlayingCharacterType m_playingCharacterType;

		private StageBlockPathManager m_blockPathManager;

		private CharacterContainer m_characterContainer;

		private LevelInformation m_levelInformation;

		private StageScoreManager m_scoreManager;

		private bool m_nowOnDestroy;

		private int m_numAirAction;

		private int m_numEnableJump = 1;

		public float m_hitWallTimer;

		private ItemType m_changePhantomCancel = ItemType.UNKNOWN;

		private WispBoostLevel m_wispBoostLevel = WispBoostLevel.NONE;

		private string m_wispBoostEffect = string.Empty;

		public bool m_isEdit;

		public bool m_notDeadNoRing;

		public bool m_noCrushDead;

		public bool m_notDropRing;

		private bool m_isAlreadySetupModel;

		public PhantomType NowPhantomType
		{
			get
			{
				return m_nowPhantomType;
			}
			set
			{
				m_nowPhantomType = value;
			}
		}

		public CharacterMovement Movement
		{
			get
			{
				return m_movement;
			}
		}

		public string BodyModelName
		{
			get
			{
				return m_bodyName;
			}
			set
			{
				m_bodyName = value;
			}
		}

		public string CharacterName
		{
			get
			{
				return m_charaName;
			}
			set
			{
				m_charaName = value;
			}
		}

		public string SuffixName
		{
			get
			{
				return m_suffixName;
			}
			set
			{
				m_suffixName = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		public CharacterParameterData Parameter
		{
			get
			{
				return GetComponent<CharacterParameter>().GetData();
			}
		}

		public float DefaultSpeed
		{
			get
			{
				return m_defaultSpeed;
			}
		}

		public int NumAirAction
		{
			get
			{
				return m_numAirAction;
			}
		}

		public int NumEnableJump
		{
			get
			{
				return m_numEnableJump;
			}
		}

		public CharaType charaType
		{
			get
			{
				return m_charaType;
			}
		}

		public WispBoostLevel BossBoostLevel
		{
			get
			{
				return m_wispBoostLevel;
			}
		}

		public string BossBoostEffect
		{
			get
			{
				return m_wispBoostEffect;
			}
		}

		public void SetPlayingType(PlayingCharacterType type)
		{
			m_playingCharacterType = type;
		}

		public void SetupModelsAndParameter()
		{
			if (m_isAlreadySetupModel)
			{
				return;
			}
			PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
			ResourceManager instance = ResourceManager.Instance;
			if (playerInformation != null && instance != null)
			{
				string text = (m_playingCharacterType != 0) ? playerInformation.SubCharacterName : playerInformation.MainCharacterName;
				if (!m_notLoadCharaParameter)
				{
					string name = text + "Parameter";
					GameObject gameObject = instance.GetGameObject(ResourceCategory.PLAYER_COMMON, name);
					if (gameObject != null)
					{
						CharacterParameter component = gameObject.GetComponent<CharacterParameter>();
						CharacterParameter component2 = GetComponent<CharacterParameter>();
						if (component2 != null && component != null)
						{
							component2.CopyData(component.GetData());
						}
					}
				}
				if ((bool)CharacterDataNameInfo.Instance)
				{
					CharacterDataNameInfo.Info dataByName = CharacterDataNameInfo.Instance.GetDataByName(text);
					if (dataByName != null)
					{
						m_charaType = dataByName.m_ID;
						m_attribute = dataByName.m_attribute;
						m_teamAttribute = dataByName.m_teamAttribute;
						m_options.Reset();
						SetOption(Option.BigSize, dataByName.BigSize);
						SetOption(Option.HighSpeedExEffect, dataByName.HighSpeedEffect);
						SetOption(Option.ThirdJump, dataByName.ThirdJump);
						SuffixName = dataByName.m_hud_suffix;
					}
				}
				if (base.name != null)
				{
					string text2 = "chr_" + text;
					text2 = text2.ToLower();
					GameObject gameObject2 = CreateChildModelObject(instance.GetGameObject(ResourceCategory.CHARA_MODEL, text2), true);
					if ((bool)gameObject2)
					{
						OffAnimatorRootAnimation(gameObject2);
					}
					BodyModelName = text2;
				}
				SetupAnimation();
				string[] phantomBodyName = CharacterDefs.PhantomBodyName;
				foreach (string name2 in phantomBodyName)
				{
					GameObject gameObject3 = CreateChildModelObject(instance.GetGameObject(ResourceCategory.PLAYER_COMMON, name2), false);
					if ((bool)gameObject3)
					{
						OffAnimatorRootAnimation(gameObject3);
					}
					Collider[] componentsInChildren = gameObject3.GetComponentsInChildren<Collider>(true);
					Collider[] array = componentsInChildren;
					foreach (Collider collider in array)
					{
						if (collider.gameObject.layer == LayerMask.NameToLayer("Magnet"))
						{
							collider.gameObject.AddComponent<CharacterMagnetPhantom>();
						}
						if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
						{
							collider.gameObject.AddComponent<CharacterPhantomCollision>();
						}
					}
				}
				GameObject gameObject4 = CreateChildObject(instance.GetGameObject(ResourceCategory.PLAYER_COMMON, "drill_truck"), false);
				if (gameObject4 != null)
				{
					SetupDrill(gameObject4);
				}
				CreateChildObject(instance.GetGameObject(ResourceCategory.COMMON_EFFECT, "ef_ph_laser_lp01"), false);
				if (text != null)
				{
					string effectName = "ef_pl_" + text.ToLower() + "_boost01";
					string spinDashSEName = CharaSEUtil.GetSpinDashSEName(m_charaType);
					CreateLoopEffectBehavior("CharacterBoost", effectName, spinDashSEName, ResourceCategory.CHARA_EFFECT);
					string effectName2 = "ef_pl_" + text.ToLower() + "_jump01";
					GameObject gameobj = CreateLoopEffectBehavior("CharacterSpinAttack", effectName2, null, ResourceCategory.CHARA_EFFECT);
					StateUtil.SetObjectLocalPositionToCenter(this, gameobj);
					if (IsExHighSpeedEffect())
					{
						string effectName3 = "ef_pl_" + text.ToLower() + "_infinityrun01";
						GameObject gameobj2 = CreateLoopEffectBehavior("CharacterBoostEx", effectName3, null, ResourceCategory.CHARA_EFFECT);
						StateUtil.SetObjectLocalPositionToCenter(this, gameobj2);
					}
					if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
					{
						for (int k = 0; k < 3; k++)
						{
							string str = (k + 1).ToString();
							string effectName4 = "ef_raid_speedup_lv" + str + "_atk01";
							GameObject gameObject5 = CreateLoopEffectBehavior("CharacterSpinAttackLv" + str, effectName4, null, ResourceCategory.EVENT_RESOURCE);
							if (!(gameObject5 != null))
							{
								continue;
							}
							StateUtil.SetObjectLocalPositionToCenter(this, gameObject5);
							if (k != 2)
							{
								continue;
							}
							CharacterMagnet partsComponentAlways = StateUtil.GetPartsComponentAlways<CharacterMagnet>(this, "CharacterMagnet");
							if (!(partsComponentAlways != null))
							{
								continue;
							}
							GameObject gameObject6 = UnityEngine.Object.Instantiate(partsComponentAlways.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
							if (gameObject6 != null)
							{
								gameObject6.name = "CharacterMagnetBossBoost";
								gameObject6.transform.parent = gameObject5.transform;
								gameObject6.transform.localPosition = partsComponentAlways.gameObject.transform.localPosition;
								gameObject6.transform.localRotation = partsComponentAlways.gameObject.transform.localRotation;
								gameObject6.transform.localScale = partsComponentAlways.gameObject.transform.localScale;
								SphereCollider component3 = gameObject6.GetComponent<SphereCollider>();
								if (component3 != null)
								{
									component3.radius = 1.5f;
								}
								gameObject6.SetActive(true);
							}
						}
					}
					CharacterName = text.ToLower();
				}
				if (IsBigSize())
				{
					CharacterMagnet partsComponentAlways2 = StateUtil.GetPartsComponentAlways<CharacterMagnet>(this, "CharacterMagnet");
					if (partsComponentAlways2 != null)
					{
						partsComponentAlways2.IsBigSize = true;
					}
					CharacterMagnet partsComponentAlways3 = StateUtil.GetPartsComponentAlways<CharacterMagnet>(this, "CharacterMagnetBossBoost");
					if (partsComponentAlways3 != null)
					{
						partsComponentAlways3.IsBigSize = true;
					}
					CharacterInvincible partsComponentAlways4 = StateUtil.GetPartsComponentAlways<CharacterInvincible>(this, "CharacterInvincible");
					if (partsComponentAlways4 != null)
					{
						partsComponentAlways4.IsBigSize = true;
					}
					CharacterBarrier partsComponentAlways5 = StateUtil.GetPartsComponentAlways<CharacterBarrier>(this, "CharacterBarrier");
					if (partsComponentAlways5 != null)
					{
						partsComponentAlways5.IsBigSize = true;
					}
				}
			}
			m_isAlreadySetupModel = true;
		}

		private GameObject CreateChildModelObject(GameObject srcObject, bool active)
		{
			if (srcObject != null)
			{
				Vector3 localPosition = srcObject.transform.localPosition;
				Quaternion localRotation = srcObject.transform.localRotation;
				GameObject gameObject = UnityEngine.Object.Instantiate(srcObject, base.transform.position, base.transform.rotation) as GameObject;
				if (gameObject != null)
				{
					gameObject.transform.parent = base.transform;
					gameObject.SetActive(active);
					gameObject.transform.localPosition = localPosition;
					gameObject.transform.localRotation = localRotation;
					gameObject.name = srcObject.name;
					return gameObject;
				}
			}
			return null;
		}

		private static void OffAnimatorRootAnimation(GameObject modelObject)
		{
			if (!(modelObject == null))
			{
				Animator component = modelObject.GetComponent<Animator>();
				if (component != null)
				{
					component.applyRootMotion = false;
				}
			}
		}

		private GameObject CreateChildObject(GameObject srcObject, bool active)
		{
			if (srcObject != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(srcObject, base.transform.position, base.transform.rotation) as GameObject;
				if ((bool)gameObject)
				{
					gameObject.SetActive(active);
					gameObject.transform.parent = base.transform;
					gameObject.name = srcObject.name;
					return gameObject;
				}
			}
			return null;
		}

		private void SetupDrill(GameObject drillObject)
		{
			DrillTrack drillTrack = drillObject.AddComponent<DrillTrack>();
			if (drillTrack != null)
			{
				drillTrack.m_Target = base.gameObject;
				GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
				if (gameObject != null)
				{
					drillTrack.m_Camera = gameObject.transform;
				}
			}
		}

		private void SetupAnimation()
		{
			GameObject gameObject = base.transform.FindChild(m_bodyName).gameObject;
			if ((bool)gameObject)
			{
				m_bodyAnimator = gameObject.GetComponent<Animator>();
			}
		}

		private GameObject CreateLoopEffectBehavior(string objectName, string effectName, string sename, ResourceCategory category)
		{
			GameObject gameObject = new GameObject(objectName);
			if (gameObject != null)
			{
				CharacterLoopEffect characterLoopEffect = gameObject.AddComponent<CharacterLoopEffect>();
				if (characterLoopEffect != null)
				{
					characterLoopEffect.Setup(effectName, category);
					characterLoopEffect.SetSE(sename);
					gameObject.transform.position = base.transform.position;
					gameObject.transform.rotation = base.transform.rotation;
					gameObject.transform.parent = base.transform;
					gameObject.SetActive(false);
					return gameObject;
				}
				UnityEngine.Object.Destroy(gameObject);
			}
			return null;
		}

		public void Start()
		{
			SetupOnStart();
		}

		private void SetupOnStart()
		{
			if (m_information == null)
			{
				GameObject gameObject = GameObject.Find("PlayerInformation");
				m_information = gameObject.GetComponent<PlayerInformation>();
			}
			if (m_blockPathManager == null)
			{
				m_blockPathManager = GameObjectUtil.FindGameObjectComponent<StageBlockPathManager>("StageBlockManager");
			}
			if (m_characterContainer == null)
			{
				m_characterContainer = GameObjectUtil.FindGameObjectComponent<CharacterContainer>("CharacterContainer");
			}
			if (m_camera == null)
			{
				GameObject gameObject2 = GameObject.FindGameObjectWithTag("MainCamera");
				m_camera = gameObject2.GetComponent<CameraManager>();
			}
			if (m_levelInformation == null)
			{
				m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
			}
			if (m_scoreManager == null)
			{
				m_scoreManager = StageScoreManager.Instance;
			}
			if (m_information != null)
			{
				m_nowSpeedLevel = m_information.SpeedLevel;
				m_information.SetPlayerAttribute(m_attribute, m_teamAttribute, m_playingCharacterType);
			}
			if (!m_isAlreadySetupModel)
			{
				SetupModelsAndParameter();
			}
			if (m_input == null)
			{
				m_input = GetComponent<CharacterInput>();
			}
			if (m_movement == null)
			{
				m_movement = GetComponent<CharacterMovement>();
			}
			SetSpeedLevel(m_nowSpeedLevel);
			StateUtil.NowLanding(this, false);
			MakeFSM();
			SetupChildObjectOnStart();
			Movement.SetupOnStart();
			m_input.CreateHistory();
		}

		private void SetupChildObjectOnStart()
		{
			foreach (Transform item in base.gameObject.transform)
			{
				item.gameObject.SetActive(false);
			}
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ShadowProjector");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, BodyModelName);
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
			m_hitWallTimer = 0f;
		}

		private void OnDestroy()
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_nowOnDestroy = true;
				m_fsm.CurrentState.Leave(this);
				m_fsm = null;
			}
		}

		private void Update()
		{
			if (!App.Math.NearZero(Time.deltaTime))
			{
				if (m_fsm != null && m_fsm.CurrentState != null)
				{
					m_fsm.CurrentState.Step(this, Time.deltaTime);
				}
				UpdateInfomations();
				CheckHitWallTimerDirty();
				CheckFallingDead();
				UpdateTransformInformations();
			}
		}

		private void MakeFSM()
		{
			if (m_fsm == null)
			{
				m_fsm = new FSMSystem<CharacterState>();
				FSMStateFactory<CharacterState>[] commonFSMTable = GetCommonFSMTable();
				FSMStateFactory<CharacterState>[] array = commonFSMTable;
				foreach (FSMStateFactory<CharacterState> stateFactory in array)
				{
					m_fsm.AddState(stateFactory);
				}
				SetAttributeState();
				if (!m_isEdit)
				{
					m_fsm.Init(this, 2);
				}
				else
				{
					m_fsm.Init(this, 1);
				}
			}
		}

		private void SetAttributeState()
		{
			switch (m_attribute)
			{
			case CharacterAttribute.SPEED:
				m_fsm.AddState(4, new StateDoubleJump());
				m_numEnableJump = 2;
				break;
			case CharacterAttribute.FLY:
				m_fsm.AddState(4, new StateFly());
				m_numEnableJump = 1;
				break;
			case CharacterAttribute.POWER:
				m_fsm.AddState(4, new StateGride());
				m_numEnableJump = 1;
				break;
			}
		}

		public void ChangeState(STATE_ID state)
		{
			m_fsm.ChangeState(this, (int)state);
			m_enteringParam = null;
			SetStatus(Status.NowLanding, false);
		}

		public void ChangeMovement(MOVESTATE_ID state)
		{
			if ((bool)m_movement)
			{
				m_movement.ChangeState(state);
			}
		}

		public T CreateEnteringParameter<T>() where T : StateEnteringParameter, new()
		{
			m_enteringParam = new T();
			return (T)m_enteringParam;
		}

		public T GetEnteringParameter<T>() where T : StateEnteringParameter
		{
			if (m_enteringParam == null)
			{
				return (T)null;
			}
			if (m_enteringParam as T != null)
			{
				return (T)m_enteringParam;
			}
			return (T)null;
		}

		public void SetVisibleBlink(bool value)
		{
			m_visibleStatus.Set(0, value);
			SetRenderEnable(!m_visibleStatus.Any());
		}

		public void SetModelNotDraw(bool value)
		{
			m_visibleStatus.Set(1, value);
			SetRenderEnable(!m_visibleStatus.Any());
			if (!(m_bodyName == "chr_omega"))
			{
				return;
			}
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "chr_omega");
			if (gameObject != null)
			{
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Booster_R");
				if (gameObject2 != null)
				{
					gameObject2.SetActive(!value);
				}
				GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "Booster_L");
				if (gameObject3 != null)
				{
					gameObject3.SetActive(!value);
				}
			}
		}

		private void SetRenderEnable(bool value)
		{
			Component[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>(true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
				skinnedMeshRenderer.enabled = value;
			}
		}

		public void StartDamageBlink()
		{
			if (m_blinkTimer == null)
			{
				m_blinkTimer = base.gameObject.AddComponent<CharacterBlinkTimer>();
			}
			m_blinkTimer.Setup(this, Parameter.m_damageInvincibleTime);
		}

		private void UpdateTransformInformations()
		{
			if (!(m_information == null))
			{
				m_information.SetTransform(base.transform);
				m_information.SetVelocity(m_movement.Velocity);
				m_information.SetFrontSpeed(m_movement.GetForwardVelocityScalar());
				m_information.SetHorzAndVertVelocity(m_movement.HorzVelocity, m_movement.VertVelocity);
			}
		}

		private void UpdateInfomations()
		{
			if (m_information == null)
			{
				return;
			}
			UpdateTransformInformations();
			Vector3 displacement = m_movement.GetDisplacement();
			if (m_information.IsMovementUpdated())
			{
				float num = 0f;
				num = ((!m_movement.IsOnGround()) ? Mathf.Max(0f, Vector3.Dot(displacement, CharacterDefs.BaseFrontTangent)) : Mathf.Max(0f, Vector3.Dot(displacement, m_movement.GetForwardDir())));
				m_information.AddTotalDistance(num);
			}
			m_information.SetDistanceToGround(m_movement.DistanceToGround);
			m_information.SetGravityDirection(m_movement.GetGravityDir());
			m_information.SetUpDirection(m_movement.GroundUpDirection);
			m_information.SetDefautlSpeed(DefaultSpeed);
			m_information.SetDead(IsDead());
			m_information.SetDamaged(IsDamaged());
			m_information.SetOnGround(m_movement.IsOnGround());
			m_information.SetEnableCharaChange(IsEnableCharaChange(false));
			m_information.SetParaloop(IsParaloop());
			m_information.SetPhantomType(m_nowPhantomType);
			m_information.SetLastChance(IsNowLastChance());
			StageBlockPathManager blockPathManager = m_blockPathManager;
			if (blockPathManager != null)
			{
				Vector3 position = Position;
				position.y = 0f;
				Vector3? wpos = position;
				Vector3? normal = base.transform.up;
				Vector3? tangent = null;
				PathEvaluator curentPathEvaluator = blockPathManager.GetCurentPathEvaluator(BlockPathController.PathType.SV);
				if (curentPathEvaluator != null)
				{
					float dist = curentPathEvaluator.Distance;
					curentPathEvaluator.GetClosestPositionAlongSpline(wpos.Value, dist - 10f, dist + 10f, out dist);
					curentPathEvaluator.GetPNT(dist, ref wpos, ref normal, ref tangent);
					m_information.SetSideViewPath(wpos.Value, normal.Value);
				}
			}
		}

		private void CheckHitWallTimerDirty()
		{
			if (!TestStatus(Status.HitWallTimerDirty))
			{
				m_hitWallTimer = 0f;
			}
			else
			{
				SetStatus(Status.HitWallTimerDirty, false);
			}
		}

		private void CheckFallingDead()
		{
			if (!IsDead() && !IsNowLastChance() && !IsHold())
			{
				Vector3 position = base.transform.position;
				if (position.y < -100f)
				{
					ChangeState(STATE_ID.FallingDead);
				}
			}
		}

		public void SetStatus(Status st, bool value)
		{
			m_status.Set((int)st, value);
		}

		public bool TestStatus(Status st)
		{
			return m_status.Test((int)st);
		}

		public void SetOption(Option op, bool value)
		{
			m_options.Set((int)op, value);
		}

		public bool TestOption(Option op)
		{
			return m_options.Test((int)op);
		}

		public bool IsDead()
		{
			return TestStatus(Status.Dead);
		}

		public bool IsDamaged()
		{
			return TestStatus(Status.Damaged);
		}

		public bool IsParaloop()
		{
			return TestStatus(Status.Paraloop);
		}

		public bool IsHold()
		{
			return TestStatus(Status.Hold);
		}

		public bool IsBigSize()
		{
			return TestOption(Option.BigSize);
		}

		public bool IsExHighSpeedEffect()
		{
			return TestOption(Option.HighSpeedExEffect);
		}

		public bool IsThirdJump()
		{
			return TestOption(Option.ThirdJump);
		}

		public bool IsNowLastChance()
		{
			return TestStatus(Status.LastChance);
		}

		public bool IsOnDestroy()
		{
			return m_nowOnDestroy;
		}

		public bool IsNowPhantom()
		{
			return m_nowPhantomType != PhantomType.NONE;
		}

		public CharacterAttribute GetCharacterAttribute()
		{
			return m_attribute;
		}

		public void SetSpeedLevel(PlayerSpeed speed)
		{
			switch (speed)
			{
			case PlayerSpeed.LEVEL_1:
				m_defaultSpeed = Parameter.m_level1Speed;
				break;
			case PlayerSpeed.LEVEL_2:
				m_defaultSpeed = Parameter.m_level2Speed;
				break;
			case PlayerSpeed.LEVEL_3:
				m_defaultSpeed = Parameter.m_level3Speed;
				break;
			}
		}

		public T GetMovementState<T>() where T : FSMState<CharacterMovement>
		{
			return Movement.GetCurrentState<T>();
		}

		public Animator GetAnimator()
		{
			return m_bodyAnimator;
		}

		public void OnAttack(AttackPower attack, DefensePower defense)
		{
			m_attack = attack;
			m_defense = defense;
		}

		public void OffAttack()
		{
			m_attack = AttackPower.PlayerNormal;
			m_defense = DefensePower.PlayerNormal;
			m_attackAttribute = 0u;
		}

		public void OnAttackAttribute(AttackAttribute attribute)
		{
			m_attackAttribute |= (uint)attribute;
		}

		public void SetLastChance(bool value)
		{
			SetStatus(Status.LastChance, value);
		}

		public void SetNotCharaChange(bool value)
		{
			SetStatus(Status.NotCharaChange, value);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeCharaButton", new MsgChangeCharaButton(!value, false), SendMessageOptions.DontRequireReceiver);
		}

		public void SetNotUseItem(bool value)
		{
			MsgItemButtonEnable value2 = new MsgItemButtonEnable(!value);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnItemEnable", value2, SendMessageOptions.DontRequireReceiver);
		}

		public bool IsEnableCharaChange(bool changeByMiss)
		{
			if (TestStatus(Status.NotCharaChange))
			{
				return false;
			}
			if (IsDead())
			{
				if (changeByMiss)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public StageBlockPathManager GetStagePathManager()
		{
			return m_blockPathManager;
		}

		public CharacterContainer GetCharacterContainer()
		{
			return m_characterContainer;
		}

		public CameraManager GetCamera()
		{
			return m_camera;
		}

		public LevelInformation GetLevelInformation()
		{
			return m_levelInformation;
		}

		public PlayerInformation GetPlayerInformation()
		{
			return m_information;
		}

		public StageScoreManager GetStageScoreManager()
		{
			return m_scoreManager;
		}

		public void ActiveCharacter(bool jump, bool damageBlink, Vector3 position, Quaternion rotation)
		{
			SetupOnStart();
			if (damageBlink)
			{
				StartDamageBlink();
			}
			WarpAndCheckOverlap(position, rotation);
			if (jump)
			{
				ClearAirAction();
				GetAnimator().CrossFade("SpringJump", 0.01f);
				JumpSpringParameter jumpSpringParameter = CreateEnteringParameter<JumpSpringParameter>();
				jumpSpringParameter.Set(base.transform.position, base.transform.rotation, 7f, 0.3f);
				ChangeState(STATE_ID.SpringJump);
			}
			else
			{
				m_numAirAction = 99;
				GetAnimator().Play("Fall");
				Movement.Velocity = new Vector3(0f, 2f, 0f);
				ChangeState(STATE_ID.Fall);
			}
			if (m_information.NumRings == 0)
			{
				StateUtil.SetEmergency(this, true);
			}
		}

		private void WarpAndCheckOverlap(Vector3 pos, Quaternion rotation)
		{
			CapsuleCollider component = GetComponent<CapsuleCollider>();
			Movement.ResetPosition(pos);
			Movement.ResetRotation(rotation);
			float radius = Mathf.Max(component.height, component.radius) + 0.2f;
			Vector3 a = pos;
			Vector3 center = component.center;
			Vector3 position = a + center.y * base.transform.up;
			int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Terrain"));
			Collider[] array = Physics.OverlapSphere(position, radius, layerMask);
			if (array.Length > 0)
			{
				Vector3 a2 = -Movement.GetGravityDir();
				float num = 5f;
				Vector3 a3 = pos + a2 * num;
				radius = component.radius;
				float d = component.height * 0.5f - radius;
				Vector3 a4 = a3 + base.transform.TransformDirection(component.center);
				Vector3 point = a4 - a2 * d;
				Vector3 point2 = a4 + a2 * d;
				Vector3 vector = -a2;
				RaycastHit hitInfo;
				if (Physics.CapsuleCast(point, point2, radius, vector, out hitInfo, num, layerMask))
				{
					Vector3 b = vector * (hitInfo.distance - 0.02f);
					pos = a3 + b;
					Movement.ResetPosition(pos);
					Debug.Log("WarpAndCheckOverlap CapsuleCast Hit:" + pos.x + " " + pos.y + " " + pos.z);
				}
			}
		}

		public void DeactiveCharacter()
		{
			StateUtil.DeactiveCombo(this, true);
			StateUtil.DeactiveInvincible(this);
			StateUtil.DeactiveBarrier(this);
			StateUtil.DeactiveMagetObject(this);
			StateUtil.DeactiveTrampoline(this);
			if (TestStatus(Status.Emergency))
			{
				StateUtil.SetEmergency(this, false);
			}
			m_status.Reset();
		}

		public void AddAirAction()
		{
			m_numAirAction++;
		}

		public void ClearAirAction()
		{
			m_numAirAction = 0;
		}

		public void SetAirAction(int num)
		{
			m_numAirAction = num;
		}

		public CharacterLoopEffect GetSpinAttackEffect()
		{
			string text = "CharacterSpinAttack";
			if (m_wispBoostLevel != WispBoostLevel.NONE)
			{
				text = text + "Lv" + (int)(m_wispBoostLevel + 1);
			}
			return GameObjectUtil.FindChildGameObjectComponent<CharacterLoopEffect>(base.gameObject, text);
		}

		public void SetBoostLevel(WispBoostLevel wispBoostLevel, string effect)
		{
			m_wispBoostLevel = wispBoostLevel;
			m_wispBoostEffect = effect;
		}

		public void SetChangePhantomCancel(ItemType itemType)
		{
			m_changePhantomCancel = itemType;
		}

		public ItemType GetChangePhantomCancel()
		{
			return m_changePhantomCancel;
		}

		public void OnAddRings(int numRing)
		{
			m_information.AddNumRings(numRing);
			ObjUtil.AddCombo();
			StateUtil.SetEmergency(this, false);
		}

		private void OnMsgTutorialGetRingNum(MsgTutorialGetRingNum msg)
		{
			msg.m_ring = m_information.NumRings;
		}

		private void OnMsgTutorialResetForRetry(MsgTutorialResetForRetry msg)
		{
			m_information.SetNumRings(msg.m_ring);
			if (msg.m_blink)
			{
				StateUtil.SetEmergency(this, msg.m_ring == 0);
			}
		}

		private void OnResetRingsForCheckPoint(MsgPlayerTransferRing msg)
		{
			if (msg.m_hud)
			{
				StateUtil.SetEmergency(this, true);
			}
		}

		private void OnResetRingsForContinue()
		{
			StateUtil.SetEmergency(this, false);
		}

		private void OnDebugAddDistance(int distance)
		{
		}

		private void OnDebugWarpPlayer(Vector3 pos)
		{
		}

		private bool IsStompHitObject(Collider other)
		{
			float vertVelocityScalar = Movement.GetVertVelocityScalar();
			if (vertVelocityScalar > -1f)
			{
				return false;
			}
			Vector3 b = other.transform.position;
			CapsuleCollider capsuleCollider = other as CapsuleCollider;
			if (capsuleCollider != null)
			{
				b = capsuleCollider.transform.TransformPoint(capsuleCollider.center);
			}
			else
			{
				SphereCollider sphereCollider = other as SphereCollider;
				if (sphereCollider != null)
				{
					b = sphereCollider.transform.TransformPoint(sphereCollider.center);
				}
				else
				{
					BoxCollider boxCollider = other as BoxCollider;
					if (boxCollider != null)
					{
						b = boxCollider.transform.TransformPoint(boxCollider.center);
					}
				}
			}
			Vector3 vector = base.transform.TransformPoint(StateUtil.GetBodyCenterPosition(this)) - b;
			if (vector.sqrMagnitude > float.Epsilon && Vector3.Dot(vector.normalized, base.transform.up) > Mathf.Cos((float)System.Math.PI / 180f * Parameter.m_enableStompDec))
			{
				return true;
			}
			return false;
		}

		public void OnTriggerEnter(Collider other)
		{
			AttackPower attackPower = m_attack;
			uint num = m_attackAttribute;
			if (StateUtil.IsInvincibleActive(this))
			{
				attackPower = AttackPower.PlayerInvincible;
				num |= 0x10;
			}
			else if (attackPower == AttackPower.PlayerStomp && IsStompHitObject(other))
			{
				attackPower = AttackPower.PlayerSpin;
				if (m_attribute == CharacterAttribute.POWER)
				{
					num |= 8;
				}
			}
			MsgHitDamage msgHitDamage = new MsgHitDamage(base.gameObject, attackPower);
			msgHitDamage.m_attackAttribute = num;
			GameObjectUtil.SendDelayedMessageToGameObject(other.gameObject, "OnDamageHit", msgHitDamage);
		}

		public void OnDefrayRing()
		{
			if (m_information.NumRings == 0)
			{
				StateUtil.SetEmergency(this, true);
			}
		}

		public void OnDamageHit(MsgHitDamage msg)
		{
			if (IsDamaged() || IsDead() || IsNowLastChance() || IsHold())
			{
				return;
			}
			m_notDropRing = false;
			if (StateUtil.IsInvincibleActive(this))
			{
				return;
			}
			int num = (int)m_defense;
			Collider component = msg.m_sender.GetComponent<Collider>();
			if (component != null && IsStompHitObject(component))
			{
				num = 2;
			}
			if (msg.m_attackPower <= num)
			{
				return;
			}
			CharacterBarrier barrier = StateUtil.GetBarrier(this);
			if (barrier != null)
			{
				barrier.Damaged();
				StartDamageBlink();
				return;
			}
			Vector3 bodyCenterPosition = StateUtil.GetBodyCenterPosition(this);
			Vector3 position = base.transform.TransformPoint(bodyCenterPosition);
			StateUtil.CreateEffect(this, position, base.transform.rotation, "ef_pl_damage_com01", true);
			if (m_information.NumRings == 0)
			{
				ChangeState(STATE_ID.Dead);
				return;
			}
			if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.GUARD_DROP_RING))
			{
				float chaoAbilityValue = StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.GUARD_DROP_RING);
				float num2 = UnityEngine.Random.Range(0f, 99.9f);
				if (chaoAbilityValue >= num2)
				{
					m_notDropRing = true;
					ObjUtil.RequestStartAbilityToChao(ChaoAbility.GUARD_DROP_RING, false);
				}
			}
			if (!m_notDropRing)
			{
				ObjUtil.CreateLostRing(base.transform.position, base.transform.rotation, m_information.NumRings);
				m_information.LostRings();
				StateUtil.SetEmergency(this, true);
			}
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.DAMAGE_TRAMPOLINE, false);
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.DAMAGE_DESTROY_ALL, false);
			m_levelInformation.Missed = true;
			if (!IsNowPhantom())
			{
				ChangeState(STATE_ID.Damage);
			}
		}

		private void OnDamageSucceed(MsgHitDamageSucceed msg)
		{
			if (m_fsm != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		public void OnAttackGuard(MsgAttackGuard msg)
		{
			if (m_fsm != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		public void OnFallingDead()
		{
			if (NowPhantomType != PhantomType.DRILL && !IsDead() && !IsNowLastChance() && !IsHold())
			{
				ChangeState(STATE_ID.FallingDead);
			}
		}

		private void OnSpringImpulse(MsgOnSpringImpulse msg)
		{
			if (!IsDead() && !IsNowPhantom() && !IsNowLastChance() && !IsHold())
			{
				JumpSpringParameter jumpSpringParameter = CreateEnteringParameter<JumpSpringParameter>();
				jumpSpringParameter.Set(msg.m_position, msg.m_rotation, msg.m_firstSpeed, msg.m_outOfControl);
				ChangeState(STATE_ID.SpringJump);
				msg.m_succeed = true;
			}
		}

		private void OnDashRingImpulse(MsgOnDashRingImpulse msg)
		{
			if (!IsDead() && !IsNowPhantom() && !IsNowLastChance() && !IsHold())
			{
				JumpSpringParameter jumpSpringParameter = CreateEnteringParameter<JumpSpringParameter>();
				jumpSpringParameter.Set(msg.m_position, msg.m_rotation, msg.m_firstSpeed, msg.m_outOfControl);
				ChangeState(STATE_ID.DashRing);
				msg.m_succeed = true;
			}
		}

		private void OnCannonImpulse(MsgOnCannonImpulse msg)
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		private void OnAbidePlayer(MsgOnAbidePlayer msg)
		{
			if (!IsDead() && !IsNowPhantom() && !IsNowLastChance())
			{
				CannonReachParameter cannonReachParameter = CreateEnteringParameter<CannonReachParameter>();
				cannonReachParameter.Set(msg.m_position, msg.m_rotation, msg.m_height, msg.m_abideObject);
				ChangeState(STATE_ID.ReachCannon);
				msg.m_succeed = true;
			}
		}

		private void OnJumpBoardHit(MsgOnJumpBoardHit msg)
		{
			if (!IsDead() && !IsNowPhantom() && !IsNowLastChance() && !IsHold())
			{
				CharacterCheckTrickJump characterCheckTrickJump = GetComponent<CharacterCheckTrickJump>();
				if (characterCheckTrickJump == null)
				{
					characterCheckTrickJump = base.gameObject.AddComponent<CharacterCheckTrickJump>();
				}
				if (characterCheckTrickJump != null)
				{
					characterCheckTrickJump.Reset();
				}
			}
		}

		private void OnJumpBoardJump(MsgOnJumpBoardJump msg)
		{
			if (!IsDead() && !IsNowPhantom() && !IsNowLastChance() && !IsHold())
			{
				bool flag = false;
				CharacterCheckTrickJump component = GetComponent<CharacterCheckTrickJump>();
				if (component != null)
				{
					flag = component.IsTouched;
					UnityEngine.Object.Destroy(component);
				}
				TrickJumpParameter trickJumpParameter = CreateEnteringParameter<TrickJumpParameter>();
				if (flag)
				{
					trickJumpParameter.Set(msg.m_position, msg.m_succeedRotation, msg.m_succeedFirstSpeed, msg.m_succeedOutOfcontrol, msg.m_succeedRotation, msg.m_succeedFirstSpeed, msg.m_succeedOutOfcontrol, flag);
				}
				else
				{
					trickJumpParameter.Set(msg.m_position, msg.m_missRotation, msg.m_missFirstSpeed, msg.m_missOutOfcontrol, msg.m_succeedRotation, msg.m_succeedFirstSpeed, msg.m_succeedOutOfcontrol, flag);
				}
				ChangeState(STATE_ID.TrickJump);
				msg.m_succeed = true;
			}
		}

		private void OnUpSpeedLevel(MsgUpSpeedLevel msg)
		{
			m_nowSpeedLevel = msg.m_level;
			SetSpeedLevel(m_nowSpeedLevel);
		}

		private void OnRunLoopPath(MsgRunLoopPath msg)
		{
			if (m_fsm != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		private void OnUseItem(MsgUseItem msg)
		{
			if (IsDead() || IsNowLastChance())
			{
				return;
			}
			switch (msg.m_itemType)
			{
			case ItemType.INVINCIBLE:
				if (!IsNowPhantom())
				{
					StateUtil.ActiveInvincible(this, msg.m_time);
				}
				break;
			case ItemType.BARRIER:
				StateUtil.ActiveBarrier(this);
				if (IsNowPhantom())
				{
					StateUtil.SetNotDrawBarrierEffect(this, true);
				}
				break;
			case ItemType.MAGNET:
				StateUtil.ActiveMagnetObject(this, msg.m_time);
				break;
			case ItemType.COMBO:
				StateUtil.ActiveCombo(this, msg.m_time);
				break;
			case ItemType.LASER:
				if (NowPhantomType == PhantomType.NONE)
				{
					StateUtil.ChangeStateToChangePhantom(this, PhantomType.LASER, msg.m_time);
				}
				break;
			case ItemType.DRILL:
				if (NowPhantomType == PhantomType.NONE)
				{
					StateUtil.ChangeStateToChangePhantom(this, PhantomType.DRILL, msg.m_time);
				}
				break;
			case ItemType.ASTEROID:
				if (NowPhantomType == PhantomType.NONE)
				{
					StateUtil.ChangeStateToChangePhantom(this, PhantomType.ASTEROID, msg.m_time);
				}
				break;
			case ItemType.TRAMPOLINE:
				StateUtil.ActiveTrampoline(this, msg.m_time);
				break;
			}
		}

		private void OnInvalidateItem(MsgInvalidateItem msg)
		{
			if (IsDead())
			{
				return;
			}
			if (IsNowLastChance())
			{
				if (msg.m_itemType == ItemType.COMBO)
				{
					StateUtil.DeactiveCombo(this, true);
				}
				return;
			}
			switch (msg.m_itemType)
			{
			case ItemType.BARRIER:
				break;
			case ItemType.INVINCIBLE:
				StateUtil.DeactiveInvincible(this);
				break;
			case ItemType.MAGNET:
				StateUtil.DeactiveMagetObject(this);
				break;
			case ItemType.COMBO:
				StateUtil.DeactiveCombo(this, true);
				break;
			case ItemType.LASER:
				if (m_fsm != null && m_fsm.CurrentState != null)
				{
					StateUtil.SetChangePhantomCancel(this, msg.m_itemType);
					m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
				}
				break;
			case ItemType.DRILL:
				if (m_fsm != null && m_fsm.CurrentState != null)
				{
					StateUtil.SetChangePhantomCancel(this, msg.m_itemType);
					m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
				}
				break;
			case ItemType.ASTEROID:
				if (m_fsm != null && m_fsm.CurrentState != null)
				{
					StateUtil.SetChangePhantomCancel(this, msg.m_itemType);
					m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
				}
				break;
			case ItemType.TRAMPOLINE:
				StateUtil.DeactiveTrampoline(this);
				break;
			}
		}

		private void WarpPosition(Vector3 pos, Quaternion rotation, bool hold)
		{
			Vector3 gravityDir = Movement.GetGravityDir();
			float distance = 0.2f;
			RaycastHit hitInfo;
			if (Physics.Raycast(pos, gravityDir, out hitInfo, distance))
			{
				pos = hitInfo.point + hitInfo.normal * 0.01f;
			}
			Movement.ResetPosition(pos);
			Movement.ResetRotation(rotation);
			if ((bool)m_information)
			{
				m_information.SetTransform(base.transform);
			}
			if (hold && m_fsm != null && m_fsm.CurrentStateID != (StateID)1)
			{
				ChangeState(STATE_ID.Hold);
			}
		}

		private void OnMsgStageReplace(MsgStageReplace msg)
		{
			Vector3 position = msg.m_position;
			Vector3 vector = new Vector3(0.5f, 0.2f, 0f);
			position += vector;
			WarpPosition(position, msg.m_rotation, true);
		}

		private void OnMsgWarpPlayer(MsgWarpPlayer msg)
		{
			WarpPosition(msg.m_position, msg.m_rotation, msg.m_hold);
		}

		private void OnMsgStageRestart(MsgStageRestart msg)
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		private void OnMsgPLHold(MsgPLHold msg)
		{
			ChangeState(STATE_ID.Hold);
		}

		private void OnMsgPLReleaseHold(MsgPLReleaseHold msg)
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		private void OnPauseItemOnBoss(MsgPauseItemOnBoss msg)
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.DispatchMessage(this, msg.ID, msg);
			}
		}

		private void OnMsgExitStage(MsgExitStage msg)
		{
			base.enabled = false;
			Movement.enabled = false;
			StateUtil.DeactiveCombo(this, true);
		}

		private void OnMsgAbilityEffectStart(MsgAbilityEffectStart msg)
		{
			if (msg.m_loop)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, msg.m_effectName);
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			GameObject gameObject2 = StateUtil.CreateEffect(this, msg.m_effectName, true, ResourceCategory.CHAO_MODEL);
			if (gameObject2 != null && msg.m_center)
			{
				StateUtil.SetObjectLocalPositionToCenter(this, gameObject2);
			}
		}

		private void OnMsgAbilityEffectEnd(MsgAbilityEffectEnd msg)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, msg.m_effectName);
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		private void OnBossBoostLevel(MsgBossBoostLevel msg)
		{
			SetBoostLevel(msg.m_wispBoostLevel, msg.m_wispBoostEffect);
		}

		public static FSMStateFactory<CharacterState>[] GetCommonFSMTable()
		{
			return new FSMStateFactory<CharacterState>[27]
			{
				new FSMStateFactory<CharacterState>(1, new StateEdit()),
				new FSMStateFactory<CharacterState>(2, new StateRun()),
				new FSMStateFactory<CharacterState>(3, new StateJump()),
				new FSMStateFactory<CharacterState>(8, new StateFall()),
				new FSMStateFactory<CharacterState>(9, new StateDamage()),
				new FSMStateFactory<CharacterState>(5, new StateSpringJump()),
				new FSMStateFactory<CharacterState>(6, new StateDashRing()),
				new FSMStateFactory<CharacterState>(10, new StateFallingDead()),
				new FSMStateFactory<CharacterState>(11, new StateDead()),
				new FSMStateFactory<CharacterState>(7, new StateAfterSpinAttack()),
				new FSMStateFactory<CharacterState>(12, new StateRunLoop()),
				new FSMStateFactory<CharacterState>(13, new StateChangePhantom()),
				new FSMStateFactory<CharacterState>(14, new StateReturnFromPhantom()),
				new FSMStateFactory<CharacterState>(15, new StatePhantomLaser()),
				new FSMStateFactory<CharacterState>(16, new StatePhantomLaserBoss()),
				new FSMStateFactory<CharacterState>(17, new StatePhantomDrill()),
				new FSMStateFactory<CharacterState>(18, new StatePhantomDrillBoss()),
				new FSMStateFactory<CharacterState>(19, new StatePhantomAsteroid()),
				new FSMStateFactory<CharacterState>(20, new StatePhantomAsteroidBoss()),
				new FSMStateFactory<CharacterState>(21, new StateReachCannon()),
				new FSMStateFactory<CharacterState>(22, new StateLaunchCannon()),
				new FSMStateFactory<CharacterState>(23, new StateHold()),
				new FSMStateFactory<CharacterState>(24, new StateTrickJump()),
				new FSMStateFactory<CharacterState>(25, new StateStumble()),
				new FSMStateFactory<CharacterState>(26, new StateDoubleJump()),
				new FSMStateFactory<CharacterState>(27, new StateThirdJump()),
				new FSMStateFactory<CharacterState>(28, new StateLastChance())
			};
		}
	}
}
