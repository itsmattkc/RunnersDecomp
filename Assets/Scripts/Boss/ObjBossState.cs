using Message;
using UnityEngine;

namespace Boss
{
	public class ObjBossState : MonoBehaviour
	{
		public bool m_debugDrawState;

		public bool m_debugDrawInfo;

		protected PlayerInformation m_playerInfo;

		protected LevelInformation m_levelInfo;

		private ObjBossParameter m_param;

		private ObjBossEffect m_effect;

		private ObjBossMotion m_motion;

		private bool m_hitCheck;

		private bool m_colorPowerHit;

		private bool m_chaoHit;

		private bool m_speedKeep;

		private float m_keepDistance;

		private bool m_bossDistanceEnd;

		private bool m_bossDistanceEndArea;

		private bool m_phantom;

		private bool m_hitBumper;

		private bool m_playerDead;

		private bool m_playerChange;

		private Vector3 m_prevPlayerPos = Vector3.zero;

		private float m_moveStep;

		private float m_moveAddStep;

		private bool m_clear;

		public bool ColorPowerHit
		{
			get
			{
				return m_colorPowerHit;
			}
			set
			{
				m_colorPowerHit = value;
			}
		}

		public bool ChaoHit
		{
			get
			{
				return m_chaoHit;
			}
			set
			{
				m_chaoHit = value;
			}
		}

		private void Start()
		{
			SetBossStateAttackOK(false);
			OnStart();
		}

		protected virtual void OnStart()
		{
		}

		public void Init()
		{
			if (m_param == null)
			{
				m_param = OnGetBossParam();
			}
			if (m_effect == null)
			{
				m_effect = OnGetBossEffect();
			}
			if (m_motion == null)
			{
				m_motion = OnGetBossMotion();
			}
			OnInit();
		}

		protected virtual void OnInit()
		{
		}

		protected virtual ObjBossParameter OnGetBossParam()
		{
			return null;
		}

		protected virtual ObjBossEffect OnGetBossEffect()
		{
			return null;
		}

		protected virtual ObjBossMotion OnGetBossMotion()
		{
			return null;
		}

		protected virtual void OnChangeChara()
		{
		}

		public void Setup()
		{
			m_playerInfo = ObjUtil.GetPlayerInformation();
			m_levelInfo = ObjUtil.GetLevelInformation();
			if (m_levelInfo != null)
			{
				m_levelInfo.BossEndTime = m_param.BossDistance;
			}
			OnSetup();
		}

		protected virtual void OnSetup()
		{
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			if (m_playerInfo != null)
			{
				if (!m_playerDead)
				{
					if (m_playerInfo.IsDead())
					{
						GameObject[] array = GameObject.FindGameObjectsWithTag("Gimmick");
						GameObject[] array2 = array;
						foreach (GameObject gameObject in array2)
						{
							gameObject.SendMessage("OnMsgNotifyDead", new MsgNotifyDead(), SendMessageOptions.DontRequireReceiver);
						}
						m_playerDead = true;
					}
				}
				else if (m_playerChange && !m_playerInfo.IsDead())
				{
					m_playerDead = false;
				}
			}
			float deltaTime = Time.deltaTime;
			OnFsmUpdate(deltaTime);
			m_hitBumper = false;
			UpdateMove(deltaTime);
			DebugDrawInfo();
		}

		protected virtual void OnFsmUpdate(float delta)
		{
		}

		public void UpdateMove(float delta)
		{
			if (m_speedKeep && !m_phantom)
			{
				Transform transform = base.transform;
				Vector3 playerPosition = GetPlayerPosition();
				float x = playerPosition.x + m_keepDistance;
				Vector3 position = base.transform.position;
				float y = position.y;
				Vector3 position2 = base.transform.position;
				transform.position = new Vector3(x, y, position2.z);
				return;
			}
			Vector3 playerPosition2 = GetPlayerPosition();
			Vector3 prevPlayerPos = m_prevPlayerPos;
			if (m_playerDead || prevPlayerPos != playerPosition2)
			{
				float keepDistance = m_keepDistance;
				float playerBossPositionX = GetPlayerBossPositionX();
				float num = keepDistance - playerBossPositionX;
				if (Mathf.Abs(num) > 0.01f && m_param.PlayerSpeed > m_param.Speed)
				{
					float num2 = num / m_param.PlayerSpeed;
					float num3 = m_param.Speed * num2;
					m_keepDistance = playerBossPositionX + num3;
				}
				else
				{
					float num4 = m_param.Speed * delta;
					m_keepDistance = playerBossPositionX + num4 + m_moveAddStep;
				}
				m_moveStep -= m_moveAddStep;
				if (m_moveStep < 0f)
				{
					m_moveStep = 0f;
					m_moveAddStep = 0f;
				}
			}
			else
			{
				float num5 = m_param.Speed * delta;
				m_moveStep += num5;
				m_moveAddStep = m_moveStep * 0.05f;
			}
			m_prevPlayerPos = playerPosition2;
			Transform transform2 = base.transform;
			Vector3 playerPosition3 = GetPlayerPosition();
			float x2 = playerPosition3.x + m_keepDistance;
			Vector3 position3 = base.transform.position;
			float y2 = position3.y;
			Vector3 position4 = base.transform.position;
			transform2.position = new Vector3(x2, y2, position4.z);
		}

		public void UpdateSpeedDown(float delta, float down)
		{
			m_speedKeep = false;
			m_param.Speed -= delta * down;
			if (m_param.Speed < m_param.MinSpeed)
			{
				m_param.Speed = m_param.MinSpeed;
			}
		}

		public void UpdateSpeedUp(float delta, float up)
		{
			m_speedKeep = false;
			m_param.Speed += delta * up;
		}

		public void SetSpeed(float speed)
		{
			m_speedKeep = false;
			m_param.Speed = speed;
			m_keepDistance = GetPlayerBossPositionX();
		}

		public void KeepSpeed()
		{
			m_speedKeep = true;
			m_param.Speed = m_param.PlayerSpeed;
			m_keepDistance = GetPlayerBossPositionX();
		}

		public void SetupMoveY(float step)
		{
			m_param.StepMoveY = step;
		}

		public void UpdateMoveY(float delta, float pos_y, float speed)
		{
			m_param.StepMoveY -= delta * m_param.StepMoveY * 0.5f * speed;
			if (m_param.StepMoveY < 0.01f)
			{
				m_param.StepMoveY = 0f;
			}
			Vector3 currentVelocity = Vector3.zero;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			Vector3 target = new Vector3(x, pos_y, position2.z);
			base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref currentVelocity, m_param.StepMoveY);
		}

		public float GetPlayerDistance()
		{
			return Mathf.Abs(GetPlayerBossPositionX());
		}

		public Vector3 GetPlayerPosition()
		{
			if (m_playerInfo != null)
			{
				return m_playerInfo.Position;
			}
			return Vector3.zero;
		}

		public float GetPlayerBossPositionX()
		{
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 playerPosition = GetPlayerPosition();
			return x - playerPosition.x;
		}

		public void DebugDrawState(string name)
		{
			if (m_debugDrawState)
			{
				Debug.Log("BossState(" + name + ")");
			}
		}

		public void SetHitCheck(bool flag)
		{
			if (m_hitCheck != flag)
			{
				m_hitCheck = flag;
				SetBossStateAttackOK(flag);
			}
		}

		public bool IsBossDistanceEnd()
		{
			return m_bossDistanceEnd;
		}

		public bool IsPlayerDead()
		{
			return m_playerDead;
		}

		public bool IsHitBumper()
		{
			return m_hitBumper;
		}

		public bool IsClear()
		{
			return m_clear;
		}

		public void AddDamage()
		{
			int hpDown = ObjUtil.GetChaoAbliltyValue(ChaoAbility.AGGRESSIVITY_UP_FOR_RAID_BOSS, m_param.BossAttackPower);
			if (ColorPowerHit || ChaoHit)
			{
				hpDown = 1;
			}
			SetHpDown(hpDown);
		}

		private void SetHpDown(int addDamage)
		{
			int count = m_param.BossHP;
			m_param.BossHP -= addDamage;
			if (m_param.BossHP < 0)
			{
				m_param.BossHP = 0;
			}
			else
			{
				count = addDamage;
			}
			if (m_param.TypeBoss != 0 && m_levelInfo != null)
			{
				m_levelInfo.AddNumBossAttack(count);
			}
			SetHpGauge(m_param.BossHP);
		}

		public void ChaoHpDown()
		{
			int bossHP = m_param.BossHP;
			int num = bossHP - ObjUtil.GetChaoAbliltyValue(ChaoAbility.MAP_BOSS_DAMAGE, bossHP);
			if (num > 0)
			{
				SetHpDown(num);
			}
		}

		public void RequestStartChaoAbility()
		{
			if (EventManager.Instance != null && !EventManager.Instance.IsRaidBossStage())
			{
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.BOSS_SUPER_RING_RATE, true);
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.BOSS_RED_RING_RATE, true);
			}
			if (m_param.TypeBoss == 0)
			{
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.BOSS_STAGE_TIME, true);
			}
			else
			{
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.MAP_BOSS_DAMAGE, true);
				ChaoHpDown();
			}
			m_effect.PlayChaoEffect();
		}

		public void BossEnd(bool dead)
		{
			bool flag = false;
			if ((bool)StageTutorialManager.Instance && !StageTutorialManager.Instance.IsCompletedTutorial())
			{
				flag = true;
			}
			if (flag)
			{
				GameObjectUtil.SendMessageFindGameObject("StageTutorialManager", "OnMsgTutorialEnd", new MsgTutorialEnd(), SendMessageOptions.DontRequireReceiver);
				return;
			}
			if (m_param.TypeBoss != 0)
			{
				AddStockRing();
			}
			MsgBossEnd value = new MsgBossEnd(dead);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnBossEnd", value, SendMessageOptions.DontRequireReceiver);
		}

		public void BossClear()
		{
			m_clear = true;
			MsgBossClear value = new MsgBossClear();
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnBossClear", value, SendMessageOptions.DontRequireReceiver);
		}

		public void SetBossStateAttackOK(bool flag)
		{
			if (m_bossDistanceEndArea)
			{
				return;
			}
			if (flag && m_param.AfterAttack)
			{
				if (ObjUtil.RequestStartAbilityToChao(ChaoAbility.PURSUES_TO_BOSS_AFTER_ATTACK, false))
				{
					flag = false;
				}
				else
				{
					m_param.AfterAttack = false;
				}
			}
			bool flag2 = false;
			if (flag)
			{
				flag2 = ObjUtil.RequestStartAbilityToChao(ChaoAbility.BOSS_ATTACK, false);
			}
			else
			{
				ObjUtil.RequestEndAbilityToChao(ChaoAbility.BOSS_ATTACK);
			}
			MsgBossCheckState.State state = MsgBossCheckState.State.IDLE;
			if (flag && !flag2)
			{
				state = MsgBossCheckState.State.ATTACK_OK;
			}
			if (StageItemManager.Instance != null)
			{
				MsgBossCheckState msg = new MsgBossCheckState(state);
				StageItemManager.Instance.OnMsgBossCheckState(msg);
			}
		}

		public void UpdateBossStateAfterAttack()
		{
			m_param.AfterAttack = !m_param.AfterAttack;
		}

		public void CreateFeverRing()
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjFeverRing");
			if (!(gameObject != null))
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				ObjFeverRing component = gameObject2.GetComponent<ObjFeverRing>();
				if ((bool)component)
				{
					component.Setup(m_param.RingCount, m_param.SuperRingRatio, m_param.RedStarRingRatio, m_param.BronzeTimerRatio, m_param.SilverTimerRatio, m_param.GoldTimerRatio, (BossType)m_param.TypeBoss);
				}
			}
		}

		public void CreateEventFeverRing(int playerAggressivity)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjFeverRing");
			if (!(gameObject != null))
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, Quaternion.identity) as GameObject;
			if (!gameObject2)
			{
				return;
			}
			int num = EventBossParamTable.GetSuperRingDropData((BossType)m_param.TypeBoss, playerAggressivity);
			gameObject2.gameObject.SetActive(true);
			ObjFeverRing component = gameObject2.GetComponent<ObjFeverRing>();
			if ((bool)component)
			{
				if (m_param.RedStarRingRatio + num > 100)
				{
					num = 100 - m_param.RedStarRingRatio;
				}
				component.Setup(m_param.RingCount, num, m_param.RedStarRingRatio, m_param.BronzeTimerRatio, m_param.SilverTimerRatio, m_param.GoldTimerRatio, (BossType)m_param.TypeBoss);
			}
		}

		public void CreateMissile(Vector3 pos)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossMissile");
			if (!(gameObject != null))
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, pos, Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				ObjBossMissile component = gameObject2.GetComponent<ObjBossMissile>();
				if ((bool)component)
				{
					component.Setup(base.gameObject, m_param.MissileSpeed, (BossType)m_param.TypeBoss);
				}
			}
		}

		public void CreateTrapBall(Vector3 colli, float attackPos, int randBall, bool bossAppear)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossTrapBall");
			if (!(gameObject != null))
			{
				return;
			}
			Vector3 position = base.transform.position;
			if (!bossAppear)
			{
				Vector3 playerPosition = GetPlayerPosition();
				position = new Vector3(playerPosition.x + 18f, attackPos, position.z);
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, position, Quaternion.identity) as GameObject;
			if (!gameObject2)
			{
				return;
			}
			gameObject2.gameObject.SetActive(true);
			ObjBossTrapBall component = gameObject2.GetComponent<ObjBossTrapBall>();
			if (!component)
			{
				return;
			}
			bool flag = true;
			if (m_param.AttackBallFlag)
			{
				flag = false;
				m_param.AttackBallFlag = false;
			}
			else
			{
				int randomRange = ObjUtil.GetRandomRange100();
				if (randomRange >= randBall && m_param.AttackTrapCount < m_param.AttackTrapCountMax)
				{
					flag = false;
				}
			}
			BossTrapBallType bossTrapBallType = BossTrapBallType.BREAK;
			if (!flag)
			{
				bossTrapBallType = BossTrapBallType.BREAK;
				m_param.AttackTrapCount++;
			}
			else
			{
				bossTrapBallType = BossTrapBallType.ATTACK;
				m_param.AttackBallFlag = true;
				m_param.AttackTrapCount = 0;
			}
			component.Setup(base.gameObject, colli, m_param.RotSpeed, m_param.AttackSpeed, bossTrapBallType, (BossType)m_param.TypeBoss, bossAppear);
		}

		public GameObject CreateBom(bool colli, float shot_speed, bool shot)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossBom");
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, ObjBossUtil.GetBossHatchPos(base.gameObject), Quaternion.identity) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.gameObject.SetActive(true);
					ObjBossBom component = gameObject2.GetComponent<ObjBossBom>();
					if ((bool)component)
					{
						component.Setup(base.gameObject, colli, GetShotRotation(m_param.ShotRotBase), shot_speed, m_param.AddSpeedRatio, shot);
					}
					return gameObject2;
				}
			}
			return null;
		}

		public void BlastBom(GameObject bom_obj)
		{
			if ((bool)bom_obj)
			{
				ObjBossBom component = bom_obj.GetComponent<ObjBossBom>();
				if ((bool)component)
				{
					component.Blast("ef_bo_em_dead_bom01", 5f);
				}
			}
		}

		public void ShotBom(GameObject bom_obj)
		{
			if ((bool)bom_obj)
			{
				ObjBossBom component = bom_obj.GetComponent<ObjBossBom>();
				if ((bool)component)
				{
					component.SetShot(true);
				}
			}
		}

		public void CreateBumper(bool bossAppear, float addX = 0f)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, "ObjBossBall");
			if (!(gameObject != null))
			{
				return;
			}
			Vector3 position = Vector3.zero;
			if (!bossAppear)
			{
				Vector3 playerPosition = GetPlayerPosition();
				float x = playerPosition.x + addX;
				Vector3 position2 = base.transform.position;
				float y = position2.y;
				Vector3 position3 = base.transform.position;
				position = new Vector3(x, y, position3.z);
			}
			else
			{
				position = ObjBossUtil.GetBossHatchPos(base.gameObject);
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, position, Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				ObjBossBall component = gameObject2.GetComponent<ObjBossBall>();
				if ((bool)component)
				{
					ObjBossBall.SetData setData = default(ObjBossBall.SetData);
					setData.obj = base.gameObject;
					setData.bound_param = GetBoundParam();
					setData.type = BossBallType.BUMPER;
					setData.shot_rot = GetShotRotation(m_param.ShotRotBase);
					setData.shot_speed = m_param.ShotSpeed;
					setData.attack_speed = 0f;
					setData.firstSpeed = m_param.BumperFirstSpeed;
					setData.outOfcontrol = m_param.BumperOutOfcontrol;
					setData.ballSpeed = m_param.BallSpeed;
					setData.bossAppear = bossAppear;
					component.Setup(setData);
				}
			}
		}

		public Quaternion GetShotRotation(Vector3 rot_angle)
		{
			float num = 0f;
			if (m_param.Speed > 0f)
			{
				num = m_param.Speed / m_param.PlayerSpeed;
			}
			float num2 = 30f * num * m_param.AddSpeedRatio;
			if (num2 > 60f)
			{
				num2 = 60f;
			}
			Vector3 euler = rot_angle * num2;
			return base.transform.rotation * Quaternion.FromToRotation(base.transform.up, -base.transform.up) * Quaternion.Euler(euler);
		}

		public void OpenHpGauge()
		{
			if (!ObjBossUtil.IsNowLastChance(m_playerInfo))
			{
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "HudBossHpGaugeOpen", new MsgHudBossHpGaugeOpen((BossType)m_param.TypeBoss, m_param.BossLevel, m_param.BossHP, m_param.BossHPMax), SendMessageOptions.DontRequireReceiver);
			}
		}

		public void StartGauge()
		{
			if (!ObjBossUtil.IsNowLastChance(m_playerInfo))
			{
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "HudBossGaugeStart", new MsgHudBossGaugeStart(), SendMessageOptions.DontRequireReceiver);
			}
		}

		public void SetHpGauge(int hp)
		{
			if (!ObjBossUtil.IsNowLastChance(m_playerInfo))
			{
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "HudBossHpGaugeSet", new MsgHudBossHpGaugeSet(m_param.BossHP, m_param.BossHPMax), SendMessageOptions.DontRequireReceiver);
			}
		}

		public void SetClear()
		{
			if (HudCaution.Instance != null)
			{
				MsgCaution caution = new MsgCaution(HudCaution.Type.MAP_BOSS_CLEAR);
				HudCaution.Instance.SetCaution(caution);
			}
		}

		public void SetFailed()
		{
			if (HudCaution.Instance != null)
			{
				MsgCaution caution = new MsgCaution(HudCaution.Type.MAP_BOSS_FAILED);
				HudCaution.Instance.SetCaution(caution);
				ObjUtil.PlaySE("boss_failed");
			}
		}

		private void AddStockRing()
		{
			ObjUtil.SendMessageTransferRing();
		}

		public float GetBoundParam()
		{
			int randomRange = ObjUtil.GetRandomRange100();
			if (randomRange < m_param.BoundMaxRand)
			{
				return GetBoundParam(m_param.BoundParamMax);
			}
			return GetBoundParam(m_param.BoundParamMin);
		}

		public float GetBoundParam(float param)
		{
			return param;
		}

		public float GetAttackInterspace()
		{
			return Random.Range(m_param.AttackInterspaceMin, m_param.AttackInterspaceMax);
		}

		public float GetDamageSpeedParam()
		{
			float num = 0f;
			float playerDistance = GetPlayerDistance();
			num = 1f - playerDistance * 0.04f;
			if (num < 0.5f)
			{
				num = 0.5f;
			}
			return num;
		}

		public void OnMsgBossDistanceEnd(MsgBossDistanceEnd msg)
		{
			if (msg.m_end)
			{
				m_bossDistanceEnd = true;
			}
			else
			{
				m_bossDistanceEndArea = true;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				string a = LayerMask.LayerToName(gameObject.layer);
				if (a == "Player")
				{
					bool flag = false;
					if (gameObject.name == "ChaoPartsAttackEnemy" || gameObject.name.Contains("pha_"))
					{
						flag = true;
					}
					else if (m_hitCheck)
					{
						flag = true;
					}
					if (flag)
					{
						MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerSpin);
						gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			if (m_hitCheck)
			{
				m_effect.SetHitOffset(gameObject.transform.position - base.transform.position);
			}
		}

		private void OnDamageHit(MsgHitDamage msg)
		{
			if (!msg.m_sender)
			{
				return;
			}
			GameObject gameObject = msg.m_sender.gameObject;
			if (!gameObject || msg.m_attackPower <= 0)
			{
				return;
			}
			if (m_hitCheck)
			{
				MsgHitDamageSucceed value = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
				gameObject.SendMessage("OnDamageSucceed", value, SendMessageOptions.DontRequireReceiver);
				if (gameObject.tag == "ChaoAttack" || gameObject.tag == "Chao")
				{
					ChaoHit = true;
				}
				if (msg.m_attackPower == 4)
				{
					ColorPowerHit = true;
				}
				OnChangeDamageState();
			}
			else if (gameObject.tag == "ChaoAttack" || gameObject.tag == "Chao" || msg.m_attackPower == 4)
			{
				MsgHitDamageSucceed value2 = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
				gameObject.SendMessage("OnDamageSucceed", value2, SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void OnChangeDamageState()
		{
		}

		public void OnTransformPhantom(MsgTransformPhantom msg)
		{
			m_phantom = true;
		}

		public void OnReturnFromPhantom(MsgReturnFromPhantom msg)
		{
			m_phantom = false;
		}

		public void OnChangeCharaSucceed(MsgChangeCharaSucceed msg)
		{
			m_playerChange = true;
			OnChangeChara();
		}

		public void OnMsgPrepareContinue(MsgPrepareContinue msg)
		{
			m_playerChange = true;
		}

		public void OnMsgDebugDead()
		{
			if (m_param.BossHP > 0 && m_param.TypeBoss != 0 && m_levelInfo != null)
			{
				m_levelInfo.AddNumBossAttack(m_param.BossHP);
			}
			BossEnd(true);
		}

		public void OnHitBumper()
		{
			m_hitBumper = true;
		}

		private void DebugDrawInfo()
		{
			if (m_debugDrawInfo)
			{
				Debug.Log("BossInfo BossSpeed=" + m_param.Speed + " PlayerSpeed=" + m_param.PlayerSpeed + "AddSpeedRatio=" + m_param.AddSpeedRatio + "AddSpeed=" + m_param.AddSpeed);
			}
		}

		public void DebugDrawInfo(string str)
		{
			if (m_debugDrawInfo)
			{
				Debug.Log("BossInfo " + str);
			}
		}
	}
}
