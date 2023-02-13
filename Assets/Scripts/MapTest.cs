using UnityEngine;

public class MapTest : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel("MapTest2");
		}
	}
}
