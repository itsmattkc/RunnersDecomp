namespace Boss
{
	public class ObjBossEggmanState : ObjBossState
	{
		private FSMSystem<ObjBossEggmanState> m_fsm;

		private STATE_ID m_initState = STATE_ID.AppearFever;

		private STATE_ID m_damageState = STATE_ID.DamageFever;

		private ObjBossEggmanParameter m_bossParam;

		private ObjBossEggmanEffect m_bossEffect;

		private ObjBossEggmanMotion m_bossMotion;

		public ObjBossEggmanParameter BossParam
		{
			get
			{
				return m_bossParam;
			}
		}

		public ObjBossEggmanEffect BossEffect
		{
			get
			{
				return m_bossEffect;
			}
		}

		public ObjBossEggmanMotion BossMotion
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
		}

		private ObjBossEggmanParameter GetBossParam()
		{
			return GetComponent<ObjBossEggmanParameter>();
		}

		private ObjBossEggmanEffect GetBossEffect()
		{
			return GetComponent<ObjBossEggmanEffect>();
		}

		private ObjBossEggmanMotion GetBossMotion()
		{
			return GetComponent<ObjBossEggmanMotion>();
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
		}

		protected override void OnChangeDamageState()
		{
			ChangeState(m_damageState);
		}

		private void MakeFSM()
		{
			FSMState<ObjBossEggmanState>[] array = new FSMState<ObjBossEggmanState>[19]
			{
				new BossStateAppearFever(),
				new BossStateAppearMap1(),
				new BossStateAppearMap2(),
				new BossStateAppearMap2_2(),
				new BossStateAppearMap3(),
				new BossStateAttackFever(),
				new BossStateAttackMap1(),
				new BossStateAttackMap2(),
				new BossStateAttackMap3(),
				new BossStateDamageFever(),
				new BossStateDamageMap1(),
				new BossStateDamageMap2(),
				new BossStateDamageMap3(),
				new BossStatePassFever(),
				new BossStatePassFeverDistanceEnd(),
				new BossStatePassMap(),
				new BossStatePassMapDistanceEnd(),
				new BossStateDeadFever(),
				new BossStateDeadMap()
			};
			m_fsm = new FSMSystem<ObjBossEggmanState>();
			int num = 0;
			FSMState<ObjBossEggmanState>[] array2 = array;
			foreach (FSMState<ObjBossEggmanState> s in array2)
			{
				m_fsm.AddState(1 + num, s);
				num++;
			}
			SetSpeed(0f);
			m_fsm.Init(this, (int)m_initState);
		}

		public void ChangeState(STATE_ID state)
		{
			m_fsm.ChangeState(this, (int)state);
		}

		public void SetInitState(STATE_ID state)
		{
			m_initState = state;
		}

		public void SetDamageState(STATE_ID state)
		{
			m_damageState = state;
		}
	}
}
