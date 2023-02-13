using App.Utility;
using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/Level")]
public class PlayerInformation : MonoBehaviour
{
	public enum Flags
	{
		OnGround,
		Damaged,
		Dead,
		EnableCharaChange,
		Paraloop,
		LastChance,
		Combo,
		MovementUpdated
	}

	private const int MAX_NUM_RINGS = 99999;

	private string m_mainCharacterName = "Sonic";

	private string m_subCharacterName;

	private int m_mainCharacterID;

	private int m_subCharacterID = -1;

	private CharacterAttribute m_mainCharaAttribute;

	private CharacterAttribute m_subCharaAttribute;

	private TeamAttribute m_mainTeamAttribute;

	private TeamAttribute m_subTeamAttribute;

	private PlayingCharacterType m_playingCharacterType;

	private CharacterAttribute m_attribute;

	private TeamAttribute m_teamAttr;

	private float m_totalDistance;

	private int m_numRings;

	private int m_lostRings;

	private Bitset32 m_flags;

	private Vector3 m_velocity;

	private Vector3 m_horzVelocity;

	private Vector3 m_vertVelocity;

	private float m_defaultSpeed;

	private float m_frontspeed;

	private float m_distanceFromGround;

	private Vector3 m_GravityDir;

	private Vector3 m_upDirection;

	private Vector3 m_pathSideViewPos;

	private Vector3 m_pathSideViewNormal;

	private PhantomType m_phantomType = PhantomType.NONE;

	[SerializeField]
	private PlayerSpeed m_speedLevel;

	[SerializeField]
	private bool m_drawInfo;

	private Rect m_window;

	public float TotalDistance
	{
		get
		{
			return m_totalDistance;
		}
		set
		{
			m_totalDistance = value;
		}
	}

	public int NumRings
	{
		get
		{
			return m_numRings;
		}
	}

	public int NumLostRings
	{
		get
		{
			return m_lostRings;
		}
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return base.transform.rotation;
		}
	}

	public float FrontSpeed
	{
		get
		{
			return m_frontspeed;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return m_velocity;
		}
	}

	public Vector3 HorizonVelocity
	{
		get
		{
			return m_horzVelocity;
		}
	}

	public Vector3 VerticalVelocity
	{
		get
		{
			return m_vertVelocity;
		}
	}

	public float DefaultSpeed
	{
		get
		{
			return m_defaultSpeed;
		}
	}

	public float DistanceFromGround
	{
		get
		{
			return m_distanceFromGround;
		}
	}

	public Vector3 GravityDir
	{
		get
		{
			return m_GravityDir;
		}
	}

	public Vector3 UpDirection
	{
		get
		{
			return m_upDirection;
		}
	}

	public PlayerSpeed SpeedLevel
	{
		get
		{
			return m_speedLevel;
		}
	}

	public Vector3 SideViewPathPos
	{
		get
		{
			return m_pathSideViewPos;
		}
	}

	public Vector3 SideViewPathNormal
	{
		get
		{
			return m_pathSideViewNormal;
		}
	}

	public string MainCharacterName
	{
		get
		{
			return m_mainCharacterName;
		}
	}

	public string SubCharacterName
	{
		get
		{
			return m_subCharacterName;
		}
	}

	public int MainCharacterID
	{
		get
		{
			return m_mainCharacterID;
		}
	}

	public int SubCharacterID
	{
		get
		{
			return m_subCharacterID;
		}
	}

	public CharacterAttribute PlayerAttribute
	{
		get
		{
			return m_attribute;
		}
	}

	public TeamAttribute PlayerTeamAttribute
	{
		get
		{
			return m_teamAttr;
		}
	}

	public PhantomType PhantomType
	{
		get
		{
			return m_phantomType;
		}
	}

	public CharacterAttribute MainCharacterAttribute
	{
		get
		{
			return m_mainCharaAttribute;
		}
	}

	public CharacterAttribute SubCharacterAttribute
	{
		get
		{
			return m_subCharaAttribute;
		}
	}

	public TeamAttribute MainTeamAttribute
	{
		get
		{
			return m_mainTeamAttribute;
		}
	}

	public TeamAttribute SubTeamAttribute
	{
		get
		{
			return m_subTeamAttribute;
		}
	}

	public PlayingCharacterType PlayingCharaType
	{
		get
		{
			return m_playingCharacterType;
		}
	}

	private void Start()
	{
		m_flags.Set(0, true);
		m_GravityDir = new Vector3(0f, -1f, 0f);
		m_upDirection = -m_GravityDir;
		if (SaveDataManager.Instance != null)
		{
			CharaType mainChara = SaveDataManager.Instance.PlayerData.MainChara;
			m_attribute = CharaTypeUtil.GetCharacterAttribute(mainChara);
			m_teamAttr = CharaTypeUtil.GetTeamAttribute(mainChara);
		}
	}

	public bool IsDead()
	{
		return m_flags.Test(2);
	}

	public bool IsDamaged()
	{
		return m_flags.Test(1);
	}

	public bool IsOnGround()
	{
		return m_flags.Test(0);
	}

	public bool IsNotCharaChange()
	{
		return m_flags.Test(3);
	}

	public bool IsNowParaloop()
	{
		return m_flags.Test(4);
	}

	public bool IsNowLastChance()
	{
		return m_flags.Test(5);
	}

	public bool IsNowCombo()
	{
		return m_flags.Test(6);
	}

	public bool IsMovementUpdated()
	{
		return m_flags.Test(7);
	}

	private void OnUpSpeedLevel(MsgUpSpeedLevel msg)
	{
		SetSpeedLevel(msg.m_level);
	}

	public void SetTransform(Transform input)
	{
		base.transform.position = input.position;
		base.transform.rotation = input.rotation;
	}

	public void SetVelocity(Vector3 velocity)
	{
		m_velocity = velocity;
	}

	public void SetHorzAndVertVelocity(Vector3 horzVelocity, Vector3 vertVelocity)
	{
		m_horzVelocity = horzVelocity;
		m_vertVelocity = vertVelocity;
	}

	public void SetDefautlSpeed(float speed)
	{
		m_defaultSpeed = speed;
	}

	public void SetFrontSpeed(float speed)
	{
		m_frontspeed = speed;
	}

	public void SetNumRings(int numRing)
	{
		m_numRings = Mathf.Clamp(numRing, 0, 99999);
	}

	public void AddNumRings(int addRings)
	{
		SetNumRings(NumRings + addRings);
	}

	public void LostRings()
	{
		m_lostRings += NumRings;
		SetNumRings(0);
	}

	public void SetDistanceToGround(float distance)
	{
		m_distanceFromGround = distance;
	}

	public void SetGravityDirection(Vector3 dir)
	{
		m_GravityDir = dir;
	}

	public void SetUpDirection(Vector3 dir)
	{
		m_upDirection = dir;
	}

	public void AddTotalDistance(float nowDistance)
	{
		m_totalDistance += nowDistance;
	}

	public void SetSideViewPath(Vector3 pos, Vector3 normal)
	{
		m_pathSideViewPos = pos;
		m_pathSideViewNormal = normal;
	}

	private void SetSpeedLevel(PlayerSpeed level)
	{
		m_speedLevel = level;
	}

	public void SetPhantomType(PhantomType type)
	{
		m_phantomType = type;
	}

	public void SetPlayerAttribute(CharacterAttribute attr, TeamAttribute teamAttr, PlayingCharacterType playingType)
	{
		m_attribute = attr;
		m_teamAttr = teamAttr;
		m_playingCharacterType = playingType;
	}

	public void SetDebugPlayerAttribute(CharaType charaType)
	{
	}

	public void SetDead(bool value)
	{
		m_flags.Set(2, value);
	}

	public void SetDamaged(bool value)
	{
		m_flags.Set(1, value);
	}

	public void SetOnGround(bool value)
	{
		m_flags.Set(0, value);
	}

	public void SetEnableCharaChange(bool value)
	{
		m_flags.Set(3, value);
	}

	public void SetParaloop(bool value)
	{
		m_flags.Set(4, value);
	}

	public void SetLastChance(bool value)
	{
		m_flags.Set(5, value);
	}

	public void SetCombo(bool value)
	{
		m_flags.Set(6, value);
	}

	public void SetMovementUpdated(bool value)
	{
		m_flags.Set(7, value);
	}

	public void SetPlayerCharacter(int main, int sub)
	{
		if ((bool)CharacterDataNameInfo.Instance)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID((CharaType)main);
			CharacterDataNameInfo.Info dataByID2 = CharacterDataNameInfo.Instance.GetDataByID((CharaType)sub);
			if (dataByID != null)
			{
				m_mainCharacterName = dataByID.m_name;
				m_mainCharacterID = (int)dataByID.m_ID;
				m_mainCharaAttribute = dataByID.m_attribute;
				m_mainTeamAttribute = dataByID.m_teamAttribute;
			}
			if (dataByID2 != null)
			{
				m_subCharacterName = dataByID2.m_name;
				m_subCharacterID = (int)dataByID2.m_ID;
				m_subCharaAttribute = dataByID2.m_attribute;
				m_subTeamAttribute = dataByID2.m_teamAttribute;
			}
		}
	}
}
