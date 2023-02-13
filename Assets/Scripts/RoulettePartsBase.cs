using System.Collections.Generic;
using UnityEngine;

public class RoulettePartsBase : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_effectList;

	protected RouletteTop m_parent;

	protected float m_roulettePartsTime;

	protected bool m_isWindow;

	protected bool m_isEffectLock;

	protected long m_partsUpdateCount;

	private float m_delayTime;

	private bool m_isEffect;

	public RouletteTop parent
	{
		get
		{
			return m_parent;
		}
	}

	public bool isDelay
	{
		get
		{
			return m_delayTime > 0f;
		}
	}

	public bool isSpin
	{
		get
		{
			if (m_parent == null)
			{
				return false;
			}
			return m_parent.isSpin;
		}
	}

	public int spinDecisionIndex
	{
		get
		{
			if (m_parent == null)
			{
				return -1;
			}
			return m_parent.spinDecisionIndex;
		}
	}

	public ServerWheelOptionsData wheel
	{
		get
		{
			if (m_parent == null)
			{
				return null;
			}
			return m_parent.wheelData;
		}
	}

	public List<GameObject> effectList
	{
		get
		{
			return m_effectList;
		}
	}

	public void SetDelayTime(float delay = 0.2f)
	{
		if (delay >= 0f)
		{
			if (delay > 10f)
			{
				delay = 10f;
			}
			m_delayTime = delay;
		}
		else
		{
			m_delayTime = 0f;
		}
	}

	private void Update()
	{
		UpdateParts();
		m_roulettePartsTime += Time.deltaTime;
		if (m_roulettePartsTime >= float.MaxValue)
		{
			m_roulettePartsTime = 1000f;
		}
		if (m_delayTime > 0f)
		{
			m_delayTime -= Time.deltaTime;
			if (m_delayTime <= 0f)
			{
				m_delayTime = 0f;
			}
		}
		if (!m_isEffectLock)
		{
			if (GeneralWindow.Created || EventBestChaoWindow.Created || m_isWindow)
			{
				if (m_isEffect)
				{
					m_isEffect = false;
					if (m_effectList != null && m_effectList.Count > 0)
					{
						foreach (GameObject effect in m_effectList)
						{
							effect.SetActive(m_isEffect);
						}
					}
				}
			}
			else if (!m_isEffect)
			{
				m_isEffect = true;
				if (m_effectList != null && m_effectList.Count > 0)
				{
					foreach (GameObject effect2 in m_effectList)
					{
						effect2.SetActive(m_isEffect);
					}
				}
			}
		}
		else if (m_isEffect)
		{
			m_isEffect = false;
			if (m_effectList != null && m_effectList.Count > 0)
			{
				foreach (GameObject effect3 in m_effectList)
				{
					effect3.SetActive(m_isEffect);
				}
			}
		}
		m_partsUpdateCount++;
	}

	protected virtual void UpdateParts()
	{
	}

	public virtual void UpdateEffectSetting()
	{
	}

	public virtual void Setup(RouletteTop parent)
	{
		m_isWindow = false;
		m_parent = parent;
		m_roulettePartsTime = 0f;
		m_delayTime = 0f;
		m_isEffect = true;
		if (m_isEffectLock)
		{
			m_isEffect = false;
		}
		if (m_effectList != null && m_effectList.Count > 0)
		{
			foreach (GameObject effect in m_effectList)
			{
				effect.SetActive(m_isEffect);
			}
		}
		m_partsUpdateCount = 0L;
	}

	public virtual void OnUpdateWheelData(ServerWheelOptionsData data)
	{
		m_roulettePartsTime = 0f;
	}

	public virtual void DestroyParts()
	{
		Object.Destroy(base.gameObject);
	}

	public virtual void OnSpinStart()
	{
	}

	public virtual void OnSpinSkip()
	{
	}

	public virtual void OnSpinDecision()
	{
	}

	public virtual void OnSpinDecisionMulti()
	{
	}

	public virtual void OnSpinEnd()
	{
	}

	public virtual void OnSpinError()
	{
	}

	public virtual void OnRouletteClose()
	{
	}

	public virtual void windowOpen()
	{
		m_isWindow = true;
	}

	public virtual void windowClose()
	{
		m_isWindow = false;
		if (GeneralWindow.Created || EventBestChaoWindow.Created || m_isEffectLock)
		{
			return;
		}
		m_isEffect = true;
		if (m_effectList == null || m_effectList.Count <= 0)
		{
			return;
		}
		foreach (GameObject effect in m_effectList)
		{
			effect.SetActive(m_isEffect);
		}
	}

	public virtual void PartsSendMessage(string mesage)
	{
	}
}
