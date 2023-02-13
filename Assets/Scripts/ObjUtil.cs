using Message;
using Mission;
using System.Collections.Generic;
using Tutorial;
using UnityEngine;

public class ObjUtil
{
	private static float PLAYER_SPEED1 = 8f;

	private static float PLAYER_SPEED2 = 10f;

	private static float PLAYER_SPEED3 = 14f;

	public static void SendMessageAddScore(int score)
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.AddScore(score);
		}
	}

	public static void SendMessageAddBonusScore(int score)
	{
		SendMessageAddScore(score);
	}

	public static void SendMessageAddAnimal(int addCount)
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.AddAnimal(addCount);
		}
	}

	public static void SendMessageAddRedRing()
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.AddRedRing();
		}
	}

	public static void SendMessageTransferRing()
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.TransferRing();
		}
	}

	public static void SendMessageTransferRingForContinue(int ring)
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.TransferRingForContinue(ring);
		}
	}

	public static void SendMessageFinalScore()
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.OnCalcFinalScore();
		}
	}

	public static void SendMessageFinalScoreBeforeResult()
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.SendMessageFinalScoreBeforeResult();
		}
	}

	public static void SendMessageAddSpecialCrystal(int count)
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.AddSpecialCrystal(count);
		}
	}

	public static void SendMessageScoreCheck(StageScoreData scoreData)
	{
		if (StageScoreManager.Instance != null)
		{
			StageScoreManager.Instance.AddScoreCheck(scoreData);
		}
	}

	public static void AddCombo()
	{
		StageComboManager instance = StageComboManager.Instance;
		if (instance != null)
		{
			instance.AddCombo();
		}
	}

	public static void SendMessageMission(MsgMissionEvent msg)
	{
		if ((bool)StageMissionManager.Instance)
		{
			GameObjectUtil.SendDelayedMessageToGameObject(StageMissionManager.Instance.gameObject, "OnMissionEvent", msg);
		}
	}

	public static void SendMessageMission2(MsgMissionEvent msg)
	{
		if ((bool)StageMissionManager.Instance)
		{
			GameObjectUtil.SendMessageFindGameObject("StageMissionManager", "OnMissionEvent", msg, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SendMessageMission(Mission.EventID id, int num)
	{
		MsgMissionEvent msg = new MsgMissionEvent(id, num);
		SendMessageMission(msg);
	}

	public static void SendMessageMission(Mission.EventID id)
	{
		MsgMissionEvent msg = new MsgMissionEvent(id);
		SendMessageMission(msg);
	}

	public static void SendMessageTutorialClear(Tutorial.EventID id)
	{
		if ((bool)StageTutorialManager.Instance)
		{
			MsgTutorialClear value = new MsgTutorialClear(id);
			GameObjectUtil.SendDelayedMessageToGameObject(StageTutorialManager.Instance.gameObject, "OnMsgTutorialClear", value);
		}
	}

	public static Vector3 GetCollisionCenter(GameObject obj)
	{
		if ((bool)obj)
		{
			SphereCollider component = obj.GetComponent<SphereCollider>();
			if ((bool)component)
			{
				return component.center;
			}
			BoxCollider component2 = obj.GetComponent<BoxCollider>();
			if ((bool)component2)
			{
				return component2.center;
			}
		}
		return Vector3.zero;
	}

	public static Vector3 GetCollisionCenterPosition(GameObject obj)
	{
		if ((bool)obj)
		{
			Vector3 collisionCenter = GetCollisionCenter(obj);
			return obj.transform.position + obj.transform.TransformDirection(collisionCenter);
		}
		return Vector3.zero;
	}

	public static void PlayEffectCollisionCenter(GameObject obj, string name, float destroy_time, bool playLightMode = false)
	{
		if ((bool)obj)
		{
			PlayEffect(name, GetCollisionCenterPosition(obj), obj.transform.rotation, destroy_time, playLightMode);
		}
	}

	public static void PlayEffect(string name, Vector3 pos, Quaternion rot, float destroy_time, bool playLightMode = false)
	{
		if (!playLightMode && IsLightMode())
		{
			return;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.COMMON_EFFECT, name);
		if (gameObject == null)
		{
			gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, name);
		}
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, pos, rot) as GameObject;
		if ((bool)gameObject2)
		{
			gameObject2.SetActive(true);
			ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
			if ((bool)component)
			{
				component.Play();
			}
			if (destroy_time > 0f)
			{
				Object.Destroy(gameObject2, destroy_time);
			}
		}
	}

	public static void PlayEffectCollisionCenterChild(GameObject obj, string name, float destroy_time, bool playLightMode = false)
	{
		if ((bool)obj)
		{
			PlayEffectChild(obj, name, GetCollisionCenter(obj), Quaternion.identity, destroy_time, playLightMode);
		}
	}

	public static void PlayEffectChild(GameObject parent, string name, Vector3 local_pos, Quaternion local_rot, float destroy_time, bool playLightMode = true)
	{
		GameObject gameObject = PlayEffectChild(parent, name, local_pos, local_rot, playLightMode);
		if ((bool)gameObject && destroy_time > 0f)
		{
			Object.Destroy(gameObject, destroy_time);
		}
	}

	public static GameObject PlayEffectChild(GameObject parent, string name, Vector3 local_pos, Quaternion local_rot, bool playLightMode = false)
	{
		if (!playLightMode && IsLightMode())
		{
			return null;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.COMMON_EFFECT, name);
		if (gameObject == null)
		{
			gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, name);
		}
		if (gameObject != null && parent != null)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, parent.transform.position, parent.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(true);
				gameObject2.transform.parent = parent.transform;
				gameObject2.transform.localPosition = local_pos;
				gameObject2.transform.localRotation = local_rot;
				ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
				if ((bool)component)
				{
					component.Play();
				}
				return gameObject2;
			}
		}
		return null;
	}

	public static GameObject PlayChaoEffect(GameObject parent, string name, Vector3 pos, float destroy_time, bool playLightMode)
	{
		if (!playLightMode && IsLightMode())
		{
			return null;
		}
		return PlayChaoEffect(parent, name, pos, destroy_time);
	}

	public static GameObject PlayChaoEffect(GameObject parent, string name, float destroy_time)
	{
		return PlayChaoEffect(parent, name, GetCollisionCenter(parent), destroy_time);
	}

	public static void PlayChaoEffect(string name, Vector3 pos, float destroy_time)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.CHAO_MODEL, name);
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, pos, Quaternion.identity) as GameObject;
		if (gameObject2 != null)
		{
			gameObject2.SetActive(true);
			ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Play();
			}
			if (destroy_time > 0f)
			{
				Object.Destroy(gameObject2, destroy_time);
			}
		}
	}

	public static GameObject PlayChaoEffect(GameObject parent, string name, Vector3 pos, float destroy_time)
	{
		if (parent != null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.CHAO_MODEL, name);
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, parent.transform.position, parent.transform.rotation) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.parent = parent.transform;
					gameObject2.transform.localPosition = pos;
					gameObject2.transform.localRotation = Quaternion.identity;
					ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
					if ((bool)component)
					{
						component.Play();
					}
					if (destroy_time > 0f)
					{
						Object.Destroy(gameObject2, destroy_time);
					}
					return gameObject2;
				}
			}
		}
		return null;
	}

	public static GameObject PlayChaoEffectForHUD(GameObject parent, string name, float destroy_time)
	{
		if (parent != null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.CHAO_MODEL, name);
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, parent.transform.position, parent.transform.rotation) as GameObject;
				if ((bool)gameObject2)
				{
					Vector3 localPosition = gameObject2.transform.localPosition;
					Vector3 localScale = gameObject2.transform.localScale;
					Quaternion localRotation = gameObject2.transform.localRotation;
					gameObject2.SetActive(true);
					gameObject2.transform.parent = parent.transform;
					gameObject2.transform.localPosition = localPosition;
					gameObject2.transform.localRotation = localRotation;
					gameObject2.transform.localScale = localScale;
					ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
					if ((bool)component)
					{
						component.Play();
					}
					if (destroy_time > 0f)
					{
						Object.Destroy(gameObject2, destroy_time);
					}
					return gameObject2;
				}
			}
		}
		return null;
	}

	public static void StopAnimation(GameObject obj)
	{
		if ((bool)obj)
		{
			Animation componentInChildren = obj.GetComponentInChildren<Animation>();
			if ((bool)componentInChildren)
			{
				componentInChildren.playAutomatically = false;
				componentInChildren.Stop();
			}
		}
	}

	public static SoundManager.PlayId PlaySE(string name, string cueSheetName = "SE")
	{
		return SoundManager.SePlay(name, cueSheetName);
	}

	public static SoundManager.PlayId PlayEventSE(string name, EventManager.EventType eventType)
	{
		return PlaySE(name, "SE_" + EventManager.GetEventTypeName(eventType));
	}

	public static SoundManager.PlayId LightPlaySE(string name, string cueSheetName = "SE")
	{
		if (IsLightMode())
		{
			return SoundManager.PlayId.NONE;
		}
		return PlaySE(name, cueSheetName);
	}

	public static SoundManager.PlayId LightPlayEventSE(string name, EventManager.EventType eventType)
	{
		return LightPlaySE(name, "SE_" + EventManager.GetEventTypeName(eventType));
	}

	public static void StopSE(SoundManager.PlayId id)
	{
		SoundManager.SeStop(id);
	}

	public static void CreateLostRing(Vector3 pos, Quaternion rot, int ringCount)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjLostRing");
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
		ObjLostRing component = gameObject2.GetComponent<ObjLostRing>();
		if (!(component != null))
		{
			return;
		}
		component.SetRingCount(ringCount);
		StageAbilityManager instance = StageAbilityManager.Instance;
		if ((bool)instance)
		{
			GameObject lostRingChao = instance.GetLostRingChao();
			if (lostRingChao != null)
			{
				component.SetChaoMagnet(lostRingChao);
			}
		}
	}

	public static void SetPlayerDeadRecoveryRing(PlayerInformation information)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null && information != null && instance.HasChaoAbility(ChaoAbility.RECOVERY_RING) && information.NumRings > 0)
		{
			instance.SetLostRingCount(information.NumRings);
			information.LostRings();
		}
	}

	public static void StartMagnetControl(GameObject obj)
	{
		StartMagnetControl(obj, 0f);
	}

	public static void StartMagnetControl(GameObject obj, GameObject target)
	{
		StartMagnetControl(obj, target, 0f);
	}

	public static void StartMagnetControl(GameObject obj, float time)
	{
		StartMagnetControl(obj, null, time);
	}

	public static void StartMagnetControl(GameObject obj, GameObject target, float time)
	{
		MagnetControl component = obj.GetComponent<MagnetControl>();
		if (component != null)
		{
			component.OnUseMagnet(new MsgUseMagnet(obj, target, time));
		}
	}

	public static float GetPlayerDefaultSpeed()
	{
		PlayerInformation playerInformation = GetPlayerInformation();
		if ((bool)playerInformation && playerInformation.DefaultSpeed > 0f)
		{
			return playerInformation.DefaultSpeed;
		}
		return PLAYER_SPEED1;
	}

	public static float GetPlayerAddSpeed()
	{
		float playerDefaultSpeed = GetPlayerDefaultSpeed();
		return Mathf.Max(playerDefaultSpeed - PLAYER_SPEED1, 0f);
	}

	public static float GetPlayerAddSpeedRatio()
	{
		float playerDefaultSpeed = GetPlayerDefaultSpeed();
		if (playerDefaultSpeed > 0f)
		{
			return Mathf.Max(playerDefaultSpeed / PLAYER_SPEED1, 1f);
		}
		return 1f;
	}

	public static void StartHudAlert(GameObject obj)
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if ((bool)gameObject)
		{
			HudAlert hudAlert = GameObjectUtil.FindChildGameObjectComponent<HudAlert>(gameObject, "HudAlert");
			if ((bool)hudAlert)
			{
				hudAlert.StartAlert(obj);
				PlaySE("obj_missile_warning");
			}
		}
	}

	public static int GetRandomRange100()
	{
		return Random.Range(0, 100);
	}

	public static int GetRandomRange(int maxRate)
	{
		return Random.Range(0, maxRate);
	}

	public static void SetModelVisible(GameObject obj, bool flag)
	{
		if ((bool)obj)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>(true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshRenderer meshRenderer = (MeshRenderer)array[i];
				meshRenderer.enabled = flag;
			}
		}
	}

	public static bool CheckGroundHit(Vector3 pos, Vector3 up, float up_hit_length, float down_hit_length, out Vector3 hit_pos)
	{
		Vector3 direction = -up;
		Vector3 origin = pos + up * up_hit_length;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction, out hitInfo, down_hit_length))
		{
			hit_pos = hitInfo.point;
			return true;
		}
		hit_pos = pos;
		return false;
	}

	public static void SendMessageAppearTrampoline()
	{
		MsgUseItem value = new MsgUseItem(ItemType.TRAMPOLINE);
		GameObjectUtil.SendMessageToTagObjects("Gimmick", "OnUseItem", value, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMessageOnObjectDead()
	{
		MsgObjectDead value = new MsgObjectDead();
		GameObjectUtil.SendDelayedMessageToTagObjects("Gimmick", "OnMsgObjectDead", value);
		GameObjectUtil.SendDelayedMessageToTagObjects("Enemy", "OnMsgObjectDead", value);
	}

	public static void SendMessageOnBossObjectDead()
	{
		MsgObjectDead value = new MsgObjectDead();
		GameObjectUtil.SendMessageToTagObjects("Gimmick", "OnMsgObjectDead", value, SendMessageOptions.DontRequireReceiver);
		GameObjectUtil.SendMessageToTagObjects("Enemy", "OnMsgObjectDead", value, SendMessageOptions.DontRequireReceiver);
	}

	private static ObjectPartTable GetObjectPartTable()
	{
		GameObject gameObject = GameObject.Find("GameModeStage");
		if (gameObject != null)
		{
			GameModeStage component = gameObject.GetComponent<GameModeStage>();
			if (component != null)
			{
				return component.GetObjectPartTable();
			}
		}
		return null;
	}

	private static BrokenBonusType GetGameModeBrokenBonusType()
	{
		ObjectPartTable objectPartTable = GetObjectPartTable();
		if (objectPartTable != null)
		{
			return objectPartTable.GetBrokenBonusType();
		}
		return BrokenBonusType.NONE;
	}

	private static BrokenBonusType GetGameModeBrokenBonusTypeForChaoAbility()
	{
		ObjectPartTable objectPartTable = GetObjectPartTable();
		if (objectPartTable != null)
		{
			return objectPartTable.GetBrokenBonusTypeForChaoAbility();
		}
		return BrokenBonusType.NONE;
	}

	public static void CreateBrokenBonus(GameObject broken_obj, GameObject playerObject, uint attackAttribute)
	{
		if (!IsAttackAttribute(attackAttribute, AttackAttribute.PhantomAsteroid) || !(broken_obj != null))
		{
			return;
		}
		BrokenBonusType brokenBonusType = GetGameModeBrokenBonusType();
		if (brokenBonusType == BrokenBonusType.NONE)
		{
			return;
		}
		LevelInformation levelInformation = GetLevelInformation();
		if (levelInformation != null && levelInformation.DestroyRingMode && brokenBonusType == BrokenBonusType.SUPER_RING)
		{
			brokenBonusType = BrokenBonusType.CRYSTAL10;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjBrokenBonus");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, broken_obj.transform.position, Quaternion.identity) as GameObject;
		if ((bool)gameObject2)
		{
			gameObject2.gameObject.SetActive(true);
			ObjBrokenBonus component = gameObject2.GetComponent<ObjBrokenBonus>();
			if ((bool)component)
			{
				component.Setup(brokenBonusType, playerObject);
			}
		}
	}

	public static void CreateBrokenBonusForChaoAbiilty(GameObject broken_obj, GameObject playerObject)
	{
		if (!(broken_obj != null))
		{
			return;
		}
		BrokenBonusType gameModeBrokenBonusTypeForChaoAbility = GetGameModeBrokenBonusTypeForChaoAbility();
		if (gameModeBrokenBonusTypeForChaoAbility == BrokenBonusType.NONE)
		{
			return;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjBrokenBonus");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, broken_obj.transform.position, Quaternion.identity) as GameObject;
		if ((bool)gameObject2)
		{
			gameObject2.gameObject.SetActive(true);
			ObjBrokenBonus component = gameObject2.GetComponent<ObjBrokenBonus>();
			if ((bool)component)
			{
				component.Setup(gameModeBrokenBonusTypeForChaoAbility, playerObject);
			}
		}
	}

	public static bool IsAttackAttribute(uint state, AttackAttribute attribute)
	{
		return ((int)state & (int)attribute) != 0;
	}

	public static PlayerInformation GetPlayerInformation()
	{
		return GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
	}

	public static LevelInformation GetLevelInformation()
	{
		return GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
	}

	public static bool IsUseTemporarySet()
	{
		return false;
	}

	public static int GetChaoAbliltyValue(ChaoAbility ability, int src_value)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if ((bool)instance)
		{
			return instance.GetChaoAbliltyValue(ability, src_value);
		}
		return src_value;
	}

	public static int GetChaoAbliltyScore(List<ChaoAbility> abilityList, int src_value)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if ((bool)instance)
		{
			return instance.GetChaoAndTeamAbliltyScoreValue(abilityList, TeamAttributeBonusType.NONE, src_value);
		}
		return src_value;
	}

	public static int GetChaoAndEnemyScore(List<ChaoAbility> abilityList, int src_value)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if ((bool)instance)
		{
			return instance.GetChaoAndEnemyScoreValue(abilityList, src_value);
		}
		return src_value;
	}

	public static void GetChaoAbliltyPhantomFlag(uint attribute_state, ref List<ChaoAbility> abilityList)
	{
		PhantomType phantomType = PhantomType.NONE;
		if (IsAttackAttribute(attribute_state, AttackAttribute.PhantomAsteroid))
		{
			phantomType = PhantomType.ASTEROID;
		}
		if (IsAttackAttribute(attribute_state, AttackAttribute.PhantomLaser))
		{
			phantomType = PhantomType.LASER;
		}
		if (IsAttackAttribute(attribute_state, AttackAttribute.PhantomDrill))
		{
			phantomType = PhantomType.DRILL;
		}
		if (phantomType != PhantomType.NONE)
		{
			GetChaoAbliltyPhantomFlag(phantomType, ref abilityList);
		}
	}

	public static void GetChaoAbliltyPhantomFlag(PlayerInformation playerInfo, ref List<ChaoAbility> abilityList)
	{
		if ((bool)playerInfo)
		{
			PhantomType phantomType = playerInfo.PhantomType;
			if (phantomType != PhantomType.NONE)
			{
				GetChaoAbliltyPhantomFlag(phantomType, ref abilityList);
			}
		}
	}

	public static void GetChaoAbliltyPhantomFlag(PhantomType type, ref List<ChaoAbility> abilityList)
	{
		if (type != PhantomType.NONE)
		{
			abilityList.Add(ChaoAbility.COLOR_POWER_SCORE);
			switch (type)
			{
			case PhantomType.ASTEROID:
				abilityList.Add(ChaoAbility.ASTEROID_SCORE);
				break;
			case PhantomType.LASER:
				abilityList.Add(ChaoAbility.LASER_SCORE);
				break;
			case PhantomType.DRILL:
				abilityList.Add(ChaoAbility.DRILL_SCORE);
				break;
			}
		}
	}

	public static bool RequestStartAbilityToChao(ChaoAbility ability, bool withEffect)
	{
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ability))
		{
			MsgChaoAbilityStart msgChaoAbilityStart = new MsgChaoAbilityStart(ability);
			GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgChaoAbilityStart", msgChaoAbilityStart, SendMessageOptions.DontRequireReceiver);
			if (withEffect)
			{
				StageAbilityManager instance = StageAbilityManager.Instance;
				if (instance != null)
				{
					instance.RequestPlayChaoEffect(ability);
				}
			}
			return msgChaoAbilityStart.m_flag;
		}
		return false;
	}

	public static void RequestStartAbilityToChao(ChaoAbility[] ability, bool withEffect)
	{
		MsgChaoAbilityStart value = new MsgChaoAbilityStart(ability);
		GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgChaoAbilityStart", value, SendMessageOptions.DontRequireReceiver);
		if (withEffect)
		{
			StageAbilityManager instance = StageAbilityManager.Instance;
			if (instance != null)
			{
				StageAbilityManager.Instance.RequestPlayChaoEffect(ability);
			}
		}
	}

	public static void RequestEndAbilityToChao(ChaoAbility ability)
	{
		MsgChaoAbilityEnd value = new MsgChaoAbilityEnd(ability);
		GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgChaoAbilityEnd", value, SendMessageOptions.DontRequireReceiver);
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			instance.RequestStopChaoEffect(ability);
		}
	}

	public static void RequestEndAbilityToChao(ChaoAbility[] ability)
	{
		MsgChaoAbilityEnd value = new MsgChaoAbilityEnd(ability);
		GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgChaoAbilityEnd", value, SendMessageOptions.DontRequireReceiver);
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			instance.RequestStopChaoEffect(ability);
		}
	}

	public static void PushCamera(CameraType type, float interpolateTime)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if ((bool)gameObject)
		{
			CameraManager component = gameObject.GetComponent<CameraManager>();
			if ((bool)component)
			{
				MsgPushCamera value = new MsgPushCamera(type, interpolateTime);
				component.SendMessage("OnPushCamera", value);
			}
		}
	}

	public static void PopCamera(CameraType type, float interpolateTime)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if ((bool)gameObject)
		{
			CameraManager component = gameObject.GetComponent<CameraManager>();
			if ((bool)component)
			{
				MsgPopCamera value = new MsgPopCamera(type, interpolateTime);
				component.SendMessage("OnPopCamera", value);
			}
		}
	}

	public static void SetTextureAnimationSpeed(GameObject obj, float speed)
	{
		if ((bool)obj)
		{
			ykTextureSheetAnimation component = obj.GetComponent<ykTextureSheetAnimation>();
			if ((bool)component)
			{
				component.SetSpeed(speed);
			}
		}
	}

	public static void CreatePrism()
	{
		StageBlockPathManager stageBlockPathManager = GameObjectUtil.FindGameObjectComponent<StageBlockPathManager>("StageBlockManager");
		if (!stageBlockPathManager)
		{
			return;
		}
		float? distance = null;
		PathComponent curentLaserPath = stageBlockPathManager.GetCurentLaserPath(ref distance);
		if (!(curentLaserPath != null))
		{
			return;
		}
		ResPathObject resPathObject = curentLaserPath.GetResPathObject();
		if (resPathObject == null || resPathObject.NumKeys <= 0)
		{
			return;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjPrism");
		if (!(gameObject != null))
		{
			return;
		}
		float num = 0f;
		PlayerInformation playerInformation = GetPlayerInformation();
		if ((bool)playerInformation)
		{
			Vector3 position = playerInformation.Position;
			num = position.x;
		}
		for (int i = 0; i < resPathObject.NumKeys; i++)
		{
			Vector3 position2 = resPathObject.GetPosition(i);
			if (!(position2.x > num))
			{
				continue;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, position2, Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(true);
				SpawnableObject component = gameObject2.GetComponent<SpawnableObject>();
				if ((bool)component)
				{
					component.AttachModelObject();
				}
			}
		}
	}

	public static void SendStartItemAndChao()
	{
		if (StageItemManager.Instance != null)
		{
			MsgUseEquipItem msg = new MsgUseEquipItem();
			StageItemManager.Instance.OnUseEquipItem(msg);
		}
		RequestStartAbilityToChao(ChaoAbility.ADD_COMBO_VALUE, false);
		RequestStartAbilityToChao(ChaoAbility.COMBO_RECEPTION_TIME, false);
		RequestStartAbilityToChao(ChaoAbility.ANIMAL_COUNT, false);
		RequestStartAbilityToChao(ChaoAbility.COMBO_BONUS_UP, false);
		RequestStartAbilityToChao(ChaoAbility.SUPER_RING_UP, false);
	}

	public static bool IsLightMode()
	{
		LevelInformation levelInformation = GetLevelInformation();
		if (levelInformation != null)
		{
			return levelInformation.LightMode;
		}
		return false;
	}

	public static void SendGetItemIcon(ItemType type)
	{
		HudCaution.Instance.SetCaution(new MsgCaution(HudCaution.Type.GET_ITEM, type));
	}

	public static void SendGetTimerIcon(int number, int second)
	{
		HudCaution.Instance.SetCaution(new MsgCaution(HudCaution.Type.GET_TIMER, number, second));
	}

	public static void PauseCombo(MsgPauseComboTimer.State value, float time = -1f)
	{
		if (StageComboManager.Instance != null)
		{
			MsgPauseComboTimer value2 = new MsgPauseComboTimer(value, time);
			GameObjectUtil.SendDelayedMessageToGameObject(StageComboManager.Instance.gameObject, "OnMsgPauseComboTimer", value2);
		}
	}

	public static void StopCombo()
	{
		if (StageComboManager.Instance != null)
		{
			MsgStopCombo value = new MsgStopCombo();
			GameObjectUtil.SendDelayedMessageToGameObject(StageComboManager.Instance.gameObject, "OnMsgStopCombo", value);
		}
	}

	public static void SetDisableEquipItem(bool disable)
	{
		if (StageItemManager.Instance != null)
		{
			StageItemManager.Instance.OnDisableEquipItem(new MsgDisableEquipItem(disable));
		}
	}

	public static void SetQuickModeTimePause(bool pauseFlag)
	{
		if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode() && StageTimeManager.Instance != null)
		{
			if (pauseFlag)
			{
				StageTimeManager.Instance.Pause();
			}
			else
			{
				StageTimeManager.Instance.Resume();
			}
		}
	}

	public static void CreateSharedMateriaDummyObject(ResourceCategory category, string name)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(category, name);
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, new Vector3(-100f, -100f, 0f), Quaternion.identity) as GameObject;
		if (!(gameObject2 != null))
		{
			return;
		}
		ykTextureSheetSharedMaterialAnimation component = gameObject2.GetComponent<ykTextureSheetSharedMaterialAnimation>();
		if ((bool)component)
		{
			if (component.enabled)
			{
			}
			component.enabled = true;
		}
		gameObject2.SetActive(true);
	}

	private static string GetRingToSuperRing(ResourceManager resourceManager, string objName)
	{
		StageComboManager instance = StageComboManager.Instance;
		if (instance != null && instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_SUPER_RING) && objName == "ObjRing")
		{
			return "ObjSuperRing";
		}
		return objName;
	}

	private static string GetAllRingToCrystal(ResourceManager resourceManager, LevelInformation levelInfo, string objName)
	{
		if (levelInfo != null && !levelInfo.NowFeverBoss && levelInfo.DestroyRingMode)
		{
			if (objName == "ObjRing")
			{
				return ObjCrystalData.GetCrystalObjectName(CtystalType.SMALL_A);
			}
			if (objName == "ObjSuperRing")
			{
				return ObjCrystalData.GetCrystalObjectName(CtystalType.BIG_A);
			}
		}
		return objName;
	}

	private static string GetEventCrystal(ResourceManager resourceManager, PlayerInformation playerInfo, string objName)
	{
		if (EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.QUICK && StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
		{
			EventObjectTableItem eventObjectTableItem = EventObjectTable.GetEventObjectTableItem(objName);
			if (eventObjectTableItem != EventObjectTableItem.NONE && playerInfo != null)
			{
				int num = EventObjectTable.GetItemData((int)playerInfo.SpeedLevel, eventObjectTableItem);
				if (EventObjectTable.IsCyrstal(eventObjectTableItem))
				{
					StageComboManager instance = StageComboManager.Instance;
					num = ((!(instance != null) || !instance.IsActiveComboChaoAbility(ChaoAbility.COMBO_ALL_SPECIAL_CRYSTAL)) ? GetChaoAbliltyValue(ChaoAbility.SPECIAL_CRYSTAL_RATE, num) : 100);
				}
				if (GetRandomRange100() < num)
				{
					string result = "ObjEventCrystal";
					if (EventObjectTable.IsEventCrystalBig(eventObjectTableItem))
					{
						result = "ObjEventCrystal10";
					}
					return result;
				}
			}
		}
		return objName;
	}

	public static GameObject GetChangeObject(ResourceManager resourceManager, PlayerInformation playerInfo, LevelInformation levelInfo, string objName)
	{
		string ringToSuperRing = GetRingToSuperRing(resourceManager, objName);
		string allRingToCrystal = GetAllRingToCrystal(resourceManager, levelInfo, ringToSuperRing);
		string eventCrystal = GetEventCrystal(resourceManager, playerInfo, allRingToCrystal);
		if (eventCrystal != objName)
		{
			return resourceManager.GetSpawnableGameObject(eventCrystal);
		}
		return null;
	}

	public static GameObject GetCrystalChangeObject(ResourceManager resourceManager, GameObject srcObj)
	{
		if (srcObj != null)
		{
			SpawnableObject component = srcObj.GetComponent<SpawnableObject>();
			if (component == null)
			{
				return null;
			}
			CtystalType ctystalType = CtystalType.NONE;
			switch (component.GetStockObjectType())
			{
			case StockObjectType.CrystalS_A:
				ctystalType = CtystalType.SMALL_A;
				break;
			case StockObjectType.CrystalS_B:
				ctystalType = CtystalType.SMALL_B;
				break;
			case StockObjectType.CrystalS_C:
				ctystalType = CtystalType.SMALL_C;
				break;
			case StockObjectType.CrystalB_A:
				ctystalType = CtystalType.BIG_A;
				break;
			case StockObjectType.CrystalB_B:
				ctystalType = CtystalType.BIG_B;
				break;
			case StockObjectType.CrystalB_C:
				ctystalType = CtystalType.BIG_C;
				break;
			default:
				return null;
			}
			CtystalType crystalModelType = ObjCrystalUtil.GetCrystalModelType(ctystalType);
			if (crystalModelType != ctystalType)
			{
				return resourceManager.GetSpawnableGameObject(ObjCrystalData.GetCrystalObjectName(crystalModelType));
			}
		}
		return null;
	}

	public static void SetHudStockRingEffectOff(bool off)
	{
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnMsgStockRingEffect", new MsgHudStockRingEffect(off), SendMessageOptions.DontRequireReceiver);
	}
}
