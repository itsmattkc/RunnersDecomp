using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Collision/ObjFallDeadCollision")]
public class ObjFallDeadCollision : ObjCollision
{
	protected override void OnSpawned()
	{
		base.OnSpawned();
	}

	private void OnTriggerExit(Collider other)
	{
		Vector3 position = other.transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 up = Vector3.up;
		Vector3 lhs = position2 - position;
		if (Vector3.Dot(lhs, up) > 0f)
		{
			other.gameObject.SendMessage("OnFallingDead");
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
