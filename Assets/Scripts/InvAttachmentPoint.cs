using UnityEngine;

[AddComponentMenu("NGUI/Examples/Item Attachment Point")]
public class InvAttachmentPoint : MonoBehaviour
{
	public InvBaseItem.Slot slot;

	private GameObject mPrefab;

	private GameObject mChild;

	public GameObject Attach(GameObject prefab)
	{
		if (mPrefab != prefab)
		{
			mPrefab = prefab;
			if (mChild != null)
			{
				Object.Destroy(mChild);
			}
			if (mPrefab != null)
			{
				Transform transform = base.transform;
				mChild = (Object.Instantiate(mPrefab, transform.position, transform.rotation) as GameObject);
				Transform transform2 = mChild.transform;
				transform2.parent = transform;
				transform2.localPosition = Vector3.zero;
				transform2.localRotation = Quaternion.identity;
				transform2.localScale = Vector3.one;
			}
		}
		return mChild;
	}
}
