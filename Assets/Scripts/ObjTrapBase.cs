using Message;
using System.Collections.Generic;
using UnityEngine;

public class ObjTrapBase : SpawnableObject
{
	protected bool m_end;

	protected override void OnSpawned()
	{
	}

	public void OnMsgObjectDead(MsgObjectDead msg)
	{
		if (!m_end)
		{
			SetBroken();
		}
	}

	protected virtual int GetScore()
	{
		return 0;
	}

	protected virtual void PlayEffect()
	{
	}

	protected virtual void TrapDamageHit()
	{
		PlayerInformation playerInformation = ObjUtil.GetPlayerInformation();
		if (!(playerInformation != null) || !playerInformation.IsNowLastChance())
		{
			ObjUtil.LightPlaySE("obj_needle_damage");
		}
	}

	private void SetPlayerBroken(uint attribute_state)
	{
		int score = GetScore();
		List<ChaoAbility> abilityList = new List<ChaoAbility>();
		ObjUtil.GetChaoAbliltyPhantomFlag(attribute_state, ref abilityList);
		score = ObjUtil.GetChaoAndEnemyScore(abilityList, score);
		ObjUtil.SendMessageAddScore(score);
		ObjUtil.SendMessageScoreCheck(new StageScoreData(1, score));
		SetBroken();
	}

	protected void SetBroken()
	{
		if (!m_end)
		{
			m_end = true;
			PlayEffect();
			ObjUtil.LightPlaySE("obj_brk");
			if (base.Share)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_end && (bool)other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerColorPower);
				gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnDamageHit(MsgHitDamage msg)
	{
		if (m_end)
		{
			return;
		}
		if (msg.m_attackPower >= 4)
		{
			if ((bool)msg.m_sender)
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
		else
		{
			TrapDamageHit();
		}
	}
}
