using Entities.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Characters.Tier3
{
	public class Blowfish : Character
	{
		public const string NAME = "Blowfish";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected override string Emoji => "🐡";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Blowfish()
		{
			Name = NAME;
			Tier = 3;
			Damage = 3;
			Health = 5;
			Ability = Ability.Hurt;
		}

		public override Character Clone()
		{
			return new Blowfish
			{
				Ability = Ability,
				BattlefieldPosition = BattlefieldPosition,
				Damage = Damage,
				Health = Health,
				Level = Level,
				Name = Name,
				Tier = Tier,
			};
		}

		/// <summary>
		/// When hurt deals 2 damage to an random oponnent
		/// </summary>
		/// <returns></returns>
		public override AbilityEventArgs TriggerAbility()
		{
			// Randomize target position
			Random rnd = new Random();
			int target = rnd.Next(0, 4);

			return new HurtEventArgs
			{
				Side = Side.Opponent,
				TargetPosition = target,
				HealthReduced = 2 * Level,
				AttackGiven = 0,
				HealthGiven = 0,
			}; // It's to the combat to see if it's a valid position
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         ABSTRACT METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
	}
}
