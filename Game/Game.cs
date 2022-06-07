using Entities.Characters;
using Entities.Characters.Tier1;
<<<<<<< HEAD
using Entities.Characters.Tier2;
using Entities.Characters.Tier4;
using Entities.Characters.Tier5;
using Entities.Characters.Tier6;
=======
using Game.Phase;
>>>>>>> 6bd1e8e (Clone Methods)
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.ExtensionMethods;

#nullable enable

namespace Game
{
	public class Game : Server.Server
	{
		public static Character[] TIER1 = { new Ant(), new Beaver(), new Mosquito()};
		public static Character[] TIER2 = { new Crab(), new Shrimp(), new Toucan() };
		public static Character[] TIER3 = { new Blowfish(), new Horse(), new Luwak() };
		public static Character[] TIER4 = { new Giraffe(), new Otter(), new Ox() };
		public static Character[] TIER5 = { new Crocodile(), new Parrot(), new Porcupine() };
		public static Character[] TIER6 = { new Lucane(), new Penguin() };
		public static Character[] ALLPETS = TIER1.Concat(TIER2).Concat(TIER3).Concat(TIER4).Concat(TIER5).Concat(TIER6).ToArray(); 
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Character[][]? allCharacters;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Game(int nbClients) : base(nbClients)
		{
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void ApplyTurn()
		{
			allCharacters = Read();

			Console.WriteLine($"Got all characters !");

			/* Apply turn - Make battle */
			Battle[] battles = CreateBattles();

			/* Apply turn - Send results to clients */
			Send();
		}

		public new void Start()
		{
			base.Start();

			WaitForClients();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Battle[] CreateBattles()
		{
			if (allCharacters is null) throw new ArgumentNullException(nameof(allCharacters));

			Battle[] battles = new Battle[allCharacters.Length];

			for (int k = 0; k < allCharacters.Length; k++)
			{
				int rnd;

				do
				{
					rnd = new Random().Next(allCharacters.Length);
				} while (rnd == k);

<<<<<<< HEAD
=======
				Character[] playerOne = allCharacters[k];
				Character[] playerTwo = allCharacters[rnd];

				battles[k] = new Battle(playerOne, playerTwo);
			}

			return battles;
		}

>>>>>>> 6bd1e8e (Clone Methods)
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
