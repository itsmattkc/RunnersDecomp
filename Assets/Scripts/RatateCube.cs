using UnityEngine;

public class RatateCube : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(0f, 15f * Time.deltaTime, 0f);
	}
}
