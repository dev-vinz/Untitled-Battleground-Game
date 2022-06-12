using Entities.Characters;
using Entities.Characters.Tier1;
using Game;
using Game.Server;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleProgram
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//RunClient();
			RunServer();

			Console.ReadLine();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static void RunClient()
		{
			Player p = new Player("157.26.104.10");
			p.Connect();

			p.PlayGame();
		}

		private static void RunServer()
		{
			Game.Game game = new Game.Game(1);

			Thread thClient = new Thread((object data) =>
			{
				if (data is not Server server) return;

				Player player = new Player(server.IP);

				player.Connect();

				player.Add(new Ant(), 0);
				player.Add(new Mosquito(), 1);

				player.PlayTurn();
			});

			Thread thGame = new Thread((object data) =>
			{
				if (data is not Game.Game game) return;

				game.Start();

				//Console.WriteLine("Le jeu démarre");

				while (true)
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
