using UnityEngine;

[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : MonoBehaviour
{
	public Vector2 degrees = new Vector2(5f, 3f);

	public float range = 1f;

	private Transform mTrans;

	private Quaternion mStart;

	private Vector2 mRot = Vector2.zero;

	private void Start()
	{
		mTrans = base.transform;
		mStart = mTrans.localRotation;
	}

	private void Update()
	{
		float deltaTime = RealTime.deltaTime;
		Vector3 mousePosition = Input.mousePosition;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		if (range < 0.1f)
		{
			range = 0.1f;
		}
		float x = Mathf.Clamp((mousePosition.x - num) / num / range, -1f, 1f);
		float y = Mathf.Clamp((mousePosition.y - num2) / num2 / range, -1f, 1f);
		mRot = Vector2.Lerp(mRot, new Vector2(x, y), deltaTime * 5f);
		mTrans.localRotation = mStart * Quaternion.Euler((0f - mRot.y) * degrees.y, mRot.x * degrees.x, 0f);
	}
}
