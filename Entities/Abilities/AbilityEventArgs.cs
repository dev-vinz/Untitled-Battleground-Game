using Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities
{
	public class AbilityEventArgs
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
		
		public Side Side { get; set; }
		public int TargetPosition { get; set; }
		public Character InitialCharacter { get; set; }
	}
}
