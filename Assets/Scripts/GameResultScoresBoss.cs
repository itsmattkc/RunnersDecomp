using AnimationOrTween;
using UnityEngine;

public class GameResultScoresBoss : GameResultScores
{
	private enum AnimState
	{
		NONE = -1,
		IDLE,
		PLAYING1,
		PLAYING2,
		FINISHED,
		END
	}

	private const string noMissBonusClip = "ui_result_nomiss_bonus_Anim";

	private AnimState m_animState;

	private Animation m_noMissBonusAnim;

	private float m_noMissBonusViewTime;

	private bool m_isNomiss;

	public void SetNoMissFlag(bool flag)
	{
		m_isNomiss = flag;
	}

	protected override bool IsBonus(StageScoreManager.ResultData data1, StageScoreManager.ResultData data2, StageScoreManager.ResultData data3)
	{
		long num = 0L;
		if (data1 != null)
		{
			num += data1.ring;
		}
		if (data2 != null)
		{
			num += data2.ring;
		}
		if (data3 != null)
		{
			num += data3.ring;
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	protected override void OnSetup(GameObject resultRoot)
	{
		m_noMissBonusViewTime = 0f;
		m_noMissBonusAnim = GameObjectUtil.FindChildGameObjectComponent<Animation>(resultRoot, "nomiss_bonus_Anim");
		if (m_noMissBonusAnim != null)
		{
			m_noMissBonusAnim.gameObject.SetActive(false);
			m_noMissBonusViewTime = m_noMissBonusAnim["ui_result_nomiss_bonus_Anim"].length * 0.3f;
		}
		if (IsBonusEvent())
		{
			SetEnableNextButton(true);
		}
		m_animState = AnimState.IDLE;
		m_isBossResult = true;
	}

	protected override void OnFinish()
	{
	}

	protected override void OnStartFinished()
	{
		SetBonusEventScoreActive(Category.NONE);
		if (m_isReplay)
		{
			m_animState = AnimState.FINISHED;
			return;
		}
		if (!m_isNomiss)
		{
			m_animState = AnimState.FINISHED;
			return;
		}
		if (m_noMissBonusAnim != null)
		{
			m_noMissBonusAnim.gameObject.SetActive(true);
		}
		SoundManager.SePlay("sys_specialegg");
		m_animState = AnimState.PLAYING1;
		SetEnableNextButton(false);
	}

	protected override void OnUpdateFinished()
	{
		if (!m_isNomiss)
		{
			return;
		}
		if (m_animState == AnimState.PLAYING1)
		{
			float time = m_noMissBonusAnim["ui_result_nomiss_bonus_Anim"].time;
			if (time > m_noMissBonusViewTime)
			{
				SetEnableNextButton(true);
				m_animState = AnimState.PLAYING2;
			}
		}
		if (m_animState == AnimState.PLAYING2 && !m_noMissBonusAnim.isPlaying)
		{
			m_animState = AnimState.FINISHED;
		}
	}

	protected override void OnSkipFinished()
	{
		if (m_isNomiss && m_animState == AnimState.PLAYING2 && m_noMissBonusAnim != null)
		{
			m_noMissBonusAnim.Stop();
			m_noMissBonusAnim.gameObject.SetActive(false);
			m_animState = AnimState.FINISHED;
		}
	}

	protected override bool IsEndFinished()
	{
		if (m_animState == AnimState.FINISHED)
		{
			return true;
		}
		return false;
	}

	protected override void OnEndFinished()
	{
		SetEnableNextButton(true);
	}

	protected override void OnScoreInAnimation(EventDelegate.Callback callback)
	{
		Animation animation = GameResultUtility.SearchAnimation(m_resultRoot);
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_result_boss_intro_score_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, callback, true);
		}
	}

	protected override void OnScoreOutAnimation(EventDelegate.Callback callback)
	{
		Animation animation = GameResultUtility.SearchAnimation(m_resultRoot);
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_result_boss_intro_score_Anim", Direction.Reverse);
			EventDelegate.Add(activeAnimation.onFinished, callback, true);
		}
	}
}
