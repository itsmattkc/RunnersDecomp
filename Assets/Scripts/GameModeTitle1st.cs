using UnityEngine;

[AddComponentMenu("Scripts/Runners/GameMode/title1st")]
public class GameModeTitle1st : MonoBehaviour
{
	private enum EventSignal
	{
		SIG_ONTOUCHED = 100
	}

	private const int NextSceneIndex = 1;

	private float m_alpha = 1f;

	private Texture2D m_texture;

	private TinyFsmBehavior m_fsm;

	private void Start()
	{
		m_texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		m_texture.SetPixel(0, 0, Color.white);
		m_texture.Apply();
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateIdle);
			description.onFixedUpdate = true;
			m_fsm.SetUp(description);
		}
		SoundManager.AddTitleCueSheet();
		SoundManager.BgmPlay("bgm_sys_title");
	}

	private void OnDestroy()
	{
		if ((bool)m_fsm)
		{
			m_fsm.ShutDown();
			m_fsm = null;
		}
	}

	private void OnGUI()
	{
	}

	private void FixedUpdate()
	{
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateFading));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFading(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_alpha = 0f;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_alpha = Mathf.Clamp(m_alpha + Time.deltaTime, 0f, 1f);
			if (m_alpha >= 1f)
			{
				Application.LoadLevel(1);
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void OnTouchTitle()
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
			m_fsm.Dispatch(signal);
		}
	}
}
