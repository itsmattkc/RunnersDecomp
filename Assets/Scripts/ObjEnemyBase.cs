using Message;
using Mission;
using System.Collections.Generic;
using Tutorial;
using UnityEngine;

public class ObjEnemyBase : SpawnableObject
{
	private bool m_rightAnimFlag;

	private bool m_setupAnimFlag;

	private bool m_destroyFlag;

	private bool m_heabyBomFlag;

	private bool m_end;

	private ObjEnemyUtil.EnemyType m_enmyType;

	protected override void OnSpawned()
	{
		if (StageComboManager.Instance != null && StageComboManager.Instance.IsChaoFlagStatus(StageComboManager.ChaoFlagStatus.ENEMY_DEAD))
		{
			SetBroken();
		}
	}

	private void Update()
	{
		if (!m_end)
		{
			if (!m_setupAnimFlag)
			{
				m_setupAnimFlag = SetupAnim();
			}
			if (m_destroyFlag)
			{
				m_end = true;
				CreateBrokenItem();
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void OnMsgObjectDead(MsgObjectDead msg)
	{
		if (base.enabled)
		{
			SetBroken();
		}
	}

	public void OnMsgHeavyBombDead(float goldAnimalPercent)
	{
		if (base.enabled)
		{
			float num = Random.Range(0f, 99.9f);
			if (goldAnimalPercent >= num)
			{
				m_heabyBomFlag = true;
			}
			SetBroken();
		}
	}

	protected override string GetModelName()
	{
		return ObjEnemyUtil.GetModelName(GetEnemyType(), GetModelFiles());
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.ENEMY_RESOURCE;
	}

	protected virtual ObjEnemyUtil.EnemyType GetOriginalType()
	{
		return ObjEnemyUtil.EnemyType.NORMAL;
	}

	protected virtual string[] GetModelFiles()
	{
		return null;
	}

	protected virtual int[] GetScoreTable()
	{
		return ObjEnemyUtil.GetDefaultScoreTable();
	}

	protected virtual ObjEnemyUtil.EnemyEffectSize GetEffectSize()
	{
		return ObjEnemyUtil.EnemyEffectSize.S;
	}

	protected virtual bool IsNormalMotion(float speed)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		float y = eulerAngles.y;
		if (y > 80f && y < 100f)
		{
			return false;
		}
		return true;
	}

	protected void SetupEnemy(uint id, float speed)
	{
		m_enmyType = GetOriginalType();
		SetupRareCheck(id);
		SetupMetalCheck();
		if (!IsNormalMotion(speed))
		{
			m_rightAnimFlag = true;
		}
	}

	private void SetupRareCheck(uint id)
	{
		StageComboManager instance = StageComboManager.Instance;
		if (instance != null && instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_RARE_ENEMY))
		{
			m_enmyType = ObjEnemyUtil.EnemyType.RARE;
		}
		if (m_enmyType == ObjEnemyUtil.EnemyType.RARE)
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindGameObjectWithTag("GameModeStage", "GameModeStage");
		if (!(gameObject != null))
		{
			return;
		}
		GameModeStage component = gameObject.GetComponent<GameModeStage>();
		if (component != null)
		{
			RareEnemyTable rareEnemyTable = component.GetRareEnemyTable();
			if (rareEnemyTable != null && rareEnemyTable.IsRareEnemy(id))
			{
				m_enmyType = ObjEnemyUtil.EnemyType.RARE;
			}
		}
	}

	private void SetupMetalCheck()
	{
		if (m_enmyType == ObjEnemyUtil.EnemyType.NORMAL)
		{
			StageComboManager instance = StageComboManager.Instance;
			if (instance != null && instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_METAL_AND_METAL_SCORE))
			{
				m_enmyType = ObjEnemyUtil.EnemyType.METAL;
			}
		}
	}

	private bool SetupAnim()
	{
		Animator componentInChildren = GetComponentInChildren<Animator>();
		if ((bool)componentInChildren)
		{
			if (m_rightAnimFlag)
			{
				componentInChildren.Play("Idle_r");
			}
			return true;
		}
		return false;
	}

	private bool IsRare()
	{
		return m_enmyType == ObjEnemyUtil.EnemyType.RARE;
	}

	private bool IsMetal()
	{
		return m_enmyType == ObjEnemyUtil.EnemyType.METAL;
	}

	private int GetScore()
	{
		return ObjEnemyUtil.GetScore(m_enmyType, GetScoreTable());
	}

	private ObjEnemyUtil.EnemyType GetEnemyType()
	{
		return m_enmyType;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_destroyFlag || !other)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if ((bool)gameObject)
		{
			AttackPower attack = AttackPower.PlayerSpin;
			if (m_enmyType == ObjEnemyUtil.EnemyType.METAL)
			{
				attack = AttackPower.PlayerSpin;
			}
			MsgHitDamage value = new MsgHitDamage(base.gameObject, attack);
			gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (m_destroyFlag || !msg.m_sender)
		{
			return;
		}
		GameObject gameObject = msg.m_sender.gameObject;
		if ((bool)gameObject)
		{
			bool flag = IsMetal();
			if (IsEnemyBroken(flag, msg.m_attackPower, msg.m_attackAttribute))
			{
				MsgHitDamageSucceed value = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
				gameObject.SendMessage("OnDamageSucceed", value, SendMessageOptions.DontRequireReceiver);
				SetPlayerBroken(flag, msg.m_attackAttribute);
				ObjUtil.CreateBrokenBonus(base.gameObject, gameObject, msg.m_attackAttribute);
			}
			else if (flag && msg.m_attackPower > 0)
			{
				MsgAttackGuard value2 = new MsgAttackGuard(base.gameObject);
				gameObject.SendMessage("OnAttackGuard", value2, SendMessageOptions.DontRequireReceiver);
				PlayGuardEffect();
				ObjUtil.LightPlaySE("enm_metal_hit");
			}
		}
	}

	private bool IsEnemyBroken(bool metal, int attackPower, uint attribute_state)
	{
		AttackPower attackPower2 = (!metal) ? AttackPower.PlayerSpin : AttackPower.PlayerPower;
		if (attackPower >= (int)attackPower2)
		{
			return true;
		}
		if (metal && attackPower == 2)
		{
			return (attribute_state & 8) != 0;
		}
		return false;
	}

	private void PlayHitEffect()
	{
		EffectPlayType type = EffectPlayType.UNKNOWN;
		switch (GetEffectSize())
		{
		case ObjEnemyUtil.EnemyEffectSize.S:
			type = EffectPlayType.ENEMY_S;
			break;
		case ObjEnemyUtil.EnemyEffectSize.M:
			type = EffectPlayType.ENEMY_M;
			break;
		case ObjEnemyUtil.EnemyEffectSize.L:
			type = EffectPlayType.ENEMY_L;
			break;
		}
		if (StageEffectManager.Instance != null)
		{
			StageEffectManager.Instance.PlayEffect(type, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
	}

	private void PlayGuardEffect()
	{
		if (StageEffectManager.Instance != null)
		{
			StageEffectManager.Instance.PlayEffect(EffectPlayType.ENEMY_GUARD, ObjUtil.GetCollisionCenterPosition(base.gameObject), Quaternion.identity);
		}
	}

	private void CreateBrokenItem()
	{
		TimerType timerType = GetTimerType();
		if (timerType != TimerType.ERROR)
		{
			ObjTimerUtil.CreateTimer(base.gameObject, timerType);
		}
		else
		{
			CreateAnimal();
		}
	}

	private TimerType GetTimerType()
	{
		if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode() && ObjTimerUtil.IsEnableCreateTimer())
		{
			GameObject gameObject = GameObjectUtil.FindGameObjectWithTag("GameModeStage", "GameModeStage");
			if (gameObject != null)
			{
				GameModeStage component = gameObject.GetComponent<GameModeStage>();
				if (component != null)
				{
					EnemyExtendItemTable enemyExtendItemTable = component.GetEnemyExtendItemTable();
					if (enemyExtendItemTable != null)
					{
						int randomRange = ObjUtil.GetRandomRange100();
						int tableItemData = enemyExtendItemTable.GetTableItemData(EnemyExtendItemTableItem.BronzeTimer);
						int num = tableItemData + enemyExtendItemTable.GetTableItemData(EnemyExtendItemTableItem.SilverTimer);
						int num2 = num + enemyExtendItemTable.GetTableItemData(EnemyExtendItemTableItem.GoldTimer);
						if (randomRange < tableItemData)
						{
							return TimerType.BRONZE;
						}
						if (randomRange < num)
						{
							return TimerType.SILVER;
						}
						if (randomRange < num2)
						{
							return TimerType.GOLD;
						}
					}
				}
			}
		}
		return TimerType.ERROR;
	}

	private void CreateAnimal()
	{
		if (m_heabyBomFlag)
		{
			ObjAnimalUtil.CreateAnimal(base.gameObject, AnimalType.FLICKY);
			m_heabyBomFlag = false;
		}
		else
		{
			ObjAnimalUtil.CreateAnimal(base.gameObject);
		}
	}

	private void SetPlayerBroken(bool metal_type, uint attribute_state)
	{
		int num = GetScore();
		if (metal_type && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.COMBO_METAL_AND_METAL_SCORE))
		{
			num = (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.COMBO_METAL_AND_METAL_SCORE);
		}
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.ENEMY_SCORE_SEVERALFOLD))
		{
			float chaoAbilityValue = StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.ENEMY_SCORE_SEVERALFOLD);
			float num2 = Random.Range(0f, 99.9f);
			if (chaoAbilityValue >= num2)
			{
				num *= (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.ENEMY_SCORE_SEVERALFOLD);
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.ENEMY_SCORE_SEVERALFOLD, false);
			}
		}
		List<ChaoAbility> abilityList = new List<ChaoAbility>();
		abilityList.Add(ChaoAbility.ENEMY_SCORE);
		ObjUtil.RequestStartAbilityToChao(ChaoAbility.ENEMY_SCORE, true);
		ObjUtil.GetChaoAbliltyPhantomFlag(attribute_state, ref abilityList);
		num = ObjUtil.GetChaoAndEnemyScore(abilityList, num);
		ObjUtil.SendMessageAddScore(num);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(1, num));
		ObjUtil.SendMessageMission(Mission.EventID.ENEMYDEAD, 1);
		if (IsRare())
		{
			ObjUtil.SendMessageMission(Mission.EventID.GOLDENENEMYDEAD, 1);
		}
		ObjUtil.SendMessageTutorialClear(Tutorial.EventID.ENEMY);
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.ENEMY_COUNT_BOMB))
		{
			GameObjectUtil.SendMessageToTagObjects("StageManager", "OnChaoAbilityEnemyBreak", null, SendMessageOptions.DontRequireReceiver);
		}
		SetBroken();
	}

	private void SetBroken()
	{
		if (!m_destroyFlag)
		{
			PlayHitEffect();
			ObjUtil.LightPlaySE(ObjEnemyUtil.GetSEName(m_enmyType));
			m_destroyFlag = true;
		}
	}
}
