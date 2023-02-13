using Message;
using UnityEngine;

public class ObjBossTrapBallCollision : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other)
		{
			GameObject gameObject = other.gameObject;
			if ((bool)gameObject)
			{
				MsgHitDamage value = new MsgHitDamage(base.gameObject, AttackPower.PlayerColorPower);
				gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnDrawGizmos()
	{
		SphereCollider component = GetComponent<SphereCollider>();
		if ((bool)component)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, component.radius);
		}
	}
}
