﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities.Tier1
{
	public class MosquitoEventArgs : AbilityEventArgs
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly Side side;
		private readonly int targetPosition;
		private readonly int healthReduced;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Side Side => side;
		public int TargetPosition => targetPosition;
		public int HealthReduced => healthReduced;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public MosquitoEventArgs(Side side, int targetPosition, int healthReduced)
		{
			this.side = side;
			this.targetPosition = targetPosition;
			this.healthReduced = healthReduced;
		}
	}
}
