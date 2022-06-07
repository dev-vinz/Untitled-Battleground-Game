using Entities.Abilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

		private int battlefieldPosition;

		private Ability ability;

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
			set { damage = value; }
		}

		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		public int BattlefieldPosition
		{
			get { return battlefieldPosition; }
			set { battlefieldPosition = value; }
		}

		public Ability Ability
		{
			get { return ability; }
			protected set { ability = value; }
		}

		public int Level
		{
			get { return level; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool LevelUp(int level = 1)
		{
			this.level += level;

			damage += level;
			health += level;

			return this.level % 3 == 0;
		}

		/// <summary>
		/// Serialize the character to string JSON
		/// </summary>
		/// <returns>The class serialized</returns>
		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, Formatting.None);
		}

		public bool IsDead()
        {
			return health <= 0;
        }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static T Parse<T>(string value) where T : Character
		{
			return JsonConvert.DeserializeObject<T>(value);
		}

		public static bool TryParse<T>(string value, out T character) where T : Character
		{
			character = Parse<T>(value);

			return character != null;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         ABSTRACT METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public abstract AbilityEventArgs TriggerAbility();

		public abstract Character Clone();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
	}
}
