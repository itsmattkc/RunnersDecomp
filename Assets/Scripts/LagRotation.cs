using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Rotation")]
public class LagRotation : MonoBehaviour
{
	public int updateOrder;

	public float speed = 10f;

	public bool ignoreTimeScale;

	private Transform mTrans;

	private Quaternion mRelative;

	private Quaternion mAbsolute;

	private void Start()
	{
		mTrans = base.transform;
		mRelative = mTrans.localRotation;
		mAbsolute = mTrans.rotation;
		if (ignoreTimeScale)
		{
			UpdateManager.AddCoroutine(this, updateOrder, CoroutineUpdate);
		}
		else
		{
			UpdateManager.AddLateUpdate(this, updateOrder, CoroutineUpdate);
		}
	}

	private void CoroutineUpdate(float delta)
	{
		Transform parent = mTrans.parent;
		if (parent != null)
		{
			mAbsolute = Quaternion.Slerp(mAbsolute, parent.rotation * mRelative, delta * speed);
			mTrans.rotation = mAbsolute;
		}
	}
}
