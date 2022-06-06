using Entities.Characters;
using Entities.Characters.Tier1;
using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		private readonly Game game;

		private int health;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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

		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Add(Character character, int position)
		{
			// Assumption, position is valid
			characters[position] = character;
		}

		public void EndTurn()
		{
			UpdateCharactersAbilities();
		}

		public bool IsDead()
		{
			return health <= 0;
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

		private void UpdateCharactersAbilities()
		{

		}

		public void PlayGame()
        {
            while(!IsDead())
            {
				Console.WriteLine($"You have {health}HP left");
				UpdateTeam();
				Send(SerializedCharacters);

				health--;
            }
        }

		private void UpdateTeam()
        {
			ObtainNewPet();
			LevelUpPet();
        }

		private void ObtainNewPet()
        {
            Ant ant = new Ant();
			Console.WriteLine($"You received a new pet:{ant.Name}. Do you want to keep it ? [Y/N]");
			DisplayPets();
			if (Console.ReadKey().Key == ConsoleKey.Y)
            {
				Console.WriteLine($"Where do you want to place it ? [0-{NB_CHARACTERS-1}/N]");
				ConsoleKey key = Console.ReadKey().Key;
				while(key < ConsoleKey.D0 || key > (ConsoleKey)(NB_CHARACTERS + (int)ConsoleKey.D0))
                {
					Console.WriteLine("Incorrect index");
					key = Console.ReadKey().Key;
                }

                if (characters[(key - ConsoleKey.D0)] is not null && characters[(key - ConsoleKey.D0)].Name == ant.Name)
                {
					characters[(key - ConsoleKey.D0)].LevelUp();
					Console.WriteLine($"You upgraded your {ant.Name}");	

				}
				else
                {
					characters[(key - ConsoleKey.D0)] = ant;
					Console.WriteLine($"{ant.Name} was added to your team");
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
			foreach(Character c in characters)
            {
				if(c is null) { i++; continue; }
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
