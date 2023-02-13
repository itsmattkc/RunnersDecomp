using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjJumpBoard")]
public class ObjJumpBoard : SpawnableObject
{
	private enum State
	{
		Idle,
		Hit,
		Jump
	}

	private const string ModelName = "obj_cmn_jumpboard";

	private State m_state;

	private ObjJumpBoardParameter m_param;

	protected override string GetModelName()
	{
		return "obj_cmn_jumpboard";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
		ObjUtil.StopAnimation(base.gameObject);
	}

	public void SetObjJumpBoardParameter(ObjJumpBoardParameter param)
	{
		m_param = param;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_state == State.Idle)
		{
			MsgOnJumpBoardHit value = new MsgOnJumpBoardHit(base.transform.position, base.transform.rotation);
			other.gameObject.SendMessage("OnJumpBoardHit", value, SendMessageOptions.DontRequireReceiver);
			m_state = State.Hit;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (m_state != State.Hit)
		{
			return;
		}
		Quaternion rot = Quaternion.Euler(0f, 0f, 0f - m_param.m_succeedAngle) * base.transform.rotation;
		Quaternion rot2 = Quaternion.Euler(0f, 0f, 0f - m_param.m_missAngle) * base.transform.rotation;
		Vector3 pos = base.transform.position + base.transform.up * 0.25f;
		MsgOnJumpBoardJump msgOnJumpBoardJump = new MsgOnJumpBoardJump(pos, rot, rot2, m_param.m_succeedFirstSpeed, m_param.m_missFirstSpeed, m_param.m_succeedOutOfcontrol, m_param.m_missOutOfcontrol);
		other.gameObject.SendMessage("OnJumpBoardJump", msgOnJumpBoardJump, SendMessageOptions.DontRequireReceiver);
		if (msgOnJumpBoardJump.m_succeed)
		{
			Animation componentInChildren = GetComponentInChildren<Animation>();
			if ((bool)componentInChildren)
			{
				componentInChildren.wrapMode = WrapMode.Once;
				componentInChildren.Play("obj_jumpboard_bounce");
			}
		}
		m_state = State.Jump;
	}
}
