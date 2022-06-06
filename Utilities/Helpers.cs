using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public static class Helpers
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static int ClearConsoleBuffer()
		{
			int nb = 0;

			while (Console.KeyAvailable)
			{
				nb++;
				Console.ReadKey(false);
			}

			return nb;
		}
	}
}
