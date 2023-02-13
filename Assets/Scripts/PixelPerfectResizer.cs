using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectResizer : MonoBehaviour
{
	public Camera cam;

	private Transform _transform;

	private float _lastOrthographicSize;

	private float _lastPixelWidth;

	private float _lastPixelHeight;

	private void Awake()
	{
		_transform = base.transform;
	}

	private void Update()
	{
		if (cam == null)
		{
			cam = Camera.main;
		}
		if (cam != null && (cam.orthographicSize != _lastOrthographicSize || cam.pixelWidth != _lastPixelWidth || cam.pixelHeight != _lastPixelHeight))
		{
			Transform transform = _transform;
			float x = (float)(int)(cam.orthographicSize * 2000f * cam.aspect / cam.pixelWidth) / 1000f;
			float y = (float)(int)(cam.orthographicSize * 2000f / cam.pixelHeight) / 1000f;
			Vector3 localScale = _transform.localScale;
			transform.localScale = new Vector3(x, y, localScale.z);
		}
	}
}
