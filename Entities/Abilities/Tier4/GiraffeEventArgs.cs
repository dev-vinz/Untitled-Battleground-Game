﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities.Tier4
{
	public class GiraffeEventArgs : AbilityEventArgs
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly Side side;
		private readonly int targetPosition;
		private readonly int healthGiven;
		private readonly int attackGiven;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Side Side => side;
		public int TargetPosition => targetPosition;
		public int HealthGiven => healthGiven;
		public int AttackGiven => attackGiven;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public GiraffeEventArgs(Side side, int targetPosition, int healthGiven, int attackGiven)
		{
			this.side = side;
			this.targetPosition = targetPosition;
			this.healthGiven = healthGiven;
			this.attackGiven = attackGiven;
		}
	}
}
