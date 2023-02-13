using UnityEngine;

namespace Player
{
	public class CharacterCheckTrickJump : MonoBehaviour
	{
		private bool m_touched;

		public bool IsTouched
		{
			get
			{
				return m_touched;
			}
		}

		private void Update()
		{
			CharacterInput component = GetComponent<CharacterInput>();
			if (component != null && component.IsTouched())
			{
				m_touched = true;
			}
		}

		public void Reset()
		{
			m_touched = false;
		}

		private void OnEnable()
		{
			Reset();
		}
	}
}
