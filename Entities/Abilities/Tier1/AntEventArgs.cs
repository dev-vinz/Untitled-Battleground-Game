using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities.Tier1
{
	public class AntEventArgs : AbilityEventArgs
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Side Side { get; set; }
		public int TargetPosition { get; set;}
		public int HealthGiven { get; set; }
		public int AttackGiven { get; set; }
	}
}
