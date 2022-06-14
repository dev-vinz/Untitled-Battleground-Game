using Game;
using Game.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgram
{
	public static class Battleground
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public static void StartClientOption()
		{
			Console.Write("Please enter the IP adress of hosting server : ");

			string ip = Console.ReadLine();

			Player player = new Player(ip);

			try
			{
				player.Connect();
			}
			catch (Exception)
			{
				Console.WriteLine($"[ERROR] Could not connect to {ip}");
				return;
			}

			while (player.IsAlive)
			{
				player.PlayTurn();
			}
		}

		public static void StartServerOption()
		{
			Console.Write("How many players are you ? ");

			string strPlayers = Console.ReadLine();

			if (!int.TryParse(strPlayers, out int nbPlayers))
			{
				Console.WriteLine($"[ERROR] {strPlayers} is not a number");
				return;
			}

			Game.Game game = new Game.Game(nbPlayers);

			Console.WriteLine();

			Console.WriteLine(@"      _____      ");
			Console.WriteLine(@"     / / \ \     ");
			Console.WriteLine(@"    / /| |\ \    ");
			Console.WriteLine(@"   / / | | \ \   Save the IP address, it will disappear once you press enter");
			Console.WriteLine(@$"  / /  |_|  \ \  IP : {game.IP}");
			Console.WriteLine(@" /_/___(_)___\_\ ");
			Console.WriteLine(@" |______|______| ");

			Console.WriteLine();
			Console.Write("\t...Press enter to start the game...");

			Console.ReadLine();

			Thread thClient = new Thread((object data) =>
			{
				if (data is not Server server) return;

				Player player = new Player(server.IP);

				player.Connect();

				while (player.IsAlive)
				{
					player.PlayTurn();
				}
			});

			Thread thGame = new Thread((object data) =>
			{
				if (data is not Game.Game game) return;

				game.Start();

				//Console.WriteLine("Le jeu démarre");

				while (!game.IsEnded)
				{
					//Console.WriteLine("*** SERVER ***");
					game.ApplyTurn();
				}
			});

			thGame.Start(game);
			thClient.Start(game);
		}
	}
}
