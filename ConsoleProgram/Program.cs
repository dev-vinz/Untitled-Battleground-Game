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
			DisplayWelcome();

			ProgramOption option = ReadOption();

			Console.WriteLine();

			switch (option)
			{
				case ProgramOption.Client:
					Battleground.StartClientOption();
					break;
				case ProgramOption.Server:
					Battleground.StartServerOption();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private static void DisplayWelcome()
		{
			Console.WriteLine();

			Console.WriteLine("\t\t───▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄───");
			Console.WriteLine("\t\t───█▒▒░░░░░░░░░▒▒█───");
			Console.WriteLine("\t\t────█░░█░░░░░█░░█────");
			Console.WriteLine("\t\t─▄▄──█░░░▀█▀░░░█──▄▄─");
			Console.WriteLine("\t\t█░░█─▀▄░░░░░░░▄▀─█░░█");
			Console.WriteLine("\t\t█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
			Console.WriteLine("\t\t█░░╦─╦╔╗╦─╔╗╔╗╔╦╗╔╗░░█");
			Console.WriteLine("\t\t█░░║║║╠─║─║─║║║║║╠─░░█");
			Console.WriteLine("\t\t█░░╚╩╝╚╝╚╝╚╝╚╝╩─╩╚╝░░█");
			Console.WriteLine("\t\t█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");

			Console.WriteLine();

			Console.WriteLine("\t* .NET Project");
			Console.WriteLine("\t* Professor : M. Stéphane Beurret");
			Console.WriteLine("\t* Authors :");
			Console.WriteLine("\t\t* Vincent Jeannin");
			Console.WriteLine("\t\t* Benjamin Mouchet");
			Console.WriteLine("\t\t* Guillaume Mouchet");

			Console.WriteLine();

			Console.Write("\t...Press enter to start the program...");
			Console.ReadLine();
		}

		private static ProgramOption ReadOption()
		{
			ConsoleKey key;

			do
			{
				Console.Clear();
				Console.WriteLine();

				Console.WriteLine("Homepage");
				Console.WriteLine();

				Console.WriteLine("\t* 1. Join an existing game");
				Console.WriteLine("\t* 2. Host a new game");

				Console.WriteLine();
				Console.Write("Select your option : ");

				key = Console.ReadKey().Key;
			} while (key != ConsoleKey.D1 && key != ConsoleKey.D2);

			return (ProgramOption)key;
		}
	}
}
