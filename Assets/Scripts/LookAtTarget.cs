using UnityEngine;

[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
	public int level;

	public Transform target;

	public float speed = 8f;

	private Transform mTrans;

	private void Start()
	{
		mTrans = base.transform;
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			Vector3 forward = target.position - mTrans.position;
			float magnitude = forward.magnitude;
			if (magnitude > 0.001f)
			{
				Quaternion to = Quaternion.LookRotation(forward);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, to, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}
	}
}
