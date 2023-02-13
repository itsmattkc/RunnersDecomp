using Message;
using UnityEngine;

namespace Player
{
	public class CharacterAssistAttack : MonoBehaviour
	{
		private enum Mode
		{
			Homing,
			ForceHoming,
			Up,
			Down
		}

		private const float FirstUpSpeed = 5f;

		private const float AttackUpSpeed = 10f;

		private const float LimitTime = 0.8f;

		private const float ForcedHomingTime = 0.2f;

		private const float GraityAcc = 35f;

		private const float TargetLimitOffset = 1f;

		private const float LimitForcedHomingTime = 2f;

		private const float FirstSpeedRate = 4f;

		private string m_name;

		private bool m_hitDamage;

		private float m_firstSpeed;

		private Vector3 m_velocity;

		private Mode m_mode;

		private float m_timer;

		private GameObject m_targetObject;

		private Vector3 m_targetPosition;

		private Camera m_mainCamera;

		private Animator m_animation;

		private void Start()
		{
			if (m_animation != null)
			{
				m_animation.SetBool("Jump", true);
			}
		}

		public void Setup(string name, Vector3 playerPos, float speed)
		{
			m_name = name;
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				Camera component = gameObject.GetComponent<Camera>();
				if (component != null)
				{
					Vector3 vector = component.WorldToScreenPoint(playerPos);
					vector = new Vector3(0f, component.pixelHeight * 0.5f, vector.z);
					Vector3 position = component.ScreenToWorldPoint(vector);
					base.transform.position = position;
					base.transform.rotation = Quaternion.FromToRotation(Vector3.forward, CharacterDefs.BaseFrontTangent);
					m_mainCamera = component;
				}
			}
			string text = "chr_" + m_name;
			text = text.ToLower();
			GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.CHARA_MODEL, text);
			GameObject gameObject3 = Object.Instantiate(gameObject2, base.transform.position, base.transform.rotation) as GameObject;
			if (gameObject3 != null)
			{
				Vector3 localPosition = gameObject2.transform.localPosition;
				Quaternion localRotation = gameObject2.transform.localRotation;
				gameObject3.transform.parent = base.transform;
				gameObject3.SetActive(true);
				gameObject3.transform.localPosition = localPosition;
				gameObject3.transform.localRotation = localRotation;
				m_animation = gameObject3.GetComponent<Animator>();
			}
			MsgBossInfo msgBossInfo = new MsgBossInfo();
			GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgBossInfo", msgBossInfo, SendMessageOptions.DontRequireReceiver);
			if (msgBossInfo.m_succeed)
			{
				m_targetObject = msgBossInfo.m_boss;
				m_targetPosition = msgBossInfo.m_position;
			}
			m_firstSpeed = speed * 4f;
			m_velocity = m_firstSpeed * base.transform.forward + 5f * Vector3.up;
		}

		private void Update()
		{
			switch (m_mode)
			{
			case Mode.Homing:
				UpdateHoming(Time.deltaTime);
				break;
			case Mode.ForceHoming:
				UpdateForcedHoming(Time.deltaTime);
				break;
			case Mode.Up:
				UpdateUp(Time.deltaTime);
				break;
			case Mode.Down:
				UpdateDown(Time.deltaTime);
				break;
			}
		}

		private void UpdateHoming(float deltaTime)
		{
			m_timer += deltaTime;
			UpdateTarget();
			if (m_targetObject == null)
			{
				base.transform.position += m_velocity * deltaTime;
				GoDown();
				return;
			}
			float firstSpeed = m_firstSpeed;
			MoveHoming(firstSpeed, deltaTime);
			if (m_timer > 0.8f)
			{
				m_mode = Mode.ForceHoming;
				m_timer = 0f;
			}
		}

		private void UpdateForcedHoming(float deltaTime)
		{
			bool flag = false;
			m_timer += deltaTime;
			if (!UpdateTarget())
			{
				base.transform.position += m_velocity * deltaTime;
				GoDown();
				return;
			}
			float magnitude = (m_targetPosition - base.transform.position).magnitude;
			float speed = magnitude / 0.2f;
			MoveHoming(speed, deltaTime);
			if (m_timer > 2f)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void UpdateUp(float deltaTime)
		{
			base.transform.position += m_velocity * deltaTime;
			m_timer += deltaTime;
			if (m_timer > 1f || IsOutsideOfCamera(base.transform.position))
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void UpdateDown(float deltaTime)
		{
			m_velocity += -Vector3.up * 35f * deltaTime;
			base.transform.position += m_velocity * deltaTime;
			m_timer += deltaTime;
			if (m_timer > 1f || IsOutsideOfCamera(base.transform.position))
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void GoUp()
		{
			m_mode = Mode.Up;
			m_velocity = base.transform.forward * 4f + 10f * Vector3.up;
			m_timer = 0f;
		}

		private void GoDown()
		{
			m_mode = Mode.Down;
			m_timer = 0f;
		}

		private void MoveHoming(float speed, float deltaTime)
		{
			Vector3 vector = m_targetPosition - base.transform.position;
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			if (magnitude < speed * deltaTime)
			{
				base.transform.position = m_targetPosition;
				return;
			}
			m_velocity = normalized * speed;
			base.transform.position += m_velocity * deltaTime;
		}

		private bool UpdateTarget()
		{
			if (m_targetObject != null)
			{
				MsgBossInfo msgBossInfo = new MsgBossInfo();
				m_targetObject.SendMessage("OnMsgBossInfo", msgBossInfo);
				if (msgBossInfo.m_succeed)
				{
					Vector3 position = msgBossInfo.m_position;
					float num = position.x + 1f;
					Vector3 position2 = base.transform.position;
					if (num > position2.x && !IsOutsideOfCamera(position))
					{
						m_targetPosition = position;
						return true;
					}
				}
			}
			return false;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!(other.gameObject != m_targetObject))
			{
				AttackPower attack = AttackPower.PlayerInvincible;
				MsgHitDamage value = new MsgHitDamage(base.gameObject, attack);
				other.gameObject.SendMessage("OnDamageHit", value, SendMessageOptions.DontRequireReceiver);
				GoUp();
			}
		}

		private bool IsOutsideOfCamera(Vector3 position)
		{
			if (m_mainCamera != null)
			{
				Vector3 vector = m_mainCamera.WorldToViewportPoint(position);
				if (vector.x < -0.3f || vector.x > 1.3f || vector.y < -0.3f || vector.y > 1.3f)
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}
}
