using UnityEngine;

public class ykTextureSheetAnimation : MonoBehaviour
{
	[SerializeField]
	private int x = 1;

	[SerializeField]
	private int y = 1;

	[SerializeField]
	private int first;

	[SerializeField]
	private int count;

	[SerializeField]
	private float speed = 1f;

	private float tOffset;

	private int nOffset;

	private Material _material;

	private void Start()
	{
		_material = GetMaterial();
	}

	private void Update()
	{
		tOffset += Time.deltaTime * speed;
		if (count <= 0)
		{
			nOffset = (int)Mathf.Repeat(tOffset, (float)x * (float)y) + first;
		}
		else
		{
			nOffset = (int)Mathf.Repeat(tOffset, count) + first;
		}
		float num = Mathf.Repeat(nOffset, x);
		float num2 = y - Mathf.FloorToInt((nOffset + x) / x);
		Vector2 mainTextureScale = new Vector2(1f / (float)x, 1f / (float)y);
		_material.mainTextureScale = mainTextureScale;
		Vector2 mainTextureOffset = new Vector2(mainTextureScale.x * num, mainTextureScale.y * num2);
		_material.mainTextureOffset = mainTextureOffset;
	}

	protected virtual Material GetMaterial()
	{
		return base.renderer.material;
	}

	protected virtual bool IsValidChange()
	{
		return true;
	}

	public void SetSpeed(float in_speed)
	{
		if (IsValidChange())
		{
			speed = in_speed;
		}
	}
}
