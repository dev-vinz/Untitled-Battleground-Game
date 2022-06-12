using Entities.Abilities;
using Entities.Characters;
using Entities.Characters.Tier1;
using Game.Phase;
using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

#nullable enable

namespace Game
{
	public class Player : Client
	{
		public static readonly int NB_CHARACTERS = 5;
		private static readonly int MAX_HEALTH = 10;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly Character[] characters;
		private int health;
		private int currentTurn;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool IsAlive => health > 0;
		public bool IsDead => !IsAlive;

		public IReadOnlyCollection<Character> Characters
		{
			get { return characters; }
		}

		public string SerializedCharacters
		{
			get
			{
				string[] tabSerialized = characters.Where(c => c != null).Select(c => c.Serialize()).ToArray();

				return JsonConvert.SerializeObject(tabSerialized);
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Player(string ip = "localhost") : base(ip)
		{
			characters = new Character[NB_CHARACTERS];
			health = MAX_HEALTH;
			currentTurn = 0;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Add(Character character, int position)
		{
			// Assumption, position is valid
			characters[position] = character;
			character.BattlefieldPosition = position;
		}

		public void PlayTurn()
		{
			#region Restore old characters

			foreach (Character c in characters)
			{
				c?.Restore();
			}

			#endregion

			#region StartOfTurn Ability

			foreach (Character character in characters.Where(c => c is not null && c.Ability == Ability.StartOfTurn))
			{
				OnStartOfTurn((StartOfTurnEventArgs)character.TriggerAbility());
			}

			#endregion

			DisplayInformations();

			Character newCharacter = Shop.GetNewCharacter(currentTurn);

			UpdateCharacter();
			HandleNewCharacter(newCharacter);

			#region EndOfTurn Ability
			#endregion

			#region Save characters

			foreach (Character c in characters)
			{
				c?.Save();
			}

			#endregion

			Console.WriteLine(">> End of turn... please wait for other players... <<");

			Send(SerializedCharacters);

			BattleHistoric battle = Read();

			if (battle.Result == BattleResult.Loose) health--;

			DisplayBattle(battle);

			currentTurn++;
		}

		public void Remove(Character character)
		{
			// Assumption, character is valid
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void DisplayBattle(BattleHistoric battle)
		{
			Console.WriteLine("Fin du tour...");
			Console.ReadLine();
		}

		private void DisplayInformations()
		{
			Console.Clear();

			Console.WriteLine($"\t-- HP : {health}/{MAX_HEALTH}\t Turn : {currentTurn} --");
			Console.WriteLine();

			foreach (Character character in characters)
			{
				if (character == null) continue;

				character.DisplayConsole();
			}

			Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 4);
			Console.WriteLine();
		}

		private void HandleNewCharacter(Character character)
		{
			ConsoleKey key;
			int nbCharacters = characters.Length;

			Console.WriteLine();
			Console.WriteLine($"You received a new pet : {character.Name} ({character.Damage}/{character.Health})");

			do
			{
				Console.Write("Do you want to keep it [Y/N] ? ");
				key = Console.ReadKey().Key;
				Console.WriteLine();
			} while (key != ConsoleKey.Y && key != ConsoleKey.N);

			if (key == ConsoleKey.Y)
			{
				do
				{
					Console.Write($"Where do you want to place it [1 - {nbCharacters}] ? ");

					key = Console.ReadKey().Key;
					Console.WriteLine();
				} while (key < ConsoleKey.D1 || key > (ConsoleKey)(nbCharacters + (int)ConsoleKey.D1));
				
				int index = key - ConsoleKey.D1;
				bool isAdded = false;

				// Add to the team
				if (characters[index] is null)
				{
					Add(character, index);
					isAdded = true;
				}
				// Update existant character
				else if (characters[index] is not null && characters[index].Name == character.Name)
				{
					characters[index].LevelUp();
				}
				// ERROR - Restart handler
				else
				{
					Console.WriteLine("[ERROR] There is already a character at this index");
					HandleNewCharacter(character);
					return;
				}

				DisplayInformations();

				if (isAdded)
				{
					Console.WriteLine($">>> Your brand new {character.Name} has been added ! <<<");
				}
				else
				{
					Console.WriteLine($"> Your {character.Name} has been leveled up ! <");
				}
			}
			else
			{
				Console.WriteLine("> This brand new character has not been added to your team <");
			}
		}

		private void OnStartOfTurn(StartOfTurnEventArgs e)
		{
			if (e.Side == Side.Opponent) return;

			int target = e.TargetPosition >= characters.Length ? characters.Length - 1 : e.TargetPosition;

			Character character = characters[target];

			if (character is null || character.IsDead) return;

			character.Health += e.HealthGiven;
			character.Damage += e.AttackGiven;
		}

		private void UpdateCharacter()
		{
			int nbCharacters = characters.Count(c => c is not null);

			Character? character = null;
			ConsoleKey key;

			do
			{
				Console.WriteLine();
				Console.Write($"Please choose a pet to upgrade [1 - {nbCharacters}] : ");

				key = Console.ReadKey().Key;
				int index = key - ConsoleKey.D1;

				if (index < characters.Length) character = characters[key - ConsoleKey.D1];
			} while (key < ConsoleKey.D1 || key > (ConsoleKey)(nbCharacters + (int)ConsoleKey.D1) || character?.Level >= 9);

			if (character is null) return;

			bool leveledUp = character.LevelUp();

			DisplayInformations();

			if (leveledUp)
			{
				Console.WriteLine($">>> Your {character.Name} is now level {character.Level % 3} ! <<<");
			}
			else
			{
				Console.WriteLine($"> Your {character.Name} is now level {(character.Level):#0.0} <");
			}
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
