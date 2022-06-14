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

		private int backupDamage;
		private int backupHealth;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected abstract string Emoji { get; }
		public abstract string Description { get; }

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
			protected set { level = value; }
		}

		public bool IsAlive => health > 0;

		public bool IsDead => !IsAlive;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected Character(int damage, int health)
		{
			this.damage = damage;
			this.health = health;
			
			backupDamage = damage;
			backupHealth = health;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void DisplayConsole()
		{
			const int PLACE_TAKEN = 10;

			(int oldLeft, int initialTop) = Console.GetCursorPosition();

			int position = PLACE_TAKEN * (4 - battlefieldPosition) + 1;

			Console.SetCursorPosition(position, initialTop);

			int nbSurroundName = (PLACE_TAKEN - name.Length) / 2;
			string strName = " ";

			for (int k = 0; k < nbSurroundName; k++) strName += " ";
			strName += name;
			for (int k = 0; k < nbSurroundName; k++) strName += " ";

			Console.Write(strName);

			Console.SetCursorPosition(position, initialTop + 1);

			Console.Write($"  {damage:#00}   {health:#00} ");

			Console.SetCursorPosition(position, initialTop + 2);

			int nbSurroundLevel = (PLACE_TAKEN - level) / 2;
			string strLevel = " ";

			for (int k = 0; k < nbSurroundLevel; k++) strLevel += " ";
			for (int k = 0; k < level; k++) strLevel += "*";
			for (int k = 0; k < nbSurroundLevel; k++) strLevel += " ";

			Console.Write(strLevel);

			Console.SetCursorPosition(position, initialTop + 3);

			Console.Write($"    ({battlefieldPosition + 1})   ");

			Console.SetCursorPosition(oldLeft, initialTop);
		}

		public bool LevelUp(int level = 1)
		{
			this.level += level;

			damage += level;
			health += level;

			return this.level % 3 == 0;
		}

		public void Restore()
		{
			damage = backupDamage;
			health = backupHealth;
		}

		public void Save()
		{
			backupDamage = damage;
			backupHealth = health;
		}

		/// <summary>
		/// Serialize the character to string JSON
		/// </summary>
		/// <returns>The class serialized</returns>
		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, Formatting.None);
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
