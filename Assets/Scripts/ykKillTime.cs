using UnityEngine;

public class ykKillTime : MonoBehaviour
{
	public enum ykKillType
	{
		destroy,
		hide,
		deactivate
	}

	private float passedTime;

	public float killTime = 1f;

	public ykKillType killType;

	private void Start()
	{
		passedTime = 0f;
	}

	private void Update()
	{
		passedTime += Time.deltaTime;
		if (!(passedTime > killTime))
		{
			return;
		}
		switch (killType)
		{
		case ykKillType.hide:
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
			break;
		}
		case ykKillType.deactivate:
			base.gameObject.SetActive(false);
			break;
		case ykKillType.destroy:
			Object.Destroy(base.gameObject);
			break;
		}
	}
}
