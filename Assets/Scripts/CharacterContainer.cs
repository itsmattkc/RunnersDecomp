using Message;
using Player;
using UnityEngine;

public class CharacterContainer : MonoBehaviour
{
	private enum Type
	{
		MAIN,
		SUB
	}

	private int m_numCurrent;

	private bool[] m_recovered = new bool[2];

	private bool m_btnCharaChange;

	private bool m_enableChange;

	private bool m_requestChange;

	private GameObject[] m_character;

	private PlayerInformation m_playerInformation;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_requestChange)
		{
			int current = (m_numCurrent == 0) ? 1 : 0;
			ChangeCurrentCharacter(current, !m_enableChange);
			MsgChangeCharaSucceed msgChangeCharaSucceed = new MsgChangeCharaSucceed();
			msgChangeCharaSucceed.m_disabled = !m_enableChange;
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeCharaSucceed", msgChangeCharaSucceed, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnChangeCharaSucceed", msgChangeCharaSucceed, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageFindGameObject("StageComboManager", "OnChangeCharaSucceed", msgChangeCharaSucceed, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendDelayedMessageToTagObjects("Boss", "OnChangeCharaSucceed", msgChangeCharaSucceed);
			if (StageItemManager.Instance != null)
			{
				StageItemManager.Instance.OnChangeCharaStart(msgChangeCharaSucceed);
			}
			GameObjectUtil.SendDelayedMessageFindGameObject("StageItemManager", "OnChangeCharaSucceed", msgChangeCharaSucceed);
			m_requestChange = false;
		}
	}

	public void Init()
	{
		m_playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		m_character = new GameObject[2];
		m_character[0] = GameObjectUtil.FindChildGameObject(base.gameObject, "PlayerCharacterMain");
		m_character[1] = GameObjectUtil.FindChildGameObject(base.gameObject, "PlayerCharacterSub");
		if (m_playerInformation != null && m_playerInformation.SubCharacterName == null)
		{
			m_character[1] = null;
		}
		m_enableChange = (m_character[1] != null);
		m_requestChange = false;
		for (int i = 0; i < 2; i++)
		{
			m_recovered[i] = false;
		}
		m_btnCharaChange = false;
		MsgChangeCharaEnable value = new MsgChangeCharaEnable(m_enableChange);
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeCharaEnable", value, SendMessageOptions.DontRequireReceiver);
		m_numCurrent = 0;
	}

	public void SetupCharacter()
	{
		int num = 0;
		GameObject[] character = m_character;
		foreach (GameObject gameObject in character)
		{
			if (gameObject != null)
			{
				CharacterState component = gameObject.GetComponent<CharacterState>();
				if (component != null)
				{
					component.SetPlayingType((PlayingCharacterType)num);
					component.SetupModelsAndParameter();
				}
			}
			num++;
		}
	}

	private GameObject GetCurrentCharacter()
	{
		return m_character[m_numCurrent];
	}

	private GameObject GetNonCurrentCharacter()
	{
		int num = (m_numCurrent == 0) ? 1 : 0;
		return m_character[num];
	}

	private void OnMsgChangeChara(MsgChangeChara msg)
	{
		GameObject currentCharacter = GetCurrentCharacter();
		if (currentCharacter != null && m_enableChange && !m_requestChange && currentCharacter.GetComponent<CharacterState>().IsEnableCharaChange(msg.m_changeByMiss))
		{
			m_requestChange = true;
			if (msg.m_changeByMiss)
			{
				m_enableChange = false;
			}
			if (msg.m_changeByBtn)
			{
				m_btnCharaChange = true;
			}
			msg.m_succeed = true;
		}
	}

	private void ChangeCurrentCharacter(int current, bool dead)
	{
		GameObject currentCharacter = GetCurrentCharacter();
		m_numCurrent = current;
		for (int i = 0; i < m_character.Length; i++)
		{
			if (i != m_numCurrent)
			{
				DeactiveCharacter(i);
			}
		}
		for (int j = 0; j < m_character.Length; j++)
		{
			if (j == m_numCurrent)
			{
				ActivateCharacter(j, currentCharacter.transform.position, currentCharacter.transform.rotation, dead, true);
			}
		}
		if (dead)
		{
			MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.STOP_END);
		}
	}

	private void ActivateCharacter(int numPlayer, Vector3 plrPos, Quaternion plrRot, bool dead, bool trampoline)
	{
		Vector3 vector = plrPos;
		Vector3 sideViewPathPos = m_playerInformation.SideViewPathPos;
		Vector3 sideViewPathNormal = m_playerInformation.SideViewPathNormal;
		Vector3 lhs = plrPos - sideViewPathPos;
		if (Vector3.Dot(lhs, sideViewPathNormal) < 0f)
		{
			plrPos = sideViewPathPos + Vector3.up * 1f;
			plrRot = Quaternion.Euler(new Vector3(0f, 90f, 0f));
		}
		Debug.Log("CharaChange Diactive POS: " + vector.x + " " + vector.y + " " + vector.z);
		Debug.Log("CharaChange Active   POS: " + plrPos.x + " " + plrPos.y + " " + plrPos.z);
		m_character[numPlayer].GetComponent<CharacterState>().ActiveCharacter(dead, dead, plrPos, plrRot);
		m_character[numPlayer].SetActive(true);
		if (trampoline)
		{
			ObjUtil.SendMessageAppearTrampoline();
		}
		if (!m_btnCharaChange)
		{
			ObjUtil.SendMessageOnObjectDead();
		}
		m_btnCharaChange = false;
	}

	private void DeactiveCharacter(int numPlayer)
	{
		m_character[numPlayer].GetComponent<CharacterState>().DeactiveCharacter();
		m_character[numPlayer].SetActive(false);
	}

	private bool IsNowLastChance(int numPlayer)
	{
		return m_character[numPlayer].GetComponent<CharacterState>().IsNowLastChance();
	}

	private void SetResetCharacterLastChance(int numPlayer)
	{
		m_character[numPlayer].GetComponent<CharacterState>().SetLastChance(false);
		if (m_playerInformation != null)
		{
			m_playerInformation.SetLastChance(false);
		}
	}

	private void SetResetCharacterDead()
	{
		if (m_playerInformation != null)
		{
			m_playerInformation.SetDead(false);
		}
	}

	public bool IsEnableChangeCharacter()
	{
		return m_enableChange;
	}

	public bool IsEnableRecovery()
	{
		if (!m_recovered[m_numCurrent] && StageAbilityManager.Instance != null)
		{
			return StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.RECOVERY_FROM_FAILURE);
		}
		return false;
	}

	public void PrepareRecovery(bool bossStage)
	{
		m_requestChange = false;
		m_recovered[m_numCurrent] = true;
		DeactiveCharacter(m_numCurrent);
		SetResetCharacterDead();
		ActivateCharacter(m_numCurrent, m_playerInformation.Position, m_playerInformation.Rotation, true, false);
		ObjUtil.SendMessageAppearTrampoline();
		MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.STOP_END);
		if (bossStage)
		{
			MsgPrepareContinue value = new MsgPrepareContinue(bossStage, false);
			GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgPrepareContinue", value, SendMessageOptions.DontRequireReceiver);
		}
		ObjUtil.RequestStartAbilityToChao(ChaoAbility.RECOVERY_FROM_FAILURE, false);
	}

	private void OnMsgPrepareContinue(MsgPrepareContinue msg)
	{
		bool flag = IsNowLastChance(m_numCurrent);
		ItemType itemType = ItemType.UNKNOWN;
		if (StageItemManager.Instance != null)
		{
			itemType = StageItemManager.Instance.GetPhantomItemType();
		}
		bool flag2 = msg.m_timeUp && itemType != ItemType.UNKNOWN && flag;
		m_enableChange = (m_character[1] != null);
		m_requestChange = false;
		for (int i = 0; i < 2; i++)
		{
			m_recovered[i] = false;
		}
		if (!flag2)
		{
			DeactiveCharacter(m_numCurrent);
		}
		SetResetCharacterLastChance(m_numCurrent);
		SetResetCharacterDead();
		if (msg.m_timeUp)
		{
			if (flag)
			{
				MsgEndLastChance value = new MsgEndLastChance();
				GameObjectUtil.SendMessageToTagObjects("Chao", "OnEndLastChance", value, SendMessageOptions.DontRequireReceiver);
				if (m_enableChange && m_numCurrent == 1)
				{
					m_numCurrent = 0;
				}
			}
		}
		else if (m_enableChange && m_numCurrent == 1)
		{
			m_numCurrent = 0;
		}
		if (flag2)
		{
			ObjUtil.SendMessageOnObjectDead();
		}
		else
		{
			ActivateCharacter(m_numCurrent, m_playerInformation.Position, m_playerInformation.Rotation, true, false);
		}
		bool flag3 = false;
		if (EventManager.Instance != null)
		{
			flag3 = EventManager.Instance.IsRaidBossStage();
		}
		StageScoreManager instance = StageScoreManager.Instance;
		if (instance != null)
		{
			ObjUtil.SendMessageTransferRingForContinue((!flag3) ? instance.ContinueRing : instance.ContinueRaidBossRing);
		}
		int numRings = m_playerInformation.NumRings;
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnAddStockRing", new MsgAddStockRing(numRings), SendMessageOptions.DontRequireReceiver);
		if (!msg.m_bossStage)
		{
			SendAddItem(ItemType.BARRIER);
			SendAddItem(ItemType.MAGNET);
			SendAddItem(ItemType.COMBO);
			SendAddDamageTrampoline();
			if (itemType == ItemType.UNKNOWN)
			{
				itemType = StageItemManager.GetRandomPhantomItem();
			}
			if (!SendAddItem(itemType))
			{
				SendAddColorItem(itemType);
				MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.STOP_END);
			}
		}
		else
		{
			SendAddItem(ItemType.INVINCIBLE);
			SendAddItem(ItemType.BARRIER);
			SendAddItem(ItemType.MAGNET);
			MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.STOP_END);
		}
		if (m_enableChange)
		{
			MsgChangeCharaEnable value2 = new MsgChangeCharaEnable(true);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeCharaEnable", value2, SendMessageOptions.DontRequireReceiver);
			MsgChangeCharaSucceed value3 = new MsgChangeCharaSucceed();
			GameObjectUtil.SendMessageFindGameObject("StageComboManager", "OnChangeCharaSucceed", value3, SendMessageOptions.DontRequireReceiver);
		}
	}

	private bool SendAddItem(ItemType itemType)
	{
		if (StageItemManager.Instance != null)
		{
			MsgAskEquipItemUsed msgAskEquipItemUsed = new MsgAskEquipItemUsed(itemType);
			StageItemManager.Instance.OnAskEquipItemUsed(msgAskEquipItemUsed);
			if (msgAskEquipItemUsed.m_ok)
			{
				StageItemManager.Instance.OnAddItem(new MsgAddItemToManager(itemType));
				return true;
			}
		}
		return false;
	}

	private void SendAddColorItem(ItemType itemType)
	{
		if (StageItemManager.Instance != null)
		{
			StageItemManager.Instance.OnAddColorItem(new MsgAddItemToManager(itemType));
		}
	}

	private void SendAddDamageTrampoline()
	{
		if (StageItemManager.Instance != null)
		{
			StageItemManager.Instance.OnAddDamageTrampoline();
		}
	}
}
