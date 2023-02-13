using UnityEngine;

[AddComponentMenu("NGUI/Examples/Window Drag Tilt")]
public class WindowDragTilt : MonoBehaviour
{
	public int updateOrder;

	public float degrees = 30f;

	private Vector3 mLastPos;

	private Transform mTrans;

	private float mAngle;

	private bool mInit = true;

	private void Start()
	{
		UpdateManager.AddCoroutine(this, updateOrder, CoroutineUpdate);
	}

	private void OnEnable()
	{
		mInit = true;
	}

	private void CoroutineUpdate(float delta)
	{
		if (mInit)
		{
			mInit = false;
			mTrans = base.transform;
			mLastPos = mTrans.position;
		}
		Vector3 vector = mTrans.position - mLastPos;
		mLastPos = mTrans.position;
		mAngle += vector.x * degrees;
		mAngle = NGUIMath.SpringLerp(mAngle, 0f, 20f, delta);
		mTrans.localRotation = Quaternion.Euler(0f, 0f, 0f - mAngle);
	}
}
