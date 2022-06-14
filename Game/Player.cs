using Entities.Abilities;
using Entities.Characters;
using Entities.Characters.Tier1;
using Entities.Characters.Tier2;
using Entities.Characters.Tier3;
using Entities.Characters.Tier4;
using Entities.Characters.Tier5;
using Entities.Characters.Tier6;
using Game.Phase;
using Game.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.ExtensionMethods;

namespace Game
{
	public class Player : Client
	{
		public static readonly int NB_CHARACTERS = 5;
		private static readonly int MAX_HEALTH = 10;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private int id;
		private readonly Character[] characters;
		private int health;
		private int currentTurn;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool IsAlive => health > 0;
		public bool IsDead => !IsAlive;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		[IgnoreDataMember]
		public IReadOnlyCollection<Character> Characters
		{
			get { return characters; }
		}

		[IgnoreDataMember]
		public string Serialized
		{
			get
			{
				return JsonConvert.SerializeObject(this);
			}
		}

		public string SerializedCharacters
		{
			get
			{
				return JsonConvert.SerializeObject(characters.Where(c => c is not null).Select(c => c.Serialize()));
			}
			set
			{
				string[] tabStr = JsonConvert.DeserializeObject<string[]>(value);

				IEnumerable<JObject> objects = tabStr.Select(c => JObject.Parse(c));
				string[] characterNames = objects.Select(o => o.First?.First?.ToString()).GetNotNullValues() ?? Array.Empty<string>();

				if (characterNames.Length < 1) return;

				for (int k = 0; k < characterNames.Length; k++)
				{
					string name = characterNames[k];

					characters[k] = name switch
					{
						Ant.NAME => Character.Parse<Ant>(tabStr[k]),
						Beaver.NAME => Character.Parse<Beaver>(tabStr[k]),
						Mosquito.NAME => Character.Parse<Mosquito>(tabStr[k]),

						Crab.NAME => Character.Parse<Crab>(tabStr[k]),
						Shrimp.NAME => Character.Parse<Shrimp>(tabStr[k]),
						Toucan.NAME => Character.Parse<Toucan>(tabStr[k]),

						Blowfish.NAME => Character.Parse<Blowfish>(tabStr[k]),
						Horse.NAME => Character.Parse<Horse>(tabStr[k]),
						Luwak.NAME => Character.Parse<Luwak>(tabStr[k]),

						Giraffe.NAME => Character.Parse<Giraffe>(tabStr[k]),
						Otter.NAME => Character.Parse<Otter>(tabStr[k]),
						Ox.NAME => Character.Parse<Ox>(tabStr[k]),

						Crocodile.NAME => Character.Parse<Crocodile>(tabStr[k]),
						Parrot.NAME => Character.Parse<Parrot>(tabStr[k]),
						Porcupine.NAME => Character.Parse<Beaver>(tabStr[k]),

						Beetle.NAME => Character.Parse<Beetle>(tabStr[k]),
						Penguin.NAME => Character.Parse<Penguin>(tabStr[k]),

						_ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
					};
				}
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		[JsonConstructor]
		public Player(string ip = "localhost") : base(ip)
		{
			characters = new Character[NB_CHARACTERS];
			health = MAX_HEALTH;
			currentTurn = 0;
		}

		public Player(Player player) : base(player.IP)
		{
			id = player.id;
			characters = player.characters.Select(c => c?.Clone()).ToArray();
			health = player.health;
			currentTurn = player.currentTurn;
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

		public new void Connect()
		{
			id = base.Connect();
		}

		public void PlayTurn()
		{
			#region Restore old characters

			foreach (Character c in characters)
			{
				c?.Restore();
			}

			#endregion

			DisplayInformations();

			#region StartOfTurn Ability

			foreach (Character character in characters.Where(c => c is not null && c.Ability == Ability.StartOfTurn))
			{
				OnStartOfTurn((StartOfTurnEventArgs)character.TriggerAbility());
			}

			#endregion

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

			Send(Serialized);

			BattleHistoric[] battles = Read();
			BattleHistoric playerBattle = battles.First(b => b.Player.id == id);

			if (playerBattle.Result == BattleResult.Lost) health--;

			/* Notify the server if we're still alive */
			Send(IsAlive.ToString());

			DisplayBattles(battles);

			if (!IsAlive) DisplayEndScreen();

			currentTurn++;
		}

		public void Remove(Character character)
		{
			// Assumption, character is valid
			characters[character.BattlefieldPosition] = null;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void DisplayBattles(BattleHistoric[] battles)
		{
			Console.WriteLine();
			Console.WriteLine(">\t BATTLE RESULTS \t<");

			foreach (BattleHistoric battle in battles)
			{
				Console.WriteLine();
				Console.WriteLine($"*** Player {battle.Player.id + 1} {battle.Result.ToString().ToLower()} against Player {battle.Opponent.id + 1}");
				Console.WriteLine($" > Player {battle.Player.id + 1}'s health : {battle.Player.health} <");

				if (this == battle.Player)
				{
					foreach (string action in battle.Actions)
					{
						Console.WriteLine($"\t> {action}");
					}
				}
			}

			Console.WriteLine();
			Console.Write("End of turn, press enter to continue...");
			Console.ReadLine();
		}

		private void DisplayEndScreen()
		{
			Console.WriteLine();
			Console.WriteLine("* * * * * * * * * * * * * * *");
			Console.WriteLine("*         GAME OVER         *");
			Console.WriteLine("* * * * * * * * * * * * * * *");
			Console.WriteLine();
			Console.WriteLine($">> You lost in turn {currentTurn} <<");
		}

		private void DisplayInformations()
		{
			Console.Clear();

			Console.WriteLine($"\t-- Player {id + 1} \t HP : {health}/{MAX_HEALTH}\t Turn : {currentTurn} --");
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
			Console.WriteLine($"\t> {character.Description}");

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
				// Replace existant character
				else
				{
					Remove(characters[index]);
					Add(character, index);
					isAdded = true;
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

			Console.WriteLine($"[{e.InitialCharacter.Name}] {e.InitialCharacter.Description}");

			character.Health += e.HealthGiven;
			character.Damage += e.AttackGiven;

			Console.WriteLine($"\t> On {character.Name} | Health : {character.Health} | Attack : {character.Damage}");
			
			Console.WriteLine();
		}
		private new BattleHistoric[] Read()
		{
			string strData = base.Read();
			string[] tabStr = JsonConvert.DeserializeObject<string[]>(strData);

			return tabStr?.Select(b => JsonConvert.DeserializeObject<BattleHistoric>(b))?.ToArray();
		}

		private void UpdateCharacter()
		{
			int nbCharacters = characters.Count(c => c is not null);

			if (nbCharacters < 1) return;

			Character character = null;
			ConsoleKey key;

			do
			{
				Console.WriteLine();
				Console.Write($"Please choose a pet to upgrade [1 - {nbCharacters}] : ");

				key = Console.ReadKey().Key;
				int index = key - ConsoleKey.D1;

				if (index < characters.Length && index >= 0) character = characters[key - ConsoleKey.D1];
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
		|*                         OVERRIDE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			Player player = obj as Player;

			return player.id == id;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static bool operator ==(Player pOne, Player pTwo)
		{
			if (pOne is null && pTwo is null) return true;

			if (pOne is null)
			{
				return pTwo.Equals(pOne);
			}
			else
			{
				return pOne.Equals(pTwo);
			}
		}

		public static bool operator !=(Player pOne, Player pTwo)
		{
			return !(pOne == pTwo);
		}
	}
}
