using System.Collections.Generic;
using UnityEngine;

public class UIEffectManager : MonoBehaviour
{
	private static UIEffectManager m_instance;

	private List<UIObjectContainer>[] m_effectList;

	public static UIEffectManager Instance
	{
		get
		{
			return m_instance;
		}
		private set
		{
		}
	}

	public void AddEffect(UIObjectContainer container)
	{
		if (!(container == null))
		{
			HudMenuUtility.EffectPriority priority = container.Priority;
			if (priority < HudMenuUtility.EffectPriority.Num)
			{
				m_effectList[(int)priority].Add(container);
			}
		}
	}

	public void RemoveEffect(UIObjectContainer container)
	{
		if (!(container == null))
		{
			HudMenuUtility.EffectPriority priority = container.Priority;
			if (priority < HudMenuUtility.EffectPriority.Num)
			{
				m_effectList[(int)priority].Remove(container);
			}
		}
	}

	public void SetActiveEffect(HudMenuUtility.EffectPriority priority, bool isActive)
	{
		for (int i = 0; i <= (int)priority; i++)
		{
			foreach (UIObjectContainer item in m_effectList[i])
			{
				if (!(item == null))
				{
					item.SetActive(isActive);
				}
			}
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			Init();
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Init()
	{
		m_effectList = new List<UIObjectContainer>[4];
		for (int i = 0; i < 4; i++)
		{
			m_effectList[i] = new List<UIObjectContainer>();
		}
	}
}
