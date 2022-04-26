using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Characters
{
	public abstract class Character
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private string name;

		private int tier;
		private int damage;
		private int health;

		// TODO : Ability

		private int level = 1;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string Name
		{
			get { return name; }
			protected set { name = value; }
		}

		public int Tier
		{
			get { return tier; }
			protected set { tier = value; }
		}

		public int Damage
		{
			get { return damage; }
			protected set { damage = value; }
		}

		public int Health
		{
			get { return health; }
			protected set { health = value; }
		}

		public int Level
		{
			get { return level; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void LevelUp(int level = 1)
		{
			this.level += level;
			damage++;
			health++;
		}
	}
}
