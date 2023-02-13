using UnityEngine;

public class PathParentObject : MonoBehaviour
{
	private static Vector3 NEW_OFFSET_POS = new Vector3(1f, 0f, 0f);

	private void Update()
	{
		UpdatePose();
	}

	public void UpdatePose()
	{
		if (base.transform.childCount <= 0)
		{
			return;
		}
		GameObject gameObject = base.transform.GetChild(0).gameObject;
		if (!gameObject)
		{
			return;
		}
		Vector3 start = gameObject.transform.position;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject2 = base.transform.GetChild(i).gameObject;
			if (!gameObject2)
			{
				continue;
			}
			int num = i + 1;
			if (num < base.transform.childCount)
			{
				GameObject gameObject3 = base.transform.GetChild(num).gameObject;
				if ((bool)gameObject3)
				{
					gameObject2.transform.LookAt(gameObject3.transform);
				}
			}
			else
			{
				GameObject gameObject4 = base.transform.GetChild(i - 1).gameObject;
				if ((bool)gameObject4)
				{
					gameObject2.transform.rotation = gameObject4.transform.rotation;
				}
			}
			Vector3 position = gameObject2.transform.position;
			Debug.DrawLine(start, position, Color.red);
			start = position;
		}
	}

	public void AddPathObject(string name, float size)
	{
		int childCount = base.transform.childCount;
		if (childCount > 0)
		{
			GameObject pathObject = GetPathObject((uint)(childCount - 1));
			if (pathObject != null)
			{
				CreatePathObject(name, pathObject.transform.position + NEW_OFFSET_POS, Quaternion.identity, size);
			}
		}
		else
		{
			CreatePathObject(name, Vector3.zero, Quaternion.identity, size);
		}
	}

	public void CreatePathObject(string name, Vector3 pos, Quaternion rot, float size)
	{
		string name2 = name + base.transform.childCount;
		GameObject gameObject = new GameObject(name2);
		if ((bool)gameObject)
		{
			gameObject.transform.parent = base.transform;
			gameObject.transform.position = pos;
			gameObject.transform.rotation = rot;
			gameObject.SetActive(true);
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			if ((bool)sphereCollider)
			{
				sphereCollider.radius = size;
			}
		}
	}

	public int GetPathObjectCount()
	{
		return base.transform.childCount;
	}

	public GameObject GetPathObject(uint index)
	{
		if (index < base.transform.childCount)
		{
			return base.transform.GetChild((int)index).gameObject;
		}
		return null;
	}

	public void SetZeroZ()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if ((bool)gameObject)
			{
				Transform transform = gameObject.transform;
				Vector3 position = gameObject.transform.position;
				float x = position.x;
				Vector3 position2 = gameObject.transform.position;
				transform.position = new Vector3(x, position2.y, 0f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if ((bool)gameObject)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(gameObject.transform.position, 0.2f);
			}
		}
	}
}
