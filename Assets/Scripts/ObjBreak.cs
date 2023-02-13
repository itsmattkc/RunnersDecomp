using GameScore;
using Message;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjBreak")]
public class ObjBreak : SpawnableObject
{
	private class BreakParam
	{
		public float m_add_x;

		public float m_add_y;

		public float m_rot_speed;

		public BreakParam(float add_x, float add_y, float rot_speed)
		{
			m_add_x = add_x;
			m_add_y = add_y;
			m_rot_speed = rot_speed;
		}
	}

	private const float ADD_SPEED = 0.8f;

	private const float END_TIME = 2.5f;

	private const float BREAK_SPEED = 6f;

	private const float BREAK_GRAVITY = -6.1f;

	private GameObject m_modelObject;

	private static readonly Vector3[] MODEL_ROT_TBL = new Vector3[4]
	{
		new Vector3(1f, 1f, 0f),
		new Vector3(0f, 1f, 1f),
		new Vector3(1f, 0f, 1f),
		new Vector3(1f, 1f, 0f)
	};

	private static readonly BreakParam[] BREAK_PARAM = new BreakParam[4]
	{
		new BreakParam(0.8f, 1.2f, 15f),
		new BreakParam(1.5f, 1.4f, 5f),
		new BreakParam(1f, 1f, 10f),
		new BreakParam(0.7f, 1.6f, 2f)
	};

	private BreakParam[] m_breakParam;

	private bool m_break;

	private uint m_model_count;

	private float m_time;

	private float m_move_speed;

	private GameObject m_break_obj;

	private bool m_setup;

	private string m_setup_name = string.Empty;

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
			}
		}
	}

	protected override void OnSpawned()
	{
		float num = ObjUtil.GetPlayerAddSpeed();
		if (num < 0f)
		{
			num = 0f;
		}
		m_move_speed = 0.8f * num;
	}

	private void Update()
	{
		if (!m_setup)
		{
			m_setup = Setup(m_setup_name);
		}
		if (m_break)
		{
			m_time += Time.deltaTime;
			if (m_time > 2.5f)
			{
				m_break = false;
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void SetObjName(string name)
	{
		m_setup_name = name;
	}

	private bool Setup(string name)
	{
		if (m_break_obj != null)
		{
			return true;
		}
		m_break_obj = CreateBreakModel(base.gameObject, name);
		if (m_break_obj != null)
		{
			BreakModelVisible(false);
			m_model_count = 0u;
			if ((bool)m_break_obj)
			{
				Component[] componentsInChildren = m_break_obj.GetComponentsInChildren<MeshRenderer>(true);
				m_model_count = (uint)componentsInChildren.Length;
			}
			if (m_model_count >= MODEL_ROT_TBL.Length)
			{
				m_model_count = (uint)MODEL_ROT_TBL.Length;
			}
			if (m_model_count >= BREAK_PARAM.Length)
			{
				m_model_count = (uint)BREAK_PARAM.Length;
			}
			m_breakParam = new BreakParam[m_model_count];
			int num = Random.Range(0, BREAK_PARAM.Length - 1);
			for (int i = 0; i < m_model_count; i++)
			{
				int num2 = num + i;
				if (num2 >= BREAK_PARAM.Length)
				{
					num2 -= BREAK_PARAM.Length;
				}
				m_breakParam[i] = new BreakParam(BREAK_PARAM[num2].m_add_x + m_move_speed, BREAK_PARAM[num2].m_add_y + m_move_speed, BREAK_PARAM[num2].m_rot_speed);
			}
			return true;
		}
		return false;
	}

	public void OnMsgObjectDead(MsgObjectDead msg)
	{
		if (base.enabled)
		{
			SetBroken();
		}
	}

	public void OnMsgStepObjectDead(MsgObjectDead msg)
	{
		if (base.enabled)
		{
			SetBroken();
		}
	}

	private void SetPlayerBroken(uint attribute_state)
	{
		int @break = Data.Break;
		List<ChaoAbility> abilityList = new List<ChaoAbility>();
		ObjUtil.GetChaoAbliltyPhantomFlag(attribute_state, ref abilityList);
		@break = ObjUtil.GetChaoAndEnemyScore(abilityList, @break);
		ObjUtil.SendMessageAddScore(@break);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(1, @break));
		SetBroken();
	}

	private void SetBroken()
	{
		if (!m_break && (bool)m_break_obj && (bool)m_modelObject)
		{
			RootModelVisible(false);
			BreakModelVisible(true);
			BreakStart();
			ObjUtil.PlayEffectCollisionCenter(base.gameObject, "ef_com_explosion_m01", 1f);
			ObjUtil.LightPlaySE("obj_brk");
			m_break = true;
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (!m_break && msg.m_attackPower >= 3 && (bool)msg.m_sender)
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

	private static GameObject CreateBreakModel(GameObject baseObj, string in_name)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.STAGE_RESOURCE, in_name + "_brk");
		GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "MotorThrow");
		if (gameObject != null && gameObject2 != null)
		{
			GameObject gameObject3 = Object.Instantiate(gameObject, baseObj.transform.position, baseObj.transform.rotation) as GameObject;
			if ((bool)gameObject3)
			{
				gameObject3.gameObject.SetActive(true);
				gameObject3.transform.parent = baseObj.transform;
				for (int i = 0; i < gameObject3.transform.childCount; i++)
				{
					GameObject gameObject4 = gameObject3.transform.GetChild(i).gameObject;
					if ((bool)gameObject4)
					{
						GameObject gameObject5 = Object.Instantiate(gameObject2, baseObj.transform.position, baseObj.transform.rotation) as GameObject;
						if ((bool)gameObject5)
						{
							gameObject5.gameObject.SetActive(true);
							gameObject5.transform.parent = gameObject4.transform;
						}
					}
				}
				return gameObject3;
			}
		}
		return null;
	}

	private void BreakModelVisible(bool flag)
	{
		if ((bool)m_break_obj)
		{
			MeshRenderer component = m_break_obj.GetComponent<MeshRenderer>();
			if ((bool)component)
			{
				component.enabled = flag;
			}
			Component[] componentsInChildren = m_break_obj.GetComponentsInChildren<MeshRenderer>(true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshRenderer meshRenderer = (MeshRenderer)array[i];
				meshRenderer.enabled = flag;
			}
		}
	}

	private void RootModelVisible(bool flag)
	{
		if (!m_modelObject)
		{
			return;
		}
		Component[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer meshRenderer = (MeshRenderer)array[i];
			meshRenderer.enabled = flag;
			BoxCollider[] componentsInChildren2 = meshRenderer.gameObject.GetComponentsInChildren<BoxCollider>(true);
			BoxCollider[] array2 = componentsInChildren2;
			foreach (BoxCollider boxCollider in array2)
			{
				boxCollider.enabled = flag;
			}
		}
	}

	private void BreakStart()
	{
		if (!m_break_obj)
		{
			return;
		}
		for (int i = 0; i < m_break_obj.transform.childCount; i++)
		{
			GameObject gameObject = m_break_obj.transform.GetChild(i).gameObject;
			if (!gameObject)
			{
				continue;
			}
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
				if ((bool)gameObject2)
				{
					MotorThrow component = gameObject2.GetComponent<MotorThrow>();
					if ((bool)component && i < m_breakParam.Length)
					{
						MotorThrow.ThrowParam throwParam = new MotorThrow.ThrowParam();
						throwParam.m_obj = gameObject;
						throwParam.m_speed = 6f;
						throwParam.m_gravity = -6.1f;
						throwParam.m_add_x = m_breakParam[i].m_add_x;
						throwParam.m_add_y = m_breakParam[i].m_add_y;
						throwParam.m_rot_speed = m_breakParam[i].m_rot_speed;
						throwParam.m_up = base.transform.up;
						throwParam.m_forward = base.transform.right;
						throwParam.m_rot_angle = MODEL_ROT_TBL[i];
						component.Setup(throwParam);
						break;
					}
				}
			}
		}
	}
}
