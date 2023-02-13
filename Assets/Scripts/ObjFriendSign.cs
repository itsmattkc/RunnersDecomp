using GameScore;
using UnityEngine;

public class ObjFriendSign : SpawnableObject
{
	private enum Mode
	{
		Idle,
		Start,
		Rot,
		End
	}

	private const string ModelName = "obj_cmn_friendsign";

	private const float END_TIME = 5f;

	private const float START_SPEED = 100f;

	private Mode m_mode;

	private float m_time;

	private float m_speed;

	private float m_rot;

	protected override string GetModelName()
	{
		return "obj_cmn_friendsign";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		switch (m_mode)
		{
		case Mode.Start:
			m_rot = 0f;
			m_speed = 100f;
			m_mode = Mode.Rot;
			break;
		case Mode.Rot:
		{
			float num = 60f * deltaTime * m_speed;
			float num2 = m_rot + num;
			if (num2 < 360f)
			{
				m_rot += num;
			}
			else
			{
				num = 360f - m_rot;
				m_speed -= m_speed * 0.5f;
				m_rot = 0f;
				if (m_speed < 3f)
				{
					m_time = 0f;
					m_mode = Mode.End;
				}
			}
			base.transform.rotation = Quaternion.Euler(0f, num, 0f) * base.transform.rotation;
			break;
		}
		case Mode.End:
			m_time += deltaTime;
			if (m_time > 5f)
			{
				Object.Destroy(base.gameObject);
				m_mode = Mode.Idle;
			}
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				HitFriendSign();
			}
		}
	}

	private void HitFriendSign()
	{
		if (m_mode == Mode.Idle)
		{
			ObjUtil.SendMessageAddScore(Data.FriendSign);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(5, Data.FriendSign));
			ObjUtil.PlaySE("obj_item_friendsign");
			m_mode = Mode.Start;
		}
	}

	public void ChangeTexture(Texture tex)
	{
		if ((bool)tex)
		{
			MeshRenderer meshRenderer = GameObjectUtil.FindChildGameObjectComponent<MeshRenderer>(base.gameObject, "obj_cmn_friendsignpicture");
			if ((bool)meshRenderer)
			{
				meshRenderer.material.mainTexture = tex;
			}
		}
	}
}
