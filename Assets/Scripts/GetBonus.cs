using UnityEngine;

public class GetBonus : MonoBehaviour
{
	private GameObject m_bonus_mng_object;

	public void AddBonusMngObject(GameObject obj)
	{
		m_bonus_mng_object = obj;
	}

	public void SetBonusCount(GameObject obj)
	{
		if ((bool)m_bonus_mng_object)
		{
			m_bonus_mng_object.SendMessage("OnTake", obj, SendMessageOptions.DontRequireReceiver);
		}
	}
}
