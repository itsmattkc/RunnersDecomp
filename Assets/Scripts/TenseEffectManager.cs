using UnityEngine;

[ExecuteInEditMode]
public class TenseEffectManager : MonoBehaviour
{
	public enum Type
	{
		TENSE_A,
		TENSE_B
	}

	[SerializeField]
	private Type m_nowType;

	private bool m_notChangeTense;

	private static TenseEffectManager instance;

	public bool NotChangeTense
	{
		get
		{
			return m_notChangeTense;
		}
		set
		{
			m_notChangeTense = value;
		}
	}

	public static TenseEffectManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObjectUtil.FindGameObjectComponent<TenseEffectManager>("TenseEffectManager");
			}
			return instance;
		}
	}

	private void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetType(Type t)
	{
		m_nowType = t;
	}

	public void FlipTenseType()
	{
		if (!NotChangeTense)
		{
			m_nowType = ((m_nowType == Type.TENSE_A) ? Type.TENSE_B : Type.TENSE_A);
		}
	}

	public Type GetTenseType()
	{
		return m_nowType;
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
