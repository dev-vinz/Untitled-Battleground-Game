using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities.Tier2
{
	public class CrabEventArgs : AbilityEventArgs
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly Side side;
		private readonly int targetPosition;
		private readonly int healthGiven;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Side Side => side;
		public int TargetPosition => targetPosition;
		public int HealthGiven => healthGiven;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public CrabEventArgs(Side side, int targetPosition, int healthGiven)
		{
			this.side = side;
			this.targetPosition = targetPosition;
			this.healthGiven = healthGiven;
		}
	}
}
