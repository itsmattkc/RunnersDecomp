using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Position")]
public class LagPosition : MonoBehaviour
{
	public int updateOrder;

	public Vector3 speed = new Vector3(10f, 10f, 10f);

	public bool ignoreTimeScale;

	private Transform mTrans;

	private Vector3 mRelative;

	private Vector3 mAbsolute;

	private void Start()
	{
		mTrans = base.transform;
		mRelative = mTrans.localPosition;
		if (ignoreTimeScale)
		{
			UpdateManager.AddCoroutine(this, updateOrder, CoroutineUpdate);
		}
		else
		{
			UpdateManager.AddLateUpdate(this, updateOrder, CoroutineUpdate);
		}
	}

	private void OnEnable()
	{
		mTrans = base.transform;
		mAbsolute = mTrans.position;
	}

	private void CoroutineUpdate(float delta)
	{
		Transform parent = mTrans.parent;
		if (parent != null)
		{
			Vector3 vector = parent.position + parent.rotation * mRelative;
			mAbsolute.x = Mathf.Lerp(mAbsolute.x, vector.x, Mathf.Clamp01(delta * speed.x));
			mAbsolute.y = Mathf.Lerp(mAbsolute.y, vector.y, Mathf.Clamp01(delta * speed.y));
			mAbsolute.z = Mathf.Lerp(mAbsolute.z, vector.z, Mathf.Clamp01(delta * speed.z));
			mTrans.position = mAbsolute;
		}
	}
}
