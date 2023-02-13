using System;
using System.Collections.Generic;

namespace App
{
	public class Random
	{
		public static void Shuffle<T>(List<T> list)
		{
			System.Random random = new System.Random();
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = random.Next(num + 1);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}

		public static void ShuffleInt(int[] array)
		{
			System.Random random = new System.Random();
			int num = array.Length;
			while (num > 1)
			{
				num--;
				int num2 = random.Next(num + 1);
				int num3 = array[num2];
				array[num2] = array[num];
				array[num] = num3;
			}
		}
	}
}
