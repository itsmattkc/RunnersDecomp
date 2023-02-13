using App.Utility;
using DataTable;
using Message;
using System.Collections;
using UnityEngine;

namespace Chao
{
	public class ChaoState : MonoBehaviour
	{
		private enum EventSignal
		{
			ENTER_TRIGGER = 100
		}

		private abstract class StateWork
		{
			private string name;

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public abstract void Destroy();
		}

		private enum FlagState
		{
			StatesStop,
			ReqestStop,
			RreviveProduct
		}

		private enum SubStateKillOut
		{
			Up,
			Forward,
			Wait
		}

		private enum AttackBossSubState
		{
			Up,
			Attack,
			After
		}

		private class AttackBossWork : StateWork
		{
			public GameObject m_attackCollision;

			public GameObject m_targetObject;

			public void DestroyAttackCollision()
			{
				if (m_attackCollision != null)
				{
					Object.Destroy(m_attackCollision);
					m_attackCollision = null;
				}
			}

			public override void Destroy()
			{
				DestroyAttackCollision();
				m_targetObject = null;
			}
		}

		private enum ChaoWalkerState
		{
			Action,
			Wait,
			SubAction,
			Out
		}

		private enum SubStateItemPresent
		{
			Action,
			Wait,
			SubAction,
			Out
		}

		private enum StockRingState
		{
			IDLE,
			CHANGE_MOVEMENT,
			PLAY_SPIN_MOTION,
			PLAY_EFECT,
			PLAY_EFFECT,
			WAIT_END
		}

		private const float ShaderLineOffsetMain = 5f;

		private const float ShaderLineOffsetSub = 15f;

		private const string ShaderName = "ykChrLine_dme1";

		private const string ChangeParamOutline = "_OutlineZOffset";

		private const string ChangeParamInnner = "_InnerZOffset";

		private const float InvalidExtremeSpeedRate = 1.5f;

		private const float EasySpeed_SpeedRate = 3f;

		private const float PhantomStartSpeedRate = 5f;

		private const string StockRingEffectName = "ef_ch_tornade_fog_";

		private const string StockRingEffectPost_SR = "sr01";

		private const string StockRingEffectPost_R = "r01";

		private const string StockRingEffectPost_N = "c01";

		private const int CHAOS_ID = 2014;

		private const int DEATH_EGG_ID = 2015;

		private const int PUYO_CARBUNCLE_ID = 2012;

		private const int PUYO_SUKETOUDARA_ID = 1018;

		private const int PUYO_PAPURISU_ID = 24;

		private const float ChaoWalkerSpeedRate = 2f;

		private const float m_hardwareAndCartridgeMoveTimer = 0.8f;

		private const float m_hardwareAndCartridgeWaitTimer = 2f;

		private const float HardwareCartridgeSpeedRate = 2f;

		private const float m_nightsMoveTimer = 1.5f;

		private const float m_nightsWaitTimer = 2f;

		private const float NightsSpeedRate = 2f;

		private const float ChaosMoveTimer = 1f;

		private const float ChaosWaitTimer = 2.2f;

		private const float ChaosSpeedRate = 2f;

		private const float PufferFishMoveTimer = 1f;

		private const float PufferFishWaitTimer = 0.5f;

		private const float PufferFishSubActionTimer = 1f;

		private const float PufferFishOutTimer = 0.5f;

		private const float PufferFishSpeedRate = 2.5f;

		private const float ItemPresentSpeedRate = 2f;

		private const float ItemPresentOutSpeedRate = 2f;

		private Bitset32 m_stateFlag;

		private TinyFsmBehavior m_fsmBehavior;

		private ChaoMovement m_movement;

		private ChaoType m_chao_type;

		private int m_chao_id = -1;

		private bool m_startEffect;

		private ChaoSetupParameterData m_setupdata = new ChaoSetupParameterData();

		private StateWork m_stateWork;

		private GameObject m_modelObject;

		private ChaoParameter m_parameter;

		private ChaoModelPostureController m_modelControl;

		private int m_attackCount;

		private int m_substate;

		private float m_stateTimer;

		private ItemType m_chaoItemType = ItemType.UNKNOWN;

		private static readonly Vector3 InvalidExtremeOffsetRate = new Vector3(0.5f, 0.7f, 0f);

		private static readonly ItemType[] ChaoAbilityItemTbl = new ItemType[5]
		{
			ItemType.COMBO,
			ItemType.MAGNET,
			ItemType.INVINCIBLE,
			ItemType.BARRIER,
			ItemType.TRAMPOLINE
		};

		private static readonly ItemType[] ChaoAbilityItemTblPhantom = new ItemType[2]
		{
			ItemType.COMBO,
			ItemType.MAGNET
		};

		private static readonly ItemType[] ChaoAbilityPhantomTbl = new ItemType[3]
		{
			ItemType.LASER,
			ItemType.DRILL,
			ItemType.ASTEROID
		};

		private static readonly Vector3 AttackBossModelSpinSpeed = new Vector3(1440f, 0f, 0f);

		private ChaoWalkerState m_chaoWalkerState;

		private static readonly Vector3 ChaoWalkerOffsetRate = new Vector3(0.5f, 0.7f, 0f);

		private static readonly Vector3 HardwareCartridgeOffsetRate = new Vector3(0.5f, 0.5f, 0f);

		private static readonly Vector3 NightsOffsetRate = new Vector3(0.66f, 0.6f, 0f);

		private static readonly Vector3 RealaOffsetRate = new Vector3(0.33f, 0.6f, 0f);

		private static readonly Vector3 ChaosOffsetRate = new Vector3(0.5f, 0.5f, 0f);

		private static readonly Vector3 PufferFishOffsetRate = new Vector3(0.5f, 0.75f, 0f);

		private static readonly Vector3 ItemPresentOffsetRate = new Vector3(0.5f, 0.7f, 0f);

		private static readonly Vector3 CuebotItemOffsetRate = new Vector3(0.5f, 0.6f, 0f);

		private static readonly Vector3 OrbotItemtOffsetRate = new Vector3(0.5f, 0.78f, 0f);

		private Camera m_uiCamera;

		private GameObject m_effectObj;

		private GameObject m_itemBtnObj;

		private Vector3 m_effectScreenPos = Vector3.zero;

		private Vector3 m_targetScreenPos = Vector3.zero;

		private float m_distance;

		private bool m_presentFlag;

		private StockRingState m_stockRingState;

		private static readonly Vector3 StockRingModelSpinSpeed = new Vector3(0f, 0f, 360f);

		public GameObject ModelObject
		{
			get
			{
				return m_modelObject;
			}
		}

		public ChaoParameterData Parameter
		{
			get
			{
				if (m_parameter != null)
				{
					return m_parameter.Data;
				}
				return null;
			}
		}

		public ShaderType ShaderOffset
		{
			get
			{
				if (m_setupdata != null)
				{
					return m_setupdata.ShaderOffset;
				}
				return ShaderType.NORMAL;
			}
		}

		private void CreateCollider()
		{
			base.gameObject.AddComponent(typeof(SphereCollider));
			SphereCollider component = base.gameObject.GetComponent<SphereCollider>();
			if (component != null)
			{
				component.isTrigger = true;
				Vector3 vector = component.center = m_setupdata.ColliCenter;
				component.radius = m_setupdata.ColliRadius;
			}
		}

		private void SetupModelAndParameter()
		{
			ResourceManager instance = ResourceManager.Instance;
			if (!(instance != null))
			{
				return;
			}
			string name = null;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(instance.gameObject, "ChaoModel" + m_chao_id.ToString("0000"));
			if (gameObject != null)
			{
				int childCount = gameObject.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
					if (gameObject2.name.IndexOf("cho_") == 0)
					{
						name = gameObject2.name;
					}
				}
				ChaoSetupParameter component = gameObject.GetComponent<ChaoSetupParameter>();
				if (component != null)
				{
					component.Data.CopyTo(m_setupdata);
				}
			}
			GameObject gameObject3 = instance.GetGameObject(ResourceCategory.CHAO_MODEL, name);
			CreateChildModelObject(gameObject3, true);
		}

		private void SetupModelPostureController(GameObject modelObject)
		{
			m_modelControl = modelObject.AddComponent<ChaoModelPostureController>();
			m_modelControl.SetModelObject(modelObject);
		}

		private void CreateChildModelObject(GameObject src_obj, bool active)
		{
			if (!(src_obj != null))
			{
				return;
			}
			Vector3 localPosition = src_obj.transform.localPosition;
			Quaternion localRotation = src_obj.transform.localRotation;
			GameObject gameObject = Object.Instantiate(src_obj, localPosition, localRotation) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.parent = base.transform;
				gameObject.SetActive(active);
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.localRotation = localRotation;
				gameObject.name = src_obj.name;
				OffAnimatorRootAnimation(gameObject);
				m_modelObject = gameObject;
				SetupModelPostureController(m_modelObject);
				float shaderOffsetValue = GetShaderOffsetValue();
				ChangeShaderOffsetChild(m_modelObject, shaderOffsetValue);
				ChaoPartsObjectMagnet component = gameObject.GetComponent<ChaoPartsObjectMagnet>();
				if (component != null)
				{
					component.Setup();
				}
			}
		}

		private void OffAnimatorRootAnimation(GameObject modelObject)
		{
			if (modelObject != null)
			{
				Animator component = modelObject.GetComponent<Animator>();
				if (component != null)
				{
					component.applyRootMotion = false;
				}
			}
		}

		private float GetShaderOffsetValue()
		{
			ChaoType chaoType = ChaoUtility.GetChaoType(base.gameObject);
			float result = (chaoType != 0) ? 15f : 5f;
			if (m_setupdata.ShaderOffset != 0)
			{
				ChaoType type = (chaoType == ChaoType.MAIN) ? ChaoType.SUB : ChaoType.MAIN;
				if (m_setupdata.ShaderOffset == ShaderType.MAIN)
				{
					if (ChaoUtility.GetChaoShaderType(base.gameObject.transform.parent.gameObject, type) == ShaderType.SUB)
					{
						result = 5f;
					}
				}
				else if (m_setupdata.ShaderOffset == ShaderType.SUB && ChaoUtility.GetChaoShaderType(base.gameObject.transform.parent.gameObject, type) == ShaderType.MAIN)
				{
					result = 15f;
				}
			}
			return result;
		}

		private void ChangeShaderOffsetChild(GameObject parent, float offset)
		{
			foreach (Transform item in parent.transform)
			{
				ChangeShaderOffsetChild(item.gameObject, offset);
				Renderer component = item.GetComponent<Renderer>();
				if (!(component != null))
				{
					continue;
				}
				Material[] materials = component.materials;
				Material[] array = materials;
				foreach (Material material in array)
				{
					if (material.HasProperty("_OutlineZOffset"))
					{
						float @float = material.GetFloat("_OutlineZOffset");
						material.SetFloat("_OutlineZOffset", @float + offset);
					}
					if (material.HasProperty("_InnerZOffset"))
					{
						float float2 = material.GetFloat("_InnerZOffset");
						material.SetFloat("_InnerZOffset", float2 + offset);
					}
				}
			}
		}

		public void Start()
		{
			SetChaoData();
			if (m_chao_id < 0)
			{
				base.gameObject.SetActive(false);
				return;
			}
			SetupModelAndParameter();
			CreateTinyFsm();
			SetChaoMovement();
			base.enabled = false;
		}

		private void OnDestroy()
		{
			if (m_fsmBehavior != null)
			{
				m_fsmBehavior.ShutDown();
				m_fsmBehavior = null;
			}
			DestroyStateWork();
		}

		private void CreateTinyFsm()
		{
			m_fsmBehavior = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
			if (m_fsmBehavior != null)
			{
				TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
				description.initState = new TinyFsmState(StateInit);
				m_fsmBehavior.SetUp(description);
			}
		}

		private StageItemManager GetItemManager()
		{
			if (StageItemManager.Instance != null)
			{
				return StageItemManager.Instance;
			}
			return null;
		}

		public T GetCurrentMovement<T>() where T : FSMState<ChaoMovement>
		{
			return m_movement.GetCurrentState<T>();
		}

		private void SetChaoData()
		{
			m_chao_type = ChaoUtility.GetChaoType(base.gameObject);
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				switch (m_chao_type)
				{
				case ChaoType.MAIN:
					m_chao_id = instance.PlayerData.MainChaoID;
					break;
				case ChaoType.SUB:
					m_chao_id = instance.PlayerData.SubChaoID;
					break;
				}
			}
			GameObject gameObject = GameObject.Find("StageChao/ChaoParameter");
			if (gameObject != null)
			{
				m_parameter = gameObject.GetComponent<ChaoParameter>();
			}
		}

		private static ChaoData.Rarity GetRarity(int chaoId)
		{
			ChaoData chaoData = ChaoTable.GetChaoData(chaoId);
			if (chaoData != null)
			{
				return chaoData.rarity;
			}
			return ChaoData.Rarity.NONE;
		}

		private void RequestStartEffect()
		{
			if (!m_startEffect)
			{
				m_startEffect = true;
				StageAbilityManager instance = StageAbilityManager.Instance;
				if (instance != null)
				{
					instance.RequestPlayChaoEffect(ChaoAbility.ALL_BONUS_COUNT, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.SCORE_COUNT, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.RING_COUNT, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.RED_RING_COUNT, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.ANIMAL_COUNT, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.RUNNIGN_DISTANCE, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.RARE_ENEMY_UP, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.ENEMY_SCORE, m_chao_type);
					instance.RequestPlayChaoEffect(ChaoAbility.COMBO_RECEPTION_TIME, m_chao_type);
				}
			}
		}

		private void SetChaoMovement()
		{
			m_movement = ChaoMovement.Create(base.gameObject, m_setupdata);
		}

		private void ChangeState(TinyFsmState nextState)
		{
			if (m_fsmBehavior != null)
			{
				m_fsmBehavior.ChangeState(nextState);
			}
		}

		private void ChangeMovement(MOVESTATE_ID state)
		{
			if (m_movement != null)
			{
				m_movement.ChangeState(state);
			}
		}

		private TinyFsmState GetCurrentState()
		{
			if (m_fsmBehavior != null)
			{
				return m_fsmBehavior.GetCurrentState();
			}
			return null;
		}

		private void OnMsgReceive(MsgChaoState message)
		{
			if (message != null && m_fsmBehavior != null)
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateMessage(message);
				m_fsmBehavior.Dispatch(signal);
			}
		}

		private void OnMsgStageReplace(MsgStageReplace msg)
		{
			ChangeState(new TinyFsmState(StateComeIn));
		}

		private void OnMsgStartBoss()
		{
			m_attackCount = 0;
		}

		private void OnStartLastChance(MsgStartLastChance message)
		{
			StageAbilityManager instance = StageAbilityManager.Instance;
			if (instance != null && instance.HasChaoAbility(ChaoAbility.LAST_CHANCE, m_chao_type) && message != null && m_fsmBehavior != null)
			{
				MsgChaoState msg = new MsgChaoState(MsgChaoState.State.LAST_CHANCE);
				TinyFsmEvent signal = TinyFsmEvent.CreateMessage(msg);
				m_fsmBehavior.Dispatch(signal);
			}
		}

		private void OnEndLastChance(MsgEndLastChance message)
		{
			if (message != null && m_fsmBehavior != null)
			{
				MsgChaoState msg = new MsgChaoState(MsgChaoState.State.LAST_CHANCE_END);
				TinyFsmEvent signal = TinyFsmEvent.CreateMessage(msg);
				m_fsmBehavior.Dispatch(signal);
			}
		}

		private void OnPauseChangeLevel()
		{
			SetMagnetPause(true);
		}

		private void OnResumeChangeLevel()
		{
			SetMagnetPause(false);
		}

		private void OnMsgChaoAbilityStart(MsgChaoAbilityStart msg)
		{
			StageAbilityManager instance = StageAbilityManager.Instance;
			ChaoAbility[] ability = msg.m_ability;
			foreach (ChaoAbility chaoAbility in ability)
			{
				if (!(instance != null) || !instance.HasChaoAbility(chaoAbility, m_chao_type))
				{
					continue;
				}
				switch (chaoAbility)
				{
				case ChaoAbility.RUNNIGN_DISTANCE:
				case ChaoAbility.SPECIAL_CRYSTAL_COUNT:
				case ChaoAbility.RAID_BOSS_RING_COUNT:
				case ChaoAbility.COMBO_SMALL_CRYSTAL_RED:
				case ChaoAbility.COMBO_SMALL_CRYSTAL_GREEN:
				case ChaoAbility.COMBO_BIG_CRYSTAL_RED:
				case ChaoAbility.COMBO_BIG_CRYSTAL_GREEN:
				case ChaoAbility.COMBO_RARE_ENEMY:
				case ChaoAbility.COMBO_RECOVERY_ANIMAL:
				case ChaoAbility.COMBO_SUPER_RING:
				case ChaoAbility.COMBO_ADD_COMBO_VALUE:
				case ChaoAbility.ITEM_TIME:
				case ChaoAbility.COMBO_TIME:
				case ChaoAbility.TRAMPOLINE_TIME:
				case ChaoAbility.MAGNET_TIME:
				case ChaoAbility.RARE_ENEMY_UP:
				case ChaoAbility.SPECIAL_CRYSTAL_RATE:
				case ChaoAbility.LAST_CHANCE:
				case ChaoAbility.MAP_BOSS_DAMAGE:
				case ChaoAbility.CHECK_POINT_COMBO_UP:
				case ChaoAbility.SPIN_DASH_MAGNET:
				case ChaoAbility.JUMP_RAMP:
				case ChaoAbility.ENEMY_SCORE:
				case ChaoAbility.MAGNET_RANGE:
				case ChaoAbility.AGGRESSIVITY_UP_FOR_RAID_BOSS:
				case ChaoAbility.CANNON_MAGNET:
				case ChaoAbility.DASH_RING_MAGNET:
				case ChaoAbility.JUMP_RAMP_MAGNET:
				case ChaoAbility.SPECIAL_ANIMAL:
					break;
				case ChaoAbility.ANIMAL_COUNT:
					if (m_chao_id == 2014)
					{
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp4_deatheggstar_flash_sr01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					}
					break;
				case ChaoAbility.BOSS_ATTACK:
					if ((float)m_attackCount < instance.GetChaoAbilityValue(chaoAbility))
					{
						msg.m_flag = true;
						ChangeState(new TinyFsmState(StateAttackBoss));
					}
					break;
				case ChaoAbility.PURSUES_TO_BOSS_AFTER_ATTACK:
				{
					float num3 = Random.Range(0f, 100f);
					if (num3 < instance.GetChaoAbilityValue(chaoAbility))
					{
						msg.m_flag = true;
						ChangeState(new TinyFsmState(StatePursuesAttackBoss));
					}
					break;
				}
				case ChaoAbility.RECOVERY_RING:
					ChangeState(new TinyFsmState(StateStockRing));
					break;
				case ChaoAbility.COMBO_ITEM_BOX:
					if (!IsNowLastChance())
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateItemPresent));
					}
					break;
				case ChaoAbility.CHECK_POINT_ITEM_BOX:
				{
					PlayerInformation playerInformation3 = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (!(playerInformation3 != null))
					{
						break;
					}
					bool flag = false;
					if (playerInformation3.PhantomType == PhantomType.NONE)
					{
						m_chaoItemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
						flag = CheckItemPresent(m_chaoItemType);
					}
					if (!flag)
					{
						m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
						flag = CheckItemPresent(m_chaoItemType);
					}
					if (flag)
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateItemPresent2));
					}
					break;
				}
				case ChaoAbility.COMBO_COLOR_POWER:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StatePhantomPresent));
					break;
				case ChaoAbility.COMBO_BARRIER:
				{
					m_chaoItemType = ItemType.BARRIER;
					if (!CheckItemPresent(m_chaoItemType))
					{
						break;
					}
					PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation != null)
					{
						if (playerInformation.PhantomType == PhantomType.NONE)
						{
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp2_erazordjinn_magic_sr01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value2 = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddItem", value2, SendMessageOptions.DontRequireReceiver);
						}
					}
					break;
				}
				case ChaoAbility.DAMAGE_TRAMPOLINE:
				{
					if (ObjUtil.GetRandomRange100() >= (int)instance.GetChaoAbilityValue(chaoAbility))
					{
						break;
					}
					m_chaoItemType = ItemType.TRAMPOLINE;
					PlayerInformation playerInformation2 = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation2 != null && playerInformation2.PhantomType == PhantomType.NONE)
					{
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_damaged_trampoline_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						if (m_chao_id == 1018)
						{
							ObjUtil.LightPlaySE("act_chao_puyo");
						}
					}
					StageItemManager itemManager3 = GetItemManager();
					if (itemManager3 != null)
					{
						itemManager3.SendMessage("OnAddDamageTrampoline", null, SendMessageOptions.DontRequireReceiver);
					}
					break;
				}
				case ChaoAbility.BOSS_STAGE_TIME:
					ObjUtil.LightPlaySE("act_arthur");
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					break;
				case ChaoAbility.BOSS_RED_RING_RATE:
				case ChaoAbility.BOSS_SUPER_RING_RATE:
					ObjUtil.LightPlaySE("act_ship_laser");
					break;
				case ChaoAbility.COMBO_WIPE_OUT_ENEMY:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateKillOut));
					break;
				case ChaoAbility.COMBO_RECOVERY_ALL_OBJ:
					if (m_chao_id == 2012)
					{
						ObjUtil.LightPlaySE("act_chao_puyo");
					}
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp3_carbuncle_magic_sr01", -1f);
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					break;
				case ChaoAbility.COMBO_DESTROY_TRAP:
					ObjUtil.LightPlaySE("act_chao_mag");
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_trap_cry_c01", -1f);
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgObjectDeadChaoCombo", new MsgObjectDead(ChaoAbility.COMBO_DESTROY_TRAP));
					break;
				case ChaoAbility.COMBO_DESTROY_AIR_TRAP:
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_brk_airtrap_c01", -1f);
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					if (m_chao_id == 24)
					{
						ObjUtil.LightPlaySE("act_chao_puyo");
					}
					GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgObjectDeadChaoCombo", new MsgObjectDead(ChaoAbility.COMBO_DESTROY_AIR_TRAP));
					break;
				case ChaoAbility.SPECIAL_ANIMAL_PSO2:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.LightPlaySE("act_chao_rappy");
					break;
				case ChaoAbility.COLOR_POWER_SCORE:
				case ChaoAbility.ASTEROID_SCORE:
				case ChaoAbility.DRILL_SCORE:
				case ChaoAbility.LASER_SCORE:
				case ChaoAbility.COLOR_POWER_TIME:
				case ChaoAbility.ASTEROID_TIME:
				case ChaoAbility.DRILL_TIME:
				case ChaoAbility.LASER_TIME:
					ChangeState(new TinyFsmState(StateItemPhantom));
					break;
				case ChaoAbility.RECOVERY_FROM_FAILURE:
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp4_deatheggstar_flash_sr01", -1f);
					ObjUtil.LightPlaySE("act_chao_deathegg");
					break;
				case ChaoAbility.ADD_COMBO_VALUE:
					ChangeState(new TinyFsmState(StateGameCartridge));
					break;
				case ChaoAbility.COMBO_RECEPTION_TIME:
					ChangeState(new TinyFsmState(StateGameHardware));
					break;
				case ChaoAbility.COMBO_ALL_SPECIAL_CRYSTAL:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					if (m_chao_id == 1018)
					{
						ObjUtil.LightPlaySE("act_chao_puyo");
					}
					break;
				case ChaoAbility.LOOP_COMBO_UP:
					ChangeState(new TinyFsmState(StateNights));
					break;
				case ChaoAbility.LOOP_MAGNET:
					ChangeState(new TinyFsmState(StateReala));
					break;
				case ChaoAbility.DAMAGE_DESTROY_ALL:
					if (ObjUtil.GetRandomRange100() < (int)instance.GetChaoAbilityValue(chaoAbility))
					{
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_clb_pian_magic_r01", -1f);
						ObjUtil.LightPlaySE("act_chao_pian");
						MsgObjectDead value3 = new MsgObjectDead();
						GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgObjectDead", value3);
						GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgObjectDead", value3);
					}
					break;
				case ChaoAbility.JUMP_RAMP_TRICK_SUCCESS:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_jumpboard_success_r01", -1f);
					break;
				case ChaoAbility.ITEM_REVIVE:
					if (!m_stateFlag.Test(2))
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateReviveEquipItem));
					}
					break;
				case ChaoAbility.COMBO_EQUIP_ITEM_EXTRA:
					if (!IsNowLastChance())
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StatePresentSRareEquipItem));
					}
					break;
				case ChaoAbility.COMBO_EQUIP_ITEM:
					if (!IsNowLastChance())
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StatePresentEquipItem));
					}
					break;
				case ChaoAbility.COMBO_STEP_DESTROY_GET_10_RING:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_brk_airfloor_r01", -1f);
					GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgStepObjectDead", new MsgObjectDead());
					ObjUtil.LightPlaySE("act_chao_killerwhale");
					break;
				case ChaoAbility.TRANSFER_DOUBLE_RING:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateDoubleRing));
					break;
				case ChaoAbility.ENEMY_SCORE_SEVERALFOLD:
					if (!IsPlayingAbilityAnimation())
					{
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp6_kingboombo_magic_sr01", -1f);
						ObjUtil.LightPlaySE("act_chao_effect");
					}
					break;
				case ChaoAbility.COMBO_METAL_AND_METAL_SCORE:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_enemy_m_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					break;
				case ChaoAbility.GUARD_DROP_RING:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_damaged_keepring_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					break;
				case ChaoAbility.MAGNET_SPEED_TYPE_JUMP:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_typeactionS_magnet_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					break;
				case ChaoAbility.MAGNET_FLY_TYPE_JUMP:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_typeactionF_magnet_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					break;
				case ChaoAbility.MAGNET_POWER_TYPE_JUMP:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_typeactionP_magnet_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					break;
				case ChaoAbility.COMBO_DESTROY_AND_RECOVERY:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_brkall_magnetall_sr01", -1f);
					ObjUtil.LightPlaySE("act_chao_papaopa");
					ObjUtil.SendMessageOnObjectDead();
					GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgStepObjectDead", new MsgObjectDead());
					break;
				case ChaoAbility.COMBO_RING_BANK:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					StartCoroutine(RingBank());
					break;
				case ChaoAbility.ENEMY_COUNT_BOMB:
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_enemy_brk_enemy_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_heavybomb");
					if (StageAbilityManager.Instance != null)
					{
						float chaoAbilityExtraValue = StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.ENEMY_COUNT_BOMB);
						GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgHeavyBombDead", chaoAbilityExtraValue);
					}
					break;
				case ChaoAbility.INVALIDI_EXTREME_STAGE:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateInvalidExtreme));
					break;
				case ChaoAbility.CHAO_RING_MAGNET:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateRingMagnet));
					break;
				case ChaoAbility.CHAO_CRYSTAL_MAGNET:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateCrystalMagnet));
					break;
				case ChaoAbility.COMBO_BONUS_UP:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateComboBonusUp));
					break;
				case ChaoAbility.EASY_SPEED:
					if (m_stateFlag.Test(0))
					{
						m_stateFlag.Set(1, true);
					}
					ChangeState(new TinyFsmState(StateEasySpeed));
					break;
				case ChaoAbility.COMBO_COLOR_POWER_ASTEROID:
				{
					StageItemManager itemManager6 = GetItemManager();
					if (itemManager6 != null)
					{
						m_chaoItemType = ItemType.ASTEROID;
						itemManager6.SendMessage("OnAddColorItem", new MsgAddItemToManager(m_chaoItemType), SendMessageOptions.DontRequireReceiver);
						ObjUtil.LightPlaySE("act_chao_effect");
						ObjUtil.SendGetItemIcon(m_chaoItemType);
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_asteroid_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					}
					break;
				}
				case ChaoAbility.COMBO_COLOR_POWER_DRILL:
				{
					StageItemManager itemManager5 = GetItemManager();
					if (itemManager5 != null)
					{
						m_chaoItemType = ItemType.DRILL;
						itemManager5.SendMessage("OnAddColorItem", new MsgAddItemToManager(m_chaoItemType), SendMessageOptions.DontRequireReceiver);
						ObjUtil.LightPlaySE("act_chao_effect");
						ObjUtil.SendGetItemIcon(m_chaoItemType);
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_drill_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					}
					break;
				}
				case ChaoAbility.COMBO_COLOR_POWER_LASER:
				{
					StageItemManager itemManager4 = GetItemManager();
					if (itemManager4 != null)
					{
						m_chaoItemType = ItemType.LASER;
						itemManager4.SendMessage("OnAddColorItem", new MsgAddItemToManager(m_chaoItemType), SendMessageOptions.DontRequireReceiver);
						ObjUtil.LightPlaySE("act_chao_effect");
						ObjUtil.SendGetItemIcon(m_chaoItemType);
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_laser_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					}
					break;
				}
				case ChaoAbility.COMBO_CHANGE_EQUIP_ITEM:
				{
					StageItemManager itemManager2 = GetItemManager();
					if (itemManager2 != null && itemManager2.IsEquipItem())
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateChangeEquipItem));
					}
					break;
				}
				case ChaoAbility.COMBO_RANDOM_ITEM_MINUS_RING:
				{
					if (IsNowLastChance())
					{
						break;
					}
					long num2 = 500L;
					if (!(StageAbilityManager.Instance != null) || !(StageScoreManager.Instance != null))
					{
						break;
					}
					num2 = (long)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.COMBO_RANDOM_ITEM_MINUS_RING);
					if (StageScoreManager.Instance.DefrayItemCostByRing(num2))
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateOrbotItemPresent));
					}
					else
					{
						ObjUtil.LightPlaySE("sys_error");
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_randombuyitem_fail_r01", -1f);
					}
					break;
				}
				case ChaoAbility.COMBO_RANDOM_ITEM:
				{
					if (IsNowLastChance())
					{
						break;
					}
					float num = 0f;
					if (StageAbilityManager.Instance != null)
					{
						num = StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.COMBO_RANDOM_ITEM);
					}
					if (Random.Range(0f, 100f) < num)
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateCuebotItemPresent));
					}
					else
					{
						if (m_stateFlag.Test(0))
						{
							m_stateFlag.Set(1, true);
						}
						ChangeState(new TinyFsmState(StateFailCuebotItemPresent));
					}
					break;
				}
				case ChaoAbility.JUMP_DESTROY_ENEMY_AND_TRAP:
				{
					MsgObjectDead value = new MsgObjectDead();
					GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgObjectDead", value);
					GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgObjectDead", value);
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_raid2_ganmen_atk_sr01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					break;
				}
				case ChaoAbility.JUMP_DESTROY_ENEMY:
					GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgObjectDead", new MsgObjectDead());
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_nodamage_movetrap_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					break;
				case ChaoAbility.SUPER_RING_UP:
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_up_ring10_r01", -1f);
					ObjUtil.LightPlaySE("act_chao_effect");
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					break;
				}
				break;
			}
		}

		private bool CheckItemPresent(ItemType itemType)
		{
			if (IsNowLastChance())
			{
				return false;
			}
			StageItemManager itemManager = GetItemManager();
			if (itemManager != null)
			{
				MsgAskEquipItemUsed msgAskEquipItemUsed = new MsgAskEquipItemUsed(itemType);
				itemManager.SendMessage("OnAskEquipItemUsed", msgAskEquipItemUsed);
				if (msgAskEquipItemUsed.m_ok)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsNowLastChance()
		{
			PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
			if (playerInformation != null)
			{
				return playerInformation.IsNowLastChance();
			}
			return false;
		}

		private void OnMsgChaoAbilityEnd(MsgChaoAbilityEnd msg)
		{
			StageAbilityManager instance = StageAbilityManager.Instance;
			ChaoAbility[] ability = msg.m_ability;
			foreach (ChaoAbility ability2 in ability)
			{
				if (instance != null && instance.HasChaoAbility(ability2, m_chao_type) && instance != null && instance.HasChaoAbility(ability2, m_chao_type))
				{
					TinyFsmEvent signal = TinyFsmEvent.CreateMessage(msg);
					m_fsmBehavior.Dispatch(signal);
					break;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEventObject(100, base.collider);
			m_fsmBehavior.Dispatch(signal);
		}

		private void OnTriggerStay(Collider other)
		{
		}

		private void OnTriggerExit(Collider other)
		{
		}

		private void OnDamageSucceed(MsgHitDamageSucceed msg)
		{
			if (m_fsmBehavior != null)
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateMessage(msg);
				m_fsmBehavior.Dispatch(signal);
			}
		}

		private T CreateStateWork<T>() where T : StateWork, new()
		{
			if (m_stateWork != null)
			{
				DestroyStateWork();
			}
			T val = new T();
			val.Name = val.ToString();
			m_stateWork = val;
			return val;
		}

		private void DestroyStateWork()
		{
			if (m_stateWork != null)
			{
				m_stateWork.Destroy();
				m_stateWork = null;
			}
		}

		private T GetStateWork<T>() where T : StateWork
		{
			if (m_stateWork != null && m_stateWork is T)
			{
				return m_stateWork as T;
			}
			return (T)null;
		}

		private TinyFsmState StateIdle(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				return TinyFsmState.End();
			case -4:
				return TinyFsmState.End();
			case 0:
				ChangeState(new TinyFsmState(StateComeIn));
				return TinyFsmState.End();
			case 1:
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateInit(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				return TinyFsmState.End();
			case -4:
				return TinyFsmState.End();
			case 0:
				ChangeState(new TinyFsmState(StateComeIn));
				return TinyFsmState.End();
			case 1:
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateComeIn(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				m_stateFlag.Set(0, false);
				m_stateFlag.Set(1, false);
				ChangeMovement(MOVESTATE_ID.ComeIn);
				return TinyFsmState.End();
			case -4:
				return TinyFsmState.End();
			case 0:
				if (m_movement != null && m_movement.NextState)
				{
					ChangeState(new TinyFsmState(StatePursue));
				}
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null)
				{
					MsgChaoState.State state = msgChaoState.state;
					if (state == MsgChaoState.State.STOP)
					{
						ChangeState(new TinyFsmState(StateStop));
					}
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePursue(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				m_stateFlag.Set(0, false);
				m_stateFlag.Set(1, false);
				RequestStartEffect();
				ChangeMovement(MOVESTATE_ID.Pursue);
				return TinyFsmState.End();
			case -4:
				return TinyFsmState.End();
			case 0:
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null)
				{
					switch (msgChaoState.state)
					{
					case MsgChaoState.State.STOP:
						ChangeState(new TinyFsmState(StateStop));
						break;
					case MsgChaoState.State.LAST_CHANCE:
						ChangeState(new TinyFsmState(StateLastChance));
						break;
					}
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateLastChance(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.LastChance);
				StageAbilityManager instance2 = StageAbilityManager.Instance;
				if (instance2 != null)
				{
					instance2.RequestPlayChaoEffect(ChaoAbility.LAST_CHANCE, m_chao_type);
				}
				ObjUtil.LightPlaySE("act_chip_fly");
				return TinyFsmState.End();
			}
			case -4:
			{
				StageAbilityManager instance = StageAbilityManager.Instance;
				if (instance != null)
				{
					instance.RequestStopChaoEffect(ChaoAbility.LAST_CHANCE);
				}
				return TinyFsmState.End();
			}
			case 0:
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null && msgChaoState.state == MsgChaoState.State.LAST_CHANCE_END)
				{
					ChangeState(new TinyFsmState(StateLastChanceEnd));
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateLastChanceEnd(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				SetAnimationFlagForAbility(true);
				ObjUtil.LightPlaySE("act_chip_pose");
				ChangeMovement(MOVESTATE_ID.LastChanceEnd);
				return TinyFsmState.End();
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null)
				{
					if (msgChaoState.state == MsgChaoState.State.COME_IN)
					{
						ChangeState(new TinyFsmState(StateComeIn));
					}
					else if (msgChaoState.state == MsgChaoState.State.STOP_END)
					{
						ChangeState(new TinyFsmState(StateStopEnd));
					}
					else if (msgChaoState.state == MsgChaoState.State.STOP)
					{
						ChangeState(new TinyFsmState(StateStop));
					}
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateStop(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				m_stateFlag.Set(0, true);
				m_stateFlag.Set(1, false);
				ChangeMovement(MOVESTATE_ID.Stop);
				return TinyFsmState.End();
			case -4:
				m_stateFlag.Set(0, false);
				return TinyFsmState.End();
			case 0:
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null)
				{
					switch (msgChaoState.state)
					{
					case MsgChaoState.State.STOP_END:
						ChangeState(new TinyFsmState(StateStopEnd));
						break;
					case MsgChaoState.State.COME_IN:
						ChangeState(new TinyFsmState(StateComeIn));
						break;
					case MsgChaoState.State.LAST_CHANCE:
						ChangeState(new TinyFsmState(StateLastChance));
						break;
					}
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateStopEnd(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				ChangeMovement(MOVESTATE_ID.StopEnd);
				return TinyFsmState.End();
			case -4:
				return TinyFsmState.End();
			case 0:
				if (m_movement != null && m_movement.NextState)
				{
					ChangeState(new TinyFsmState(StatePursue));
				}
				return TinyFsmState.End();
			case 1:
			{
				MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
				if (msgChaoState != null)
				{
					switch (msgChaoState.state)
					{
					case MsgChaoState.State.STOP:
						ChangeState(new TinyFsmState(StateStop));
						break;
					case MsgChaoState.State.COME_IN:
						ChangeState(new TinyFsmState(StateComeIn));
						break;
					}
				}
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateInvalidExtreme(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget2);
				ChaoMoveGoCameraTargetUsePlayerSpeed currentMovement = GetCurrentMovement<ChaoMoveGoCameraTargetUsePlayerSpeed>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(InvalidExtremeOffsetRate, 1.5f);
				}
				m_substate = 0;
				m_stateTimer = 0.8f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_magic_darkqueen_sr01", -1f);
						ObjUtil.LightPlaySE("act_sharla_magic");
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateComboBonusUp(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 1f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_bonus_comboscore_sr01", -1f);
						ObjUtil.LightPlaySE("act_chao_effect");
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 1f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateEasySpeed(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget2);
				ChaoMoveGoCameraTargetUsePlayerSpeed currentMovement = GetCurrentMovement<ChaoMoveGoCameraTargetUsePlayerSpeed>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(InvalidExtremeOffsetRate, 3f);
				}
				m_substate = 0;
				m_stateTimer = 0.8f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_clb_nights_magic_sr02", -1f);
						ObjUtil.LightPlaySE("act_chao_effect");
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateOutItemPhantom(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.OutCameraTarget);
				ChaoMoveOutCameraTarget currentMovement = GetCurrentMovement<ChaoMoveOutCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				return TinyFsmState.End();
			}
			case -4:
				return TinyFsmState.End();
			case 0:
				if (m_movement != null && m_movement.NextState)
				{
					ChangeState(new TinyFsmState(StateStop));
				}
				else if (!m_stateFlag.Test(1))
				{
					ChangeState(new TinyFsmState(StateStopEnd));
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateItemPhantom(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					Vector3 screenOffsetRate = (m_chao_type != ChaoType.SUB) ? new Vector3(0.5f, 0.5f, 0f) : new Vector3(0.5f, 0.7f, 0f);
					currentMovement.SetParameter(screenOffsetRate, 5f);
				}
				SetAnimationFlagForAbility(true);
				m_stateTimer = 1f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				m_stateTimer -= getDeltaTime;
				if (m_stateTimer < 0f)
				{
					ChangeMovement(MOVESTATE_ID.Stay);
				}
				return TinyFsmState.End();
			}
			case 1:
				switch (fsmEvent.GetMessage.ID)
				{
				case 21762:
					ChangeMovement(MOVESTATE_ID.Stay);
					return TinyFsmState.End();
				case 21760:
				{
					MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
					if (msgChaoState != null && msgChaoState.state == MsgChaoState.State.COME_IN)
					{
						ChangeState(new TinyFsmState(StateComeIn));
						return TinyFsmState.End();
					}
					break;
				}
				}
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateKillOut(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				ChangeMovement(MOVESTATE_ID.GoKillOut);
				m_stateTimer = 1f;
				m_stateFlag.Reset();
				m_substate = 0;
				if (StageAbilityManager.Instance != null)
				{
					StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.COMBO_WIPE_OUT_ENEMY, m_chao_type);
				}
				return TinyFsmState.End();
			case -4:
				if (StageAbilityManager.Instance != null)
				{
					StageAbilityManager.Instance.RequestStopChaoEffect(ChaoAbility.COMBO_WIPE_OUT_ENEMY);
				}
				if (StageComboManager.Instance != null)
				{
					StageComboManager.Instance.SetChaoFlagStatus(StageComboManager.ChaoFlagStatus.ENEMY_DEAD, false);
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				m_stateTimer -= getDeltaTime;
				switch (m_substate)
				{
				case 0:
					if (m_stateTimer < 0f)
					{
						m_stateTimer = 1f;
						ChaoMoveGoKillOut currentMovement = GetCurrentMovement<ChaoMoveGoKillOut>();
						if (currentMovement != null)
						{
							currentMovement.ChangeMode(ChaoMoveGoKillOut.Mode.Forward);
						}
						m_substate = 1;
					}
					break;
				case 1:
					if (m_stateTimer < 0f)
					{
						GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgObjectDead", new MsgObjectDead());
						if (StageComboManager.Instance != null)
						{
							StageComboManager.Instance.SetChaoFlagStatus(StageComboManager.ChaoFlagStatus.ENEMY_DEAD, true);
						}
						ObjUtil.LightPlaySE("act_exp");
						Camera camera = GameObjectUtil.FindGameObjectComponentWithTag<Camera>("MainCamera", "GameMainCamera");
						if (camera != null)
						{
							Vector3 zero = Vector3.zero;
							Vector3 position = base.transform.position;
							float z = position.z;
							Vector3 position2 = camera.transform.position;
							zero.z = z - position2.z;
							ObjUtil.PlayChaoEffect(camera.gameObject, "ef_ch_bomber_atk_r01", zero, -1f);
						}
						m_substate = 2;
						m_stateTimer = StageComboManager.CHAO_OBJ_DEAD_TIME;
					}
					break;
				case 2:
					if (!m_stateFlag.Test(1) && m_stateTimer < 0f)
					{
						ChangeState(new TinyFsmState(StateComeIn));
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private void SetAnimationFlagForAbility(bool value)
		{
			if (!m_fsmBehavior.NowShutDown && m_modelObject != null)
			{
				Animator component = m_modelObject.GetComponent<Animator>();
				if (component != null && component.enabled)
				{
					component.SetBool("Ability", value);
				}
			}
		}

		private IEnumerator SetAnimationFlagForAbilityCourutine(bool value)
		{
			if (m_fsmBehavior.NowShutDown || !(m_modelObject != null))
			{
				yield break;
			}
			Animator animator = m_modelObject.GetComponent<Animator>();
			if (!(animator != null))
			{
				yield break;
			}
			animator.SetBool("Ability", value);
			if (value)
			{
				int wait = 2;
				while (wait > 0)
				{
					wait--;
					yield return null;
				}
				if (animator != null && animator.enabled)
				{
					animator.SetBool("Ability", false);
				}
			}
		}

		private bool IsPlayingAbilityAnimation()
		{
			if (m_modelObject != null)
			{
				Animator component = m_modelObject.GetComponent<Animator>();
				if (component != null)
				{
					return component.GetBool("Ability");
				}
			}
			return false;
		}

		private void SetMessageComeInOut(TinyFsmEvent fsmEvent)
		{
			MsgChaoState msgChaoState = fsmEvent.GetMessage as MsgChaoState;
			if (msgChaoState != null)
			{
				switch (msgChaoState.state)
				{
				case MsgChaoState.State.COME_IN:
					m_stateFlag.Set(1, false);
					break;
				case MsgChaoState.State.STOP:
					m_stateFlag.Set(1, true);
					break;
				case MsgChaoState.State.STOP_END:
					m_stateFlag.Set(1, false);
					break;
				}
			}
		}

		private TinyFsmState StateAttackBoss(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				ChangeMovement(MOVESTATE_ID.AttackBoss);
				CreateAttackBossWork();
				m_substate = 0;
				m_stateTimer = 0.8f;
				SetSpinModelControl(true);
				ObjUtil.LightPlaySE("act_sword_fly");
				return TinyFsmState.End();
			case -4:
				DestroyStateWork();
				SetSpinModelControl(false);
				return TinyFsmState.End();
			case 0:
			{
				AttackBossWork stateWork4 = GetStateWork<AttackBossWork>();
				if (stateWork4 != null)
				{
					if (stateWork4.m_targetObject == null)
					{
						ChangeState(new TinyFsmState(StatePursue));
						return TinyFsmState.End();
					}
					float getDeltaTime = fsmEvent.GetDeltaTime;
					if (stateWork4 != null)
					{
						UpdateAttackBoss(fsmEvent, getDeltaTime, stateWork4);
					}
					return TinyFsmState.End();
				}
				ChangeState(new TinyFsmState(StatePursue));
				return TinyFsmState.End();
			}
			case 1:
				switch (fsmEvent.GetMessage.ID)
				{
				case 16385:
				{
					AttackBossWork stateWork3 = GetStateWork<AttackBossWork>();
					AttackBossCheckAndChanegStateOnDamageBossSucceed(fsmEvent, stateWork3);
					return TinyFsmState.End();
				}
				case 21762:
				{
					AttackBossWork stateWork2 = GetStateWork<AttackBossWork>();
					if (stateWork2 != null && m_substate < 2)
					{
						ChangeState(new TinyFsmState(StatePursue));
						return TinyFsmState.End();
					}
					break;
				}
				case 12323:
				{
					MsgBossCheckState msgBossCheckState = fsmEvent.GetMessage as MsgBossCheckState;
					if (msgBossCheckState != null && !msgBossCheckState.IsAttackOK())
					{
						AttackBossWork stateWork = GetStateWork<AttackBossWork>();
						if (stateWork != null && m_substate < 2)
						{
							ChangeState(new TinyFsmState(StatePursue));
							return TinyFsmState.End();
						}
					}
					break;
				}
				}
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePursuesAttackBoss(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				ChangeMovement(MOVESTATE_ID.AttackBoss);
				CreateAttackBossWork();
				m_substate = 0;
				m_stateTimer = 0.8f;
				SetSpinModelControl(true);
				ObjUtil.LightPlaySE("act_sword_fly");
				return TinyFsmState.End();
			case -4:
				DestroyStateWork();
				SetSpinModelControl(false);
				return TinyFsmState.End();
			case 0:
			{
				AttackBossWork stateWork4 = GetStateWork<AttackBossWork>();
				if (stateWork4 != null)
				{
					if (stateWork4.m_targetObject == null)
					{
						ChangeState(new TinyFsmState(StatePursue));
						return TinyFsmState.End();
					}
					float getDeltaTime = fsmEvent.GetDeltaTime;
					if (stateWork4 != null)
					{
						UpdatePursuesAttackBoss(fsmEvent, getDeltaTime, stateWork4);
					}
					return TinyFsmState.End();
				}
				ChangeState(new TinyFsmState(StatePursue));
				return TinyFsmState.End();
			}
			case 1:
				switch (fsmEvent.GetMessage.ID)
				{
				case 16385:
				{
					AttackBossWork stateWork3 = GetStateWork<AttackBossWork>();
					PursuesAttackBossCheckAndChanegStateOnDamageBossSucceed(fsmEvent, stateWork3);
					return TinyFsmState.End();
				}
				case 21762:
				{
					AttackBossWork stateWork2 = GetStateWork<AttackBossWork>();
					if (stateWork2 != null && m_substate < 2)
					{
						ChangeState(new TinyFsmState(StatePursue));
						return TinyFsmState.End();
					}
					break;
				}
				case 12323:
				{
					MsgBossCheckState msgBossCheckState = fsmEvent.GetMessage as MsgBossCheckState;
					if (msgBossCheckState != null && !msgBossCheckState.IsAttackOK())
					{
						AttackBossWork stateWork = GetStateWork<AttackBossWork>();
						if (stateWork != null && m_substate < 2)
						{
							ChangeState(new TinyFsmState(StatePursue));
							return TinyFsmState.End();
						}
					}
					break;
				}
				}
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState UpdateAttackBoss(TinyFsmEvent fsmEvent, float deltaTime, AttackBossWork work)
		{
			switch (m_substate)
			{
			case 0:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					m_stateTimer = 2f;
					SetChaoMoveAttackBoss(ChaoMoveAttackBoss.Mode.Homing);
					SetSpinModelControl(false);
					m_substate = 1;
				}
				break;
			case 1:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					ChangeState(new TinyFsmState(StatePursue));
					return TinyFsmState.End();
				}
				break;
			case 2:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					ChangeState(new TinyFsmState(StatePursue));
					return TinyFsmState.End();
				}
				break;
			}
			return TinyFsmState.End();
		}

		private TinyFsmState UpdatePursuesAttackBoss(TinyFsmEvent fsmEvent, float deltaTime, AttackBossWork work)
		{
			switch (m_substate)
			{
			case 0:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					m_stateTimer = 2f;
					SetChaoMoveAttackBoss(ChaoMoveAttackBoss.Mode.Homing);
					SetSpinModelControl(false);
					SetAnimationFlagForAbility(true);
					m_substate = 1;
				}
				break;
			case 1:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					ChangeState(new TinyFsmState(StatePursue));
					return TinyFsmState.End();
				}
				break;
			case 2:
				m_stateTimer -= deltaTime;
				if (m_stateTimer <= 0f)
				{
					ChangeState(new TinyFsmState(StatePursue));
					return TinyFsmState.End();
				}
				break;
			}
			return TinyFsmState.End();
		}

		private bool AttackBossCheckAndChanegStateOnDamageBossSucceed(TinyFsmEvent fsmEvent, AttackBossWork work)
		{
			MsgHitDamageSucceed msgHitDamageSucceed = fsmEvent.GetMessage as MsgHitDamageSucceed;
			if (msgHitDamageSucceed != null && work != null && msgHitDamageSucceed.m_sender == work.m_targetObject)
			{
				if (work != null && m_substate < 2)
				{
					ObjUtil.PlayChaoEffect("ef_ch_slash_sr01", msgHitDamageSucceed.m_position, 3f);
					ObjUtil.LightPlaySE("act_sword_attack");
					m_stateTimer = 0.5f;
					m_substate = 2;
					SetChaoMoveAttackBoss(ChaoMoveAttackBoss.Mode.AfterAttack);
					work.DestroyAttackCollision();
					SetSpinModelControl(true);
					m_attackCount++;
				}
				else
				{
					ChangeState(new TinyFsmState(StatePursue));
				}
				return true;
			}
			return false;
		}

		private bool PursuesAttackBossCheckAndChanegStateOnDamageBossSucceed(TinyFsmEvent fsmEvent, AttackBossWork work)
		{
			MsgHitDamageSucceed msgHitDamageSucceed = fsmEvent.GetMessage as MsgHitDamageSucceed;
			if (msgHitDamageSucceed != null && work != null && msgHitDamageSucceed.m_sender == work.m_targetObject)
			{
				if (work != null && m_substate < 2)
				{
					ObjUtil.PlayChaoEffect("ef_ch_raid1_moon_atk_sr01", msgHitDamageSucceed.m_position, 3f);
					ObjUtil.LightPlaySE("act_sword_attack");
					m_stateTimer = 0.5f;
					m_substate = 2;
					SetAnimationFlagForAbility(false);
					SetChaoMoveAttackBoss(ChaoMoveAttackBoss.Mode.AfterAttack);
					work.DestroyAttackCollision();
					SetSpinModelControl(true);
				}
				else
				{
					ChangeState(new TinyFsmState(StatePursue));
				}
				return true;
			}
			return false;
		}

		private void CreateAttackBossWork()
		{
			MsgBossInfo msgBossInfo = new MsgBossInfo();
			GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgBossInfo", msgBossInfo, SendMessageOptions.DontRequireReceiver);
			AttackBossWork attackBossWork = CreateStateWork<AttackBossWork>();
			if (msgBossInfo.m_succeed)
			{
				ChaoMoveAttackBoss currentMovement = GetCurrentMovement<ChaoMoveAttackBoss>();
				attackBossWork.m_targetObject = msgBossInfo.m_boss;
				if (currentMovement != null)
				{
					currentMovement.SetTarget(msgBossInfo.m_boss);
				}
			}
			attackBossWork.m_attackCollision = ChaoPartsAttackEnemy.Create(base.gameObject);
		}

		private void SetChaoMoveAttackBoss(ChaoMoveAttackBoss.Mode mode)
		{
			ChaoMoveAttackBoss currentMovement = GetCurrentMovement<ChaoMoveAttackBoss>();
			if (currentMovement != null)
			{
				currentMovement.ChangeMode(mode);
			}
		}

		private void SetSpinModelControl(bool flag)
		{
			if (m_modelControl != null)
			{
				if (flag)
				{
					m_modelControl.ChangeStateToSpin(AttackBossModelSpinSpeed);
				}
				else
				{
					m_modelControl.ChangeStateToReturnIdle();
				}
			}
		}

		private TinyFsmState StateRingMagnet(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ChaoWalkerOffsetRate, 2f);
				}
				m_chaoWalkerState = ChaoWalkerState.Action;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				return TinyFsmState.End();
			case 0:
				switch (m_chaoWalkerState)
				{
				case ChaoWalkerState.Action:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_effect");
						SetEnableMagnetComponet(ChaoAbility.CHAO_RING_MAGNET);
						m_stateTimer = 1f;
						m_chaoWalkerState = ChaoWalkerState.Wait;
					}
					break;
				case ChaoWalkerState.Wait:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateCrystalMagnet(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ChaoWalkerOffsetRate, 2f);
				}
				m_chaoWalkerState = ChaoWalkerState.Action;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				return TinyFsmState.End();
			case 0:
				switch (m_chaoWalkerState)
				{
				case ChaoWalkerState.Action:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_effect");
						SetEnableMagnetComponet(ChaoAbility.CHAO_CRYSTAL_MAGNET);
						m_stateTimer = 1f;
						m_chaoWalkerState = ChaoWalkerState.Wait;
					}
					break;
				case ChaoWalkerState.Wait:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private void SetEnableMagnetComponet(ChaoAbility ability)
		{
			float enable = 4f;
			if (StageAbilityManager.Instance != null)
			{
				enable = StageAbilityManager.Instance.GetChaoAbilityValue(ability);
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				ChaoPartsObjectMagnet component = child.GetComponent<ChaoPartsObjectMagnet>();
				if (component != null)
				{
					component.SetEnable(enable);
					break;
				}
			}
		}

		private void SetMagnetPause(bool flag)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				ChaoPartsObjectMagnet component = child.GetComponent<ChaoPartsObjectMagnet>();
				if (component != null)
				{
					component.SetPause(flag);
					break;
				}
			}
		}

		private TinyFsmState StateGameHardware(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(HardwareCartridgeOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.8f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_megadrive");
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_time_combotime_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateGameCartridge(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(HardwareCartridgeOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.8f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (!(m_stateTimer < 0f))
					{
						break;
					}
					if (StageAbilityManager.Instance != null)
					{
						int addCombo = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.ADD_COMBO_VALUE);
						if (StageComboManager.Instance != null)
						{
							StageComboManager.Instance.AddComboForChaoAbilityValue(addCombo);
						}
					}
					ObjUtil.LightPlaySE("act_chao_cartridge");
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_up_combocount_r01", -1f);
					StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
					m_stateTimer = 2f;
					m_substate = 1;
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateNights(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(NightsOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 1.5f;
				StageItemManager itemManager = GetItemManager();
				if (itemManager != null)
				{
					itemManager.SendMessage("OnChaoAbilityLoopComboUp", null, SendMessageOptions.DontRequireReceiver);
				}
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_nights");
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_clb_nights_magic_sr01", -1f);
						m_stateTimer = 2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateReala(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(RealaOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 1.5f;
				StageItemManager itemManager = GetItemManager();
				if (itemManager != null)
				{
					itemManager.SendMessage("OnChaoAbilityLoopMagnet", null, SendMessageOptions.DontRequireReceiver);
				}
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_reala");
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_clb_reala_magic_sr01", -1f);
						m_stateTimer = 2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private void SetItemBtnObjAndUICamera()
		{
			if (m_uiCamera != null && m_itemBtnObj != null)
			{
				return;
			}
			GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
			if (cameraUIObject != null)
			{
				m_uiCamera = cameraUIObject.GetComponent<Camera>();
				GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "HudCockpit");
				if (gameObject != null)
				{
					m_itemBtnObj = GameObjectUtil.FindChildGameObject(gameObject, "HUD_btn_item");
				}
			}
		}

		private void SetTargetScreenPos()
		{
			if (m_uiCamera != null && m_itemBtnObj != null)
			{
				m_targetScreenPos = m_uiCamera.WorldToScreenPoint(m_itemBtnObj.transform.position);
			}
		}

		private void SetPresentEquipItemPos()
		{
			if (Camera.main != null && m_uiCamera != null && (bool)m_itemBtnObj)
			{
				Vector3 position = Camera.main.WorldToScreenPoint(base.transform.position);
				Vector3 vector = m_uiCamera.WorldToScreenPoint(m_itemBtnObj.transform.position);
				position.x = vector.x;
				position.y = vector.y;
				Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
				position2.z = 0f;
				m_effectObj.transform.position = position2;
			}
		}

		private TinyFsmState StateItemPresent(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
					StageItemManager itemManager2 = GetItemManager();
					if (CheckItemPresent(m_chaoItemType) && itemManager2 != null)
					{
						MsgAddItemToManager value3 = new MsgAddItemToManager(m_chaoItemType);
						itemManager2.SendMessage("OnAddItem", value3, SendMessageOptions.DontRequireReceiver);
					}
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
				{
					m_stateTimer -= getDeltaTime;
					if (!(m_stateTimer < 0f))
					{
						break;
					}
					bool flag = false;
					PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation != null)
					{
						if (playerInformation.PhantomType == PhantomType.NONE)
						{
							m_chaoItemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
						if (!flag)
						{
							m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
					}
					if (flag)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							ItemType chaoItemType = m_chaoItemType;
							MsgAddItemToManager value = new MsgAddItemToManager(chaoItemType);
							itemManager.SendMessage("OnAddItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("obj_itembox");
							ObjUtil.LightPlaySE("act_sharla_magic");
							ObjUtil.SendGetItemIcon(chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_magic_item_st_sr01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
							MsgAbilityEffectStart value2 = new MsgAbilityEffectStart(ChaoAbility.COMBO_ITEM_BOX, "ef_ch_magic_item_ht_sr01", false, true);
							GameObjectUtil.SendDelayedMessageToTagObjects("Player", "OnMsgAbilityEffectStart", value2);
						}
						m_presentFlag = true;
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					else if (m_stateFlag.Test(1))
					{
						ChangeState(new TinyFsmState(StateOutItemPhantom));
					}
					else
					{
						ChangeState(new TinyFsmState(StatePursue));
					}
					break;
				}
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateItemPresent2(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
					StageItemManager itemManager2 = GetItemManager();
					if (CheckItemPresent(m_chaoItemType) && itemManager2 != null)
					{
						MsgAddItemToManager value3 = new MsgAddItemToManager(m_chaoItemType);
						itemManager2.SendMessage("OnAddItem", value3, SendMessageOptions.DontRequireReceiver);
					}
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
				{
					m_stateTimer -= getDeltaTime;
					if (!(m_stateTimer < 0f))
					{
						break;
					}
					bool flag = false;
					PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation != null)
					{
						if (playerInformation.PhantomType == PhantomType.NONE)
						{
							m_chaoItemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
						if (!flag)
						{
							m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
					}
					if (flag)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("obj_itembox");
							ObjUtil.LightPlaySE("act_chao_quna");
							ObjUtil.SendGetItemIcon(m_chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_clb_quna_item_st_sr01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
							MsgAbilityEffectStart value2 = new MsgAbilityEffectStart(ChaoAbility.CHECK_POINT_ITEM_BOX, "ef_ch_clb_quna_item_ht_sr01", false, true);
							GameObjectUtil.SendDelayedMessageToTagObjects("Player", "OnMsgAbilityEffectStart", value2);
						}
						m_stateTimer = 1.2f;
						m_substate = 1;
						m_presentFlag = true;
					}
					else if (m_stateFlag.Test(1))
					{
						ChangeState(new TinyFsmState(StateOutItemPhantom));
					}
					else
					{
						ChangeState(new TinyFsmState(StatePursue));
					}
					break;
				}
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePhantomPresent(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
				if (playerInformation != null)
				{
					switch (playerInformation.PhantomType)
					{
					case PhantomType.NONE:
					{
						StageItemManager itemManager2 = GetItemManager();
						if (itemManager2 != null)
						{
							m_chaoItemType = itemManager2.GetPhantomItemType();
						}
						if (m_chaoItemType == ItemType.UNKNOWN)
						{
							m_chaoItemType = ChaoAbilityPhantomTbl[Random.Range(0, ChaoAbilityPhantomTbl.Length)];
						}
						break;
					}
					case PhantomType.LASER:
						m_chaoItemType = ItemType.LASER;
						break;
					case PhantomType.DRILL:
						m_chaoItemType = ItemType.DRILL;
						break;
					case PhantomType.ASTEROID:
						m_chaoItemType = ItemType.ASTEROID;
						break;
					}
				}
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					StageItemManager itemManager3 = GetItemManager();
					if (itemManager3 != null)
					{
						MsgAddItemToManager value2 = new MsgAddItemToManager(m_chaoItemType);
						itemManager3.SendMessage("OnAddColorItem", value2, SendMessageOptions.DontRequireReceiver);
					}
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddColorItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("obj_itembox");
							ObjUtil.LightPlaySE("act_sharla_magic");
							ObjUtil.SendGetItemIcon(m_chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_sp1_merlina_magic_sr01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_presentFlag = true;
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateReviveEquipItem(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				m_stateFlag.Set(2, true);
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ChaosOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 1f;
				SetItemBtnObjAndUICamera();
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				m_stateFlag.Set(2, false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_chaosadv");
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_itemrecycle_sr01", -1f);
						ObjUtil.PlayChaoEffectForHUD(m_itemBtnObj, "ef_ch_itemrecycle_ht_sr01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 2.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePresentSRareEquipItem(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				SetItemBtnObjAndUICamera();
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					StageItemManager itemManager = GetItemManager();
					if (itemManager != null)
					{
						itemManager.SendMessage("OnAddEquipItem", null, SendMessageOptions.DontRequireReceiver);
						m_presentFlag = true;
					}
				}
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_sharla_magic");
						m_effectObj = ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_magic_darkqueen_st_sr01", -1f);
						ObjUtil.PlayChaoEffectForHUD(m_itemBtnObj, "ef_ch_magic_darkqueen_ht_sr01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePresentEquipItem(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(PufferFishOffsetRate, 2.5f);
				}
				m_substate = 0;
				m_stateTimer = 1f;
				SetItemBtnObjAndUICamera();
				SetTargetScreenPos();
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				m_effectObj = null;
				if (!m_presentFlag)
				{
					StageItemManager itemManager2 = GetItemManager();
					if (itemManager2 != null)
					{
						itemManager2.SendMessage("OnAddEquipItem", null, SendMessageOptions.DontRequireReceiver);
						m_presentFlag = true;
					}
				}
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("act_chao_effect");
						m_effectObj = ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_getitem_r01", 2.5f);
						if (m_effectObj != null && Camera.main != null)
						{
							m_effectScreenPos = Camera.main.WorldToScreenPoint(m_effectObj.transform.position);
							m_targetScreenPos.z = m_effectScreenPos.z;
						}
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 0.5f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						m_distance = Vector3.Distance(m_effectScreenPos, m_targetScreenPos);
						m_stateTimer = 1f;
						m_substate = 2;
					}
					break;
				case 2:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_effectObj != null)
					{
						m_effectScreenPos = Vector3.MoveTowards(m_effectScreenPos, m_targetScreenPos, m_distance * Time.deltaTime);
						if (Camera.main != null)
						{
							Vector3 position = Camera.main.ScreenToWorldPoint(m_effectScreenPos);
							position.z = -1.5f;
							m_effectObj.transform.position = position;
						}
					}
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							itemManager.SendMessage("OnAddEquipItem", null, SendMessageOptions.DontRequireReceiver);
						}
						m_presentFlag = true;
						m_stateTimer = 0.5f;
						m_substate = 3;
					}
					break;
				case 3:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePhantomPresentAsteroid(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				m_chaoItemType = ItemType.ASTEROID;
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddColorItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.SendGetItemIcon(m_chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_asteroid_r01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePhantomPresentDrill(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				m_chaoItemType = ItemType.DRILL;
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddColorItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.SendGetItemIcon(m_chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_drill_r01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StatePhantomPresentLaser(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				m_chaoItemType = ItemType.LASER;
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							MsgAddItemToManager value = new MsgAddItemToManager(m_chaoItemType);
							itemManager.SendMessage("OnAddColorItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.SendGetItemIcon(m_chaoItemType);
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_laser_r01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_stateTimer = 1.2f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateChangeEquipItem(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				SetItemBtnObjAndUICamera();
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					StageItemManager itemManager2 = GetItemManager();
					if (itemManager2 != null)
					{
						itemManager2.SendMessage("OnChangeItem", null, SendMessageOptions.DontRequireReceiver);
					}
					m_presentFlag = true;
				}
				return TinyFsmState.End();
			case 0:
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						StageItemManager itemManager = GetItemManager();
						if (itemManager != null)
						{
							itemManager.SendMessage("OnChangeItem", null, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_changeitem_r01", -1f);
							ObjUtil.PlayChaoEffectForHUD(m_itemBtnObj, "ef_ch_combo_changeitem_ht_r01", -1f);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_stateTimer = 1f;
						m_substate = 1;
						m_presentFlag = true;
					}
					break;
				case 1:
					m_stateTimer -= fsmEvent.GetDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateOrbotItemPresent(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(ItemPresentOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
					if (CheckItemPresent(m_chaoItemType) && StageItemManager.Instance != null)
					{
						StageItemManager.Instance.SendMessage("OnAddItem", new MsgAddItemToManager(m_chaoItemType), SendMessageOptions.DontRequireReceiver);
					}
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
				{
					m_stateTimer -= getDeltaTime;
					if (!(m_stateTimer < 0f))
					{
						break;
					}
					bool flag = false;
					PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation != null)
					{
						if (playerInformation.PhantomType == PhantomType.NONE)
						{
							m_chaoItemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
						if (!flag)
						{
							m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
					}
					if (flag)
					{
						if (StageItemManager.Instance != null)
						{
							ItemType chaoItemType = m_chaoItemType;
							MsgAddItemToManager value = new MsgAddItemToManager(chaoItemType);
							StageItemManager.Instance.SendMessage("OnAddItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("obj_itembox");
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_randombuyitem_r01", -1f);
							ObjUtil.SendGetItemIcon(chaoItemType);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_presentFlag = true;
						m_stateTimer = 0.8f;
						m_substate = 2;
					}
					else if (m_stateFlag.Test(1))
					{
						ChangeState(new TinyFsmState(StateOutItemPhantom));
					}
					else
					{
						ChangeState(new TinyFsmState(StatePursue));
					}
					break;
				}
				case 2:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						m_stateTimer += 1f;
						ObjUtil.LightPlaySE("act_ringspread");
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateCuebotItemPresent(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(CuebotItemOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				m_presentFlag = false;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				if (!m_presentFlag)
				{
					m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
					if (CheckItemPresent(m_chaoItemType) && StageItemManager.Instance != null)
					{
						StageItemManager.Instance.SendMessage("OnAddItem", new MsgAddItemToManager(m_chaoItemType), SendMessageOptions.DontRequireReceiver);
					}
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
				{
					m_stateTimer -= getDeltaTime;
					if (!(m_stateTimer < 0f))
					{
						break;
					}
					bool flag = false;
					PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
					if (playerInformation != null)
					{
						if (playerInformation.PhantomType == PhantomType.NONE)
						{
							m_chaoItemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
						if (!flag)
						{
							m_chaoItemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
							flag = CheckItemPresent(m_chaoItemType);
						}
					}
					if (flag)
					{
						if (StageItemManager.Instance != null)
						{
							ItemType chaoItemType = m_chaoItemType;
							MsgAddItemToManager value = new MsgAddItemToManager(chaoItemType);
							StageItemManager.Instance.SendMessage("OnAddItem", value, SendMessageOptions.DontRequireReceiver);
							ObjUtil.LightPlaySE("obj_itembox");
							ObjUtil.LightPlaySE("act_chao_effect");
							ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_randomitem_r01", -1f);
							ObjUtil.SendGetItemIcon(chaoItemType);
							StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						}
						m_presentFlag = true;
						m_stateTimer = 1.8f;
						m_substate = 1;
					}
					else if (m_stateFlag.Test(1))
					{
						ChangeState(new TinyFsmState(StateOutItemPhantom));
					}
					else
					{
						ChangeState(new TinyFsmState(StatePursue));
					}
					break;
				}
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateFailCuebotItemPresent(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
			{
				ChangeMovement(MOVESTATE_ID.GoCameraTarget);
				ChaoMoveGoCameraTarget currentMovement = GetCurrentMovement<ChaoMoveGoCameraTarget>();
				if (currentMovement != null)
				{
					currentMovement.SetParameter(CuebotItemOffsetRate, 2f);
				}
				m_substate = 0;
				m_stateTimer = 0.6f;
				return TinyFsmState.End();
			}
			case -4:
				SetAnimationFlagForAbility(false);
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				switch (m_substate)
				{
				case 0:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						ObjUtil.LightPlaySE("sys_error");
						ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_randomitem_fail_r01", -1f);
						StartCoroutine(SetAnimationFlagForAbilityCourutine(true));
						m_stateTimer = 1.8f;
						m_substate = 1;
					}
					break;
				case 1:
					m_stateTimer -= getDeltaTime;
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateOutItemPhantom));
						}
						else
						{
							ChangeState(new TinyFsmState(StatePursue));
						}
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private ItemType GetOrbotCuebotItem()
		{
			ItemType itemType = ItemType.BARRIER;
			PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponentWithTag<PlayerInformation>("StageManager", "PlayerInformation");
			if (playerInformation != null)
			{
				bool flag = false;
				if (playerInformation.PhantomType == PhantomType.NONE)
				{
					itemType = ChaoAbilityItemTbl[Random.Range(0, ChaoAbilityItemTbl.Length)];
					flag = CheckItemPresent(itemType);
				}
				if (!flag)
				{
					itemType = ChaoAbilityItemTblPhantom[Random.Range(0, ChaoAbilityItemTblPhantom.Length)];
				}
			}
			return itemType;
		}

		private TinyFsmState StateStockRing(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				m_stateTimer = 2f;
				m_stateFlag.Reset();
				m_stockRingState = StockRingState.CHANGE_MOVEMENT;
				return TinyFsmState.End();
			case -4:
			{
				if (m_modelControl != null)
				{
					m_modelControl.ChangeStateToReturnIdle();
				}
				GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ef_ch_tornade_fog_(Clone)");
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
				return TinyFsmState.End();
			}
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				m_stateTimer -= getDeltaTime;
				switch (m_stockRingState)
				{
				case StockRingState.CHANGE_MOVEMENT:
					ChangeMovement(MOVESTATE_ID.GoRingBanking);
					m_stockRingState = StockRingState.PLAY_SPIN_MOTION;
					break;
				case StockRingState.PLAY_SPIN_MOTION:
					if (m_modelControl != null)
					{
						m_modelControl.ChangeStateToSpin(StockRingModelSpinSpeed);
					}
					m_stockRingState = StockRingState.PLAY_EFECT;
					break;
				case StockRingState.PLAY_EFECT:
				{
					string text = "ef_ch_tornade_fog_";
					switch (GetRarity(m_chao_id))
					{
					case ChaoData.Rarity.NORMAL:
						text += "c01";
						break;
					case ChaoData.Rarity.RARE:
						text += "r01";
						break;
					case ChaoData.Rarity.SRARE:
						text += "sr01";
						ObjUtil.LightPlaySE("act_tornado_fly");
						break;
					}
					ObjUtil.PlayEffectChild(base.gameObject, text, Vector3.zero, Quaternion.identity);
					m_stockRingState = StockRingState.PLAY_EFFECT;
					break;
				}
				case StockRingState.PLAY_EFFECT:
					if (m_stateTimer < 1f && StageAbilityManager.Instance != null)
					{
						StageAbilityManager.Instance.RequestPlayChaoEffect(ChaoAbility.RECOVERY_RING, m_chao_type);
						m_stockRingState = StockRingState.WAIT_END;
					}
					break;
				case StockRingState.WAIT_END:
					if (m_stateTimer < 0f)
					{
						ChangeState(new TinyFsmState(StateComeIn));
						m_stockRingState = StockRingState.IDLE;
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private TinyFsmState StateDoubleRing(TinyFsmEvent fsmEvent)
		{
			switch (fsmEvent.Signal)
			{
			case -3:
				m_stateTimer = 2.5f;
				m_stockRingState = StockRingState.CHANGE_MOVEMENT;
				return TinyFsmState.End();
			case -4:
				if (m_modelControl != null)
				{
					m_modelControl.ChangeStateToReturnIdle();
				}
				return TinyFsmState.End();
			case 0:
			{
				float getDeltaTime = fsmEvent.GetDeltaTime;
				m_stateTimer -= getDeltaTime;
				switch (m_stockRingState)
				{
				case StockRingState.CHANGE_MOVEMENT:
					ChangeMovement(MOVESTATE_ID.GoRingBanking);
					m_stockRingState = StockRingState.PLAY_SPIN_MOTION;
					break;
				case StockRingState.PLAY_SPIN_MOTION:
					if (m_modelControl != null)
					{
						m_modelControl.ChangeStateToSpin(StockRingModelSpinSpeed);
					}
					m_stockRingState = StockRingState.PLAY_EFECT;
					break;
				case StockRingState.PLAY_EFECT:
					ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_up_bank_sr01", -1f);
					ObjUtil.LightPlaySE("act_chao_tornado2");
					m_stockRingState = StockRingState.WAIT_END;
					break;
				case StockRingState.WAIT_END:
					if (m_stateTimer < 0f)
					{
						if (m_stateFlag.Test(1))
						{
							ChangeState(new TinyFsmState(StateStop));
						}
						else
						{
							ChangeState(new TinyFsmState(StateComeIn));
						}
						m_stockRingState = StockRingState.IDLE;
					}
					break;
				}
				return TinyFsmState.End();
			}
			case 1:
				SetMessageComeInOut(fsmEvent);
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		}

		private IEnumerator RingBank()
		{
			ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_combo_bank_r01", -1f);
			ObjUtil.LightPlaySE("act_chao_effect");
			int waite_frame2 = 1;
			while (waite_frame2 > 0)
			{
				waite_frame2--;
				yield return null;
			}
			int stockRing = 0;
			if (StageAbilityManager.Instance != null)
			{
				stockRing = (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.COMBO_RING_BANK);
				if (StageScoreManager.Instance != null)
				{
					StageScoreManager.Instance.TransferRingForChaoAbility(stockRing);
				}
			}
			waite_frame2 = 2;
			while (waite_frame2 > 0)
			{
				waite_frame2--;
				yield return null;
			}
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnAddStockRing", new MsgAddStockRing(stockRing), SendMessageOptions.DontRequireReceiver);
		}
	}
}
