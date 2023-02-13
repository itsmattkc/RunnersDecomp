using Message;
using UnityEngine;

namespace Player
{
	public class StatePhantomAsteroid : FSMState<CharacterState>
	{
		private float m_time;

		private GameObject m_effect;

		private bool m_changePhantomCancel;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Asteroid);
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			m_effect = PhantomAsteroidUtil.ChangeVisualOnEnter(context);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			context.OnAttackAttribute(AttackAttribute.PhantomAsteroid);
			StateUtil.DeactiveInvincible(context);
			StateUtil.SetNotDrawItemEffect(context, true);
			StateUtil.SetPhantomMagnetColliderRange(context, PhantomType.ASTEROID);
			m_time = -1f;
			ChangePhantomParameter enteringParameter = context.GetEnteringParameter<ChangePhantomParameter>();
			if (enteringParameter != null)
			{
				m_time = enteringParameter.Timer;
			}
			StateUtil.SendMessageTransformPhantom(context, PhantomType.ASTEROID);
			if (context.GetChangePhantomCancel() == ItemType.ASTEROID)
			{
				m_changePhantomCancel = true;
			}
			else
			{
				m_changePhantomCancel = false;
			}
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			StateUtil.SetNotDrawItemEffect(context, false);
			PhantomAsteroidUtil.ChangeVisualOnLeave(context, m_effect);
			m_effect = null;
			StateUtil.SendMessageReturnFromPhantom(context, PhantomType.ASTEROID);
			context.SetNotCharaChange(false);
			context.SetNotUseItem(false);
			context.SetChangePhantomCancel(ItemType.UNKNOWN);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			if (m_changePhantomCancel)
			{
				m_changePhantomCancel = false;
				DispatchMessage(context, 12289, new MsgInvalidateItem(ItemType.ASTEROID));
				return;
			}
			Vector3 a = context.Parameter.m_asteroidSpeed * context.Movement.GetForwardDir();
			Vector3 b = Vector3.zero;
			float limitHeitht = context.Parameter.m_limitHeitht;
			Vector3 pos = context.Position;
			StateUtil.GetBaseGroundPosition(context, ref pos);
			if (context.m_input.IsHold())
			{
				float num = Vector3.Magnitude(context.Position - pos);
				if (num < limitHeitht)
				{
					b = -context.Movement.GetGravityDir() * context.Parameter.m_asteroidUpForce;
				}
			}
			else if (!context.Movement.IsOnGround())
			{
				b = context.Movement.GetGravityDir() * context.Parameter.m_asteroidDownForce;
			}
			context.Movement.Velocity = a + b;
			GameObject modelObject = PhantomAsteroidUtil.GetModelObject(context);
			if (modelObject != null)
			{
				Transform transform = modelObject.transform;
				Quaternion identity = Quaternion.identity;
				identity.SetLookRotation(CharacterDefs.BaseFrontTangent, -context.Movement.GetGravityDir());
				transform.rotation = identity;
			}
			if (m_time > 0f)
			{
				m_time -= deltaTime;
				if (m_time < 0f)
				{
					StateUtil.ResetVelocity(context);
					StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.ASTEROID, false);
				}
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			switch (messageId)
			{
			case 12289:
			{
				MsgInvalidateItem msgInvalidateItem = msg as MsgInvalidateItem;
				if (msgInvalidateItem != null && msgInvalidateItem.m_itemType == ItemType.ASTEROID)
				{
					StateUtil.ResetVelocity(context);
					StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.ASTEROID, false);
					return true;
				}
				return true;
			}
			case 16385:
			{
				MsgHitDamageSucceed msgHitDamageSucceed = msg as MsgHitDamageSucceed;
				if (msgHitDamageSucceed != null)
				{
					StateUtil.CreateEffect(context, msgHitDamageSucceed.m_position, msgHitDamageSucceed.m_rotation, "ef_ph_aste_bom01", true);
				}
				return true;
			}
			case 12292:
				StateUtil.ResetVelocity(context);
				StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.ASTEROID, false);
				return true;
			default:
				return false;
			}
		}
	}
}
