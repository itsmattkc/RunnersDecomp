namespace App.Utility
{
	public struct Bitset32
	{
		private uint m_Value;

		public Bitset32(Bitset32 rhs)
		{
			m_Value = rhs.m_Value;
		}

		public Bitset32(uint x)
		{
			m_Value = x;
		}

		public override bool Equals(object o)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public bool Test(int pos)
		{
			return (m_Value & (1 << pos)) != 0;
		}

		public bool Any()
		{
			return m_Value != 0;
		}

		public bool None()
		{
			return !Any();
		}

		public int Count()
		{
			uint num = 1u;
			int num2 = 0;
			for (int num3 = 32; num3 > 0; num3--)
			{
				if ((m_Value & num) != 0)
				{
					num2++;
				}
				num <<= 1;
			}
			return num2;
		}

		public Bitset32 Set(int pos)
		{
			m_Value |= (uint)(1 << pos);
			return this;
		}

		public Bitset32 Set(int pos, bool flag)
		{
			if (flag)
			{
				m_Value |= (uint)(1 << pos);
			}
			else
			{
				m_Value &= (uint)(~(1 << pos));
			}
			return this;
		}

		public Bitset32 Set()
		{
			m_Value = uint.MaxValue;
			return this;
		}

		public Bitset32 Reset(int pos)
		{
			m_Value ^= (uint)(1 << pos);
			return this;
		}

		public Bitset32 Reset()
		{
			m_Value = 0u;
			return this;
		}

		public Bitset32 Flip(int pos)
		{
			m_Value ^= (uint)(1 << pos);
			return this;
		}

		public Bitset32 Flip()
		{
			m_Value = ~m_Value;
			return this;
		}

		public uint to_ulong()
		{
			return m_Value;
		}

		public static bool operator ==(Bitset32 lhs, Bitset32 rhs)
		{
			return lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(Bitset32 lhs, Bitset32 rhs)
		{
			return lhs.m_Value != rhs.m_Value;
		}

		public static Bitset32 operator &(Bitset32 lhs, Bitset32 rhs)
		{
			return new Bitset32(lhs.m_Value & rhs.m_Value);
		}

		public static Bitset32 operator |(Bitset32 lhs, Bitset32 rhs)
		{
			return new Bitset32(lhs.m_Value | rhs.m_Value);
		}
	}
}
