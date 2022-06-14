using Entities.Characters;
using Entities.Characters.Tier1;
using Entities.Characters.Tier2;
using Entities.Characters.Tier3;
using Entities.Characters.Tier4;
using Entities.Characters.Tier5;
using Entities.Characters.Tier6;
using Game.Phase;
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
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Player[]? allPlayers;
		private readonly BattleHistoric[] tabBattles;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private string SerializedBattles
		{
			get
			{
				return JsonConvert.SerializeObject(tabBattles.Select(b => b.Serialized).ToArray());
			}
		}

		public bool IsEnded => NbClients < 2;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                            CONSTRUCTORS                           *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Game(int nbClients) : base(nbClients)
		{
			tabBattles = new BattleHistoric[nbClients];
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void ApplyTurn()
		{
			/* Get all players still in the game */
			allPlayers = Read();

			//Console.WriteLine($"Got all characters !");

			/* Make battles */
			HandleBattles();

			/* Send results to clients */
			Send(SerializedBattles);

			/* Check who's dead */
			HandlePlayerStates();
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
			if (allPlayers is null) throw new ArgumentNullException(nameof(allPlayers));

			Random random = new Random();
			Battle[] battles = new Battle[allPlayers.Length];

			for (int k = 0; k < allPlayers.Length; k++)
			{
				int rnd;

				do
				{
					rnd = random.Next(allPlayers.Length);
				} while (rnd == k && allPlayers.Length > 1);

				Player playerOne = new Player(allPlayers[k]);
				Player playerTwo = new Player(allPlayers[rnd]);

				battles[k] = new Battle(playerOne, playerTwo);
			}

			return battles;
		}

		private void HandleBattles()
		{
			Battle[] battles = CreateBattles();
			Thread[] threads = new Thread[battles.Length];

			for (int k = 0; k < battles.Length; k++)
			{
				threads[k] = new Thread((object? data) =>
				{
					if (data is not Battle battle) return;

					battle.Start();

					string[] actions = battle.Run(out BattleResult playerResult);
					tabBattles[battle.Player.Id] = new BattleHistoric(playerResult, actions, battle.Player, battle.Opponent);

					if (playerResult == BattleResult.Lost) battle.Player.Health--;
				});

				threads[k].Start(battles[k]);
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}
		}

		private void HandlePlayerStates()
		{
			Thread[] threads = new Thread[NbClients];

			for (int k = 0; k < threads.Length; k++)
			{
				threads[k] = new Thread((object? data) =>
				{
					if (data is not int index) return;

					string pAlive = Read(index);
					bool isAlive = bool.Parse(pAlive);

					if (!isAlive) RemoveClient(index);
				});

				threads[k].Start(k);
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}
		}

		private Player[] Read()
		{
			/* Wait for players informations */
			Thread[] threads = new Thread[NbClients];
			Player[] tabPlayers = new Player[NbClients];

			for (int i = 0; i < NbClients; i++)
			{
				threads[i] = new Thread((object? oIndex) =>
				{
					if (oIndex is null) return;

					int index = (int)oIndex;

					string strPlayer = Read(index);

					Player? player = JsonConvert.DeserializeObject<Player>(strPlayer);

					if (player is null) return;

					tabPlayers[index] = player;
				});

				threads[i].Start(i);
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}

			return tabPlayers;
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
