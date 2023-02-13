using Message;
using UnityEngine;

public class ChaoMagnet : MonoBehaviour
{
	private SphereCollider m_collider;

	private string m_hitLayer = string.Empty;

	private void Start()
	{
		base.enabled = false;
	}

	public void Setup(float colliRadius, string hitLayer)
	{
		m_hitLayer = hitLayer;
		m_collider = base.gameObject.GetComponent<SphereCollider>();
		if (m_collider == null)
		{
			m_collider = base.gameObject.AddComponent<SphereCollider>();
		}
		if (m_collider != null)
		{
			m_collider.radius = colliRadius;
			m_collider.isTrigger = true;
			m_collider.enabled = false;
		}
	}

	public void SetEnable(bool flag)
	{
		if (m_collider != null)
		{
			m_collider.enabled = flag;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!string.IsNullOrEmpty(m_hitLayer) && other.gameObject.layer == LayerMask.NameToLayer(m_hitLayer))
		{
			MsgOnDrawingRings value = new MsgOnDrawingRings(base.gameObject);
			other.gameObject.SendMessage("OnDrawingRingsToChao", value, SendMessageOptions.DontRequireReceiver);
		}
	}
}
