using UnityEngine;

internal class CircularBuffer<T>
{
	private T[] m_Buffer;

	private int m_Cursor;

	private int m_Head;

	private int m_Tail;

	private int m_Size;

	public int Capacity
	{
		get
		{
			return m_Buffer.Length;
		}
	}

	public int Size
	{
		get
		{
			return m_Size;
		}
	}

	public int Head
	{
		get
		{
			return m_Head;
		}
	}

	public int Tail
	{
		get
		{
			return m_Tail;
		}
	}

	public CircularBuffer(int capacity)
	{
		m_Buffer = new T[capacity];
	}

	public T GetAt(int index)
	{
		return m_Buffer[index];
	}

	public void Add(T item)
	{
		m_Tail = m_Cursor;
		m_Buffer[m_Cursor] = item;
		m_Cursor = (m_Cursor + 1) % Capacity;
		m_Head = ((m_Size >= Capacity) ? m_Cursor : 0);
		m_Size = Mathf.Min(m_Size + 1, Capacity);
	}
}
