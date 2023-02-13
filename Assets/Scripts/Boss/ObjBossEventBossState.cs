using Message;
using UnityEngine;

namespace Boss
{
	public class ObjBossEventBossState : ObjBossState
	{
		private const float WISPSTART_TIME = 0f;

		private const float WISP_POSY_MIN = 0.5f;

		private const float WISP_POSY_MAX = 3f;

		private const float WISP_POSX = 15f;

		private bool m_damageWispCancel = true;

		private FSMSystem<ObjBossEventBossState> m_fsm;

		private EVENTBOSS_STATE_ID m_initState = EVENTBOSS_STATE_ID.AppearEvent1;

		private EVENTBOSS_STATE_ID m_damageState = EVENTBOSS_STATE_ID.DamageEvent1;

		private ObjBossEventBossParameter m_bossParam;

		private ObjBossEventBossEffect m_bossEffect;

		private ObjBossEventBossMotion m_bossMotion;

		private float m_wispTime;

		private float m_wispTimeMax;

		private WispBoostLevel m_currentBoostLevel = WispBoostLevel.NONE;

		private static readonly string[] BOOST_SE_NAME = new string[3]
		{
			"rb_boost_1",
			"rb_boost_2",
			"rb_boost_3"
		};

		public ObjBossEventBossParameter BossParam
		{
			get
			{
				return m_bossParam;
			}
		}

		public ObjBossEventBossEffect BossEffect
		{
			get
			{
				return m_bossEffect;
			}
		}

		public ObjBossEventBossMotion BossMotion
		{
			get
			{
				return m_bossMotion;
			}
		}

		protected override void OnStart()
		{
			OnInit();
		}

		protected override void OnInit()
		{
			if (m_bossParam == null)
			{
				m_bossParam = GetBossParam();
			}
			if (m_bossEffect == null)
			{
				m_bossEffect = GetBossEffect();
			}
			if (m_bossMotion == null)
			{
				m_bossMotion = GetBossMotion();
			}
			m_wispTime = 0f;
			m_wispTimeMax = 0f;
			m_currentBoostLevel = WispBoostLevel.NONE;
		}

		private ObjBossEventBossParameter GetBossParam()
		{
			return GetComponent<ObjBossEventBossParameter>();
		}

		private ObjBossEventBossEffect GetBossEffect()
		{
			return GetComponent<ObjBossEventBossEffect>();
		}

		private ObjBossEventBossMotion GetBossMotion()
		{
			return GetComponent<ObjBossEventBossMotion>();
		}

		protected override ObjBossParameter OnGetBossParam()
		{
			return GetBossParam();
		}

		protected override ObjBossEffect OnGetBossEffect()
		{
			return GetBossEffect();
		}

		protected override ObjBossMotion OnGetBossMotion()
		{
			return GetBossMotion();
		}

		protected override void OnChangeChara()
		{
			ResetWisp();
		}

		protected override void OnSetup()
		{
			m_bossParam.Setup();
			m_bossMotion.Setup();
			MakeFSM();
		}

		private void OnDestroy()
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.Leave(this);
			}
		}

		protected override void OnFsmUpdate(float delta)
		{
			if (m_fsm != null && m_fsm.CurrentState != null)
			{
				m_fsm.CurrentState.Step(this, delta);
			}
			UpdateWisp(delta);
		}

		protected override void OnChangeDamageState()
		{
			ChangeState(m_damageState);
		}

		private void MakeFSM()
		{
			FSMState<ObjBossEventBossState>[] array = new FSMState<ObjBossEventBossState>[11]
			{
				new BossStateAppearEvent1(),
				new BossStateAppearEvent2(),
				new BossStateAppearEvent1_2(),
				new BossStateAppearEvent2_2(),
				new BossStateAttackEvent1(),
				new BossStateAttackEvent2(),
				new BossStateDamageEvent1(),
				new BossStateDamageEvent2(),
				new BossStatePassEvent(),
				new BossStatePassEventDistanceEnd(),
				new BossStateDeadEvent()
			};
			m_fsm = new FSMSystem<ObjBossEventBossState>();
			int num = 0;
			FSMState<ObjBossEventBossState>[] array2 = array;
			foreach (FSMState<ObjBossEventBossState> s in array2)
			{
				m_fsm.AddState(1 + num, s);
				num++;
			}
			SetSpeed(0f);
			m_fsm.Init(this, (int)m_initState);
		}

		public void ChangeState(EVENTBOSS_STATE_ID state)
		{
			m_fsm.ChangeState(this, (int)state);
		}

		public void SetInitState(EVENTBOSS_STATE_ID state)
		{
			m_initState = state;
		}

		public void SetDamageState(EVENTBOSS_STATE_ID state)
		{
			m_damageState = state;
		}

		private void OnGetWisp()
		{
			int num = (int)m_bossParam.BoostLevel;
			float num2 = m_bossParam.BoostRatio + m_bossParam.BoostRatioAdd;
			if ((double)num2 >= 1.0)
			{
				num2 = 1f;
				if (num < 2)
				{
					num++;
				}
			}
			SetBoostLevel((WispBoostLevel)num, num2);
		}

		private void ResetWisp()
		{
			if (m_bossParam.BoostRatio > 0f)
			{
				m_bossParam.BoostRatio = 0f;
				m_bossParam.BoostLevel = WispBoostLevel.NONE;
				SetBoostLevel(m_bossParam.BoostLevel, m_bossParam.BoostRatio);
			}
		}

		public int GetDropRingAggressivity()
		{
			int num = 1;
			if (base.ColorPowerHit || base.ChaoHit)
			{
				return 1;
			}
			return ObjUtil.GetChaoAbliltyValue(ChaoAbility.AGGRESSIVITY_UP_FOR_RAID_BOSS, (int)m_bossParam.ChallengeValue);
		}

		private void BoostMeter()
		{
			MsgCaution caution = new MsgCaution(HudCaution.Type.WISPBOOST, m_bossParam);
			HudCaution.Instance.SetCaution(caution);
		}

		private void UpdateWisp(float delta)
		{
			m_wispTime += delta;
			if (m_wispTime > m_wispTimeMax)
			{
				m_wispTime = 0f;
				m_wispTimeMax = m_bossParam.WispInterspace;
				float y = Random.Range(0.5f, 3f);
				Vector3 playerPosition = GetPlayerPosition();
				float x = playerPosition.x + 15f;
				Vector3 position = base.transform.position;
				Vector3 pos = new Vector3(x, y, position.z);
				CreateWisp(pos);
			}
			if (m_bossParam.BoostRatio > 0f)
			{
				m_bossParam.BoostRatio -= delta * m_bossParam.BoostRatioDown;
				if (m_bossParam.BoostRatio <= 0f)
				{
					m_bossParam.BoostRatio = 0f;
					m_bossParam.BoostLevel = WispBoostLevel.NONE;
					SetBoostLevel(m_bossParam.BoostLevel, m_bossParam.BoostRatio);
				}
			}
		}

		private void CreateWisp(Vector3 pos)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "ObjBossWisp");
			if (!(gameObject != null))
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, pos, Quaternion.identity) as GameObject;
			if (!gameObject2)
			{
				return;
			}
			gameObject2.gameObject.SetActive(true);
			ObjBossWisp component = gameObject2.GetComponent<ObjBossWisp>();
			if ((bool)component)
			{
				float speed = Random.Range(m_bossParam.WispSpeedMin, m_bossParam.WispSpeedMax);
				float num = Random.Range(m_bossParam.WispSwingMin, m_bossParam.WispSwingMax);
				float addX = Random.Range(m_bossParam.WispAddXMin, m_bossParam.WispAddXMax);
				float num2 = pos.y - num;
				if (num2 < 0f)
				{
					num = pos.y;
				}
				component.Setup(base.gameObject, speed, num, addX);
			}
		}

		private void SetBoostLevel(WispBoostLevel level, float ratio)
		{
			bool flag = false;
			if (m_currentBoostLevel != level)
			{
				flag = true;
				m_currentBoostLevel = level;
			}
			m_bossParam.BoostLevel = level;
			m_bossParam.BoostRatio = ratio;
			if (flag)
			{
				if (level == WispBoostLevel.NONE)
				{
					m_bossParam.BossAttackPower = 1;
				}
				else
				{
					m_bossParam.BossAttackPower = m_bossParam.GetBoostAttackParam(level);
					ObjUtil.PlayEventSE(GetBoostSE(level), EventManager.EventType.RAID_BOSS);
				}
				BoostMeter();
				SendPlayerBoostLevel(level);
			}
		}

		private void SendPlayerBoostLevel(WispBoostLevel level)
		{
			string boostEffect = ObjBossEventBossEffect.GetBoostEffect(level);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnBossBoostLevel", new MsgBossBoostLevel(level, boostEffect), SendMessageOptions.DontRequireReceiver);
		}

		private static string GetBoostSE(WispBoostLevel level)
		{
			if ((uint)level < BOOST_SE_NAME.Length)
			{
				return BOOST_SE_NAME[(int)level];
			}
			return string.Empty;
		}

		private void OnPlayerDamage(MsgBossPlayerDamage msg)
		{
			bool flag = m_damageWispCancel;
			if (msg.m_dead)
			{
				flag = true;
			}
			if (flag && m_bossParam.BoostLevel != WispBoostLevel.NONE)
			{
				m_bossParam.BoostRatio = 0f;
				m_bossParam.BoostLevel = WispBoostLevel.NONE;
				SetBoostLevel(m_bossParam.BoostLevel, m_bossParam.BoostRatio);
			}
		}
	}
}
