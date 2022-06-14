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

		private readonly List<string> actions;

		private readonly Player playerOne;
		private readonly Player playerTwo;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private bool IsEnded => (playerOne.Characters.All(c => c is null || c.IsDead) && playerTwo.Characters.All(c => c is null || c.IsDead)) ||
								(playerOne.Characters.Any(c => c is not null && c.IsAlive) && playerTwo.Characters.All(c => c is null || c.IsDead)) ||
								(playerOne.Characters.All(c => c is null || c.IsDead) && playerTwo.Characters.Any(c => c is not null && c.IsAlive));

		public Player Player => playerOne;
		public Player Opponent => playerTwo;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Battle(Player playerOne, Player playerTwo)
		{
			actions = new List<string>();

			this.playerOne = playerOne;
			this.playerTwo = playerTwo;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Start()
		{
			actions.Add("Coucou maggle");

			#region StartOfBattle Ability

			foreach (Character character in playerOne.Characters.Where(c => c is not null && c.Ability == Ability.StartOfBattle))
			{
				OnStartOfBattle((StartOfBattleEventArgs)character.TriggerAbility());
			}

			foreach (Character character in playerTwo.Characters.Where(c => c is not null && c.Ability == Ability.StartOfBattle))
			{
				OnStartOfBattle((StartOfBattleEventArgs)character.TriggerAbility());
			}

			#endregion
		}

		public string[] Run(out BattleResult playerResult)
		{
			while (!IsEnded)
			{
				Character aliveOne = playerOne.Characters.First(c => c is not null && c.IsAlive);
				Character aliveTwo = playerTwo.Characters.First(c => c is not null && c.IsAlive);

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

			playerResult = playerOne.Characters.Any(c => c is not null && c.IsAlive) && playerTwo.Characters.All(c => c is null || c.IsDead) ? BattleResult.Won :
							playerOne.Characters.All(c => c is null || c.IsDead) && playerTwo.Characters.Any(c => c is not null && c.IsAlive) ? BattleResult.Lost :
							BattleResult.Tied;

			return actions.ToArray();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void OnFaint(FaintEventArgs e)
		{
			IReadOnlyCollection<Character> targetPlayer = e.Side == Side.Player ? playerOne.Characters : playerTwo.Characters;

			int target = e.TargetPosition >= targetPlayer.Count ? targetPlayer.Count - 1 : e.TargetPosition;

			Character character = targetPlayer.ElementAt(target);

			if (character is null || character.IsDead) return;

			character.Health -= e.HealthReduced;
			character.Health += e.HealthGiven;

			character.Damage += e.AttackGiven;
		}

		private void OnHurt(HurtEventArgs e)
		{
			IReadOnlyCollection<Character> targetPlayer = e.Side == Side.Player ? playerOne.Characters : playerTwo.Characters;

			int target = e.TargetPosition >= targetPlayer.Count ? targetPlayer.Count - 1 : e.TargetPosition;

			Character character = targetPlayer.ElementAt(target);

			if (character is null || character.IsDead) return;

			character.Health -= e.HealthReduced;
			character.Health += e.HealthGiven;

			character.Damage += e.AttackGiven;
		}

		private void OnStartOfBattle(StartOfBattleEventArgs e)
		{
			IReadOnlyCollection<Character> targetPlayer = e.Side == Side.Player ? playerOne.Characters : playerTwo.Characters;

			int target = e.TargetPosition >= targetPlayer.Count ? targetPlayer.Count - 1 : e.TargetPosition;

			Character character = targetPlayer.ElementAt(target);

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
