using Message;
using UnityEngine;

public class ChaoPartsAttackEnemy : MonoBehaviour
{
	private const float ColliRadius = 1f;

	private const float ColliHeight = 3f;

	private void Awake()
	{
		Setup();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Setup()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Player");
		base.gameObject.tag = "ChaoAttack";
		CapsuleCollider capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.radius = 1f;
		capsuleCollider.height = 3f;
		capsuleCollider.isTrigger = true;
	}

	public static GameObject Create(GameObject parent)
	{
		GameObject gameObject = new GameObject("ChaoPartsAttackEnemy");
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.AddComponent<ChaoPartsAttackEnemy>();
		return gameObject;
	}

	public void OnTriggerEnter(Collider other)
	{
		AttackPower attack = AttackPower.PlayerInvincible;
		MsgHitDamage msgHitDamage = new MsgHitDamage(base.gameObject, attack);
		msgHitDamage.m_attackAttribute = 32u;
		GameObjectUtil.SendDelayedMessageToGameObject(other.gameObject, "OnDamageHit", msgHitDamage);
	}

	private void OnDamageSucceed(MsgHitDamageSucceed msg)
	{
		base.gameObject.transform.parent.SendMessage("OnDamageSucceed", msg, SendMessageOptions.DontRequireReceiver);
	}
}
