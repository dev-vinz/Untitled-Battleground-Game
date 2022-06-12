using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Entities.Characters;
using Newtonsoft.Json;
using Entities.Characters.Tier1;
using Newtonsoft.Json.Linq;
using Utilities.ExtensionMethods;
using Entities.Characters.Tier2;
using Entities.Characters.Tier3;
using Entities.Characters.Tier4;
using Entities.Characters.Tier5;
using Entities.Characters.Tier6;

#nullable enable

namespace Game.Server
{
	public class Server
	{
		public static int TCP_PORT = 8001;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly string ip;
		private readonly int nbClients;
		private readonly Socket[] clients;
		private readonly TcpListener listener;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string IP
		{
			get { return ip; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <see cref="https://stackoverflow.com/questions/19387086/how-to-set-up-tcplistener-to-always-listen-and-accept-multiple-connections"/>
		/// <seealso cref="https://stackoverflow.com/questions/6803073/get-local-ip-address"/>
		public Server(int nbClients)
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			string? ip = host.AddressList.Last(i => i.AddressFamily == AddressFamily.InterNetwork)?.ToString();

			if (ip != null)
			{
				this.ip = ip;
				this.nbClients = nbClients;
				this.clients = new Socket[this.nbClients];
				this.listener = new TcpListener(IPAddress.Parse(ip), TCP_PORT);
			}
			else
			{
				throw new ArgumentNullException(nameof(ip));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Stop()
		{
			foreach (Socket socket in clients)
			{
				socket.Close();
			}

			listener.Stop();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected Character[][] Read()
		{
			/* Wait for players comp */
			Thread[] threads = new Thread[nbClients];
			Character[][] tabCharacters = new Character[nbClients][];

			for (int i = 0; i < nbClients; i++)
			{
				threads[i] = new Thread((object? oIndex) =>
				{
					if (oIndex is null) return;

					int index = (int)oIndex;

					byte[] tabResponse = new byte[1000];
					Socket socket = clients[index];

					int nbBytes = socket.Receive(tabResponse);
					string strCharacters = "";

					/* Transform bytes to JSON string */
					for (int k = 0; k < nbBytes; k++)
					{
						strCharacters += Convert.ToChar(tabResponse[k]);
					}

					string[]? tabStr = JsonConvert.DeserializeObject<string[]>(strCharacters);

					IEnumerable<JObject>? objects = tabStr?.Select(c => JObject.Parse(c));
					string?[] characterNames = objects?.Select(o => o.First?.First?.ToString())?.GetNotNullValues() ?? Array.Empty<string>();

					if (characterNames.Length < 1) return;

					tabCharacters[index] = new Character[characterNames.Length];

					for (int k = 0; k < characterNames.Length; k++)
					{
						string? name = characterNames[k];

						tabCharacters[index][k] = name switch
						{
							Ant.NAME => Character.Parse<Ant>(tabStr?[k]),
							Beaver.NAME => Character.Parse<Beaver>(tabStr?[k]),
							Mosquito.NAME => Character.Parse<Mosquito>(tabStr?[k]),

							Crab.NAME => Character.Parse<Crab>(tabStr?[k]),
							Shrimp.NAME => Character.Parse<Shrimp>(tabStr?[k]),
							Toucan.NAME => Character.Parse<Toucan>(tabStr?[k]),

							Blowfish.NAME => Character.Parse<Blowfish>(tabStr?[k]),
							Horse.NAME => Character.Parse<Horse>(tabStr?[k]),
							Luwak.NAME => Character.Parse<Luwak>(tabStr?[k]),

							Giraffe.NAME => Character.Parse<Giraffe>(tabStr?[k]),
							Otter.NAME => Character.Parse<Otter>(tabStr?[k]),
							Ox.NAME => Character.Parse<Ox>(tabStr?[k]),

							Crocodile.NAME => Character.Parse<Crocodile>(tabStr?[k]),
							Parrot.NAME => Character.Parse<Parrot>(tabStr?[k]),
							Porcupine.NAME => Character.Parse<Beaver>(tabStr?[k]),

							Beetle.NAME => Character.Parse<Beetle>(tabStr?[k]),
							Penguin.NAME => Character.Parse<Penguin>(tabStr?[k]),

							_ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
						};
					}
				});

				threads[i].Start(i);
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}

			return tabCharacters;
		}

		protected void Send()
		{
			foreach (Socket socket in clients)
			{
				ASCIIEncoding asen = new ASCIIEncoding();
				socket.Send(asen.GetBytes($"*** SERVER *** : Just sent battle result to {((IPEndPoint?)socket.RemoteEndPoint)?.Address}"));
			}
		}

		protected void Start()
		{
			try
			{
				listener.Start();

				//Console.WriteLine($"The server is running at port {TCP_PORT}...");
				//Console.WriteLine("The local End point is  :\t" + listener.LocalEndpoint);
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected void WaitForClients()
		{
			int clientsConnected = 0;

			//Console.WriteLine("Attente des joueurs...");

			while (clientsConnected < nbClients)
			{
				Socket socket = listener.AcceptSocket();
				clients[clientsConnected] = socket;

				clientsConnected++;

				//Console.WriteLine($"Joueur {clientsConnected} connecté à la partie");
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
