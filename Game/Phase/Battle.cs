using Entities.Abilities;
using Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Game.Phase
{
	public class Battle
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly int battleId;
		private Character[] playerOne;
		private Character[] playerTwo;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
		
		private bool IsEnded => playerOne.Any(c => c is not null && c.IsAlive) && playerOne.Any(c => c is not null && c.IsAlive);


		public int Id => battleId;
		public bool PlayerWon { get; private set; }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Battle(int battleId, Character[] playerOne, Character[] playerTwo)
		{
			this.battleId = battleId;
			this.playerOne = playerOne;
			this.playerTwo = playerTwo;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Start()
		{
			#region StartOfBattle Ability

			foreach (Character character in playerOne.Where(c => c.Ability == Ability.StartOfBattle))
			{
				OnStartOfBattle((StartOfBattleEventArgs)character.TriggerAbility());
			}

			foreach (Character character in playerTwo.Where(c => c.Ability == Ability.StartOfBattle))
			{
				OnStartOfBattle((StartOfBattleEventArgs)character.TriggerAbility());
			}

			#endregion
		}

		public void Run()
		{
			while (!IsEnded)
			{
				Character aliveOne = playerOne.First(c => c is not null && c.IsAlive);
				Character aliveTwo = playerTwo.First(c => c is not null && c.IsAlive);

				aliveOne.Health -= aliveTwo.Damage;
				aliveTwo.Health -= aliveOne.Damage;

				#region Hurt Ability

				if (aliveOne.IsAlive && aliveOne.Ability == Ability.Hurt)
				{
					OnHurt((HurtEventArgs)aliveOne.TriggerAbility());
				}

				if (aliveTwo.IsAlive && aliveTwo.Ability == Ability.Hurt)
				{
					OnHurt((HurtEventArgs)aliveTwo.TriggerAbility());
				}

				#endregion

				#region Faint Ability

				if (aliveOne.IsDead && aliveOne.Ability == Ability.Faint)
				{
					OnFaint((FaintEventArgs)aliveOne.TriggerAbility());
				}

				if (aliveTwo.IsDead && aliveTwo.Ability == Ability.Faint)
				{
					OnFaint((FaintEventArgs)aliveTwo.TriggerAbility());
				}

				#endregion
			}

			PlayerWon = playerOne.Any(c => c.IsAlive);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void OnFaint(FaintEventArgs e)
		{
			Character[] targetPlayer = e.Side == Side.Player ? playerOne : playerTwo;

			Character character = targetPlayer[e.TargetPosition];

			if (character is null || character.IsDead) return;

			character.Health -= e.HealthReduced;
			character.Health += e.HealthGiven;

			character.Damage += e.AttackGiven;
		}

		private void OnHurt(HurtEventArgs e)
		{
			Character[] targetPlayer = e.Side == Side.Player ? playerOne : playerTwo;

			Character character = targetPlayer[e.TargetPosition];

			if (character is null || character.IsDead) return;

			character.Health -= e.HealthReduced;
			character.Health += e.HealthGiven;

			character.Damage += e.AttackGiven;
		}

		private void OnStartOfBattle(StartOfBattleEventArgs e)
		{
			Character[] targetPlayer = e.Side == Side.Player ? playerOne : playerTwo;

			Character character = targetPlayer[e.TargetPosition];

			if (character is null || character.IsDead) return;

			character.Health -= e.HealthReduced;
			character.Health += e.HealthGiven;
		}

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
