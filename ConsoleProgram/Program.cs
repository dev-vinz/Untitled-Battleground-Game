using Entities.Characters;
using Entities.Characters.Tier1;
using Game;
using Game.Server;
using System;
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

			p.Add(new Ant(), 0);
			p.Send(p.SerializedCharacters);
		}

		private static void RunServer()
		{
			Server server = new Server(1);

			Thread thClient = new Thread((object data) =>
			{
				if (data is not Server server) return;

				Player player = new Player(server.IP);

				player.Connect();

				player.Add(new Ant(), 0);

				while (true)
				{
					Console.WriteLine("*** PLAYER ***");
					player.Send(player.SerializedCharacters);
					Thread.Sleep(5000);
				}
			});

			Thread thServer = new Thread((object data) =>
			{
				if (data is not Server server) return;

				server.WaitForClients();

				Console.WriteLine("Le jeu démarre");

				while (true)
				{
					Console.WriteLine("*** SERVER ***");
					server.ApplyTurn();
				}
			});

			thServer.Start(server);
			thClient.Start(server);			
		}
	}
}
