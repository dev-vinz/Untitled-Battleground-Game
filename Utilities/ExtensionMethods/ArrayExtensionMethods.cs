using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Utilities.ExtensionMethods
{
	public static class ArrayExtensionMethods
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static int GetNotNullLength<T>(this IEnumerable<T?> array)
		{
			return array.GetNotNullValues().Length;
		}

		public static T[] GetNotNullValues<T>(this IEnumerable<T?> array)
		{
			List<T> list = new List<T>();

			foreach (T? item in array)
			{
				if (item is not null)
				{
					list.Add(item);
				}
			}

			return list.ToArray();
		}
	}
}
