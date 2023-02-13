using Message;
using Player;
using System;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjCannon")]
public class ObjCannon : SpawnableObject
{
	private enum State
	{
		Start,
		Idle,
		Wait,
		Set,
		Shot
	}

	private const string ModelName = "obj_cmn_cannon";

	private const float m_roundParam = 5f;

	private float m_firstSpeed = 5f;

	private float m_outOfcontrol = 0.5f;

	private GameObject m_sendObject;

	private float m_moveSpeed = 0.4f;

	private float m_rotArea = 25f;

	private float m_time;

	private bool m_shot;

	private Quaternion m_centerRotation = Quaternion.identity;

	private Quaternion m_startRotation = Quaternion.identity;

	private CharacterInput m_input;

	private CameraManager m_camera;

	private State m_state;

	protected override string GetModelName()
	{
		return "obj_cmn_cannon";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected virtual string GetShotEffectName()
	{
		return "ef_ob_canon_st01";
	}

	protected virtual string GetShotAnimName()
	{
		return "obj_cannon_shot";
	}

	protected virtual bool IsRoulette()
	{
		return false;
	}

	protected override void OnSpawned()
	{
		m_input = GetComponent<CharacterInput>();
		ObjUtil.StopAnimation(base.gameObject);
		m_centerRotation = Quaternion.Euler(0f, 0f, -45f) * base.transform.rotation;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		switch (m_state)
		{
		case State.Start:
			SetGuideline(false);
			m_state = State.Idle;
			break;
		case State.Set:
			UpdateStart(deltaTime);
			break;
		case State.Shot:
			UpdateMove(deltaTime);
			break;
		}
		UpdateInputShot();
	}

	public void SetObjCannonParameter(ObjCannonParameter param)
	{
		m_firstSpeed = param.firstSpeed;
		m_outOfcontrol = param.outOfcontrol;
		m_moveSpeed = param.moveSpeed;
		m_rotArea = param.moveArea * 0.5f;
	}

	protected virtual Quaternion GetStartRot()
	{
		return Quaternion.Euler(0f, 0f, 0f - m_rotArea) * m_centerRotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_state != State.Idle)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if ((bool)gameObject)
		{
			Quaternion rot = Quaternion.FromToRotation(base.transform.up, base.transform.right) * base.transform.rotation;
			MsgOnAbidePlayer msgOnAbidePlayer = new MsgOnAbidePlayer(base.transform.position, rot, 1f, base.gameObject);
			gameObject.SendMessage("OnAbidePlayer", msgOnAbidePlayer, SendMessageOptions.DontRequireReceiver);
			if (msgOnAbidePlayer.m_succeed)
			{
				ObjUtil.PlaySE("obj_cannon_in");
				m_sendObject = gameObject;
				m_state = State.Wait;
			}
		}
	}

	private void UpdateStart(float delta)
	{
		m_time += delta;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, m_startRotation, m_time * m_moveSpeed * 0.5f * Time.timeScale);
		float num = Mathf.Abs(Vector3.Distance(base.transform.rotation.eulerAngles, m_startRotation.eulerAngles));
		if (num < 0.1f)
		{
			m_time = 0f;
			m_state = State.Shot;
		}
	}

	private void UpdateMove(float delta)
	{
		if (m_rotArea > 0f)
		{
			m_time += delta;
			float num = Mathf.Cos((float)Math.PI * 2f * m_time * m_moveSpeed) * -1f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num * m_rotArea) * m_centerRotation;
		}
	}

	private void UpdateInputShot()
	{
		if (m_shot && (bool)m_input && m_input.IsTouched())
		{
			Shot();
			SetGuideline(false);
			m_shot = false;
			m_state = State.Idle;
		}
	}

	private void Shot()
	{
		if ((bool)m_sendObject)
		{
			Quaternion rot = Quaternion.FromToRotation(base.transform.up, base.transform.right) * base.transform.rotation;
			if (!IsRoulette())
			{
				Vector3 eulerAngles = rot.eulerAngles;
				float num = eulerAngles.z + 2.5f;
				if (num != 0f)
				{
					int num2 = (int)(num / 5f);
					Vector3 eulerAngles2 = rot.eulerAngles;
					float x = eulerAngles2.x;
					Vector3 eulerAngles3 = rot.eulerAngles;
					rot = Quaternion.Euler(x, eulerAngles3.y, 5f * (float)num2);
				}
			}
			MsgOnCannonImpulse value = new MsgOnCannonImpulse(base.transform.position, rot, m_firstSpeed, m_outOfcontrol, IsRoulette());
			m_sendObject.SendMessage("OnCannonImpulse", value, SendMessageOptions.DontRequireReceiver);
			Animation componentInChildren = GetComponentInChildren<Animation>();
			if ((bool)componentInChildren)
			{
				componentInChildren.wrapMode = WrapMode.Once;
				componentInChildren.Play(GetShotAnimName());
			}
			ObjUtil.PlayEffectChild(base.gameObject, GetShotEffectName(), new Vector3(1f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, -90f)), 1f);
			ObjUtil.PlaySE("obj_cannon_shoot");
		}
		ObjUtil.PopCamera(CameraType.CANNON, 0.5f);
	}

	private void OnAbidePlayerLocked(MsgOnAbidePlayerLocked msg)
	{
		SetGuideline(true);
		m_startRotation = GetStartRot();
		m_state = State.Set;
		m_shot = true;
		ObjUtil.PushCamera(CameraType.CANNON, 0.5f);
	}

	private void OnExitAbideObject(MsgOnExitAbideObject msg)
	{
		m_state = State.Idle;
	}

	private void SetGuideline(bool on)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "obj_cmn_cannonguideline");
		if (gameObject != null)
		{
			gameObject.SetActive(on);
		}
	}
}
