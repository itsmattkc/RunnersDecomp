using UnityEngine;

namespace Player
{
	public class StateEdit : FSMState<CharacterState>
	{
		public float m_moveSpeed = 2f;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.IgnoreCollision);
			Collider[] componentsInChildren = context.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				collider.enabled = false;
			}
			Collider component = context.GetComponent<Collider>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}

		public override void Leave(CharacterState context)
		{
			Collider[] componentsInChildren = context.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				collider.enabled = true;
			}
			Collider component = context.GetComponent<Collider>();
			if ((bool)component)
			{
				component.enabled = true;
			}
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			CharacterInput component = context.GetComponent<CharacterInput>();
			Vector3 vector = component.GetStick() * m_moveSpeed * Time.deltaTime;
			context.transform.position += vector;
			if (Input.GetButtonDown("ButtonX"))
			{
				m_moveSpeed *= 2f;
				if (m_moveSpeed >= 50f)
				{
					m_moveSpeed = 2f;
				}
			}
			if (Input.GetButtonDown("ButtonA"))
			{
				context.ChangeState(STATE_ID.Fall);
			}
		}

		public override void OnGUI(CharacterState context)
		{
			Rect position = new Rect(10f, 10f, 120f, 30f);
			string text = "Cursor Speed :" + m_moveSpeed;
			GUI.TextField(position, text);
		}
	}
}
