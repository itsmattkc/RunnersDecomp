using Message;
using UnityEngine;

public class ObjTutorialEndCollision : ObjCollision
{
	private const float COLLIDER_X_SIZE = 2f;

	protected override void OnSpawned()
	{
		base.OnSpawned();
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			Vector3 size = component.size;
			if (size.x < 2f)
			{
				Vector3 size2 = component.size;
				float y = size2.y;
				Vector3 size3 = component.size;
				component.size = new Vector3(2f, y, size3.z);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObjectUtil.SendMessageFindGameObject("StageTutorialManager", "OnMsgTutorialEnd", new MsgTutorialEnd(), SendMessageOptions.DontRequireReceiver);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
