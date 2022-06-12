using Entities.Abilities;
using Entities.Characters;
using Entities.Characters.Tier1;
using Game.Phase;
using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

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

			character.LevelUp(8);
		}

		public void EndTurn()
		{
			
		}

		public void PlayTurn()
		{
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

			Send(SerializedCharacters);

			BattleHistoric battle = Read();

			if (battle.Result == BattleResult.Loose) health--;

			DisplayBattle(battle);

			currentTurn++;
		}

		public void PlayGame()
		{
			int turn = 1;

			while (IsAlive)
			{
				Helpers.ClearConsoleBuffer();
				Console.WriteLine($"You have {health}HP left");

				UpdateTeam(turn);
				Send(SerializedCharacters);

				bool won = Read2();

				if (!won) health--;
				turn++;
			}
		}

		public void Remove(Character character)
		{
			// Assumption, character is valid
		}

		public void StartTurn()
		{

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
			Console.WriteLine($"\t-- HP : {health}/{MAX_HEALTH}\t Turn : {currentTurn} --");
			Console.WriteLine();

			foreach (Character character in characters)
			{
				if (character == null) continue;

				character.DisplayConsole();
			}

			Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 4);
			Console.WriteLine();

			Console.WriteLine("Pressez enter pour continuer...");
			Console.ReadLine();
		}

		private void HandleNewCharacter(Character character)
		{

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

		}

		private void UpdateTeam(int turn)
		{
			ObtainNewPet(turn);
			LevelUpPet();
		}

		private void ObtainNewPet(int turn)
		{
			Character pet = Shop.GetNewCharacter(turn);
			Console.WriteLine($"You received a new pet:{pet.Name}. Do you want to keep it ? [Y/N]");
			DisplayPets();
			if (Console.ReadKey().Key == ConsoleKey.Y)
			{
				Console.WriteLine($"Where do you want to place it ? [0-{NB_CHARACTERS - 1}/N]");
				ConsoleKey key = Console.ReadKey().Key;
				while (key < ConsoleKey.D0 || key > (ConsoleKey)(NB_CHARACTERS + (int)ConsoleKey.D0))
				{
					Console.WriteLine("Incorrect index");
					key = Console.ReadKey().Key;
				}

				if (characters[(key - ConsoleKey.D0)] is not null && characters[(key - ConsoleKey.D0)].Name == pet.Name)
				{
					characters[(key - ConsoleKey.D0)].LevelUp();
					Console.WriteLine($"You upgraded your {pet.Name}");

				}
				else
				{
					characters[(key - ConsoleKey.D0)] = pet;
					Console.WriteLine($"{pet.Name} was added to your team");
					DisplayPets();
				}
			}
		}

		private void LevelUpPet()
		{
			//Check character level	
			Console.WriteLine($"Please choose a pet to upgrade [[0-{NB_CHARACTERS - 1}]");
			ConsoleKey key = Console.ReadKey().Key;
			if (key >= ConsoleKey.D0 && key <= (ConsoleKey)(NB_CHARACTERS + (int)ConsoleKey.D0))
			{
				if (characters[key - ConsoleKey.D0] is not null)
				{
					characters[key - ConsoleKey.D0].LevelUp();
				}
			}
		}

		private void DisplayPets()
		{
			int i = 0;
			foreach (Character c in characters)
			{
				if (c is null) { i++; continue; }
				Console.Write($"{c.Name} lvl{c.Level} [{i++}]");
			}
			Console.WriteLine();
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
