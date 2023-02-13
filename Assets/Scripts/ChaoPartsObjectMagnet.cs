using UnityEngine;

public class ChaoPartsObjectMagnet : MonoBehaviour
{
	public enum HitType
	{
		RING,
		CRYSTAL
	}

	private enum Mode
	{
		Idle,
		Magnet,
		Respite
	}

	public float m_colliRadius = 2.5f;

	public float m_magnetRadius = 4f;

	public string m_effectName = string.Empty;

	public string m_hitLayer = string.Empty;

	public HitType m_hitType;

	private SphereCollider m_collider;

	private GameObject m_magnetObj;

	private ChaoMagnet m_magnet;

	private GameObject m_effect;

	private Animator m_animator;

	private Mode m_mode;

	private float m_time;

	private bool m_pauseFlag;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_pauseFlag)
		{
			return;
		}
		switch (m_mode)
		{
		case Mode.Magnet:
			m_time -= Time.deltaTime;
			if (m_time < 0f)
			{
				SetRespite();
				m_time = 1f;
				m_mode = Mode.Respite;
			}
			break;
		case Mode.Respite:
			m_time -= Time.deltaTime;
			if (m_time < 0f)
			{
				SetDisable();
				m_mode = Mode.Idle;
			}
			break;
		}
	}

	public void Setup()
	{
		string layerName = "HitRing";
		HitType hitType = m_hitType;
		if (hitType != 0 && hitType == HitType.CRYSTAL)
		{
			layerName = "HitCrystal";
		}
		base.gameObject.layer = LayerMask.NameToLayer(layerName);
		m_animator = base.gameObject.GetComponent<Animator>();
		m_collider = base.gameObject.AddComponent<SphereCollider>();
		if (m_collider != null)
		{
			m_collider.radius = m_colliRadius;
			m_collider.isTrigger = true;
			m_collider.enabled = false;
		}
		m_magnetObj = new GameObject();
		if (m_magnetObj != null)
		{
			m_magnetObj.name = "magnet";
			m_magnetObj.transform.parent = base.gameObject.transform;
			m_magnetObj.layer = LayerMask.NameToLayer(layerName);
			m_magnet = m_magnetObj.AddComponent<ChaoMagnet>();
			if (m_magnet != null)
			{
				m_magnet.Setup(m_magnetRadius, m_hitLayer);
			}
		}
		base.enabled = false;
	}

	public void SetEnable(float time)
	{
		m_time = time;
		if (m_effect != null)
		{
			Object.Destroy(m_effect);
			m_effect = null;
		}
		if (!string.IsNullOrEmpty(m_effectName))
		{
			m_effect = ObjUtil.PlayChaoEffect(base.gameObject, m_effectName, -1f);
		}
		if (m_collider != null)
		{
			m_collider.enabled = true;
		}
		if (m_magnet != null)
		{
			m_magnet.SetEnable(true);
		}
		SoundManager.SePlay("obj_magnet");
		m_mode = Mode.Magnet;
		base.enabled = true;
		if (m_pauseFlag)
		{
			SetPause(m_pauseFlag);
		}
		else
		{
			SetAnimation(true);
		}
	}

	public void SetPause(bool flag)
	{
		m_pauseFlag = flag;
		switch (m_mode)
		{
		case Mode.Magnet:
			m_effect.SetActive(!m_pauseFlag);
			if (m_magnet != null)
			{
				m_magnet.SetEnable(!m_pauseFlag);
			}
			SetAnimation(!m_pauseFlag);
			if (!m_pauseFlag)
			{
				SoundManager.SePlay("obj_magnet");
			}
			break;
		}
	}

	private void SetRespite()
	{
		if (m_effect != null)
		{
			Object.Destroy(m_effect);
			m_effect = null;
		}
		if (m_magnet != null)
		{
			m_magnet.SetEnable(false);
		}
		SetAnimation(false);
	}

	private void SetDisable()
	{
		if (m_collider != null)
		{
			m_collider.enabled = false;
		}
		SetAnimation(false);
		base.enabled = false;
	}

	private void SetAnimation(bool flag)
	{
		if (m_animator != null)
		{
			m_animator.SetBool("Ability", flag);
		}
	}
}
