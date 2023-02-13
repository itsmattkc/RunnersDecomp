using UnityEngine;

public class ObjBossUtil
{
	private const float BALL_DOWNSPEED = 2f;

	private const float BALL_DOWNPOS = 0.3f;

	private static Vector3 EFFECT_ROT = new Vector3(-90f, 0f, 0f);

	public static void PlayShotEffect(GameObject boss_obj)
	{
		if ((bool)boss_obj)
		{
			Quaternion local_rot = Quaternion.Euler(EFFECT_ROT);
			ObjUtil.PlayEffectChild(GetBossHatchNode(boss_obj), "ef_bo_em_muzzle01", Vector3.zero, local_rot, 1f);
		}
	}

	public static void PlayShotSE()
	{
		ObjUtil.PlaySE("boss_bomb_drop2");
	}

	public static void SetupBallAppear(GameObject boss_obj, GameObject ball_obj)
	{
		if ((bool)boss_obj && (bool)ball_obj)
		{
			Vector3 bossHatchPos = GetBossHatchPos(boss_obj);
			Transform transform = ball_obj.transform;
			float x = bossHatchPos.x;
			Vector3 position = ball_obj.transform.position;
			transform.position = new Vector3(x, position.y, bossHatchPos.z);
			ObjUtil.SetModelVisible(ball_obj, true);
		}
	}

	public static bool UpdateBallAppear(float delta, GameObject boss_obj, GameObject ball_obj, float add_speed)
	{
		if ((bool)boss_obj && (bool)ball_obj)
		{
			Vector3 bossHatchPos = GetBossHatchPos(boss_obj);
			float num = delta * (2f * add_speed);
			Transform transform = ball_obj.transform;
			float x = bossHatchPos.x;
			Vector3 position = ball_obj.transform.position;
			transform.position = new Vector3(x, position.y - num, bossHatchPos.z);
			float y = bossHatchPos.y;
			Vector3 position2 = ball_obj.transform.position;
			if (y - position2.y > 0.3f)
			{
				ball_obj.transform.position = new Vector3(bossHatchPos.x, bossHatchPos.y - 0.3f, bossHatchPos.z);
				return true;
			}
		}
		return false;
	}

	public static void UpdateBallAttack(GameObject boss_obj, GameObject ball_obj, float time, float attack_speed)
	{
		if ((bool)boss_obj && (bool)ball_obj)
		{
			float num = 0.1f - time * attack_speed;
			if (num < 0.01f)
			{
				num = 0f;
			}
			Vector3 currentVelocity = Vector3.zero;
			Vector3 position = boss_obj.transform.position;
			ball_obj.transform.position = Vector3.SmoothDamp(ball_obj.transform.position, position, ref currentVelocity, num);
		}
	}

	public static void UpdateBallRot(float delta, GameObject ball_obj, Vector3 agl, float speed)
	{
		if ((bool)ball_obj)
		{
			ball_obj.transform.rotation = Quaternion.Euler(agl * 60f * delta * speed) * ball_obj.transform.rotation;
		}
	}

	public static GameObject GetBossHatchNode(GameObject boss_obj)
	{
		if ((bool)boss_obj)
		{
			return GameObjectUtil.FindChildGameObject(boss_obj, "BombPoint");
		}
		return null;
	}

	public static Vector3 GetBossHatchPos(GameObject boss_obj)
	{
		GameObject bossHatchNode = GetBossHatchNode(boss_obj);
		if (bossHatchNode != null)
		{
			return bossHatchNode.transform.position;
		}
		return Vector3.zero;
	}

	public static Quaternion GetShotRotation(Quaternion rot, bool playerDead)
	{
		if (playerDead)
		{
			Vector3 eulerAngles = rot.eulerAngles;
			return Quaternion.Euler(0f, eulerAngles.y, eulerAngles.z);
		}
		return rot;
	}

	public static bool IsNowLastChance(PlayerInformation playerInfo)
	{
		if (playerInfo != null && playerInfo.IsNowLastChance())
		{
			return true;
		}
		return false;
	}
}
