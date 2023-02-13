using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin")]
public class Spin : MonoBehaviour
{
	public Vector3 rotationsPerSecond = new Vector3(0f, 0.1f, 0f);

	private Rigidbody mRb;

	private Transform mTrans;

	private void Start()
	{
		mTrans = base.transform;
		mRb = base.rigidbody;
	}

	private void Update()
	{
		if (mRb == null)
		{
			ApplyDelta(Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (mRb != null)
		{
			ApplyDelta(Time.deltaTime);
		}
	}

	public void ApplyDelta(float delta)
	{
		delta *= 360f;
		Quaternion rhs = Quaternion.Euler(rotationsPerSecond * delta);
		if (mRb == null)
		{
			mTrans.rotation *= rhs;
		}
		else
		{
			mRb.MoveRotation(mRb.rotation * rhs);
		}
	}
}
