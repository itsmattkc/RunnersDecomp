using System;
using UnityEngine;

namespace Player
{
	[Serializable]
	[ExecuteInEditMode]
	public class CharacterParameterData
	{
		public float m_maxSpeed = 20f;

		public float m_runAccel = 5f;

		public float m_runLoopAccel = 10f;

		public float m_runDec = 2f;

		public float m_airForwardAccel = 4f;

		public float m_jumpForce = 13.5f;

		public float m_jumpAddAcc;

		public float m_jumpAddSec;

		public float m_doubleJumpForce = 13.5f;

		public float m_doubleJumpAddAcc;

		public float m_doubleJumpAddSec;

		public float m_spinAttackForce = 12f;

		public float m_limitHeitht = 13f;

		public float m_goStumbleMaxHeight = 0.5f;

		public float m_stumbleJumpForce = 20f;

		public float m_level1Speed = 8f;

		public float m_level2Speed = 10f;

		public float m_level3Speed = 14f;

		public float m_spinDashSpeed = 15f;

		public float m_asteroidSpeed = 7.5f;

		public float m_laserSpeed = 18f;

		public float m_drillSpeed = 18f;

		public float m_minLoopRunSpeed = 10f;

		public float m_lastChanceSpeed = 20f;

		public float m_asteroidUpForce = 4f;

		public float m_asteroidDownForce = 4f;

		public float m_enableStompDec = 85f;

		public float m_drillJumpForce = 17.5f;

		public float m_drillJumpGravity = 35f;

		public float m_laserDrawingTime = 0.2f;

		public float m_laserWaitDrawingTime = 0.5f;

		public float m_damageSpeedRate = 0.75f;

		public float m_damageStumbleTime = 0.75f;

		public float m_damageEnableJumpTime = 0.75f;

		public float m_damageInvincibleTime = 1f;

		public float m_flyUpFirstSpeed = 8f;

		public float m_flyUpSpeedMax = 8f;

		public float m_flydownSpeedMax = 6f;

		public float m_flyUpForce = 6f;

		public float m_flySpeedRate = 1f;

		public float m_canFlyTime = 0.5f;

		public float m_flyGravityRate = 1f;

		public float m_flyDecSec2ndPress;

		public float m_powerGrideSpeedRate = 1.2f;

		public float m_disableGravityTime = 0.3f;

		public float m_grideTime = 0.4f;

		public float m_grideGravityRate = 0.2f;

		public float m_grideFirstUpForce = 1f;

		public float m_hitWallDeadTime = 0.25f;

		public float m_hitWallUpSpeedGround = 0.2f;

		public float m_hitWallUpSpeedAir;
	}
}
