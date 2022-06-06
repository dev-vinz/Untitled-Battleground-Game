using Entities.Characters;
using Entities.Characters.Tier1;
using Game;
using Game.Server;
using System;

namespace ConsoleProgram
{
	internal class Program
	{
		// Thread Client
		// Thread Server

		static void Main(string[] args)
		{
			/*
			Player p = new Player("157.26.104.10");
			p.Connect();

			p.Add(new Ant(), 0);
			p.Send(p.SerializedCharacters);
			*/

			/*
			string asm = "Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
			string nmspace = "Entities.Characters.Tier1.Ant";

			Type type = Type.GetType($"{nmspace}, {asm}");

			string str = p.SerializedCharacters;
			*/

			/* Test for Hosting */

			Server server = new Server(1);
			Player player = new Player(server.IP);

			player.Connect();
			server.WaitForClients();

			Console.WriteLine("Le jeu démarre");

			player.Add(new Ant(), 0);

			while (true)
			{
				Console.WriteLine("******");
				player.Send(player.SerializedCharacters);
				server.ApplyTurn();
			}

			Console.ReadLine();
		}
	}
}
