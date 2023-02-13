using AnimationOrTween;
using UI;
using UnityEngine;

public class GameResultScoresRaidBoss : GameResultScores
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

	private const string destroyBonusClip = "ui_EventResult_raidboss_destroy_bonus_Anim";

	private const string introScoreAnimClip = "ui_EventResult_raidboss_intro_Anim";

	private GameObject m_raidbossResultRoot;

	private AnimState m_animState;

	private Animation m_destroyBonusAnim;

	private float m_destroyBonusViewTime;

	private bool m_isBossDestroy;

	public void SetBossDestroyFlag(bool flag)
	{
		m_isBossDestroy = flag;
	}

	protected override bool IsBonus(StageScoreManager.ResultData data1, StageScoreManager.ResultData data2, StageScoreManager.ResultData data3)
	{
		long num = 0L;
		long num2 = 0L;
		if (data1 != null)
		{
			num += data1.raid_boss_ring;
			num2 += data1.raid_boss_reward;
		}
		if (data2 != null)
		{
			num += data2.raid_boss_ring;
			num2 += data2.raid_boss_reward;
		}
		if (data3 != null)
		{
			num += data3.raid_boss_ring;
			num2 += data3.raid_boss_reward;
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	protected override void OnSetup(GameObject resultRoot)
	{
		Debug.Log("GameResultScoresRaidBoss:OnSetup");
		m_destroyBonusViewTime = 0f;
		m_raidbossResultRoot = GameObjectUtil.FindChildGameObject(base.gameObject, "EventResult_raidboss");
		if (m_raidbossResultRoot != null)
		{
			m_destroyBonusAnim = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_raidbossResultRoot, "destroy_bonus_Anim");
			if (m_destroyBonusAnim != null)
			{
				m_destroyBonusAnim.gameObject.SetActive(false);
				m_destroyBonusViewTime = m_destroyBonusAnim["ui_EventResult_raidboss_destroy_bonus_Anim"].length * 0.3f;
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(resultRoot, "nomiss_bonus_Anim");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(resultRoot, "window_result");
		if (gameObject2 != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_word_ring");
			if (uILabel != null)
			{
				uILabel.text = HudUtility.GetEventSpObjectName();
				UILocalizeText uILocalizeText = GameObjectUtil.FindChildGameObjectComponent<UILocalizeText>(gameObject2, "Lbl_word_ring");
				if (uILocalizeText != null)
				{
					uILocalizeText.enabled = false;
				}
			}
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_icon_ring");
			if (uISprite != null)
			{
				uISprite.spriteName = "ui_event_ring_icon";
			}
		}
		SetEnableNextButton(true);
		m_animState = AnimState.IDLE;
		m_isBossResult = true;
	}

	protected override void OnFinish()
	{
	}

	protected override void OnStartBeginning()
	{
		SetBonusEventScoreActive(Category.NONE);
		if (m_isReplay)
		{
			m_animState = AnimState.FINISHED;
			return;
		}
		if (!m_isBossDestroy)
		{
			m_animState = AnimState.FINISHED;
			return;
		}
		if (m_destroyBonusAnim != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_raidbossResultRoot, "Lbl_destroy_bonus");
			if (uILabel != null)
			{
				uILabel.text = GameResultUtility.GetRaidbossBeatBonus().ToString();
			}
			m_destroyBonusAnim.gameObject.SetActive(true);
		}
		SoundManager.SePlay("sys_specialegg");
		m_animState = AnimState.PLAYING1;
		SetEnableNextButton(false);
	}

	protected override void OnUpdateBeginning()
	{
		if (!m_isBossDestroy)
		{
			return;
		}
		if (m_animState == AnimState.PLAYING1)
		{
			float time = m_destroyBonusAnim["ui_EventResult_raidboss_destroy_bonus_Anim"].time;
			if (time > m_destroyBonusViewTime)
			{
				SetEnableNextButton(true);
				m_animState = AnimState.PLAYING2;
			}
		}
		if (m_animState == AnimState.PLAYING2 && !m_destroyBonusAnim.isPlaying)
		{
			m_animState = AnimState.FINISHED;
		}
	}

	protected override void OnSkipBeginning()
	{
		if (m_isBossDestroy && m_animState == AnimState.PLAYING2 && m_destroyBonusAnim != null)
		{
			m_destroyBonusAnim.Stop();
			m_destroyBonusAnim.gameObject.SetActive(false);
			m_animState = AnimState.FINISHED;
		}
	}

	protected override bool IsEndBeginning()
	{
		if (m_animState == AnimState.FINISHED)
		{
			return true;
		}
		return false;
	}

	protected override void OnEndBeginning()
	{
		SetEnableNextButton(true);
		HudEventResultRaidBoss hudEventResultRaidBoss = GameObjectUtil.FindChildGameObjectComponent<HudEventResultRaidBoss>(base.gameObject, "EventResult_raidboss");
		if (hudEventResultRaidBoss != null)
		{
			hudEventResultRaidBoss.SetEnableDamageDetailsButton(true);
		}
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
		HudEventResultRaidBoss hudEventResultRaidBoss = GameObjectUtil.FindChildGameObjectComponent<HudEventResultRaidBoss>(base.gameObject, "EventResult_raidboss");
		if (hudEventResultRaidBoss != null)
		{
			hudEventResultRaidBoss.SetEnableDamageDetailsButton(false);
		}
	}
}
