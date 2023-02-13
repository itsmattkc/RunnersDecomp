using UnityEngine;

namespace Player
{
	public class CharacterPhantomCollision : MonoBehaviour
	{
		private GameObject m_parent;

		private CharacterState m_state;

		private void Start()
		{
			m_parent = base.transform.parent.gameObject;
			if (m_parent != null)
			{
				m_state = m_parent.GetComponent<CharacterState>();
			}
		}

		private void OnAddRings(int numRing)
		{
			if (m_state != null)
			{
				m_state.OnAddRings(numRing);
			}
		}

		private void OnFallingDead()
		{
			if (m_state != null)
			{
				m_state.OnFallingDead();
			}
		}
	}
}
