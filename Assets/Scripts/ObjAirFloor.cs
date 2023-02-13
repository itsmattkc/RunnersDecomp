using GameScore;
using Message;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjFloor")]
public class ObjAirFloor : SpawnableObject
{
	private GameObject m_modelObject;

	private static readonly string[] FLOOR_TYPENAME = new string[3]
	{
		"1m",
		"2m",
		"3m"
	};

	private static readonly float[] FLOOR_TYPESIZE = new float[3]
	{
		1f,
		2f,
		3f
	};

	private static readonly string[] FLOOR_EFFNAME = new string[3]
	{
		"ef_com_explosion_m01",
		"ef_com_explosion_m01",
		"ef_com_explosion_l01"
	};

	private static Vector3 COLLI_CENTER = new Vector3(0f, -0.15f, 0f);

	private static Vector3 COLLI_SIZE = new Vector3(0f, 0.4f, 3f);

	private int m_type_index;

	private bool m_end;

	public GameObject ModelObject
	{
		get
		{
			return m_modelObject;
		}
		set
		{
			if (m_modelObject == null)
			{
				m_modelObject = value;
				m_modelObject.SetActive(true);
				m_modelObject.isStatic = true;
			}
		}
	}

	protected override void OnSpawned()
	{
		base.enabled = false;
	}

	public void Setup(string name)
	{
		for (int i = 0; i < FLOOR_TYPENAME.Length; i++)
		{
			if (name.IndexOf(FLOOR_TYPENAME[i]) != -1)
			{
				m_type_index = i;
				break;
			}
		}
		if (m_type_index < FLOOR_TYPESIZE.Length)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if ((bool)component)
			{
				component.center = COLLI_CENTER;
				component.size = new Vector3(FLOOR_TYPESIZE[m_type_index], COLLI_SIZE.y, COLLI_SIZE.z);
			}
		}
	}

	public void OnMsgStepObjectDead(MsgObjectDead msg)
	{
		if (!m_end)
		{
			GameObject gameObject = GameObject.FindWithTag("Player");
			if (gameObject != null)
			{
				ObjUtil.CreateBrokenBonusForChaoAbiilty(base.gameObject, gameObject);
			}
			SetBroken();
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (!m_end && !ObjUtil.IsAttackAttribute(msg.m_attackAttribute, AttackAttribute.Invincible) && msg.m_attackPower >= 4 && (bool)msg.m_sender)
		{
			GameObject gameObject = msg.m_sender.gameObject;
			if ((bool)gameObject)
			{
				MsgHitDamageSucceed value = new MsgHitDamageSucceed(base.gameObject, 0, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation);
				gameObject.SendMessage("OnDamageSucceed", value, SendMessageOptions.DontRequireReceiver);
				SetPlayerBroken(msg.m_attackAttribute);
				ObjUtil.CreateBrokenBonus(base.gameObject, gameObject, msg.m_attackAttribute);
			}
		}
	}

	private void SetPlayerBroken(uint attribute_state)
	{
		if (!m_end)
		{
			int airFloor = Data.AirFloor;
			List<ChaoAbility> abilityList = new List<ChaoAbility>();
			ObjUtil.GetChaoAbliltyPhantomFlag(attribute_state, ref abilityList);
			airFloor = ObjUtil.GetChaoAndEnemyScore(abilityList, airFloor);
			ObjUtil.SendMessageAddScore(airFloor);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(1, airFloor));
			SetBroken();
		}
	}

	private void SetBroken()
	{
		if (!m_end)
		{
			m_end = true;
			if (m_type_index < FLOOR_EFFNAME.Length)
			{
				ObjUtil.PlayEffectCollisionCenter(base.gameObject, FLOOR_EFFNAME[m_type_index], 1f);
			}
			ObjUtil.LightPlaySE("obj_brk");
			Object.Destroy(base.gameObject);
		}
	}
}
